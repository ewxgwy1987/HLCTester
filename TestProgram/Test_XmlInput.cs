using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Linq;
using BHS;
using BHS.PLCSimulator;
using BHS.PLCSimulator.Controller;

namespace TestProgram
{
    static class Test_XmlInput
    {
        public static void test(string filepath)
        {
            XmlInput xin = new XmlInput(filepath);
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

            string[] fds = xin.GetAllFieldsFromNode(nodename, tlgmtype);
            Console.Write("Fields: ");
            if (fds != null)
            {
                foreach (string fd in fds)
                    Console.Write(fd + " ");
                Console.WriteLine();
            }

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
        }
    }
}
