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
using System.Text;
using lmsda.domain;
using lmsda.domain.util;
using System.IO;
using System.Windows.Forms;

namespace lmsda.persistence.settings
{

    /// <summary>
    ///     Author: Maarten Meuris
    ///     This class allows a global all-user install and uninstall in which all users have their own settings nonetheless.
    ///     It handles registering user settings folders in one file in the Common Application Data folder,
    ///     and has a function to remove the settings of all registered users on uninstall.
    /// </summary>
    static class Uninstall
    {
        const String USERINFOTEXT1 = "Settings folders for all users of the program";
        const String USERINFOTEXT2 = "Do not delete: this is used for uninstalling the program!";
        const Char   USERINFOCOMMENT = '#';

        /// <summary>
        ///     This writes the current user's settings folder to a text file so it can be removed on uninstall.
        /// </summary>
        public static void registerUser()
        {
            String userstoragepath = ProgramConstants.getProgramStoragePath();
            String currentuserpath = ProgramConstants.getProgramSettingsPath();
            String filepath = userstoragepath + ProgramConstants.USER_STORAGE_FILE_NAME;
            // probably not needed if created by the installer
            if (!Utility.doesFolderExist(userstoragepath))
            {
                DirectoryInfo globaldir = new DirectoryInfo(userstoragepath);
                globaldir.Create();
            }
            // read current list of user paths
            List<String> registeredusers = readRegisteredUsers();

            // add this user
            if (!registeredusers.Contains(currentuserpath))
            {
                StreamWriter streamWriter = null;
                try
                {
                    if (!File.Exists(filepath) || registeredusers.Count == 0)
                    {
                        streamWriter = File.CreateText(filepath);
                        streamWriter.Write(getUsersFileHeader());
                        streamWriter.Close();
                        streamWriter = null;
                    }
                    streamWriter = File.AppendText(filepath);
                    streamWriter.Write(Environment.NewLine + currentuserpath);
                }
                catch { }
                finally
                {
                    if (streamWriter != null)
                        streamWriter.Close();
                }
            }
        }

        /// <summary>
        ///     Delete settings folders and their contents for all users.
        ///     This is normally only ran on uninstall, with full admin rights
        /// </summary>
        public static void removeUserFolders()
        {
            // read current list of user paths
            List<String> registeredUsers = readRegisteredUsers();
            List<String> usersToDelete;
            String nonAdminUser = filterNonAdminUser(registeredUsers);
            if (nonAdminUser != null)
                usersToDelete = new List<String>(new String[] { nonAdminUser });
            else
                usersToDelete = new List<String>(registeredUsers);

            List<String> deletedUsers = new List<String>();
            // Delete settings folders and their contents of all users.
            foreach (String path in usersToDelete)
            {
                if (Utility.doesFileExist(path + ProgramConstants.SETTINGS_FILE_NAME))
                {
                    Settings settings = new Settings(path + ProgramConstants.SETTINGS_FILE_NAME, false);

                    // don't delete if default (My Documents) folder.
                    if (!settings.getStatsFolder().Equals(Environment.SpecialFolder.Personal))
                        try { Directory.Delete(settings.getStatsFolder()); }
                        catch { }
                    if (!settings.getSubjectsFolder().Equals(Environment.SpecialFolder.Personal))
                        try { Directory.Delete(settings.getSubjectsFolder()); }
                        catch { }
                }
                try
                {
                    Directory.Delete(path, true);
                    deletedUsers.Add(path);
                }
                catch { }
            }
            if (deletedUsers.Count() == registeredUsers.Count() || nonAdminUser == null)
            {
                // either only installed for current user, or all users are deleted by admin uninstall: remove storage path.
                try { Directory.Delete(ProgramConstants.getProgramStoragePath(), true); }
                catch { }
            }
            else if (registeredUsers.Count() > 1 && nonAdminUser != null && registeredUsers.Contains(nonAdminUser) && deletedUsers.Contains(nonAdminUser))
            {
                // remove current user from path listing; to make sure the code keeps working for multiple non-admin installs
                // on one machine.
                registeredUsers.Remove(nonAdminUser);

                // rewrite file without current user's folder
                String filepath = ProgramConstants.getProgramStoragePath() + ProgramConstants.USER_STORAGE_FILE_NAME;
                StreamWriter streamWriter = null;
                try
                {
                    streamWriter = File.CreateText(filepath);
                    streamWriter.Write(getUsersFileHeader());
                    streamWriter.Close();
                    streamWriter = File.AppendText(filepath);

                    foreach (String path in registeredUsers)
                        streamWriter.Write(Environment.NewLine + path);
                }
                catch { }
                finally
                {
                    if (streamWriter != null)
                        streamWriter.Close();
                }
            }
        }

        /// <summary>
        ///     Returns a list of all user settings folders saved in
        ///     {CommonApplicationData}\LMS Desktop Assistant\userfolders.txt
        /// </summary>
        /// <returns>The list of all user settings folders</returns>
        private static List<String> readRegisteredUsers()
        {
            String filepath = ProgramConstants.getProgramStoragePath() + ProgramConstants.USER_STORAGE_FILE_NAME;
            List<String> registeredusers = new List<String>();
            try
            {
                String line;
                StreamReader streamReader = File.OpenText(filepath);
                while ((line = streamReader.ReadLine()) != null)
                {
                    // comments with #
                    if (line.Trim().Length > 0 && line[0] != USERINFOCOMMENT)
                        registeredusers.Add(line);
                }
                streamReader.Close();
            }
            catch { }
            return registeredusers;
        }

        /// <summary>
        ///     If the installation is a non-admin install, filter out the folder of the installed user.
        /// </summary>
        /// <param name="registeredusers">The list of user folders to filter from</param>
        /// <returns>The settings folder of the admin user that installed the program as non-admin, or null if not found</returns>
        private static String filterNonAdminUser(List<String> registeredusers)
        {
            // for non-admin install: only return the path of the user that installed it, even if there are more.
            // This system is built in to prevent non-admin installs performed by an admin user from removing settings
            // of other non-admin installs on the same system.

            // The non-admin installer program writes the "{userappdata}\LMS Desktop Assistant" path
            // of the user that installed it to the "nonadmin.ini" file in the application folder.

            String inipath = Path.GetDirectoryName(Application.ExecutablePath).TrimEnd('\\') + @"\nonadmin.ini";
            if (Utility.doesFileExist(inipath))
            {
                try
                {
                    String line = null;
                    String userFolderKey = "settingspath=";
                    String userFolder = null;

                    StreamReader streamReader = File.OpenText(inipath);
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        // find key
                        if (line.StartsWith(userFolderKey))
                        {
                            userFolder = line.Substring(userFolderKey.Length).TrimEnd('\\');
                            break;
                        }
                    }
                    streamReader.Close();
                    if (userFolder != null)
                    {
                        foreach (String path in registeredusers)
                        {
                            if (path.TrimEnd('\\').Equals(userFolder))
                                return path;
                        }
                    }
                }
                catch { }
            }
            return null;
        }

        /// <summary>
        ///     Returns the header for the user folders file.
        /// </summary>
        /// <returns>The header for the user folders file.</returns>
        private static String getUsersFileHeader()
        {
            return USERINFOCOMMENT + Environment.NewLine
                                     + USERINFOCOMMENT + ' ' + USERINFOTEXT1 + Environment.NewLine
                                     + USERINFOCOMMENT + ' ' + USERINFOTEXT2 + Environment.NewLine
                                     + USERINFOCOMMENT;
        }
    }
}
