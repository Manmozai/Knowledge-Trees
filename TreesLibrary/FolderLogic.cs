﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TreesLibrary
{
    public static class FolderLogic
    {
        public static string GetFullTreePath(string baseDirectory, string treeName)
        {
            return $@"{ baseDirectory }\{ treeName }";
        }

        public static string GetFullLeafName(string currentLeafName) // Adds in .docx extension.
        {
            string output = String.Concat(currentLeafName, ".docx");

            return output;
        }

        public static string[] GetAllTreePaths(string baseDirectory)
        {
            return Directory.GetDirectories(baseDirectory);
        }

        public static string[] GetAllTreeNames(string baseDirectory)
        {
            return Directory.GetDirectories(baseDirectory).Select(Path.GetFileName).ToArray();
        }

        public static string[] GetAllLeafNames(string treePath)
        {
            // Includes meta word files.
            List<string> allNames = Directory.GetFiles(treePath).Select(Path.GetFileName).ToList<string>();

            // Will only include actual files.
            List<string> cleanNames = new List<string>();

            // Excludes meta files (which include "~" in their name. These files exist while the actual file is being modified).
            foreach (string name in allNames)
            {
                if (name.Contains("~") == false)
                {
                    cleanNames.Add(name);
                }
            }

            return cleanNames.ToArray();
        }

        /// <summary>
        /// Returns all leaf names with no .docx extension, so that they can be displayed in the dashboard.
        /// </summary>
        public static string[] GetAllLeafNamesWithNoExtension(string treePath) 
        {
            string[] current = Directory.GetFiles(treePath).Select(Path.GetFileName).ToArray();
            List<string> output = new List<string>();

            for (int i = 0; i < current.Length; i++)
            {
                // Word documents create meta files when they are running,
                // this method excludes these files from our leaves list.
                if (current[i].Contains("~"))
                    continue;

                output.Add(current[i].Substring(0, current[i].Length - 5));
            }

            return output.ToArray();
        }

        public static void CreateNewTreeFolder(string fullTreePath)
        {
            Directory.CreateDirectory(fullTreePath);
        }

        public static void DeleteTree(string treePath)
        {
            Directory.Delete(treePath, true);
        }

        public static void DeleteLeaf(string leafPath)
        {
            File.Delete(leafPath);
        }

        public static void BackupTrees(string currentLocation, string backupLocation, bool copySubDirectories)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo sourceDirectory = new DirectoryInfo(currentLocation);

            if (!sourceDirectory.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: "
                    + currentLocation);
            }

            DirectoryInfo[] sourceDirectories = sourceDirectory.GetDirectories();

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(backupLocation))
            {
                Directory.CreateDirectory(backupLocation);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = sourceDirectory.GetFiles();
            foreach (FileInfo file in files)
            {
                string temporaryPath = Path.Combine(backupLocation, file.Name);
                file.CopyTo(temporaryPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirectories)
            {
                foreach (DirectoryInfo subDirectory in sourceDirectories)
                {
                    string temppath = Path.Combine(backupLocation, subDirectory.Name);
                    BackupTrees(subDirectory.FullName, temppath, copySubDirectories);
                }
            }
        }

        public static string GetThemeStatus(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    string[] result = File.ReadAllLines(path);
                    return result[0];
                }
                catch
                {

                }
            }

            return "green";
        }
    }
}
