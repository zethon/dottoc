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
    public partial class LoginForm : Form
    {
        static ILog log = LogManager.GetLogger(typeof(LoginForm));

        Thread t;
        BuddyListForm _blf;
        TOC _toc = new TOC();

        public LoginForm()
        {
            log.Info("LoginForm created");
            InitializeComponent();
            _toc.OnSignedOn += new TOC.OnSignedOnHandler(_toc_OnSignedOn);
        }

        void _toc_OnSignedOn()
        {
            log.Info("TOC Signed On");

            //Thread t = new Thread(new ThreadStart(SignedOn));
            //t.Start();
            HideForm();
            _blf = new BuddyListForm(_toc);
            _blf.FormClosed += new FormClosedEventHandler(bl_FormClosed);
            t = new Thread(new ThreadStart(SignedOn));
            t.Start();
            //_blf.Show();
            //HideForm();
        }

        private void SignedOn()
        {
            _blf.ShowDialog();
            //_blf.Show();

            //this.Hide();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (usernameTxt.Text != string.Empty && passwordTxt.Text != string.Empty)
            {
                try
                {
                    log.InfoFormat("Connecting {0}", usernameTxt.Text);
                    _toc.Connect(usernameTxt.Text, passwordTxt.Text);
                }
                catch (Exception ex)
                {
                    log.Debug("Could not connect", ex);
                }
            }
        }

        void bl_FormClosed(object sender, FormClosedEventArgs e)
        {
            //throw new NotImplementedException();
            //this.Close();
            //Close();
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
    }
}
