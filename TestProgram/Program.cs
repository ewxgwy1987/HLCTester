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
            for (i = 0; i < 10; i++)
            {
                Console.WriteLine(Util.GetGID());
                Thread.Sleep(20);
            }
            Console.Read();
        }
    }
}
