using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MIBAgent
{
    //This class provides you with the method to monitor all the runnig processes in the operating system
    class ProcessMonitor
    {
        //Return String All PrcessList and their usage in formated form
        public string GetString()
        {
            Console.WriteLine("Processing... Please Wait!");
            string str = "List Of Processes Running... \n ";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PerfFormattedData_PerfProc_Process");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                str = str + string.Format("{0} \t\t PID: {1} \t RAM:{2}MB \t CPU:{3}% \n ", queryObj["Name"], queryObj["IDProcess"], Math.Round(Convert.ToDouble(queryObj["WorkingSet"]) / 1048576,0), queryObj["PercentProcessorTime"]);
            }
            return str;
        }
        /*
         return Json file containing the process list with process name pid cpu usage in percent and ram uage in mbs for each process 
         Example
            {
              "0": {
                "pn": "dwm",
                "id": 984,
                "ru": "28MB",
                "cpu": "0"
              },
              "1": {
                "pn": "sqlservr",
                "id": 2756,
                "ru": "11MB",
                "cpu": "0"
              }
              ................
              ................
              .  .  .  .  .  .
              .  .  .  .  .  .
            }
         
         
         */
        private int Number = 0;
        public int GetNumber()
        {
            return Number;
        }
        public void SetNumber(int x)
        {
            Number = x;
        }
        public string GetJson()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2","SELECT * FROM Win32_PerfFormattedData_PerfProc_Process");
            IDictionary<int, ProcessModel> list = new Dictionary<int, ProcessModel>();
            int i = 0;
            foreach (ManagementObject queryObj in searcher.Get())
            {
                list.Add(i, new ProcessModel(Convert.ToString(queryObj["Name"]), Convert.ToInt64(queryObj["IDProcess"]), Convert.ToInt64(Math.Round(Convert.ToDouble(queryObj["WorkingSet"]) / 1048576, 0)), Convert.ToInt32(queryObj["PercentProcessorTime"])));
                i++;
            }
            SetNumber(i);
            return JsonConvert.SerializeObject(list); ;
        }
       
        
        public double[] GetProcessUsage(Process p)
        {
            //Process p = new Process();
            PerformanceCounter ramCounter = new PerformanceCounter("Process", "Working Set", p.ProcessName);
            PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", p.ProcessName);
            System.Threading.Thread.Sleep(10);
            double ram = Math.Round(ramCounter.NextValue() / 1024 / 1024, 0);
            double cpu = Math.Round(cpuCounter.NextValue(),0);
            double[] info = {ram,cpu};
            //Console.WriteLine("RAM: "+(ram/1024/1024)+" MB; CPU: "+(cpu)+" %");
            return info;
        }
    }
}
