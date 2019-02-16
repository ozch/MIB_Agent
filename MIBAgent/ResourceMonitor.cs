using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MIBAgent
{
    class ResourceMonitor
    {
        private double mem_size = 1;
        private double max_clk_sp = 1;
        
        public ResourceMonitor()
        {
            mem_size = GetTotalMemoryInGigaBytes();
            max_clk_sp = GetMaxClockSpeed();
        }

        public string GetProcessedString(string p)
        {
            int place = p.LastIndexOf(",");
            p = p.Remove(place,1);
            return p;
        }
        
        // this function return the Disk storage of all device as json (Storage in GB)
        public string GetDisksStorageJson()
        {
            try
            {
                string str = "{";
                int i = 0;
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\Microsoft\\Windows\\Storage", "SELECT * FROM MSFT_Disk");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    str = str + string.Format("\"{0}\": \"{1}\" , ", i, Math.Round(Convert.ToDouble(queryObj["Size"]) / 1073741824,0));
                    i++;
                }
                str = GetProcessedString(str) + "}";
                return str;
            }
            catch (ManagementException e)
            {
                return "-1";
            }
        }


        //This function GetUsedRamPercet() returns the Amount of Ram used by processes in GBs
        //This function is dependent of GetUsedRamPercent();
        public double GetUsedRamPercent()
        {
            double mem_uti = GetUsedRamSize();
            double mem_per = Math.Round((mem_uti/mem_size)*100, 2);
            return mem_per;
        }
        public double GetAvailableRamPercentage()
        {
            return 100 - GetUsedRamPercent();
        }

        //This function GetUsedRamSize() returns the Amount of Ram used by processes in GBs
        public double GetUsedRamSize()
        {
            double mem_free_mb;
            PerformanceCounter ram = new PerformanceCounter("Memory", "Available MBytes");
            mem_free_mb = ram.NextValue();
            
            double mem_free_gb = Math.Round(mem_free_mb/1024, 2);
            double mem_uti_gb = mem_size - mem_free_gb;
            return mem_uti_gb;
        }

        public double GetAvailableRamSize()
        {
            return mem_size - GetUsedRamSize();
        }

        /*
         This function uses load to Aproximately determine the Current Clock Speed of CPU
         
         * Which is not right method 
         
         * Recommandation to Change this method or not use it at all
         
         */
        public double GetCurrentClockSpeed()
        {
            double current_clk_sp;
            double load_per = GetCPULoad();
            current_clk_sp = Math.Round(((load_per / 100) * max_clk_sp), 2);
            return current_clk_sp;
        }


        //This Function resturns the CPU Load in Percentage 
        //for load <30% doesn't work correctly but is acceptable becuase  non of the other work
        //p.s. CPU Load has nothing to do with Clock speed they are somewhat different things
        private double GetCPULoad()
        {

            PerformanceCounter cpu;
            cpu = new PerformanceCounter();
            cpu.CategoryName = "Processor";
            cpu.CounterName = "% Processor Time";
            cpu.InstanceName = "_Total";
            cpu.NextValue();
            System.Threading.Thread.Sleep(500);
            return Math.Round(Convert.ToDouble(cpu.NextValue()),0);
        }
        //For Deletion
        public string Calling()
        {
            ResourceMonitor orm = new ResourceMonitor();
            while (true)
            {
                Console.WriteLine("mem_size : {0}GB", orm.mem_size);
                Console.WriteLine("max_clk_sp : {0}Ghz\n", orm.max_clk_sp);
                Console.WriteLine("GetCPULoad() : {0}", orm.GetCPULoad());
                Console.WriteLine("GetUsedRam() : {0}GB", orm.GetUsedRamSize());
                Console.WriteLine("GetUsedRamPercent() : {0}%", orm.GetUsedRamPercent());
                System.Threading.Thread.Sleep(2000);
                Console.Clear();
            }
            
        }

       
        /*
        *These three funtion are copied from InitialDataCollector.cs
        *and are made private only used when constructor is used
        */
        private double GetTotalMemoryInMegaBytes()
        {
            double mem = Convert.ToDouble(new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory);
            //1048576 = 1024*1024 to convert byte in MegaBy 
            double mem_mb = Math.Round(mem / 1048576, 2);
            return mem_mb;
        }
        
        private double GetTotalMemoryInGigaBytes()
        {
            double mem_mb = GetTotalMemoryInMegaBytes();
            double mem_gb = Math.Round(mem_mb / 1024, 2);
            return mem_gb;
        }
        private double GetMaxClockSpeed()
        {
            uint max_sp;
            using (ManagementObject Mo = new ManagementObject("Win32_Processor.DeviceID='CPU0'"))
            {
                max_sp = (uint)(Mo["MaxClockSpeed"]);
            }
            //Divid by 10^3 to Conver MHz into GHz
            double max_sp_d = Math.Round(((double)max_sp) / 1000, 2);
            return max_sp_d; //Converting in GHz
        }

        public string GetString()
        {

            Console.WriteLine("Getting Resource Info String");
            string result = "\n==========================="
                + "\nRam used(GB): " + GetUsedRamSize()
                + "\nRam used(Percent): " + GetUsedRamPercent() + "%"
                + "\nAvailable Ram Percentage: " + GetAvailableRamPercentage() + "%"
                + "\nAvailable Ram Size: " + GetAvailableRamSize()
                //+ "\nCurrent Clock Speed" + GetCurrentClockSpeed()
                + "\nCPU Load: " + GetCPULoad() + "%"
            ;
            return result;
        }

        public string GetJson()
        {
            string result = JsonConvert.SerializeObject(new ResourceModel(GetUsedRamSize(), GetAvailableRamSize(), GetUsedRamPercent(), GetAvailableRamPercentage(), GetCPULoad()));
            return result;
        }

    }

}
