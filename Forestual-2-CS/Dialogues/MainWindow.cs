using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using F2Core;
using F2Core.Management;
using Forestual2CS;
using Forestual2CS.Management;
using Newtonsoft.Json;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace Forestual2CS.Dialogues
{
    public partial class MainWindow : Form, IClient
    {
        private readonly RSACryptoServiceProvider ServiceProvider = new RSACryptoServiceProvider(4096);

        private const string PublicKey = "<RSAKeyValue><Modulus>n1G5qxqPSnvu3A0ympXV/qHCeMasaXOqrmlIF/2sAMgrjYmCXcAeyplvirGPDOUPHHUIBmZzqbtmU5Ol2l9VpMEesuDneEZh8nB9dpvtNe+LpoDAX4qVvrf78SXDzT9biFwJj8AAUgYI1JA2lN/+rHYCOYTlfrn1cln3q2F1sbtOKfJyYdt5PsbALI2In3b134k4XP93W5fLqNSFHbG3LcWTLkU06/cobg8etttjyyg5svUAEN+LnhtfrGilLW67oi4vHnjzhggEy7zo2RGfs2PJ8CnwlmAOGGtN/DaPTjobeHZRrIsIWy9/SPpSozaUV/mNxkrvYFEgE0BP6KCgS7HVXcJbsOcNIKIdUhRgRkXKT5XF7wakw9SjD3BCNZRIbfruBbN/dUx0jHgdU1zLJ1gVQcE0P/Fyrubq6VcKSTLrhygz2CkRSqUmE9MVmbISmDv13cI/lg/sTbEEpxWF+6lZdxmts5GVxjvTLbbv0CglRu8SyYGycWtHkSYsVEKYwBV5DRXfEWN8/uJcgrWxYNKH8+1nld/RSKVQ2lYKK2b0cJF4OHuhNGubNDDUn99LZviQmNQAzaK4hTFtRGaTVhcMOgl6KdEafQ6/oy9l9ynk+dw9HUJSq521ef1tFEvFYp3jNIjG8fcMikr5XuOoETaLFsdBNRPqMGQm7BuRzc8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private readonly TcpClient FClient = new TcpClient();
        private Connection FConnection;
        private readonly Thread Listening;
        private delegate void DParseStreamContent(string content);

        private bool IsSupposedClosing;
        private List<Account> Accounts;
        private List<Rank> Ranks;
        private List<Channel> Channels;
        public static List<Enumerations.Flag> Flags;

        private int ExtensionCount;

        private ChannelControl Channel;
        private string ActiveChannelControlId = "forestual";

        public enum AccountState
        {
            Offline,
            Online,
            Banned
        }
        private int YCoordinate;


        public MainWindow() {
            InitializeComponent();
            Closing += OnClosing;
            Listening = new Thread(Listen);
            tbxInput.KeyDown += OnTbxInputKeyDown;
            btnSend.Click += OnBtnSendClick;
            Channel = new ChannelControl();
            Channel.Dock = DockStyle.Fill;
            pnlConversation.Controls.Add(Channel);
            cbxSidebar.CheckedChanged += (sender, args) => pnlAccounts.Width = (cbxSidebar.Checked ? 300 : 0);
        }

        private void OnBtnSendClick(object sender, EventArgs e) {
            var Content = tbxInput.Text;
            if (!string.IsNullOrEmpty(Content) && !string.IsNullOrWhiteSpace(Content)) {
                FConnection.SetStreamContent(string.Join("|", Enumerations.Action.Plain.ToString(), Content));
                tbxInput.Clear();
            }
        }

        private void OnTbxInputKeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                btnSend.PerformClick();
            }
        }

        private void OnClosing(object sender, CancelEventArgs e) {
            try {
                IsSupposedClosing = true;
                Listening.Abort();
                FConnection.Dispose();
                FClient.Dispose();
            } catch { }
            Application.Exit();
        }

        public void Connect(string address, int port, string accountId, string password) {
            try {
                FClient.Connect(address, port);
                FConnection = new Connection(FClient.GetStream());
                Listening.Start();
                ServiceProvider.FromXmlString(PublicKey);
                var SessionData = Cryptography.GenerateAesData();
                FConnection.AesData = SessionData;
                FConnection.HmacKey = Cryptography.GenerateHmacKey();
                var SessionDataString = string.Join("|", Convert.ToBase64String(SessionData.Key), Convert.ToBase64String(SessionData.IV), Convert.ToBase64String(FConnection.HmacKey));
                FConnection.SetRawStreamContent(Cryptography.RSAEncrypt(SessionDataString, ServiceProvider));
                FConnection.SetStreamContent(string.Join("|", accountId, Cryptography.ComputeHash(password)));

                // Clear Channel Control
                Channel.Clear();

                // Add Handlers

                Directory.EnumerateFiles(Path.Combine(Application.StartupPath, "Extensions")).ToList().ForEach(File.Delete);
                ClientManagement.RegisterClient(this);

            } catch {
                var Result = MessageBox.Show("Forestual 2 couldn't connect to the server. Make sure the entered address and port is correct and the server is up and running and that your account isn't connected already.", "Forestual 2", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (Result == DialogResult.Retry) {
                    Connect(address, port, accountId, password);
                } else {
                    Application.Restart();
                }
            }
        }

        private void Listen() {
            while (FClient.Connected) {
                try {
                    Invoke(new DParseStreamContent(ParseStreamContent), FConnection.GetStreamContent());
                } catch {
                    if (!IsSupposedClosing)
                        MessageBox.Show("Forestual 2 lost the connection to the server.", "Forestual 2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    FConnection.Dispose();
                    FClient.Dispose();
                    Application.Exit();
                }
            }
        }

        private void ParseStreamContent(string content) {
            if (!string.IsNullOrEmpty(content) && !string.IsNullOrWhiteSpace(content)) {
                var Contents = content.Split('|');
                var Type = (Enumerations.Action) Enum.Parse(typeof(Enumerations.Action), Contents[0]);
                switch (Type) {
                case Enumerations.Action.SetState:
                    if (Contents[1] == Enumerations.ClientState.Banned.ToString()) {
                        var Punishment = JsonConvert.DeserializeObject<Punishment>(Contents[2]);
                        var Message = $"You're banned by {Punishment.CreatorId}. This ban lasts {(Punishment.Type == Enumerations.PunishmentType.Bann ? "permanently." : $"until\n{Punishment.EndDate.ToShortDateString()} {Punishment.EndDate.ToLongTimeString()}")}\nReason: {Punishment.Reason}";
                        MessageBox.Show(Message, "Forestual 2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Close();
                    } else if (Contents[1] == Enumerations.ClientState.Muted.ToString()) {
                        var Punishment = JsonConvert.DeserializeObject<Punishment>(Contents[2]);
                        var Message = $"You're muted by {Punishment.CreatorId}. This mute lasts until:\n{Punishment.EndDate.ToShortDateString()} {Punishment.EndDate.ToLongTimeString()}\nReason: {Punishment.Reason}";
                        MessageBox.Show(Message, "Forestual 2", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    break;
                case Enumerations.Action.LoginResult:
                    if (Contents[1] == "hej") {

                    } else if (Contents[1] == "authentificationFailed") {
                        MessageBox.Show("Authentification failed. The given password isn't correct.\nMake sure you entered the correct password and try again.", "Forestual 2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Restart();
                    } else {
                        MessageBox.Show("Authentification failed. The given account-id does not exist.\nMake sure you entered the correct id and try again.", "Forestual 2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Restart();
                    }
                    break;
                case Enumerations.Action.ExtensionTransport:
                    GC.Collect();
                    var Bytes = JsonConvert.DeserializeObject<byte[]>(Contents[1]);
                    if (!Directory.Exists(Path.Combine(Application.StartupPath, "Extensions")))
                        Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Extensions"));
                    File.WriteAllBytes(Path.Combine(Application.StartupPath, $"Extensions\\extension{ExtensionCount}.dll"), Bytes);
                    ExtensionManager.LoadExtension(Path.Combine(Application.StartupPath, $"Extensions\\extension{ExtensionCount}.dll"));
                    ExtensionCount++;
                    break;
                case Enumerations.Action.Extension:
                    ListenerManager.InvokeSpecialEvent(JsonConvert.DeserializeObject<EventArguments>(Contents[1]));
                    break;
                case Enumerations.Action.Plain:
                    Invoke(new Action(() => Channel.AddMessage(JsonConvert.DeserializeObject<F2Core.Message>(Contents[1]))));
                    break;
                case Enumerations.Action.ClearConversation:
                    Invoke(new Action(() => Channel.Clear()));
                    break;
                case Enumerations.Action.SetRankList:
                    Ranks = JsonConvert.DeserializeObject<List<Rank>>(Contents[1]);
                    break;
                case Enumerations.Action.SetAccountList:
                    Accounts = JsonConvert.DeserializeObject<List<Account>>(Contents[1]);
                    DisplayAccounts();
                    break;
                case Enumerations.Action.SetChannelList:
                    Channels = JsonConvert.DeserializeObject<List<Channel>>(Contents[1]);
                    // Refresh Channels
                    break;
                case Enumerations.Action.SetFlags:
                    Flags = JsonConvert.DeserializeObject<List<Enumerations.Flag>>(Contents[1]);
                    SetControlAccessability();
                    break;
                case Enumerations.Action.SetChannel:
                    // Set Channel
                    break;
                case Enumerations.Action.SetAccountData:
                    GC.Collect();
                    var Window = new ProfileWindow();
                    var AvatarPath = Path.Combine(Application.StartupPath, "Storage\\avatar.png");
                    File.WriteAllBytes(AvatarPath, JsonConvert.DeserializeObject<byte[]>(Contents[1]));
                    var HeaderPath = Path.Combine(Application.StartupPath, "Storage\\header.png");
                    File.WriteAllBytes(HeaderPath, JsonConvert.DeserializeObject<byte[]>(Contents[2]));
                    Window.ShowDialog(Image.FromFile(AvatarPath), Image.FromFile(HeaderPath), bool.Parse(Contents[3]), Contents[4], bool.Parse(Contents[5]), Contents[6], Contents[7], Contents[8]);
                    break;
                case Enumerations.Action.Disconnect:
                    MessageBox.Show($"The connection was closed by the server.\n\n{Contents[1]}", "Forestual 2", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FConnection.Dispose();
                    FClient.Dispose();
                    Application.Exit();
                    break;
                }
            }
        }

        private void DisplayAccounts() {
            pnlAccounts.Controls.Clear();
            YCoordinate = 0;
            foreach (var Rank in Ranks) {
                var Header = GetHeaderPanel(Rank.Name, ColorTranslator.FromHtml(Rank.Color));
                Header.Location = new Point(0, YCoordinate);
                YCoordinate += 20;
                pnlAccounts.Controls.Add(Header);
                foreach (var Account in Accounts.FindAll(a => a.RankId == Rank.Id)) {
                    var Item = GetItemPanel(Account.Name, ColorTranslator.FromHtml(Rank.Color), Account.Online);
                    Item.Location = new Point(0, YCoordinate);
                    YCoordinate += 51;
                    pnlAccounts.Controls.Add(Item);
                }
                YCoordinate -= 1;
            }
        }

        private Panel GetHeaderPanel(string header, Color background) {
            var Panel = new Panel();
            Panel.Width = pnlAccounts.Width - 17;
            Panel.Height = 20;
            Panel.BackColor = background;
            var Label = new Label();
            Label.Text = header;
            Label.ForeColor = Color.White;
            Label.AutoSize = false;
            Label.TextAlign = ContentAlignment.MiddleCenter;
            Label.Dock = DockStyle.Fill;
            Panel.Controls.Add(Label);
            return Panel;
        }

        private Panel GetItemPanel(string accountName, Color background, bool online) {
            var Panel = new Panel {
                Width = pnlAccounts.Width - 17,
                Height = 50,
                BackColor = Color.Gainsboro,
                Name = $"pnlListEntry:{accountName}",
                Cursor = Cursors.Hand
            };
            Panel.Click += OnAccountListEntryClicked;
            var Label = new Label {
                ForeColor = Color.White,
                Padding = new Padding(3),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Text = accountName,
                AutoSize = true,
                BackColor = background,
                Location = new Point(15, 15),
                Name = $"lblListEntry:{accountName}",
                Cursor = Cursors.Hand
            };
            Label.Click += OnAccountListEntryClicked;
            Panel.Controls.Add(Label);
            var Status = new DoubleBufferedPanel {
                Size = new Size(20, 20),
                Location = new Point(Panel.Width - 35, 15),
                State = online ? AccountState.Online : AccountState.Offline
            };
            Status.Paint += StatusElementPaint;
            Panel.Controls.Add(Status);
            if (online) {
                var Label2 = new Label {
                    ForeColor = Color.DimGray,
                    Padding = new Padding(3),
                    Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                    Text = "in Forestual",
                    //$"in {Accounts.Find(a => a.Name == accountName).Channel.Name}";
                    AutoSize = true,
                    BackColor = Color.White,
                    Location = new Point(Label.Width + 21, 15),
                    Name = $"lblListStatusEntry:{accountName}",
                    Cursor = Cursors.Hand
                };
                Label2.Click += OnAccountListEntryClicked;
                Panel.Controls.Add(Label2);
            } else {
                Status.State = AccountState.Offline;
            }
            return Panel;
        }

        private void OnAccountListEntryClicked(object sender, EventArgs e) {
            var AccountName = ((Control) sender).Name.Split(':')[1];
            SendToServer(string.Join("|", Enumerations.Action.GetAccountData, AccountName));
        }

        private void StatusElementPaint(object sender, PaintEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            SolidBrush Brush;
            var Panel = (DoubleBufferedPanel) sender;
            if (Panel.State == AccountState.Online) {
                Brush = new SolidBrush(ColorTranslator.FromHtml("#1ED760"));
            } else if (Panel.State == AccountState.Offline) {
                Brush = new SolidBrush(Color.DimGray);
            } else {
                Brush = new SolidBrush(ColorTranslator.FromHtml("#FC3539"));
            }
            e.Graphics.FillEllipse(Brush, new RectangleF(0, 0, 19F, 19F));
        }

        private void SetControlAccessability() {

        }

        public void DisplayForm(Form form) {
            Invoke(new Action<Form>(f => f.Show()), form);
        }

        public void SendToServer(string content) {
            FConnection.SetStreamContent(content);
        }

        public string Serialize(dynamic content, bool indented) {
            if (indented)
                return JsonConvert.SerializeObject(content, Formatting.Indented);
            return JsonConvert.SerializeObject(content);
        }

        public dynamic Deserialize(string content) {
            return JsonConvert.DeserializeObject(content);
        }
    }
}
