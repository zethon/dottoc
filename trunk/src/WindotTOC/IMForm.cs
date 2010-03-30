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

            this.Text = _strUsername;
        }

        public IMForm(TOC toc, string strToUser)
        {
            InitializeComponent();
            _toc = toc;
            _strUsername = strToUser;

            this.Text = _strUsername;
        }

        delegate void NewIMMessageHandler(InstantMessage im);
        public void NewIMMessage(InstantMessage im)
        {
            if (!InvokeRequired)
            {
                _strUsername = im.From.Name;
                this.Text = _strUsername;

                int iSelectStart = msgText.Text.Length;

                // append the message
                msgText.Text += string.Format("{0}: {1}\r\n", im.From.Name, im.Message);

                // add the username
                msgText.SelectionStart = iSelectStart;
                msgText.SelectionLength = im.From.Name.Length;
                msgText.SelectionColor = Color.DarkRed;
                msgText.SelectionFont = new Font(msgText.Font, FontStyle.Bold);

                // color the message
                msgText.SelectionStart = iSelectStart + im.From.Name.Length + 1;
                msgText.SelectionLength = im.Message.Length + 1;
                msgText.SelectionColor = Color.Black;
                msgText.SelectionFont = new Font(msgText.Font, FontStyle.Regular);
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
                int iSelectionStart = msgText.Text.Length;
                msgText.Text += string.Format("{0}: {1}\r\n", _toc.User.DisplayName, textBox1.Text);

                // color the username
                msgText.SelectionStart = iSelectionStart;
                msgText.SelectionLength = _toc.User.DisplayName.Length;
                msgText.SelectionColor = Color.DarkBlue;
                msgText.SelectionFont = new Font(msgText.Font, FontStyle.Bold);

                // color the message
                msgText.SelectionStart = iSelectionStart + _toc.User.DisplayName.Length;
                msgText.SelectionLength = textBox1.Text.Length;
                msgText.SelectionColor = Color.Black;
                msgText.SelectionFont = new Font(msgText.Font, FontStyle.Regular);


                // reset the input box
                textBox1.Text = string.Empty;
                textBox1.SelectionStart = 0;
                e.Handled = true;
            }
        }
    }
}
