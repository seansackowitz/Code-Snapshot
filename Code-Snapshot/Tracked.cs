using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Code_Snapshot
{
    public class Tracked : TreeViewItem
    {
        public Tracked parent;
        public string ItemName;
        public string Location;

        /// <summary>
        /// Removes the specified Tracked object from the list of items associated with this Tracked object
        /// </summary>
        /// <param name="item">The item to remove</param>
        public void Remove(Tracked item)
        {
            Items.Remove(item);
        }

        /// <summary>
        /// Used to display the appropriate string in the tree view
        /// </summary>
        public override string ToString()
        {
            return ItemName;
        }
    }
}
