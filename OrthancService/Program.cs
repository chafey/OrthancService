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

using System.ServiceProcess;

namespace OrthancService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Note that Main can be launched in a console mode. For the Console Output (STDOUT) to work
        /// be sure that the Application Property' s Output Type to be set to the Console Applicaiton
        /// </summary>
       static void Main(string[] args)
        {
            bool serviceMode = true;

            if (args.Length > 0)
            {
                if (args[0].ToLower().Contains("c")) serviceMode = false;
            }

            if (serviceMode)
            {
                var ServicesToRun = new ServiceBase[]
                {
                    new Services()
                };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                var pm = new ProcessManager();
                pm.Start();
            }
        }
    }
}


