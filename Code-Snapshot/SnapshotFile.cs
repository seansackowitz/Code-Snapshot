using System.IO;
using System.Collections.Generic;

namespace Code_Snapshot
{
    public class SnapshotFile : Tracked
    {
        public List<Snapshot> Snapshots = new List<Snapshot>();
        public FileType Type;

        public SnapshotFile(string location, Tracked parent)
        {
            Location = location;
            this.parent = parent;
            ItemName = Path.GetFileName(location);
            Header = ToString();
            Type = ParseType(Path.GetExtension(location));
        }

        public void TakeSnapshot()
        {
            Snapshots.Add(new Snapshot(this));
        }

        public void RestoreSnapshot(Snapshot snap)
        {
            File.WriteAllText(Location, snap.Value);
        }

        public void DeleteSnapshot(Snapshot snap)
        {
            Snapshots.Remove(snap);
        }

        public FileType ParseType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".c":
                    return FileType.CPP;
                case ".h":
                    return FileType.CPP;
                case ".cc":
                    return FileType.CPP;
                case ".cpp":
                    return FileType.CPP;
                case ".hpp":
                    return FileType.CPP;
                case ".cs":
                    return FileType.CSHARP;
                case ".css":
                    return FileType.CSS;
                case ".htm":
                    return FileType.HTML;
                case ".html":
                    return FileType.HTML;
                case ".java":
                    return FileType.JAVA;
                case ".js":
                    return FileType.JS;
                case ".php":
                    return FileType.PHP;
                case ".xml":
                    return FileType.XML;
                case ".xsl":
                    return FileType.XML;
                case ".xslt":
                    return FileType.XML;
                case ".xsd":
                    return FileType.XML;
                case ".manifest":
                    return FileType.XML;
                case ".config":
                    return FileType.XML;
                case ".addin":
                    return FileType.XML;
                case ".xshd":
                    return FileType.XML;
                case ".wxs":
                    return FileType.XML;
                case ".wxi":
                    return FileType.XML;
                case ".wxl":
                    return FileType.XML;
                case ".proj":
                    return FileType.XML;
                case ".csproj":
                    return FileType.XML;
                case ".vbproj":
                    return FileType.XML;
                case ".ilproj":
                    return FileType.XML;
                case ".booproj":
                    return FileType.XML;
                case ".build":
                    return FileType.XML;
                case ".xfrm":
                    return FileType.XML;
                case ".targets":
                    return FileType.XML;
                case ".xaml":
                    return FileType.XML;
                case ".xpt":
                    return FileType.XML;
                case ".xft":
                    return FileType.XML;
                case ".map":
                    return FileType.XML;
                case ".wsdl":
                    return FileType.XML;
                case ".disco":
                    return FileType.XML;
                case ".ps1xml":
                    return FileType.XML;
                case ".nuspec":
                    return FileType.XML;
                default:
                    return FileType.Other;
            }
        }
    }
}