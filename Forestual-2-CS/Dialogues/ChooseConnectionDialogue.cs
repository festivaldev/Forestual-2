using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forestual2CS.Dialogues
{
    public partial class ChooseConnectionDialogue : Form
    {
        public int SelectedIndex {
            get {
                if (lbxServers != null) {
                    return lbxServers.SelectedIndex;
                }
                return -1;
            }
            set { }
        }

        public ChooseConnectionDialogue() {
            InitializeComponent();

            lbxServers.SelectedIndexChanged += OnLbxServersSelectedIndexChanged;
        }

        private void OnLbxServersSelectedIndexChanged(object sender, EventArgs e) {
            btnConnect.Enabled = lbxServers.SelectedItems.Count > 0;
        }

        public DialogResult ShowDialog(IEnumerable<string> connections) {
            foreach (var Connection in connections) {
                lbxServers.Items.Add(Connection);
            }
            return base.ShowDialog();
        }
    }
}
