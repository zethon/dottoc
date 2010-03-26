namespace WindotTOC
{
    partial class BuddyListForm
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
            this.buddyTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // buddyTree
            // 
            this.buddyTree.Location = new System.Drawing.Point(8, 15);
            this.buddyTree.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buddyTree.Name = "buddyTree";
            this.buddyTree.Size = new System.Drawing.Size(255, 398);
            this.buddyTree.TabIndex = 0;
            // 
            // BuddyListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 422);
            this.Controls.Add(this.buddyTree);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "BuddyListForm";
            this.Text = "BuddyListForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView buddyTree;
    }
}