using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Forestual2CS.Dialogues;

namespace Forestual2CS
{
    public partial class DoubleBufferedPanel : Panel
    {
        public MainWindow.AccountState State { get; set; }

        public DoubleBufferedPanel() : base() {
            DoubleBuffered = true;
        }
    }
}
