using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBAgent
{
    class OpenPortModel
    {
        public string type;
        public string address;
        public string process;

        public OpenPortModel(string type, string address, string process)
        {
            this.type = type;
            this.address = address;
            this.process = process;
        }
    }
}
