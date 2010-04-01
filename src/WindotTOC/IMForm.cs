using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using dotTOC;
using log4net;

namespace WindotTOC
{
    public partial class IMForm : Form
    {
        static ILog log = LogManager.GetLogger(typeof(IMForm));

        string _strUsername = string.Empty;
        public string Username
        {
            get { return _strUsername; }
        }

        TOC _toc = null;

        public IMForm(TOC toc)
        {
            InitializeComponent();
            _toc = toc;
        }

        public IMForm(TOC toc, string strToUser)
        {
            InitializeComponent();
            _toc = toc;
            _strUsername = strToUser;
        }

        private void IMForm_Load(object sender, EventArgs e)
        {
            this.Text = _strUsername;
            editor1.ReadOnly = true;
        }

        delegate void NewIMMessageHandler(InstantMessage im);
        public void NewIMMessage(InstantMessage im)
        {
            if (!InvokeRequired)
            {
                _strUsername = im.From.Name;
                this.Text = _strUsername;

                if (editor1.BodyText == string.Empty)
                {
                    editor1.DocumentText += string.Format("<html><body><b><font color=\"#CC0000\">{0}</font></b>: {1}<br/></body></html>", im.From.Name, im.RawMessage);
                }
                else
                {
                    editor1.BodyHtml += string.Format("<b><font color=\"#CC0000\">{0}</font></b>: {1}<br/>", im.From.Name, im.RawMessage);
                }

                // TODO: scroll the window
            }
            else
            {
                Invoke(new NewIMMessageHandler(NewIMMessage), new object[] { im });
            }
        }

         private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                // send the TOC message
                _toc.SendIM(new InstantMessage { To = new Buddy { Name = _strUsername }, RawMessage = textBox1.Text });

                // add message to text box
                if (editor1.BodyText == string.Empty)
                {
                    editor1.DocumentText += string.Format("<html><body><b><font color=\"#204A9D\">{0}</font></b>: {1}<br/></body></html>", _toc.User.DisplayName, textBox1.Text);
                }
                else
                {
                    editor1.BodyHtml += string.Format("<b><font color=\"#204A9D\">{0}</font></b>: {1}<br/>", _toc.User.DisplayName, textBox1.Text);
                }

                // TODO: scroll the window

                // reset the input box
                textBox1.Text = string.Empty;
                textBox1.SelectionStart = 0;
                e.Handled = true;
            }
            else if (e.KeyChar == (char)27)
            {
                this.Close();
            }
        }


    }
}
