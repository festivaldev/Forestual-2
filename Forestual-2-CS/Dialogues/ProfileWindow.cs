﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forestual2CS.Dialogues
{
    public partial class ProfileWindow : Form
    {
        public Image Avatar { get; set; }
        public Image Header { get; set; }
        public bool Online { get; set; }
        public string AccountName { get; set; }
        public bool Editable { get; set; }
        public string Rank { get; set; }
        public string Money { get; set; }
        public string LastSeen { get; set; }

        public ProfileWindow() {
            InitializeComponent();
            pnlHeader.Paint += OnPanelHeaderPaint;
            this.Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e) {
            Avatar.Dispose();
            Header.Dispose();
        }

        public DialogResult ShowDialog(Image avatar, Image header, bool online, string accountName, bool editable, string rank, string money, string lastSeen) {
            Avatar = avatar;
            Header = header;
            Online = online;
            AccountName = accountName;
            Editable = editable;
            Rank = rank;
            Money = money;
            LastSeen = lastSeen;
            lblRank.Text = Rank;
            lblMoney.Text = Money;
            lblLastSeen.Text = LastSeen;
            return base.ShowDialog();
        }

        private void OnPanelHeaderPaint(object sender, PaintEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            var FullRectangle = new Rectangle(0, 0, 650, 120);
            e.Graphics.DrawImage(Blur(Header, FullRectangle, 2), FullRectangle);

            e.Graphics.FillRectangle(new LinearGradientBrush(new Point(0, 0), new Point(0, 120), Color.Transparent, Color.FromArgb(190, 0, 0, 0)), FullRectangle);

            //var AvatarRectangle = new Rectangle(20, 30, 120, 120);
            //e.Graphics.DrawImage(Circlelize(Avatar), AvatarRectangle);

            //var TextRectangle = new Rectangle(160, 30, 435, 90);
            //e.Graphics.DrawString(AccountName, new Font("Segoe UI Semibold", 34F), new SolidBrush(Color.WhiteSmoke), TextRectangle, new StringFormat { LineAlignment = StringAlignment.Center });

            //e.Graphics.FillEllipse(new SolidBrush(ColorTranslator.FromHtml(Online ? "#19E68C" : "#FC3539")), new Rectangle(615, 70, 15, 15));

            var AvatarRectangle = new Rectangle(20, 20, 80, 80);
            e.Graphics.DrawImage(Circlelize(Avatar), AvatarRectangle);
            var TextRectangle = new Rectangle(120, 0, 530, 120);
            e.Graphics.DrawString(AccountName, new Font("Segoe UI Semibold", 34F), new SolidBrush(Color.WhiteSmoke), TextRectangle, new StringFormat { LineAlignment = StringAlignment.Center });
            e.Graphics.FillEllipse(new SolidBrush(ColorTranslator.FromHtml(Online ? "#19E68C" : "#FC3539")), new Rectangle(615, 52, 15, 15));
            GC.Collect();
        }

        public Image Circlelize(Image source) {
            Image Target = new Bitmap(source.Width, source.Height);
            var G = Graphics.FromImage(Target);
            var GraphicsPath = new GraphicsPath();
            GraphicsPath.AddEllipse(0, 0, Target.Width, Target.Height);
            G.SetClip(GraphicsPath);
            G.DrawImage(source, 0, 0);
            GraphicsPath.Dispose();
            G.Dispose();
            return Target;
        }

        private static Bitmap Blur(Image image, Rectangle rectangle, int blurSize) {
            var Blurred = new Bitmap(image.Width, image.Height);
            
            using (var Graphics = System.Drawing.Graphics.FromImage(Blurred)) {
                Graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
            
            for (var xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++) {
                for (var yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++) {
                    int AverageR = 0, AverageG = 0, AverageB = 0;
                    var BlurPixelCount = 0;
                    
                    for (var x = xx; x < xx + blurSize && x < image.Width; x++) {
                        for (var y = yy; y < yy + blurSize && y < image.Height; y++) {
                            var Pixel = Blurred.GetPixel(x, y);

                            AverageR += Pixel.R;
                            AverageG += Pixel.G;
                            AverageB += Pixel.B;

                            BlurPixelCount++;
                        }
                    }

                    AverageR = AverageR / BlurPixelCount;
                    AverageG = AverageG / BlurPixelCount;
                    AverageB = AverageB / BlurPixelCount;
                    
                    for (var x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++) {
                        for (var y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++) {
                            Blurred.SetPixel(x, y, Color.FromArgb(AverageR, AverageG, AverageB));
                        }
                    }
                }
            }
            return Blurred;
        }
    }
}
