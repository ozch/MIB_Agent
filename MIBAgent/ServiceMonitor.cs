using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MIBAgent
{
    class ServiceMonitor
    {
        private int number = 0;
        public int GetNumber()
        {
            return this.number;
        }
        public void SetNumber(int x)
        {
            this.number = x;
        }
        
        public string GetJson()
        {
            try
            {
                int i = 0;
                IDictionary<int, ServiceModel> list = new Dictionary<int, ServiceModel>();
                ManagementObjectSearcher searcher =new ManagementObjectSearcher("root\\CIMV2","SELECT * FROM Win32_Service");
                foreach (ManagementObject queryObj in searcher.Get())
                {

                    list.Add(i, new ServiceModel(Convert.ToString(queryObj["Name"]), Convert.ToString(queryObj["State"])));
                    i++;
                    

                }
                SetNumber(i);
                return JsonConvert.SerializeObject(list);

            }
            catch (ManagementException e)
            {
                return "-1";
            }
        }
    }
}
