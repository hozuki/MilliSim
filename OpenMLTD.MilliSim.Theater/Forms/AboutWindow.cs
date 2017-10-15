using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OpenMLTD.MilliSim.Theater.Interop;

namespace OpenMLTD.MilliSim.Theater.Forms {
    public partial class AboutWindow : Form {

        public AboutWindow() {
            InitializeComponent();
            RegisterEventHandlers();
        }

        ~AboutWindow() {
            UnregisterEventHandlers();
        }

        private void UnregisterEventHandlers() {
            Load -= AboutWindow_Load;
        }

        private void RegisterEventHandlers() {
            Load += AboutWindow_Load;
        }

        private void AboutWindow_Load(object sender, EventArgs e) {
            var codeName = ApplicationHelper.GetCodeName();
            var version = Application.ProductVersion;

            label1.Text = $"MilliSim (\"{codeName}\")";
            label2.Text = version;

            var lv = pluginsListView;
            lv.View = View.Details;
            lv.Columns.Add("ID");
            lv.Columns.Add("Name");
            lv.Columns.Add("Author");
            lv.Columns.Add("Version");
            lv.Columns.Add("Description");

            lv.FullRowSelect = true;
            lv.MultiSelect = false;
            lv.LabelEdit = false;
            lv.ShowGroups = true;

            var categories = new Dictionary<string, ListViewGroup>();
            foreach (var plugin in Program.PluginManager.LoadedPlugins) {
                ListViewGroup cat;
                if (categories.ContainsKey(plugin.PluginCategory)) {
                    cat = categories[plugin.PluginCategory];
                } else {
                    cat = new ListViewGroup(plugin.PluginCategory);
                    categories[plugin.PluginCategory] = cat;
                    lv.Groups.Add(cat);
                }

                var listViewItem = new ListViewItem(cat);
                listViewItem.Text = plugin.PluginID;
                listViewItem.SubItems.AddRange(new[] {
                    plugin.PluginName, plugin.PluginAuthor, plugin.PluginVersion.ToString(), plugin.PluginDescription
                });
                lv.Items.Add(listViewItem);
            }

            var osVersion = Environment.OSVersion;
            if (osVersion.Platform == PlatformID.Win32NT) {
                // https://www.codeproject.com/Articles/31276/Add-Group-Collapse-Behavior-on-a-Listview-Control
                try {
                    for (var i = 0; i <= lv.Groups.Count; ++i) {
                        var lvg = new NativeStructures.LVGROUP();
                        lvg.cbSize = Marshal.SizeOf(lvg);
                        lvg.state = NativeConstants.LVGS_COLLAPSIBLE | NativeConstants.LVGS_COLLAPSED;
                        lvg.mask = NativeConstants.LVGF_STATE;
                        lvg.iGroupId = i;
                        NativeMethods.SendMessage(lv.Handle, NativeConstants.LVM_SETGROUPINFO, (IntPtr)i, ref lvg);
                    }
                } catch (Exception ex) {
                    Debug.Print(ex.Message);
                }
            }
        }

    }
}
