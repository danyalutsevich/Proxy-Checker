using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.IO;

namespace Proxy_Checker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Proxy.GetProxiesFromLinks();
            Proxy.FilterProxies();
            Proxy.SaveProxies();
        }
    }
}
