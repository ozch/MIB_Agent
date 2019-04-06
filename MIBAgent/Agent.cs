﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Net;

namespace MIBAgent
{
    class Agent
    {

        static int sleepy = 4500;
        static int sleepy_update = 30000;
        static string[] nic_info = { "Null", "Null", "Null", "Null", "Null", "Null" };
        public static void Main(string[] args)
        {
            Agent tm = new Agent();
            InitialDataCollector idc = new InitialDataCollector();
            nic_info = idc.GetInterfaceCardInfo();
            string mac = nic_info[3];
            string ip = nic_info[0];
            string mask = nic_info[2];


            // Need to be change to /config.yaml only
            var file = new StreamReader(@"../../config.yaml");
            var deserial = new DeserializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build();
            var config = deserial.Deserialize<dynamic>(file);
            string api_server = config["apiserver"];
            string token = config["token"];
            string request = api_server +"agent/token/"+mac + "/" + token + "/";
            sleepy_update = Convert.ToInt16(config["intervel"]);
            string server = config["server"];
            string port = config["port"];

            Console.WriteLine(request);
            string token_status = tm.CheckTokenAuthentication(request);
            
            if (token_status != "200")
            {
                Environment.Exit(1);
            }



            int sleep_time = 6000;
            DBConnection dbc = new DBConnection(server,port);


            bool tryAgain = true;
            while (tryAgain)
            {
                try
                {
                    dbc = new DBConnection();
                    tryAgain = false;
                }
                catch (Exception e)
                {
                    System.Threading.Thread.Sleep(sleep_time);
                    dbc = new DBConnection();
                    if (dbc.IsConnect())
                    {
                        tryAgain = false;
                    }
                    else
                    {
                        sleep_time = sleep_time + sleep_time;
                        Console.WriteLine("Server Connection Failure,Retrying in " + sleep_time);
                        if (sleep_time < 0)
                        {
                            sleep_time = 12000;
                        }

                    }
                }
            }

            MySqlConnection db_con = dbc.GetConnection();
            Console.WriteLine("Connection Established");
            //string Query = "insert into student.studentinfo(idStudentInfo,Name,Father_Name,Age,Semester) values('" + this.IdTextBox.Text + "','" + this.NameTextBox.Text + "','" + this.FnameTextBox.Text + "','" + this.AgeTextBox.Text + "','" + this.SemesterTextBox.Text + "');";
            //Getting device information to get started with 
            //device identity creation
            //identities are based on mac adress, ip_address and subnet mask all combined
            



            
            tm.InitialUpdate(db_con, mac, ip, mask, idc.GetJson());
            tm.ServiceUpdate(db_con, mac);
            tm.ProcessUpdate(db_con, mac);
            DateTime start = DateTime.Now;
            while (true)
            {
                tm.Executioner(db_con, mac,tm);
                System.Threading.Thread.Sleep(sleepy);
                TimeSpan timeDiff = DateTime.Now - start;
                if(timeDiff.Seconds > sleepy_update) { 
                    start = DateTime.Now;
                    tm.ServiceUpdate(db_con, mac);
                    tm.ProcessUpdate(db_con, mac);
                    
                }
            }







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
            //Console.WriteLine(ops.GetJson());
            //Console.ReadKey();

            //TaskKill tk = new TaskKill();
            //tk.KillProcess(15072);
            //ServiceController sc = new ServiceController();
            //sc.StopService("olevba");
            //sc.StartService("nonpe");
            //Console.WriteLine("Hello World" + dbc.IsConnect());



        }
        public string Executioner(MySqlConnection con, string mac,Agent tm)
        {

            string stm = string.Format("SELECT * FROM mib.execute where mac_address='{0}'", mac);
            MySqlCommand cmd = con.CreateCommand();
            cmd.CommandText = stm;
            
            MySqlDataReader scal = cmd.ExecuteReader();
            scal.Read();
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
            scal.Close();

            
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
                tm.ServiceUpdate(con, mac);
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
                tm.ProcessUpdate(con, mac);
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
                string query2 = string.Format("UPDATE execute SET mac_address = '{0}',boot_flag ={1},service_flag = {2},kill_flag = {3},script_flag = {4},port_flag = {5},boot_command = '{6}',service_name = '{7}',kill_name = '{8}',script = '{9}',portno={10},online=NOW() WHERE mac_address = '{0}'", mac, boot_flag, service_flag, kill_flag, script_flag, port_flag, boot_command, service_name, kill_name, script, port_flag);
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

            string query = string.Format("UPDATE mib.execute SET mac_address = '{0}',boot_flag = {1},service_flag = {2},kill_flag = {3},script_flag = {4},port_flag = {5},boot_command = '{6}',service_name = '{7}',kill_name = '{8}',script = '{9}', portno = {10},online=NOW() WHERE mac_address = '{0}'", mac,boot_flag,service_flag,kill_flag,script_flag,port_flag,boot_command,service_name,kill_name,script,port_flag);
// Console.WriteLine(query);
            cmd = con.CreateCommand();
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            Console.WriteLine("Ran UPDATE");
            return "0";
        }
        public string ProcessUpdate(MySqlConnection con, string mac)
        {


            string stm = string.Format("select * from mib.process_info where mac_address='{0}'", mac);
            MySqlCommand cmd = con.CreateCommand();
            cmd.CommandText = stm;

            ProcessMonitor pm = new ProcessMonitor();
            int number = pm.GetNumber();
            Console.WriteLine(number);
            string json = pm.GetJson();

            //change it to ExecuteScaler();
            var scal = cmd.ExecuteScalar();
            Console.WriteLine(scal);
            if (scal != null)
            {
                string query = string.Format("UPDATE mib.process_info SET mac_address = '{0}', process_info = '{1}', number = {2} WHERE mac_address = '{0}'", mac, json, number);
                cmd = con.CreateCommand();
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                Console.WriteLine("Ran UPDATE");
                return "0";
            }
            else
            {

                string query = string.Format("INSERT INTO mib.process_info(mac_address, process_info, number) VALUES ('{0}','{1}',{2})", mac, json, number);
                cmd = con.CreateCommand();
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                Console.WriteLine("Ran Insert");
                return "0";
            }


        }
        public string PortUpdate(MySqlConnection con, string mac)
        {


            string stm = string.Format("select * from mib.open_ports where mac_address='{0}'", mac);
            MySqlCommand cmd = con.CreateCommand();
            cmd.CommandText = stm;

            OpenPortScan sm = new OpenPortScan();
            int number = sm.GetNumber();
            Console.WriteLine(number);
            string json = sm.GetJson();

            //change it to ExecuteScaler();
            var scal = cmd.ExecuteScalar();
            Console.WriteLine(scal);
            if (scal != null)
            {
                string query = string.Format("UPDATE mib.open_ports SET mac_address = '{0}', json = '{1}', number = {2} WHERE mac_address = '{0}'", mac, json, number);
                cmd = con.CreateCommand();
                cmd.CommandText = query;
                //cmd.ExecuteNonQuery();
                Console.WriteLine("Ran UPDATE");
                return "0";
            }
            else
            {

                string query = string.Format("INSERT INTO mib.open_ports(mac_address, json, number) VALUES ('{0}','{1}',{2})", mac, json, number);
                cmd = con.CreateCommand();
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                Console.WriteLine("Ran Insert");
                return "0";
            }


        }
        public string ServiceUpdate(MySqlConnection con, string mac)
        {


            string stm = string.Format("select * from mib.services_info where mac_address='{0}'", mac);
            MySqlCommand cmd = con.CreateCommand();
            cmd.CommandText = stm;

            ServiceMonitor sm = new ServiceMonitor();
            int number = sm.GetNumber();
            Console.WriteLine(number);
            string json = sm.GetJson();

            //change it to ExecuteScaler();
            var scal = cmd.ExecuteScalar();
            Console.WriteLine(scal);
            if (scal != null)
            {
                string query = string.Format("UPDATE mib.services_info SET mac_address = '{0}', services_info = '{1}', total = {2} WHERE mac_address = '{0}'", mac, json,number);
                cmd = con.CreateCommand();
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                Console.WriteLine("Ran UPDATE");
                return "0";
            }
            else
            {

                string query = string.Format("INSERT INTO mib.services_info(mac_address, services_info, total) VALUES ('{0}','{1}',{2})", mac, json, number);
                cmd = con.CreateCommand();
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                Console.WriteLine("Ran Insert");
                return "0";
            }


        }
        public string InitialUpdate(MySqlConnection con,string mac,string ip,string mask,string json)
        {
           

                string stm = string.Format("select * from mib.device_info where mac_address='{0}'",mac);
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = stm;
               
                
                //change it to ExecuteScaler();
                var scal = cmd.ExecuteScalar();
                Console.WriteLine(scal);
                if (scal != null)
                {
                    string query = string.Format("UPDATE mib.device_info SET mac_address = '{0}', ip_address = '{1}', mask = '{2}', sys_info = '{3}' WHERE mac_address = '{0}'", mac, ip, mask, json);
                    cmd = con.CreateCommand();
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Ran UPDATE");
                    return "0";
                }
                else
                {
                    
                    string query = string.Format("INSERT INTO mib.device_info(mac_address,ip_address,mask,sys_info) VALUES ('{0}','{1}','{2}','{3}')", mac, ip, mask, json);
                    
                    cmd = con.CreateCommand();
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    query = string.Format("INSERT INTO mib.execute(mac_address)VALUES ('{0}')", mac);
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Ran Insert");
                    return "0";
                }
                
            
        }
        public string CheckTokenAuthentication(string request)
        {
            try
            {
                string webAddr = request;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var response = streamReader.ReadToEnd();
                    Console.WriteLine("Token Status : "+ response);
                    return response;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return "404";
            }
        }
    }
    
    
}

