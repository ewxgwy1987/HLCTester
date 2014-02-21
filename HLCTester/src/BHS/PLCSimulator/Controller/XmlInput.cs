using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Linq;
//using System.Xml.XPath;
using System.IO;

namespace BHS.PLCSimulator.Controller
{
    // This Class use Linq to XML to process CFG_InputDefine.xml
    public class XmlInput
    {
        #region Class Field and Property

        private string m_InputXMLPath;
        //private XPathDocument m_InputXPHDoc;
        //private XPathNavigator m_RootNavg;
        private XDocument m_XLinqInputDoc;
        private XElement m_XLinqRoot;

        // Root tag
        private const string XCFG_ROOT = "InputDefine";

        //The tags used in project
        private const string XCFG_PROJECT = "Project";
        private const string XCFG_NEXTNODE = "NextNode";

        //The tags used in input format
        private const string XCFG_INPUTFORMAT = "InputFormat";
        private const string XCFG_GLOBALSETTING = "GlobalSetting";
        private const string XCFG_TELEGRAM = "Telegram";
        private const string XCFG_TLGMTYPE = "TelegramType";
        private const string XCFG_NODES = "Nodes";
        private const string XCFG_NODE = "Node";
        private const string XCFG_NODENAME = "NodeName";

        private const string XCFG_TLGMALIAS = "TlgmAlias";
        private const string XCFG_TLGMFIELD = "TlgmField";

        // attributes for fields
        private const string XCFG_ATTB_DEPENDTYPE = "DependType";
        private const string XCFG_ATTB_DECIDEFIELD = "DecideField";


        //The pre-defined value for attributes
        private const string PDVAL_DCFLD_TRUE = "true";
        private const string PDVAL_DPTYPE_INPUT = "input";
        private const string PDVAL_DPTYPE_TLGM = "telegram";

        //common xpath used in this class
        //private string XPH_GlobalTelegram;
        //private string XPH_Node;
        private IEnumerable<XElement> XELMs_GlobalTelegrams;
        private IEnumerable<XElement> XELMs_Nodes;

        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Class Constructor, Dispose, & Destructor

        /// <summary>
        /// Use a FilePath to initialize the Class with Linq. This class is used to analyze the CFG_InputDefine.xml
        /// </summary>
        /// <param name="FilePath"></param>
        public XmlInput(string FilePath)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            try
            {
                this.m_InputXMLPath = FilePath;
                this.m_XLinqInputDoc = XDocument.Load(FilePath);
                this.m_XLinqRoot = this.m_XLinqInputDoc.Root;

                this.XELMs_GlobalTelegrams = this.m_XLinqRoot.Element(XCFG_INPUTFORMAT).Element(XCFG_GLOBALSETTING).Elements(XCFG_TELEGRAM);
                this.XELMs_Nodes = this.m_XLinqRoot.Element(XCFG_INPUTFORMAT).Element(XCFG_NODES).Elements(XCFG_NODE);
            }            
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }
        }

        ~XmlInput()
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

            if (XELMs_GlobalTelegrams != null) XELMs_GlobalTelegrams = null;
            if (XELMs_Nodes != null) XELMs_Nodes = null;
            if (m_XLinqInputDoc != null) m_XLinqInputDoc = null;
            if (m_XLinqRoot != null) m_XLinqRoot = null;

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
        /// Get <NextNode> part for specific project name and check-in line name
        /// </summary>
        /// <param name="projname"></param>
        /// <param name="CLineName"></param>
        /// <returns></returns>
        public XElement GetEntryPoint(string projname,string CLineName)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";
            try
            {
                if (this.m_XLinqRoot == null
                    || this.m_XLinqRoot.Element(XCFG_PROJECT) == null
                    || this.m_XLinqRoot.Element(XCFG_PROJECT).Element(projname) == null
                    || this.m_XLinqRoot.Element(XCFG_PROJECT).Element(projname).Element(CLineName) == null)
                {
                    return null;
                }

                return this.m_XLinqRoot.Element(XCFG_PROJECT).Element(projname).Element(CLineName).Element(XCFG_NEXTNODE);
            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NodeName"></param>
        /// <param name="TlgmType"></param>
        /// <returns></returns>
        public string[] GetAllFieldsFromNode(string NodeName, string TlgmType)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";
            string[] fields = null;

            if (this.XELMs_GlobalTelegrams == null || this.XELMs_Nodes == null)
                return null;

            try
            {
                var xelm_fieldnames = from xnode in this.XELMs_Nodes
                                      where xnode.Element(XCFG_NODENAME).Value == NodeName
                                      from xtlgm in xnode.Elements(XCFG_TELEGRAM)
                                      where xtlgm.Element(XCFG_TLGMTYPE).Value == TlgmType
                                      from field in xtlgm.Elements()
                                      where field.Name != XCFG_TLGMTYPE
                                      select field.Name;

                if (xelm_fieldnames != null && xelm_fieldnames.Any())
                {
                    int cnt = xelm_fieldnames.Count();
                    fields = new string[cnt];

                    int i = 0;
                    foreach (var xfd in xelm_fieldnames)
                    {
                        fields[i] = xfd.ToString();
                        i++;
                    }
                }
                else
                {
                    errstr += "Cannot find Fields for Node Name:" + NodeName + ", Telegram Type:" + TlgmType;
                    _logger.Warn(errstr);
                }
            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return fields;
        }
        
        /// <summary>
        /// Get value with XElement type for specific Node Name, Telegram Name, Field Name in &lt;Nodes&gt; part. 
        /// And return Depend type("input","Telegram","") in an out-parameter
        /// </summary>
        /// <param name="NodeName"></param>
        /// <param name="TlgmType"></param>
        /// <param name="FieldName"></param>
        /// <param name="DpndType"></param>
        /// <returns></returns>
        public XElement GetXValFromNode(string NodeName, string TlgmType, string FieldName, out string DpndType)
        {
            
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            XElement xval = null;
            DpndType = "";

            if (this.XELMs_GlobalTelegrams == null || this.XELMs_Nodes == null)
                return null;

            try
            {
                var fields = from tnode in this.XELMs_Nodes
                             where tnode.Element(XCFG_NODENAME).Value == NodeName
                             from tlgm in tnode.Elements(XCFG_TELEGRAM)
                             where tlgm.Element(XCFG_TLGMTYPE).Value == TlgmType
                             from field in tlgm.Elements()
                             where field.Name == FieldName
                             select field;
                if (fields != null && fields.Any())
                {
                    xval = fields.First<XElement>();
                    if (fields.First<XElement>().Attribute(XCFG_ATTB_DEPENDTYPE) != null)
                        DpndType = fields.First<XElement>().Attribute(XCFG_ATTB_DEPENDTYPE).Value;
                }
                else
                {
                    errstr += "Cannot find XElement for Node Name:" + NodeName + ", Telegram Type:" + TlgmType + ", Field Name:" + FieldName;
                    _logger.Warn(errstr);
                }
            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
                xval = null;
                DpndType = null;
            }

            return xval;
        }
        

        /// <summary>
        /// Get value with XElement type for specific Telegram Name, Field Name in &lt;GlobalSetting&gt; part. 
        /// And return Depend type("input","Telegram","") in an out-parameter
        /// </summary>
        /// <param name="TlgmType"></param>
        /// <param name="FieldName"></param>
        /// <param name="DpndType"></param>
        /// <returns></returns>
        public XElement GetXValFromGlobal(string TlgmType,string FieldName, out string DpndType)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            XElement xval = null;
            DpndType = "";

            if (this.XELMs_GlobalTelegrams == null || this.XELMs_Nodes == null)
                return null;

            try
            {
                var fields = from tlgm in this.XELMs_GlobalTelegrams
                             where tlgm.Element(XCFG_TLGMTYPE).Value == TlgmType
                             from field in tlgm.Elements()
                             where field.Name == FieldName
                             select field;
                if (fields != null && fields.Any())
                {
                    xval = fields.First<XElement>();
                    if (fields.First<XElement>().Attribute(XCFG_ATTB_DEPENDTYPE) != null)
                        DpndType = fields.First<XElement>().Attribute(XCFG_ATTB_DEPENDTYPE).Value;
                }
                else
                {
                    errstr += "Cannot find XElement for Global. Telegram Type:" + TlgmType + ", Field Name:" + FieldName;
                    _logger.Warn(errstr);
                }
            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return xval;
        }

        
        /// <summary>
        /// Get value of decide field with string type for specified Node Name in &lt;Nodes&gt; part. 
        /// And return decided telegram and field name in out-parameters
        /// </summary>
        /// <param name="NodeName"></param>
        /// <param name="DecdTlgm"></param>
        /// <param name="DecdField"></param>
        /// <returns></returns>
        public string GetValFromDecide(string NodeName, out string DecdTlgm, out string DecdField)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            DecdTlgm = "";
            DecdField = "";
            string val="";

            if (this.XELMs_GlobalTelegrams == null || this.XELMs_Nodes == null)
                return null;

            try
            {
                var fields = from tnode in this.XELMs_Nodes
                             where tnode.Element(XCFG_NODENAME).Value == NodeName
                             from tlgm in tnode.Elements(XCFG_TELEGRAM)
                             from field in tlgm.Elements()
                             where field.Name != XCFG_TLGMTYPE
                             && field.Attribute(XCFG_ATTB_DECIDEFIELD) != null
                             && field.Attribute(XCFG_ATTB_DECIDEFIELD).Value == PDVAL_DCFLD_TRUE
                             select field;
                if (fields != null && fields.Any())
                {
                    val = fields.First<XElement>().Value;
                    DecdField = fields.First<XElement>().Name.ToString();
                    XElement curr_field = fields.First<XElement>();
                    if (curr_field.ElementsBeforeSelf(XCFG_TLGMTYPE).Any())
                        DecdTlgm = curr_field.ElementsBeforeSelf(XCFG_TLGMTYPE).First().Value;
                }
                else
                {
                    errstr += "Cannot find XElement for Decide Node. Node Name:" + NodeName;
                    _logger.Error(errstr);
                }
            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return val;
        }

        /// <summary>
        /// Get value with string type for specific path in "NodeName\TelegramType\FieldName" format
        /// And return Depend type("input","Telegram","") in an out-parameter 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="DpndType"></param>
        /// <returns></returns>
        public XElement GetXValFromPath(string path, out string DpndType)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            XElement xval = null;
            DpndType = "";

            if (this.XELMs_GlobalTelegrams == null || this.XELMs_Nodes == null)
                return null;

            string[] tagpath = path.Split('\\');
            if (tagpath.Length == 3)
            {
                string nodename = tagpath[0];
                string tlgmtype = tagpath[1];
                string fieldname = tagpath[2];
                
                if(nodename=="Global")
                {
                    xval = this.GetXValFromGlobal(tlgmtype, fieldname, out DpndType);
                }
                else
                {
                    xval = this.GetXValFromNode(nodename, tlgmtype, fieldname, out DpndType);
                }

            }
            else
            {
                errstr += "The path should have 3 part: NodeName\\TelegramType\\FieldName. Path:" + path;
                _logger.Error(errstr);
            }

            if (xval == null)
            {
                errstr += "Cannot find XElement for Path:" + path;
                _logger.Error(errstr);
            }

            return xval;

        }

        /// <summary>
        /// Analyse the field with DependField="Telegram" to get the depended telegram alias and field
        /// </summary>
        /// <param name="XField"></param>
        /// <param name="AliasVal"></param>
        /// <param name="FieldVal"></param>
        /// <returns></returns>
        public bool ParseTlgmFLD(XElement XField, out string AliasVal, out string FieldVal)
        {
            AliasVal = "";
            FieldVal = "";
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            if (XField.HasAttributes
                && XField.Attribute(XCFG_ATTB_DEPENDTYPE) != null
                && XField.Attribute(XCFG_ATTB_DEPENDTYPE).Value == PDVAL_DPTYPE_TLGM)
            {
                try
                {
                    AliasVal = XField.Element(XCFG_TLGMALIAS).Value;
                    FieldVal = XField.Element(XCFG_TLGMFIELD).Value;
                }
                catch (Exception exp)
                {
                    errstr += exp.ToString();
                    _logger.Error(errstr);
                    return false;
                }
            }
            else
            {
                errstr += "This Field does not have attribute[" + XCFG_ATTB_DEPENDTYPE + "] with type[" + PDVAL_DPTYPE_TLGM + "].";
                _logger.Error(errstr);
                return false;
            }

            
            return true;
        }

        /// <summary>
        /// Find input pos in CFG_InputDefine.xml for specific Node Name, Telegram Type and Field Name
        /// </summary>
        /// <param name="NodeName"></param>
        /// <param name="TlgmType"></param>
        /// <param name="FieldName"></param>
        /// <returns></returns>
        public string FindInputPos(string NodeName, string TlgmType, string FieldName)
        {
            string val = "";
            string dpndtype="";
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            if (this.XELMs_GlobalTelegrams == null || this.XELMs_Nodes == null)
                return null;

            // Search in <Nodes> part
            XElement xval = this.GetXValFromNode(NodeName, TlgmType, FieldName, out dpndtype);
            // If cannot find value in <Nodes> part, then search <GlobalSetting> part
            if (xval == null)
            {
                xval = GetXValFromGlobal(TlgmType, FieldName, out dpndtype);
            }
            while (xval != null && dpndtype != PDVAL_DPTYPE_TLGM && dpndtype != "" && dpndtype != null)
            {
                if (dpndtype == PDVAL_DPTYPE_INPUT)
                {
                    string path = xval.Value;
                    xval = this.GetXValFromPath(path, out dpndtype);
                }
                else
                {
                    errstr += "Unknow Depend Type:" + dpndtype;
                    _logger.Error(errstr);
                    xval = null;
                    break;
                }
            }

            // Find pos in <Nodes> part without depending telegram
            if (xval != null && dpndtype != PDVAL_DPTYPE_TLGM)
            {
                val = xval.Value;
            }
            // Find value in <Nodes> part with depending telegram
            else if (xval != null && dpndtype == PDVAL_DPTYPE_TLGM)
            {
                string tlgmtype, tlgmfld;
                ParseTlgmFLD(xval, out tlgmtype, out tlgmfld);
                val = "Global@" + tlgmtype + '@' + tlgmfld;
            }
            // cannot find value
            else
            {
                errstr += "Cannot Find Value for Node Name:" + NodeName + ", Telegram Type:" + TlgmType + ", Field Name:" + FieldName;
                _logger.Error(errstr);
            }
            return val;
        }


        #endregion
    }
}
