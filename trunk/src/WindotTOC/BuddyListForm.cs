﻿using System;
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

        public bool IsExiting = false;

        // privates
        UserConfig _config;
        Dictionary<string, IMForm> _IMForms;
        TOC _toc = null;

        public BuddyListForm(TOC tocObj)
        {
            InitializeComponent();
            _IMForms = new Dictionary<string, IMForm>();

            _toc = tocObj;
            _toc.OnIMIn += new TOC.OnIMInHandler(OnNewMessage);
            _toc.OnUpdateBuddy += new TOC.OnUpdateBubbyHandler(OnUpdateBuddy);
        }

        private delegate void UpdateBuddyHandler(Buddy buddy);
        private void OnUpdateBuddy(Buddy buddy)
        {
            if (!InvokeRequired)
            {
                log.DebugFormat("Received TOC UPDATE_BUDDY2 for `{0}`", buddy.NormalizedName);

                // search the buddlist. 
                var q = from n in buddyTree.Nodes.Find(buddy.Name, true)
                        where n.Tag is Buddy && n.Name == buddy.NormalizedName 
                        select n;

                // the buddy exists on the treeview
                if (q.Count() > 0)
                {
                    log.DebugFormat("Buddy `{0}` exists in buddyTree", buddy.NormalizedName);
                    foreach (TreeNode currentNode in q)
                    {
                        if (!buddy.Online)
                        {
                            log.DebugFormat("Removing buddy `{0}`", buddy.NormalizedName);
                            currentNode.Remove();
                        }
                        else
                        {
                            // TODO: update the buddy's display on the treeview
                            log.DebugFormat("Updating Buddy `{0}`", buddy.NormalizedName);
                        }
                    }
                }
                else if (buddy.Online)
                { // buddy is not on list and is now online

                    log.DebugFormat("Buddy `{0}` is not in buddyTree but is Online", buddy.NormalizedName);

                    // check the config to see if we have info about this buddy in the config
                    foreach (string strKey in _config.BuddyList.Keys)
                    {
                        List<Buddy> bl = _config.BuddyList[strKey];

                        var q1 = from b in bl
                                 where b.NormalizedName == buddy.NormalizedName
                                 select b;

                        if (q1.Count() > 0)
                        {
                            log.DebugFormat("Buddy `{0}` exists in config in group `{1}`", buddy.NormalizedName, strKey);

                            // buddy can't be in the same group more than once
                            Buddy configBuddyObj = q1.Single() as Buddy;

                            TreeNode[] groupNodes = buddyTree.Nodes.Find(strKey, false);

                            //// see if a node for the group exists in the treeview
                            TreeNode groupNode = null;
                            if (groupNodes.Count() == 0)
                            { // group node not found, add it to the treeview

                                TreeNode tempNode = new TreeNode
                                {
                                    Name = strKey,
                                    Text = strKey,
                                };

                                buddyTree.Nodes.Add(tempNode);
                                tempNode.Expand();

                                groupNode = tempNode;
                            }
                            else
                            {
                                groupNode = groupNodes[0];
                            }

                            string strImageKey = @"online";
                            FontStyle fs = FontStyle.Regular;

                            if (buddy.MarkedUnavailable)
                            {
                                strImageKey = @"unavailable";
                                fs = FontStyle.Strikeout;
                            }
                            else if (buddy.IdleTime > 0)
                            {
                                strImageKey = @"idle";
                                fs = FontStyle.Italic;
                            }

                            // add the buddy to the treeview node
                            groupNode.Nodes.Add(new TreeNode
                            {
                                Name = buddy.NormalizedName,
                                Text = buddy.Name,
                                Tag = buddy,
                                NodeFont = new Font(buddyTree.Font, fs),
                                ImageKey = strImageKey,
                                SelectedImageKey = strImageKey
                            });

                            break;
                        }
                    }
                }
                else
                {
                    log.DebugFormat("No buddyTree action taken for `{0}`", buddy.NormalizedName);
                }
            }
            else
            {
                Invoke(new UpdateBuddyHandler(OnUpdateBuddy), new object[] { buddy } );
            }
        }

        private delegate void ProcessConfigHanlder(UserConfig config);
        public void ProcessConfig(UserConfig config)
        {
            if (!InvokeRequired)
            {
                _config = config;
            }
            else
            {
                Invoke(new ProcessConfigHanlder(ProcessConfig), new object[] { config });
            }
        }

        private delegate void NewWindowHandler(InstantMessage im);
        void OnNewMessage(InstantMessage im)
        {
            if (!InvokeRequired)
            {
                log.InfoFormat("IMSG {0}:{1}", im.From, im.Message);

                if (_IMForms.ContainsKey(im.From.Name))
                {
                    IMForm imf = _IMForms[im.From.Name];
                    imf.NewIMMessage(im);
                }
                else
                {
                    IMForm imf = new IMForm(_toc);
                    imf.FormClosed += new FormClosedEventHandler(OnIMFormClosed);

                    lock (_IMForms)
                    {
                        _IMForms.Add(im.From.Name, imf);
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsExiting = true;
            _toc.Disconnect();
            Application.Exit();
        }

        private void logOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _toc.Disconnect();
            this.Hide();
            this.Dispose();
        }
        
        private void buddyTree_DoubleClick(object sender, EventArgs e)
        {
            TreeView tv = sender as TreeView;

            if (tv == null)
            {
                log.Error("Object not a TreeView!");
                return;
            }

            TreeNode snode = tv.SelectedNode;
            if (snode != null)
            {
                if (snode.Tag is Buddy)
                {
                    if (_IMForms.ContainsKey(snode.Name))
                    {
                        IMForm form = _IMForms[snode.Name];
                        form.BringToFront();
                    }
                    else
                    {
                        IMForm nf = new IMForm(_toc, snode.Name);
                        nf.FormClosed += new FormClosedEventHandler(OnIMFormClosed);

                        _IMForms.Add(snode.Name, nf);
                        nf.Show();
                    }
                }
            }
        }
    }
}
