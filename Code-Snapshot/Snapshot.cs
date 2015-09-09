using System;
using System.IO;

namespace Code_Snapshot
{
    public class Snapshot
    {
        public SnapshotFile SnapshotFile;
        public string SnapshotName;
        public string Value;
        
        public Snapshot(SnapshotFile file)
        {
            SnapshotFile = file;
            SnapshotName = SnapshotFile.ItemName + " " + DateTime.UtcNow.ToString();
            Value = File.ReadAllText(file.Location);
        }

        public override string ToString()
        {
            return SnapshotName;
        }
    }
}