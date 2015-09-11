using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Code_Snapshot
{
    public partial class MainWindow
    {
        public static FileType CurrentHighlighting = FileType.Other;
        SnapshotFile currentFile;
        IEnumerable<FileType> FileTypes;
        bool SyntaxInit = false;
        MenuItem deleteFromTreeView, refreshFolderTreeView;

        public MainWindow()
        {
            InitializeComponent();
            FileTypes = Enum.GetValues(typeof(FileType)).Cast<FileType>();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            fileTypeBox.ItemsSource = FileTypes;

            treeView.ContextMenu = new ContextMenu();
            deleteFromTreeView = new MenuItem();
            deleteFromTreeView.Header = "Delete";
            deleteFromTreeView.Click += deleteFromTreeView_Click;
            refreshFolderTreeView = new MenuItem();
            refreshFolderTreeView.Header = "Refresh";
            refreshFolderTreeView.Click += refreshFolderTreeView_Click;
        }

        private void loadProject_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog dlg = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();
            dlg.Title = "Select Project Folder";
            dlg.IsFolderPicker = true;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                treeView.ItemsSource = null;
                Project.LoadProject(dlg.FileName);
                treeView.ItemsSource = Project.Tracked;
            }
        }

        private void loadFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog dlg = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();
            dlg.Title = "Select File";
            dlg.IsFolderPicker = false;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                treeView.ItemsSource = null;
                Project.LoadFile(dlg.FileName);
                treeView.ItemsSource = Project.Tracked;
            }
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView.SelectedItem != null)
            {
                if (treeView.SelectedItem.GetType() == typeof(SnapshotFile))
                {
                    currentFile = (SnapshotFile)treeView.SelectedItem;
                    comboBox.ItemsSource = null;
                    comboBox.ItemsSource = currentFile.Snapshots;
                    fileLocation.Text = currentFile.Location;
                    if (comboBox.Items.Count > 0)
                        Select((Snapshot)comboBox.Items[0]);
                    else
                        DeSelect();
                }
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.SelectedItem != null)
                Select((Snapshot)comboBox.SelectedItem);
        }

        public void Select(Snapshot snap)
        {
            comboBox.SelectedItem = snap;
            fileName.Text = snap.SnapshotName;
            fileText.Text = snap.Value;
            fileTypeBox.SelectedItem = snap.SnapshotFile.Type;
            LoadSyntaxHighlighting();
        }

        public void DeSelect()
        {
            fileName.Text = "";
            fileText.Text = "";
        }

        private void takeSnapshot_Click(object sender, RoutedEventArgs e)
        {
            if (currentFile != null)
            {
                currentFile.TakeSnapshot();
                comboBox.ItemsSource = null;
                comboBox.ItemsSource = currentFile.Snapshots;
                if (comboBox.Items.Count > 0)
                    Select((Snapshot)comboBox.Items[0]);
                else
                    DeSelect();
            }
        }

        private void restoreSnapshot_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedItem != null)
            {
                currentFile.RestoreSnapshot((Snapshot)comboBox.SelectedItem);
            }
        }

        private void deleteSnapshot_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedItem != null)
            {
                currentFile.DeleteSnapshot((Snapshot)comboBox.SelectedItem);
                comboBox.ItemsSource = null;
                comboBox.ItemsSource = currentFile.Snapshots;
                if (comboBox.Items.Count > 0)
                    Select((Snapshot)comboBox.Items[comboBox.Items.Count - 1]);
                else
                    DeSelect();
            }
        }

        #region GUI Events
        private void button_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)e.Source).Foreground = (SolidColorBrush)this.FindResource("toolBarButtonMouseHover");
        }

        private void button_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)e.Source).Foreground = (SolidColorBrush)this.FindResource("toolBarButtonMouseLeave");
        }

        private void titleBarButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)e.Source).Foreground = (SolidColorBrush)this.FindResource("titleBarButtonMouseHover");
        }

        private void titleBarButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)e.Source).Foreground = (SolidColorBrush)this.FindResource("titleBarButtonMouseLeave");
        }
        #endregion

        private void fileName_KeyUp(object sender, KeyEventArgs e)
        {
            if (currentFile != null)
            {
                ((Snapshot)comboBox.SelectedItem).SnapshotName = fileName.Text;
            }
        }

        private void fileName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedItem != null)
            {
                Snapshot s = (Snapshot)comboBox.SelectedItem;
                comboBox.ItemsSource = null;
                comboBox.ItemsSource = currentFile.Snapshots;
                comboBox.SelectedItem = s;
            }
        }

        private void openSettings_Click(object sender, RoutedEventArgs e)
        {
            settingsFlyout.IsOpen = !settingsFlyout.IsOpen;
            blockingPanel.Visibility = settingsFlyout.IsOpen ? Visibility.Visible : Visibility.Collapsed;
            if (settingsFlyout.IsOpen)
            {
                txtFolderExcludes.Text = Properties.Settings.Default.FolderExcludes;
                txtFileExcludes.Text = Properties.Settings.Default.FileExcludes;
                chkHiddenFolders.IsChecked = Properties.Settings.Default.IgnoreHiddenFolders;
                chkHiddenFiles.IsChecked = Properties.Settings.Default.IgnoreHiddenFiles;
            }
        }

        private void settingsAccept_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.FolderExcludes = txtFolderExcludes.Text;
            Properties.Settings.Default.FileExcludes = txtFileExcludes.Text;
            Properties.Settings.Default.IgnoreHiddenFolders = chkHiddenFolders.IsChecked.Value;
            Properties.Settings.Default.IgnoreHiddenFiles = chkHiddenFiles.IsChecked.Value;
            Properties.Settings.Default.Save();
            settingsFlyout.IsOpen = false;
            blockingPanel.Visibility = Visibility.Collapsed;
        }

        private void blockingPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            settingsFlyout.IsOpen = false;
            blockingPanel.Visibility = Visibility.Collapsed;
        }

        private void deleteFromTreeView_Click(object sender, RoutedEventArgs e)
        {
            if (treeView.SelectedItem != null)
            {
                if (((Tracked)treeView.SelectedItem).parent != null)
                    ((Tracked)treeView.SelectedItem).parent.Remove((Tracked)treeView.SelectedItem);
                else
                    Project.Tracked.Remove((Tracked)treeView.SelectedItem);

                treeView.ItemsSource = null;
                treeView.ItemsSource = Project.Tracked;
            }
        }

        private void refreshFolderTreeView_Click(object sender, RoutedEventArgs e)
        {
            if (treeView.SelectedItem != null)
            {
                ((SnapshotFolder)treeView.SelectedItem).Refresh();
            }
        }

        public void LoadSyntaxHighlighting()
        {
            if (CurrentHighlighting != currentFile.Type || !SyntaxInit)
            {
                string fileName = "Syntax\\";
                switch (currentFile.Type)
                {
                    case FileType.CPP:
                        fileName += "CPP.xshd";
                        break;
                    case FileType.CSHARP:
                        fileName += "CSHARP.xshd";
                        break;
                    case FileType.CSS:
                        fileName += "CSS.xshd";
                        break;
                    case FileType.HTML:
                        fileName += "HTML.xshd";
                        break;
                    case FileType.JAVA:
                        fileName += "JAVA.xshd";
                        break;
                    case FileType.JS:
                        fileName += "JS.xshd";
                        break;
                    case FileType.PHP:
                        fileName += "PHP.xshd";
                        break;
                    case FileType.XML:
                        fileName += "XML.xshd";
                        break;
                    default:
                        fileName += "OTHER.xshd";
                        break;
                }
                
                using (XmlTextReader reader = new XmlTextReader(AppDomain.CurrentDomain.BaseDirectory + fileName))
                {
                    fileText.SyntaxHighlighting = null;
                    if (currentFile.Type != FileType.Other)
                    fileText.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
                SyntaxInit = true;
            }
        }

        private void fileTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentFile != null)
            {
                currentFile.Type = (FileType)fileTypeBox.SelectedItem;
                LoadSyntaxHighlighting();
            }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void treeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        private void treeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                if (treeViewItem.GetType() == typeof(SnapshotFile))
                {
                    treeView.ContextMenu.Items.Clear();
                    treeView.ContextMenu.Items.Add(deleteFromTreeView);
                }
                else
                {
                    treeView.ContextMenu.Items.Clear();
                    treeView.ContextMenu.Items.Add(refreshFolderTreeView);
                    treeView.ContextMenu.Items.Add(deleteFromTreeView);
                }
            }
        }
    }
}