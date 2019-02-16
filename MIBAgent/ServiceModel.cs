using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBAgent
{
    class ServiceModel
    {
        public String name;
        public String status;
        public ServiceModel(String name,String status)
        {
            this.name = name;
            this.status = status;
        }
    }
}
