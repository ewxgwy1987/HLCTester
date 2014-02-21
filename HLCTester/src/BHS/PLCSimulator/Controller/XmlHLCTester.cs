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
        private const string XCFG_TELEGRAMALIAS = "TelegramAlias";
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

                this.XELMs_NodeTypes = this.m_XLinqRoot.Element(XCFG_NODETYPE).Elements();
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

        ~XmlHLCTester()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disp_mark)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string infostr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            if (disp_mark)
            {
                _logger.Info(".");
                _logger.Info("Class:[" + _className + "] object is being destroyed... <" + thisMethod + ">");
            }

            //---Clean Variable
            if (XELMs_NodeTypes != null) XELMs_NodeTypes = null;
            if (XELMs_TlgmNodes != null) XELMs_TlgmNodes = null;
            if (XELMs_Nodes != null) XELMs_Nodes = null;
            if (XELMs_DependNodes != null) XELMs_DependNodes = null;

            if (this.m_XLinqTesterDoc != null) m_XLinqTesterDoc = null;
            if (this.m_XLinqRoot != null) m_XLinqRoot = null;

            //--Clear END

            if (disp_mark)
            {
                if (_logger.IsInfoEnabled)
                    _logger.Info("Class:[" + _className + "] object has been destroyed. <" + thisMethod + ">");
            }
        }

        #endregion

        #region Member Function

        /// <summary>
        /// return all alias of telegram nodes defined in &lt;DpndTlgmNodes&gt; part
        /// </summary>
        /// <returns></returns>
        public string[] GetAllTlgmDpndNodes()
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
                {
                    errstr += "Cannot get Depended Telegram Nodes.";
                    _logger.Warn(errstr);
                    return null;
                }
                
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
                               where tlgmnode.Element(XCFG_TLGMNODE_ALIAS).Value == alias
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
                    return true; 
                }
                else
                {
                    errstr += "Cannot get the field name and type of Depended Telegram Nodes. Telgram Alias:" + alias;
                    _logger.Warn(errstr);
                    return false;
                }

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
        public string[] GetAllLocDpndNodes()
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
                {
                    errstr += "Cannot get Normal Depended Nodes(Normal).";
                    _logger.Warn(errstr);
                    return null;
                }

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
        /// <param name="tlgmalias"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string GetDirection(string nodetype, string tlgmalias, string data)
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
                                     where tlgm.Element(XCFG_TELEGRAMALIAS).Value == tlgmalias
                                     from stat in tlgm.Element(XCFG_STATUSLIST).Elements(XCFG_INDIV_STATUS)
                                     where stat.Element(XCFG_STAT_VALUE).Value == data
                                     select stat.Element(XCFG_STAT_DIRECTION).Value;

                if (stat_direction.Any())
                {
                    direction = stat_direction.First();
                }
                else
                {
                    errstr += "Cannot convert to Direction. NodeType:" + nodetype + " ,TelegramType:" + tlgmalias + ", Data:" + data;
                    _logger.Warn(errstr);
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return direction;
        }

        /// <summary>
        /// GetDefalutValue: return default value defined for specified node type and telegram type
        /// </summary>
        /// <param name="nodetype"></param>
        /// <param name="tlgmtype"></param>
        /// <returns></returns>
        public XmlNode GetDefalutValue(string nodetype, string tlgmalias)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";
            if (init_mark == false)
                return null;

            XmlNode default_value = null;
            XElement xelm_dfval = new XElement(TC_TAG_TELEGRAM, new XAttribute(TC_ATTB_TLGMALIAS, tlgmalias));
            try
            {
                var xelm_dfval_parts = from ndtp in this.XELMs_NodeTypes
                                       where ndtp.Name == nodetype
                                       from tlgm in ndtp.Element(XCFG_TELEGRAMS).Elements(XCFG_INDIV_TELEGRAM)
                                       where tlgm.Element(XCFG_TELEGRAMALIAS).Value == tlgmalias
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
                else
                {
                    errstr += "Cannot find telegram default value. NodeType:" + nodetype + " , TelegramAlias:" + tlgmalias;
                    _logger.Warn(errstr);
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return default_value;
        }

        /// <summary>
        /// GetLocationByNode: return location of this node name
        /// </summary>
        /// <param name="nodename"></param>
        /// <returns></returns>
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
                else
                {
                    errstr += "Cannot find node location. Nodename:" + nodename;
                    _logger.Warn(errstr);
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return location;
        }

        /// <summary>
        /// GetTypeByNode: return node type of this node name
        /// </summary>
        /// <param name="nodename"></param>
        /// <returns></returns>
        public string GetTypeByNode(string nodename)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            string nodetype = "";
            try
            {
                var type = from nd in this.XELMs_Nodes
                           where nd.Element(XCFG_NODE_NAME).Value == nodename
                           select nd.Element(XCFG_NODE_TYPE).Value;

                if (type.Any())
                {
                    nodetype = type.First<string>();
                }
                else
                {
                    errstr += "Cannot find node type. Nodename:" + nodename;
                    _logger.Warn(errstr);
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return nodetype;
        }

        /// <summary>
        /// GetDpndNodeByNode: return all depended nodes belonging to this node
        /// </summary>
        /// <param name="nodename"></param>
        /// <param name="dpndtypes"></param>
        /// <returns></returns>
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
                                     from dpnd in nd.Elements(XCFG_NODE_DEPENDNODE)
                                     select dpnd;

                if (xelm_dpndnodes.Any())
                {
                    int cnt = xelm_dpndnodes.Count();
                    dpndnodes = new string[cnt];
                    dpndtypes = new string[cnt];
                    for (i = 0; i < cnt; i++)
                    {
                        dpndnodes[i] = "";
                        dpndtypes[i] = "";
                    }

                    i = 0;
                    foreach (var dpndnode in xelm_dpndnodes)
                    {
                        dpndnodes[i] = dpndnode.Value;
                        if (dpndnode.Attribute(XCFG_ATTB_DPND_TYPE) != null)
                            dpndtypes[i] = dpndnode.Attribute(XCFG_ATTB_DPND_TYPE).Value;
                        i++;
                    }
                }
                else
                {
                    errstr += "Cannot find depended node. Nodename:" + nodename;
                    _logger.Warn(errstr);
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return dpndnodes;
        }

        /// <summary>
        /// XGetFirstNextNode: return the first next node with XElement type of this node
        /// </summary>
        /// <param name="nodename"></param>
        /// <returns></returns>
        public XElement XGetFirstNextNode(string nodename)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            XElement firstnextnode = null;
            try
            {
                var xelm_fnxnd = from nd in this.XELMs_Nodes
                                 where nd.Element(XCFG_NODE_NAME).Value == nodename
                                 && nd.Element(XCFG_NEXTNODES) != null
                                 && nd.Element(XCFG_NEXTNODES).Element(XCFG_INDIV_NEXTNODE) != null
                                 select nd.Element(XCFG_NEXTNODES).Element(XCFG_INDIV_NEXTNODE);

                if (xelm_fnxnd.Any())
                {
                    firstnextnode = xelm_fnxnd.First();
                }
                else
                {
                    errstr += "Cannot find first next node. Nodename:" + nodename;
                    _logger.Warn(errstr);
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return firstnextnode;
        }

        /// <summary>
        /// XGetSecureNXND: return the secure next node with XElement type of this node
        /// </summary>
        /// <param name="nodename"></param>
        /// <returns></returns>
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
                                   where nxnd.Attribute(XCFG_ATTB_NXND_ISSECR) != null
                                   && nxnd.Attribute(XCFG_ATTB_NXND_ISSECR).Value == PDVAL_IS_TRUE
                                   select nxnd;

                if (xelm_secnxnd.Any())
                {
                    securenxxd = xelm_secnxnd.First();
                }
                else
                {
                    errstr += "Cannot get secure next node. Nodename:" + nodename;
                    _logger.Warn(errstr);
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return securenxxd;
        }

        /// <summary>
        /// XGetNextNode: return the next node with XElement type of this node by specified direction
        /// </summary>
        /// <param name="nodename"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public XElement XGetNextNode(string nodename, string direction)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            XElement nextnode = null;
            try
            {
                var xelm_nextnode = from nd in this.XELMs_Nodes
                                    where nd.Element(XCFG_NODE_NAME).Value == nodename
                                    && nd.Element(XCFG_NEXTNODES).Elements(XCFG_INDIV_NEXTNODE).Any() == true
                                    from nxnd in nd.Element(XCFG_NEXTNODES).Elements(XCFG_INDIV_NEXTNODE)
                                    where nxnd.Attribute(XCFG_ATTB_NXND_DIRECTION).Value == direction
                                    select nxnd;

                if (xelm_nextnode.Any())
                {
                    nextnode = xelm_nextnode.First();
                }
                else
                {
                    errstr += "Cannot get next node. Nodename:" + nodename + " ,Direction:" + direction;
                    _logger.Warn(errstr);
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return nextnode;
        }

        /// <summary>
        /// ParseNextNode_Name: Parse the next node XElement to get next node name
        /// </summary>
        /// <param name="xnextnode"></param>
        /// <returns></returns>
        public string ParseNextNode_Name(XElement xnextnode)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            string nextnode_name = "";
            
            try
            {
                if (xnextnode != null && xnextnode.Element(XCFG_NEXTNODE_NAME) != null)
                    nextnode_name = xnextnode.Element(XCFG_NEXTNODE_NAME).Value;
                else
                {
                    errstr += "Cannot parse next node name.";
                    _logger.Warn(errstr);
                }
                
            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }
            return nextnode_name;
        }

        /// <summary>
        /// ParseNextNode_isSend: Parse the next node XElement to check whether the telegram shouled be sent before go to next node.
        /// </summary>
        /// <param name="xnextnode"></param>
        /// <returns></returns>
        public bool ParseNextNode_isSend(XElement xnextnode)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            bool isSend = true;

            try
            {
                if (xnextnode != null && xnextnode.Attribute(XCFG_ATTB_NXND_ISSEND) != null)
                {
                    if (xnextnode.Element(XCFG_NEXTNODE_NAME).Value == PDVAL_IS_FALSE)
                        isSend = false;
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }
            return isSend;
        }

        /// <summary>
        /// ParseNextNode_PRDLOC: Parse the next node XElement to get the proceeded location.
        /// </summary>
        /// <param name="xnextnode"></param>
        /// <returns></returns>
        public string ParseNextNode_PRDLOC(XElement xnextnode)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            string prdloc = "";

            try
            {
                if (xnextnode != null && xnextnode.Element(XCFG_NEXTNODE_PRDLOC) != null)
                    prdloc = xnextnode.Element(XCFG_NEXTNODE_PRDLOC).Value;
                else
                {
                    errstr += "Cannot parse proceeded location.";
                    _logger.Warn(errstr);
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }
            return prdloc;
        }

        /// <summary>
        /// ParseNextNode_Distance: Parse the next node XElement to get distance.
        /// </summary>
        /// <param name="xnextnode"></param>
        /// <returns></returns>
        public string ParseNextNode_Distance(XElement xnextnode)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";
   
            string distance = "";
            try
            {

                if (xnextnode != null && xnextnode.Element(XCFG_NEXTNODE_DISTANCE) != null)
                    distance = xnextnode.Element(XCFG_NEXTNODE_DISTANCE).Value;
                else
                {
                    errstr += "Cannot parse distance.";
                    _logger.Warn(errstr);
                    
                }
            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }
            return distance;
        }

        /// <summary>
        /// ParseNextNode_Speed: Parse the next node XElement to get speed.
        /// </summary>
        /// <param name="xnextnode"></param>
        /// <returns></returns>
        public string ParseNextNode_Speed(XElement xnextnode)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            string speed = "";
            try
            {
                if (xnextnode != null && xnextnode.Element(XCFG_NEXTNODE_SPEED) != null)
                    speed = xnextnode.Element(XCFG_NEXTNODE_SPEED).Value;
                else
                {
                    errstr += "Cannot parse speed.";
                    _logger.Warn(errstr);
                }
            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }
            return speed;
        }

        /// <summary>
        /// GetSpecialTlgm: Get the special telegrams with specified telegram type(like "random") by node type
        /// </summary>
        /// <param name="nodetype"></param>
        /// <param name="tlgmtype"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public string[] GetSpecialTlgm(string nodetype, string tlgmtype, out double[] rate)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            string[] tlgmalias = null;
            rate = null;
            try
            {
                var xelm_tlgms = from ndtp in this.XELMs_NodeTypes
                                 where ndtp.Name == nodetype
                                 from tlgm in ndtp.Element(XCFG_TELEGRAMS).Elements(XCFG_INDIV_TELEGRAM)
                                 where tlgm.Attribute(XCFG_ATTB_NDTP_TLM_TYPE) != null
                                 && tlgm.Attribute(XCFG_ATTB_NDTP_TLM_TYPE).Value == tlgmtype
                                 select tlgm;

                if (xelm_tlgms.Any())
                {
                    int i = 0;
                    int cnt = xelm_tlgms.Count();
                    tlgmalias = new string[cnt];
                    rate = new double[cnt];
                    for (i = 0; i < cnt; i++)
                    {
                        tlgmalias[i] = "";
                        rate[i] = -1;
                    }

                    i = 0;
                    foreach (var tlgm in xelm_tlgms)
                    {
                        tlgmalias[i] = tlgm.Element(XCFG_TELEGRAMALIAS).Value;
                        if (tlgm.Attribute(XCFG_ATTB_NDTP_TLM_RATE) != null)
                            rate[i] = double.Parse(tlgm.Attribute(XCFG_ATTB_NDTP_TLM_RATE).Value);
                        i++;
                    }
                }
                else
                {
                }
            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }
            return tlgmalias;
        }

        #endregion
    }
}
