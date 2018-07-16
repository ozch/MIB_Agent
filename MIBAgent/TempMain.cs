using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBAgent
{
    class TempMain
    {
        public static void Main(string[] args)
        {

            InitialDataCollector orm = new InitialDataCollector();
                Console.WriteLine(orm.GetJsonPretty());
                Console.ReadKey();
        }
    }
}

