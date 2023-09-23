using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using DdnsSharp.Core.Model;
using DdnsSharp.Model;
using System.Xml.Linq;

namespace DdnsSharp.Core
{
    public static class Utils
    {
        public static NetworkModel GetNetworkIp()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            NetworkModel net = new NetworkModel() { Ipv4 = new(), Ipv6 = new() };

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {

                if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                    (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                     networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                {
                    Dictionary<string, List<string>> networkv4 = new Dictionary<string, List<string>>();
                    Dictionary<string, List<string>> networkv6 = new Dictionary<string, List<string>>();
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

        public static IEnumerable<NetinterfaceData> V4NetinterfaceDatas()
        {
            var v4 = GetNetworkIp().Ipv4;
            foreach (var i in v4)
            {
                foreach (var ii in i)
                {
                    foreach (var iii in ii.Value)
                    {
                        //yield return new NetinterfaceData() { Name = $"{ii.Key}({iii})", Netinterface = (ii.Key, (byte)ii.Value.IndexOf(iii)) };
                        yield return new NetinterfaceData() { Name = $"{ii.Key}({iii})", Netinterface = new(ii.Key, (byte)ii.Value.IndexOf(iii)) };
                    }
                }
            }
        }

        public static IEnumerable<NetinterfaceData> V6NetinterfaceDatas()
        {
            var v6 = GetNetworkIp().Ipv6;
            foreach (var i in v6)
            {
                foreach (var ii in i)
                {
                    foreach (var iii in ii.Value)
                    {
                        yield return new NetinterfaceData() { Name = $"{ii.Key}({iii})", Netinterface = new(ii.Key, (byte)ii.Value.IndexOf(iii)) };
                    }
                }
            }
        }
    }
}
