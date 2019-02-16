using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBAgent
{
    class ProcessModel
    {
        public String pn;
        public long pid;
        public long ru;
        public int ppt;
        public ProcessModel(string pn,long pid,long ru,int ppt)
        {
            this.pn = pn;
            this.pid = pid;
            this.ru = ru;
            this.ppt = ppt;
        }
    }
}
