using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BHS.PLCSimulator.Messages.TelegramFormat
{
    public class FieldValue
    {
        #region Class Field and Property

        private string m_fieldname;
        private string m_datatype;
        private int m_showlength;
        private string m_strvalue;
        private byte[] m_bytevalue;
        private int m_length;

        public string FieldName
        {
            get
            {
                return this.m_fieldname;
            }
            //set
            //{
            //    this.m_fieldname = value;
            //}
        }

        public string DataType
        {
            get
            {
                return this.m_datatype;
            }
            //set
            //{
            //    this.m_datatype = value;
            //}
        }

        public int ShowLength
        {
            get
            {
                return this.m_showlength;
            }
            //set
            //{
            //    this.m_showlength = Convert.ToInt32(value);
            //}
        }

        public string StringValue
        {
            get
            {
                return this.m_strvalue;
            }
            set
            {
                if (value.Length != this.m_length && this.DataType == "string")
                    this.m_strvalue = new string(Util.CharPad(value.ToCharArray(), this.m_length));
                else
                    this.m_strvalue = value;
            }
        }

        public byte[] ByteValue
        {
            get
            {
                return this.m_bytevalue;
            }
            set
            {
                this.m_bytevalue = value;
            }
        }

        public string Length
        {
            get
            {
                return this.m_length.ToString();
            }
            //set
            //{
            //    this.m_length = Convert.ToInt32(value);
            //}
        }

        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region General_Telegram Constructor, Dispose, Finalize and Destructor

        public FieldValue(string fieldname, string datatype, string showlength, string length)
        {
            string errorstr = "";
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            try
            {
                this.m_fieldname = fieldname;
                this.m_datatype = datatype;
                this.m_showlength = Convert.ToInt32(showlength);
                this.m_length = Convert.ToInt32(length);

                if (!CheckDataType())
                {
                    errorstr += "Error in " + thisMethod + "\n";
                    errorstr += "DataType is not match with the length of field. DataType: " + this.DataType + ", Length: " + this.Length + "\n";
                    _logger.Error(errorstr);
                    throw new Exception(errorstr);
                }
            }
            catch (Exception exp)
            {
                errorstr += "Error in " + thisMethod + "\n" + exp.ToString();
                _logger.Error(errorstr);
            }

        }

        #endregion

        #region Member Function

        private bool CheckDataType()
        {
            bool chkres = false;
            switch (this.m_datatype)
            {
                case "byte":
                    if (this.m_length == sizeof(byte))
                        chkres = true;
                    break;
                case "uint":
                    if (this.m_length == sizeof(uint))
                        chkres = true;
                    break;
                case "ushort":
                    if (this.m_length == sizeof(ushort))
                        chkres = true;
                    break;
                case "string":
                    chkres = true;
                    break;
                case "binary":
                    chkres = true;
                    break;
                default:
                    break;
            }

            return chkres;
        }

        public bool FieldValue_StrToByte()
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errorstr = "Field:" + this.FieldName + ". Field StrValue:" + this.StringValue + ".\n";

            if (this.m_strvalue == null || this.m_strvalue.Length == 0)
            {
                errorstr += "Error in " + thisMethod + "Not assign string value to convert\n";
                _logger.Error(errorstr);
                return false;
            }


            try
            {
                if (CheckDataType())
                {
                    switch (this.m_datatype)
                    {
                        case "byte":
                            byte temp_byte;
                            temp_byte = Convert.ToByte(this.m_strvalue);
                            this.m_bytevalue = new byte[1] { temp_byte };
                            break;
                        case "uint":
                            uint temp_uint;
                            temp_uint = Convert.ToUInt32(this.m_strvalue);
                            this.m_bytevalue = Util.Reverse(BitConverter.GetBytes(temp_uint));
                            break;
                        case "ushort":
                            ushort temp_ushort;
                            temp_ushort = Convert.ToUInt16(this.m_strvalue);
                            this.m_bytevalue = Util.Reverse(BitConverter.GetBytes(temp_ushort));
                            break;
                        case "string":
                            this.m_bytevalue = Encoding.Default.GetBytes(this.m_strvalue);
                            break;
                        case "binary":
                            this.m_bytevalue = Util.HexByteStrToArray(this.m_strvalue);
                            break;
                        default:
                            errorstr += "Error in " + thisMethod + ".  Unknown DataType.\n";
                            throw new Exception(errorstr);
                            //return false;
                            //break;
                    }
                }
                else
                {
                    errorstr += "Error in " + thisMethod + "\n";
                    errorstr += "DataType is not match with the length of field. DataType: " + this.DataType + ", Length: " + this.Length + "\n";
                    _logger.Error(errorstr);
                    return false;
                }
            }
            catch (Exception exp)
            {
                errorstr += "Error in " + thisMethod + "\n";
                errorstr += exp.ToString();
                _logger.Error(errorstr);
                return false;
            }

            if (IsValidByteData())
                return true;
            else
            {
                errorstr += "Error in " + thisMethod + "\n";
                errorstr += "The length of byte array is not match with the defined length of field\n";
                _logger.Error(errorstr);
                return false;
            }
        }

        public bool FieldValue_ByteToStr()
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errorstr = "";

            if (this.m_bytevalue == null || this.m_bytevalue.Length == 0)
            {
                errorstr += "Error in " + thisMethod + "Not assign byte value to convert\n";
                _logger.Error(errorstr);
                return false;
            }

            try
            {
                if (CheckDataType())
                {
                    if (!IsValidByteData())
                    {
                        errorstr += "Error in " + thisMethod + "\n";
                        errorstr += "The length of byte array is not match with the defined length of field\n";
                        _logger.Error(errorstr);
                        return false;
                    }

                    switch (this.m_datatype)
                    {
                        case "byte":
                            byte temp_byte;
                            temp_byte = this.m_bytevalue[0];
                            this.m_strvalue = temp_byte.ToString("D" + this.m_showlength.ToString());
                            break;
                        case "uint":
                            uint temp_uint;
                            temp_uint = BitConverter.ToUInt32(Util.Reverse(this.m_bytevalue), 0);
                            this.m_strvalue = temp_uint.ToString("D" + this.m_showlength.ToString());
                            break;
                        case "ushort":
                            ushort temp_ushort;
                            temp_ushort = BitConverter.ToUInt16(Util.Reverse(this.m_bytevalue), 0);
                            this.m_strvalue = temp_ushort.ToString("D" + this.m_showlength.ToString());
                            break;
                        case "string":
                            this.m_strvalue = new string(Util.ByteArrayToCharArray(this.m_bytevalue));
                            break;
                        case "binary":
                            this.m_strvalue =  BitConverter.ToString(this.m_bytevalue);
                            break;
                        default:
                            errorstr += "Error in " + thisMethod + ".  Unknown DataType.";
                            throw new Exception(errorstr);
                            //return false;
                            //break;
                    }
                }
                else
                {
                    errorstr += "Error in " + thisMethod + "\n";
                    errorstr += "DataType is not match with the length of field. DataType: " + this.DataType + ", Length: " + this.Length;
                    _logger.Error(errorstr);
                    return false;
                }
            }
            catch (Exception exp)
            {
                errorstr += "Error in " + thisMethod + "\n";
                errorstr += exp.ToString();
                _logger.Error(errorstr);
                return false;
            }

            return true;
        }

        public bool IsValidByteData()
        {
            if (this.m_bytevalue != null)
                return this.m_bytevalue.Length == this.m_length;
            else
                return false;
        }

        #endregion
    }
}
