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
            log.InfoFormat("Loading IMForm for `{0}`", _strUsername);
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
                    editor1.DocumentText += string.Format("<html><body><font color=\"#CC0000\">({0}) <b>{1}</b></font>: {2}<br/></body></html>", 
                        DateTime.Now.ToShortTimeString(), im.From.Name, im.RawMessage);
                }
                else
                {
                    editor1.BodyHtml += string.Format("<font color=\"#CC0000\">({0}) <b>{1}</b></font>: {2}<br/>", 
                        DateTime.Now.ToShortTimeString(), im.From.Name, im.RawMessage);

                    editor1.Document.Window.ScrollTo(0, editor1.Document.Body.ScrollRectangle.Height);
                }
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
                    editor1.DocumentText += string.Format("<html><body><font color=\"#204A9D\">({0}) <b>{1}</b></font>: {2}<br/></body></html>", 
                        DateTime.Now.ToShortTimeString(), _toc.User.DisplayName, textBox1.Text);
                }
                else
                {
                    editor1.BodyHtml += string.Format("<font color=\"#204A9D\">({0}) <b>{1}</b></font>: {2}<br/>", 
                        DateTime.Now.ToShortTimeString(), _toc.User.DisplayName, textBox1.Text);

                    editor1.Document.Window.ScrollTo(0, editor1.Document.Body.ScrollRectangle.Height);
                }

                // reset the input box
                textBox1.Text = string.Empty;
                textBox1.SelectionStart = 0;
                e.Handled = true;
            }
        }


    }
}
