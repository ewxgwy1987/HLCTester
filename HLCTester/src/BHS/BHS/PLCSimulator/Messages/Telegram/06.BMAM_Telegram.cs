using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BHS.PLCSimulator
{
    //(0006) Baggage Measurement Array Message
    class BMAM_Telegram : SAC2PLCTelegram
    {
        #region Class Field and Property
        // Class const variable
        private const string m_aliasname = "BMAM";

        // fdn - field name
        private const string FDN_GID_MSB = "GID_MSB";
        private const string FDN_GID_LSB = "GID_LSB";
        private const string FDN_LOCATION = "LOCATION";
        private const string FDN_LENGTH = "LENGTH";
        private const string FDN_WIDTH = "WIDTH";
        private const string FDN_HEIGHT = "HEIGHT";
        private const string FDN_BDDTYPE = "TYPE";

        // private variable -- Field Value
        private byte m_GID_MSB = 0;
        private uint m_GID_LSB = 0;
        private ushort m_location = 0;
        private ushort m_length = 0;
        private ushort m_width = 0;
        private ushort m_height = 0;
        private byte m_bddtype = 0;

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
                return m_GID_MSB.ToString();
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
                return m_GID_LSB.ToString();
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
                return m_location.ToString();
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

        public string LENGTH
        {
            get
            {
                return m_length.ToString();
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_length = Convert.ToUInt16(value);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }
        
        public string WIDTH
        {
            get
            {
                return m_width.ToString();
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_width = Convert.ToUInt16(value);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }

        public string HEIGHT
        {
            get
            {
                return m_height.ToString();
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_height = Convert.ToUInt16(value);
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
                return m_bddtype.ToString();
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_bddtype = Convert.ToByte(value);
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

        public BMAM_Telegram(byte[] rawdata)
            : base(rawdata, m_aliasname)
        {
            Console.WriteLine("ICR_Telegram Constructor with byte[] rawdata");
            this.TelegramDecoding();
        }

        public BMAM_Telegram(byte[] rawdata, string channel)
            : base(rawdata, channel, m_aliasname)
        {
        }

        public BMAM_Telegram()
            : base(m_aliasname)
        {
        }

        public BMAM_Telegram
            (byte gid_msb, uint gid_lsb, ushort location, ushort length, ushort width, ushort height, byte type)
            : base(m_aliasname)
        {
            SetICRData(gid_msb, gid_lsb, location, length, width, height, type);
            byte[] Telegram_datafield = this.TelegramEncoding();
        }

        public BMAM_Telegram
            (string gid_msb, string gid_lsb, string location, string length, string width, string height, string type)
            : base(m_aliasname)
        {
            SetICRData(gid_msb, gid_lsb, location, length, width, height, type);
            byte[] Telegram_datafield = this.TelegramEncoding();
        }
        #endregion

        #region Member Function
        public bool SetICRData
            (string gid_msb, string gid_lsb, string location, string length, string width, string height, string type)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            try
            {
                this.GID_MSB = gid_msb;
                this.GID_LSB = gid_lsb;
                this.LOCATION = location;
                this.LENGTH = length;
                this.WIDTH = width;
                this.HEIGHT = height;
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
            (byte gid_msb, uint gid_lsb, ushort location, ushort length, ushort width, ushort height, byte type)
        {
            this.m_GID_MSB = gid_msb;
            this.m_GID_LSB = gid_lsb;
            this.m_location = location;
            this.m_length = length;
            this.m_width = width;
            this.m_height = height;
            this.m_bddtype = type;
            return true;
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
            return (this.m_GID_MSB > 0)
                && (this.m_GID_LSB > 0)
                && (this.m_location > 0)
                && (this.m_length > 0)
                && (this.m_width > 0)
                && (this.m_height > 0)
                && (this.m_bddtype > 0);
        }

        protected override bool FillFieldData()
        {
            byte[] b_gidmsb = new byte[1] { this.m_GID_MSB };
            // BitConverter.GetBytes returns the byte array which is from low byte to hight byte
            byte[] b_gidlsb = Util.Reverse(BitConverter.GetBytes(this.m_GID_LSB));
            byte[] b_location = Util.Reverse(BitConverter.GetBytes(this.m_location));
            byte[] b_length = Util.Reverse(BitConverter.GetBytes(this.m_length));
            byte[] b_width = Util.Reverse(BitConverter.GetBytes(this.m_width));
            byte[] b_height = Util.Reverse(BitConverter.GetBytes(this.m_height));
            byte[] b_bddtype = new byte[] { this.m_bddtype };

            int datafield_length
                = b_gidmsb.Length
                + b_gidlsb.Length
                + b_location.Length
                + b_length.Length
                + b_width.Length
                + b_height.Length
                + b_bddtype.Length;

            // create raw data template
            CreateRawdataTemplate(datafield_length);

            bool result = true;
            result &= SetFieldValue(FDN_GID_MSB, b_gidmsb);
            result &= SetFieldValue(FDN_GID_LSB, b_gidlsb);
            result &= SetFieldValue(FDN_LOCATION, b_location);
            result &= SetFieldValue(FDN_LENGTH, b_length);
            result &= SetFieldValue(FDN_WIDTH, b_width);
            result &= SetFieldValue(FDN_HEIGHT, b_height);
            result &= SetFieldValue(FDN_BDDTYPE, b_bddtype);

            return result;
        }
        #endregion
    }
}
