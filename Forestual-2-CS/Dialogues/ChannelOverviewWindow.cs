using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using F2Core;
using F2Core.Management;
using Forestual2CS.Management;
using Newtonsoft.Json;

namespace Forestual2CS.Dialogues
{
    public partial class ChannelOverviewWindow : Form
    {
        public ChannelOverviewWindow() {
            InitializeComponent();

            btnJoin.Click += OnBtnJoinClick;
            btnCreate.Click += OnBtnCreateClick;
            btnEdit.Click += OnBtnEditClick;
            btnClose.Click += OnBtnCloseClick;
            btnHypermove.Click += OnBtnHypermoveClick;
        }

        private void OnBtnJoinClick(object sender, EventArgs e) {
        }

        private void OnBtnHypermoveClick(object sender, EventArgs e) {
            if (MainWindow.LuvaValues.CheckValue("forestual.canHyper")) {

            }
        }

        private void OnBtnCloseClick(object sender, EventArgs e) {
            if (MainWindow.LuvaValues.CheckValue("forestual.canCloseChannels")) {

            }
        }

        private void OnBtnEditClick(object sender, EventArgs e) {
            if (MainWindow.LuvaValues.CheckValue("forestual.canEditChannels")) {

            }
        }

        private void OnBtnCreateClick(object sender, EventArgs e) {
            if (MainWindow.LuvaValues.CheckValue("forestual.canCreateChannels")) {
                var Dialog = new ChannelCreateDialog();
                if (Dialog.ShowDialog() == DialogResult.OK) {
                    var Channel = Dialog.GetChannel();
                    ExtensionPool.Client.SendToServer(string.Join("|", Enumerations.Action.CreateChannel, JsonConvert.SerializeObject(Channel)));
                    Close();
                }
            }
        }

        private int YCoordinate;

        public void SetChannels(IEnumerable<Channel> channels) {
            YCoordinate = 0;
            foreach (var Item in pnlItemContainer.Controls.OfType<ChannelListItem>()) {
                Item.SelectionChanged -= OnItemSelectionChanged;
            }
            pnlItemContainer.Controls.Clear();
            var ItemSize = new Size(channels.Count() * 68 > pnlItemContainer.Height ? 507 : 524, 60);
            foreach (var Channel in channels) {
                var Item = new ChannelListItem {
                    ChannelName = Channel.Name,
                    ChannelId = Channel.Id,
                    ChannelCreator = Channel.OwnerId,
                    ChannelProtection = Channel.JoinRestrictionMode.ToString(),
                    MemberCount = Channel.MemberIds.Count,
                    MemberCapacity = Channel.Capacity
                };
                Item.SelectionChanged += OnItemSelectionChanged;
                Item.Location = new Point(0, YCoordinate);
                Item.Size = ItemSize;
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
