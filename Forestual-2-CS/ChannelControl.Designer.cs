﻿namespace Forestual2CS
{
    partial class ChannelControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent() {
            this.webConversation = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webConversation
            // 
            this.webConversation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webConversation.IsWebBrowserContextMenuEnabled = false;
            this.webConversation.Location = new System.Drawing.Point(0, 0);
            this.webConversation.MinimumSize = new System.Drawing.Size(20, 20);
            this.webConversation.Name = "webConversation";
            this.webConversation.Size = new System.Drawing.Size(150, 150);
            this.webConversation.TabIndex = 0;
            // 
            // ChannelControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.webConversation);
            this.Name = "ChannelControl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webConversation;
    }
}
