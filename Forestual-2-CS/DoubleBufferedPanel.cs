﻿using System.Windows.Forms;
using Forestual2CS.Forms;

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
