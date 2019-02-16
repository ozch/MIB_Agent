using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
/*
> Objective <
This Class Only Provide you with all the methods to collect the
system information required only once at the start of the comp-
uter 
>>Basic<<
* OS Version
* OS Name
* System Architecture Type
* User Name
* PC Name (Machine Name)
* CPU Model
* Number of Core
* Max Clock Speed
* RAM Size
>>Networking<<
* IP Address
* Subnet Mask
* MAC Address
* Default Gateway
*/
namespace MIBAgent
{
    class InitialDataCollector
    {


        //The function GetOSName() returns friendly Windows OS Version name make it easier to identify
        public string GetOSName()
        {

            string result = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                result = os["Caption"].ToString();
                break;
            }
            return result;
        }


        //The function GetOSVersion() returns the NT OS version Number of Operating System
        public string GetOSVersion()
        {
            return Environment.OSVersion.ToString();
        }


        //The given function GetMachineName() returns the name given to Computer by the user
        public string GetMachineName()
        {
            return Environment.MachineName.ToString();
        }


        //Function GetUserName() returns the name of current active user using the PC
        public string GetUserName()
        {
            return Environment.UserName.ToString();
        }


        //The Function GetSystemType() returns the Architecture type of the System in String form
        //It tell weather the PC is 32bit or 64bit
        public string GetSystemType()
        {
            string result = "Null";
            if (Environment.Is64BitOperatingSystem)
                result = "x64";
            else
                result = "x86";

            return result;
        }


        //Function GetCoreCount() returns Core count of CPU
        //in this case core count is equal to the total number of physical + logical core
        public string GetCoreCount()
        {
            return Environment.ProcessorCount.ToString();
        }


        //The Fucntion GetMacAddress() return the MAC address of first active(Status==UP) NIC
        //MAC address is in the following form  "28C63F2056CF" 
        public string GetMacAddress()
        {
            string mac_addresses = string.Empty;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                //Checking which NIC is UP
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    //Get the Mac Address of First NIC which is UP and Break the Loop
                    //Only Getting First IP
                    mac_addresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            return mac_addresses;
        }


        //This Function GetLocalIPAddress() Return the IP address avoiding get VMWare host or other invalid IP address.
        public string GetLocalIpAddress()
        {
            UnicastIPAddressInformation most_suit_ip = null;
            var network_interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var network in network_interfaces)
            {
                //Skipping Down NICs 
                if (network.OperationalStatus != OperationalStatus.Up)
                    continue;

                var properties = network.GetIPProperties();

                if (properties.GatewayAddresses.Count == 0)
                    continue;

                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    if (IPAddress.IsLoopback(address.Address))
                        continue;

                    if (!address.IsDnsEligible)
                    {
                        if (most_suit_ip == null)
                            most_suit_ip = address;
                        continue;
                    }
                    // The best IP is the IP got from DHCP server
                    if (address.PrefixOrigin != PrefixOrigin.Dhcp)
                    {
                        if (most_suit_ip == null || !most_suit_ip.IsDnsEligible)
                            most_suit_ip = address;
                        continue;
                    }

                    return address.Address.ToString();
                }
            }

            return most_suit_ip != null ? most_suit_ip.Address.ToString() : "Null";
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
         * * * * * * * Caution * * * * * * *
         * This function perform the functionality of 6 seperate function
         * and is availbe to increase the performance of the program
         * For finding IP address i recommand GetLocalIPAddress() Method
         * because it's more reliable
         * Seperate function are availbe to perform the task
         */
        public string[] GetInterfaceCardInfo()
        {
            UnicastIPAddressInformation unicast = null;
            string[] info = { "Null", "Null", "Null", "Null", "Null", "Null" };
            var network_interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var net_interface in network_interfaces)
            {
                if (net_interface.OperationalStatus != OperationalStatus.Up)
                    continue;

                var properties = net_interface.GetIPProperties();

                if (properties.GatewayAddresses.Count == 0)
                    continue;

                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    if (IPAddress.IsLoopback(address.Address))
                        continue;

                    unicast = address;

                    IPInterfaceProperties adapterProperties = net_interface.GetIPProperties();
                    GatewayIPAddressInformationCollection gw_addresses = adapterProperties.GatewayAddresses;

                    int nic_speed = Convert.ToInt32(net_interface.Speed.ToString());

                    info[5] = Convert.ToString((nic_speed) / 1048576);
                    info[4] = net_interface.Name;
                    info[3] = net_interface.GetPhysicalAddress().ToString(); //MAC Address
                    info[2] = unicast.IPv4Mask.ToString(); //SubnetMask
                    info[1] = gw_addresses[0].Address.ToString();//Gateway
                    info[0] = address.Address.ToString(); //IP Address 
                    return info;
                }
            }
            return info;
        }


        //Return the CPU Model of the System, i.e "Intel i7 7700HQ"
        public string GetCPUModel()
        {
            string result = "Null";
            ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                result = mo["Name"].ToString();
            }
            return result;
        }


        //The Function returns GetTotalMemoryInMegaBytes() return the memory of device
        public double GetTotalMemoryInMegaBytes()
        {
            double mem = Convert.ToDouble(new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory);
            //1048576 = 1024*1024 to convert byte in MegaBy 
            double mem_mb = Math.Round(mem / 1048576, 1);
            return mem_mb;
        }


        //The Function returns GetTotalMemoryInGigaBytes() return the memory of device
        //Dependent on GetTotalMemoryInMegaBytes() function
        public double GetTotalMemoryInGigaBytes()
        {
            double mem_mb = GetTotalMemoryInMegaBytes();
            double mem_gb = Math.Round(mem_mb / 1024, 1);
            return mem_gb;
        }


        //This Function GetMaxClockSpeed Returns the Maximum Clock Speed of CPU in GHz
        //Tested on single CPU machine (never been test on server grade multiple CPU machines)
        public double GetMaxClockSpeed()
        {
            uint max_sp;
            using (ManagementObject Mo = new ManagementObject("Win32_Processor.DeviceID='CPU0'"))
            {
                //currentsp = (uint)(Mo["CurrentClockSpeed"]);      
                max_sp = (uint)(Mo["MaxClockSpeed"]);
            }
            //Divid by 10^3 to Conver MHz into GHz
            double max_sp_d = Math.Round(((double)max_sp) / 1000, 2);
            return max_sp_d; //Converting in GHz
        }
        public string GetString()
        {
            
            string[] nic_data = GetInterfaceCardInfo();
            Console.WriteLine("Please Wait Getting Details...\n");
            string result = "============================================" 
                + "\nOSVersion: " + GetOSVersion()
                + "\nOSName: " + GetOSName()
                + "\nMachineName: " + GetMachineName()
                + "\nCores (Logical and Physcial): " + GetCoreCount()
                + "\nUserName: " + GetUserName()
                + "\nSystem Architecture Type: " + GetSystemType()
                + "\nNIC Type: " + nic_data[4]
                + "\nMAC Address: " + nic_data[3]
                + "\nLocal IP Address: " + nic_data[0]
                + "\nSubnet Mask: " + nic_data[2]
                + "\nDefault Gateway: " + nic_data[1]
                + "\nNIC Speed Current Approx Mbps : " + nic_data[5]
                + "\nCPU Model: " + GetCPUModel()
                + "\nRAM (GBs): " + GetTotalMemoryInGigaBytes()
                + "\nCPU ClockSpeed: " + GetMaxClockSpeed() + "\n";
            return result;
        }
        /*
        "OSVer": "Microsoft Windows NT 6.3.9600.0",
        "OSN": "Microsoft Windows 8.1 Pro",
        "MN": "PERSONAL-PC",
        "Cor": "8",
        "UN": "UZAIR AEZAD",
        "SysArcType": "x64",
        "nicType": "Wi-Fi",
        "MACAdd": "002682531B3C",
        "LocalIP": "192.168.1.7",
        "SubnetMask": "255.255.255.0",
        "DefGateway": "192.168.1.1",
        "NICCurSpeed": "286",
        "CPUModel": "Intel(R) Core(TM) i7 CPU       Q 720  @ 1.60GHz",
        "ram": "4",
        "CPUClock": "1.6"
            */
        public string GetJson()
        {
            string[] nic_data = GetInterfaceCardInfo();
            InitialDataModel list = new InitialDataModel(
               GetOSVersion(),
               GetOSName(),
               GetMachineName(),
               Convert.ToInt32(GetCoreCount()),
               GetUserName(),
               GetSystemType(),
               nic_data[4],
               nic_data[3],
               nic_data[0],
               nic_data[2],
               nic_data[1],
               nic_data[5],
               GetCPUModel(),
               Math.Round(GetTotalMemoryInGigaBytes(),2),
               Math.Round(GetMaxClockSpeed(),2));
            return JsonConvert.SerializeObject(list);
        }
        public string GetJsonPretty()
        {

            string[] nic_data = GetInterfaceCardInfo();
            Console.WriteLine("Please Wait Getting Json...\n");
            string result = "{"
                + "\n\"OSVer\": \"" + GetOSVersion() + "\","
                + "\n\"OSN\": \"" + GetOSName() + "\","
                + "\n\"MN\": \"" + GetMachineName() + "\","
                + "\n\"Cor\": \"" + GetCoreCount() + "\","
                + "\n\"UN\": \"" + GetUserName() + "\","
                + "\n\"SysArcType\": \"" + GetSystemType() + "\","
                + "\n\"nicType\": \"" + nic_data[4] + "\","
                + "\n\"MACAdd\": \"" + nic_data[3] + "\","
                + "\n\"LocalIP\": \"" + nic_data[0] + "\","
                + "\n\"SubnetMask\": \"" + nic_data[2] + "\","
                + "\n\"DefGateway\": \"" + nic_data[1] + "\","
                + "\n\"NICCurSpeed\": \"" + nic_data[5] + "\","
                + "\n\"CPUModel\": \"" + GetCPUModel() + "\","
                + "\n\"ram\": \"" + GetTotalMemoryInGigaBytes() + "\","
                + "\n\"CPUClock\": \"" + GetMaxClockSpeed() + "\""
                + "\n}";
            return result;
        }

    }
}
