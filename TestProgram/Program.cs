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
using PALS.Configure;
using PALS.Utilities;
using PALS.Telegrams;

namespace TestProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            int i;

            string xmlpath1 = @"../../../cfg/CFG_TestCase.xml";


            Console.WriteLine("------------------Linq XML-----------------");
            XDocument xdoc = XDocument.Load(xmlpath1);
            var tlgm_list = from tlgm in xdoc.Descendants("telegram")
                            where tlgm.Attribute("alias").Value == "ICR"
                            select tlgm;
                            //new
                            //{
                            //    location = tlgm.Element("LOCATION").Value,
                            //    lvl = tlgm.Element("SCR_LVL").Value,
                            //    res = tlgm.Element("SCR_RES").Value,
                            //};
            //foreach (XElement xmlstr in tlgm_list)
            foreach (var xmlstr in tlgm_list)
            {
                Console.WriteLine(xmlstr.ToString());
            }

            Console.WriteLine("------------------Xpath XML-----------------");
            string xpath = "/configuration/telegram[PLC_IDX='207']";
            string xpath2 = "child::LOCATION";
            string xpath3 = "./LOCATION";
            XPathDocument xpdoc = new XPathDocument(xmlpath1);
            XPathNavigator xpnav = xpdoc.CreateNavigator();
            XPathNodeIterator xpnoditer = xpnav.Select(xpath);
            //xpnav.Evaluate(xpath);
            while (xpnoditer.MoveNext())
            {
                Console.WriteLine(xpnoditer.Current.OuterXml);
                XPathNodeIterator lociter = xpnoditer.Current.Select(xpath2);
                while (lociter.MoveNext())
                {
                    Console.WriteLine("location: " + lociter.Current.Value);
                }

                XPathNodeIterator testiter = xpnoditer.Current.Select(xpath3);
                while (testiter.MoveNext())
                {
                    Console.WriteLine("location2: " + lociter.Current.Value);
                }
                Console.WriteLine("**************************************");
                //Console.WriteLine(xpnoditer.Current.GetAttribute();
            }

            //Console.WriteLine(xmlpath1);

            Console.Read();
        }
    }
}
