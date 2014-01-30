using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;

using PALS.Utilities;


namespace BHS.PLCSimulator
{
    public class TelegramFormat
    {
        #region Class Field and Property
        private const string XML_CFG_TELEGRAM_ATTIBUTE_ALIAS = "alias";
        private const string XML_CFG_FIELD = "field";
        private const string XML_CFG_FIELD_ATTRIBUTE_NAME = "name";
        private const string XML_CFG_FIELD_ATTRIBUTE_OFFSET = "offset";
        private const string XML_CFG_FIELD_ATTRIBUTE_LENGTH = "length";
        private const string XML_CFG_FIELD_ATTRIBUTE_DEFAULT = "default";

        private const string XML_CFG_FIELD_ATTRIBUTE_DATATYPE = "datatype";
        private const string XML_CFG_FIELD_ATTRIBUTE_SHOWLENGTH = "showlength";

        // FieldFormat Collection
        // key is fieldname, value is FieldForamt Object
        public Hashtable HT_FieldList; 

        private string m_telegram_type;
        private string m_telegram_name;
        private string m_telegram_alias;

        private const int init_value = -1000;


        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        public TelegramFormat()
        {
        }

        //// Constructor with alias name of telegram
        //public TelegramFormat(string telegram_alias)
        //{
        //    HT_FieldList = new Hashtable();

        //    if (TelegramTypeName.HasAlias(telegram_alias))
        //    {
        //        this.m_telegram_type = TelegramTypeName.GetTypeByAlias(telegram_alias);
        //        this.m_telegram_alias = telegram_alias;
        //        this.m_telegram_name = TelegramTypeName.GetNameByType(this.m_telegram_type);

        //        TestFormat_ICR(telegram_alias);
        //    }
        //    else
        //    {
        //        Console.WriteLine("TelegramFormat failed! No such Alias: " + telegram_alias);
        //    }
        //}

        public TelegramFormat(XmlNode node_Telegram)
        {
            int i;
            string name, offset, length, f_default;
            string datatype, showlength;

            HT_FieldList = new Hashtable();

            string telegram_alias = XMLConfig.GetSettingFromAttribute(node_Telegram, XML_CFG_TELEGRAM_ATTIBUTE_ALIAS, "test");
            if (telegram_alias == "test")
            {
                throw new Exception("ConfigSet of Telegram is wroing.\n" + node_Telegram.OuterXml);
            }
            else
            {
                if (TelegramTypeName.HasAlias(telegram_alias))
                {
                    this.m_telegram_type = TelegramTypeName.GetTypeByAlias(telegram_alias);
                    this.m_telegram_alias = telegram_alias;
                    this.m_telegram_name = TelegramTypeName.GetNameByType(this.m_telegram_type);

                    for (i = 0; i < node_Telegram.ChildNodes.Count; i++)
                    {
                        XmlNode temp_nodefield = node_Telegram.ChildNodes[i];
                        if (temp_nodefield.NodeType != XmlNodeType.Comment && temp_nodefield.Name == XML_CFG_FIELD)
                        {
                            name = XMLConfig.GetSettingFromAttribute(temp_nodefield, XML_CFG_FIELD_ATTRIBUTE_NAME, "noname");
                            offset = XMLConfig.GetSettingFromAttribute(temp_nodefield, XML_CFG_FIELD_ATTRIBUTE_OFFSET, "nooffset");
                            length = XMLConfig.GetSettingFromAttribute(temp_nodefield, XML_CFG_FIELD_ATTRIBUTE_LENGTH, "nolength");
                            f_default = XMLConfig.GetSettingFromAttribute(temp_nodefield, XML_CFG_FIELD_ATTRIBUTE_DEFAULT, "?");

                            datatype = XMLConfig.GetSettingFromAttribute(temp_nodefield, XML_CFG_FIELD_ATTRIBUTE_DATATYPE, "unknown");
                            showlength = XMLConfig.GetSettingFromAttribute(temp_nodefield, XML_CFG_FIELD_ATTRIBUTE_SHOWLENGTH, "unknown");

                            if (name == "noname" || offset == "nooffset" || length == "nolength" || datatype == "unknown" || showlength == "unknown")
                            {
                                string errorstring = "Field set problems,Telegram: "
                                    + this.m_telegram_alias
                                    + " NAME--" + name
                                    + " OFFSET--" + offset
                                    + " LENGTH--" + length
                                    + " DEFAULT--" + f_default
                                    + " Datatype--" + datatype
                                    + " ShowLength--" + showlength;
                                _logger.Error(errorstring);
                                throw new Exception(errorstring);
                            }
                            else
                            {
                                FieldFormat temp_format = new FieldFormat(name, offset, length, f_default, datatype, showlength);
                                HT_FieldList.Add(temp_format.FieldName, temp_format);
                            }
                        }
                    }
                }
                else
                {
                    _logger.Error("TelegramFormat failed! No such Alias: " + telegram_alias);
                }
            }
        }

        public void ConstructFormat(string telegram_type)
        {
        }

        public string Type
        {
            get
            {
                return this.m_telegram_type;
            }
           
        }

        public string Alias
        {
            get
            {
                return this.m_telegram_alias;
            }
            
        }

        public string Name
        {
            get
            {
                return this.m_telegram_name;
            }
            
        }

        public bool HasField(string fieldname)
        {
            return HT_FieldList.ContainsKey(fieldname);
        }

        public string GetAttributeValue(string fieldname, string attribute)
        {
            string attr_value = "";

            if (this.HT_FieldList.ContainsKey(fieldname))
            {
                FieldFormat temp_format = HT_FieldList[fieldname] as FieldFormat;
                switch (attribute)
                {
                    case XML_CFG_FIELD_ATTRIBUTE_OFFSET:
                        attr_value = temp_format.Offset;
                        break;
                    case XML_CFG_FIELD_ATTRIBUTE_LENGTH:
                        attr_value = temp_format.FieldLength;
                        break;
                    case XML_CFG_FIELD_ATTRIBUTE_DEFAULT:
                        attr_value = temp_format.DefaultValue;
                        break;
                    case XML_CFG_FIELD_ATTRIBUTE_DATATYPE:
                        attr_value = temp_format.DataType;
                        break;
                    case XML_CFG_FIELD_ATTRIBUTE_SHOWLENGTH:
                        attr_value = temp_format.ShowLength;
                        break;
                    default:
                        Console.WriteLine("Unkown Attribute:" + attribute + " in Field:" + fieldname + "in TelegramFormat:" + this.m_telegram_alias);
                        break;
                }

                //if (attribute == XML_CFG_FIELD_ATTRIBUTE_OFFSET)
                //{
                //    attr_value = temp_format.Offset;
                //}
                //else if(attribute==XML_CFG_FIELD_ATTRIBUTE_LENGTH)
                //{
                //    attr_value = temp_format.FieldLength;
                //}
                //else if (attribute == XML_CFG_FIELD_ATTRIBUTE_DEFAULT)
                //{
                //    attr_value = temp_format.DefaultValue;
                //}
                //else
                //{
                //    Console.WriteLine("Unkown Attribute:" + attribute + " in Field:" + fieldname + "in TelegramFormat:" + this.m_telegram_alias);
                //}
            }
            else
            {
                Console.WriteLine("Wrong filed name: " + fieldname + " in TelegramFormat: " + m_telegram_alias);
            }

            return attr_value;
        }

        public void ShowFieldList()
        {
            string outstr = "";
            string fieldname, offset, length, defaultvalue;
            Console.WriteLine("Hello TelegramFormat: " + this.m_telegram_alias);
            Console.WriteLine("Name \t Alias\t\t Type");
            Console.WriteLine(m_telegram_name + "\t\t" + m_telegram_alias + "\t\t" + m_telegram_type);

            Console.WriteLine("Filed List:");
            Console.WriteLine("Field Name\t Offset\t Length\t Default Value");
            foreach (DictionaryEntry de in HT_FieldList)
            {
                fieldname = de.Key.ToString(); // Field Name
                offset = GetAttributeValue(fieldname,XML_CFG_FIELD_ATTRIBUTE_OFFSET);
                length = GetAttributeValue(fieldname,XML_CFG_FIELD_ATTRIBUTE_LENGTH);
                defaultvalue = GetAttributeValue(fieldname,XML_CFG_FIELD_ATTRIBUTE_DEFAULT);

                outstr += fieldname + "\t\t" + offset + "\t\t" + length + "\t\t" + defaultvalue + "\n";
            }
            Console.WriteLine(outstr);
        }
        
    }
}
