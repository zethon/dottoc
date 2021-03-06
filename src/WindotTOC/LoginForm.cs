﻿using System;
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

        //Thread t;
        BuddyListForm _blf;
        TOC _toc = new TOC();

        public LoginForm()
        {
            InitializeComponent();

            _toc.OnDisconnect += new TOC.OnDisconnectHandler(_toc_OnDisconnect);
            _toc.OnDoDisconnect += new TOC.OnDoDisconnectHandler(_toc_OnDoDisconnect);

            _toc.OnSignedOn += new IncomingHandlers.OnSignedOnHandler(_toc_OnSignedOn);
            _toc.OnTOCError += new TOC.OnTOCErrorHandler(_toc_OnTOCError);
            _toc.OnConfig += new IncomingHandlers.OnConfigHandler(_toc_OnConfig);

            _toc.OnSendServerMessage += new OutgoingHandlers.OnSendServerMessageHandler(_toc_OnSendServerMessage);
            _toc.OnFlapData += new FlapHandlers.OnFlapDataHandler(_toc_OnFlapData);

            _toc.OnFlapUnknown += new FlapHandlers.OnFlapUnknownHandler(_toc_OnFlapUnknown);


            log.Info("LoginForm created");
        }

        void _toc_OnDoDisconnect()
        {
            log.Info("Connection to TOC server closed by client");
        }

        void _toc_OnDisconnect(Exception ex)
        {
            log.Info("Connection to TOC server disconnected", ex);
        }

        void _toc_OnSendServerMessage(string Outgoing)
        {
            log.DebugFormat("OUTMSG:{0}", Outgoing);
        }

        void _toc_OnFlapData(FlapHeader fh, byte[] buffer)
        {
            string strMessage = Encoding.ASCII.GetString(buffer, 6, fh.DataLength);
            log.DebugFormat("INMSG({0}): {1}", fh.DataLength, strMessage);
        }

        void _toc_OnFlapUnknown(FlapHeader fh, byte[] buffer)
        {
            // Log Unknown Flap Types
            log.WarnFormat("Unknown FlapType `{0}` DataLength={1}", fh.FlapType, fh.DataLength);
            log.Warn(buffer.Take(fh.DataLength));

            string temp = System.Text.ASCIIEncoding.ASCII.GetString(buffer);
            log.Warn(temp);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            try
            {
                FileVersionInfo info = FileVersionInfo.GetVersionInfo("WindotTOC.exe");
                versionLabel.Text = "WindotTOC v" + info.FileVersion;

                info = FileVersionInfo.GetVersionInfo("dottoc.dll");
                label3.Text = "dotTOC v" + info.FileVersion;

                linkLabel1.Links.Remove(linkLabel1.Links[0]);
                linkLabel1.Links.Add(0, linkLabel1.Text.Length, @"http://code.google.com/p/dottoc/");

                log.Info("LoginForm loaded");
            }
            catch (Exception ex)
            {
                log.Fatal("Could not load LoginForm", ex);
                Program.AbortProgram();                
            }
        }

        private delegate void FormClosingHandler(object sender, FormClosingEventArgs e);
        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new FormClosingHandler(LoginForm_FormClosing), new object[] { sender, e });
            }
            else
            {
                if (!Properties.Settings.Default.RememberUser)
                {
                    usernameTxt.Text = string.Empty;
                }

                if (!Properties.Settings.Default.SavePassword || usernameTxt.Text == string.Empty)
                {
                    passwordTxt.Text = string.Empty;   
                }

                Properties.Settings.Default.Save();
                log.Info("LoginForm closing and defaults saved");
            }
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
            Invoke(new MethodInvoker(delegate
                {
                    log.Info("TOC Signed On");
                    _blf.Show();
                    this.Hide();
                }));
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

        void bl_FormClosed(object sender, FormClosedEventArgs e)
        {
            Invoke(new MethodInvoker(delegate
                {
                    if (!_blf.IsExiting)
                    {
                        _blf = null;
                        this.Show();
                    }
                }));
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(e.Link.LinkData.ToString());
            Process.Start(sInfo);
        }
    }
}
