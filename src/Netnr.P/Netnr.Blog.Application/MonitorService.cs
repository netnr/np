using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Netnr.Blog.Application
{
    public class MonitorService
    {
        public bool PingHost(string hostNameOrAddress)
        {
            var ping = new Ping();
            var pr = ping.Send(hostNameOrAddress);
            return pr.Status == IPStatus.Success;
        }
    }
}
