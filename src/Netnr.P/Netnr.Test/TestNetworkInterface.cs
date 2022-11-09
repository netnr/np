using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Xunit;

namespace Netnr.Test
{
    public class TestNetworkInterface
    {
        [Fact]
        public void NetworkInterfaceInfo()
        {
            var nis = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var ni in nis)
            {
                Debug.WriteLine(ni.ToJson(true));
                Debug.WriteLine(ni.GetIPStatistics().ToJson(true));

                var ipStats = ni.GetIPStatistics();

                var dic = new Dictionary<string, object>
                {
                    { "Bytes Sent", ParsingTo.FormatByteSize(ipStats.BytesSent) },
                    { "Bytes Received", ParsingTo.FormatByteSize(ipStats.BytesReceived) },
                    { "PhysicalAddress(MAC)", ni.GetPhysicalAddress() },
                };
                foreach (var key in dic.Keys)
                {
                    Debug.WriteLine($"{key}: {dic[key]}");
                }

                Debug.WriteLine("\r\n");
            }
        }

        [Fact]
        public void IPStatistics()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            var ipstat = properties.GetIPv4GlobalStatistics();
            Debug.WriteLine("\r\nIPv4 Statistics");
            Debug.WriteLine("  Forwarding enabled .............. : {0}", ipstat.ForwardingEnabled);
            Debug.WriteLine("  Interfaces ...................... : {0}", ipstat.NumberOfInterfaces);
            Debug.WriteLine("  IP addresses .................... : {0}", ipstat.NumberOfIPAddresses);
            Debug.WriteLine("  Routes .......................... : {0}", ipstat.NumberOfRoutes);
            Debug.WriteLine("  Default TTL ..................... : {0}", ipstat.DefaultTtl);
            Debug.WriteLine("");
            Debug.WriteLine("  Inbound Packet Data:");
            Debug.WriteLine("      Received .................... : {0}", ipstat.ReceivedPackets);
            Debug.WriteLine("      Forwarded ................... : {0}", ipstat.ReceivedPacketsForwarded);
            Debug.WriteLine("      Delivered ................... : {0}", ipstat.ReceivedPacketsDelivered);
            Debug.WriteLine("      Discarded ................... : {0}", ipstat.ReceivedPacketsDiscarded);
            Debug.WriteLine("      Header Errors ............... : {0}", ipstat.ReceivedPacketsWithHeadersErrors);
            Debug.WriteLine("      Address Errors .............. : {0}", ipstat.ReceivedPacketsWithAddressErrors);
            Debug.WriteLine("      Unknown Protocol Errors ..... : {0}", ipstat.ReceivedPacketsWithUnknownProtocol);
            Debug.WriteLine("");
            Debug.WriteLine("  Outbound Packet Data:");
            Debug.WriteLine("      Requested ................... : {0}", ipstat.OutputPacketRequests);
            Debug.WriteLine("      Discarded ................... : {0}", ipstat.OutputPacketsDiscarded);
            Debug.WriteLine("      No Routing Discards ......... : {0}", ipstat.OutputPacketsWithNoRoute);
            Debug.WriteLine("      Routing Entry Discards ...... : {0}", ipstat.OutputPacketRoutingDiscards);
            Debug.WriteLine("");
            Debug.WriteLine("  Reassembly Data:");
            Debug.WriteLine("      Reassembly Timeout .......... : {0}", ipstat.PacketReassemblyTimeout);
            Debug.WriteLine("      Reassemblies Required ....... : {0}", ipstat.PacketReassembliesRequired);
            Debug.WriteLine("      Packets Reassembled ......... : {0}", ipstat.PacketsReassembled);
            Debug.WriteLine("      Packets Fragmented .......... : {0}", ipstat.PacketsFragmented);
            Debug.WriteLine("      Fragment Failures ........... : {0}", ipstat.PacketFragmentFailures);
            Debug.WriteLine("");
        }

        [Fact]
        public void TraceRoute()
        {
            var result = GetTraceRoute("google.com");
            foreach (var item in result)
            {
                Debug.WriteLine($"{item.Status} {item.RoundtripTime}ms {item.Address}");
            }
        }

        /// <summary>
        /// 路由追踪
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        private static IEnumerable<PingReply> GetTraceRoute(string hostname)
        {
            const int timeout = 5000;
            const int maxTTL = 30;
            const int bufferSize = 32;

            byte[] buffer = new byte[bufferSize];
            new Random().NextBytes(buffer);

            using var pinger = new Ping();
            var address = Dns.GetHostAddresses(hostname).First();
            var finalTimeout = 0;
            for (int ttl = 1; ttl <= maxTTL; ttl++)
            {
                var options = new PingOptions(ttl, true);
                var reply = pinger.Send(address, timeout, buffer, options);
                if (reply.Status == IPStatus.TimedOut && reply.Address.Equals(address))
                {
                    finalTimeout++;
                }
                if (finalTimeout > 3)
                {
                    break;
                }

                yield return reply;

                if (reply.Status != IPStatus.TtlExpired && reply.Status != IPStatus.TimedOut)
                    break;
            }
        }

        /// <summary>
        /// 获取本地 IP
        /// https://stackoverflow.com/questions/6803073/get-local-ip-address
        /// </summary>
        [Fact]
        public void GetLocalIPAddress_list()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    Debug.WriteLine(ip.ToString());
                }
            }
        }

        /// <summary>
        /// 本地IP 且可访问公网
        /// </summary>
        [Fact]
        public void GetLocalIPAddress_Internet()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("114.114.114.114", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            Debug.WriteLine(endPoint.Address.ToString());
        }
    }
}
