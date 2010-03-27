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

        UserConfig _config;
        Dictionary<string, IMForm> _IMForms;
        TOC _toc = null;

        public BuddyListForm(TOC tocObj)
        {
            InitializeComponent();
            _IMForms = new Dictionary<string, IMForm>();

            _toc = tocObj;
            _toc.OnIMIn += new TOC.OnIMInHandler(OnNewMessage);
            _toc.OnUpdateBuddy += new TOC.OnUpdateBubbyHandler(_toc_OnUpdateBuddy);
        }

        void _toc_OnUpdateBuddy(Buddy buddy)
        {
            UpdateBuddy(buddy);
        }

        private delegate void UpdateBuddyHandler(Buddy buddy);
        private void UpdateBuddy(Buddy buddy)
        {
            if (!InvokeRequired)
            {
                //TreeNode[] buddyNodes = buddyTree.Nodes.Find(buddy.Name, true);

                // search the buddlist. 
                var q = from n in buddyTree.Nodes.Find(buddy.Name, true)
                        where n.Tag is Buddy && n.Name == buddy.NormalizedName 
                        select n;

                // the buddy exists on the list
                if (q.Count() > 0)
                {
                    foreach (TreeNode currentNode in q)
                    {
                        if (!buddy.Online)
                        {
                            currentNode.Remove();
                        }
                        else
                        {
                            // TODO: update the buddy's display on the treeview
                            log.InfoFormat("Updating Buddy `{0}`", buddy.NormalizedName);
                        }
                    }
                }
                else if (buddy.Online)
                { // buddy is not on list and is now online

                    // check the config to see if we have info about this buddy in the config
                    foreach (string strKey in _config.BuddyList.Keys)
                    {
                        List<Buddy> bl = _config.BuddyList[strKey];

                        var q1 = from b in bl
                                where b.NormalizedName == buddy.NormalizedName
                                select b;

                        if (q1.Count() > 0)
                        {
                            // buddy can't be in the same group more than once
                            Buddy configBuddyObj = q1.Single() as Buddy;

                            //  unnecessary?
                            if (configBuddyObj != null)
                            {
                                // see if a node for the group exists in the treeview
                                TreeNode groupNode = null;
                                foreach (TreeNode node in buddyTree.Nodes)
                                {
                                    if (node.Text == strKey)
                                    {
                                        groupNode = node;
                                        break;
                                    }
                                }

                                if (groupNode == null)
                                {
                                    TreeNode tempNode = new TreeNode 
                                    { 
                                        Name = strKey, 
                                        Text = strKey,
                                        NodeFont = new Font(buddyTree.Font, FontStyle.Bold)
                                    };

                                    buddyTree.Nodes.Add(tempNode);
                                    groupNode = tempNode;
                                }

                                // add the buddy to the treeview node
                                groupNode.Nodes.Add(new TreeNode 
                                {
                                    Name = buddy.NormalizedName, 
                                    Text = buddy.Name, 
                                    Tag = buddy 
                                });
                            }

                            break;
                        }
                    }
                }
            }
            else
            {
                Invoke(new UpdateBuddyHandler(UpdateBuddy), new object[] { buddy } );
            }
        }

        private delegate void ProcessConfigHanlder(UserConfig config);
        public void ProcessConfig(UserConfig config)
        {
            if (!InvokeRequired)
            {
                //foreach (string strGroupName in config.BuddyList.Keys)
                //{
                //    TreeNode newnode = buddyTree.Nodes.Add(strGroupName);

                //    foreach (Buddy buddy in config.BuddyList[strGroupName])
                //    {
                //        TreeNode bNode = new TreeNode { Text = buddy.Name, Tag = buddy };
                //        newnode.Nodes.Add(bNode);
                //    }
                //}

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
