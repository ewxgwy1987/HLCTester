using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;

namespace BHS.PLCSimulator
{
    public static class TelegramFormatList
    {
        private static Hashtable HT_TelegramFormatList;

        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static TelegramFormatList()
        {
            HT_TelegramFormatList = new Hashtable();

            //TelegramFormat ICR_Format = new TelegramFormat("ICR");
            //HT_TelegramFormatList.Add("ICR", ICR_Format);
        }

        public static bool Init(ref XmlNode node_apptele)
        {
            bool result = true;

            TelegramTypeName.Init(ref node_apptele);

            foreach (XmlNode temp_NodeTele in node_apptele)
            {
                if (temp_NodeTele.NodeType != XmlNodeType.Comment)
                {
                    TelegramFormat temp_TeleFormat = new TelegramFormat(temp_NodeTele);
                    HT_TelegramFormatList.Add(temp_TeleFormat.Alias, temp_TeleFormat);
                }
            }

            return result;
        }

        public static void ShowTelegramFormat(string Tel_Alias)
        {
            if (HT_TelegramFormatList.ContainsKey(Tel_Alias))
            {
                TelegramFormat new_telformat = HT_TelegramFormatList[Tel_Alias] as TelegramFormat;
                new_telformat.ShowFieldList();
            }
            else
            {
                Console.WriteLine("TelegramFormatList does not have such Telegram: " + Tel_Alias);
            }
        }

        public static bool HasTelegram(string tel_alias)
        {
            return HT_TelegramFormatList.ContainsKey(tel_alias);
        }

        public static TelegramFormat GetTelegramFormat(string Tel_Alias)
        {
            TelegramFormat new_telformat = null;
            if (HT_TelegramFormatList.ContainsKey(Tel_Alias))
            {
                new_telformat = HT_TelegramFormatList[Tel_Alias] as TelegramFormat;
            }
            else
            {
                Console.WriteLine("TelegramFormatList does not have such Telegram: " + Tel_Alias);
            }
            return new_telformat;
        }
    }
}
