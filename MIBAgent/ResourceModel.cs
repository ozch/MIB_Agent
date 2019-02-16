using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBAgent
{
    class ResourceModel
    {
        public double ram_used_gb;
        public double ram_free_gb;
        public double ram_used_per;
        public double ram_free_per;
        public double cpu_load;
        public ResourceModel(double ram_used_gb, double ram_free_gb, double ram_used_per, double ram_free_per, double cpu_load)
        {
            this.ram_used_gb = Math.Round(ram_used_gb,2);
            this.ram_free_gb = Math.Round(ram_free_gb,2);
            this.ram_used_per = Math.Round(ram_used_per,2);
            this.ram_free_per = Math.Round(ram_free_per,2);
            this.cpu_load = Math.Round(cpu_load,1);
        }
    }
}
