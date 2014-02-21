using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Linq;
using BHS;
using BHS.PLCSimulator;
using BHS.PLCSimulator.Controller;
using BHS.PLCSimulator.Messages.Telegram;
using BHS.PLCSimulator.Messages.TelegramFormat;

namespace TestProgram
{
    static class Test_XmlHLCTester
    {
        public static void test(string filepath)
        {
            XmlHLCTester hlctester = new XmlHLCTester(filepath);

            Console.WriteLine(hlctester.DefaultDistance + " " + hlctester.DefaultSpeed);

            //GetAllLocDpndNodes
            string[] dpnd_lname = hlctester.GetAllLocDpndNodes();
            Console.Write("Depended normal nodes: ");
            foreach (string name in dpnd_lname)
                Console.Write(name + " ");
            Console.WriteLine();

            //GetAllTlgmDpndNodes
            string[] dpnd_tname = hlctester.GetAllTlgmDpndNodes();
            Console.Write("Depended telegram nodes: ");
            foreach (string name in dpnd_tname)
                Console.Write(name + " ");
            Console.WriteLine();

            //GetDefalutValue
            string nodetype="BDD";
            string tlgmalias="BMAM";
            XmlNode dfval = hlctester.GetDefalutValue(nodetype, tlgmalias);
            Console.WriteLine(dfval.OuterXml);
            //General_Telegram tlgm = new General_Telegram(dfval,tlgmalias);

            //GetDirection
            nodetype = "EDS";
            tlgmalias = "ICR";
            string data = "22";
            string clr_d = hlctester.GetDirection(nodetype, tlgmalias, data);
            Console.WriteLine(data + ":" + clr_d);
            data = "24";
            string alm_d = hlctester.GetDirection(nodetype, tlgmalias, data);
            Console.WriteLine(data + ":" + alm_d);

            //GetDpndNodeByNode
            string nodename="MU1#IPR";
            string[] dpndnodes;
            string[] dpndtypes;
            dpndnodes = hlctester.GetDpndNodeByNode(nodename, out dpndtypes);
            foreach (string nd in dpndnodes)
                Console.Write(nd + " ");
            Console.WriteLine();
            foreach (string ndty in dpndtypes)
                Console.Write(ndty + " ");
            Console.WriteLine();

            // GetLocationByNode
            nodename = "MU1#IPR";
            string loc = hlctester.GetLocationByNode(nodename);
            Console.WriteLine("LOC: " + loc);

            //GetSpecialTlgm
            nodetype = "MUC";
            string tlgmtype = "random";
            double[] rates;
            string[] tlgms = hlctester.GetSpecialTlgm(nodetype, tlgmtype, out rates);
            Console.Write("Telegram: ");
            foreach (string tlgm in tlgms)
                Console.Write(tlgm + " ");
            Console.WriteLine();
            Console.Write("Rate: ");
            foreach (double r in rates)
                Console.Write(r.ToString() + " ");
            Console.WriteLine();

            //GetTlgmDpndField
            tlgmalias = "IEC";
            string fdname, fdtype;
            hlctester.GetTlgmDpndField(tlgmalias, out fdname, out fdtype);
            Console.WriteLine("FDname:" + fdname + " FDtype:" + fdtype);

            //GetTypeByNode
            nodename = "MU1#IPR";
            string ndtp = hlctester.GetTypeByNode(nodename);
            Console.WriteLine("GetTypeByNode: " + ndtp);

            //XGetFirstNextNode
            nodename = "MU1#IPR";
            XElement fnxnd = hlctester.XGetFirstNextNode(nodename);
            Console.WriteLine("XGetFirstNextNode:");
            Console.WriteLine(fnxnd);

            //XGetNextNode
            string dirction = "RM";
            XElement nxnd = hlctester.XGetNextNode(nodename, dirction);
            Console.WriteLine("XGetNextNode:");
            Console.WriteLine(nxnd);

            //XGetSecureNXND
            XElement scnxnd = hlctester.XGetSecureNXND(nodename);
            Console.WriteLine("XGetSecureNXND:");
            Console.WriteLine(scnxnd);

            nodename = "ME1-9#IRY";
            fnxnd = hlctester.XGetFirstNextNode(nodename);
            
            //ParseNextNode_Name
            string nxnd_name = hlctester.ParseNextNode_Name(fnxnd);

            //ParseNextNode_isSend
            bool nxnd_isSend = hlctester.ParseNextNode_isSend(fnxnd);

            //ParseNextNode_PRDLOC
            string nxnd_prdloc = hlctester.ParseNextNode_PRDLOC(fnxnd);

            //ParseNextNode_Distance
            string nxnd_dist = hlctester.ParseNextNode_Distance(fnxnd);

            //ParseNextNode_Speed
            string nxnd_speed = hlctester.ParseNextNode_Speed(fnxnd);

            Console.WriteLine("ParseNextNode");
            Console.WriteLine("Next Node: " + nxnd_name);
            Console.WriteLine("IsSend: " + nxnd_isSend);
            Console.WriteLine("PRDLOC: " + nxnd_prdloc);
            Console.WriteLine("Distance: " + nxnd_dist);
            Console.WriteLine("Speed: " + nxnd_speed);
            Console.WriteLine();
        }
    }
}
