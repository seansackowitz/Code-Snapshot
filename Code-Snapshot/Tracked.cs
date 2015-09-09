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

        public override string ToString()
        {
            return ItemName;
        }

        public void Remove(Tracked item)
        {
            Items.Remove(item);
        }
    }
}
