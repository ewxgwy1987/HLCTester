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

        private const string XML_TCS_TELEGRAM = "telegram";

        private string m_aliasname;
        private Hashtable HT_FieldValueList;

        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region General_Telegram Constructor, Dispose, Finalize and Destructor

        public General_Telegram(XmlNode xml_TeleTestcase, string tel_aliasname)
            :base(tel_aliasname)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                // Initilize the HT_FieldValueList
                foreach (FieldFormat field in this.m_TelFormat.HT_FieldList)
                {
                    FieldValue new_fvalue = new FieldValue(field.FieldName, field.DataType, field.ShowLength, field.FieldLength);
                    HT_FieldValueList.Add(field.FieldName, new_fvalue);
                }
            }
            catch (Exception exp)
            {
                string errorstr = "Error in " + thisMethod + "\n" + exp.ToString();
                _logger.Error(errorstr);
            }
        }

        #endregion

        #region Member Function

        protected override bool AnalyseFieldData()
        {
            return true;
        }

        protected override bool FillFieldData()
        {
            return true;
        }

        public override string ShowAllData()
        {
            return "";
        }

        protected override bool HasAllData()
        {
            return true;
        }
        #endregion
    }
}
