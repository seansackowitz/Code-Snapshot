using System.Collections.Generic;

namespace Code_Snapshot
{
    public static class Project
    {
        public static List<Tracked> Tracked = new List<Tracked>();

        public static void LoadProject(string Location)
        {
            Tracked.Add(new SnapshotFolder(Location, null));
        }

        public static void LoadFile(string Location)
        {
            Tracked.Add(new SnapshotFile(Location, null));
        }
    }
}