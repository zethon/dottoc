using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using dotTOC;
using log4net;

namespace WindotTOC
{
    public partial class LoginForm : Form
    {
        static ILog log = LogManager.GetLogger(typeof(LoginForm));

        Thread t;
        BuddyListForm _blf;
        TOC _toc = new TOC();

        public LoginForm()
        {
            InitializeComponent();

            _toc.OnSignedOn += new TOC.OnSignedOnHandler(_toc_OnSignedOn);
            _toc.OnTOCError += new TOC.OnTOCErrorHandler(_toc_OnTOCError);
            _toc.OnConfig += new TOC.OnConfigHandler(_toc_OnConfig);

            _toc.OnServerMessage += new TOC.OnServerMessageHandler(_toc_OnServerMessage);
            _toc.OnSendServerMessage += new TOC.OnSendServerMessageHandler(_toc_OnSendServerMessage);
            
            log.Info("LoginForm created");
        }

        void _toc_OnSendServerMessage(string Outgoing)
        {
            log.DebugFormat("<< {0}", Outgoing);
        }

        void _toc_OnServerMessage(string strIncoming)
        {
            log.DebugFormat(">> {0}", strIncoming);
        }

        void _toc_OnConfig(UserConfig config)
        {
            _blf.ProcessConfig(config);
        }

        delegate void OnErrorHandler(TOCError error);
        void _toc_OnTOCError(TOCError error)
        {
            if (!InvokeRequired)
            {
                string strError = string.Empty;
                switch (error.Code)
                {
                    case "980":
                        strError += "Invalid login";
                    break;

                    default:
                        strError += string.Format("Unknown error: {0}", error.Code);
                    break;
                }

                errorLabel.Visible = true;
                errorLabel.Text = strError;

                log.ErrorFormat("TOC Error Code:{0} Argument:{1}", error.Code, error.Argument);
            }
            else
            {
                Invoke(new OnErrorHandler(_toc_OnTOCError), new object[] { error });
            }
        }

        void _toc_OnSignedOn()
        {
            log.Info("TOC Signed On");

            HideForm();


            t = new Thread(new ThreadStart(SignedOn));
            t.Start();
        }

        private void SignedOn()
        {
            _blf.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (usernameTxt.Text != string.Empty && passwordTxt.Text != string.Empty)
            {
                try
                {
                    log.InfoFormat("Connecting {0}", usernameTxt.Text);

                    // initialize the buddy list form so it can receive callbacks
                    _blf = new BuddyListForm(_toc);
                    _blf.FormClosed += new FormClosedEventHandler(bl_FormClosed);

                    _toc.Connect(usernameTxt.Text, passwordTxt.Text);
                    errorLabel.Visible = false;
                    errorLabel.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    log.Error("Could not connect", ex);
                }
            }
            else
            {
                log.Info("Login clicked without username or pw");
            }
        }

        private delegate void FormClosedHandler(object sender, FormClosedEventArgs e);
        void bl_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new FormClosedHandler(bl_FormClosed), new object[] { sender, e });
            }
            else
            {
                if (!_blf.IsExiting)
                {
                    this.Show();
                }
            }
        }

        private delegate void HideFormCallback();
        virtual public void HideForm()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    HideFormCallback cb = new HideFormCallback(HideForm);
                    this.Invoke(cb);
                }
                else
                {
                    lock (this)
                    {
                        this.Hide();
                        Application.DoEvents();
                    }
                }
            }
            catch
            {

            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            FileVersionInfo info = FileVersionInfo.GetVersionInfo("WindotTOC.exe");
            versionLabel.Text += " "+info.FileMajorPart+"."+info.FileMinorPart;

            linkLabel1.Links.Remove(linkLabel1.Links[0]);
            linkLabel1.Links.Add(0, linkLabel1.Text.Length, @"http://code.google.com/p/dottoc/");  
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(e.Link.LinkData.ToString());
            Process.Start(sInfo);
        }
    }
}
