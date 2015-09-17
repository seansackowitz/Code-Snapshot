using System.Collections.Generic;

namespace Code_Snapshot
{
    public static class Project
    {
        /// <summary>
        /// The List of all top level items
        /// </summary>
        public static List<Tracked> Tracked = new List<Tracked>();

        /// <summary>
        /// Adds a new snapshot folder to the top level of the hierarchy
        /// </summary>
        /// <param name="Location">The system path of the file</param>
        public static void LoadProject(string Location)
        {
            Tracked.Add(new SnapshotFolder(Location, null));
        }

        /// <summary>
        /// Adds a new snapshot file to the top level of the hierarchy
        /// </summary>
        /// <param name="Location">The system path of the file</param>
        public static void LoadFile(string Location)
        {
            Tracked.Add(new SnapshotFile(Location, null));
        }
    }
}