using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using PALS.Utilities;

namespace BHS.PLCSimulator
{
    // (0003) GID USED
    public class GID_Telegram : SAC2PLCTelegram
    {
        #region Class Field and Property
        // Class const variable
        private const string m_aliasname = "GID";

        // fdn - field name
        private const string FDN_GID_MSB = "GID_MSB";
        private const string FDN_GID_LSB = "GID_LSB";
        private const string FDN_LOCATION = "LOCATION";
        private const string FDN_TYPE = "TYPE";

        // Real Value Length
        private const int LEN_GID_MSB = 2;
        private const int LEN_GID_LSB = 8;
        private const int LEN_LOCATION = 5;
        private const int LEN_TYPE = 1;


        // private variable -- Field Value
        private byte m_GID_MSB = 0;
        private uint m_GID_LSB = 0;
        private ushort m_location = 0;
        private byte m_Type = 0;

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
                return m_GID_MSB.ToString("D"+LEN_GID_MSB.ToString());
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
                    this.m_GID_LSB = Convert.ToUInt32(value);
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
                    this.m_location = Convert.ToUInt16(value);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }

        public string TYPE
        {
            get
            {
                //return this.m_Type.ToString();
                return m_Type.ToString("D" + LEN_TYPE.ToString());
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_Type = Convert.ToByte(value);
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

        public GID_Telegram(byte[] rawdata)
            : base(rawdata, m_aliasname)
        {
            Console.WriteLine("ICR_Telegram Constructor with byte[] rawdata");
            this.TelegramDecoding();
        }

        public GID_Telegram(byte[] rawdata, string channel)
            : base(rawdata, channel, m_aliasname)
        {
        }

        public GID_Telegram()
            : base(m_aliasname)
        {
        }

        public GID_Telegram
            (byte gid_msb, uint gid_lsb, ushort location, byte type)
            : base(m_aliasname)
        {
            SetICRData(gid_msb, gid_lsb, location, type);
            byte[] Telegram_datafield = this.TelegramEncoding();
        }

        public GID_Telegram
            (string gid_msb, string gid_lsb, string location, string type)
            : base(m_aliasname)
        {
            SetICRData(gid_msb, gid_lsb, location, type);
            byte[] Telegram_datafield = this.TelegramEncoding();
        }

        public GID_Telegram(XmlNode xml_TeleTestcase)
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
                string type = XMLConfig.GetSettingFromInnerText(xml_TeleTestcase, FDN_TYPE, "2");
                SetICRData(gid_msb, gid_lsb, location, type);
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
            (string gid_msb, string gid_lsb, string location, string type)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            try
            {
                this.GID_MSB = gid_msb;
                this.GID_LSB = gid_lsb;
                this.LOCATION = location;
                this.TYPE = type;
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
            (byte gid_msb, uint gid_lsb, ushort location, byte type)
        {
            this.m_GID_MSB = gid_msb;
            this.m_GID_LSB = gid_lsb;
            this.m_location = location;
            this.m_Type = type;
            return true;
        }

        protected override bool AnalyseFieldData()
        {
            throw new NotImplementedException();
        }

        public override string ShowAllData()
        {
            string showstr = "";
            showstr += "Telegram:" + this.TelegramAlias + "  Name:" + this.TelegramName + " ";
            showstr += "TLG_TYPE:" + this.TLG_TYPE + " ";
            showstr += "TLG_LENGTH:" + this.TLG_LEN + " ";
            showstr += "TLG_SEQ:" + this.TLG_SEQ + " ";
            showstr += FDN_GID_MSB + ":" + this.GID_MSB + ' ';
            showstr += FDN_GID_LSB + ":" + this.GID_LSB + ' ';
            showstr += FDN_LOCATION + ":" + this.LOCATION + ' ';
            showstr += FDN_TYPE + ":" + this.TYPE + ' ';
            return showstr;
        }

        protected override bool HasAllData()
        {
            return (this.m_GID_MSB > 0)
                && (this.m_GID_LSB > 0)
                && (this.m_location > 0)
                && (this.m_Type > 0);
        }

        protected override bool FillFieldData()
        {
            byte[] b_gidmsb = new byte[1] { this.m_GID_MSB };
            // BitConverter.GetBytes returns the byte array which is from low byte to hight byte
            byte[] b_gidlsb = Util.Reverse(BitConverter.GetBytes(this.m_GID_LSB));
            byte[] b_location = Util.Reverse(BitConverter.GetBytes(this.m_location));
            byte[] b_type = new byte[1] { this.m_Type };


            int datafield_length
                = b_gidmsb.Length
                + b_gidlsb.Length
                + b_location.Length
                + b_type.Length;

            // create raw data template
            CreateRawdataTemplate(datafield_length);

            bool result = true;
            result &= SetFieldValue(FDN_GID_MSB, b_gidmsb);
            result &= SetFieldValue(FDN_GID_LSB, b_gidlsb);
            result &= SetFieldValue(FDN_LOCATION, b_location);
            result &= SetFieldValue(FDN_TYPE, b_type);

            return result;
        }
        #endregion
    }
}
