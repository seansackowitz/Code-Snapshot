using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Code_Snapshot
{
    public class SnapshotFolder : Tracked
    {
        public SnapshotFolder(string location, Tracked parent)
        {
            Location = location;
            this.parent = parent;
            ItemName = Path.GetFileName(location);

            foreach (string s in GetFolders(location, Properties.Settings.Default.FolderExcludes, SearchOption.TopDirectoryOnly))
            {
                if (Properties.Settings.Default.IgnoreHiddenFolders && (File.GetAttributes(s) & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    continue;
                }
                else
                    Items.Add(new SnapshotFolder(s, this));
            }

            foreach (string s in GetFiles(location, Properties.Settings.Default.FileExcludes, SearchOption.TopDirectoryOnly))
            {
                if (Properties.Settings.Default.IgnoreHiddenFolders && (File.GetAttributes(s) & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    continue;
                }
                else
                    Items.Add(new SnapshotFile(s, this));
            }

            Header = ToString();
        }

        public void Refresh()
        {
            foreach (string s in GetFolders(Location, Properties.Settings.Default.FolderExcludes, SearchOption.TopDirectoryOnly))
            {
                if (Properties.Settings.Default.IgnoreHiddenFolders && (File.GetAttributes(s) & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    continue;
                }
                else
                {
                    bool matched = false;
                    foreach (Tracked i in Items)
                    {
                        if (i.Location == s)
                        {
                            matched = true;
                            break;
                        }
                    }
                    if (!matched)
                        Items.Add(new SnapshotFolder(s, this));
                }
            }

            foreach (string s in GetFiles(Location, Properties.Settings.Default.FileExcludes, SearchOption.TopDirectoryOnly))
            {
                if (Properties.Settings.Default.IgnoreHiddenFolders && (File.GetAttributes(s) & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    continue;
                }
                else
                {
                    bool matched = false;
                    foreach (Tracked i in Items)
                    {
                        if (i.Location == s)
                        {
                            matched = true;
                            break;
                        }
                    }
                    if (!matched)
                        Items.Add(new SnapshotFile(s, this));
                }
            }

            foreach (Tracked folder in Items)
            {
                if (folder.GetType() == typeof(SnapshotFolder))
                    ((SnapshotFolder)folder).Refresh();
            }
        }

        private static string[] GetFiles(string sourceFolder, string filters, SearchOption searchOption)
        {
            string[] allFiles = Directory.GetFiles(sourceFolder);
            string[] unWantedFiles;
            if (filters != null && filters != "")
            {
                unWantedFiles = filters.Split('|').SelectMany(filter => Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
                List<string> value = new List<string>();
                foreach (string s in allFiles)
                {
                    bool wanted = true;
                    foreach (string unwanted in unWantedFiles)
                    {
                        if (s == unwanted)
                        {
                            wanted = false;
                            break;
                        }
                    }
                    if (wanted)
                        value.Add(s);
                }

                return value.ToArray();
            }
            else
                return allFiles;
        }

        private static string[] GetFolders(string sourceFolder, string filters, SearchOption searchOption)
        {
            string[] allDirectories = Directory.GetDirectories(sourceFolder);
            string[] unWantedDirectories;
            if (filters != null && filters != "")
            {
                unWantedDirectories = filters.Split('|').SelectMany(filter => Directory.GetDirectories(sourceFolder, filter, searchOption)).ToArray();
                List<string> value = new List<string>();
                foreach (string s in allDirectories)
                {
                    bool wanted = true;
                    foreach (string unwanted in unWantedDirectories)
                    {
                        if (s == unwanted)
                        {
                            wanted = false;
                            break;
                        }
                    }
                    if (wanted)
                        value.Add(s);
                }

                return value.ToArray();
            }
            else
                return allDirectories;
        }
    }
}
