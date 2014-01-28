using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
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

            string str = "";
            char[] tch = str.ToCharArray();
            char[] ch = Util.CharPad(str.ToCharArray(),8);

            string output = ">" + new string(ch) + "<";

            Console.WriteLine(output);

            Console.Read();
        }
    }
}
