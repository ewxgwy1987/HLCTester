using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;

using PALS.Utilities;

namespace BHS.PLCSimulator
{
    public static class TelegramTypeName
    {
        //0001, CRQ  - Application Layer Connection Confirm
        //0002, CCF  - Application Layer Connection Confirm
        //0003, GID  - GID Generated Message
        //0004, ICR  - Item Screened Message
        //0005, ISC  - Item Scanned Message
        //0006, BMAM - Baggage Measurement Array Message
        //0007, IRD  - Item Redirect Message
        //0008, ISE  - Item Sortation Event Message
        //0009, IPR  - Item Proceeded Message
        //0010, ILT  - Item Lost Message
        //0011, ITI  - Item Tracking Information Message
        //0012, MER  - Manual Encoding Request Message
        //0013, AFAI - Airport Code and Function Allocation Information Message
        //0014, CRAI - Carrier Allocation Information Message
        //0015, FBTI - Fallback Tag Information Message
        //0016, FPTI - Four Digits Tag Information Message
        //0017, 1500P - 1500P Information
        //0090, SOL  - Sign-of-life (Keep-Alive) Message
        //0091, TSYN - Time Synchronization Message
        //0099, ACK  - Acknowledge Message

        private const string XML_CFG_TELEGRAM = "telegram";
        private const string XML_CFG_HEADER = "Header";
        private const string XML_CFG_TELEGRAM_ATTIBUTE_ALIAS = "alias";
        private const string XML_CFG_TELEGRAM_ATTIBUTE_NAME = "name";
        private const string XML_CFG_FIELD = "field";
        private const string XML_CFG_FIELD_ATTIBUTE_NAME = "name";
        private const string XML_CFG_FIELD_ATTIBUTEVALUE_NAME = "Type";
        private const string XML_CFG_FIELD_ATTIBUTE_DEFAULT = "default";

        private static Hashtable ht_TypeToName;
        private static Hashtable ht_TypeToAlias;
        private static Hashtable ht_AliasToType;

        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static TelegramTypeName()
        {
            
        }

        public static bool Init(ref XmlNode node_apptele)
        {
            bool result = true;
            int i, num_set = node_apptele.ChildNodes.Count;
            // remove the count of comment nodes in xml
            for (i = 0; i < node_apptele.ChildNodes.Count; i++)
            {
                if (node_apptele.ChildNodes[i].NodeType == XmlNodeType.Comment)
                    num_set--;
            }

            string[] tele_type = new string[num_set];
            string[] tele_alias = new string[num_set];
            string[] tele_name = new string[num_set];
            Init_MapTypeAliasName(ref node_apptele, tele_type, tele_alias, tele_name);
            Init_HTTypeToAlias(tele_type, tele_alias);
            Init_HTAliasToType();
            Init_HTTypeToName(tele_type, tele_name);
            return result;
        }



        private static bool Init_MapTypeAliasName
            (ref XmlNode node_apptele, string[] tele_type, string[] tele_alias, string[] tele_name)
        {
            bool result = true;

            int i, mapindex = 0;
            XmlNode temp_TeleNode;
            XmlNode temp_FieldNode;
            string temp_type;
            for (i = 0; i < node_apptele.ChildNodes.Count; i++)
            {
                temp_TeleNode = null;
                temp_TeleNode = node_apptele.ChildNodes[i];

                if (temp_TeleNode.NodeType != XmlNodeType.Comment)
                {
                    // Get telegram alias name
                    tele_alias[mapindex] = XMLConfig.GetSettingFromAttribute(temp_TeleNode, XML_CFG_TELEGRAM_ATTIBUTE_ALIAS, "test");

                    // Get telegram full name
                    tele_name[mapindex] = XMLConfig.GetSettingFromAttribute(temp_TeleNode, XML_CFG_TELEGRAM_ATTIBUTE_NAME, "testname");

                    // Get the telegram field name="Type"
                    temp_FieldNode = XMLConfig.GetConfigSetElement
                        (ref temp_TeleNode, XML_CFG_FIELD, XML_CFG_FIELD_ATTIBUTE_NAME, XML_CFG_FIELD_ATTIBUTEVALUE_NAME);

                    // Get telegram type default="48,48,48,49"
                    temp_type = XMLConfig.GetSettingFromAttribute(temp_FieldNode, XML_CFG_FIELD_ATTIBUTE_DEFAULT, "");
                    tele_type[mapindex] = Util.DecByteStrToStr(temp_type);

                    _logger.Info("Get TypeName Map: "
                        + "Telegram--" + temp_TeleNode.Name + " "
                        + "Alias--" + tele_alias[mapindex] + " "
                        + "Name--" + tele_name[mapindex] + " "
                        + "Type--" + tele_type[mapindex]);

                    if (tele_alias[mapindex] == "test" || tele_name[mapindex] == "testname" || (tele_type[mapindex] == "" && tele_alias[mapindex] != XML_CFG_HEADER))
                    {
                        //_logger.Info("");
                        throw new Exception("ConfigSet of Telegram is wroing.\n" + temp_TeleNode.OuterXml);
                    }
                    mapindex++;
                }
            }

            return result;
        }

        

        private static void Init_HTTypeToAlias(string[] tele_type, string[] tele_alias)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            
            int i;
            try
            {
                ht_TypeToAlias = new Hashtable();
                for (i = 0; i < tele_type.Length; i++)
                {
                    ht_TypeToAlias.Add(tele_type[i], tele_alias[i]);
                }
            }
            catch (Exception exp)
            {
                _logger.Error(thisMethod + "Failed", exp);
            }
        }

        private static void Init_HTAliasToType()
        {
            ht_AliasToType = new Hashtable();
            if (ht_TypeToAlias != null)
            {
                foreach (DictionaryEntry de in ht_TypeToAlias)
                {
                    ht_AliasToType.Add(de.Value, de.Key);
                }
            }
            else
            {
                Console.WriteLine("wrong order of Initialization of hashtable - ht_TypeToAlias");
            }
        }

        private static void Init_HTTypeToName(string[] tele_type, string[] tele_name)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            int i;
            try
            {
                ht_TypeToName = new Hashtable();
                for (i = 0; i < tele_type.Length; i++)
                {
                    ht_TypeToName.Add(tele_type[i], tele_name[i]);
                }
            }
            catch (Exception exp)
            {
                _logger.Error(thisMethod + "Failed", exp);
            }
            
            //ht_TypeToName.Add("0001", "Application Layer Connection Confirm");
            //ht_TypeToName.Add("0002", "Application Layer Connection Confirm");
            //ht_TypeToName.Add("0003", "GID Generated Message");
            //ht_TypeToName.Add("0004", "Item Screened Message");
            //ht_TypeToName.Add("0005", "Item Scanned Message");
            //ht_TypeToName.Add("0006", "Baggage Measurement Array Message");
            //ht_TypeToName.Add("0007", "Item Redirect Message");
            //ht_TypeToName.Add("0008", "Item Sortation Event Message");
            //ht_TypeToName.Add("0009", "Item Proceeded Message");
            //ht_TypeToName.Add("0010", "Item Lost Message");
            //ht_TypeToName.Add("0011", "Item Tracking Information Message");
            //ht_TypeToName.Add("0012", "Manual Encoding Request Message");
            //ht_TypeToName.Add("0013", "Airport Code and Function Allocation Information Message");
            //ht_TypeToName.Add("0014", "Carrier Allocation Information Message");
            //ht_TypeToName.Add("0015", "Fallback Tag Information Message");
            //ht_TypeToName.Add("0016", "Four Digits Tag Information Message");
            //ht_TypeToName.Add("0017", "1500P Information");
            //ht_TypeToName.Add("0090", "Sign-of-life (Keep-Alive) Message");
            //ht_TypeToName.Add("0091", "Time Synchronization Message");
            //ht_TypeToName.Add("0099", "Acknowledge Message");

        }

        public static bool HasType(string type)
        {
            return ht_TypeToAlias.ContainsKey(type) && ht_TypeToName.ContainsKey(type);
        }

        public static bool HasAlias(string alias)
        {
            return ht_AliasToType.Contains(alias);
        }

        public static string GetTypeByAlias(string alias)
        {
            string tele_type = "XXXX";
            if (ht_AliasToType.ContainsKey(alias))
            {
                tele_type = ht_AliasToType[alias].ToString();
            }
            else
            {
                Console.WriteLine("No such Alias: " + alias);
            }

            return tele_type;
        }

        public static string GetAliasByType(string type)
        {
            string tele_alias = "XXXX";
            if (ht_TypeToAlias.ContainsKey(type))
            {
                tele_alias = ht_TypeToAlias[type].ToString();
            }
            else
            {
                Console.WriteLine("No such Alias: " + type);
            }

            return tele_alias;
        }

        public static string GetNameByType(string type)
        {
            string tele_name = "XXXX";
            if (ht_TypeToName.ContainsKey(type))
            {
                tele_name = ht_TypeToName[type].ToString();
            }
            else
            {
                Console.WriteLine("No such Alias: " + type);
            }

            return tele_name;
        }
    }
}
