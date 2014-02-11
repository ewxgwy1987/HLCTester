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

        private const string XCFG_ATTB_TYPE = "type";
        private const string XCFG_ATTB_RATE = "rate";

        //The tags and attributes used in Layout
        private const string XCFG_LAYOUT="Layout";
        private const string XCFG_DF_DISTANCE="DefaultDistance";
        private const string XCFG_DF_SPEED="DefaultSpeed";

        private const string XCFG_TLGM_NODES = "DpndTlgmNodes";
        private const string XCFG_TLGM_INDIVNODE = "DependNode";
        private const string XCFG_TLGMNODE_ALIAS="TlgmAlias";
        private const string XCFG_TLGMNODE_FIELD="TlgmField";
        private const string XCFG_ATTB_TYPE="type";

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
        
        #endregion

        #region Class Constructor, Dispose, & Destructor
        #endregion

        #region Member Function
        #endregion
    }
}
