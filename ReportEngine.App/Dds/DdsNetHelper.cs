using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.App.Dds
{
    public class DdsNetHelper
    {
        private static readonly List<string> _publicIpCheckers = new List<string>
        {
            "https://api.ipify.org",
            "https://icanhazip.com",
            "https://checkip.amazonaws.com",
            "https://ifconfig.me/ip",
            "https://ipinfo.io/ip"
        };

        private static readonly List<string> _publicDnsResolvers = new List<string>
        {
            "8.8.8.8", //Google DNS
            "1.1.1.1", //Cloudflare DNS
            "77.88.8.8" //Yandex DNS
        };


        private static readonly string _prodServerIP = "172.16.0.210";
        private static readonly string _devServerIP = "172.16.10.230";



        private static readonly int _httpClientTimeoutSeconds = 5;
        private static readonly int _pingTimeoutMilliseconds = 5000;



        public static List<string> GetLocalIpAddresses()
        {
            var addresses = new List<string>();
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork &&
                    !IPAddress.IsLoopback(ip))
                {
                    addresses.Add(ip.ToString());
                }
            }

            return addresses;
        }


        public static async Task<string?> GetPublicIPAddress()
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(_httpClientTimeoutSeconds);

            foreach (var url in _publicIpCheckers)
            {
                try
                {
                    var response = await client.GetStringAsync(url);
                    if (!string.IsNullOrEmpty(response))
                    {
                        return response.Trim();
                    }
                }
                catch
                {
                    continue;
                }
            }

            return null;
        }



        public static async Task<bool> IsInternetAvailable()
        {
            var tasks = _publicDnsResolvers.Select(async dnsIp => await PingIpAsync(dnsIp));
            var results = await Task.WhenAll(tasks);
            return results.Any(result => result);
        }


        public static async Task<bool> IsProdServerAvailable() => await PingIpAsync(_prodServerIP);


        public static async Task<bool> IsDevServerAvailable() => await PingIpAsync(_devServerIP);




        private static async Task<bool> PingIpAsync(string ipAddress)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(ipAddress, _pingTimeoutMilliseconds);
                return reply != null && reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }

        }
    }
}
