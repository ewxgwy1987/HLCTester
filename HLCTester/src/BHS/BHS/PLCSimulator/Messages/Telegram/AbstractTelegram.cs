using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BHS.PLCSimulator
{
    public abstract class AbstractTelegram
    {
        #region Class Field and Property

        private const string FD_ATTRIBUTE_OFFSET = "offset";
        private const string FD_ATTRIBUTE_LENGTH = "length";
        private const string FD_ATTRIBUTE_DEFAULT = "default";
        private const string ATTR_DEFAULT = "?";

        protected byte[] m_RawData;
        protected string m_ChannelName;
        protected TelegramFormat m_TelFormat;

        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string ChannelName
        {
            get
            {
                return this.m_ChannelName;
            }
            set
            {
                this.m_ChannelName = value;
            }
        }

        public byte[] RawData
        {
            get
            {
                return this.m_RawData;
            }
            set
            {
                this.m_RawData = value;
            }
        }

        #endregion

        #region AbstractTelegram Constructor, Dispose, Finalize and Destructor

        public AbstractTelegram(string tel_Alias)
        {
            Init_AbstractTelegram(tel_Alias);
        }

        public AbstractTelegram(byte[] data, string tel_Alias)
        {
            Init_AbstractTelegram(tel_Alias);
            Console.WriteLine("AbstractTelegram Constructor with byte[] rawdata");
            this.m_RawData = data;
        }

        public AbstractTelegram(byte[] rawdata, string channelname, string tel_Alias)
        {
            Init_AbstractTelegram(tel_Alias);
            SetRawTelegramData(rawdata, channelname);
        }
        #endregion

        private void Init_AbstractTelegram(string tel_Alias)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            if (TelegramTypeName.HasAlias(tel_Alias))
            {
                this.m_TelFormat = TelegramFormatList.GetTelegramFormat(tel_Alias);
            }
            else
            {
                Console.WriteLine("Failed " + thisMethod + ". No such Alias: " + tel_Alias);
            }
            if (this.m_TelFormat == null)
            {
                Console.WriteLine("AbstractTelegram - No such alias: " + tel_Alias);
            }
            this.m_ChannelName = null;
            this.m_RawData = null;
        }

        public bool SetRawTelegramData(byte[] rawdata, string channelname)
        {
            this.m_RawData = rawdata;
            this.m_ChannelName = channelname;
            Console.WriteLine("AbstractTelegram Complete!");
            return true;
        }

        public override string ToString()
        {
            if (this.m_RawData == null || this.m_RawData.Length == 0)
                return null;
            else
            {

                char[] str_rawdata = new char[this.m_RawData.Length];
                for (int i = 0; i < this.m_RawData.Length; i++)
                {
                    str_rawdata[i] = Convert.ToChar(this.m_RawData[i]);
                }
                return new string(str_rawdata);
            }
        }

        public string GetHexString()
        {
            string str_rawdata = "";
            // StringBuilder
            if ((this.m_RawData == null) || (this.m_RawData.Length == 0))
                Console.WriteLine("rawdata is <none>");
            else
            {
                for (int i = 0; i < this.m_RawData.Length; i++)
                {
                    //Console.Write("{0:X2} ", this.m_RawData[i]);
                    str_rawdata += "<" + this.m_RawData[i].ToString("X2") + ">";
                }
                //Console.WriteLine();
            }
            return str_rawdata;
        }

        protected byte[] GetFieldValue(string fieldname)
        {
            byte[] fieldvalue;
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errorstr = "";

            if (this.m_RawData == null)
            {
                errorstr += "Position: " + thisMethod + '\n';
                errorstr += "TelegramFormat: " + this.m_TelFormat.Alias;
                errorstr += "The raw data of telegram is not assigned. So cannot get filed value";
                Console.WriteLine(errorstr);
                _logger.Error(errorstr);
                return null;
            }

            if (this.m_TelFormat != null && this.m_TelFormat.HasField(fieldname))
            {
                int offset = 0, length = 0;
                string str_offset, str_length;

                str_offset = this.m_TelFormat.GetAttributeValue(fieldname, FD_ATTRIBUTE_OFFSET);
                str_length = this.m_TelFormat.GetAttributeValue(fieldname, FD_ATTRIBUTE_LENGTH);

                try
                {
                    offset = int.Parse(str_offset);
                    length = int.Parse(str_length);
                }
                catch (FormatException exp)
                {
                    errorstr += "Position: " + thisMethod + '\n';
                    errorstr += "TelegramFormat: " + this.m_TelFormat.Alias + " Filename:" + fieldname + '\n';
                    errorstr += "Not a integer. Offset: " + str_offset + " Length: " + str_length;
                    errorstr += exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                    return null;
                }
                catch (Exception exp)
                {
                    errorstr += "Position: " + thisMethod + '\n';
                    errorstr += " Filename:" + fieldname + '\n';
                    errorstr += exp.ToString() + '\n';
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                    return null;
                }

                if (offset + length - 1 > m_RawData.Length && length > 0)
                {
                    errorstr += "Position: " + thisMethod + '\n';
                    errorstr += "TelegramFormat: " + this.m_TelFormat.Alias + " Filename:" + fieldname + '\n';
                    errorstr += "The data length is out of range";
                    errorstr += "data offset: " + offset;
                    errorstr += "data length: " + length;
                    errorstr += "Rawdata length: " + m_RawData.Length;
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                    return null;
                }

                fieldvalue = new byte[length];
                Array.Copy(m_RawData, offset, fieldvalue, 0, length);
            }
            else if (this.m_TelFormat != null && !(this.m_TelFormat.HasField(fieldname)))
            {
                errorstr += "Position: " + thisMethod + '\n';
                errorstr += "Wrong Filed Name: " + fieldname;
                Console.WriteLine(errorstr);
                _logger.Error(errorstr);
                return null;
            }
            else
            {
                errorstr += "Position: " + thisMethod + '\n';
                errorstr += "TelegramFormat: " + this.m_TelFormat.Alias + " Filename:" + fieldname + '\n';
                errorstr += "No Telegram Format is assigned. <" + thisMethod + ">";
                Console.WriteLine(errorstr);
                _logger.Error(errorstr);
                return null;
            }

            return fieldvalue;
        }

        protected bool SetFieldValue(string fieldname, byte[] fieldvalue)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            if (this.m_TelFormat != null && this.m_TelFormat.HasField(fieldname))
            {
                int offset = 0, length = 0;
                string str_offset, str_length, defaultvalue;

                str_offset = this.m_TelFormat.GetAttributeValue(fieldname, FD_ATTRIBUTE_OFFSET);
                str_length = this.m_TelFormat.GetAttributeValue(fieldname, FD_ATTRIBUTE_LENGTH);
                defaultvalue = this.m_TelFormat.GetAttributeValue(fieldname, FD_ATTRIBUTE_DEFAULT);
                try
                {
                    offset = int.Parse(str_offset);
                    length = int.Parse(str_length);
                }
                catch (FormatException exp)
                {
                    Console.WriteLine("TelegramFormat: " + this.m_TelFormat.Alias);
                    Console.WriteLine("Not a integer. Offset: " + str_offset + " Length: " + str_length);
                    Console.WriteLine(exp.ToString());
                    return false;
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.ToString());
                    return false;
                }


                if (length != fieldvalue.Length)
                {
                    string errorstr = "The data length of filed:" + fieldname + "is not match";
                    errorstr += "TelegramFormat Length: " + length + "; Data Length: " + fieldvalue.Length + ";";
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                    return false;
                }

                if (offset + length - 1 > m_RawData.Length && length > 0)
                {
                    string errorstr = "The data length is out of range";
                    errorstr += "data offset: " + offset;
                    errorstr += "data length: " + length;
                    errorstr += "Rawdata length: " + m_RawData.Length;
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                    return false;
                }

                if (fieldvalue == null || fieldvalue.Length == 0)
                {
                    if (defaultvalue == ATTR_DEFAULT)
                    {
                        string errorstr = "Must Set Field Value. Telegram:" + this.m_TelFormat.Alias + " Field:" + fieldname;
                        Console.WriteLine(errorstr);
                        _logger.Error(errorstr);
                        return false;
                    }
                    else
                    {
                        fieldvalue = Util.DecByteStrToArray(defaultvalue);
                    }
                }

                Array.Copy(fieldvalue, 0, m_RawData, offset, length);
                return true;
            }
            else if (!this.m_TelFormat.HasField(fieldname))
            {
                string errorstr = "Wrong Filed Name: " + fieldname;
                Console.WriteLine(errorstr);
                _logger.Error(errorstr);
                return false;
            }
            else
            {
                string errorstr = "No Telegram Format is assigned. <" + thisMethod + ">";
                Console.WriteLine(errorstr);
                _logger.Error(errorstr);
                return false;
            }

        }


        abstract public void TelegramDecoding();

        abstract public byte[] TelegramEncoding();
    }
}
