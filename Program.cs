using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace How_Is_My_Dns
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Missing parameter. ");
                Console.WriteLine("Please specify web page address for example https://github.com");
                return;
            }

            var uri = new Uri(args[0]);
            Console.WriteLine($"Getting DNS info for host {uri.Host}");
            var hostEntry = Dns.GetHostEntry(uri.Host);
            if (hostEntry?.AddressList == null || hostEntry?.AddressList.Length == 0)
            {
                Console.WriteLine("DNS does not know this host.");
                return;
            }

            Console.WriteLine("Returned IP addresses");
            foreach (var addr in hostEntry.AddressList)
            {
                var type = addr.AddressFamily.ToString();
                if (addr.AddressFamily == AddressFamily.InterNetwork) type = "IPv4";
                if (addr.AddressFamily == AddressFamily.InterNetworkV6) type = "IPv6";
                Console.WriteLine($"{type} {addr}");
            }

            Console.WriteLine();
            Console.WriteLine("Checking connectivity.");
            foreach (var addr in hostEntry.AddressList.OrderBy(a => a.AddressFamily))
            {
                var type = addr.AddressFamily.ToString();
                if (addr.AddressFamily == AddressFamily.InterNetwork) type = "IPv4";
                if (addr.AddressFamily == AddressFamily.InterNetworkV6) type = "IPv6";
                Console.Write($"{type} {addr} .. ");

                try
                {
                    var ipUri = new UriBuilder(uri);

                    var httpClient = new HttpClient();
                    httpClient.Timeout = TimeSpan.FromSeconds(15);
                    if (ipUri.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase))
                    {
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Host", uri.Host);
                    }

                    ipUri.Host = addr.AddressFamily == AddressFamily.InterNetworkV6 ? $"[{addr}]" : addr.ToString();
                    var something = httpClient.GetAsync(ipUri.Uri).Result;
                    Console.WriteLine("OK");
                }
                catch (AggregateException exc)
                {
                    if (exc.InnerExceptions.Any(e => e.GetType() == typeof(TaskCanceledException)))
                    {
                        Console.WriteLine("timeout");
                    }
                    else Console.WriteLine(exc.Message);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("timeout");
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
            }
        }
    }
}
