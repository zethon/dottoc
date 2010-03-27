namespace WindotTOC
{
    partial class IMForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.msgText = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveConversationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearScrollbackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableLoggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTimestampsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // msgText
            // 
            this.msgText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.msgText.BackColor = System.Drawing.SystemColors.Window;
            this.msgText.Location = new System.Drawing.Point(9, 27);
            this.msgText.Multiline = true;
            this.msgText.Name = "msgText";
            this.msgText.ReadOnly = true;
            this.msgText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.msgText.Size = new System.Drawing.Size(275, 187);
            this.msgText.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(9, 220);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(274, 41);
            this.textBox1.TabIndex = 0;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(295, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveConversationToolStripMenuItem,
            this.clearScrollbackToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem1.Text = "&File";
            // 
            // saveConversationToolStripMenuItem
            // 
            this.saveConversationToolStripMenuItem.Name = "saveConversationToolStripMenuItem";
            this.saveConversationToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.saveConversationToolStripMenuItem.Text = "&Save Conversation";
            // 
            // clearScrollbackToolStripMenuItem
            // 
            this.clearScrollbackToolStripMenuItem.Name = "clearScrollbackToolStripMenuItem";
            this.clearScrollbackToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.clearScrollbackToolStripMenuItem.Text = "&Clear Scrollback";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableLoggingToolStripMenuItem,
            this.showTimestampsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // enableLoggingToolStripMenuItem
            // 
            this.enableLoggingToolStripMenuItem.Name = "enableLoggingToolStripMenuItem";
            this.enableLoggingToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.enableLoggingToolStripMenuItem.Text = "Enable Logging";
            // 
            // showTimestampsToolStripMenuItem
            // 
            this.showTimestampsToolStripMenuItem.Name = "showTimestampsToolStripMenuItem";
            this.showTimestampsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.showTimestampsToolStripMenuItem.Text = "Show Timestamps";
            // 
            // IMForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(295, 273);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.msgText);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "IMForm";
            this.Text = "IMForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox msgText;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveConversationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearScrollbackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableLoggingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showTimestampsToolStripMenuItem;
    }
}