﻿using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using Xunit;

namespace Netnr.Test
{
    public class TestProtocol
    {
        [Fact]
        public void Protocol_HTTP_GET()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://zme.ink")
            };
            var result = client.Send(request);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void Protocol_HTTP_POST()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://zme.ink")
            };
            var result = client.Send(request);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void Protocol_PING()
        {
            var pingSender = new Ping();

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            // Wait 10 seconds for a reply.
            int timeout = 10000;

            // Set options for transmission:
            // The data can go through 64 gateways or routers
            // before it is destroyed, and the data packet
            // cannot be fragmented.
            var options = new PingOptions(64, true);

            // Send the request.
            PingReply reply = pingSender.Send("zme.ink", timeout, buffer, options);
            Debug.WriteLine(reply.Status);

            Assert.True(reply.Status == IPStatus.Success);
            if (reply.Status == IPStatus.Success)
            {
                Debug.WriteLine("Address: {0}", reply.Address.ToString());
                Debug.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                Debug.WriteLine("Time to live: {0}", reply.Options.Ttl);
                Debug.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                Debug.WriteLine("Buffer size: {0}", reply.Buffer.Length);
            }
        }

        [Fact]
        public void Protocol_TCP()
        {
            try
            {
                using var client = new TcpClient();
                var host = Dns.GetHostAddresses("zme.ink").First();
                var port = 80;
                var result = client.BeginConnect(host, port, null, null);

                var success = result.AsyncWaitHandle.WaitOne(2000);
                if (success)
                {
                    // 连接成功
                    client.EndConnect(result);
                }
                else
                {
                    // 连接超时
                    client.Close();
                }
                Assert.True(success);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [Fact]
        public async Task Protocol_DNS()
        {
            var addresses = await Dns.GetHostAddressesAsync("zme.ink");
            Assert.True(addresses.Length > 0);
        }
    }
}
