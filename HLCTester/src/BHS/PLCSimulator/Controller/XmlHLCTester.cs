using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace BHS.PLCSimulator.Controller
{
    // This Class use Linq to XML to process CFG_HLCTester.xml
    public class XmlHLCTester
    {
        #region Class Field and Property

        private string m_TesterXMLPath;
        private XDocument m_XLinqTesterDoc;
        private XElement m_XLinqRoot;

        //Root tag
        private const string XCFG_ROOTBHS = "BHS";

        //The tags and attributes used in NodeType
        private const string XCFG_NODETYPE = "NodeType";
        private const string XCFG_NDTP_NOTRACKING = "NTRCV";
        private const string XCFG_TELEGRAMS = "Telegrams";
        private const string XCFG_INDIV_TELEGRAM = "Telegram";
        private const string XCFG_TELEGRAMTYPE = "TelegramType";
        private const string XCFG_DEFAULTVALUE = "DefaultValue";
        private const string XCFG_STATUSLIST = "StatusList";
        private const string XCFG_INDIV_STATUS = "Status";
        private const string XCFG_STAT_VALUE = "Value";
        private const string XCFG_STAT_DIRECTION = "Direction";

        private const string XCFG_ATTB_NDTP_TLM_TYPE = "type";
        private const string XCFG_ATTB_NDTP_TLM_RATE = "rate";

        //The tags and attributes used in Layout
        private const string XCFG_LAYOUT="Layout";
        private const string XCFG_DF_DISTANCE="DefaultDistance";
        private const string XCFG_DF_SPEED="DefaultSpeed";

        private const string XCFG_TLGM_NODES = "DpndTlgmNodes";
        private const string XCFG_TLGM_INDIVNODE = "DependNode";
        private const string XCFG_TLGMNODE_ALIAS="TlgmAlias";
        private const string XCFG_TLGMNODE_FIELD="TlgmField";
        private const string XCFG_ATTB_TLMND_TYPE="type";

        private const string XCFG_NODES = "Nodes";
        private const string XCFG_INDIV_NODE = "Node";
        private const string XCFG_NODE_NAME = "Name";
        private const string XCFG_NODE_LOCATION = "Location";
        private const string XCFG_NODE_TYPE = "Type";
        private const string XCFG_NODE_DEPENDNODE = "DependNode";
        private const string XCFG_NEXTNODES = "NextNodes";
        private const string XCFG_INDIV_NEXTNODE = "NextNode";
        private const string XCFG_NEXTNODE_NAME = "Name";
        private const string XCFG_NEXTNODE_DISTANCE = "Distance";
        private const string XCFG_NEXTNODE_SPEED = "Speed";
        private const string XCFG_NEXTNODE_PRDLOC = "ProceedLoc";

        private const string XCFG_ATTB_DPND_TYPE = "type";
        private const string XCFG_ATTB_NXND_DIRECTION = "direction";
        private const string XCFG_ATTB_NXND_ISSECR = "isSecure";
        private const string XCFG_ATTB_NXND_ISSEND = "isSent";
        private const string XCFG_ATTB_PRDTYPE = "Location";


        // pre-defined value for attributes
        private const string PDVAL_ENDPOINT = "OVER";
        private const string PDVAL_DIRT_OTHER = "other";
        private const string PDVAL_DIRT_LOST = "lost";
        private const string PDVAL_IS_TRUE = "true";
        private const string PDVAL_IS_FALSE = "false";
        private const string PDVAL_DPTYPE_TLGM = "telegram";

        // 
        private IEnumerable<XElement> XELMs_NodeTypes;
        private IEnumerable<XElement> XELMs_TlgmNodes;
        private IEnumerable<XElement> XELMs_Nodes;
        private IEnumerable<XElement> XELMs_DependNodes;

        // pre-defined tags used to construct XmlElement with defalut value to initialize the telegram
        private const string TC_TAG_TELEGRAM = "telegram";
        private const string TC_ATTB_TLGMALIAS = "alias";

        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int m_df_distance;
        public int DefaultDistance
        {
            get
            {
                return this.m_df_distance;
            }
        }

        private int m_df_speed;
        public int DefaultSpeed
        {
            get
            {
                return this.m_df_speed;
            }
        }

        private bool init_mark;
        
        #endregion

        #region Class Constructor, Dispose, & Destructor

        /// <summary>
        /// This class is used to analyze the CFG_HLCTester.xml. Use a FilePath to initialize the Class with Linq. 
        /// </summary>
        /// <param name="FilePath"></param>
        public XmlHLCTester(string FilePath)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";
            init_mark = false;

            try
            {
                this.m_TesterXMLPath = FilePath;
                this.m_XLinqTesterDoc = XDocument.Load(FilePath);
                this.m_XLinqRoot = this.m_XLinqTesterDoc.Root;

                this.XELMs_NodeTypes = this.m_XLinqRoot.Elements(XCFG_NODETYPE);
                this.XELMs_TlgmNodes = this.m_XLinqRoot.Element(XCFG_LAYOUT).Element(XCFG_TLGM_NODES).Elements(XCFG_TLGM_INDIVNODE);
                this.XELMs_Nodes = this.m_XLinqRoot.Element(XCFG_LAYOUT).Element(XCFG_NODES).Elements(XCFG_INDIV_NODE);
                this.XELMs_DependNodes = this.m_XLinqRoot.Element(XCFG_LAYOUT).Element(XCFG_NODES).Descendants(XCFG_NODE_DEPENDNODE);

                this.m_df_distance = -1;
                this.m_df_speed = -1;
                string dis = this.m_XLinqRoot.Element(XCFG_LAYOUT).Element(XCFG_DF_DISTANCE).Value;
                string speed = this.m_XLinqRoot.Element(XCFG_LAYOUT).Element(XCFG_DF_SPEED).Value;
                this.m_df_distance = int.Parse(dis);
                this.m_df_speed = int.Parse(speed);

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }
            init_mark = true;
        }

        #endregion

        #region Member Function

        /// <summary>
        /// return all alias of telegram nodes defined in &lt;DpndTlgmNodes&gt; part
        /// </summary>
        /// <returns></returns>
        public string[] GetTlgmDpndNodes()
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";
            if (init_mark == false)
                return null;

            string[] tlgmalias_list;
            try
            {
                int i;
                var tlgmlist = from tlgmnode in this.XELMs_TlgmNodes
                                where tlgmnode.Element(XCFG_TLGMNODE_ALIAS) != null
                                select tlgmnode.Element(XCFG_TLGMNODE_ALIAS);

                if (tlgmlist.Any())
                {
                    tlgmalias_list = new string[tlgmlist.Count()];
                    i = 0;
                    foreach (var tlgm in tlgmlist)
                    {
                        tlgmalias_list[i] = tlgm.Value;
                        i++;
                    }
                    return tlgmalias_list;
                }
                else
                    return null;
                
            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
                return null;
            }
        }

        /// <summary>
        /// return the field name and type
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="fieldname"></param>
        /// <param name="fieldtype">If fieldtype is "location", it needs some special process</param>
        /// <returns></returns>
        public bool GetTlgmDpndField(string alias, out string fieldname, out string fieldtype)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";
            fieldname = "";
            fieldtype = "";
            if (init_mark == false)
                return false;

            try
            {
                var tlgmfield = from tlgmnode in this.XELMs_TlgmNodes
                               where tlgmnode.Element(XCFG_TLGMNODE_ALIAS) != null
                               && tlgmnode.Element(XCFG_TLGMNODE_ALIAS).Value == alias
                               select tlgmnode.Element(XCFG_TLGMNODE_FIELD);

                if (tlgmfield.Any())
                {
                    XElement xfield = tlgmfield.First<XElement>();
                    fieldname = xfield.Value;
                    XAttribute xfieldattb = xfield.Attribute(XCFG_ATTB_TLMND_TYPE);
                    //Attribute "type" may be "location" or other or there is not type attribute
                    if (xfieldattb != null)
                        fieldtype = xfieldattb.Value;
                    else
                        fieldtype = "";
                    return true; ;
                }
                else
                    return false;

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
                return false;
            }
        }

        /// <summary>
        /// return all Depended nodes defined in some key nodes, which does not have type attribute
        /// </summary>
        /// <returns></returns>
        public string[] GetLocDpndNodes()
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";
            if (init_mark == false)
                return null;

            string[] locdpndnodes;
            try
            {
                int i;
                var dpndnodes = from dpndnode in this.XELMs_DependNodes
                                    where dpndnode.Attribute(XCFG_ATTB_DPND_TYPE) == null
                                    select dpndnode.Value;

                if (dpndnodes.Any())
                {
                    List<string> dpndnode_list = new List<string>();
                    
                    i = 0;
                    foreach (var dpnd in dpndnodes)
                    {
                        if (dpndnode_list.IndexOf(dpnd) == -1)
                        {
                            dpndnode_list.Add(dpnd);
                            i++;
                        }
                    }
                    locdpndnodes = dpndnode_list.ToArray();
                    return locdpndnodes;
                }
                else
                    return null;

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
                return null;
            }
        }

        /// <summary>
        /// Convert data to dirction according to the map of &lt;StatusList&gt; 
        /// </summary>
        /// <param name="nodetype"></param>
        /// <param name="tlgmtype"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string GetDirection(string nodetype, string tlgmtype, string data)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";
            if (init_mark == false)
                return "";

            string direction = "";
            try
            {
                var stat_direction = from ndtp in this.XELMs_NodeTypes
                                     where ndtp.Name == nodetype
                                     from tlgm in ndtp.Element(XCFG_TELEGRAMS).Elements(XCFG_INDIV_TELEGRAM)
                                     where tlgm.Element(XCFG_TELEGRAMTYPE).Value == tlgmtype
                                     from stat in tlgm.Element(XCFG_STATUSLIST).Elements(XCFG_INDIV_STATUS)
                                     where stat.Element(XCFG_STAT_VALUE).Value == data
                                     select stat.Element(XCFG_STAT_DIRECTION).Value;

                if (stat_direction.Any())
                {
                    direction = stat_direction.First();
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return direction;
        }

        public XmlNode GetDefalutValue(string nodetype, string tlgmtype)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";
            if (init_mark == false)
                return null;

            XmlNode default_value = null;
            XElement xelm_dfval = new XElement(TC_TAG_TELEGRAM, new XAttribute(TC_ATTB_TLGMALIAS, tlgmtype));
            try
            {
                var xelm_dfval_parts = from ndtp in this.XELMs_NodeTypes
                                       where ndtp.Name == nodetype
                                       from tlgm in ndtp.Element(XCFG_TELEGRAMS).Elements(XCFG_INDIV_TELEGRAM)
                                       where tlgm.Element(XCFG_TELEGRAMTYPE).Value == tlgmtype
                                       select tlgm.Element(XCFG_DEFAULTVALUE).Elements();

                if (xelm_dfval_parts.Any())
                {
                    foreach (var xfield in xelm_dfval_parts)
                    {
                        xelm_dfval.Add(xfield);
                    }
                    XmlReader xreader = xelm_dfval.CreateReader();
                    xreader.MoveToContent();
                    XmlDocument xdfdoc = new XmlDocument();
                    default_value = xdfdoc.ReadNode(xreader);         
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return default_value;
        }

        public string GetLocationByNode(string nodename)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            string location = "";
            try
            {
                var loc = from nd in this.XELMs_Nodes
                          where nd.Element(XCFG_NODE_NAME) != null
                          && nd.Element(XCFG_NODE_LOCATION) != null
                          && nd.Element(XCFG_NODE_NAME).Value == nodename
                          select nd.Element(XCFG_NODE_LOCATION).Value;

                if (loc.Any())
                {
                    location = loc.First<string>();
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return location;
        }

        public string GetTypeByNode(string nodename)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            string nodetype = "";
            try
            {
                var type = from nd in this.XELMs_Nodes
                           where nd.Element(XCFG_NODE_NAME) != null
                           && nd.Element(XCFG_NODE_TYPE) != null
                           && nd.Element(XCFG_NODE_NAME).Value == nodename
                           select nd.Element(XCFG_NODE_TYPE).Value;

                if (type.Any())
                {
                    nodetype = type.First<string>();
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return nodetype;
        }

        public string[] GetDpndNodeByNode(string nodename, out string[] dpndtypes)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            int i;
            string[] dpndnodes = null;
            dpndtypes = null;
            try
            {
                var xelm_dpndnodes = from nd in this.XELMs_Nodes
                                     where nd.Element(XCFG_NODE_NAME) != null
                                     && nd.Element(XCFG_NODE_DEPENDNODE) != null
                                     && nd.Element(XCFG_NODE_NAME).Value == nodename
                                     select nd.Elements(XCFG_NODE_DEPENDNODE);

                if (xelm_dpndnodes.Any())
                {
                    int cnt = xelm_dpndnodes.Count();
                    dpndnodes = new string[cnt];
                    dpndtypes = new string[cnt];
                    i = 0;
                    foreach (XElement dpndnode in xelm_dpndnodes)
                    {
                        dpndnodes[i] = dpndnode.Value;
                        dpndtypes[i] = dpndnode.Attribute(XCFG_ATTB_DPND_TYPE).Value;
                        i++;
                    }
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return dpndnodes;
        }

        public XElement XGetFirstNextNode(string nodename)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            int i;
            XElement firstnextnode = null;
            try
            {
                var xelm_fnxnd = from nd in this.XELMs_Nodes
                                 where nd.Element(XCFG_NODE_NAME) != null
                                 && nd.Element(XCFG_NODE_NAME).Value == nodename
                                 && nd.Element(XCFG_NEXTNODES) != null
                                 && nd.Element(XCFG_NEXTNODES).Element(XCFG_INDIV_NEXTNODE) != null
                                 select nd.Element(XCFG_NEXTNODES).Element(XCFG_INDIV_NEXTNODE);

                if (xelm_fnxnd.Any())
                {
                    firstnextnode = xelm_fnxnd.First();
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return firstnextnode;
        }

        public XElement XGetSecureNXND(string nodename)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            int i;
            XElement securenxxd = null;
            try
            {
                var xelm_secnxnd = from nd in this.XELMs_Nodes
                                   where nd.Element(XCFG_NODE_NAME).Value == nodename
                                   && nd.Element(XCFG_NEXTNODES) != null
                                   && nd.Element(XCFG_NEXTNODES).Elements(XCFG_INDIV_NEXTNODE).Any() == true
                                   from nxnd in nd.Element(XCFG_NEXTNODES).Elements(XCFG_INDIV_NEXTNODE)
                                   where nxnd.Attribute(XCFG_ATTB_NXND_ISSECR).Value == PDVAL_IS_TRUE
                                   select nxnd;

                if (xelm_secnxnd.Any())
                {
                    securenxxd = xelm_secnxnd.First();
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return securenxxd;
        }

        public XElement XGetNextNode(string nodename, string direction)
        {
        }

        public string ParseNextNode_Name(XElement xnextnode)
        {
        }

        public string ParseNextNode_PRDLOC(XElement xnextnode)
        {
        }

        public int ParseNextNode_WaitTime(XElement xnextnode)
        {
        }

        public string[] GetSpecialTlgm(string nodetype, string tlgmtype, out double rate)
        {
        }

        #endregion
    }
}
