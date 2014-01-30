using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;//SizeOf


namespace BHS.PLCSimulator
{
    public abstract class SAC2PLCTelegram : AbstractTelegram
    {
        #region Class Field and Property
        // Telegram Header
        private const string FDN_TelegramType = "Type";
        private const string FDN_TelegramLength = "Length";
        private const string FDN_TelegramSeqNo = "Sequence";
        private const string FD_ATTRIBUTE_DEFAULT = "default";

        protected string m_TelegramAlias;
        protected string m_TelegramName;
        protected char[] m_TelegramType;
        protected char[] m_TelegramLength;
        protected int m_TelegramSeqNo;
        private static int Seed_TelegramSeqNo = 1;
        private static Object SeedSeqnoLock = new Object();

        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Public Property
        public string TelegramAlias
        {
            get
            {
                return m_TelegramAlias;
            }
        }

        public string TelegramName
        {
            get
            {
                return m_TelegramName;
            }
        }

        public string TLG_TYPE
        {
            get
            {
                return new string(this.m_TelegramType);
            }
        }

        public string TLG_LEN
        {
            get
            {
                return new string(this.m_TelegramLength);
            }
        }

        public string TLG_SEQ
        {
            get
            {
                return this.m_TelegramSeqNo.ToString();
            }
        }
        
        #endregion

        #region SAC2PLCTelegram Constructor, Dispose, Finalize and Destructor

        public SAC2PLCTelegram(string tel_Alias)
            : base(tel_Alias)
        {
            Init_SAC2PLCTelegram(tel_Alias);
        }

        public SAC2PLCTelegram(byte[] data, string tel_Alias)
            : base(data, tel_Alias)
        {
            Init_SAC2PLCTelegram(tel_Alias);
            Console.WriteLine("SAC2PLCTelegram Constructor with byte[] rawdata");
            //this.TelegramDecoding();
        }

        public SAC2PLCTelegram(byte[] data, string channel, string tel_Alias)
            : base(data, channel, tel_Alias)
        {
            Init_SAC2PLCTelegram(tel_Alias);
            Console.WriteLine("SAC2PLCTelegram Constructor with byte[] rawdata");
            //this.TelegramDecoding();
        }

        #endregion

        private void Init_SAC2PLCTelegram(string tel_Alias)
        {
            m_TelegramType = new char[4];
            m_TelegramLength = new char[4];
            lock (SeedSeqnoLock)
            {
                m_TelegramSeqNo = Seed_TelegramSeqNo;
                Seed_TelegramSeqNo++;
            }

            if (TelegramTypeName.HasAlias(tel_Alias))
            {
                this.m_TelegramAlias = tel_Alias;
                this.m_TelegramType = TelegramTypeName.GetTypeByAlias(tel_Alias).ToCharArray();
                this.m_TelegramName = TelegramTypeName.GetNameByType(new string(m_TelegramType));
                if (this.m_TelegramType.Length > 4)
                {
                    Console.WriteLine("The Length of Telegram Type:" + m_TelegramType + " is larger than 4 bytes.");
                }
            }
            else
            {
                Console.WriteLine("Failed  SAC2PLCTelegram(string tel_type). No such Alias: " + tel_Alias);
            }
        }

        abstract protected bool AnalyseFieldData();
        abstract public string ShowAllData();

        private bool AnalyseTelegramData()
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errorstr = "";

            if (this.m_RawData == null)
            {
                errorstr += "Position: " + thisMethod + '\n';
                errorstr += "Telegram: " + this.m_TelegramAlias + '\n';
                errorstr += "The raw data of telegram is not assigned. So cannot get Telegram Type";
                Console.WriteLine(errorstr);
                _logger.Error(errorstr);
                return false;
            }
            else
            {
                // 1.Check the telegram type is right 
                // (the type from rawdata is equal to the type in this Class )
                byte[] by_teltype = this.GetFieldValue(FDN_TelegramType);
                char[] ch_teltype = Util.ByteArrayToCharArray(by_teltype);
                if (!ch_teltype.SequenceEqual(this.m_TelegramType))
                {
                    errorstr += "Position: " + thisMethod + '\n';
                    errorstr += "Telegram: " + this.m_TelegramAlias + '\n';
                    errorstr += "The telegram type is not match. Tyte in rawdata:" + ch_teltype.ToString() + " Type in Class:" + this.m_TelegramType;
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                    return false;
                }

                // 2.If the default length of telegram is not "?", then check whether the length 
                // in rawdata field is equal to the default length
                byte[] by_length = this.GetFieldValue(FDN_TelegramLength);
                string default_length = this.m_TelFormat.GetAttributeValue(FDN_TelegramLength, FD_ATTRIBUTE_DEFAULT);
                if (default_length != "?")
                {
                    byte[] byte_defaul_length = Util.DecByteStrToArray(default_length);
                    if (!by_length.SequenceEqual(byte_defaul_length))
                    {
                        errorstr += "Position: " + thisMethod + '\n';
                        errorstr += "Telegram: " + this.m_TelegramAlias + '\n';
                        errorstr += "The telegram length is not match with default length. Length in rawdata:" + BitConverter.ToString(by_length) + " Default Length in Class:" + BitConverter.ToString(byte_defaul_length);
                        Console.WriteLine(errorstr);
                        _logger.Error(errorstr);
                        return false;
                    }
                }

                char[] tel_length = Util.ByteArrayToCharArray(by_length);

                // 3. Check whether the data length in field is the length of the rawdata
                int reallength = Convert.ToInt32(new string(tel_length));
                if (reallength != this.m_RawData.Length)
                {
                    errorstr += "Position: " + thisMethod + '\n';
                    errorstr += "Telegram: " + this.m_TelegramAlias + '\n';
                    errorstr += "The length in data field does not match the real length of rawdata" + " Real length in rawdata:" + this.m_RawData.Length + " Length in Field:" + reallength;
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                    return false;
                }

                this.m_TelegramLength = tel_length;

                // 4. Get field value of sequence number
                byte[] by_seqno = this.GetFieldValue(FDN_TelegramSeqNo);
                this.m_TelegramSeqNo = BitConverter.ToInt32(Util.Reverse(by_seqno), 0);

                return true;
            }
        }

        public override void TelegramDecoding()
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errorstr = "";

            if (AnalyseTelegramData())
            {
                if (AnalyseFieldData())
                {
                    string infostr = "Telegram:" + this.m_TelegramAlias + " decoding Complete!";
                }
                else
                {
                    errorstr += "Position: " + thisMethod + '\n';
                    errorstr += "Telegram: " + this.m_TelegramAlias + '\n';
                    errorstr += "AnalyseFieldData() Failed\n";
                }
            }
            else
            {
                errorstr += "Position: " + thisMethod + '\n';
                errorstr += "Telegram: " + this.m_TelegramAlias + '\n';
                errorstr += "AnalyseTelegramData() Failed\n";
            }

            if (errorstr.Length != 0)
            {
                Console.WriteLine(errorstr);
                _logger.Error(errorstr);
            }

        }

        abstract protected  bool HasAllData();
        abstract protected bool FillFieldData();

        private bool FillTelegramData()
        {
            string errorstr = "";
            int tellength;
            if (m_RawData != null)
            {
                tellength = m_RawData.Length;
            }
            else
            {
                errorstr += "RawData Template is not created. Type:" + this.m_TelegramType + " Alias:" + this.m_TelFormat.Alias + '\n';
                errorstr += "Create Telegram Failed. RawData Template is not created.\n";
                Console.WriteLine(errorstr);
                _logger.Error(errorstr);
                return false;
            }

            // "D4" means the length of string is 4, 
            // and the number is padded with "0" to its left if nessary.
            this.m_TelegramLength = tellength.ToString("D4").ToCharArray();

            bool result = true;

            byte[] b_TelType = Encoding.Default.GetBytes(this.m_TelegramType);
            byte[] b_TelLength = Encoding.Default.GetBytes(this.m_TelegramLength);
            byte[] b_TelSeqno = Util.Reverse(BitConverter.GetBytes(m_TelegramSeqNo));

            result &= SetFieldValue(FDN_TelegramType, b_TelType);
            result &= SetFieldValue(FDN_TelegramLength, b_TelLength);
            result &= SetFieldValue(FDN_TelegramSeqNo, b_TelSeqno);

            Console.WriteLine("General Telegram Encoding Complete!");

            return result;
        }

        public override byte[] TelegramEncoding()
        {
            this.m_RawData = null;
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errorstr = "";

            if (HasAllData())
            {
                if (FillFieldData())
                {
                    if (FillTelegramData())
                    {
                        Console.WriteLine("Telegram:" + this.m_TelegramAlias + " encoding Complete!");
                        _logger.Info("Telegram:" + this.m_TelegramAlias + " encoding Complete!");
                    }
                    else
                    {
                        errorstr = "Telegram:" + this.m_TelegramAlias + "  FillTelegramData failed.";
                    }
                }
                else
                {
                    errorstr = "Telegram:" + this.m_TelegramAlias + " FillFieldData failed.";
                }
            }
            else
            {
                errorstr = "Telegram:" + this.m_TelegramAlias + " do not have complete data.";
            }

            if (errorstr.Length != 0)
            {
                _logger.Error(errorstr);
                throw new Exception(errorstr);
            }
            return m_RawData;
        }

        protected bool CreateRawdataTemplate(int data_length)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errorstr = "";

            int telegram_length
                = this.m_TelegramType.Length
                + this.m_TelegramLength.Length
                + Marshal.SizeOf(m_TelegramSeqNo)
                + data_length;

            // Check the default length
            string default_length_str = this.m_TelFormat.GetAttributeValue(FDN_TelegramLength, FD_ATTRIBUTE_DEFAULT);
            if (default_length_str != "?")
            {
                int default_length = Convert.ToInt32(Util.DecByteStrToStr(default_length_str));
                if (!(telegram_length == default_length))
                {
                    errorstr += "Position: " + thisMethod + '\n';
                    errorstr += "Telegram: " + this.m_TelegramAlias + '\n';
                    errorstr += "The telegram length is not match with default length. Telegram Length :" + telegram_length.ToString() + " Default Length:" + default_length.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                    return false;
                }
            }
            this.RawData = new byte[telegram_length];
            return true;
        }

        

    }
}
