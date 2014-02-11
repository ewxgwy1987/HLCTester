using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using System.IO;
using System.Threading;

using BHS;
using BHS.PLCSimulator;
using BHS.PLCSimulator.Controller;
using PALS.Configure;
using PALS.Utilities;
using PALS.Telegrams;

namespace TestProgram
{

    class Program
    {
        private static string str;

        static void Main(string[] args)
        {
            int i;

            string xmlpath = @"../../../cfg/CFG_InputDefine.xml";
            string Path_XMLFileSetting = @"../../../cfg/CFG_PLCSimulator.xml";
            FileInfo FI_Setting = new FileInfo(Path_XMLFileSetting);

            XmlElement xmlRoot = PALS.Utilities.XMLConfig.GetConfigFileRootElement(ref FI_Setting);
            XmlElement log4netConfig = (XmlElement)PALS.Utilities.XMLConfig.GetConfigSetElement(ref xmlRoot, "log4net");
            log4net.Config.XmlConfigurator.Configure(log4netConfig);

            XmlInput xin = new XmlInput(xmlpath);
            XElement xval;
            string val, dpndtype;
            string nodename = "SB1-13#1500P";
            string tlgmtype = "1500P";
            string fieldname = "LIC";
            //xval = xin.GetValFromNode(nodename, tlgmtype, fieldname, out dpndtype);
            //Console.WriteLine("{0} - {1}", xval, dpndtype);

            //xval = xin.GetValFromGlobal("ISE", "TYPE", out dpndtype);
            //Console.WriteLine("{0} - {1}", xval, dpndtype);

            //val = xin.GetValFromDecide("ED1-13#ICR", out dpndtype);
            //Console.WriteLine("{0} - {1}", val, dpndtype);

            nodename = "SB1-13#1500P";
            tlgmtype = "1500P";
            fieldname = "LIC";
            val = xin.FindInputPos(nodename, tlgmtype, fieldname);
            Console.WriteLine(val);
            fieldname = "TYPE";
            val = xin.FindInputPos(nodename, tlgmtype, fieldname);
            Console.WriteLine(val);
            nodename = "Global";
            tlgmtype = "ISE";
            fieldname = "TYPE";
            val = xin.FindInputPos(nodename, tlgmtype, fieldname);
            Console.WriteLine(val);
            nodename = "SS1-2#BMAM";
            tlgmtype = "BMAM";
            fieldname = "TYPE";
            val = xin.FindInputPos(nodename, tlgmtype, fieldname);
            Console.WriteLine(val);

            string tlgmfld;
            val = xin.GetValFromDecide(nodename, out tlgmtype, out tlgmfld);
            Console.WriteLine("{0} - {1} - {2}", val, tlgmtype, tlgmfld);
            nodename = "ML1-2#ISC";
            tlgmtype = "ISC";
            fieldname = "SCN_STS";
            val = xin.FindInputPos(nodename, tlgmtype, fieldname);
            Console.WriteLine(val);

            nodename = "ML1-2#ISC";
            tlgmtype = "ISC";
            fieldname = "SCN_STS";
            val = xin.FindInputPos(nodename, tlgmtype, fieldname);
            Console.WriteLine(val);

            //Console.WriteLine("------------------Xpath XML-----------------");
            //string xpath = "/configuration/telegram[PLC_IDX='207']";
            //string xpath2 = "child::LOCATION";
            //string xpath3 = "./LOCATION";
            //XPathDocument xpdoc = new XPathDocument(xmlpath1);

            //XPathNavigator xpnav = xpdoc.CreateNavigator();
            //XPathNodeIterator xpnoditer = xpnav.Select(xpath);
            ////xpnav.Evaluate(xpath);
            //while (xpnoditer.MoveNext())
            //{
            //    Console.WriteLine(xpnoditer.Current.OuterXml);
            //    XPathNodeIterator lociter = xpnoditer.Current.Select(xpath2);
            //    while (lociter.MoveNext())
            //    {
            //        Console.WriteLine("location: " + lociter.Current.Value);
            //    }

            //    XPathNodeIterator testiter = xpnoditer.Current.Select(xpath3);
            //    while (testiter.MoveNext())
            //    {
            //        Console.WriteLine("location2: " + lociter.Current.Value);
            //    }
            //    Console.WriteLine("**************************************");
            //    //Console.WriteLine(xpnoditer.Current.GetAttribute();
            //}

            //Console.WriteLine(xmlpath1);
            str = "teststr";
            Thread thrd_test = new Thread(ThrdFun_Test);
            thrd_test.IsBackground = true;
            thrd_test.Start(str);

            Console.Read();
        }

        static private void ThrdFun_Test(object args)
        {
            string outstr = (string)args;
            str = "new string";
            Console.WriteLine(outstr);
        }
    }

    public class tester
    {
        public tester(string str)
        {
            Console.WriteLine("haha");
            Console.WriteLine(str);
        }
    }
}
