#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Forms.Hosts;
using Gizmox.WebGUI.Common.Resources;

#endregion

namespace xPort5.Controls.Reporting
{
    public partial class OlapViewer : UserControl
    {
        OlapViewerBox objOlapViewerBox = new OlapViewerBox();

        public OlapViewer()
        {
            InitializeComponent();

            objOlapViewerBox.Dock = DockStyle.Fill;
            objOlapViewerBox.BackgroundImage = new ImageResourceHandle("loading.gif");
            objOlapViewerBox.BackgroundImageLayout = ImageLayout.None;      // 唔可以居中，有機會穿崩

            this.Controls.Add(objOlapViewerBox);
        }

        private void OlapViewer_Load(object sender, EventArgs e)
        {
            objOlapViewerBox.BackColor = Color.White;
            objOlapViewerBox.Path = this.AspxPagePath;
        }

        #region Variables

        private string aspxPagePath = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the aspx page.
        /// </summary>
        /// <value>The name of the aspx page.</value>
        public string AspxPagePath
        {
            get
            {
                return aspxPagePath;
            }
            set
            {
                aspxPagePath = value;
            }
        }

        #endregion
    }

    public class OlapViewerBox : AspPageBox
    {
        //protected override void FireEvent(Gizmox.WebGUI.Common.Interfaces.IEvent objEvent)
        //{
        //    base.FireEvent(objEvent);

        //    if (objEvent.Type == "MessageBox")
        //    {
        //        MessageBox.Show(objEvent["message"]);
        //    }
        //}
    }
}