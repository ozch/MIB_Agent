using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace MIBAgent
{
    class Agent
    {
        static int sleepy = 5000;
        
        static string[] nic_info = { "Null", "Null", "Null", "Null", "Null", "Null" };
        public static void Main(string[] args)
        {
            //Connecting to Database for data storage
            int sleep_time = 6000;
            try
            {

            }
            catch (Exception e)
            {
                sleep_time = sleep_time + sleep_time;
                if (sleep_time < 0)
                {
                    sleep_time = 12000;
                }

            }


            string tokenx = "15AS94D";
            InitialDataCollector idc = new InitialDataCollector();
            OpenPortScan ops = new OpenPortScan();
            ProcessMonitor pm = new ProcessMonitor();
            ResourceMonitor rm = new ResourceMonitor();
            ServiceMonitor sm = new ServiceMonitor();
            string ops_data = ops.GetJson();
            //string pm_data = pm.GetJson();
            //string sm_data = sm.GetJson();
            //string rm_data = rm.GetJson();
            string idc_data = idc.GetJson();
            nic_info = idc.GetInterfaceCardInfo();
            string mac = nic_info[3];
            string ip = nic_info[0];
            string mask = nic_info[2];
            Agent tm = new Agent();

          
            APIRequest api = new APIRequest();
            Console.WriteLine(ops_data);

            //api.sendAllDataPM(pm_data,mac,tokenx);
            //api.sendAllDataDI(idc_data, mac, tokenx);
            //api.sendAllDataSM(sm_data, mac, tokenx);
            api.sendAllDataOPS(ops_data, mac, tokenx);

            Console.ReadKey();
            /*
           
            */
            
            //while (true) {
            //     tm.Executioner(mac);
            //    System.Threading.Thread.Sleep(sleepy);
            //}


            /*
             * This Function Returns an Array Contain IP,Gateway,Subnet and MAC Addresses of active
             * Network Interface Card
             * Index 0 = IP Address
             * Index 1 = Gateway Address
             * Index 2 = Subnet Mask
             * Index 3 = MAC Address
             * Index 4 = NIC Type
             * Index 5 = Speed MB's
             */


            //ProcessMonitor pm = new ProcessMonitor();
            //ServiceMonitor sm = new ServiceMonitor();
            //ResourceMonitor rm = new ResourceMonitor();
            //OpenPortScan ops = new OpenPortScan();


            //TaskKill tk = new TaskKill();
            //tk.KillProcess(15072);
            //ServiceController sc = new ServiceController();
            //sc.StopService("olevba");
            //sc.StartService("nonpe");
            //Console.WriteLine("Hello World" + dbc.IsConnect());



        }

        /*
        public string Executioner(string mac)
        {
            int boot_flag = Convert.ToInt16(scal[1]);
            int service_flag = Convert.ToInt16(scal[2]);
            int kill_flag = Convert.ToInt16(scal[3]);
            int script_flag = Convert.ToInt16(scal[4]);
            int port_flag = Convert.ToInt16(scal[5]);
            
            string boot_command = Convert.ToString(scal[6]);
            string service_name = Convert.ToString(scal[7]);
            string kill_name = Convert.ToString(scal[8]);
            string script = Convert.ToString(scal[9]);
            int portno = Convert.ToInt16(scal[10]);     
            if(service_flag == 1)
            {
                try { 
                ServiceController sc = new ServiceController();
               string[] val = service_name.Split(';');
               string re = "";
               if(val[0].Equals("start", StringComparison.InvariantCultureIgnoreCase)){
                   re = sc.StartService(val[1]);
                   if (re == "0")
                       service_name= "started";
                       service_flag = 0;
               }
               else if (val[0].Equals("stop", StringComparison.InvariantCultureIgnoreCase))
               {
                   re = sc.StopService(val[1]);
                   if (re == "0")
                       service_flag = 0;
                   service_name = "stopped";
               }
               else if (val[0].Equals("delete", StringComparison.InvariantCultureIgnoreCase))
               {
                   re = sc.DeleteService(val[1]);
                   if (re == "0")
                       service_flag = 0;
                   service_name = "deleted";
               }
               else
               {
                   service_flag = 1;
               }
                }
                catch (Exception)
                {
                    service_flag = 2;
                }

            }
            if (kill_flag == 1)
            {
                try { 
                TaskKill tk = new TaskKill();
                tk.KillByName(kill_name);
                    kill_flag = 0;
                    kill_name = "Done";
                }
                catch(Exception){
                    kill_flag = 2;
                }
            }
            if (script_flag == 1)
            {
                try
                {
                    PowerShellExecutor pse = new PowerShellExecutor();
                    string val = pse.RunShellScript(script);
                        script_flag = 0;
                    script = val;
                }
                catch (Exception e)
                {
                    script_flag = 2;
                }

                
            }
            if(port_flag == 1)
            {
                try { 
                OpenPortScan ops = new OpenPortScan();
                string val = ops.GetPortProcessName(portno);
                if (val == "-1")
                {
                    port_flag = 2;
                }
                else
                {
                    TaskKill tk = new TaskKill();
                    tk.KillByName(val);
                    port_flag = 0;
                }
                }
                catch (Exception)
                {
                    port_flag = 2;
                }
            }

            if (boot_flag == 1)
            {
                try { 
                boot_flag = 0;
                string query2 = string.Format("UPDATE execute SET mac_address = '{0}',boot_flag ={1},service_flag = {2},kill_flag = {3},script_flag = {4},port_flag = {5},boot_command = '{6}',service_name = '{7}',kill_name = '{8}',script = '{9}',portno={10} WHERE mac_address = '{0}'", mac, boot_flag, service_flag, kill_flag, script_flag, port_flag, boot_command, service_name, kill_name, script, port_flag);
                Console.WriteLine(query2);
                cmd = con.CreateCommand();
                cmd.CommandText = query2;
                cmd.ExecuteNonQuery();
                Console.WriteLine("Ran UPDATE");
                if (boot_command.Equals("shutdown", StringComparison.InvariantCultureIgnoreCase))
                {
                    BootController bc = new BootController();
                    string val = bc.ShutdownWindows();
                    if (val == "0")
                        boot_flag = 0;
                }
                else if (boot_command.Equals("restart", StringComparison.InvariantCultureIgnoreCase))
                {
                    BootController bc = new BootController();
                    string val = bc.RestartWindows();
                    if (val == "0")
                        boot_flag = 0;
                }
                else if (boot_command.Equals("sleep", StringComparison.InvariantCultureIgnoreCase))
                {
                    BootController bc = new BootController();
                    string val = bc.SleepWindows();
                    if (val == "0")
                        boot_flag = 0;
                }
                else
                {
                    boot_flag = 1;
                }
                }
                catch (Exception)
                {
                    boot_flag = 2;
                }
            }

            string query = string.Format("UPDATE mib.execute SET mac_address = '{0}',boot_flag = {1},service_flag = {2},kill_flag = {3},script_flag = {4},port_flag = {5},boot_command = '{6}',service_name = '{7}',kill_name = '{8}',script = '{9}', portno = {10} WHERE mac_address = '{0}'",mac,boot_flag,service_flag,kill_flag,script_flag,port_flag,boot_command,service_name,kill_name,script,port_flag);
            Console.WriteLine(query);
            cmd = con.CreateCommand();
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            Console.WriteLine("Ran UPDATE");
            return "0";
        }
          */
        public string GetProcessedString(string p)
        {

            p = p.Replace("<", "{");
            p = p.Replace(">", "}");
            return p;
        }

    }
    
    
}

