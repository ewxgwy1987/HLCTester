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

        private Hashtable HT_TlgmData;
        Queue<SAC2PLCTelegram> msgsend_queue;
        Queue<SAC2PLCTelegram> msgrecv_queue;
        private Thread thrd_recv;
        private bool mark_thrdrecv;
        private Thread thrd_send;
        private bool mark_thrdsend;

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

        

        #endregion

        #region Member Function

        public bool Init()
        {
            // Find entry point of test
            XEntrypoint = this.InputAnalyser.GetEntryPoint(this.m_projname, this.m_ckinline);
            return true;
        }

        #endregion

        
    }
}
