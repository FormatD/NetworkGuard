using Serilog;
using System;
using System.Diagnostics;
using System.Management;

namespace NetworkGuard
{
    public class NetworkAdapterUtil
    {
        public static void Disable(string interfaceName)
        {
            ExecuteNetshCommand("netsh", DisableInterfaceCommand(interfaceName));
        }

        public static void Enable(string interfaceName)
        {
            ExecuteNetshCommand("netsh", EnableInterfaceCommand(interfaceName));
        }

        private static void ExecuteNetshCommand(string processName, string commandName)
        {
            Log.Information($"Runing:{processName} {commandName}");
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo(processName, commandName)
            };
            p.Start();
        }

        //set interface name="Ethernet" admin=ENABLE
        private static string EnableInterfaceCommand(string interfaceName) =>
            "interface set interface name=" + interfaceName + " admin=ENABLE";

        //set interface name="Ethernet" admin=DISABLE
        private static string DisableInterfaceCommand(string interfaceName) =>
            "interface set interface name=" + interfaceName + " admin=DISABLE";
    }

    public class NetworkAdapterUtil1
    {
        public static void Disable(string interfaceName)
        {
            ExecuteNetshCommand(interfaceName, "Disable");
        }

        public static void Enable(string interfaceName)
        {
            ExecuteNetshCommand(interfaceName, "Enable");
        }

        private static void ExecuteNetshCommand(string interfaceName, string commandName)
        {
            Log.Information("Runing:{interfaceName} {commandName}", interfaceName, commandName);

            try
            {
                SelectQuery wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
                ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(wmiQuery);
                foreach (ManagementObject item in searchProcedure.Get())
                {
                    var connectionName = (string)item["NetConnectionId"];
                    Log.Debug("found interface:{connectionName}", connectionName);

                    if (connectionName == interfaceName)
                    {
                        Log.Debug("Executing:{commandName} on {interfaceName}", commandName, interfaceName);
                        item.InvokeMethod(commandName, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning("Executing failed,message", ex.Message);
            }
        }
    }
}
