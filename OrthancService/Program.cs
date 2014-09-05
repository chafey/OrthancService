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
using System.ServiceProcess;
using NLog;

namespace OrthancService
{
    static class Program
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Logger.Info("Service Starting");

            // The path to the configuration file must be passed in
            if (args.Length < 1)
            {
                Logger.Error("Path to configuration file not specified, aborting");
                return;
            }

            // Load the configuration
            OrthancServiceConfiguration config;
            try
            {
                Logger.Info("Reading configuration from {0}", args[0]);
                config = OrthancServiceConfiguration.Read(args[0]);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected exception reading configuration file {0}", ex);
                return;
            }

            // Start up the service
            if (Environment.UserInteractive)
            {
                var pm = new ProcessManager(config, null);
                pm.Start();
            }
            else
            {
                ServiceBase.Run(new Services(config));
            }
        }
    }
}


