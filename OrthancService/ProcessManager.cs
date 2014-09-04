/*
    ORTHANC WINDOWS SERVICE
    Windows Service Shell to Run Orthanc in Service Mode
    Copyright (C) 2014  LURY, LLC. <http://lury.net>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */


using System;
using System.Diagnostics;
using System.Threading;
using NLog;
using OrthancService.Properties;
using ThreadState = System.Threading.ThreadState;

namespace OrthancService
{
    public class ProcessManager
    {
        public ProcessManager()
        {
            Settings p = Settings.Default;
            _ExeAbsolutePath = p.ExeAbsolutePath;
            _ExeArgumentsOrLeaveBlank = p.ExeArgumentsOrLeaveBlank;
            _RetryPhase1MaxRettryCounts = p.RetryPhase1MaxRettryCounts;
            _RetryPhase1IntervalMSec = (int) p.RetryPhase1IntervalFloatSec*1000;
            _RetryPhase2MaxRettryCounts = p.RetryPhase2MaxRetryCounts;
            _RetryPhase2IntervalMSec = (int) p.RetryPhase2IntervalFloatSec*1000;
            _ProcessCheckIntervalMsec = (int) p.ProcessCheckIntervalFloatSec*1000;
            _State = State.Stopped;
        }

        public void Start()
        {
            try
            {
                _Logger.Info("Start request received. {0} {1}", _ExeAbsolutePath, _ExeArgumentsOrLeaveBlank);

                if (_State != State.Stopped)
                {
                    _Logger.Error("Cannot run. Current state is in {0}", _State.ToString());
                    return;
                }

                _State = State.Stopped;
                _MonitorThread = new Thread(MonitorProcess);
                _TotalRestarts = 0;
                _MonitorThread.Start();
            }
            catch (Exception ex)
            {
                _Logger.Error("Failed to start the process '{0} {1}' due to {2}", _ExeAbsolutePath,
                    _ExeArgumentsOrLeaveBlank, ex.Message);
            }
        }

        /// <summary>
        ///     Stops the monitoring process.
        /// </summary>
        public void Stop()
        {
            _Logger.Info("Stop request received.");

            try
            {
                _MonitorThread.Abort();
                _MonitorThread.Join();

                _Process.Refresh();
                _Process.Kill();

                _State = State.Stopped;
                _Process = null;
                _Logger.Info("Process stopped sucesfully.");
            }
            catch (Exception ex)
            {
                _Logger.Error("Process top failed due to {0}", ex.Message);
            }
        }

        /// <summary>
        ///     The main thread that monitors the underlying EXE
        /// </summary>
        public void MonitorProcess()
        {  
            var startTime = DateTime.Now;

            while (true)
            {
                try
                {
                    bool quit;

                    int maxRestartCount = _RetryPhase1MaxRettryCounts + _RetryPhase2MaxRettryCounts;
                    int checkInterval = _ProcessCheckIntervalMsec;
                    if (_TotalRestarts > _RetryPhase1MaxRettryCounts) checkInterval = _RetryPhase2IntervalMSec;
                    else if (_TotalRestarts > 0) checkInterval = _RetryPhase1IntervalMSec;

                    if (_TotalRestarts <= maxRestartCount && _State != State.Running)
                    {
                        if (_State != State.Stopped)
                        {
                            _TotalRestarts++;
                            _Logger.Warn("Restarting process. State: {0}, Restarted {1} times.", _State.ToString(),
                                _TotalRestarts);
                        }
                        else
                        {
                            _Logger.Info("Starting process. State: {0}, Restarted {1} times.", _State.ToString(),
                                _TotalRestarts);
                        }

                        _Process = new Process
                        {
                            StartInfo = {FileName = _ExeAbsolutePath, Arguments = _ExeArgumentsOrLeaveBlank}
                        };

                        _Process.Start();
                        Thread.Sleep(5000); // Give a bit of time buffer before checking things out.
                        var t = _Process.Threads;

                        _State = State.Running;

                        _Logger.Info("Process stared: {0} {1}", _ExeAbsolutePath, _ExeArgumentsOrLeaveBlank);
                    }

                    if (_Process != null)
                    {
                        _Process.Refresh();

                        if (_Process.HasExited)
                        {
                            _Logger.Error("Process has exited. Attempting to restart...");
                            _State = State.Exited;
                        }
                        else
                        {
                            _TotalRestarts = 0;
                            _State = State.Running;
                            var up = DateTime.Now - startTime;
                            _Logger.Info("Process is up {0} day, {1} hours, {2} min", up.Days, up.Hours, up.Minutes);
                        }
                    }

                    // This is a check to see THIS check thread is supposed to be running
                    // The OnStop event causes this thread to stop and then we need to exit.
                    switch (_MonitorThread.ThreadState)
                    {
                        case ThreadState.Running:
                            quit = false;
                            break;

                        default:
                            quit = true;
                            break;
                    }

                    if (quit)
                    {
                        _State = State.Stopped;
                        break;
                    }

                    Thread.Sleep(checkInterval);
                }
                catch (ThreadAbortException ex)
                {
                    _Logger.Error("Monitoring thread has died. {0}", ex.Message);
                    _State = State.Exited;
                }
                catch (Exception ex)
                {
                    _Logger.Error("Monitoring thread exception caught due to {0}", ex.Message);
                }
            } 
        }

        #region PRIVATE_PARTS

        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();
        private readonly String _ExeAbsolutePath;
        private readonly String _ExeArgumentsOrLeaveBlank;
        private readonly int _ProcessCheckIntervalMsec;

        private readonly int _RetryPhase1IntervalMSec;
        private readonly int _RetryPhase1MaxRettryCounts;

        private readonly int _RetryPhase2IntervalMSec;
        private readonly int _RetryPhase2MaxRettryCounts;

        private Thread _MonitorThread;

        private Process _Process = null;
        private State _State;
        private int _TotalRestarts;

        private enum State
        {
            Stopped,
            Running,
            Exited
        };

        #endregion
    }
}