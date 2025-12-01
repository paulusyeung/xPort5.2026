#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Forms;

using xPort5.EF6;
using xPort5.Common;

#endregion

namespace xPort5.Settings.Coding
{
    public partial class MigrateProductPicture : UserControl
    {
        public MigrateProductPicture()
        {
            InitializeComponent();
        }

        private bool MigratePicture(Article prod)
        {
            bool result = false;

            string oldPictureDirectory = @"C:\Shared\xPort5\Picture\";
            if (ConfigurationManager.AppSettings["OldPictureDirectory"] != null)
            {
                oldPictureDirectory = ConfigurationManager.AppSettings["OldPictureDirectory"];
            }
            string oldFilePath = Path.Combine(oldPictureDirectory, prod.ColorPattern.Replace(@"L:\eTrader\Picture\", ""));
            string newFilePath = xPort5.Controls.Utility.Resources.PictureFilePath(prod.ArticleId, Path.GetFileName(oldFilePath));

            if (File.Exists(oldFilePath))
            {
                if (!File.Exists(newFilePath))
                {
                    File.Copy(oldFilePath, newFilePath, true);

                    result = true;
                }
            }

            return result;
        }

        private void cmdMigrate_Click(object sender, EventArgs e)
        {
            gbResult.Visible = false;

            string where = "Status > 0";
            string[] orderBy = new string[] { "ArticleCode" };
            ArticleCollection products = Article.LoadCollection(where, orderBy, true);

            pbProgress.Maximum = products.Count;
            int counter = 0, iExist = 0, iMissing = 0;
            foreach (Article prod in products)
            {
                cmdMigrate.Text = prod.ArticleCode;
                pbProgress.Value = counter;
                this.Update();
                if (prod.ColorPattern != String.Empty)
                {
                    if (MigratePicture(prod))
                    {
                        SaveToResource(prod.ArticleId, Path.GetFileName(prod.ColorPattern));

                        iExist++;
                    }
                    else
                    {
                        iMissing++;
                    }

                    T_AgeGrading ageGrading = T_AgeGrading.Load(prod.AgeGradingId);
                    if (ageGrading != null)
                    {
                        prod.ColorPattern = ageGrading.AgeGradingName;
                        prod.Save();
                    }
                }

                counter++;
            }

            //MessageBox.Show(String.Format("Records processed: {0}", counter.ToString()));

            gbResult.Visible = true;
            txtExistedPicture.Text = iExist.ToString("n0");
            txtMissedPicture.Text = iMissing.ToString("n0");
        }

        private void SaveToResource(Guid productId, string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                Resources resc = new Resources();
                resc.Keyword = xPort5.Controls.Utility.Product.ProductCode(productId);
                resc.ContentType = (int)Common.Enums.ContentType.Image;
                resc.OriginalFileName = fileName;
                resc.SaveAsFileId = productId.ToString();
                resc.SaveAsFileName = fileName;
                resc.CreatedBy = Common.Config.CurrentUserId;
                resc.CreatedOn = DateTime.Now;
                resc.ModifiedBy = Common.Config.CurrentUserId;
                resc.ModifiedOn = DateTime.Now;

                resc.Save();

                if (!xPort5.Controls.Utility.Product.HasKeyPicture(productId))
                {
                    xPort5.Controls.Utility.Product.SaveKeyPicture(productId, resc.ResourcesId);
                }
            }
        }
    }
}