using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace tccdotnet
{
    /// <summary>
    /// This class was modified and is now independent - Aivan
    /// </summary>
    public sealed class ParseTreeViewer
    {
        private ParseTreeViewer()
        {
        }

        public static void Populate(TreeView treeview, ParseTree parsetree)
        {
            treeview.Visible = false;
            treeview.SuspendLayout();
            treeview.Nodes.Clear();
            treeview.Tag = parsetree;

            ParseNode start = parsetree.Nodes[0];
            TreeNode node = new TreeNode(start.Text);
            node.Tag = start;
            node.ForeColor = Color.SteelBlue;
            treeview.Nodes.Add(node);

            PopulateNode(node, start);
            treeview.ExpandAll();
            treeview.ResumeLayout();
            treeview.Visible = true;
        }
            
        private static void PopulateNode(TreeNode node, ParseNode start)
        {
            foreach (ParseNode ipn in start.Nodes)
            {
                TreeNode tn = new TreeNode(ipn.Text);
                tn.Tag = ipn;
                node.Nodes.Add(tn);
                PopulateNode(tn, ipn);
            }
        }

    }
}
