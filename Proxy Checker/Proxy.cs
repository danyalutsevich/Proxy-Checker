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
            #region
            //GetProxyFromSource("https://www.proxyscan.io/download?type=https", CorrectProxy);
            //GetProxyFromSource("https://www.proxyscan.io/download?type=http", CorrectProxy);
            //GetProxyFromSource("https://www.proxyscan.io/download?type=socks4", CorrectProxy);
            //GetProxyFromSource("https://www.proxyscan.io/download?type=socks5", CorrectProxy);
            //GetProxyFromSource("http://rootjazz.com/proxies/proxies.txt", CorrectProxy);
            //GetProxyFromSource("https://free-proxy-list.net/", CorrectProxy);
            //GetProxyFromSource("http://proxysearcher.sourceforge.net/Proxy%20List.php?type=http&filtered=true", CorrectProxy);
            //GetProxyFromSource("http://proxysearcher.sourceforge.net/Proxy%20List.php?type=socks&filtered=true", CorrectProxy);
            //GetProxyFromSource("https://www.my-proxy.com/free-proxy-list-10.html", CorrectProxy);
            //GetProxyFromSource("https://www.my-proxy.com/free-elite-proxy.html", CorrectProxy);
            //GetProxyFromSource("https://proxypedia.org/", CorrectProxy);
            //GetProxyFromSource("https://kidux.net/proxies", CorrectProxy);
            //GetProxyFromSource("https://proxy-daily.com/", CorrectProxy);
            //GetProxyFromSource("https://webanetlabs.net/publ/24-1-0-1392", CorrectProxy);
            //GetProxyFromSource("https://www.us-proxy.org/", CorrectProxy);
            //GetProxyFromSource("https://openproxy.space/list/http", CorrectProxy);
            //GetProxyFromSource("https://openproxy.space/list/socks4", CorrectProxy);
            //GetProxyFromSource("https://openproxy.space/list/socks5", CorrectProxy);

            //GetProxyFromSource("https://raw.githubusercontent.com/clarketm/proxy-list/master/proxy-list-raw.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/TheSpeedX/PROXY-List/master/http.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/TheSpeedX/PROXY-List/master/socks4.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/TheSpeedX/PROXY-List/master/socks5.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/jetkai/proxy-list/main/online-proxies/txt/proxies.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/jetkai/proxy-list/main/archive/txt/working-proxies-history.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/Volodichev/proxy-list/main/http.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/mertguvencli/http-proxy-list/main/proxy-list/data.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/UserR3X/proxy-list/main/online/all.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/sunny9577/proxy-scraper/master/proxies.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/ShiftyTR/Proxy-List/master/proxy.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/hookzof/socks5_list/master/proxy.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/UptimerBot/proxy-list/main/proxies/http.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/UptimerBot/proxy-list/main/proxies/socks4.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/UptimerBot/proxy-list/main/proxies/socks5.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/mmpx12/proxy-list/master/proxies.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/manuGMG/proxy-365/main/SOCKS5.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/roosterkid/openproxylist/main/SOCKS5_RAW.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/roosterkid/openproxylist/main/SOCKS4_RAW.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/roosterkid/openproxylist/main/HTTPS_RAW.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/KUTlime/ProxyList/main/ProxyList.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/clarketm/proxy-list/master/proxy-list-raw.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/B4RC0DE-TM/proxy-list/main/proxies.txt", CorrectProxy);
            //GetProxyFromSource("https://raw.githubusercontent.com/SkyWtkh/HTTP-Proxy-List/main/free_http_proxy_list.txt", CorrectProxy);


            //for (int i = 0; i < 9000; i += 64)
            //{
            //    GetProxyFromSource($"https://hidemy.name/en/proxy-list/?start=" + i + "#list", HideMeProxy);
            //}

            #endregion

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
            Console.Clear();
            #region Task

            //ThreadPool.SetMinThreads(Proxies.Count, Proxies.Count);

            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (var proxy in Proxies)
            {
                //tasks.Add(Task.Factory.StartNew(() => { ProxyTest(i); }));
                //tasks.Add(new Task(() => { ProxyTest(proxy); }));
                tasks.Add(ProxyTest(proxy));
                //tasks[tasks.Count - 1];
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
        private static async Task<bool> ProxyTest(object o)
        {

            return await Task.Run(() =>
            {
                //semaphore.WaitOne();
                WebProxy proxy = null;
                Proxy p = null;
                try
                {
                    p = o as Proxy;
                    proxy = new WebProxy(p.ProxyAddress);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(p.ProxyAddress); ;
                    Console.WriteLine(ex);
                }

                PrintProxyStat();

                using (var web = new WebClient())
                {
                    try
                    {
                        web.Proxy = proxy;
                        Stopwatch timer = new Stopwatch();
                        timer.Start();

                        var data = web.DownloadString("https://2ip.ru/");

                        timer.Stop();
                        p.ResponseTime = timer.ElapsedMilliseconds;

                        //Console.WriteLine(data);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        p.ResponseTime = 0;
                        return false;
                    }
                }
                //semaphore.Release();
            });
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
                Console.WriteLine(ex);
                Console.WriteLine($"INVALID LINK {Source}");
            }

        }
        private static void PrintProxyStat()
        {
            var sb = new StringBuilder();
            Console.CursorVisible = false;
            Console.CursorTop = 0;
            Console.CursorLeft = 0;
            //Console.Clear();
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
