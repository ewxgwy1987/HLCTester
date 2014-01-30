using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using System.Threading;

using PALS.Telegrams;
using PALS.Utilities;

namespace BHS.PLCSimulator
{
    /// <summary>
    /// Application layer message handler of Gateway Service application. The incoming application 
    /// message from both Gateway-Internal and Gateway-External communication chains will be forwarded 
    /// to this class by the OnReceived() event fired by most top class, GW2InternalSessionForwarder
    /// and GW2ExternalSessionForwarder, in both chains. 
    /// <para>
    /// If messages come from Gateway-Externall chain, then this MessageHandler class will forward them
    /// to Gateway-Internal chain for sending out to BHS Engine Service application. As per the 
    /// <![CDATA[TCPServer&Client]]> protocol design, all messages received by Gateway-External chain
    /// and reach this MessageHandler class shall be forward to Engine Service application for business
    /// logic handling.
    /// </para>
    /// <para>
    /// If messages come from Gateway-Internal chain, and this message needs to be sent to external
    /// devices (e.g. PLCs, CCTV server, BIS/FIS servers, etc.), then this MessageHandler class will 
    /// forward them to Gateway-Internal chain for sending out to external devices. As per the 
    /// <![CDATA[TCPServer&Client]]> protocol design, not all messages received by Gateway-Internal
    /// chain and reach this MessageHandler class shall be forward to external devices. For example:
    /// ServiceStatusRequest message shall be handled by this MessageHandler class itself, instead
    /// of forward to external devices.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// There is a class level internal buffer will be created when class is instantiated for storing
    /// incoming messages. All incoming messages will be stored in this internal buffer. One seperate
    /// thread is running in background to retrieve the incoming messages one by one from this buffer,
    /// and handle it according to the business rules.
    /// </para>
    /// <para>There is no outgoing message queue was implemented in this layer class. It because all 
    /// acknowledge required outgoing messages will be buffered by bottom ACK class. Such message won't 
    /// be lost in case of the sending process failure or it is not acknowledged. But for those acknowledge 
    /// unrequired message, they are not buffered in any layer. Such message will be sent and forget. 
    /// Hence, if the connection is broken at the time of Send() method is invoked, this acknowledge 
    /// unrequired message will be lost. Hence, all critical messages should be defined as the 
    /// acknowledge required message in the interface protocol design.
    /// </para>
    /// <para>
    /// As per the Gateway Service application design, this gateway application is acted as the TCP client
    /// of both connections to BHS internal Engine Service applications and to external devices. Hence, 
    /// the both Engine Service application and external devices are TCP server and always in the 
    /// listening mode to wait for the TCP connection request from Gateway service application.
    /// </para>
    /// <para>
    /// Gateway-Internal chain has the higher priority than Gateway-External chain. The connection 
    /// (Application layer and TCP Socket layer) between gateway service to external devices shall be 
    /// opened only after connection (both application and socket layers) between gateway service to 
    /// Engine Service application is opened. If gateway to Engine connection is interrupted, the 
    /// all connections of gateway to external devices have to be closed immediately. This design is 
    /// for gateway service application, which is running on the seperate SAC-COM server (B), can has the 
    /// chance to open the connection to external devices, when the Engine Service on the SAC-COM
    /// server (A) is dead but the gateway service on server (A) is still working. Otherwise, the 
    /// gateway service running on server (A) will continue hold the connection to external devices, 
    /// even its associated engine service has been dead.
    /// </para>
    /// <para>
    /// Upon channel conenction is opened, closed, or message is received, MessageHandler class will
    /// raise 
    /// OnConnected(object sender, MessageEventArgs e), 
    /// OnDisconnected(object sender, MessageEventArgs e),
    /// and OnReceived(object sender, MessageEventArgs e) 
    /// events to wrapper class. In the event MessageEventArgs type parameter e, the ChainName, 
    /// ChannelName, OpenedChannelCount, and Message will be forwarded to wrapper class.
    /// ChainName   - The name of communication Chain in which the event is fired. One Chain could have
    ///               multiple channel connections.
    /// ChannelName - The name of communication Channel where the connection is opened/closed, or 
    ///               message is received from. One Chain could have multiple channel connections.
    /// OpenedChannelCount - The number of current opened channel connections.
    /// Message     - The received message.
    /// </para>
    /// </remarks>
    public class MessageHandler
    {
        #region Class fields and Properties Declaration

        private const string SOURCE_PLC2GW = "PLC2GW";
        private const int THREAD_INTERVAL = 10; // 10 millisecond

        // The name of current class
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //public Queue _incomingQueue;
        private Queue _incomingQueue;
        private Queue _syncdIncomingQueue;
        private Thread _handlingThread;

        private bool _isToGatewayChainOpened;

        private long _threadCounter;
        private long _noOfMsg2GW;

        private PALS.Diagnostics.ClassStatus _perfMonitor;

        /// <summary>
        /// Message forwarder of GW2External channel for forwarding incoming message to centrialized message handler.
        /// </summary>
        private PLC2GWSessionForward _forwarderPLC2GW;

        // Upon ConnectionOpened() method is invoked by bottom chain class,
        // the ChannelName will be stored into this ArrayList.
        // Once the bottom protocol layer connection is closed, its ChannelName
        // will be removed from this ArrayList accordingly.
        private ArrayList _channelListPLC2GW, _syncdChannelListPLC2GW;

        /// <summary>
        /// Event will be raised when message is received.
        /// </summary>
        public event EventHandler<MessageEventArgs> OnReceived;
        /// <summary>
        /// Event will be raised when specific channel connection of Gateway-External device chain is opened.
        /// </summary>
        public event EventHandler<MessageEventArgs> OnConnected;
        /// <summary>
        /// Event will be raised when specific channel connection of Gateway-External device chain is closed.
        /// </summary>
        public event EventHandler<MessageEventArgs> OnDisconnected;

        /// <summary>
        /// null
        /// </summary>
        public string ObjectID { get; set; }
        /// <summary>
        /// Property, object of MessageHandlerParameters class.
        /// </summary>
        public MessageHandlerParameters ClassParameters { get; set; }

        /// <summary>
        /// null
        /// </summary>
        public PALS.Diagnostics.ClassStatus PerfMonitor
        {
            get
            {
                try
                {
                    _perfMonitor.ObjectID = ObjectID;
                    PerfCounterRefresh();
                    return _perfMonitor;
                }
                catch (Exception ex)
                {
                    string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

                    if (_logger.IsErrorEnabled)
                        _logger.Error("Exception occurred! <" + thisMethod + ">", ex);

                    return null;
                }
            }
        }

        /// <summary>
        /// The Hashtable that contains the ClassStatus object of current class and all of its instance of sub classes.
        /// </summary> 
        public ArrayList PerfMonitorList { get; set; }

        #endregion

        #region Class Constructor, Dispose, & Destructor

        /// <summary>
        /// Class constructer.
        /// </summary>
        public MessageHandler(PALS.Common.IParameters param,
                    PLC2GWSessionForward forwarderPLC2GW)
        {
            if (param == null)
                throw new Exception("Constractor parameter can not be null! Creating class " + _className +
                    " object failed! <BHS.Gateway.Messages.Handlers.MessageHandler.Constructor()>");

            if (forwarderPLC2GW == null)
                throw new Exception("Constractor parameter can not be null! Creating class " + _className +
                    " object failed! <BHS.Gateway.Messages.Handlers.MessageHandler.Constructor()>");

            ClassParameters = (MessageHandlerParameters)param;
            _forwarderPLC2GW = forwarderPLC2GW;

            // Call Init() method to perform class initialization tasks.
            Init();
        }

        /// <summary>
        /// Class destructer.
        /// </summary>
        ~MessageHandler()
        {
            Dispose(false);
        }

        /// <summary>
        /// Class method to be called by class wrapper for release resources explicitly.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        // Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, 
        // the method has been called directly or indirectly by a user's code. Managed and 
        // unmanaged resources can be disposed.
        // If disposing equals false, the method has been called by the runtime from inside the 
        // finalizer and you should not reference other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            // Release managed & unmanaged resources...
            if (disposing)
            {
                if (_logger.IsInfoEnabled)
                    _logger.Info("Class:[" + _className + "] object is being destroyed... <" + thisMethod + ">");
            }

            if (_perfMonitor != null)
            {
                _perfMonitor.Dispose();
                _perfMonitor = null;
            }

            // Terminate message handling thread.
            if (_handlingThread != null)
            {
                _handlingThread.Abort();
                _handlingThread.Join();
                _handlingThread = null;
            }

            // Release incoming message buffer
            if (_syncdIncomingQueue != null)
            {
                _syncdIncomingQueue.Clear();
                _syncdIncomingQueue = null;
            }

            // Add codes here to release resource
            if (_syncdChannelListPLC2GW != null)
            {
                _syncdChannelListPLC2GW.Clear();
                _syncdChannelListPLC2GW = null;
            }

            if (disposing)
            {
                if (_logger.IsInfoEnabled)
                    _logger.Info("Class:[" + _className + "] object has been destroyed. <" + thisMethod + ">");
            }
        }

        #endregion

        #region Class Method Declaration

        private void Init()
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            if (_logger.IsInfoEnabled)
                _logger.Info("Class:[" + _className + "] object is initializing... <" + thisMethod + ">");

            _isToGatewayChainOpened = false;

            _forwarderPLC2GW.OnReceived += new EventHandler<MessageEventArgs>(ForwarderPLC2GW_OnReceived);
            _forwarderPLC2GW.OnConnected += new EventHandler<MessageEventArgs>(ForwarderPLC2GW_OnConnected);
            _forwarderPLC2GW.OnDisconnected += new EventHandler<MessageEventArgs>(ForwarderPLC2GW_OnDisconnected);

            // Create incoming message buffer
            _incomingQueue = new Queue();
            _syncdIncomingQueue = Queue.Synchronized(_incomingQueue);
            _syncdIncomingQueue.Clear();

            // Create ArrayList object for store opened channel connection name list
            _channelListPLC2GW = new ArrayList();
            _syncdChannelListPLC2GW = ArrayList.Synchronized(_channelListPLC2GW);
            _syncdChannelListPLC2GW.Clear();

            // Create message handling thread
            _handlingThread = new System.Threading.Thread(new ThreadStart(MessageHandlingThread));
            _handlingThread.Name = _className + ".MessageHandlingThread";

            _threadCounter = 0;
            _noOfMsg2GW = 0;
            _perfMonitor = new PALS.Diagnostics.ClassStatus();

            // Start message handling thread;
            _handlingThread.Start();
            Thread.Sleep(0);

            if (_logger.IsInfoEnabled)
                _logger.Info("Class:[" + _className + "] object has been initialized. <" + thisMethod + ">");
        }


        private void ForwarderPLC2GW_OnReceived(object sender, MessageEventArgs e)
        {
            PALS.Telegrams.Common.MessageAndSource msgSource = new PALS.Telegrams.Common.MessageAndSource();

            msgSource.Source = SOURCE_PLC2GW;
            msgSource.Message = e.Message;

            lock (_incomingQueue.SyncRoot)
            {
                _incomingQueue.Enqueue(msgSource);
            }

            // Copy to a temporary variable to be thread-safe.
            EventHandler<MessageEventArgs> temp = OnReceived;
            // Event could be null if there are no subscribers, so check it before raise event
            if (temp != null)
            {
                e.ChainName = SOURCE_PLC2GW;
                // Raise OnReceived event upon message is received.
                temp(this, e);
            }
        }

        private void ForwarderPLC2GW_OnConnected(object sender, MessageEventArgs e)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            lock (_syncdChannelListPLC2GW.SyncRoot)
            {
                if (_syncdChannelListPLC2GW.Contains(e.ChannelName) == false)
                    _syncdChannelListPLC2GW.Add(e.ChannelName);
            }

            if (_logger.IsInfoEnabled)
                _logger.Info("Gateway-PLC device connection <Channel:" + e.ChannelName +
                    "> has been opened. <" + thisMethod + ">");

            // Copy to a temporary variable to be thread-safe.
            EventHandler<MessageEventArgs> temp = OnConnected;
            // Event could be null if there are no subscribers, so check it before raise event
            if (temp != null)
            {
                e.ChainName = SOURCE_PLC2GW;
                // Raise OnConnected event upon channel connection is opened.
                temp(this, e);
            }

            _isToGatewayChainOpened = true;
            
        }

        private void ForwarderPLC2GW_OnDisconnected(object sender, MessageEventArgs e)
        {
            // Ignore the OnDisconnected event if it is not in the opened channel list.
            if (_syncdChannelListPLC2GW.Contains(e.ChannelName) == true)
            {
                _isToGatewayChainOpened = false;

                lock (_syncdChannelListPLC2GW.SyncRoot)
                {
                    _syncdChannelListPLC2GW.Remove(e.ChannelName);
                }

                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

                if (_logger.IsErrorEnabled)
                    _logger.Error("Gateway-PLC device connection <Channel:" +
                            e.ChannelName + "> has been closed. <" + thisMethod + ">");

                // Copy to a temporary variable to be thread-safe.
                EventHandler<MessageEventArgs> temp = OnDisconnected;
                // Event could be null if there are no subscribers, so check it before raise event
                if (temp != null)
                {
                    e.ChainName = SOURCE_PLC2GW;
                    // Raise OnDisconnected event upon channel connection is closed.
                    temp(this, e);
                }
            }
        }

        /// <summary>
        /// Close the specified connection of Gateway-PLC communication chain.
        /// If value null is passed to this method, then all connections of this chain will be closed.
        /// <para>
        /// Disconnect command will be passed to most top class GW2ExternalSessionForwarder object
        /// in the chain, and then passed down to every chain classes to close each layer connections.
        /// </para>
        /// </summary>
        /// <param name="channelName">name of channel</param>
        public void DisconnectToGW(string channelName)
        {
            _forwarderPLC2GW.Disconnect(channelName);
        }

        /// <summary>
        /// Sending message to BHS External devices via Gateway-External chain classes.
        /// </summary>
        /// <para>
        /// The message will be sent to all current opened connections of Gateway-External chain.
        /// </para>
        /// <param name="data"></param>
        public void SentToGW(byte[] data)
        {
            if ((data == null) || (data.Length == 0))
                return;
            else
            {
                Telegram message = new Telegram(ref data);
                _noOfMsg2GW = Functions.CounterIncrease(_noOfMsg2GW);
                _forwarderPLC2GW.Send(message);
            }
        }

        /// <summary>
        /// Sending message to BHS Internal Engine Service application via Gateway-Internal chain classes.
        /// <para>
        /// The message will be sent to all current opened connections of Gateway-Internal chain.
        /// </para>
        /// </summary>
        /// <param name="message"></param>
        public void SentToGW(Telegram message)
        {
            if (message == null)
                return;
            else
            {
                _noOfMsg2GW = Functions.CounterIncrease(_noOfMsg2GW);
                _forwarderPLC2GW.Send(message);
            }
        }

        /// <summary>
        /// Message handling thread.
        /// This thread will be permanently running in background after application is started.
        /// </summary>
        private void MessageHandlingThread()
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            if (_logger.IsInfoEnabled)
                _logger.Info("Message handling thread has been started. <" + thisMethod + ">");

            try
            {
                int count;
                PALS.Telegrams.Common.MessageAndSource msgSource;

                while (true)
                {
                    count = 0;
                    count = _incomingQueue.Count;

                    for (int i = 0; i < count; i++)
                    {
                        msgSource = null;
                        lock (_incomingQueue.SyncRoot)
                        {
                            msgSource = (PALS.Telegrams.Common.MessageAndSource)_incomingQueue.Dequeue();
                        }

                        // Incoming message handling.
                        IncomingMessageHandling(msgSource);
                    }
                    _threadCounter = Functions.CounterIncrease(_threadCounter);
                    Thread.Sleep(THREAD_INTERVAL);
                }
            }
            catch (ThreadAbortException ex)
            {
                Thread.ResetAbort();

                if (_logger.IsInfoEnabled)
                    _logger.Info("Message handling thread has been stopped. <" + thisMethod + ">", ex);
            }
            catch (Exception ex)
            {
                if (_logger.IsErrorEnabled)
                    _logger.Error("Message handling thread failed. <" + thisMethod + ">", ex);

            }
        }

        // <summary>
        /// According to interface protocol design (ItemTracking protocol and TCPServer2Client 
        /// protocol), there are 3 types of incoming messages:
        /// <para>ItemTracking message - Only come from Gateway-External communication chain. 
        /// E.g. come from PLC;</para> 
        /// <para>BagEvent (BEV) message - Only come from Gateway-Internal communication chain;
        /// E.g. come from SAC SortEngine Service;</para> 
        /// <para>Running Status Request (SRQ) and Reply (SRP) message - Only come from 
        /// Gateway-Internal communication chain. E.g. come from BHS Console Console Application;
        /// </para>
        /// </summary>
        /// <param name="msgSource"></param>
        private void IncomingMessageHandling(PALS.Telegrams.Common.MessageAndSource msgSource)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string source;
            PALS.Telegrams.Telegram message;

            try
            {
                if (msgSource == null)
                {
                    return;
                }
                else
                {
                    source = msgSource.Source;
                    message = msgSource.Message;
                }

                if (message.Format == null)
                {
                    if (_logger.IsErrorEnabled)
                        _logger.Error("[Channel:" + message.ChannelName +
                                "] Telegram format is not defined for this incoming message! Message is discarded! [Msg(APP):" +
                                message.ToString(PALS.Utilities.HexToStrMode.ToAscPaddedHexString) + "]. <"
                                + thisMethod + ">");
                }
                else
                {
                    byte[] originType = message.GetFieldActualValue("Type");
                     string type = PALS.Utilities.Functions.ConvertByteArrayToString(
                            originType, -1, PALS.Utilities.HexToStrMode.ToAscString);
                    
                    if (source == SOURCE_PLC2GW)
                    {
                        //SentToGW(message);
                    }
                    /*
                     * else if(){}
                     */
                    else
                    {
                        // Undesired message will be discarded.
                        if (_logger.IsErrorEnabled)
                            _logger.Error("[Channel:" + message.ChannelName +
                                    "] Undesired message is received! it will be discarded... [Msg(APP):" +
                                    message.ToString(PALS.Utilities.HexToStrMode.ToAscPaddedHexString) + "]. <"
                                    + thisMethod + ">");
                    }
                }
            }
            catch (Exception ex)
            {
                if (_logger.IsErrorEnabled)
                    _logger.Error("Incoming message handling is failed. <" + thisMethod + ">", ex);

            }

        }



        private void PerfCounterRefresh()
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                // Refresh current class object status counters.
                if ((_perfMonitor != null) & (ObjectID != string.Empty))
                {
                    _perfMonitor.OpenObjectNode();
                    _perfMonitor.AddObjectStatus("class", _className);
                    _perfMonitor.AddObjectStatus("threadCounter", _threadCounter.ToString());
                    _perfMonitor.AddObjectStatus("incomingQueue", _syncdIncomingQueue.Count.ToString());
                    _perfMonitor.AddObjectStatus("msgs2GW", _noOfMsg2GW.ToString());
                    //_perfMonitor.AddObjectStatus("msgs2SortEngn", _noOfMsg2SortEngine.ToString());
                    _perfMonitor.CloseObjectNode();
                }

            }
            catch (Exception ex)
            {
                if (_logger.IsErrorEnabled)
                    _logger.Error("Exception occurred! <" + thisMethod + ">", ex);
            }
        }

        #endregion
    }
}
