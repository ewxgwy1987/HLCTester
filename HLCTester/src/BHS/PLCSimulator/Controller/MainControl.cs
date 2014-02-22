using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using System.IO;
using System.Threading;
using System.Xml.Linq;

using BHS.PLCSimulator.Controller;
using BHS.PLCSimulator.Messages.Telegram;
using BHS.PLCSimulator.Messages.TelegramFormat;


namespace BHS.PLCSimulator.Controller
{
    public class MainControl
    {
        #region Class Field and Property

        private XmlHLCTester HLCAnalyser;
        private string m_path_HLCXml;
        private XmlInput InputAnalyser;
        private string m_path_InputXml;

        private string m_projname;
        public string ProjName
        {
            set
            {
                this.m_projname = value;
            }
            get
            {
                return this.m_projname;
            }
        }

        private string m_ckinline;
        public string CheckInLine
        {
            set
            {
                this.m_ckinline = value;
            }
            get
            {
                return this.m_ckinline;
            }
        }

        private XElement XEntrypoint;
        private List<BagNavigator> list_BagNvgs;

        private Hashtable HT_TlgmData;
        Queue<SAC2PLCTelegram> msgsend_queue;
        Queue<SAC2PLCTelegram> msgrecv_queue;
        private Thread thrd_recv;
        private bool mark_thrdrecv;
        private Thread thrd_send;
        private bool mark_thrdsend;
        private const int SLEEP_INTERVAL = 500;

        private bool init_mark;

        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Class Constructor, Dispose, & Destructor

        public MainControl(string xmlHLC_path, string xmlinput_path, string project, string checkin)
        {
            HLCAnalyser = new XmlHLCTester(xmlHLC_path);
            InputAnalyser = new XmlInput(xmlinput_path);
            this.m_path_HLCXml = xmlHLC_path;
            this.m_path_InputXml = xmlinput_path;
            this.ProjName = project;
            this.CheckInLine = checkin;

            HT_TlgmData = new Hashtable();
            this.msgrecv_queue = new Queue<SAC2PLCTelegram>();
            this.msgrecv_queue = new Queue<SAC2PLCTelegram>();
            mark_thrdrecv = true;
            mark_thrdsend = true;
        }

        ~MainControl()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disp_mark)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string infostr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            if (disp_mark)
            {
                _logger.Info(".");
                _logger.Info("Class:[" + _className + "] object is being destroyed... <" + thisMethod + ">");
            }

            //---Clean Variable
            if (HLCAnalyser != null)
            {
                HLCAnalyser.Dispose();
                HLCAnalyser = null;
            }

            if (InputAnalyser != null)
            {
                InputAnalyser.Dispose();
                InputAnalyser = null;
            }

            if (XEntrypoint != null) XEntrypoint = null;

            if (HT_TlgmData != null)
            {
                HT_TlgmData.Clear();
                HT_TlgmData = null;
            }

            if (msgrecv_queue != null)
            {
                msgrecv_queue.Clear();
                msgrecv_queue = null;
            }

            if (msgrecv_queue != null)
            {
                msgrecv_queue.Clear();
                msgrecv_queue = null;
            }

            if (thrd_recv != null)
            {
                if (thrd_recv.IsAlive)
                    mark_thrdrecv = false;
                thrd_recv = null;
            }

            if (thrd_send != null)
            {
                if (thrd_send.IsAlive)
                    mark_thrdsend = false;
                thrd_send = null;
            }

            //--Clear END

            if (disp_mark)
            {
                if (_logger.IsInfoEnabled)
                    _logger.Info("Class:[" + _className + "] object has been destroyed. <" + thisMethod + ">");
            }
        }

        /// <summary>
        /// Init 
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            bool bres = true;
            this.init_mark = false;
            try
            {
                // 1.Find entry point of test
                XEntrypoint = this.InputAnalyser.GetEntryPoint(this.m_projname, this.m_ckinline);
                if(XEntrypoint==null || XEntrypoint.ToString()=="")
                {
                    errstr += "Cannot find entry point. Project:" + m_projname + ", Checkin Line:" + m_ckinline + "\n";
                    errstr += "MainControl  initialization failed.";
                    _logger.Error(errstr);
                    return false;
                }

                // 2.Initiate the HT_TlgmData
                string[] tlgmnodes = this.HLCAnalyser.GetAllTlgmDpndNodes();
                foreach (string tlgm in tlgmnodes)
                {
                    Hashtable ht_newtlgm = new Hashtable();
                    HT_TlgmData.Add(tlgm, ht_newtlgm);
                }

                // 3.read input files and import data;
                string filepath = this.InputAnalyser.GetInputFilePath(this.m_projname, this.m_ckinline);
                double itlrate = this.InputAnalyser.GetDefaultITLRate(this.m_projname, this.m_ckinline);

                FileInfo inputfile = new FileInfo(filepath);
                if (inputfile.Exists)
                {
                    using (StreamReader sr = inputfile.OpenText())
                    {
                        // the first line is column head
                        string rawdata = sr.ReadLine();
                        while ((rawdata = sr.ReadLine()) != null)
                        {
                            BagNavigator newbag = new BagNavigator(this.m_path_HLCXml, this.m_path_InputXml, rawdata, this.XEntrypoint, this.msgsend_queue, this.HT_TlgmData);
                            this.list_BagNvgs.Add(newbag);
                        }
                    }
                }
                else
                {
                    errstr += "Cannot find input file. File Path:" + filepath + ", Project:" + m_projname + ", Checkin Line:" + m_ckinline + "\n";
                    errstr += "MainControl  initialization failed.";
                    _logger.Error(errstr);
                    return false;
                }

                // 4.create receiving and sending threads
                this.thrd_send = new Thread(ThrdFun_SendMsg);
                this.thrd_send.IsBackground=true;
                this.thrd_send.Start();

            }
            catch (Exception exp)
            {
                errstr += exp.ToString() + "\n";
                errstr += "MainControl  initialization failed.";
                _logger.Error(errstr);
                bres = false;
            }
            this.init_mark = true;
            _logger.Info("MainControl is initialized successfully.");
            return bres;
        }

        #endregion

        #region Member Function

       
        /// <summary>
        /// Begin Test for each bag in the input file
        /// </summary>
        public void BeginTest()
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            if (this.init_mark == true)
            {
                foreach (BagNavigator bagnvg in this.list_BagNvgs)
                {
                    bagnvg.Start();
                }
            }
            else
            {
                errstr += "MailControl is not init. So cannot begin test.";
                _logger.Error(errstr);
            }
        }

        private void ThrdFun_SendMsg()
        {
            while (this.mark_thrdsend)
            {
                if (this.msgsend_queue.Peek() != null)
                {
                    SAC2PLCTelegram sendtlgm = this.msgsend_queue.Dequeue();
                    //_init.MsgHandler.SentToGW(sendtlgm.RawData);
                }
                Thread.Sleep(SLEEP_INTERVAL);
            }
        }

        public bool ReceiveTelegram(MessageEventArgs e)
        {

        }

        #endregion

        
    }
}
