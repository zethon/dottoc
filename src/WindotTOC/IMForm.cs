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

namespace WindotTOC
{
    public partial class IMForm : Form
    {
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

        delegate void NewIMMessageHandler(InstantMessage im);
        public void NewIMMessage(InstantMessage im)
        {
            if (!InvokeRequired)
            {
                _strUsername = im.From;
                msgText.Text += string.Format("{0}: {1}\r\n", im.From, im.Message);
                msgText.SelectionStart = msgText.Text.Length - 1;
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
                _toc.SendIM(new InstantMessage { To = _strUsername, Message = textBox1.Text });
                msgText.Text += string.Format("{0}: {1}\r\n", _toc.User.DisplayName, textBox1.Text);
                textBox1.Text = string.Empty;
            }
        }
    }
}
