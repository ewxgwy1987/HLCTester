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
        private const string Path_XMLFileTelegram = @"../../../cfg/CFG_Telegrams.xml";
        private const string Path_XMLFileSetting = @"../../../cfg/CFG_PLCSimulator.xml";
        private const string Path_XMLFileTestCase = @"../../../cfg/CFG_TestCase.xml";
        private const string xmlinput_path = @"../../../cfg/CFG_InputDefine.xml";
        private const string xmltester_path = @"../../../cfg/CFG_HLCTester.xml";

        static void Main(string[] args)
        {
            int i;

            FileInfo[] cfgFiles = new FileInfo[3];
            cfgFiles[0] = new FileInfo(Path_XMLFileSetting); // CFG_Simulator.xml
            cfgFiles[1] = new FileInfo(Path_XMLFileTelegram); // CFG_Telegram.xml
            cfgFiles[2] = new FileInfo(Path_XMLFileTestCase); // CFG_TestCase.xml

            Initializer _init = new BHS.PLCSimulator.Initializer(cfgFiles);
            _init.Init();

            Test_XmlInput.test(xmlinput_path);
            //Test_XmlHLCTester.test(xmltester_path);

            
            str = "teststr";
            Thread thrd_test = new Thread(ThrdFun_Test);
            thrd_test.IsBackground = true;
            thrd_test.Start(str);

            Console.Read();
            _init.Dispose();
            _init = null;
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
