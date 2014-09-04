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
    public partial class Services : ServiceBase
    {
        public Services()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            pm.Start();
        }

        protected override void OnStop()
        {
            pm.Stop();
        }

        private readonly ProcessManager pm = new ProcessManager();
    }
}

