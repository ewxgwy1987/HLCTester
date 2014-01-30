using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using PALS.Utilities;

namespace BHS.PLCSimulator
{
    // (0005) Item Scanned
    public class ISC_Telegram : SAC2PLCTelegram
    {
         #region Class Field and Property
        // Class const variable
        private const string m_aliasname = "ISC";

        // fdn - field name
        private const string FDN_GID_MSB = "GID_MSB";
        private const string FDN_GID_LSB = "GID_LSB";
        private const string FDN_LOCATION = "LOCATION";
        private const string FDN_LIC1 = "LIC_1";
        private const string FDN_LIC2 = "LIC_2";
        private const string FDN_SCN_HEAD = "SCN_HEAD";
        private const string FDN_SCN_STS = "SCN_STS";
        private const string FDN_PLC_IDX = "PLC_IDX";

        // Real Value Length (For Display)
        private const int LEN_GID_MSB = 2;
        private const int LEN_GID_LSB = 8;
        private const int LEN_LOCATION = 5;
        private const int LEN_LIC1 = 10;
        private const int LEN_LIC2 = 10;
        private const int LEN_SCN_HEAD = 0;// no use
        private const int LEN_SCN_STS = 1;
        private const int LEN_PLC_IDX = 5;

        private const int LP_LENGTH = 10;
        private const int SCN_HEAD_BYTENO = 2;

        // private variable -- Field Value
        private byte m_GID_MSB = 0;
        private int m_GID_LSB = 0;
        private short m_location = 0;
        private char[] m_LIC1;// 10 bytes
        private char[] m_LIC2;// 10 bytes
        private byte[] m_SCN_HEAD;// 2 bytes
        private byte m_SCN_STS = 0;
        private ushort m_PLC_IDX = 0;

        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string GID_MSB
        {
            get
            {
                //return m_GID_MSB.ToString();
                return m_GID_MSB.ToString("D" + LEN_GID_MSB.ToString());
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_GID_MSB = Convert.ToByte(value);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }

        public string GID_LSB
        {
            get
            {
                //return m_GID_LSB.ToString();
                return m_GID_LSB.ToString("D" + LEN_GID_LSB.ToString());
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_GID_LSB = Convert.ToInt32(value);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }

        public string LOCATION
        {
            get
            {
                //return m_location.ToString();
                return m_location.ToString("D" + LEN_LOCATION.ToString());
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_location = Convert.ToInt16(value);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }

        public string LIC_1
        {
            get
            {
                return new string(this.m_LIC1);
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_LIC1 = value.ToCharArray(0, LP_LENGTH);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }

        public string LIC_2
        {
            get
            {
                return new string(this.m_LIC2);
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_LIC2 = value.ToCharArray(0, LP_LENGTH);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }

        public string SCN_HEAD
        {
            get
            {
                string str_scnhead = "";
                int i;
                for (i = 0; i < this.m_SCN_HEAD.Length; i++)
                {
                    str_scnhead += "<"+this.m_SCN_HEAD[i].ToString("X2")+">";
                }
                return str_scnhead;
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_SCN_HEAD = Util.HexByteStrToArray(value);
                    if (this.m_SCN_HEAD.Length != SCN_HEAD_BYTENO)
                        throw new Exception("The length of SCN_HEAD is not ." + SCN_HEAD_BYTENO);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }

        public string SCN_STS
        {
            get
            {
                //return m_SCN_STS.ToString();
                return m_SCN_STS.ToString("D" + LEN_SCN_STS.ToString());
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_SCN_STS = Convert.ToByte(value);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }

        public string PLC_IDX
        {
            get
            {
                //return m_PLC_IDX.ToString();
                return m_PLC_IDX.ToString("D" + LEN_PLC_IDX.ToString());
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_PLC_IDX = Convert.ToUInt16(value);
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

        #region SAC2PLCTelegram Constructor, Dispose, Finalize and Destructor

        public ISC_Telegram(byte[] rawdata)
            : base(rawdata, m_aliasname)
        {
            Console.WriteLine("ICR_Telegram Constructor with byte[] rawdata");
            this.TelegramDecoding();
        }

        public ISC_Telegram(byte[] rawdata, string channel)
            : base(rawdata, channel, m_aliasname)
        {
        }

        public ISC_Telegram()
            : base(m_aliasname)
        {
        }

        public ISC_Telegram
            (byte gid_msb, int gid_lsb, short location, char[] lic1, char[] lic2, byte[] scn_head, byte scn_sts, byte plc_idx)
            : base(m_aliasname)
        {
            SetICRData(gid_msb, gid_lsb, location, lic1, lic2, scn_head, scn_sts, plc_idx);
            byte[] Telegram_datafield = this.TelegramEncoding();
        }

        public ISC_Telegram
            (string gid_msb, string gid_lsb, string location, string lic1, string lic2, string scn_head, string scn_sts, string plc_idx)
            : base(m_aliasname)
        {
            SetICRData(gid_msb, gid_lsb, location, lic1, lic2, scn_head, scn_sts, plc_idx);
            byte[] Telegram_datafield = this.TelegramEncoding();
        }

        public ISC_Telegram(XmlNode xml_TeleTestcase)
            : base(m_aliasname)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            try
            {
                string gid_msb = XMLConfig.GetSettingFromInnerText(xml_TeleTestcase, FDN_GID_MSB, "?");
                string gid_lsb = XMLConfig.GetSettingFromInnerText(xml_TeleTestcase, FDN_GID_LSB, "?");
                if (gid_msb == "?" || gid_lsb == "?")
                {
                    char[] gid = Util.GetGID();
                    gid_msb = new string(gid, 0, LEN_GID_MSB);
                    gid_lsb = new string(gid, LEN_GID_MSB, LEN_GID_LSB);
                }
                string location = XMLConfig.GetSettingFromInnerText(xml_TeleTestcase, FDN_LOCATION, "00000");
                string lic1 = XMLConfig.GetSettingFromInnerText(xml_TeleTestcase, FDN_LIC1, "0000000000");
                string lic2 = XMLConfig.GetSettingFromInnerText(xml_TeleTestcase, FDN_LIC2, "0000000000");
                string scn_head = XMLConfig.GetSettingFromInnerText(xml_TeleTestcase, FDN_SCN_HEAD, "<00><00>");
                string scn_sts = XMLConfig.GetSettingFromInnerText(xml_TeleTestcase, FDN_SCN_STS, "6");
                string plc_idx = XMLConfig.GetSettingFromInnerText(xml_TeleTestcase, FDN_PLC_IDX, "00");
                SetICRData(gid_msb, gid_lsb, location, lic1, lic2, scn_head, scn_sts, plc_idx);
                byte[] Telegram_datafield = this.TelegramEncoding();
            }
            catch (Exception exp)
            {
                string errorstr = "Error in " + thisMethod + "\n" + exp.ToString();
                _logger.Error(errorstr);
            }
        }
        #endregion

        #region Member Function
        public bool SetICRData
            (string gid_msb, string gid_lsb, string location, string lic1, string lic2, string scn_head, string scn_sts, string plc_idx)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            try
            {
                this.GID_MSB = gid_msb;
                this.GID_LSB = gid_lsb;
                this.LOCATION = location;
                this.LIC_1 = lic1;
                this.LIC_2 = lic2;
                this.SCN_HEAD = scn_head;
                this.SCN_STS = scn_sts;
                this.PLC_IDX = plc_idx;
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

        public bool SetICRData
            (byte gid_msb, int gid_lsb, short location, char[] lic1, char[] lic2, byte[] scn_head, byte scn_sts, byte plc_idx)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            this.m_GID_MSB = gid_msb;
            this.m_GID_LSB = gid_lsb;
            this.m_location = location;
            if (lic1.Length == LP_LENGTH && lic2.Length == LP_LENGTH)
            {
                this.m_LIC1 = lic1;
                this.m_LIC2 = lic2;
            }
            else
            {
                string errorstr = "The Length of License Plate is not " + LP_LENGTH + " in " + thisMethod;
                Console.WriteLine(errorstr);
                _logger.Error(errorstr);
            }

            if (scn_head.Length == SCN_HEAD_BYTENO)
            {
                this.m_SCN_HEAD = scn_head;
            }
            else
            {
                string errorstr = "The Length of SCN_HEAD is not " + SCN_HEAD_BYTENO + " in " + thisMethod;
                Console.WriteLine(errorstr);
                _logger.Error(errorstr);
            }
            this.m_SCN_STS = scn_sts;
            this.m_PLC_IDX = plc_idx;
            return true;
        }

        protected override bool AnalyseFieldData()
        {
            throw new NotImplementedException();
        }

        public override string ShowAllData()
        {
            string showstr = "";
            showstr += "Telegram:" + this.TelegramAlias + "  Name:" + this.TelegramName + "\n";
            showstr += "TLG_TYPE:" + this.TLG_TYPE + " ";
            showstr += "TLG_LENGTH:" + this.TLG_LEN + " ";
            showstr += "TLG_SEQ:" + this.TLG_SEQ + " ";
            showstr += FDN_GID_MSB + ":" + this.GID_MSB + ' ';
            showstr += FDN_GID_LSB + ":" + this.GID_LSB + ' ';
            showstr += FDN_LOCATION + ":" + this.LOCATION + ' ';
            showstr += FDN_LIC1 + ":" + this.LIC_1 + ' ';
            showstr += FDN_LIC2 + ":" + this.LIC_2 + ' ';
            showstr += FDN_SCN_HEAD + ":" + this.SCN_HEAD + ' ';
            showstr += FDN_SCN_STS + ":" + this.SCN_STS + ' ';
            showstr += FDN_PLC_IDX + ":" + this.PLC_IDX + ' ';
            
            return showstr;
        }

        protected override bool HasAllData()
        {
            return (this.m_GID_MSB > 0)
                && (this.m_GID_LSB > 0)
                && (this.m_location > 0)
                && (this.m_LIC1 != null && this.m_LIC1.Length == LP_LENGTH)
                && (this.m_LIC2 != null && this.m_LIC2.Length == LP_LENGTH)
                && (this.m_SCN_HEAD != null && this.m_SCN_HEAD.Length == SCN_HEAD_BYTENO)
                && (this.m_SCN_STS > 0)
                && (this.m_PLC_IDX > 0);
        }

        protected override bool FillFieldData()
        {
            byte[] b_gidmsb = new byte[1] { this.m_GID_MSB };
            // BitConverter.GetBytes returns the byte array which is from low byte to hight byte
            byte[] b_gidlsb = Util.Reverse(BitConverter.GetBytes(this.m_GID_LSB));
            byte[] b_location = Util.Reverse(BitConverter.GetBytes(this.m_location));
            byte[] b_lic1 = Encoding.Default.GetBytes(this.m_LIC1);
            byte[] b_lic2 = Encoding.Default.GetBytes(this.m_LIC2);
            byte[] b_scnhead = this.m_SCN_HEAD;
            byte[] b_scnsts = new byte[1] { this.m_SCN_STS };
            byte[] b_plcidx = Util.Reverse(BitConverter.GetBytes(this.m_PLC_IDX));

            int datafield_length
                = b_gidmsb.Length
                + b_gidlsb.Length
                + b_location.Length
                + b_lic1.Length
                + b_lic2.Length
                + b_scnhead.Length
                + b_scnsts.Length
                + b_plcidx.Length;

            // create raw data template
            CreateRawdataTemplate(datafield_length);

            bool result = true;
            result &= SetFieldValue(FDN_GID_MSB, b_gidmsb);
            result &= SetFieldValue(FDN_GID_LSB, b_gidlsb);
            result &= SetFieldValue(FDN_LOCATION, b_location);
            result &= SetFieldValue(FDN_LIC1, b_lic1);
            result &= SetFieldValue(FDN_LIC2, b_lic2);
            result &= SetFieldValue(FDN_SCN_HEAD, b_scnhead);
            result &= SetFieldValue(FDN_SCN_STS, b_scnsts);
            result &= SetFieldValue(FDN_PLC_IDX, b_plcidx);

            return result;
        }
        #endregion
    }
}
