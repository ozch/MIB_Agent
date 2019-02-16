using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBAgent
{
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
    class InitialDataModel
    {
        public String os_version;
        public String os_name;
        public String machine_name;
        public int cores;
        public String username;
        public String arc_type;
        public String nic_type;
        public String mac;
        public String ip;
        public String mask;
        public String gateway;
        public String nic_speed;
        public String cpu_model;
        public double ram_gb;
        public double cpu_clock;

        public InitialDataModel(string os_version, string os_name, string machine_name, int cores, string username, string arc_type, string nic_type, string mac, string ip, string mask, string gateway, string nic_speed, string cpu_model, double ram_gb, double cpu_clock)
        {
            this.os_version = os_version;
            this.os_name = os_name;
            this.machine_name = machine_name;
            this.cores = cores;
            this.username = username;
            this.arc_type = arc_type;
            this.nic_type = nic_type;
            this.mac = mac;
            this.ip = ip;
            this.mask = mask;
            this.gateway = gateway;
            this.nic_speed = nic_speed;
            this.cpu_model = cpu_model;
            this.ram_gb = ram_gb;
            this.cpu_clock = cpu_clock;
        }
    }
}
