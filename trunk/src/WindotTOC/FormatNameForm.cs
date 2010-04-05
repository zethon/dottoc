using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;

namespace WindotTOC
{
    public partial class FormatNameForm : Form
    {
        static ILog log = LogManager.GetLogger(typeof(FormatNameForm));

        public FormatNameForm()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                BuddyListForm blf = this.Owner as BuddyListForm;

                if (blf != null)
                {
                    blf.TOC.FormatNickname(textBox1.Text);
                }
                else
                {
                    log.Error("Could not get BuddyListForm object from this.Owner");
                    this.DialogResult = DialogResult.Abort;
                }
            }
            catch (Exception ex)
            {
                log.Error("Could not handle `Save`", ex);
            }
            finally
            {
                this.Close();
            }
        }
    }
}
