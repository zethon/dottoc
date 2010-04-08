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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IMForm));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveConversationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearScrollbackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableLoggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTimestampsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.editor1 = new Design.Editor();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 360);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(443, 50);
            this.textBox1.TabIndex = 0;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveConversationToolStripMenuItem,
            this.clearScrollbackToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(44, 24);
            this.toolStripMenuItem1.Text = "&File";
            // 
            // saveConversationToolStripMenuItem
            // 
            this.saveConversationToolStripMenuItem.Name = "saveConversationToolStripMenuItem";
            this.saveConversationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveConversationToolStripMenuItem.Size = new System.Drawing.Size(249, 24);
            this.saveConversationToolStripMenuItem.Text = "&Save Conversation";
            // 
            // clearScrollbackToolStripMenuItem
            // 
            this.clearScrollbackToolStripMenuItem.Name = "clearScrollbackToolStripMenuItem";
            this.clearScrollbackToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.clearScrollbackToolStripMenuItem.Size = new System.Drawing.Size(249, 24);
            this.clearScrollbackToolStripMenuItem.Text = "&Clear Scrollback";
            this.clearScrollbackToolStripMenuItem.Click += new System.EventHandler(this.clearScrollbackToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableLoggingToolStripMenuItem,
            this.showTimestampsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(73, 24);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // enableLoggingToolStripMenuItem
            // 
            this.enableLoggingToolStripMenuItem.Name = "enableLoggingToolStripMenuItem";
            this.enableLoggingToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.enableLoggingToolStripMenuItem.Text = "Enable Logging";
            // 
            // showTimestampsToolStripMenuItem
            // 
            this.showTimestampsToolStripMenuItem.Name = "showTimestampsToolStripMenuItem";
            this.showTimestampsToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.showTimestampsToolStripMenuItem.Text = "Show Timestamps";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(472, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // editor1
            // 
            this.editor1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.editor1.BodyHtml = null;
            this.editor1.BodyText = null;
            this.editor1.DocumentText = resources.GetString("editor1.DocumentText");
            this.editor1.EditorBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.editor1.EditorForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.editor1.FontName = null;
            this.editor1.FontSize = Design.FontSize.NA;
            this.editor1.Location = new System.Drawing.Point(16, 14);
            this.editor1.Margin = new System.Windows.Forms.Padding(5);
            this.editor1.Name = "editor1";
            this.editor1.ReadOnly = false;
            this.editor1.Size = new System.Drawing.Size(440, 338);
            this.editor1.TabIndex = 3;
            // 
            // IMForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 426);
            this.Controls.Add(this.editor1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "IMForm";
            this.Text = "IMForm";
            this.Load += new System.EventHandler(this.IMForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private Design.Editor editor1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveConversationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearScrollbackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableLoggingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showTimestampsToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
    }
}