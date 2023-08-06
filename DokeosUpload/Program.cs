/*
*  
*  Copyright (C) 2010-2011 University College Ghent.
*  
*  For a full list of contributors, see "credits.txt".
*  
*  This file is part of LMS Desktop Assistant.
*  
*  LMS Desktop Assistant is free software: you can redistribute it and/or modify
*  it under the terms of the GNU General Public License as published by
*  the Free Software Foundation, either version 3 of the License, or
*  (at your option) any later version.
*  
*  LMS Desktop Assistant is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*  GNU General Public License for more details.
*  
*  You should have received a copy of the GNU General Public License
*  along with LMS Desktop Assistant. If not, see <http://www.gnu.org/licenses/>.
*  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using lmsda.gui;
using lmsda.domain;
using lmsda.persistence.settings;
using System.IO;
using lmsda.domain.util;

namespace lmsda
{
    static class Program
    {

        /// <summary>Use a different mutex for the portable version?</summary>
        private static Boolean portableMutex = false;

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            testStandAlone();

            // Mutex: external ID string that can be used by the
            // installer / uninstaller to detect that the program is running.
            using (Mutex mutex = new Mutex(false,
                (PortableIdentifier.Instance().isPortable && portableMutex ?
                "LMSDesktopAssistantPortable" : "LMSDesktopAssistant")))
            {

                // Code to register users on startup and remove them when uninstalling.
                // The installer contains code to call the program with "-uninstall" parameter
                // at the start of the uninstallation process.

                // Uninstall all settings if uninstall parameter was given
                if (!PortableIdentifier.Instance().isPortable)
                {
                    if (Environment.GetCommandLineArgs().Contains("-uninstall"))
                    {
                        Uninstall.removeUserFolders();
                        return;
                    }
                    //Register the current user in the users list
                    Uninstall.registerUser();
                }
                //Initialize the domaincontroller for the first time.
                DomainController.Instance();

                //Start the GUI application.
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ContainerFrame());
            }
        }

        private static void testStandAlone()
        {
            String standalonesettings = ProgramConstants.getProgramPath() + ProgramConstants.SETTINGS_FILE_NAME;
            if (File.Exists(standalonesettings))
            {
                Settings testsettings = new Settings(standalonesettings, false);
                if (testsettings.getStandAlone())
                    PortableIdentifier.CreatePortableInstance();
            }
        }

    }
}
