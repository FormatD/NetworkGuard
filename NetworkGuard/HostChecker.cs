﻿using Serilog;
using System;
using System.Net.NetworkInformation;

namespace NetworkGuard
{
    public class HostChecker
    {
        public static bool Ping(string nameOrAddress)
        {
            try
            {
                Log.Information("Try ping {nameOrAddress}",nameOrAddress);
                using var pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                Log.Information("Return:{status}", reply.Status);
                return reply.Status == IPStatus.Success;
            }
            catch (PingException ex)
            {
                Log.Information("Ping failed,Message {ex}",ex);
                return false;
            }
            catch (Exception ex)
            {
                Log.Error("Ping failed,Message {ex}",ex);
                return false;
            }
        }

    }
}
