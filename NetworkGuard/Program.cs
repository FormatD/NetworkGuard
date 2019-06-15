using Serilog;
using Serilog.Events;
using System;
using System.Threading;

namespace NetworkGuard
{
    class Program
    {
        private static readonly int CheckIntervalInMs = 60 * 1000;
        private static readonly int WaitAdapterRecoverTimeInMs = 20 * 1000;

        static void Main(string[] args)
        {

            var log = new LoggerConfiguration()
                .WriteTo.Console(LogEventLevel.Verbose)
                .WriteTo.RollingFile("log-{Date}.txt")
                .CreateLogger();


            while (true)
            {
                var isOnline = HostChecker.Ping("www.baidu1111w.com");
                if (isOnline)
                    Thread.Sleep(CheckIntervalInMs);
                else
                {
                    var interfaceName = "以太网";
                    NetworkAdapterUtil1.Disable(interfaceName);
                    NetworkAdapterUtil1.Enable(interfaceName);
                    Thread.Sleep(WaitAdapterRecoverTimeInMs);
                }
            }
        }
    }
}
