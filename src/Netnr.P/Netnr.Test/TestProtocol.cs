using System.Net;
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
            Ping pingSender = new Ping();

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
            var client = new TcpClient();
            try
            {
                Assert.True(client.ConnectAsync(Dns.GetHostAddresses("zme.ink").First(), 80).Wait(2000));
            }
            catch (SocketException ex)
            {
                Debug.WriteLine(ex.Message);
                Assert.True(client.Client.Connected);
            }
            finally
            {
                client.Close();
            }
        }

        [Fact]
        public void Protocol_DNS()
        {
            var addresses = Dns.GetHostAddresses("zme.ink");
            Assert.True(addresses.Length > 0);
        }
    }
}
