using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Threading;

namespace NetworkGuard
{
    class Program
    {
        private static readonly string ConfigFileName = "config.json";
        private static string _lastLogFile;

        static void Main()
        {
            InitLogger();

            while (true)
            {
                var config = LoadConfig();
                InitLogger(config.LogFile);

                var isOnline = HostChecker.Ping(config.TestHostName);

                if (isOnline)
                    Thread.Sleep(config.CheckIntervalInMs);
                else
                {
                    NetworkAdapterUtil1.Disable(config.InterfaceName);
                    NetworkAdapterUtil1.Enable(config.InterfaceName);
                    Thread.Sleep(config.WaitAdapterRecoverTimeInMs);
                }
            }
        }

        private static Config LoadConfig()
        {
            Config config = null;
            if (File.Exists(ConfigFileName))
            {
                try
                {
                    config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFileName));
                }
                catch (Exception ex)
                {
                    Log.Warning("Load config file failed,invalid config,message:{message}", ex.Message);
                }
            }

            if (config == null)
            {
                config = new Config();
                Log.Information("Try create default config file.");
                try
                {
                    File.WriteAllText(ConfigFileName, JsonConvert.SerializeObject(config, Formatting.Indented));
                    Log.Information("Default config file created.");
                }
                catch (Exception ex)
                {
                    Log.Warning("Create default config failed,message {message}.", ex.Message);
                }
            }

            return config;
        }

        private static void InitLogger(string fileName = null)
        {
            if (fileName != null && fileName != _lastLogFile)
            {
                _lastLogFile = fileName;
                var log = new LoggerConfiguration()
                                .WriteTo.ColoredConsole(LogEventLevel.Verbose)
                                .WriteTo.RollingFile(fileName ?? new Config().LogFile)
                                .CreateLogger();
                Log.Logger = log;
            }
        }
    }

    public class Config
    {
        public int CheckIntervalInMs { get; set; } = 60 * 1000;

        public int WaitAdapterRecoverTimeInMs { get; set; } = 20 * 1000;

        public string InterfaceName { get; set; } = "以太网";

        public string TestHostName { get; set; } = "www.baidu.com";

        public string LogFile { get; set; } = "logs\\{Date}.log";
    }
}
