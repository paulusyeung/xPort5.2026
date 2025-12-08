#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
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
            // Use ViewService instead of direct SQL query
            DataSet ds = ViewService.Default.GetCategoryList("", "DeptName");
            DataTable dt = ds.Tables[0];

            // Get distinct departments
            var depts = dt.AsEnumerable()
                .Select(row => new { 
                    DeptId = row.Field<Guid>("DeptId"), 
                    DeptName = row.Field<string>("DeptName") 
                })
                .Distinct()
                .OrderBy(d => d.DeptName);

            foreach (var dept in depts)
            {
                TreeNode oNode = new TreeNode();

                oNode.Tag = dept.DeptId;
                oNode.Label = dept.DeptName.ToUpper();
                //oNode.Image = new IconResourceHandle("16x16.group.png");
                //oNode.ExpandedImage = new IconResourceHandle("16x16.group.png");
                oNode.IsExpanded = false;

                target.Add(oNode);
                LoadClass(oNode, dt);
            }
        }

        private void LoadClass(TreeNode oNodes, DataTable dt)
        {
            Guid deptId = (Guid)oNodes.Tag;
            
            // Filter classes for this department
            var classes = dt.AsEnumerable()
                .Where(row => row.Field<Guid>("DeptId") == deptId)
                .Select(row => new { 
                    ClassId = row.Field<Guid>("ClassId"), 
                    ClassName = row.Field<string>("ClassName") 
                })
                .Distinct()
                .OrderBy(c => c.ClassName);

            foreach (var cls in classes)
            {
                TreeNode oNode = new TreeNode();

                oNode.Tag = cls.ClassId;
                oNode.Label = cls.ClassName.ToUpper();
                //oNode.Image = new IconResourceHandle("16x16.group.png");
                //oNode.ExpandedImage = new IconResourceHandle("16x16.group.png");
                oNode.IsExpanded = false;

                oNodes.Nodes.Add(oNode);
                LoadCategory(oNode, dt);
            }
        }

        private void LoadCategory(TreeNode oNodes, DataTable dt)
        {
            Guid deptId = (Guid)oNodes.Parent.Tag;
            Guid classId = (Guid)oNodes.Tag;
            
            // Filter categories for this department and class
            var categories = dt.AsEnumerable()
                .Where(row => row.Field<Guid>("DeptId") == deptId && row.Field<Guid>("ClassId") == classId)
                .Select(row => new { 
                    CategoryId = row.Field<Guid>("CategoryId"), 
                    CategoryName = row.Field<string>("CategoryName") ?? "" 
                })
                .OrderBy(c => c.CategoryName);

            foreach (var category in categories)
            {
                TreeNode oNode = new TreeNode();

                oNode.Tag = category.CategoryId;
                oNode.Label = category.CategoryName;
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
