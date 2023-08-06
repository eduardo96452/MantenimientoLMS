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
using System.Text.RegularExpressions;
using System.IO;
using lmsda.persistence.platform.service;
using System.Diagnostics;
using Microsoft.Win32;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using lmsda.persistence.document;
using System.Xml;

namespace lmsda.domain.util
{
    /// <summary>
    ///     Author: Gianni Van Hoecke.
    ///     This static class provides useful methods.
    /// </summary>
    /// <remarks>
    ///     Last updated on 13/08/2010 by Gianni Van Hoecke
    ///      -> Improved error handling.
    /// </remarks>
    static class Utility
    {
        public static String EMPTY_PARAGRAPH_REGEX = @"<p\s*>(&nbsp;|\s)*</p>";
        public static String ONLY_EMPTY_PARAGRAPH_REGEX = "^" + EMPTY_PARAGRAPH_REGEX + "$";

        #region Files and folders

        /// <summary>
        ///     Replaces all invalid file name characters by spaces.
        /// </summary>
        /// <param name="filename">The string. (Most likely a file name.)</param>
        /// <returns>A valid file name.</returns>
        public static String getFileNameWithoutBadCharacters(String filename)
        {
            String invalidChars = new String(Path.GetInvalidFileNameChars());
            String invalidCharsAndSpace = invalidChars + " ";
            String invalidCharsRegex = Regex.Escape(invalidChars);
            // Extra trim added to remove '\r' characters from the end since they appear in the text taken from word paragraphs.
            filename = filename.Trim(invalidCharsAndSpace.ToCharArray());
            // removes the optional hyphen characters from the name
            filename = filename.Replace(String.Empty + (char)0x1F, String.Empty);
            String invalidReStr = String.Format(@"[{0}]", invalidCharsRegex);
            String value = Regex.Replace(filename, invalidReStr, " ");
            return value.Trim();
        }

        /// <summary>
        ///     Returns all files within the given folder, filtered on extension.
        /// </summary>
        /// <param name="fullPath">The path to the folder.</param>
        /// <param name="extension">The extension.</param>
        /// <returns>A list containing all files (inc. path).</returns>
        public static List<String> getAllFilesInFolder(String fullPath, String extension)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(fullPath);
                FileInfo[] fileInfo = directoryInfo.GetFiles("*" + extension);
                List<String> files = new List<String>();
                for (int i = 0; i < fileInfo.Length; i++)
                {
                    files.Add(fullPath.TrimEnd('\\') + @"\" + fileInfo[i].Name);
                }
                return files;
            }
            catch (Exception ex)
            {
                if (DomainController.hasInstance())
                {
                    List<String[]> pairs = new List<String[]>();
                    pairs.Add(new String[] { "File", fullPath });
                    pairs.Add(new String[] { "Extension", extension });
                    DomainController.Instance().processError(ex,
                                                            !DomainController.Instance().isSynchronization,
                                                            ex.Message,
                                                            false,
                                                            pairs);
                }
                return new List<String>();
            }
        }

        /// <summary>
        ///     Checks if the given full file name exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>True if the given full file name exists.</returns>
        public static Boolean doesFileExist(String path)
        {
            try
            {
                return new FileInfo(path).Exists;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Checks if the given folder exists.
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <returns>True if the given folder exists.</returns>
        public static Boolean doesFolderExist(String path)
        {
            try
            {
                return new DirectoryInfo(path).Exists;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Checks if the given file or folder exists.
        /// </summary>
        /// <param name="path">The path to the folder or to the file.</param>
        /// <param name="showMessageBoxOnError">Indicates if a message box has to be shown on error.</param>
        /// <returns>True if the given folder or file exists.</returns>
        public static Boolean doesFileOrFolderExist(String path, Boolean showMessageBoxOnError)
        {
            Boolean retValue;
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                if((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    retValue = doesFolderExist(path);
                else
                    retValue = doesFileExist(path);
            }
            catch
            {
                retValue = false;
            }
            
            if(showMessageBoxOnError && !retValue && DomainController.hasInstance())
                DomainController.Instance().fireMessageBox(DomainController.Instance().getLanguageString("given_file_or_folder_does_not_exist"), System.Windows.Forms.MessageBoxIcon.Error);

            return retValue;
        }

        /// <summary>
        ///     Tries to delete the given file.
        /// </summary>
        /// <param name="fileName">The full file name.</param>
        /// <returns>True if the removal succeeds.</returns>
        public static Boolean tryDeleteFile(String fileName)
        {
            try
            {
                File.Delete(fileName);
                return true;
            }
            catch
            {
                if (DomainController.hasInstance())
                    DomainController.Instance().writeToLog("delete_failed", true, true, false);
                return false;
            }
        }

        /// <summary>
        ///     Tries to rename a file.
        /// </summary>
        /// <param name="newName">The source full file name.</param>
        /// <param name="sourceFile">The new full file name.</param>
        /// <returns>True if the rename succeeds.</returns>
        public static Boolean tryRenameFile(String sourceFile, String newName)
        {
            try
            {
                File.Move(sourceFile, newName);
                return true;
            }
            catch
            {
                if (DomainController.hasInstance())
                    DomainController.Instance().writeToLog("rename_failed", true, true, false);
                return false;
            }
        }

        /// <summary>
        ///     Removes a folder, including its contents.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns>True if the removal succeeds.</returns>
        public static Boolean deleteFolderAndContents(String folder)
        {
            try
            {
                Directory.Delete(folder, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Returns all subfolders within the specified folder.
        /// </summary>
        /// <param name="basePath">The folder path.</param>
        /// <returns>All folders within the specified folder.</returns>
        public static String[] getSubfolders(String basePath)
        {
            try
            {
                DirectoryInfo[] directoryInfo = new DirectoryInfo(basePath).GetDirectories();

                String[] folders = new String[directoryInfo.Length];

                for (int i = 0; i < directoryInfo.Length; i++)
                {
                    folders[i] = directoryInfo[i].Name;
                }

                return folders;
            }
            catch (Exception ex)
            {
                if (DomainController.hasInstance())
                {
                    List<String[]> pairs = new List<String[]>();
                    pairs.Add(new String[] { "File", basePath });
                    DomainController.Instance().processError(ex,
                                                            false,
                                                            ex.Message,
                                                            false,
                                                            pairs);
                }
                return new String[]{};
            }
        }

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static String MakeRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");
            if (!fromPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                fromPath += Path.DirectorySeparatorChar;

            Uri testuri = new Uri(@"C:\Documents and Settings\mmeu164\Mijn documenten\..\..\All Users\");
            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);

            return relativeUri.OriginalString.Replace('/',Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Creates an absolute path from a base and a relative path.
        /// </summary>
        /// <param name="basePath">Contains an absolute path to a directory.</param>
        /// <param name="toPath">Contains the relative file or directory path to be added to the absolute base path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static String combineRelativePath(String basePath, String relPath)
        {
            String newpath = basePath;
            String separator = Path.DirectorySeparatorChar.ToString();
            String doubleseparator = String.Empty + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar;
            if (!newpath.EndsWith(separator))
                newpath += separator;

            newpath += relPath;

            while (newpath.Contains(doubleseparator))
                newpath.Replace(doubleseparator,separator);

            return new Uri(newpath).AbsolutePath;
        }

        /// <summary>
        ///     Checks whether a file or folder is in one of the folders in a list.
        ///     Folder names should end with a separator for this to work.
        /// </summary>
        /// <param name="relativeFileName">The relative file name of a file or folder.</param>
        /// <returns>True if the file or folder is in one of the folders in the list.</returns>
        public static Boolean isInFolderList(String relativeFileName, List<String> folders, char separator, Boolean recursive)
        {
            String folder;
            foreach (String s in folders)
            {
                folder = s.TrimEnd(separator) + separator;
                if (relativeFileName.StartsWith(folder))
                {
                    if (recursive)
                        return true;
                    else if (!relativeFileName.Substring(0, folder.Length).Contains(separator))
                        return true;
                }
            }
            return false;
        }

        #endregion

        #region Validators

        /// <summary>
        ///     Checks if the given string is a valid numeric value.
        /// </summary>
        /// <param name="value">The String.</param>
        /// <returns>True if the string is a valid numeric value.</returns>
        public static Boolean isNumeric(String value)
        {
            try
            {
                Convert.ToInt16(value);
                return true;
            }
            catch (Exception)
            {
                //When conversion fails, the string is not a valid numeric value.
                return false;
            }
        }

        /// <summary>
        ///     Checks if the given character is a valid numeric value.
        /// </summary>
        /// <param name="value">The character.</param>
        /// <returns>True if the character is a valid numeric value.</returns>
        public static Boolean isNumeric(char value)
        {
            // convert char to actual numeric value
            int val=value-0x30;
            // test if numeric value is between 0 and 9
            if (val<0 || val>9)
                return false;
            else
                return true;
        }

        #endregion

        #region Formatting

        /// <summary>
        ///     Strips all HTML from a string.
        /// </summary>
        /// <param name="htmlText">The text to strip all HTML from.</param>
        /// <returns>The same string, but without all HTML.</returns>
        public static String stripHTML(String htmlText)
        {
            return Regex.Replace(htmlText, @"<(.|\n)*?>", String.Empty);
        }

        public static String fillEmptyHTMLParagraphs(String htmlText)
        {
            htmlText = Regex.Replace(htmlText, ONLY_EMPTY_PARAGRAPH_REGEX, String.Empty);
            Regex r = new Regex(@"<p(\s.*?>|>)\s*</p>", RegexOptions.IgnoreCase);
            Match matcher = r.Match(htmlText);
            while (matcher.Success)
            {
                if (!Regex.IsMatch(matcher.Groups[1].Value, @"<p(\s|>)"))
                {
                    String orig = matcher.Groups[0].Value;

                    String repl = matcher.Groups[1].Value.Trim();
                    repl = "<p" + (repl.Equals(String.Empty) ? String.Empty : " ") + repl + "&nbsp;</p>";
                    htmlText = htmlText.Replace(orig, repl);
                }
                matcher = matcher.NextMatch();
            }
            return htmlText;
        }

        /// <summary>
        ///     Replaces "&lt;p&gt;...&lt;/p&gt;&lt;p&gt;...&lt;/p&gt;" by "...&lt;br /&gt;..."
        /// </summary>
        /// <param name="htmlText">The text to strip all HTML from.</param>
        /// <returns>The same string, but without all HTML.</returns>
        public static String stripHTMLParagraphs(String htmlText)
        {
            htmlText = Regex.Replace(htmlText, "<p.*?>", "<p>");
            htmlText = Regex.Replace(htmlText, ONLY_EMPTY_PARAGRAPH_REGEX, String.Empty);
            htmlText = Regex.Replace(htmlText, @"\s*</p>\s*$", String.Empty);
            htmlText = Regex.Replace(htmlText, @"<p.*?>", String.Empty);
            htmlText = Regex.Replace(htmlText, @"\s*</p>", "<br />");
            return htmlText;
        }

        /// <summary>
        ///     Replaces "&lt;p&gt;...&lt;/p&gt;&lt;p&gt;...&lt;/p&gt;" by "...&lt;br /&gt;..."
        ///     (alternate method, not sure which is better)
        /// </summary>
        /// <param name="htmlText">The text to strip all HTML from.</param>
        /// <returns>The same string, but without all HTML.</returns>
        public static String stripHTMLParagraphs2(String htmlText)
        {
            htmlText = Regex.Replace(htmlText, "<p.*?>", "<p>");
            htmlText = Regex.Replace(htmlText, ONLY_EMPTY_PARAGRAPH_REGEX, String.Empty);
            htmlText = Regex.Replace(htmlText, @"^<p.*?>", String.Empty);
            htmlText = Regex.Replace(htmlText, @"<p.*?>", "<br />");
            htmlText = Regex.Replace(htmlText, @"\s*</p>", String.Empty);
            return htmlText;
        }

        /// <summary>
        ///     Converts the given bytes to a formatted string.
        ///     E.g.: 2683283 bytes will become 2,56 MiB.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The formatted value.</returns>
        public static String getFormattedBytes(long bytes)
        {
            String formattedBytes = String.Empty;
            long rawBytes = bytes;
            double fBytes = 0.0;
            int counter = 0;

            while(bytes >= 1024)
            {
                counter++;
                bytes /= 1024;
            }

            double rawBytesD = Convert.ToDouble(rawBytes);
            double pow = Convert.ToDouble(Math.Pow(1024.00, Convert.ToDouble(counter)));

            fBytes = Math.Round(rawBytesD / pow, 2);

            formattedBytes = Convert.ToString(fBytes) + " " + Utility.getSuffixForBytesFormatting(counter);

            return formattedBytes;
        }

        /// <summary>
        ///     Returns the text representing the number.
        ///     E.g.:
        ///         0 returns "B" (of bytes)
        ///         2 returns "MiB" (of mebibytes)
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The text representing that number.</returns>
        public static String getSuffixForBytesFormatting(int index)
        {
            String[] bytesSuffix = new String[]
                                        {
                                            "B",     //byte
                                            "KiB",   //kilobyte
                                            "MiB",   //megabyte
                                            "GiB",   //gigabyte
                                            "TiB",   //terabyte
                                            "PiB",   //petabyte
                                            "EiB",   //exabyte
                                            "ZiB",   //zettabyte
                                            "YiB"    //yottabyte
                                        };
            return bytesSuffix[index];
        }

        #endregion

        #region Conversions

        /// <summary>
        ///     Converts a two dimensional object array to a two dimensional string array.
        /// </summary>
        /// <param name="array">The two dimensional object array.</param>
        /// <param name="startsOn">Start index. (E.g.: 0 or 1)</param>
        /// <returns>The two dimensional String array.</returns>
        public static String[,] convertTwoDimensionalObjectArrayToStringArray(Object[,] array, int startsOn)
        {
            String[,] returnArray = new String[array.GetLength(0) + 1, array.GetLength(1) + 1];

            for (int i = startsOn; i <= array.GetLength(0); i++)
            {
                for (int j = startsOn; j <= array.GetLength(1); j++)
                {
                    returnArray[i, j] = Convert.ToString(array[i, j]);
                }
            }

            return returnArray;
        }

        /// <summary>
        ///     Converts the Service enum class to an String array.
        /// </summary>
        /// <returns>An array.</returns>
        public static String[] serviceAsStringArray()
        {
            int counter = 0;

            foreach(int value in Enum.GetValues(typeof(Service)))
            {
                counter++;
            }

            String[] returnValue = new String[counter];

            counter = 0;

            foreach(int value in Enum.GetValues(typeof(Service)))
            {
                returnValue[counter] = Enum.GetName(typeof(Service), counter);
                counter++;
            }

            return returnValue;
        }

        /// <summary>
        ///     Checks whether a new version string is actually newer than the current version.
        /// </summary>
        /// <param name="currentVer">The current version string</param>
        /// <param name="newVer">The new version string</param>
        /// <returns>True if the new version is higher</returns>
        public static Boolean isHigherNewVersion(String currentVer, String newVer)
        {
            if (currentVer.Equals(newVer))
                return false;
            String[] curVersion = currentVer.Split('.');
            String[] newVersion = newVer.Split('.');;
            int longest = Math.Max(curVersion.Length, newVersion.Length);
            for (int i = 0; i < longest; i++)
            {
                int curVal = 0;
                int newVal = 0;
                try { curVal = Int32.Parse(curVersion[i]); } catch {}
                try { newVal = Int32.Parse(newVersion[i]); } catch {}

                // return from the first real difference
                if (newVal > curVal)
                    return true;
                else if (newVal < curVal)
                    return false;
            }
            return false;
        }

        #endregion

        #region Processes

        public static void openFolderInExplorer(String path)
        {
            if (new DirectoryInfo(path).Exists)
                Process.Start(Environment.ExpandEnvironmentVariables("%systemroot%") + @"\explorer.exe", "/e,\"" + path.Trim('\"') + "\"");
        }

        /// <summary>
        ///     Opens a local file or url in the default web browser.
        /// </summary>
        /// <param name="path">Path of the local file or url</param>
        public static void openInDefaultBrowser(String pathOrUrl)
        {
            pathOrUrl = "\"" + pathOrUrl.Trim('"') + "\"";
            RegistryKey defBrowserKey = Registry.ClassesRoot.OpenSubKey( @"http\shell\open\command" );
            if (defBrowserKey != null && defBrowserKey.ValueCount > 0 && defBrowserKey.GetValue("") != null)
            {
                String defBrowser = (String)defBrowserKey.GetValue("");
                if (defBrowser.Contains("%1"))
                {
                    defBrowser = defBrowser.Replace("%1", pathOrUrl);
                }
                else
                {
                    defBrowser += " " + pathOrUrl;
                }
                String defBrowserProcess;
                String defBrowserArgs;
                if (defBrowser[0] == '"')
                {
                    defBrowserProcess = defBrowser.Substring(0, defBrowser.Substring(1).IndexOf('"')+2).Trim();
                    defBrowserArgs =    defBrowser.Substring(defBrowser.Substring(1).IndexOf('"')+2).TrimStart();
                }
                else
                {
                    defBrowserProcess = defBrowser.Substring(0, defBrowser.IndexOf(" ")).Trim();
                    defBrowserArgs = defBrowser.Substring(defBrowser.IndexOf(" ")).Trim();
                }
                if (new FileInfo(defBrowserProcess.Trim('"')).Exists)
                    Process.Start(defBrowserProcess, defBrowserArgs);
            }
        }

        /// <summary>
        ///     Starts the given path to a file or folder.
        /// </summary>
        /// <param name="path">The path to file or folder.</param>
        public static void StartWithShell(String path)
        {
            try 
            { 
                Process.Start(path); 
            } 
            catch(Exception ex)
            {
                if (DomainController.hasInstance())
                {
                    List<String[]> pairs = new List<String[]>();
                    pairs.Add(new String[] { "File", path });
                    DomainController.Instance().processError(ex,
                                                            !DomainController.Instance().isSynchronization,
                                                            ex.Message,
                                                            false,
                                                            pairs);
                }
            }
        }

        #endregion

        #region String manipulations

        /// <summary>
        ///     Gets a value yyy from URL-arguments of the type "url://address?attr=yyy&amp;attr=yyy".
        ///     for a specified attributename attr.
        /// </summary>
        /// <param name="url">URL in which to find the value.</param>
        /// <param name="attributename">Name of the attribute we're looking for.</param>
        /// <returns>Value of the attribute.</returns>
        public static String findValueInURL(String url, String attributename)
        {
            if (!url.Contains('?') || !url.Contains('='))
                return String.Empty;
            url = url.Replace("&amp;","&");
            url=url.Substring(url.IndexOf('?')+1);
            if(url.Contains('#'))
                url=url.Substring(0, url.IndexOf('#'));
            String[] args = url.Split(new Char[]{'&'},StringSplitOptions.None);
            foreach (String arg in args)
            { 
                if(arg.Contains('='))
                {
                    if (arg.Substring(0, arg.IndexOf('=')).Equals(attributename))
                    {
                        return arg.Substring(arg.IndexOf('=') + 1);
                    }
                }
            }
            return String.Empty;
        }

        public static String findPropertyInXmlNodeText(String xmlString, String property)
        {
            // must be valid opening tag
            xmlString = xmlString.Trim();
            if (!xmlString.StartsWith("<") || !xmlString.EndsWith(">") || xmlString.StartsWith("</"))
                return String.Empty;
            // find name: 1. make sure there is a space, 2. trim to the first space, 3. Trim the < off start.
            String xmlName = xmlString.Replace("/>", " />").Replace(">", " >").Substring(0, xmlString.IndexOf(" ")).TrimStart('<');
            // ensure there is an ending
            if (!xmlString.Contains("/>") && !xmlString.Contains("</" + xmlName + ">"))
                xmlString = xmlString.Substring(0, xmlString.IndexOf(">") - 1) + " />";
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode node;
            try
            {
                // no checks needed; catch will simply return empty if
                // anything goes wrong or if the property is not found.
                xmlDocument.InnerXml = xmlString;
                node = xmlDocument.SelectSingleNode(xmlName);
                return node.Attributes[property].Value;
            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary>
        ///     Removes all special characters (including spaces) from a string.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <returns>The new string.</returns>
        public static String removeSpecialCharacters(String value)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in value)
            {
                if (char.IsLetter(c))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().ToLowerInvariant().Trim();
        }

        /// <summary>
        ///     Scans the end of a string to find a weight value, and returns the full weight string it found.
        /// </summary>
        /// <param name="input">The string to search</param>
        /// <param name="forceSign">Only accpet the score if there is a sign character present.</param>
        /// <returns>The full weight string that is found</returns>
        public static String findWeightInString(String input, Boolean forceSign)
        {
            // Abort immediately if string is empty or does not end with numeric value
            if (input == null || input.Equals(String.Empty) || !Utility.isNumeric(input[input.Length - 1]))
                return String.Empty;

            // Set start index to end of string
            int lastProcessed=input.Length - 1;
            String weight = String.Empty;

            // scans score text from end of line
            while (lastProcessed >= 0 && Utility.isNumeric(input[lastProcessed]))
            {
                weight = input[lastProcessed] + weight;
                lastProcessed--;
            }
            // adds leading sign to score
            if (
                !weight.Equals(String.Empty) && (lastProcessed >= 0)
                && (input[lastProcessed] == '-' || input[lastProcessed] == '+')
               )
            {
                weight = input[lastProcessed] + weight;
                lastProcessed--;
            }
            else if (forceSign)
                weight = String.Empty;

            // makes sure a single number on a line is not seen as score;
            // it MUST have a leading space or be the only thing on the line.
            if (lastProcessed >= 0 && input[lastProcessed] != ' ')
            {
                weight = String.Empty;
            }

            // If the score is not 0 and no sign is found in it, it is not seen as score.
            if (!weight.Equals(String.Empty) && Convert.ToInt16(weight) != 0 && weight[0] != '+' && weight[0] != '-')
            {
                weight = String.Empty;
            }
            return weight;
        }

        public static String trimWeightFromString(String input, Boolean forceSign)
        {
            String weight = findWeightInString(input, forceSign);
            if (weight.Equals(String.Empty))
                return input;
            else
                return input.Substring(input.Length - weight.Length).TrimEnd();
        }

        /// <summary>
        ///     Checks if a string is a truncated version of another string ending on the given truncation string.
        ///     Note that this returns False if the strings are equal and the truncate string is empty.
        /// </summary>
        /// <param name="toMatch">The string to evaluate</param>
        /// <param name="original">The original non-truncated string</param>
        /// <param name="truncateString">the suffix added at the end of toMatch after the cutoff</param>
        /// <returns></returns>
        public static Boolean isTruncatedVersionOf(String toMatch, String original, String truncateString)
        {
            if (truncateString == null)
                truncateString = String.Empty;
            int trunlen = toMatch.Length - truncateString.Length;
            return toMatch.EndsWith(truncateString)
                    && original.Length > trunlen
                    && original.StartsWith(toMatch.Substring(0, trunlen));
        }


        #endregion

        #region Image manipulations

        public static ImageAttributes getDarkGreyMask()
        { 
            // excluded image gets converted to greyscale
            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][]
                {
                    new float[] { .30f, .30f, .30f, 0, 0}, // greyscale red factors
                    new float[] { .59f, .59f, .59f, 0, 0}, // greyscale green factors
                    new float[] { .11f, .11f, .11f, 0, 0}, // greyscale blue factors
                    new float[] {   0,    0,    0,  1, 0}, // no change in transparency
                    new float[] {-.30f,-.30f,-.30f, 0, 1}  // darken to -30%
                });
            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);
            return attributes;
        }

        #endregion

        #region Hash calculation

        /// <summary>
        ///     Calculates and returns the MD5 hash value of a file.
        /// </summary>
        /// <param name="relativeFileName">The relative path.</param>
        /// <returns>The MD5 hash.</returns>
        public static String calculateMD5ForFile(String path)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);

                file.Close();

                // convert to hexadecimal string
                foreach (byte b in retVal)
                    sb.Append(b.ToString("x2"));
            }
            catch (Exception ex)
            {
                if (DomainController.hasInstance())
                {
                    List<String[]> pairs = new List<String[]>() { new String[] { "File", path } };
                    DomainController.Instance().processError(ex,
                                                            !DomainController.Instance().isSynchronization,
                                                            ex.Message,
                                                            false,
                                                            pairs);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Calculates and returns the MD5 hash value of a file.
        /// </summary>
        /// <param name="relativeFileName">The relative path.</param>
        /// <returns>The MD5 hash.</returns>
        public static String calculateMD5ForPDFFile(String path)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                FileStream file = new PDFFilterFileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);

                file.Close();

                // convert to hexadecimal string
                foreach (byte b in retVal)
                    sb.Append(b.ToString("x2"));
            }
            catch (Exception ex)
            {
                if (DomainController.hasInstance())
                {
                    List<String[]> pairs = new List<String[]>() { new String[] { "File", path } };
                    DomainController.Instance().processError(ex,
                                                            !DomainController.Instance().isSynchronization,
                                                            ex.Message,
                                                            false,
                                                            pairs);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Calculates and returns the MD5 hash value of a string.
        /// </summary>
        /// <param name="data">The data to calculate the MD5 from.</param>
        /// <returns>The MD5 hash.</returns>
        public static String calculateMD5ForString(String data)
        {
            StringBuilder sb = new StringBuilder();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(Encoding.Default.GetBytes(data));

            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        #endregion

        #region Registry (As of 1.08)

        /// <summary>
        ///     Changes a registry setting.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item to change.</param>
        /// <param name="value">The value for the item.</param>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 18/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public static void setRegistryKey(String key, String item, int value)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(key, true);
            regKey.SetValue(item, value, RegistryValueKind.DWord);
            regKey.Close();
        }

        /// <summary>
        ///     Gets the value of a certain item in a registry key.
        /// </summary>
        /// <param name="key">The registry key.</param>
        /// <param name="item">The item.</param>
        /// <returns>An object representing the value of the item.</returns>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 18/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public static object getRegistryKey(String key, String item)
        {
            object retValue = null;
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(key);
            retValue = regKey.GetValue(item);
            regKey.Close();
            return retValue;
        }

        #endregion

        #region randomizing
        
        public static Int64 NextInt64(this Random rnd)
        {
            var buffer = new byte[sizeof(Int64)];
            rnd.NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        #endregion

    }
}
