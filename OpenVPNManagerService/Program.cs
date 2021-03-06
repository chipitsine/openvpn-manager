﻿using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System.Diagnostics;
using OpenVPNUtils;

namespace OpenVPNManagerService
{
    /// <summary>
    /// Main program
    /// </summary>

    static class Program
    {
        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args">Parameters passed to the binary</param>
        [STAThread]
        static void Main(string[] args)
        {

            try
            {
                if (args.Length == 0)
                {
                    ServiceHelper.ExecuteService();
                }
                else if (args.Length == 1)
                {
                    RunDevHelper(args);
                }
                else if (args.Length == 2)
                {
                    RunSetup(args);
                }
            }
            catch (Exception ex)
            {
                string eventlogAppName = "OpenVPNManager";
                if (!EventLog.SourceExists(eventlogAppName))
                    EventLog.CreateEventSource(eventlogAppName, "Application");
                EventLog.WriteEntry(eventlogAppName, ex.ToString(), EventLogEntryType.Error, 0);
            }
        }

        private static void RunDevHelper(string[] args)
        {
            string command = args[0];
            if (command.Equals("EXECUTESERVICEASCONSOLE", StringComparison.InvariantCultureIgnoreCase))
            {
                ServiceHelper.ExecuteServiceAsConsole();
            }
        }

        private static void RunSetup(string[] args)
        {
            string command = args[0];
            string openVPNPath = args[1];

            if (command.Equals("INSTALL", StringComparison.InvariantCultureIgnoreCase))
            {
                OpenVPNManagerServiceInstaller.Install(false, openVPNPath);
                StartService();
            }
            else if (command.Equals("REINSTALL", StringComparison.InvariantCultureIgnoreCase))
            {
                StopService();
                OpenVPNManagerServiceInstaller.SetParameters(openVPNPath);
                StartService();
            }
            else if (command.Equals("UNINSTALL", StringComparison.InvariantCultureIgnoreCase))
            {
                StopService();
                OpenVPNManagerServiceInstaller.Install(true, openVPNPath);
            }
        }

        private static void StopService()
        {
            ServiceController sc = new ServiceController();
            sc.ServiceName = "OpenVPNManager";
            if (sc.Status == ServiceControllerStatus.Running)
            {
                try
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(60));
                }
                catch (InvalidOperationException)
                { }
            }
        }

        private static void StartService()
        {
            ServiceController sc = new ServiceController();
            sc.ServiceName = "OpenVPNManager";
            if (sc.Status == ServiceControllerStatus.Stopped)
            {
                try
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(60));
                }
                catch (InvalidOperationException)
                { }
            }
        }
    }
}