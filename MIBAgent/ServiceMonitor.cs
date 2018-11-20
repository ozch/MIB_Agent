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
            return number;
        }
        public void SetNumber(int x)
        {
            number = x;
        }
        public string GetProcessedString(string p)
        {
            int place = p.LastIndexOf(",");
            p = p.Remove(place,1);
            return p;
        }
        public string GetJson()
        {
            try
            {
                int i = 0;
                string str = "{\"services\":["; 
                ManagementObjectSearcher searcher =new ManagementObjectSearcher("root\\CIMV2","SELECT * FROM Win32_Service");
                foreach (ManagementObject queryObj in searcher.Get())
                {

                    i++;
                    str = str + string.Format("\"{0}\",",queryObj["Name"]);

                }
                str = str + "]}";
                SetNumber(i);
                str = GetProcessedString(str);
                return str;

            }
            catch (ManagementException e)
            {
                return "-1";
            }
        }
    }
}
