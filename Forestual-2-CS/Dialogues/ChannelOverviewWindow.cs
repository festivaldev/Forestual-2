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
    public partial class ChannelOverviewWindow : Form
    {
        public ChannelOverviewWindow() {
            InitializeComponent();
        }

        private int YCoordinate;

        public void SetChannels(IEnumerable<Channel> channels) {
            YCoordinate = 0;
            foreach (var Item in pnlItemContainer.Controls.OfType<ChannelListItem>()) {
                Item.SelectionChanged -= OnItemSelectionChanged;
            }
            pnlItemContainer.Controls.Clear();
            var Size = new Size(channels.Count() * 68 > pnlItemContainer.Height ? 507 : 524, 60);
            foreach (var Channel in channels) {
                var Item = new ChannelListItem {
                    ChannelName = Channel.Name,
                    ChannelCreator = Channel.Owner.Id,
                    ChannelProtection = Channel.JoinRestrictionMode.ToString(),
                    MemberCount = Channel.MemberIds.Count,
                    MemberCapacity = -1
                };
                Item.SelectionChanged += OnItemSelectionChanged;
                Item.Location = new Point(0, YCoordinate);
                Item.Size = Size;
                YCoordinate += 68;
                pnlItemContainer.Controls.Add(Item);
                Item.RefreshItem();
            }
        }

        private void OnItemSelectionChanged(ChannelListItem sender, bool selected) {
            var SelectedItem = pnlItemContainer.Controls.OfType<ChannelListItem>().ToList().Find(i => i.Mode == ChannelListItem.SelectionMode.Selected && i.ChannelName != sender.ChannelName);
            if (SelectedItem != null) {
                SelectedItem.Mode = ChannelListItem.SelectionMode.None;
            }
        }
    }
}
