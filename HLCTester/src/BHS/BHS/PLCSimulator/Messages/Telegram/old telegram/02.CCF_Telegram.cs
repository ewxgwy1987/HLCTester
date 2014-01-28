using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BHS.PLCSimulator
{
    //(0002) Application Layer Connection Confirm
    class CCF_Telegram : SAC2PLCTelegram
    {
        #region Class Field and Property
        // Class const variable
        private const string m_aliasname = "CCF";

        // fdn - field name
        private const string FDN_CLIENTAPPCODE = "ClientAppCode";

        // private variable -- Field Value
        private char[] m_ClientAppCode;

        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string ClientAppCode
        {
            get
            {
                return new string(m_ClientAppCode);
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_ClientAppCode = value.ToCharArray(0, 8);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }
        #endregion

        #region CCF_Telegram Constructor, Dispose, Finalize and Destructor

        public CCF_Telegram(byte[] rawdata)
            : base(rawdata,m_aliasname)
        {
            Console.WriteLine("CQR_Telegram Constructor with byte[] rawdata");
            this.TelegramDecoding();
        }

        public CCF_Telegram(byte[] rawdata, string channel)
            : base(rawdata, channel, m_aliasname)
        {
        }

        public CCF_Telegram()
            : base(m_aliasname)
        {
        }

        public CCF_Telegram
            (string v_ClientAppCode)
            : base(m_aliasname)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            if (SetICRData(v_ClientAppCode))
            {
                byte[] Telegram_datafield = this.TelegramEncoding();
            }
            else
            {
                string errorstr = "SetICRData Failed. In " + thisMethod;
                Console.WriteLine(errorstr);
                _logger.Error(errorstr);
            }
        }

        #endregion

        #region Member Function

        public bool SetICRData
            (string v_ClientAppCode)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            try
            {
                this.ClientAppCode = v_ClientAppCode;
                return true;
            }
            catch (Exception exp)
            {
                string errorstr = "Error in " + thisMethod + "\n" + exp.ToString();
                Console.WriteLine(errorstr);
                _logger.Error(errorstr);
                return false;
            }
        }

        protected override bool AnalyseFieldData()
        {
            throw new NotImplementedException();
        }

        public override string ShowAllData()
        {
            string showstr = "";
            return showstr;
        }

        protected override bool HasAllData()
        {
            return (this.m_ClientAppCode != null);
        }

        protected override bool FillFieldData()
        {
            byte[] b_clientappcode = Encoding.Default.GetBytes(this.m_ClientAppCode);

            int datafield_length = b_clientappcode.Length;

            // create raw data template
            CreateRawdataTemplate(datafield_length);

            bool result = true;
            //result &= SetFieldValue(fdn_ICR_SEQ,
            result &= SetFieldValue(FDN_CLIENTAPPCODE, b_clientappcode);

            return result;
        }
        #endregion
    }
}
