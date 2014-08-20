﻿// BukgetPluginInstaller.cs in bukkitgui2/bukkitgui2
// Created 2014/07/13
// Last edited at 2014/08/17 11:19
// ©Bertware, visit http://bertware.net

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Net.Bertware.Bukkitgui2.AddOn.Plugins.InstalledPlugins;
using Net.Bertware.Bukkitgui2.Core.FileLocation;
using Net.Bertware.Bukkitgui2.Core.Logging;
using Net.Bertware.Bukkitgui2.Core.Translation;
using Net.Bertware.Bukkitgui2.Core.Util;
using Net.Bertware.Bukkitgui2.Core.Util.Web;

namespace Net.Bertware.Bukkitgui2.AddOn.Plugins.Bukget.api3
{
    internal class BukgetPluginInstaller
    {
        /// <summary>
        ///     Install a plugin, supports .jar and .zip files
        /// </summary>
        /// <param name="version">Version to install</param>
        /// <param name="targetlocation">Target location, plugins/name by default</param>
        /// <param name="updatelist">Update the list of installed plugins</param>
        /// <param name="showUi">Allow pop-up dialogs</param>
        /// <remarks></remarks>
        public static Boolean Install(BukgetPluginVersion version, string targetlocation = "", bool updatelist = true,
            bool showUi = true)
        {
            if (string.IsNullOrEmpty(targetlocation) && version.Filename != null)
            {
                targetlocation = Fl.Location(RequestFile.Plugindir) + "/" + version.Filename;
            }

            if (version == null || version.Filename == null) return false;

            if (version.Filename.EndsWith(".jar"))
            {
                return InstallJar(version, targetlocation, updatelist, showUi);
            }
            if (version.Filename.EndsWith(".zip"))
            {
                return InstallZip(version, updatelist, showUi);
            }
            MessageBox.Show(
                Translator.Tr("The file you chose to download is not supported yet.") + Constants.vbCrLf +
                Translator.Tr("At this moment only .jar and .zip files are supported."),
                Translator.Tr("Not supported"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        /// <summary>
        ///     Install a jarfile
        /// </summary>
        /// <param name="version">Version to install</param>
        /// <param name="targetlocation">Target location, plugins/name by default</param>
        /// <param name="updatelist">Update the list of installed plugins</param>
        /// <param name="showUi">Allow pop-up dialogs</param>
        /// <remarks></remarks>
        private static Boolean InstallJar(BukgetPluginVersion version, string targetlocation = "",
            bool updatelist = true,
            bool showUi = true)
        {
            if (showUi)
            {
                if (
                    MessageBox.Show(
                        Translator.Tr("You are about to install") + " " + version.Filename.Replace(".jar", "") + " (" +
                        version.VersionNumber + ")" + Constants.vbCrLf + Translator.Tr("Do you wish to continue?"),
                        Translator.Tr("Continue?"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return false;
            }

            Logger.Log(LogLevel.Info, "BukGetAPI", "Installing plugin:" + version.Filename + ", packed as jar file");

            if (string.IsNullOrEmpty(targetlocation))
                targetlocation = Fl.Location(RequestFile.Plugindir) + "/" + version.Filename;

            FileDownloader fdd = new FileDownloader();

            fdd.AddFile(version.DownloadLink, targetlocation);
            fdd.ShowDialog();

            InstalledPluginManager.ReloadInstalledPluginFile(targetlocation);
            if (showUi)
            {
                MessageBox.Show(
                    version.Filename.Replace(".jar", "") + " (" + version.VersionNumber + ") " +
                    Translator.Tr("was installed succesfully"), Translator.Tr("Plugin Installed"), MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            if (updatelist)
                InstalledPluginManager.RefreshAllInstalledPluginsAsync();
            //refresh installed list
            return true;
        }

        /// <summary>
        ///     Install plguins from a zip file
        /// </summary>
        /// <param name="version">Version to install</param>
        /// <param name="updatelist">Update the list of installed plugins</param>
        /// <param name="showUi">Allow pop-up dialogs</param>
        /// <remarks></remarks>
        private static Boolean InstallZip(BukgetPluginVersion version,
            bool updatelist = true,
            bool showUi = true)
        {
            if (showUi)
            {
                if (
                    MessageBox.Show(
                        Translator.Tr("You are about to install") + " " + version.Filename.Replace(".zip", "") + " (" +
                        version.VersionNumber + ")" + Constants.vbCrLf + Translator.Tr("Do you wish to continue?"),
                        Translator.Tr("Continue?"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return false;
            }

            Logger.Log(LogLevel.Info, "BukGetAPI", "Installing plugin:" + version.Filename + ", packed as zip file");

            string zipfile = Fl.SafeLocation(RequestFile.Temp) + "install.zip";
            string extraction = Fl.SafeLocation(RequestFile.Temp) + "/install/";


            FileDownloader fdd = new FileDownloader();
            fdd.AddFile(version.DownloadLink, zipfile);
            fdd.ShowDialog();

            Compression.Decompress(extraction, zipfile);

            // ******************************
            // At this point, the zip file is extracted to a temporary location
            // Now only the needed files should be moved


            Boolean hasFileBeenMoved = false;

            Boolean hasFolderBeenMoved = false;

            //file is decompressed, now search the needed files
            DirectoryInfo extracteDirectoryInfo = new DirectoryInfo(extraction);

            List<string> extractedFileNamesList = new List<string>();
            foreach (FileInfo fileInfo in extracteDirectoryInfo.GetFiles())
            {
                if (fileInfo.Extension == ".jar")
                {
                    File.Copy(fileInfo.FullName,
                        Fl.Location(RequestFile.Plugindir) + "/" + fileInfo.Name, true);
                    extractedFileNamesList.Add(fileInfo.Name);
                    hasFileBeenMoved = true;
                    Logger.Log(LogLevel.Info, "BukGetAPI", "Jar file found in .zip (L1), copied:" + fileInfo.Name);
                }
            }

            // now we check if there's a folder with the same name as the plugin. This folder should also be moved to the /plugins folder
            foreach (DirectoryInfo directoryInZipInfo in extracteDirectoryInfo.GetDirectories())
            {
                Boolean folderShouldBeMoved = false;

                foreach (string f in extractedFileNamesList)
                {
                    if (f.Contains(directoryInZipInfo.Name))
                    {
                        folderShouldBeMoved = true;
                        Logger.Log(LogLevel.Info, "BukgetAPI",
                            "Config/Info folder found in .zip, marked directory for copy:" + directoryInZipInfo.Name);
                    }
                }
                if (!folderShouldBeMoved)
                {
                    foreach (FileInfo fileInfo in directoryInZipInfo.GetFiles())
                    {
                        if (fileInfo.Extension == ".txt" | fileInfo.Extension == ".yml" | fileInfo.Extension == ".cfg" |
                            fileInfo.Extension == ".csv" | fileInfo.Extension == ".js")
                        {
                            folderShouldBeMoved = true;
                            Logger.Log(LogLevel.Info, "BukgetAPI",
                                "Config/Info file found in .zip, marked directory for copy:" + fileInfo.Name);
                        }
                    }
                }
                if (folderShouldBeMoved)
                {
                    Directory.Move(directoryInZipInfo.FullName,
                        Fl.Location(RequestFile.Plugindir) + "/" + directoryInZipInfo.Name);
                    hasFileBeenMoved = false;
                    hasFolderBeenMoved = true;
                }

                // If we didn't copy a file yet, check other folders for jar files
                //L2
                if (!hasFileBeenMoved)
                {
                    foreach (FileInfo fileInfo in directoryInZipInfo.GetFiles())
                    {
                        if (fileInfo.Extension != ".jar") continue;
                        fileInfo.MoveTo(Fl.Location(RequestFile.Plugindir) + "/" + fileInfo.Name);
                        hasFileBeenMoved = true;
                        Logger.Log(LogLevel.Info, "BukgetAPI", "Jar file found in .zip (L2), copied:" + fileInfo.Name);
                    }
                }

                if (hasFolderBeenMoved) continue;
                // If we didn't find a config folder yet, check deeper. config folders are not required
                foreach (DirectoryInfo dir2 in directoryInZipInfo.GetDirectories())
                {
                    bool copy2 = false;
                    foreach (string f in extractedFileNamesList)
                    {
                        if (!f.Contains(dir2.Name)) continue;
                        copy2 = true;
                        Logger.Log(LogLevel.Info, "BukgetAPI",
                            "Config/Info folder found in .zip, marked directory for copy:" + dir2.Name);
                    }
                    foreach (FileInfo fileInfo in dir2.GetFiles())
                    {
                        if (fileInfo.Extension == ".txt" | fileInfo.Extension == ".yml" | fileInfo.Extension == ".cfg" |
                            fileInfo.Extension == ".csv" | fileInfo.Extension == ".js")
                        {
                            copy2 = true;
                            Logger.Log(LogLevel.Info, "BukgetAPI",
                                "Config/Info file found in .zip, marked directory for copy:" + fileInfo.Name);
                        }
                    }
                    if (copy2)
                    {
                        Directory.Move(directoryInZipInfo.FullName,
                            Fl.Location(RequestFile.Plugindir) + "/" + dir2.Name);
                        hasFileBeenMoved = false;
                        hasFolderBeenMoved = true;
                    }
                }

                // end of second level searching
            }


            Logger.Log(LogLevel.Info, "BukgetAPI",
                "Finished plugin installation: Success?" + (hasFileBeenMoved || hasFolderBeenMoved));

            if (updatelist)
                InstalledPluginManager.RefreshAllInstalledPluginsAsync();
            //refresh installed list
            return (hasFileBeenMoved || hasFolderBeenMoved);
        }
    }
}