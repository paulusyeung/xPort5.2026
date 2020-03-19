#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Common.Resources;
using Gizmox.WebGUI.Forms;

#endregion

namespace xPort5.NavPane
{
    public partial class AdminNav : UserControl
    {
        public AdminNav()
        {
            InitializeComponent();

            NavPane.NavMenu.FillNavTree("Admin", this.navAdmin.Nodes);
        }

        private void navMemberMgmt_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Control[] controls = this.Form.Controls.Find("wspPane", true);
            if (controls.Length > 0)
            {
                Panel wspPane = (Panel)controls[0];
                wspPane.Text = navAdmin.SelectedNode.Text;
                //wspPane.BackColor = xPort5.Controls.Utility.Default.TopPanelBackgroundColor;
                wspPane.Controls.Clear();
                ShowWorkspace(ref wspPane, (string)navAdmin.SelectedNode.Tag);
                ShowAppToolStrip((string)navAdmin.SelectedNode.Tag);
            }
        }

        private void ShowWorkspace(ref Panel wspPane, string Tag)
        {
            if (!string.IsNullOrEmpty(Tag))
            {
                switch (Tag)
                {
                    #region Olap
                    case "Olap.SalesTurnover":
                        AddOlapPage(ref wspPane, "Admin/Olap/SalesTurnover_v5.aspx", "Customer", true, false);
                        break;
                    case "Olap.Top10Supplier":
                        AddOlapPage(ref wspPane, "Admin/Olap/Top10Suppliers_v5.aspx", "Supplier", true, false);
                        break;
                    case "Olap.OutstandingOrder":
                        AddOlapPage(ref wspPane, "Admin/Olap/OutstandingOrderSummary_v5.aspx", "Customer", false, false);
                        break;
                    case "Olap.InvoiceSummary":
                        AddOlapPage(ref wspPane, "Admin/Olap/InvoiceSummary_v5.aspx", "Customer", false, false);
                        break;
                    case "Olap.InvoiceByMonth":
                        AddOlapPage(ref wspPane, "Admin/Olap/InvoiceSummaryByMonth_v5.aspx", "Customer", true, false);
                        break;
                    case "Olap_SalesContract":
                        AddOlapPage(ref wspPane, "Admin/Olap/SalesContract_v5.aspx", "Customer", true, false);
                        break;
                    case "Olap.OutstandingProfit":
                        AddOlapPage(ref wspPane, "Admin/Olap/OutstandingProfitSummary_v5.aspx", "Customer", false, false);
                        break;
                    case "Olap.ShipmentSummary":
                        AddOlapPage(ref wspPane, "Admin/Olap/ShipmentSummary_v5.aspx", "Customer", true, true);
                        break;
                    #endregion

                    #region Coding
                    case "Coding.Address":
                        xPort5.Admin.Coding.Address.AddressList addrList = new xPort5.Admin.Coding.Address.AddressList();
                        addrList.DockPadding.All = 6;
                        addrList.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(addrList);
                        break;
                    case "Coding.Category":
                        xPort5.Admin.Coding.Category.CategoryList category = new xPort5.Admin.Coding.Category.CategoryList();
                        category.DockPadding.All = 6;
                        category.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(category);
                        break;
                    case "Coding.Charge":
                        xPort5.Admin.Coding.Charge.ChargeList charge = new xPort5.Admin.Coding.Charge.ChargeList();
                        charge.DockPadding.All = 6;
                        charge.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(charge);
                        break;
                    case "Coding.City":
                        xPort5.Admin.Coding.City.CityList city = new xPort5.Admin.Coding.City.CityList();
                        city.DockPadding.All = 6;
                        city.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(city);
                        break;
                    case "Coding.Class":
                        xPort5.Admin.Coding.Class.ClassList oClass = new xPort5.Admin.Coding.Class.ClassList();
                        oClass.DockPadding.All = 6;
                        oClass.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(oClass);
                        break;
                    case "ac_agegrading":
                        xPort5.Admin.Coding.AgeGrading.AgeGradingList ageGrading = new xPort5.Admin.Coding.AgeGrading.AgeGradingList();
                        ageGrading.DockPadding.All = 6;
                        ageGrading.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(ageGrading);
                        break;
                    case "Coding.Color":
                        xPort5.Admin.Coding.ColorTable.ColorList colorTable = new xPort5.Admin.Coding.ColorTable.ColorList();
                        colorTable.DockPadding.All = 6;
                        colorTable.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(colorTable);
                        break;
                    case "Coding.Country":
                        xPort5.Admin.Coding.Country.CountryList country = new xPort5.Admin.Coding.Country.CountryList();
                        country.DockPadding.All = 6;
                        country.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(country);
                        break;
                    case "Coding.Dept":
                        xPort5.Admin.Coding.Dept.DeptList dept = new xPort5.Admin.Coding.Dept.DeptList();
                        dept.DockPadding.All = 6;
                        dept.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(dept);
                        break;
                    case "Coding.JobTitle":
                        xPort5.Admin.Coding.JobTitle.JobTitleList jobTitleList = new xPort5.Admin.Coding.JobTitle.JobTitleList();
                        jobTitleList.DockPadding.All = 6;
                        jobTitleList.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(jobTitleList);
                        break;
                    case "Coding.Origin":
                        xPort5.Admin.Coding.Origin.OriginList origin = new xPort5.Admin.Coding.Origin.OriginList();
                        origin.DockPadding.All = 6;
                        origin.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(origin);
                        break;
                    case "Coding.Package":
                        xPort5.Admin.Coding.Package.PackageList package = new xPort5.Admin.Coding.Package.PackageList();
                        package.DockPadding.All = 6;
                        package.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(package);
                        break;
                    case "Coding.PaymentTerms":
                        xPort5.Admin.Coding.Terms.TermsList terms = new xPort5.Admin.Coding.Terms.TermsList();
                        terms.DockPadding.All = 6;
                        terms.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(terms);
                        break;
                    case "Coding.Port":
                        xPort5.Admin.Coding.Port.PortList port = new xPort5.Admin.Coding.Port.PortList();
                        port.DockPadding.All = 6;
                        port.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(port);
                        break;
                    case "Coding.Province":
                        xPort5.Admin.Coding.Province.ProvinceList province = new xPort5.Admin.Coding.Province.ProvinceList();
                        province.DockPadding.All = 6;
                        province.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(province);
                        break;
                    case "Coding.Region":
                        xPort5.Admin.Coding.Region.RegionList region = new xPort5.Admin.Coding.Region.RegionList();
                        region.DockPadding.All = 6;
                        region.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(region);
                        break;
                    case "Coding.Remarks":
                        xPort5.Admin.Coding.Remarks.RemarksList remarks = new xPort5.Admin.Coding.Remarks.RemarksList();
                        remarks.DockPadding.All = 6;
                        remarks.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(remarks);
                        break;
                    case "Coding.Salutation":
                        xPort5.Admin.Coding.Salutation.SalutationList salutation = new xPort5.Admin.Coding.Salutation.SalutationList();
                        salutation.DockPadding.All = 6;
                        salutation.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(salutation);
                        break;
                    case "Coding.Shippingmark":
                        xPort5.Admin.Coding.ShippingMark.ShippingMarkList shippingmark = new xPort5.Admin.Coding.ShippingMark.ShippingMarkList();
                        shippingmark.DockPadding.All = 6;
                        shippingmark.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(shippingmark);
                        break;
                    case "Coding.Uom":
                        xPort5.Admin.Coding.Uom.UomList uomList = new xPort5.Admin.Coding.Uom.UomList();
                        uomList.DockPadding.All = 6;
                        uomList.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(uomList);
                        break;
                    case "Coding.User":
                        xPort5.Admin.Coding.User.UserList userList = new xPort5.Admin.Coding.User.UserList();
                        userList.DockPadding.All = 6;
                        userList.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(userList);
                        break;
                    case "Coding.Staff":
                        xPort5.Admin.Coding.Staff.StaffList staffList = new xPort5.Admin.Coding.Staff.StaffList();
                        staffList.DockPadding.All = 6;
                        staffList.Dock = DockStyle.Fill;
                        wspPane.Controls.Add(staffList);
                        break;
                    #endregion
                }
            }
        }

        private void AddOlapPage(ref Panel wspPane, string pagePath, string treeviewData, bool showPeriodPage, bool withCurrencyList)
        {
            xPort5.Controls.Reporting.OlapViewerWithCriteria olapViewer = new xPort5.Controls.Reporting.OlapViewerWithCriteria();
            olapViewer.AspxPagePath = pagePath;
            olapViewer.TreeviewData = treeviewData;
            olapViewer.ShowPeriodTabPage = showPeriodPage;
            olapViewer.WithCurrencyList = withCurrencyList;
            olapViewer.DockPadding.All = 6;
            olapViewer.Dock = DockStyle.Fill;
            wspPane.Controls.Add(olapViewer);
        }

        #region Show private AppToolStrip
        private void ShowAppToolStrip(string Tag)
        {
            if (!string.IsNullOrEmpty(Tag))
            {
                Control[] controls = this.Form.Controls.Find("atsPane", true);
                if (controls.Length > 0)
                {
                    Panel atsPane = (Panel)controls[0];

                    //switch (Tag.ToLower())
                    //{
                    //    case "admin_staff":
                    //        xPort5.Admin.Staff.StaffListAts stafflistAts = new xPort5.Admin.Staff.StaffListAts();
                    //        stafflistAts.Dock = DockStyle.Fill;
                    //        atsPane.Controls.Clear();
                    //        atsPane.Controls.Add(stafflistAts);
                    //        break;
                    //}
                }
            }
        }
        #endregion
    }
}