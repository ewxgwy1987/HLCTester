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

        // FieldFormat Collection
        // key is fieldname, value is FieldForamt Object
        private Hashtable HT_FieldList; 

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

                            if (name == "noname" || offset == "nooffset" || length == "nolength")
                            {
                                string errorstring = "Field set problems,Telegram: " + this.m_telegram_alias + "NAME--" + name + " OFFSET--" + offset + " LENGTH--" + length + " DEFAULT--" + f_default;
                                _logger.Error(errorstring);
                                throw new Exception("errorstring");
                            }
                            else
                            {
                                FieldFormat temp_format = new FieldFormat(name, offset, length, f_default);
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

        //public void TestFormat_ICR(string telegram_alias)
        //{
        //    if (telegram_alias == "ICR")
        //    {
              
        //        FieldFormat icr_type = new FieldFormat("TLG_TYPE", "0", "4", "48,48,48,52");
        //        FieldFormat icr_length = new FieldFormat("TLG_LEN", "4", "4", "48,48,50,50");
        //        FieldFormat icr_Seqno = new FieldFormat("TLG_SEQ", "8", "4", "?");
        //        FieldFormat icr_gidmsb = new FieldFormat("GID_MSB", "12", "1", "?");
        //        FieldFormat icr_gidlsb = new FieldFormat("GID_LSB", "13", "4", "?");
        //        FieldFormat icr_location = new FieldFormat("LOCATION", "17", "2", "?");
        //        FieldFormat icr_scrlvl = new FieldFormat("SCR_LVL", "19", "1", "?");
        //        FieldFormat icr_scrres = new FieldFormat("SCR_RES", "20", "1", "?");
        //        FieldFormat icr_plcidx = new FieldFormat("PLC_IDX", "21", "1", "?");

        //        HT_FieldList.Add(icr_type.FieldName, icr_type);
        //        HT_FieldList.Add(icr_length.FieldName, icr_length);
        //        HT_FieldList.Add(icr_Seqno.FieldName, icr_Seqno);
        //        HT_FieldList.Add(icr_gidmsb.FieldName, icr_gidmsb);
        //        HT_FieldList.Add(icr_gidlsb.FieldName, icr_gidlsb);
        //        HT_FieldList.Add(icr_location.FieldName, icr_location);
        //        HT_FieldList.Add(icr_scrlvl.FieldName, icr_scrlvl);
        //        HT_FieldList.Add(icr_scrres.FieldName, icr_scrres);
        //        HT_FieldList.Add(icr_plcidx.FieldName, icr_plcidx);
        //    }
        //}

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

        //public int GetOffset(string fieldname)
        //{
        //    int offset = init_value;
        //    if (this.HT_FieldList.ContainsKey(fieldname))
        //    {
        //        FieldFormat temp_format = HT_FieldList[fieldname] as FieldFormat;
        //        try
        //        {
        //            offset = int.Parse(temp_format.Offset);
        //        }
        //        catch (FormatException exp)
        //        {
        //            Console.WriteLine("TelegramFormat: " + m_telegram_alias);
        //            Console.WriteLine("Offset is not a integer. Offset: " + temp_format.Offset);
        //            Console.WriteLine(exp.ToString());
        //        }
        //        catch (Exception exp)
        //        {
        //            Console.WriteLine(exp.ToString());
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("wrong filed name: " + fieldname + " in TelegramFormat: " + m_telegram_alias);
        //    }
        //    return offset;
        //}

        //public int GetLength(string fieldname)
        //{
        //    int length = init_value;

        //    if (this.HT_FieldList.ContainsKey(fieldname))
        //    {
        //        FieldFormat temp_format = HT_FieldList[fieldname] as FieldFormat;
        //        try
        //        {
        //            length = int.Parse(temp_format.FieldLength);
        //        }
        //        catch (FormatException exp)
        //        {
        //            Console.WriteLine("TelegramFormat: " + m_telegram_alias);
        //            Console.WriteLine("Length is not a integer. Length: " + temp_format.FieldLength);
        //            Console.WriteLine(exp.ToString());
        //        }
        //        catch (Exception exp)
        //        {
        //            Console.WriteLine(exp.ToString());
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("wrong filed name: " + fieldname + " in TelegramFormat: " + m_telegram_alias);
        //    }
        //    return length;
        //}

        //public string GetDefaultValue(string fieldname)
        //{
        //    string default_value = "";

        //    if (this.HT_FieldList.ContainsKey(fieldname))
        //    {
        //        FieldFormat temp_format = HT_FieldList[fieldname] as FieldFormat;
        //        default_value = temp_format.DefaultValue;
        //    }
        //    else
        //    {
        //        Console.WriteLine("Wrong filed name: " + fieldname + " in TelegramFormat: " + m_telegram_alias);
        //    }

        //    return default_value;
        //}

        public string GetAttributeValue(string fieldname, string attribute)
        {
            string attr_value = "";

            if (this.HT_FieldList.ContainsKey(fieldname))
            {
                FieldFormat temp_format = HT_FieldList[fieldname] as FieldFormat;
                if (attribute == XML_CFG_FIELD_ATTRIBUTE_OFFSET)
                {
                    attr_value = temp_format.Offset;
                }
                else if(attribute==XML_CFG_FIELD_ATTRIBUTE_LENGTH)
                {
                    attr_value = temp_format.FieldLength;
                }
                else if (attribute == XML_CFG_FIELD_ATTRIBUTE_DEFAULT)
                {
                    attr_value = temp_format.DefaultValue;
                }
                else
                {
                    Console.WriteLine("Unkown Attribute:" + attribute + " in Field:" + fieldname + "in TelegramFormat:" + this.m_telegram_alias);
                }
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
