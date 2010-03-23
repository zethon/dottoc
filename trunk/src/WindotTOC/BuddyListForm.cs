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
    public partial class BuddyListForm : Form
    {
        static ILog log = LogManager.GetLogger(typeof(BuddyListForm));

        Dictionary<string, IMForm> _IMForms;
        TOC _toc = null;

        public BuddyListForm(TOC tocObj)
        {
            InitializeComponent();
            _IMForms = new Dictionary<string, IMForm>();

            _toc = tocObj;
            _toc.OnIMIn += new TOC.OnIMInHandler(OnNewMessage);
        }

        private delegate void NewWindowHandler(InstantMessage im);
        void OnNewMessage(InstantMessage im)
        {
            if (!InvokeRequired)
            {
                log.InfoFormat("IMSG {0}:{1}", im.From, im.Message);

                if (_IMForms.ContainsKey(im.From))
                {
                    IMForm imf = _IMForms[im.From];
                    imf.NewIMMessage(im);
                }
                else
                {
                    IMForm imf = new IMForm(_toc);
                    imf.FormClosed += new FormClosedEventHandler(OnIMFormClosed);

                    lock (_IMForms)
                    {
                        _IMForms.Add(im.From, imf);
                    }

                    imf.NewIMMessage(im);
                    imf.Show();
                }
            }
            else
            {
                Invoke(new NewWindowHandler(OnNewMessage), new object[] { im });
            }
        }

        void OnIMFormClosed(object sender, FormClosedEventArgs e)
        {
            IMForm frm = sender as IMForm;

            if (frm != null)
            {
                _IMForms.Remove(frm.Username);
            }
        }
    }
}
