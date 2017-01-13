using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using F2Core;

namespace Forestual2CS.Dialogues
{
    public partial class LuvaDialog : Form
    {
        public string LuvaValue { get; set; }
        public Severity Severity { get; set; }

        public LuvaDialog() {
            InitializeComponent();
        }

        public new DialogResult ShowDialog() {
            lblLuvaValue.Text = LuvaValue;
            lblSeverity.Text = Severity.Description;
            lblSeverity.ForeColor = ColorTranslator.FromHtml(Severity.Color);
            return base.ShowDialog();
        }
    }
}
