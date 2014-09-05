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
using System.IO;

namespace ConfigurationFileWriter
{
    /// <summary>
    /// This program is called by the installer to patch the various file paths in the configuration
    /// files used by Orthanc and OrthancService to point to the directory paths specified by the
    /// user during installation.  While this should be possible to do in NSIS, I was unsuccesful
    /// at getting the nsJSON plugin to parse/read the configuration files so I gave up and
    /// decided to write this program instead.  
    /// 
    /// NOTE: The patching does a very simple search and replace on the files and is therefore
    /// dependent upon specific strings (and casing) in the configuration files.  A simple
    /// search and replace is used because the comments in the configuration files would be
    /// lost if the file was saved back out.
    /// </summary>
    class Program
    {
        static void PatchFile(string filePath, string dataFolder, string programFolder)
        {
            Console.WriteLine("Patching File " + filePath);
            var fileContents = File.ReadAllText(filePath);
            fileContents = fileContents.Replace("C:/Orthanc", dataFolder);
            fileContents = fileContents.Replace("C:/Program Files (x86)/Orthanc", programFolder);
            fileContents = fileContents.Replace("OrthancStorage", dataFolder + "/OrthancStorage");
            File.WriteAllText(filePath, fileContents);
        }

        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("USAGE:");
                Console.WriteLine("  ConfigurationFileWriter [dataFolder] [programFolder]");
                return -1;
            }

            try
            {
                var dataFolder = args[0].Replace("\\", "/");
                var programFolder = args[1].Replace("\\", "/");
                PatchFile(Path.Combine(dataFolder, "OrthancServiceConfiguration.json"), dataFolder, programFolder);
                PatchFile(Path.Combine(dataFolder, "Configuration.json"), dataFolder, programFolder);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Exception - " + ex);
                return -2;
            }
            return 0;
        }
    }
}
