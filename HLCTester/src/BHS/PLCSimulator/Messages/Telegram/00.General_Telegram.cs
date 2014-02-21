using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using System.Xml;
using PALS.Utilities;

using BHS.PLCSimulator.Messages.TelegramFormat;

namespace BHS.PLCSimulator.Messages.Telegram
{
    // new way to implement the telegram
    public class General_Telegram : SAC2PLCTelegram
    {
        #region Class Field and Property

        public Hashtable HT_FieldValueList;

        private const string XML_TCS_TELEGRAM = "telegram";
        private const string FDN_TelegramType = "Type";
        private const string FDN_TelegramLength = "Length";
        private const string FDN_TelegramSeqNo = "Sequence";
        private const string FDN_GID_MSB = "GID_MSB";
        private const string FDN_GID_LSB = "GID_LSB";

        private string m_aliasname;
        private XmlNode m_xmltestcase;
        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region General_Telegram Constructor, Dispose, Finalize and Destructor

        public General_Telegram(XmlNode xml_TeleTestcase, string tel_aliasname)
            : base(tel_aliasname)
        {
            this.m_aliasname = tel_aliasname;
            this.m_xmltestcase = xml_TeleTestcase;

            HT_FieldValueList = new Hashtable();

            try
            {
                // Initilize the HT_FieldValueList
                foreach (DictionaryEntry fielditem in this.m_TelFormat.HT_FieldList)
                {
                    FieldFormat field = fielditem.Value as FieldFormat;
                    if (field.FieldName != FDN_TelegramType && field.FieldName != FDN_TelegramLength && field.FieldName != FDN_TelegramSeqNo)
                    {
                        FieldValue new_fvalue = new FieldValue(field.FieldName, field.DataType, field.ShowLength, field.FieldLength);
                        HT_FieldValueList.Add(field.FieldName, new_fvalue);
                    }
                }

                this.InitializeTelegram();

                //Encoding the telegram
                //this.TelegramEncoding();
            }
            catch (Exception exp)
            {
                _logger.Error(exp.ToString());
                throw exp;
            }

        }

        public General_Telegram(byte[] data, string tel_aliasname)
            : base(data, tel_aliasname)
        {
            this.m_aliasname = tel_aliasname;

            HT_FieldValueList = new Hashtable();

            try
            {
                // Initilize the HT_FieldValueList
                foreach (DictionaryEntry fielditem in this.m_TelFormat.HT_FieldList)
                {
                    FieldFormat field = fielditem.Value as FieldFormat;
                    if (field.FieldName != FDN_TelegramType && field.FieldName != FDN_TelegramLength && field.FieldName != FDN_TelegramSeqNo)
                    {
                        FieldValue new_fvalue = new FieldValue(field.FieldName, field.DataType, field.ShowLength, field.FieldLength);
                        HT_FieldValueList.Add(field.FieldName, new_fvalue);
                    }
                }

                this.TelegramDecoding();
            }
            catch (Exception exp)
            {
                _logger.Error(exp.ToString());
                throw exp;
            }
        }



        #endregion

        #region Member Function

        public bool InitializeTelegram()
        {
            string errorstr = "Telegram: " + m_aliasname + ". ";
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            try
            {
                // read each field name and field name of the telegram, and store them into HT_FieldValueList
                foreach (XmlNode tempnode in this.m_xmltestcase.ChildNodes)
                {
                    // The names of xmlnodes in CFG_TestCase.xml is the filed name of telegram,
                    // which must be same with (match with) the filed name in CFG_Telegrams.xml
                    if (tempnode.NodeType != XmlNodeType.Comment)
                    {
                        if (HT_FieldValueList.ContainsKey(tempnode.Name))
                        {
                            ((FieldValue)HT_FieldValueList[tempnode.Name]).StringValue = tempnode.InnerText;
                        }
                        else
                        {
                            errorstr += "Unknown Filed: " + tempnode.Name + "\n";
                            throw new Exception(errorstr);
                        }

                    }
                }

                // If GID_MSB or GID_LSB is "?", then assign a new GID
                if (HT_FieldValueList.ContainsKey(FDN_GID_LSB) && HT_FieldValueList.ContainsKey(FDN_GID_MSB))
                {
                    FieldValue fdval_GIDMSB = HT_FieldValueList[FDN_GID_MSB] as FieldValue;
                    FieldValue fdval_GIDLSB = HT_FieldValueList[FDN_GID_LSB] as FieldValue;

                    if (fdval_GIDLSB.StringValue == "?" || fdval_GIDMSB.StringValue == "?")
                    {
                        char[] gid = Util.GetGID();
                        ((FieldValue)HT_FieldValueList[FDN_GID_MSB]).StringValue = new string(gid, 0, fdval_GIDMSB.ShowLength);
                        ((FieldValue)HT_FieldValueList[FDN_GID_LSB]).StringValue = new string(gid, fdval_GIDMSB.ShowLength, fdval_GIDLSB.ShowLength);
                    }
                }

                
            }
            catch (Exception exp)
            {
                errorstr = "Error in " + thisMethod + "\n" + exp.ToString();
                _logger.Error(errorstr);
                return false;
            }

            return true;
        }

        protected override bool AnalyseFieldData()
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errorstr = "Error in " + thisMethod + "\n";
            errorstr += "Telegram: " + m_aliasname + ". ";

            bool chkres = true;

            try
            {
                foreach (DictionaryEntry de in HT_FieldValueList)
                {
                    ((FieldValue)de.Value).ByteValue = this.GetFieldValue(de.Key.ToString());
                    chkres &= ((FieldValue)de.Value).FieldValue_ByteToStr();
                }
            }
            catch (Exception exp)
            {
                errorstr += exp.ToString();
                _logger.Error(errorstr);
            }

            return chkres;
        }

        protected override bool FillFieldData()
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errorstr = "Error in " + thisMethod + "\n";
            errorstr += "Telegram: " + m_aliasname + ". ";

            int datafield_length = 0;
            bool chkres = true;
            FieldValue fdval;

            try
            {
                foreach (DictionaryEntry de in HT_FieldValueList)
                {
                    chkres &= ((FieldValue)de.Value).FieldValue_StrToByte();
                    datafield_length += ((FieldValue)de.Value).ByteValue.Length;
                }

                if (chkres == true)
                {
                    chkres &= CreateRawdataTemplate(datafield_length);

                    foreach (DictionaryEntry de in HT_FieldValueList)
                    {
                        fdval = de.Value as FieldValue;
                        chkres &= this.SetFieldValue(de.Key.ToString(), fdval.ByteValue);
                    }

                }
                else
                {
                    errorstr += "Conversion from string to byte of the field value is failed.\n";
                    _logger.Error(errorstr);
                }
            }
            catch (Exception exp)
            {
                errorstr += exp.ToString();
                _logger.Error(errorstr);
            }

            return chkres;
        }

        public string ShowFieldValue(string fieldname)
        {
            string errorstr = "Telegram: " + m_aliasname + ". ";
            if (HT_FieldValueList.ContainsKey(fieldname))
            {
                FieldValue temp_fdval = HT_FieldValueList[fieldname] as FieldValue;
                return temp_fdval.FieldName + ":" + temp_fdval.StringValue;
            }
            else
            {
                errorstr += "No such field: " + fieldname + "to show its value";
                return null;
            }
        }

        public override string ShowAllData()
        {
            FieldValue fdval;
            string showstr = "";
            showstr += "Telegram:" + this.TelegramAlias + "  Name:" + this.TelegramName + "\r\n";
            showstr += "TLG_TYPE:" + this.TLG_TYPE + "\r\n";
            showstr += "TLG_LENGTH:" + this.TLG_LEN + "\r\n";
            showstr += "TLG_SEQ:" + this.TLG_SEQ + "\r\n";
            foreach (DictionaryEntry de in HT_FieldValueList)
            {
                fdval = de.Value as FieldValue;
                showstr += fdval.FieldName + ":" + fdval.StringValue + "\r\n";
            }
            Console.WriteLine(showstr);
            return showstr; ;
        }

        protected override bool HasAllData()
        {
            bool chkres = true;
            foreach (DictionaryEntry de in HT_FieldValueList)
            {
                chkres &= (((FieldValue)de.Value).StringValue != null);
            }
            return chkres;
        }
        #endregion
    }
}
