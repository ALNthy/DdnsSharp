using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace DdnsCore
{
    public static class Utils
    {
        public static NetworkModel GetNetworkIp()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            NetworkModel net = new NetworkModel() { Ipv4 = new(),Ipv6=new() };

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                
                if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                    (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                     networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                {
                    Dictionary<string,List<string>> networkv4 = new Dictionary<string, List<string>>();
                    Dictionary<string,List<string>> networkv6 = new Dictionary<string, List<string>>();
                    List<string> v4 = new List<string>();
                    List<string> v6 = new List<string>();
                    foreach (UnicastIPAddressInformation ipInfo in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        
                        if (ipInfo.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            v6.Add(ipInfo.Address.ToString());
                        }
                        if (ipInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            v4.Add(ipInfo.Address.ToString());
                        }
                    }
                    networkv4.Add(networkInterface.Name, v4);
                    networkv6.Add(networkInterface.Name, v6);
                    net.Ipv4.Add(networkv4);
                    net.Ipv6.Add(networkv6);
                }
            }
            return net;
        }
    }
}
