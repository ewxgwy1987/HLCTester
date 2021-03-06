﻿using System;
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
    public class BagNavigator
    {
        #region Class Field and Property

        private XmlHLCTester HLCAnalyser;
        private XmlInput InputAnalyser;

        private Hashtable HT_EventList;
        private Hashtable HT_DpndNodes;

        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Class Constructor, Dispose, & Destructor

        public BagNavigator(string xmlHLC_path, string xmlinput_path, string rawdata, XElement entrypoint, Queue<SAC2PLCTelegram> sendqueue, Hashtable ht_tlgmnodes)
        {
            HLCAnalyser = new XmlHLCTester(xmlHLC_path);
            InputAnalyser = new XmlInput(xmlinput_path);

            HT_EventList = new Hashtable();
            HT_DpndNodes = new Hashtable();
        }

        public bool Init()
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            bool bres = true;
            try
            {
                // Initiate the HT_DpndNodes
                string[] dpndnodes = this.HLCAnalyser.GetAllLocDpndNodes();

                // Initiate the HT_EventList
                

                // 

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
                bres = false;
            }
            return bres;
        }

        ~BagNavigator()
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


            //--Clear END

            if (disp_mark)
            {
                if (_logger.IsInfoEnabled)
                    _logger.Info("Class:[" + _className + "] object has been destroyed. <" + thisMethod + ">");
            }
        }

        #endregion

        #region Member Function

        private string GetDataByPos(int pos, string rawdata)
        {
            string[] dataparts = rawdata.Split(',');
            if (pos < dataparts.Length)
                return dataparts[pos];
            else
                return "";
        }

        public void Start()
        {
        }

        #endregion

        
    }
}
