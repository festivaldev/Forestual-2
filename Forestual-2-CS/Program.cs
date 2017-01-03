using System;
using System.Windows.Forms;
using Forestual2CS.Dialogues;

namespace Forestual2CS
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main() {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginDialogue());
        }
    }
}
