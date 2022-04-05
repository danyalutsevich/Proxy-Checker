using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.IO;

namespace Proxy_Checker
{
    class Proxy
    {
        public static List<Proxy> Proxies { get; set; }
        private static Regex IP { get; set; }
        private static Regex Port { get; set; }
        private static Regex CorrectProxy { get; set; }
        private static Regex HideMeProxy { get; set; }
        public static Semaphore semaphore { get; set; }

        static Proxy()
        {
            Proxies = new List<Proxy>();
            IP = new Regex(@"\b(?:(?:2(?:[0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9])\.){3}(?:(?:2([0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9]))\b");
            Port = new Regex(@":\d+");
            CorrectProxy = new Regex(@"(?:(?:2(?:[0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9])\.){3}(?:(?:2([0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9])):\d+");
            HideMeProxy = new Regex(@"(?:(?:2(?:[0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9])\.){3}(?:(?:2([0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9]))</td><td>\d+");
            semaphore = new Semaphore(2000, 2000);
        }

        public string ProxyAddress { get; set; }
        public long ResponseTime { get; set; }
        public int RequestsSent { get; set; }

        public override string ToString()
        {
            return $"{ProxyAddress}/{ResponseTime}";
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is Proxy)
            {
                Proxy proxy = (Proxy)obj;
                return proxy.ProxyAddress.Equals(this.ProxyAddress);
            }
            else
            {
                return false;
            }
        }
        public static void GetProxiesFromLinks()
        {

            string sources = String.Empty;

            using (var sr = new StreamReader("ProxySource.txt"))
            {
                sources = sr.ReadToEnd();
            }

            List<Task> tasks = new List<Task>();

            string[] src = sources.Split("\n");

            foreach (var s in sources.Split("\n"))
            {
                tasks.Add(GetProxyFromSource(s));
            }

            Task.WaitAll(tasks.ToArray());

            //lefts only unique proxies
            Proxies = Proxies.GroupBy(i => i.ProxyAddress).Select(i => i.FirstOrDefault()).ToList();

        }

        public static void SaveProxies()
        {
            var sb = new StringBuilder();

            foreach (var proxy in Proxies)
            {
                sb.Append(proxy.ProxyAddress);
                sb.Append("\n");
            }

            using (var sw = new StreamWriter("proxy.txt"))
            {
                sw.Write(sb.ToString());
            }
        }

        public static void GetProxiesFromFile()
        {
            using (var sr = new StreamReader("proxy.txt"))
            {
                var data = sr.ReadToEnd();

                foreach (var i in CorrectProxy.Matches(data).GroupBy(i => i.Value).Select(i => i.FirstOrDefault()))
                {
                    Proxies.Add(new Proxy { ProxyAddress = i.Value });
                }
            }
        }

        public static void FilterProxies()
        {
            #region Task

            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (var proxy in Proxies)
            {
                tasks.Add(ProxyTestAsync(proxy));
            }

            Task.WaitAll(tasks.ToArray());

            #endregion

            #region Thread

            //foreach (var proxy in Proxies)
            //{
            //    new Thread(ProxyTest).Start(proxy);
            //}

            //while (Process.GetCurrentProcess().Threads.Count > 20)
            //{
            //    PrintProxyStat();
            //}

            #endregion

            Proxies = Proxies.Where(i => i.ResponseTime != 0).ToList();
        }

        public static Proxy GetProxy()
        {
            // Gets the fastest of least used 
            int MinRequests = Proxies.Min(p => p.RequestsSent);

            var LeastUsedProxies = Proxies.FindAll(p => p.RequestsSent == MinRequests);

            long MinResponseTime = LeastUsedProxies.Min(p => p.ResponseTime);

            var proxy = LeastUsedProxies.Find(p => p.ResponseTime == MinResponseTime);

            proxy.RequestsSent++;

            return proxy;

        }

        private static void ProxyTest(object o)
        {
            Proxy proxy = null;

            using (var web = new WebClient())
            {
                try
                {
                    proxy = o as Proxy;
                    web.Proxy = new WebProxy(proxy.ProxyAddress);
                    web.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.51 Safari/537.36";

                    Stopwatch timer = new Stopwatch();
                    timer.Start();

                    Uri uri = new Uri("https://www.google.com/");

                    var data = web.DownloadString(uri);


                    timer.Stop();
                    proxy.ResponseTime = timer.ElapsedMilliseconds;

                }
                catch
                {
                    proxy.ResponseTime = 0;
                }
            }
        }
        private static async Task<bool> ProxyTestAsync(object o)
        {
            Proxy proxy = null;

            using (var web = new WebClient())
            {
                try
                {
                    proxy = o as Proxy;
                    web.Proxy = new WebProxy(proxy.ProxyAddress);
                    web.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.51 Safari/537.36";

                    Stopwatch timer = new Stopwatch();
                    timer.Start();

                    Uri uri = new Uri("https://www.google.com/");
                    await web.DownloadStringTaskAsync(uri);

                    timer.Stop();
                    proxy.ResponseTime = timer.ElapsedMilliseconds;
                    PrintProxyStat();

                    return true;
                }
                catch
                {
                    proxy.ResponseTime = 0;
                    PrintProxyStat();

                    return false;
                }
            }
        }

        private static async Task GetProxyFromSource(object o)
        {
            string Source = o as string;
            try
            {
                using (var wc = new WebClient())
                {
                    //need this so service wont identify you as a bot
                    wc.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.51 Safari/537.36";

                    var data = wc.DownloadString(Source);

                    Console.WriteLine(data);
                    //        gets array of strings that matches RegEx                      lefts only unique items
                    //                           |                                          |
                    foreach (var i in CorrectProxy.Matches(data.Replace("</td><td>", ":")).GroupBy(i => i.Value).Select(i => i.FirstOrDefault()))
                    {
                        Proxies.Add(new Proxy { ProxyAddress = i.Value });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"INVALID LINK {Source}");
            }

        }

        private static void PrintProxyStat()
        {
            var sb = new StringBuilder();
            Console.CursorVisible = false;
            Console.CursorTop = 0;
            Console.CursorLeft = 0;
            sb.Append("Threads: ");
            sb.AppendLine($"{Process.GetCurrentProcess().Threads.Count}   ");

            #region

            sb.Append("1 sec: ");
            sb.AppendLine(Proxies.Count(i => i.ResponseTime < 1000 && i.ResponseTime != 0).ToString());

            sb.Append("2 sec: ");
            sb.AppendLine(Proxies.Count(i => i.ResponseTime < 2000 && i.ResponseTime > 1000).ToString());

            sb.Append("3 sec: ");
            sb.AppendLine(Proxies.Count(i => i.ResponseTime < 3000 && i.ResponseTime > 2000).ToString());

            sb.Append("4 sec: ");
            sb.AppendLine(Proxies.Count(i => i.ResponseTime < 4000 && i.ResponseTime > 3000).ToString());

            sb.Append("5 sec: ");
            sb.AppendLine(Proxies.Count(i => i.ResponseTime < 5000 && i.ResponseTime > 4000).ToString());

            sb.Append("10 sec: ");
            sb.AppendLine(Proxies.Count(i => i.ResponseTime < 10000 && i.ResponseTime > 5000).ToString());

            sb.Append("15 sec: ");
            sb.AppendLine(Proxies.Count(i => i.ResponseTime < 15000 && i.ResponseTime > 10000).ToString());

            sb.Append("20 sec: ");
            sb.AppendLine(Proxies.Count(i => i.ResponseTime < 20000 && i.ResponseTime > 15000).ToString());

            sb.Append("Not working: ");
            sb.AppendLine(Proxies.Count(i => i.ResponseTime == 0).ToString());

            sb.Append("Responses: ");
            sb.AppendLine(Proxies.Count(i => i.ResponseTime != 0).ToString());

            Console.WriteLine(sb.ToString());
            Console.CursorVisible = true;
            #endregion

        }
    }
}
