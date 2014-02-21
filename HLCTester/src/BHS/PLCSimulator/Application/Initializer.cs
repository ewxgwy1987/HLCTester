using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml;
using System.Collections;

using PALS;
using PALS.Net;
using PALS.Configure;
using PALS.Utilities;

namespace BHS.PLCSimulator
{
    public class Initializer:IDisposable
    {
        #region Class Fields and Properties Declaration

        private const string XMLCONFIG_LOG4NET = "log4net";

        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Configuration files
        private FileInfo _xmlFileSetting;
        private FileInfo _xmlFileTelegram;
        private FileInfo _xmlFileTestcase;

        // Used to store the reference of ConfigureAndWatchHandler class object for proper release of file 
        // watchers (done by Dispose() method of Initializer class) when application is closed .
        private ConfigureAndWatchHandler _fileWatchHandler;

        // Object of class XmlSettingLoader derived from interface IConfigurationLoader for loading setting from XML file.
        private BHS.PLCSimulator.XmlSettingLoader _xmlLoader;


        // Declare Simulator - External device(TCP Server) To BHS (TCP Client) chain classes
        // PALS.Net.Handlers classes object
        private PALS.Common.IChain _forwarderPLC2GW;
        // PALS.Net.Managers.SessionManager object
        private PALS.Net.Managers.SessionManager _managerPLCSimulator;
        // PALS.Net.Filters chain classes
        private PALS.Common.IChain _outMIDGW2External;
        private PALS.Common.IChain _ackGW2External;
        private PALS.Common.IChain _inMIDGW2External;
        private PALS.Common.IChain _solGW2External;
        private PALS.Common.IChain _tsynGW2External;
        private PALS.Common.IChain _appServerPLC2GW;
        private PALS.Common.IChain _cipPLC2GW;
        private PALS.Common.IChain _eipPLC2GW;

        private BHS.PLCSimulator.MessageHandler _msgHandler;

        //// The ClassStatus object of current class
        //private PALS.Diagnostics.ClassStatus _perfMonitor;
        //// The Hashtable that contains the ClassStatus object of current class 
        //// and all of its instance of sub classes.
        //private ArrayList _perfMonitorList;

        /// <summary>
        /// Get or set the BHS.Gateway.TCPClientTCPClientChains.Messages.Handlers.Messagehandler class object.
        /// </summary>
        public BHS.PLCSimulator.MessageHandler MsgHandler
        {
            get { return _msgHandler; }
            set { _msgHandler = value; }
        }

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

        public Hashtable HT_TelegramTestcase;

        ///// <summary>
        ///// null
        ///// </summary>
        //public PALS.Diagnostics.ClassStatus PerfMonitor
        //{
        //    get
        //    {
        //        try
        //        {
        //            //_perfMonitor.ObjectID = ObjectID;
        //            PerfCounterRefresh();
        //            return _perfMonitor;
        //        }
        //        catch (Exception ex)
        //        {
        //            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

        //            if (_logger.IsErrorEnabled)
        //                _logger.Error("Exception occurred! <" + thisMethod + ">", ex);

        //            return null;
        //        }
        //    }
        //}

        ///// <summary>
        ///// null
        ///// </summary>
        //public ArrayList PerfMonitorList
        //{
        //    get
        //    {
        //        try
        //        {
        //            PALS.Diagnostics.ClassStatus temp = PerfMonitor;

        //            return _perfMonitorList;
        //        }
        //        catch (Exception ex)
        //        {
        //            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

        //            if (_logger.IsErrorEnabled)
        //                _logger.Error("Exception occurred! <" + thisMethod + ">", ex);

        //            return null;
        //        }
        //    }
        //}

        #endregion

        #region Class Constructor, Dispose, & Destructor

        public Initializer(FileInfo[] cfgFiles)
        {
            XmlElement xmlRoot = PALS.Utilities.XMLConfig.GetConfigFileRootElement(cfgFiles[0].FullName);
            if (xmlRoot == null)
            {
                throw new Exception("Open application setting XML configuration file failure!");
            }


            XmlElement log4netConfig = (XmlElement)PALS.Utilities.XMLConfig.GetConfigSetElement(ref xmlRoot, XMLCONFIG_LOG4NET);
            if (log4netConfig == null)
            {
                throw new System.Exception("There is no <" + XMLCONFIG_LOG4NET +
                                "> settings in the XML configuration file!");
            }
            else
            {
                _xmlFileSetting = cfgFiles[0];
                _xmlFileTelegram = cfgFiles[1];
                _xmlFileTestcase = cfgFiles[2];

                log4net.Config.XmlConfigurator.Configure(log4netConfig);
                _logger.Info(".");
                _logger.Info(".");
                _logger.Info(".");
                _logger.Info("[................................] <" + _className + ".Initializer()>");
                _logger.Info("[...App PLC Simulator Starting...] <" + _className + ".Initializer()>");
                _logger.Info("[................................] <" + _className + ".Initializer()>");
            }
        }

        /// <summary>
        /// Destructer of Initializer class.
        /// </summary>
        ~Initializer()
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
            if (disposing)
            {
                _logger.Info(".");
                _logger.Info("Class:[" + _className + "] object is being destroyed... <" + thisMethod + ">");
            }

            // -----------------------------------------------------------------------------
            // Destory Simulator chain classes
            if (_outMIDGW2External != null)
            {
                PALS.Net.Filters.Application.OutgoingMessageIdentifier outMID =
                            (PALS.Net.Filters.Application.OutgoingMessageIdentifier)_outMIDGW2External;
                outMID.Dispose();
                _outMIDGW2External = null;
            }

            if (_ackGW2External != null)
            {
                PALS.Net.Filters.Acknowledge.ACK ack =
                            (PALS.Net.Filters.Acknowledge.ACK)_ackGW2External;
                ack.Dispose();
                _ackGW2External = null;
            }

            if (_inMIDGW2External != null)
            {
                PALS.Net.Filters.Application.IncomingMessageIdentifier inMID =
                            (PALS.Net.Filters.Application.IncomingMessageIdentifier)_inMIDGW2External;
                inMID.Dispose();
                _inMIDGW2External = null;
            }

            if (_tsynGW2External != null)
            {
                PALS.Net.Filters.TimeSynchronizing.TimeSync tsyn =
                            (PALS.Net.Filters.TimeSynchronizing.TimeSync)_tsynGW2External;
                tsyn.Dispose();
                _tsynGW2External = null;
            }

            if (_solGW2External != null)
            {
                PALS.Net.Filters.SignOfLife.SOL sol =
                            (PALS.Net.Filters.SignOfLife.SOL)_solGW2External;
                sol.Dispose();
                _solGW2External = null;
            }

            if (_appServerPLC2GW != null)
            {
                PALS.Net.Filters.Application.AppServer appCln =
                            (PALS.Net.Filters.Application.AppServer)_appServerPLC2GW;
                appCln.Dispose();
                _appServerPLC2GW = null;
            }

            if (_cipPLC2GW != null)
            {
                PALS.Net.Filters.EIPCIP.CIP cip =
                            (PALS.Net.Filters.EIPCIP.CIP)_cipPLC2GW;
                cip.Dispose();
                _cipPLC2GW = null;
            }

            if (_eipPLC2GW != null)
            {
                PALS.Net.Filters.EIPCIP.EIP eip =
                            (PALS.Net.Filters.EIPCIP.EIP)_eipPLC2GW;
                eip.Dispose();
                _eipPLC2GW = null;
            }

            if (_forwarderPLC2GW != null)
            {
                BHS.PLCSimulator.PLC2GWSessionForward fwdr =
                            (BHS.PLCSimulator.PLC2GWSessionForward)_forwarderPLC2GW;
                fwdr.Dispose();
                _forwarderPLC2GW = null;
            }

            if (_managerPLCSimulator != null)
            {
                PALS.Net.Managers.SessionManager mgr =
                            (PALS.Net.Managers.SessionManager)_managerPLCSimulator;
                mgr.Dispose();
                _managerPLCSimulator = null;

                System.Threading.Thread.Sleep(200);
            }
            // -----------------------------------------------------------------------------

            // Destory centralized message handler.
            if (_msgHandler != null)
            {
                _msgHandler.Dispose();
                _msgHandler = null;
            }

            // Destory configuration file watcher.
            if (_fileWatchHandler != null) _fileWatchHandler.Dispose();
            if (_xmlLoader != null) _xmlLoader.Dispose();
            // -----------------------------------------------------------------------------

            if (disposing)
            {
                _logger.Info("Class:[" + _className + "] object has been destroyed. <" + thisMethod + ">");
                _logger.Info("[..................] <" + thisMethod + ">");
                _logger.Info("[...App Stopped....] <" + thisMethod + ">");
                _logger.Info("[..................] <" + thisMethod + ">");
            }
        }
        #endregion

        /// <summary>
        /// Init() method of Initializer class is the place to perform the initialization
        /// tasks for current application. All initialization tasks needed to be done during
        /// the application startup time should be performed here.
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            try
            {
                if (_logger.IsInfoEnabled)
                    _logger.Info("Initializing application settings... <" + thisMethod + ">");

                _xmlLoader = new BHS.PLCSimulator.XmlSettingLoader();
                _xmlLoader.OnReloadSettingCompleted += new EventHandler(Handler_OnReloadSettingCompleted);

                // Load system parameters from two configuration files (CFG_MDS2CCTVGW.xml, CFG_Telegrams.xml).
                // And also start watcher to detect the change of files.  and reload setting if change is detected.
                _fileWatchHandler = PALS.Configure.AppConfigurator.ConfigureAndWatch(
                                        _xmlLoader, _xmlFileSetting, _xmlFileTelegram,_xmlFileTestcase);
                // Initialize TelegramTypeName, TelegramFormatList, and TelegramFormat

                BuildSimulatorProtocolChain();

                _msgHandler = new BHS.PLCSimulator.MessageHandler(
                    _xmlLoader.Paramters_MsgHandler,
                    (BHS.PLCSimulator.PLC2GWSessionForward)_forwarderPLC2GW);

                _msgHandler.OnReceived += new EventHandler<MessageEventArgs>(MsgHandler_OnReceived);
                _msgHandler.OnConnected += new EventHandler<MessageEventArgs>(MsgHandler_OnConnected);
                _msgHandler.OnDisconnected += new EventHandler<MessageEventArgs>(MsgHandler_OnDisconnected);


                if (_managerPLCSimulator != null)
                    _managerPLCSimulator.SessionStart();
                else
                    throw new Exception("SessionManager of GW2Internal chain is not created!");

                if (_logger.IsInfoEnabled)
                {
                    _logger.Info("Initializing application setting is successed. <" + thisMethod + ">");
                    _logger.Info(".");
                }

                // Initialize the Test Case
                int idx = 1;
                string errorstr="";
                HT_TelegramTestcase = new Hashtable();
                XmlElement rootTestCase = XMLConfig.GetConfigFileRootElement(ref _xmlFileTestcase);
                foreach (XmlNode temp_node in  rootTestCase.ChildNodes)
                {
                    if (temp_node.NodeType != XmlNodeType.Comment && temp_node.Name == "telegram")
                    {
                        string alias = XMLConfig.GetSettingFromAttribute(temp_node, "alias", "WrongAlias");
                        if (TelegramFormatList.HasTelegram(alias))
                        {
                            Type telegram_type;
                            if (alias == "CRAI" || alias == "FBTI" || alias == "FPTI")
                                telegram_type = Type.GetType("BHS.PLCSimulator." + alias + "_Telegram");
                            else
                                telegram_type = Type.GetType("BHS.PLCSimulator.Messages.Telegram.General_Telegram");

                            SAC2PLCTelegram new_telegram = (SAC2PLCTelegram)Activator.CreateInstance(telegram_type, new Object[2] { temp_node, alias });
                            new_telegram.TelegramEncoding();
                            this.HT_TelegramTestcase.Add(idx, new_telegram);
                            idx++;
                        }
                        else
                        {
                            errorstr += "Error in" + thisMethod + "\r\n";
                            errorstr += "No such Alias:" + alias + "for TestCase: " + _xmlFileTestcase.FullName + "\r\n";
                            _logger.Error(errorstr);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                if (_logger.IsErrorEnabled)
                    _logger.Error("Initializing application setting is failed! <" + thisMethod + ">", ex);

                return false;
            }
        }

       

        /// <summary>
        /// ------------------------------------------------------------------------------------
        /// Build "Handler-Filter-Transport" chain for PLCSimulator TCPClient chain by following below sequence:
        /// ------------------------------------------------------------------------------------
        /// BHS.PLCSimulator.PLC2GWSessionForward
        /// PALS.Net.handlers.SessionHandler
        /// PALS.Net.Filters.Application.OutgoingMessageIdentifier 
        /// PALS.Net.Filters.Acknowledge.ACK 
        /// PALS.Net.Filters.Application.IncomingMessageIdentifier 
        /// PALS.Net.Filters.SignOfLife.SOL 
        /// PALS.Net.Filters.Application.APPClient 
        /// PALS.Net.Filters.EIPCIP.CIP
        /// PALS.Net.Filters.EIPCIP.EIP
        /// PALS.Net.Transports.TCP.TCPClient
        /// ------------------------------------------------------------------------------------
        /// </summary>
        private void BuildSimulatorProtocolChain()
        {
            // Instantiate Sessionmanager class to build basice TCPClient-SessioHandler chain
            PALS.Common.IParameters paramTCPSvrCln = _xmlLoader.Paramters_TCPServerClient_PLCSimulator;
            _managerPLCSimulator = new PALS.Net.Managers.SessionManager(
                        PALS.Net.Common.TransportProtocol.TCPServerClient, ref paramTCPSvrCln);
            //_managerPLCSimulator.ObjectID = OBJECT_ID_GW2EXTERNAL_SESSIONMANAGER;
            //_managerPLCSimulator.TransportObjectID = OBJECT_ID_GW2EXTERNAL_TCPSERVERCLIENT;
            //_managerPLCSimulator.HandlerObjectID = OBJECT_ID_GW2EXTERNAL_SESSIONHANDLER;


            // Instantiate EIP class
            PALS.Common.IParameters paramEIP = _xmlLoader.Paramters_EIP;
            _eipPLC2GW = new PALS.Net.Filters.EIPCIP.EIP(ref paramEIP);
            //((PALS.Net.Common.AbstractProtocolChain)_eipPLC2GW).ObjectID = OBJECT_ID_GW2EXTERNAL_EIP;

            // Instantiate CIP class
            PALS.Common.IParameters paramCIP = _xmlLoader.Paramters_CIP;
            _cipPLC2GW = new PALS.Net.Filters.EIPCIP.CIP(ref paramCIP);
            //((PALS.Net.Common.AbstractProtocolChain)_cipPLC2GW).ObjectID = OBJECT_ID_GW2EXTERNAL_CIP;

            // Instantiate AppClient class
            PALS.Common.IParameters paramApp = _xmlLoader.Paramters_AppServer_PLCSimulator;
            _appServerPLC2GW = new PALS.Net.Filters.Application.AppServer(ref paramApp);
            //((PALS.Net.Common.AbstractProtocolChain)_appServerPLC2GW).ObjectID = OBJECT_ID_GW2EXTERNAL_APPCLIENT;

            // Instantiate SOL class
            PALS.Common.IParameters paramSOL = _xmlLoader.Paramters_SOL;
            _solGW2External = new PALS.Net.Filters.SignOfLife.SOL(ref paramSOL);
            //((PALS.Net.Common.AbstractProtocolChain)_solGW2External).ObjectID = OBJECT_ID_GW2EXTERNAL_SOL;

            // Instantiate TSYN class
            PALS.Common.IParameters paramTSYN = _xmlLoader.Paramters_TSYN;
            _tsynGW2External = new PALS.Net.Filters.TimeSynchronizing.TimeSync(ref paramTSYN);
            //((PALS.Net.Common.AbstractProtocolChain)_tsynGW2External).ObjectID = OBJECT_ID_GW2EXTERNAL_TSYN;

            // Instantiate Message Identifier (MID) class
            PALS.Common.IParameters paramMID = _xmlLoader.Paramters_MID;
            _inMIDGW2External = new PALS.Net.Filters.Application.IncomingMessageIdentifier(ref paramMID);
            //((PALS.Net.Common.AbstractProtocolChain)_inMIDGW2External).ObjectID = OBJECT_ID_GW2EXTERNAL_INMID;
            _outMIDGW2External = new PALS.Net.Filters.Application.OutgoingMessageIdentifier(ref paramMID);
            //((PALS.Net.Common.AbstractProtocolChain)_outMIDGW2External).ObjectID = OBJECT_ID_GW2EXTERNAL_OUTMID;

            // Instantiate ACK class
            PALS.Common.IParameters paramACK = _xmlLoader.Paramters_ACK;
            _ackGW2External = new PALS.Net.Filters.Acknowledge.ACK(ref paramACK);
            //((PALS.Net.Common.AbstractProtocolChain)_ackGW2External).ObjectID = OBJECT_ID_GW2EXTERNAL_ACK;

            // Instantiate GW2ExternalSessionForwarder class
            _forwarderPLC2GW = new BHS.PLCSimulator.PLC2GWSessionForward(null);
            //((PALS.Net.Common.AbstractProtocolChain)_forwarderGW2External).ObjectID = OBJECT_ID_GW2EXTERNAL_SESSIONFORWARDER;

            // Build GW2External communication chain
            _managerPLCSimulator.AddHandlerToLast(ref _forwarderPLC2GW);
            _managerPLCSimulator.AddFilterToLast(ref _outMIDGW2External);
            _managerPLCSimulator.AddFilterToLast(ref _ackGW2External);
            _managerPLCSimulator.AddFilterToLast(ref _inMIDGW2External);
            _managerPLCSimulator.AddFilterToLast(ref _tsynGW2External);
            _managerPLCSimulator.AddFilterToLast(ref _solGW2External);
            _managerPLCSimulator.AddFilterToLast(ref _appServerPLC2GW);
            _managerPLCSimulator.AddFilterToLast(ref _cipPLC2GW);
            _managerPLCSimulator.AddFilterToLast(ref _eipPLC2GW);
        }

        /// <summary>
        /// Event handler of ReloadSettingCompleted event fired by IConfigurationLoader interface 
        /// implemented class method LoadSettingFromConfigFile() upon the reloading setting from
        /// changed file is successfully completed. 
        /// 
        /// This event handler is to make sure the reloaded settings can be taken effective 
        /// immediately.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Handler_OnReloadSettingCompleted(object sender, EventArgs e)
        {
            // Reassign the reference of new parameter class object to GW2CCTV chain classes
            ((PALS.Net.Managers.SessionManager)_managerPLCSimulator).ClassParameters =
                    _xmlLoader.Paramters_TCPServerClient_PLCSimulator;
            ((PALS.Net.Filters.EIPCIP.EIP)_eipPLC2GW).ClassParameters =
                    (PALS.Net.Filters.EIPCIP.EIPParameters)_xmlLoader.Paramters_EIP;
            ((PALS.Net.Filters.EIPCIP.CIP)_cipPLC2GW).ClassParameters =
            (PALS.Net.Filters.EIPCIP.CIPParameters)_xmlLoader.Paramters_CIP;
            ((PALS.Net.Filters.Application.AppServer)_appServerPLC2GW).ClassParameters =
                    (PALS.Net.Filters.Application.AppServerParameters)_xmlLoader.Paramters_AppServer_PLCSimulator;
            ((PALS.Net.Filters.SignOfLife.SOL)_solGW2External).ClassParameters =
                    (PALS.Net.Filters.SignOfLife.SOLParameters)_xmlLoader.Paramters_SOL;
            ((PALS.Net.Filters.TimeSynchronizing.TimeSync)_tsynGW2External).ClassParameters =
                    (PALS.Net.Filters.TimeSynchronizing.TimeSyncParameters)_xmlLoader.Paramters_TSYN;
            ((PALS.Net.Filters.Application.IncomingMessageIdentifier)_inMIDGW2External).ClassParameters =
                    (PALS.Net.Filters.Application.MessageIdentifierParameters)_xmlLoader.Paramters_MID;
            ((PALS.Net.Filters.Acknowledge.ACK)_ackGW2External).ClassParameters =
                    (PALS.Net.Filters.Acknowledge.ACKParameters)_xmlLoader.Paramters_ACK;
            ((PALS.Net.Filters.Application.OutgoingMessageIdentifier)_outMIDGW2External).ClassParameters =
                    (PALS.Net.Filters.Application.MessageIdentifierParameters)_xmlLoader.Paramters_MID;

            // Reassign the reference of new parameter class object to MessageHandler class
            ((BHS.PLCSimulator.MessageHandler)_msgHandler).ClassParameters =
                    (BHS.PLCSimulator.MessageHandlerParameters)_xmlLoader.Paramters_MsgHandler;
        }

        private void MsgHandler_OnReceived(object sender, MessageEventArgs e)
        {
            // Copy to a temporary variable to be thread-safe.
            EventHandler<MessageEventArgs> temp = OnReceived;
            // Event could be null if there are no subscribers, so check it before raise event
            if (temp != null)
                // Raise OnReceived event upon message is received.
                temp(this, e);
        }

        private void MsgHandler_OnConnected(object sender, MessageEventArgs e)
        {
            // Copy to a temporary variable to be thread-safe.
            EventHandler<MessageEventArgs> temp = OnConnected;
            // Event could be null if there are no subscribers, so check it before raise event
            if (temp != null)
                // Raise OnConnected event upon channel connection is opened.
                temp(this, e);
        }

        private void MsgHandler_OnDisconnected(object sender, MessageEventArgs e)
        {
            // Copy to a temporary variable to be thread-safe.
            EventHandler<MessageEventArgs> temp = OnDisconnected;
            // Event could be null if there are no subscribers, so check it before raise event
            if (temp != null)
                // Raise OnDisconnected event upon channel connection is closed.
                temp(this, e);
        }
    }
}
