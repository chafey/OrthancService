/**
 * Orthanc Service - A Windows Service for Orthanc
 * Copyright (C) 2012-2014 Lury, LLC
 *
 * This program is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 *
 * In addition, as a special exception, the copyright holders of this
 * program give permission to link the code of its release with the
 * OpenSSL project's "OpenSSL" library (or with modified versions of it
 * that use the same license as the "OpenSSL" library), and distribute
 * the linked executables. You must obey the GNU General Public License
 * in all respects for all of the code used other than "OpenSSL". If you
 * modify file(s) with this exception, you may extend this exception to
 * your version of the file(s), but you are not obligated to do so. If
 * you do not wish to do so, delete this exception statement from your
 * version. If you delete this exception statement from all source files
 * in the program, then also delete it here.
 * 
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 **/

using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using NLog;

namespace OrthancService
{
    /// <summary>
    /// This class is responsible for launching Orthanc and restarting it if
    /// it terminates unexpectedly.  
    /// </summary>
    public class ProcessManager
    {
        public ProcessManager(OrthancServiceConfiguration config, ServiceBase serviceBase)
        {
            _config = config;
            _serviceBase = serviceBase;

            Logger.Info("OrthancPath = {0}", config.OrthancPath);
            Logger.Info("CommandLineArguments = {0}", config.CommandLineArguments);
            Logger.Info("WorkingDirectory = {0}", config.WorkingDirectory);
        }

        public void Start()
        {
            try
            {
                Logger.Info("Starting Service");

                if (_monitorThread != null && _monitorThread.IsAlive)
                {
                    Logger.Warn("Monitor Thread is already running, aborting it");
                    _monitorThread.Abort();
                    _monitorThread.Join();
                    _monitorThread = null;
                    return;
                }

                _monitorThread = new Thread(MonitorProcess);
                _monitorThread.Start();

                Logger.Info("Service Started Succesfuly");
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected exception while starting service {0}", ex.Message);
            }
        }

        /// <summary>
        ///     Stops the monitoring process.
        /// </summary>
        public void Stop()
        {
            try
            {
                Logger.Info("Stopping service");

                // Abort the monitor thread
                if (_monitorThread != null && _monitorThread.IsAlive)
                {
                    _monitorThread.Abort();
                    _monitorThread.Join();
                    _monitorThread = null;
                }

                // Kill the child process
                _process.Refresh();
                // XXX: We should consider shutting down Orthanc gracefully
                _process.Kill();
                _process = null;

                Logger.Info("Service stopped succesfully");
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected exception while stopping service {0}", ex.Message);
            }
        }

        public void MonitorProcess()
        {
            Logger.Debug("Monitor thread started");
            while (true)
            {
                try
                {
                    // Setup the process object
                    _process = new Process
                    {
                        StartInfo =
                        {
                            FileName = _config.OrthancPath,
                            Arguments = _config.CommandLineArguments,
                            WorkingDirectory = _config.WorkingDirectory
                        }
                    };

                    // Attempt to start the process.   
                    if (StartProcess() == false)
                    {
                        // Could not start process, if we are a windows service, stop ourselves 
                        if (_serviceBase != null)
                        {
                            Logger.Info("Stopping Service");
                            _serviceBase.Stop();
                        }
                        // exit the thread so we stop trying to start the process
                        return;
                    }

                    _process.WaitForExit();

                    _totalRestarts++;

                    Logger.Warn("Orthanc exited, restarting it (Restart # {0}).", _totalRestarts);
                }
                catch (ThreadAbortException ex)
                {
                    Logger.Debug("Orthanc Monitoring thread exiting due to ThreadAbortException {0}", ex.Message);
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Warn("Monitoring thread ignoring unexpected exception {0}", ex.Message);
                }
            }
        }
     
        private bool StartProcess()
        {
            try
            {
                Logger.Info("Starting Orthanc");
                _process.Start();
                Logger.Info("Orthanc Started");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception caught trying to start Orthanc - {0}", ex.Message);
                return false;
            }
        }

        #region PRIVATE_PARTS

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly OrthancServiceConfiguration _config;
        private readonly ServiceBase _serviceBase;

        private Thread _monitorThread;
        private Process _process;
        private int _totalRestarts;

        #endregion
    }
}