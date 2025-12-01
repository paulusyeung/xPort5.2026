using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Web.Security;
using System.Text;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Forms.Authentication;
using Gizmox.WebGUI.Common.Interfaces;

using xPort5.EF6;
using xPort5.Common;

namespace xPort5.Public
{
    public partial class Logon : LogonForm
    {
        public Logon()
        {
            InitializeComponent();
            SetCaptions();
            VersionNumber();

#if (DEBUG)
            txtUserName.Text = "admin";
            txtPassword.Text = "82279602";
#endif
        }

        private void SetCaptions()
        {
            nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(DAL.Common.Config.CurrentWordDict, DAL.Common.Config.CurrentLanguageId);

            lblUserName.Text = oDict.GetWordWithColon("logon_user");
            lblPassword.Text = oDict.GetWordWithColon("password");
            btnLogon.Text = oDict.GetWord("logon");
        }

        private void VersionNumber()
        {
            System.Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            this.lblVersionNumber.Text = version.ToString();
        }

        public bool Verify()
        {
            bool result = true;
            if (txtUserName.Text.Length == 0)
            {
                errorProvider.SetError(txtUserName, "Cannot be blank!");
            }
            else
            {
                errorProvider.SetError(txtUserName, string.Empty);
                result = result & true;
            }

            if (txtPassword.Text.Length == 0)
            {
                errorProvider.SetError(txtPassword, "Cannot be blank!");
            }
            else
            {
                errorProvider.SetError(txtPassword, string.Empty);
                result = result & true;
            }

            return result;
        }

        private bool AuthLogon()
        {
            if (Verify())
            {
                string sql = "LoginName = N'" + txtUserName.Text.Trim().Replace("'", "") + "' AND LoginPassword = '" + txtPassword.Text.Trim().Replace("'", "") + "'";
                xPort5.EF6.UserProfile oUser = xPort5.EF6.UserProfile.LoadWhere(sql);
                if (oUser != null)
                {
                    this.Context.Session.IsLoggedOn = true;

                    Common.Config.CurrentUserId = oUser.UserSid;
                    Common.Config.CurrentUserType = oUser.UserType ?? 0;

                    // The below code will logout the loggedin user when idle for the time specified
                    if (ConfigurationManager.AppSettings["sessionTimeout"] != null)
                    {
                        this.Context.HttpContext.Session.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["sessionTimeout"]);
                    }

                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Login, this.ToString());
                }
                else
                {
                    // When user inputs incorrect staff number or password, prompt user the error message.
                    // To Do: We can try to limited the times of attempt to 5 or less.
                    this.lblErrorMessage.Text = "Incorrect User Name or Password...Please try again!";
                    this.Context.Session.IsLoggedOn = false;
                }
            }
            else
            {
                this.Context.Session.IsLoggedOn = false;
            }

            return this.Context.Session.IsLoggedOn;
        }

        private void btnLogon_Click(object sender, EventArgs e)
        {
            DoLogon();
        }

        private void Logon_Load(object sender, EventArgs e)
        {
            txtUserName.Focus();
        }

        private void txtPassword_EnterKeyDown(object sender, KeyEventArgs e)
        {
            DoLogon();
        }

        private void txtUserName_EnterKeyDown(object sender, KeyEventArgs e)
        {
            DoLogon();
        }

        private void DoLogon()
        {
            if (AuthLogon())
            {
                // Close the Logon form
                this.Close();
            }
        }
    }
}
