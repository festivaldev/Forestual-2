using System.Windows.Forms;

namespace Forestual2CS.Forms
{
    public partial class VersionConflictDialogue : Form
    {
        public VersionConflictDialogue() {
            InitializeComponent();
        }

        public void Initialize(F2Core.Compatibility.Version clientVersion, F2Core.Compatibility.Version serverVersion, string serverCoreVersion, string details) {
            lblClientVersion.Text = $"F2 {clientVersion.ToMediumString()}";
            lblClientCoreVersion.Text = $"F2C {new F2Core.Compatibility.Version().ToMediumString()}";
            lblSupportedServerVersion.Text = $"F2S {clientVersion.SupportedVersion}";

            lblServerVersion.Text = $"F2S {serverVersion.ToMediumString()}";
            lblServerCoreVersion.Text = $"F2C {serverCoreVersion}";
            lblSupportedClientVersion.Text = $"F2 {serverVersion.SupportedVersion}";

            rtbDetails.Text = details;
        }
    }
}
