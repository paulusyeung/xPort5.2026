#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Common.Resources;
using Gizmox.WebGUI.Forms;

using xPort5.EF6;
using xPort5.Common;

#endregion

namespace xPort5.Controls
{
    public partial class CategoryComboBoxTreeForm : Form
    {
        private CategoryComboBoxTree_ComboBox mobjParent = null;

        public CategoryComboBoxTreeForm(CategoryComboBoxTree_ComboBox objParent)
        {
            mobjParent = objParent;

            this.Width = mobjParent.Width;
            this.Height = 200;

            CategoryComboBoxTree_TreeView objTreeView = new CategoryComboBoxTree_TreeView();
            objTreeView.Dock = DockStyle.Fill;

            LoadTree(objTreeView.Nodes);

            this.Controls.Add(objTreeView);

            objTreeView.BeforeSelect += new TreeViewCancelEventHandler(objTreeView_BeforeSelect);
            objTreeView.AfterSelect += new TreeViewEventHandler(objTreeView_AfterSelect);
        }

        private void objTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.HasNodes)
            {
                e.Cancel = true;
            }
        }

        private void objTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            mobjParent.Text = e.Node.Text;
            mobjParent.Tag = e.Node.Tag;
            this.Close();
        }

        private void LoadTree(TreeNodeCollection target)
        {
            string sql = @"
SELECT DISTINCT [DeptId], [DeptName]
FROM [dbo].[vwCategoryList]
ORDER BY [DeptName]
";
            SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql);

            while (reader.Read())
            {
                TreeNode oNode = new TreeNode();

                oNode.Tag = reader.GetGuid(0);
                oNode.Label = reader.GetString(1).ToUpper();
                //oNode.Image = new IconResourceHandle("16x16.group.png");
                //oNode.ExpandedImage = new IconResourceHandle("16x16.group.png");
                oNode.IsExpanded = false;

                target.Add(oNode);
                LoadClass(oNode);
            }
        }

        private void LoadClass(TreeNode oNodes)
        {
            string sql = String.Format(@"
SELECT DISTINCT [ClassId], [ClassName]
  FROM [dbo].[vwCategoryList]
  WHERE [DeptId] = '{0}'
  ORDER BY [ClassName]
", oNodes.Tag);
            SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql);

            while (reader.Read())
            {
                TreeNode oNode = new TreeNode();

                oNode.Tag = reader.GetGuid(0);
                oNode.Label = reader.GetString(1).ToUpper();
                //oNode.Image = new IconResourceHandle("16x16.group.png");
                //oNode.ExpandedImage = new IconResourceHandle("16x16.group.png");
                oNode.IsExpanded = false;

                oNodes.Nodes.Add(oNode);
                LoadCategory(oNode);
            }
        }

        private void LoadCategory(TreeNode oNodes)
        {
            string sql = String.Format(@"
SELECT [CategoryId]
      ,ISNULL([CategoryName], '')
FROM [dbo].[vwCategoryList]
WHERE [DeptId] = '{0}' AND [ClassId] = '{1}'
ORDER BY [CategoryName]
", oNodes.Parent.Tag, oNodes.Tag);
            SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql);

            while (reader.Read())
            {
                TreeNode oNode = new TreeNode();

                oNode.Tag = reader.GetGuid(0);
                oNode.Label = reader.GetString(1);
                //oNode.Image = new IconResourceHandle("16x16.group.png");
                oNode.IsExpanded = false;

                oNodes.Nodes.Add(oNode);
            }
        }
    }

    //[Serializable()]
    public class CategoryComboBoxTree_ComboBox : ComboBox
    {
        private CategoryComboBoxTreeForm mobjDropDown = null;

        public CategoryComboBoxTree_ComboBox()
        {
            this.DropDownStyle = ComboBoxStyle.DropDown;
        }

        protected override Form GetCustomDropDown()
        {
            if (mobjDropDown == null)
            {
                mobjDropDown = new CategoryComboBoxTreeForm(this);
            }

            return mobjDropDown;
        }

        protected override bool IsCustomDropDown
        {
            get
            {
                return true;
            }
        }
    }

    //[Serializable()]
    public class CategoryComboBoxTree_TreeView : TreeView
    {
        public CategoryComboBoxTree_TreeView()
        {

        }

        protected override bool IsDelayedDrawing
        {
            get
            {
                return false;
            }
        }
    }
}
