using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Web;

using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Common.Resources;

using NetSqlAzMan;
using NetSqlAzMan.Interfaces;

using xPort5.DAL;

namespace xPort5.Controls
{
    public class Utility
    {
        /// <summary>
        /// Holding the properties for OLAP reportings
        /// </summary>
        public class OlapAdmin
        {

            /// <summary>
            /// Gets or sets the date period.
            /// </summary>
            /// <value>The date period.</value>
            public static string DatePeriod
            {
                get
                {
                    string datePeriod = string.Empty;
                    if (HttpContext.Current.Request.Cookies["xPort3_Admin_DatePeriod"] != null)
                    {
                        datePeriod = HttpContext.Current.Request.Cookies["xPort3_Admin_DatePeriod"].Value;
                    }
                    return datePeriod;
                }
                set
                {
                    System.Web.HttpCookie oCookie = new System.Web.HttpCookie("xPort3_Admin_DatePeriod");

                    if (value != string.Empty)
                    {
                        // create the cookie
                        DateTime now = DateTime.Now;

                        oCookie.Value = value.ToString();
                        oCookie.Expires = now.AddYears(1);

                        System.Web.HttpContext.Current.Response.Cookies.Add(oCookie);
                    }
                    else
                    {
                        // destory the cookie
                        DateTime now = DateTime.Now;

                        oCookie.Value = value.ToString();
                        oCookie.Expires = now.AddDays(-1);

                        System.Web.HttpContext.Current.Response.Cookies.Add(oCookie);
                    }
                }
            }

            /// <summary>
            /// Gets or sets the selected currency.
            /// </summary>
            /// <value>The selected currency.</value>
            public static string SelectedCurrency
            {
                get
                {
                    string datePeriod = string.Empty;
                    if (HttpContext.Current.Request.Cookies["xPort3_Admin_SelectedCurrency"] != null)
                    {
                        datePeriod = HttpContext.Current.Request.Cookies["xPort3_Admin_SelectedCurrency"].Value;
                    }
                    return datePeriod;
                }
                set
                {
                    System.Web.HttpCookie oCookie = new System.Web.HttpCookie("xPort3_Admin_SelectedCurrency");

                    if (value != string.Empty)
                    {
                        // create the cookie
                        DateTime now = DateTime.Now;

                        oCookie.Value = value.ToString();
                        oCookie.Expires = now.AddYears(1);

                        System.Web.HttpContext.Current.Response.Cookies.Add(oCookie);
                    }
                    else
                    {
                        // destory the cookie
                        DateTime now = DateTime.Now;

                        oCookie.Value = value.ToString();
                        oCookie.Expires = now.AddDays(-1);

                        System.Web.HttpContext.Current.Response.Cookies.Add(oCookie);
                    }
                }
            }
        }

        public class Customer
        {
            public static bool DeleteRec(Guid customerId)
            {
                bool result = false;

                xPort5.DAL.Customer item = xPort5.DAL.Customer.Load(customerId);
                if (item != null)
                {
                    switch ((int)item.Status)
                    {
                        case (int)Common.Enums.Status.Active:
                            item.Status = Convert.ToInt32(Common.Enums.Status.Inactive.ToString("d"));
                            item.Retired = true;
                            item.RetiredOn = DateTime.Now;
                            item.RetiredBy = Common.Config.CurrentUserId;
                            item.Save();

                            result = true;
                            break;

                        case (int)Common.Enums.Status.Draft:
                            xPort5.DAL.CustomerAddressCollection addresses = xPort5.DAL.CustomerAddress.LoadCollection(String.Format("CustomerId = '{0}'", customerId.ToString()));
                            if (addresses.Count > 0)
                            {
                                foreach (xPort5.DAL.CustomerAddress address in addresses)
                                {
                                    address.Delete();
                                }
                            }
                            xPort5.DAL.CustomerContactCollection contacts = xPort5.DAL.CustomerContact.LoadCollection(String.Format("CustomerId = '{0}'", customerId.ToString()));
                            if (contacts.Count > 0)
                            {
                                foreach (xPort5.DAL.CustomerContact contact in contacts)
                                {
                                    contact.Delete();
                                }
                            }
                            item.Delete();

                            result = true;
                            break;
                    }

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, item.ToString());
                }
                return result;
            }

            public static string NextCustomerCode()
            {
                string result = String.Empty;

                X_CounterCollection counter = X_Counter.LoadCollection();
                if (counter.Count > 0)
                {
                    result = counter[0].NextCustCode.ToString();
                    counter[0].NextCustCode = counter[0].NextCustCode + 1;
                    counter[0].Save();
                }

                return result;
            }

            public static string GetCustomerName(Guid customerId)
            {
                xPort5.DAL.Customer item = xPort5.DAL.Customer.Load(customerId);
                if (item != null)
                {
                    return item.CustomerName;
                }

                return string.Empty;
            }
        }

        public class CustomerAddress
        {
            public static bool DeleteRec(Guid cAddressId)
            {
                bool result = true;

                xPort5.DAL.CustomerAddress item = xPort5.DAL.CustomerAddress.Load(cAddressId);
                if (item != null)
                {
                    // 2009.09.25 paulus: 直接 delete 算了
                    /*
                    Staff user = Staff.Load(item.StaffId);
                    switch ((int)user.Status)
                    {
                        case (int)Common.Enums.Status.Active:
                            item.Retired = true;
                            item.RetiredOn = DateTime.Now;
                            item.RetiredBy = Common.Config.CurrentUserId;
                            item.Save();

                            result = true;
                            break;

                        case (int)Common.Enums.Status.Draft:
                            item.Delete();
                            if (item.DefaultRec)
                            {
                                // set the next StaffAddress record to DefaultRec
                                StaffAddressCollection uAddresses = StaffAddress.LoadCollection(String.Format("StaffId = '{0}'", item.StaffId.ToString()));
                                if (uAddresses.Count > 0)
                                {
                                    StaffAddress uAddress = uAddresses[0];
                                    uAddress.DefaultRec = true;
                                    uAddress.Save();
                                }
                            }

                            result = true;
                            break;
                    }
                    */
                    item.Delete();
                    if (item.DefaultRec)
                    {
                        // set the next CustomerAddress record to DefaultRec
                        xPort5.DAL.CustomerAddressCollection uAddresses = xPort5.DAL.CustomerAddress.LoadCollection(String.Format("CustomerId = '{0}'", item.CustomerId.ToString()));
                        if (uAddresses.Count > 0)
                        {
                            xPort5.DAL.CustomerAddress uAddress = uAddresses[0];
                            uAddress.DefaultRec = true;
                            uAddress.Save();
                        }
                    }

                    result = true;

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, item.ToString());
                }
                return result;
            }

            public static int Count(Guid userId)
            {
                xPort5.DAL.CustomerAddressCollection items = xPort5.DAL.CustomerAddress.LoadCollection(String.Format("CustomerId = '{0}'", userId.ToString()));
                return items.Count;
            }
        }

        public class CustomerContact
        {
            public static bool DeleteRec(Guid cAddressId)
            {
                bool result = true;

                xPort5.DAL.CustomerContact item = xPort5.DAL.CustomerContact.Load(cAddressId);
                if (item != null)
                {
                    // 2009.09.25 paulus: 直接 delete 算了
                    /*
                    Staff user = Staff.Load(item.StaffId);
                    switch ((int)user.Status)
                    {
                        case (int)Common.Enums.Status.Active:
                            item.Retired = true;
                            item.RetiredOn = DateTime.Now;
                            item.RetiredBy = Common.Config.CurrentUserId;
                            item.Save();

                            result = true;
                            break;

                        case (int)Common.Enums.Status.Draft:
                            item.Delete();
                            if (item.DefaultRec)
                            {
                                // set the next StaffAddress record to DefaultRec
                                StaffAddressCollection uAddresses = StaffAddress.LoadCollection(String.Format("StaffId = '{0}'", item.StaffId.ToString()));
                                if (uAddresses.Count > 0)
                                {
                                    StaffAddress uAddress = uAddresses[0];
                                    uAddress.DefaultRec = true;
                                    uAddress.Save();
                                }
                            }

                            result = true;
                            break;
                    }
                    */
                    item.Delete();
                    if (item.DefaultRec)
                    {
                        // set the next CustomerContact record to DefaultRec
                        xPort5.DAL.CustomerContactCollection contacts = xPort5.DAL.CustomerContact.LoadCollection(String.Format("CustomerId = '{0}'", item.CustomerId.ToString()));
                        if (contacts.Count > 0)
                        {
                            xPort5.DAL.CustomerContact contact = contacts[0];
                            contact.DefaultRec = true;
                            contact.Save();
                        }
                    }

                    result = true;

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, item.ToString());
                }
                return result;
            }

            public static int Count(Guid userId)
            {
                xPort5.DAL.CustomerContactCollection items = xPort5.DAL.CustomerContact.LoadCollection(String.Format("CustomerId = '{0}'", userId.ToString()));
                return items.Count;
            }
        }

        public class Product
        {
            public static bool DeleteRec(Guid productId)
            {
                bool result = false;

                Article item = Article.Load(productId);
                if (item != null)
                {
                    switch ((int)item.Status)
                    {
                        case (int)Common.Enums.Status.Active:
                            item.Status = Convert.ToInt32(Common.Enums.Status.Inactive.ToString("d"));
                            item.Retired = true;
                            item.RetiredOn = DateTime.Now;
                            item.RetiredBy = Common.Config.CurrentUserId;
                            item.Save();

                            result = true;
                            break;

                        case (int)Common.Enums.Status.Draft:
                            xPort5.Controls.Utility.Resources.DeletePictures(productId);
                            ArticlePackageCollection packages = ArticlePackage.LoadCollection(String.Format("ArticleId = '{0}'", productId.ToString()));
                            if (packages.Count > 0)
                            {
                                foreach (ArticlePackage package in packages)
                                {
                                    package.Delete();
                                }
                            }
                            ArticleSupplierCollection suppliers = ArticleSupplier.LoadCollection(String.Format("ArticleId = '{0}'", productId.ToString()));
                            if (suppliers.Count > 0)
                            {
                                foreach (ArticleSupplier supplier in suppliers)
                                {
                                    supplier.Delete();
                                }
                            }
                            item.Delete();

                            result = true;
                            break;
                    }

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, item.ToString());
                }
                return result;
            }

            public static bool ApproveRec(Guid productId)
            {
                bool result = false;

                Article item = Article.Load(productId);
                if (item != null)
                {
                    item.Status = (int)Common.Enums.Status.Active;
                    item.Save();

                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Approve, item.ToString());

                    result = true;
                }

                return result;
            }

            public static string NextSKU()
            {
                string result = String.Empty;

                X_CounterCollection counter = X_Counter.LoadCollection();
                if (counter.Count > 0)
                {
                    result = counter[0].NextSKUCode.ToString();
                    counter[0].NextSKUCode = counter[0].NextSKUCode + 1;
                    counter[0].Save();
                }

                return result;
            }

            public static string NextProductCode()
            {
                string result = String.Empty;

                X_CounterCollection counter = X_Counter.LoadCollection();
                if (counter.Count > 0)
                {
                    result = counter[0].NextArticleCode.ToString();
                    counter[0].NextArticleCode = counter[0].NextArticleCode + 1;
                    counter[0].Save();
                }

                return result;
            }

            public static string ProductCode(Guid productId)
            {
                string result = String.Empty;

                Article product = Article.Load(productId);
                if (product != null)
                {
                    result = product.ArticleCode;
                }

                return result;
            }

            public static string[] ProductCodeWithColor(Guid productId)
            {
                List<string> result = new List<string>();

                Article product = Article.Load(productId);
                if (product != null)
                {
                    result.Add(product.ArticleCode);
                    result.Add(product.ColorPattern);
                }

                return result.ToArray();
            }

            public static string KeyPicture(Guid productId)
            {
                string sql = "ArticleId = '" + productId.ToString() + "'";
                ArticleKeyPicture keyPic = ArticleKeyPicture.LoadWhere(sql);
                if (keyPic != null)
                {
                    xPort5.DAL.Resources resc = xPort5.DAL.Resources.Load(keyPic.ResourcesId);
                    if (resc != null)
                    {
                        return resc.OriginalFileName;
                    }
                }

                return string.Empty;
            }

            public static bool HasKeyPicture(Guid productId)
            {
                string sql = "ArticleId = '" + productId.ToString() + "'";
                ArticleKeyPictureCollection keyPicList = ArticleKeyPicture.LoadCollection(sql);

                return (keyPicList.Count > 0);
            }

            public static bool IsKeyPicture(Guid productId, Guid resourceId)
            {
                string sql = "ArticleId = '" + productId.ToString() + "' AND ResourcesId = '" + resourceId.ToString() + "'";
                ArticleKeyPicture keyPic = ArticleKeyPicture.LoadWhere(sql);

                return (keyPic != null);
            }

            public static bool DeleteKeyPicture(Guid productId)
            {
                return DeleteKeyPicture(productId, Guid.Empty);
            }

            public static bool DeleteKeyPicture(Guid productId, Guid resourceId)
            {
                bool result = false;

                string sql = "ArticleId = '" + productId.ToString() + "'";

                if (resourceId != Guid.Empty)
                {
                    sql += " AND ResourcesId = '" + resourceId.ToString() + "'";
                }

                ArticleKeyPicture keyPic = ArticleKeyPicture.LoadWhere(sql);

                if (keyPic != null)
                {
                    keyPic.Delete();
                    result = true;
                }

                return result;
            }

            public static string GetCategoryName(Guid productId)
            {
                Article product = Article.Load(productId);
                if (product != null)
                {
                    return Category.GetCategoryName(product.CategoryId);
                }

                return string.Empty;
            }

            public static void SaveKeyPicture(Guid productId, Guid resourceId)
            {
                string sql = "ArticleId = '" + productId.ToString() + "' AND ResourcesId = '" + resourceId.ToString() + "'";
                ArticleKeyPicture keyPic = ArticleKeyPicture.LoadWhere(sql);
                if (keyPic == null)
                {
                    keyPic = new ArticleKeyPicture();
                    keyPic.ArticleId = productId;
                    keyPic.ResourcesId = resourceId;
                    keyPic.Save();
                }
                else
                {
                    keyPic.Delete();
                }
            }

            #region Split product by colors

            public static void SplitProductWithColors(Guid srcProdId, string[] colors)
            {
                for (int i = 0; i < colors.Length; i++)
                {
                    string sku = NextSKU();
                    Guid newProdId = CreateNewProductWithColor(srcProdId, sku, colors[i]);

                    if (newProdId != System.Guid.Empty)
                    {
                        string srcQuery = string.Format("ArticleId = '{0}'", srcProdId.ToString());

                        CreateProductCustomer(srcQuery, newProdId);
                        CreateProductKeyPicture(srcQuery, newProdId);
                        CreateProductPackage(srcQuery, newProdId);
                        CreateProductPrice(srcQuery, sku, newProdId);
                        CreateProductSupplier(srcQuery, newProdId);
                    }
                }
            }

            private static Guid CreateNewProductWithColor(Guid srcProdId, string sku, string colorPattern)
            {
                Guid result = System.Guid.Empty;

                Article srcProd = Article.Load(srcProdId);
                if (srcProd != null)
                {
                    Article newProd = new Article();
                    newProd.SKU = sku;
                    newProd.ArticleCode = srcProd.ArticleCode;
                    newProd.ArticleName = srcProd.ArticleName;
                    newProd.ArticleName_Chs = srcProd.ArticleName_Chs;
                    newProd.ArticleName_Cht = srcProd.ArticleName_Cht;
                    newProd.CategoryId = srcProd.CategoryId;
                    newProd.AgeGradingId = srcProd.AgeGradingId;

                    T_Color color = T_Color.LoadWhere(string.Format("ColorName = '{0}'", colorPattern));
                    if (color != null)
                    {
                        newProd.ColorId = color.ColorId;
                    }

                    newProd.OriginId = srcProd.OriginId;
                    newProd.Remarks = srcProd.Remarks;
                    newProd.ColorPattern = colorPattern;
                    newProd.Barcode = srcProd.Barcode;
                    newProd.UnitCost = srcProd.UnitCost;
                    newProd.CurrencyId = srcProd.CurrencyId;
                    newProd.Status = srcProd.Status;
                    newProd.CreatedOn = DateTime.Now;
                    newProd.CreatedBy = Common.Config.CurrentUserId;
                    newProd.ModifiedOn = DateTime.Now;
                    newProd.ModifiedBy = Common.Config.CurrentUserId;
                    newProd.Retired = srcProd.Retired;
                    newProd.RetiredOn = srcProd.RetiredOn;
                    newProd.RetiredBy = srcProd.RetiredBy;

                    newProd.Save();

                    result = newProd.ArticleId;
                }

                return result;
            }

            private static void CreateProductCustomer(string srcQuery, Guid newProdId)
            {
                ArticleCustomerCollection srcCustList = ArticleCustomer.LoadCollection(srcQuery);
                foreach (ArticleCustomer srcCust in srcCustList)
                {
                    if (srcCust != null)
                    {
                        ArticleCustomer newCust = new ArticleCustomer();

                        newCust.ArticleId = newProdId;
                        newCust.CustomerId = srcCust.CustomerId;
                        newCust.DefaultRec = srcCust.DefaultRec;
                        newCust.CustRef = srcCust.CustRef;
                        newCust.CurrencyId = srcCust.CurrencyId;
                        newCust.FCLPrice = srcCust.FCLPrice;
                        newCust.LCLPrice = srcCust.LCLPrice;
                        newCust.UnitPrice = srcCust.UnitPrice;
                        newCust.DateRevised = srcCust.DateRevised;
                        newCust.Notes = srcCust.Notes;
                        newCust.CreatedOn = DateTime.Now;
                        newCust.CreatedBy = Common.Config.CurrentUserId;
                        newCust.ModifiedOn = DateTime.Now;
                        newCust.ModifiedBy = Common.Config.CurrentUserId;
                        newCust.Retired = srcCust.Retired;
                        newCust.RetiredOn = srcCust.RetiredOn;
                        newCust.RetiredBy = srcCust.RetiredBy;

                        newCust.Save();
                    }
                }
            }

            private static void CreateProductKeyPicture(string srcQuery, Guid newProdId)
            {
                ArticleKeyPictureCollection srcKeyPicList = ArticleKeyPicture.LoadCollection(srcQuery);
                foreach (ArticleKeyPicture srcKeyPic in srcKeyPicList)
                {
                    if (srcKeyPic != null)
                    {
                        ArticleKeyPicture newKeyPic = new ArticleKeyPicture();
                        newKeyPic.ArticleId = newProdId;
                        newKeyPic.ResourcesId = srcKeyPic.ResourcesId;
                        newKeyPic.Save();
                    }
                }
            }

            private static void CreateProductPackage(string srcQuery, Guid newProdId)
            {
                ArticlePackageCollection srcPackageList = ArticlePackage.LoadCollection(srcQuery);
                foreach (ArticlePackage srcPackage in srcPackageList)
                {
                    if (srcPackage != null)
                    {
                        ArticlePackage newPackage = new ArticlePackage();
                        newPackage.ArticleId = newProdId;
                        newPackage.PackageId = srcPackage.PackageId;
                        newPackage.DefaultRec = srcPackage.DefaultRec;
                        newPackage.UomId = srcPackage.UomId;
                        newPackage.InnerBox = srcPackage.InnerBox;
                        newPackage.OuterBox = srcPackage.OuterBox;
                        newPackage.CUFT = srcPackage.CUFT;
                        newPackage.SizeLength_in = srcPackage.SizeLength_in;
                        newPackage.SizeWidth_in = srcPackage.SizeWidth_in;
                        newPackage.SizeHeight_in = srcPackage.SizeHeight_in;
                        newPackage.SizeLength_cm = srcPackage.SizeLength_cm;
                        newPackage.SizeWidth_cm = srcPackage.SizeWidth_cm;
                        newPackage.SizeHeight_cm = srcPackage.SizeHeight_cm;
                        newPackage.WeightGross_lb = srcPackage.WeightGross_lb;
                        newPackage.WeightNet_lb = srcPackage.WeightNet_lb;
                        newPackage.WeightGross_kg = srcPackage.WeightGross_kg;
                        newPackage.WeightNet_kg = srcPackage.WeightNet_kg;
                        newPackage.ContainerQty = srcPackage.ContainerQty;
                        newPackage.ContainerSize = srcPackage.ContainerSize;
                        newPackage.CreatedOn = DateTime.Now;
                        newPackage.CreatedBy = Common.Config.CurrentUserId;
                        newPackage.ModifiedOn = DateTime.Now;
                        newPackage.ModifiedBy = Common.Config.CurrentUserId;
                        newPackage.Retired = srcPackage.Retired;
                        newPackage.RetiredOn = srcPackage.RetiredOn;
                        newPackage.RetiredBy = srcPackage.RetiredBy;
                        newPackage.Save();
                    }
                }
            }

            private static void CreateProductPrice(string srcQuery, string sku, Guid newProdId)
            {
                ArticlePriceCollection srcPriceList = ArticlePrice.LoadCollection(srcQuery);
                foreach (ArticlePrice srcPrice in srcPriceList)
                {
                    if (srcPrice != null)
                    {
                        ArticlePrice newPrice = new ArticlePrice();
                        newPrice.ArticleId = newProdId;
                        newPrice.SKU = sku;
                        newPrice.CurrencyId = srcPrice.CurrencyId;
                        newPrice.DefaultRec = srcPrice.DefaultRec;
                        newPrice.FCLPrice = srcPrice.FCLPrice;
                        newPrice.LCLPrice = srcPrice.LCLPrice;
                        newPrice.UnitPrice = srcPrice.UnitPrice;
                        newPrice.Notes = srcPrice.Notes;
                        newPrice.CreatedOn = DateTime.Now;
                        newPrice.CreatedBy = Common.Config.CurrentUserId;
                        newPrice.ModifiedOn = DateTime.Now;
                        newPrice.ModifiedBy = Common.Config.CurrentUserId;
                        newPrice.Retired = srcPrice.Retired;
                        newPrice.RetiredOn = srcPrice.RetiredOn;
                        newPrice.RetiredBy = srcPrice.RetiredBy;
                        newPrice.Save();
                    }
                }
            }

            private static void CreateProductSupplier(string srcQuery, Guid newProdId)
            {
                ArticleSupplierCollection srcSupplierList = ArticleSupplier.LoadCollection(srcQuery);
                foreach (ArticleSupplier srcSupplier in srcSupplierList)
                {
                    if (srcSupplier != null)
                    {
                        ArticleSupplier newSupplier = new ArticleSupplier();
                        newSupplier.ArticleId = newProdId;
                        newSupplier.SupplierId = srcSupplier.SupplierId;
                        newSupplier.DefaultRec = srcSupplier.DefaultRec;
                        newSupplier.SuppRef = srcSupplier.SuppRef;
                        newSupplier.CurrencyId = srcSupplier.CurrencyId;
                        newSupplier.FCLCost = srcSupplier.FCLCost;
                        newSupplier.LCLCost = srcSupplier.LCLCost;
                        newSupplier.UnitCost = srcSupplier.UnitCost;
                        newSupplier.Notes = srcSupplier.Notes;
                        newSupplier.CreatedOn = DateTime.Now;
                        newSupplier.CreatedBy = Common.Config.CurrentUserId;
                        newSupplier.ModifiedOn = DateTime.Now;
                        newSupplier.ModifiedBy = Common.Config.CurrentUserId;
                        newSupplier.Retired = srcSupplier.Retired;
                        newSupplier.RetiredOn = srcSupplier.RetiredOn;
                        newSupplier.RetiredBy = srcSupplier.RetiredBy;
                        newSupplier.Save();
                    }
                }
            }
            #endregion

            public static String GetColor(Guid productId)
            {
                String result = String.Empty;

                Article product = Article.Load(productId);
                if (product != null)
                {
                    T_Color color = T_Color.Load(product.ColorId);
                    if (color != null)
                    {
                        result = color.ColorName;
                    }
                    else
                    {
                        result = product.ColorPattern;
                    }
                }

                return result;
            }
        }

        public class ProductSupplier
        {
            public static bool DeleteRec(Guid prodSupplierId)
            {
                bool result = true;

                ArticleSupplier item = ArticleSupplier.Load(prodSupplierId);
                if (item != null)
                {
                    Article prod = Article.Load(item.ArticleId);
                    switch ((int)prod.Status)
                    {
                        case (int)Common.Enums.Status.Active:
                            item.Retired = true;
                            item.RetiredOn = DateTime.Now;
                            item.RetiredBy = Common.Config.CurrentUserId;
                            item.Save();

                            result = true;
                            break;

                        case (int)Common.Enums.Status.Draft:
                            item.Delete();
                            if (item.DefaultRec)
                            {
                                // set the next ArticleSupplier record to DefaultRec
                                ArticleSupplierCollection suppliers = ArticleSupplier.LoadCollection(String.Format("ArticleId = '{0}'", item.ArticleId.ToString()));
                                if (suppliers.Count > 0)
                                {
                                    ArticleSupplier aSupplier = suppliers[0];
                                    aSupplier.DefaultRec = true;
                                    aSupplier.Save();
                                }
                            }

                            result = true;
                            break;
                    }

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, item.ToString());
                }
                return result;
            }

            public static int Count(Guid productId)
            {
                ArticleSupplierCollection items = ArticleSupplier.LoadCollection(String.Format("ArticleId = '{0}'", productId.ToString()));
                return items.Count;
            }
        }

        public class ProductPackage
        {
            public static bool DeleteRec(Guid prodPackageId)
            {
                bool result = true;

                ArticlePackage item = ArticlePackage.Load(prodPackageId);
                if (item != null)
                {
                    Article prod = Article.Load(item.ArticleId);
                    switch ((int)prod.Status)
                    {
                        case (int)Common.Enums.Status.Active:
                            item.Retired = true;
                            item.RetiredOn = DateTime.Now;
                            item.RetiredBy = Common.Config.CurrentUserId;
                            item.Save();

                            result = true;
                            break;

                        case (int)Common.Enums.Status.Draft:
                            item.Delete();
                            if (item.DefaultRec)
                            {
                                // set the next ArticlePackage record to DefaultRec
                                ArticlePackageCollection packages = ArticlePackage.LoadCollection(String.Format("ArticleId = '{0}'", item.ArticleId.ToString()));
                                if (packages.Count > 0)
                                {
                                    ArticlePackage aPackage = packages[0];
                                    aPackage.DefaultRec = true;
                                    aPackage.Save();
                                }
                            }

                            result = true;
                            break;
                    }

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, item.ToString());
                }
                return result;
            }

            public static int Count(Guid productId)
            {
                ArticlePackageCollection items = ArticlePackage.LoadCollection(String.Format("ArticleId = '{0}'", productId.ToString()));
                return items.Count;
            }
        }

        public class Category
        {
            public static void LoadTree(ref TreeView target)
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
                    oNode.IsExpanded = false;

                    target.Nodes.Add(oNode);
                    LoadClass(ref oNode);
                }
            }

            private static void LoadClass(ref TreeNode oNodes)
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
                    oNode.IsExpanded = false;

                    oNodes.Nodes.Add(oNode);
                    LoadCategory(ref oNode);
                }
            }

            private static void LoadCategory(ref TreeNode oNodes)
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

            public static string NextCategoryCode()
            {
                string result = String.Empty;

                string sql = @"
SELECT  
        CONVERT(INT, [CategoryCode])
FROM    
        [dbo].[T_Category]
ORDER BY
        CONVERT(INT, [CategoryCode]) DESC;
";
                try
                {
                    using (SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql))
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            int lastCode = reader.GetInt32(0);
                            result = (lastCode + 1).ToString();
                        }
                    }
                }
                catch { }

                return result;
            }

            public static string GetCategoryCode(Guid categoryId)
            {
                T_Category cate = T_Category.Load(categoryId);
                if (cate != null)
                {
                    return cate.CategoryCode;
                }

                return string.Empty;
            }

            public static string GetCategoryName(Guid categoryId)
            {
                T_Category cate = T_Category.Load(categoryId);
                if (cate != null)
                {
                    return cate.CategoryName;
                }

                return string.Empty;
            }
        }

        public class Currency
        {
            public static Guid CurrencyUsed(string currencyCode)
            {
                Guid result = Guid.Empty;

                T_Currency currency = T_Currency.LoadWhere("CurrencyCode = '" + currencyCode.Trim() + "'");
                if (currency != null)
                {
                    result = currency.CurrencyId;
                }

                return result;
            }

            public static string CurrencyUsed(Guid currencyId)
            {
                string result = String.Empty;

                T_Currency currency = T_Currency.Load(currencyId);
                if (currency != null)
                {
                    result = currency.CurrencyCode;
                }

                return result;
            }

            public static Decimal XchgRate(String localAmount, String foreignAmount, int XchgBase)
            {
                Decimal result = 0;

                try
                {
                    result = XchgRate(Convert.ToDecimal(localAmount), Convert.ToDecimal(foreignAmount), XchgBase);
                }
                catch { }

                return result;
            }

            public static Decimal XchgRate(Decimal localAmount, Decimal foreignAmount, int XchgBase)
            {
                Decimal result = 0;

                if (XchgBase == 0)
                {
                    result = localAmount / foreignAmount;
                }
                else
                {
                    result = foreignAmount / localAmount;
                }

                return result;
            }

            public static Decimal GetXchgRate(Guid currencyId)
            {
                Decimal result = 0;

                T_Currency cny = T_Currency.Load(currencyId);
                if (cny != null)
                {
                    result = cny.XchgRate;
                }

                return result;
            }

            public static Decimal GetXchgRate(string currencyCode)
            {
                Decimal result = 0;

                T_Currency cny = T_Currency.LoadWhere(String.Format("CurrencyCode = '{0}'", currencyCode));
                if (cny != null)
                {
                    result = cny.XchgRate;
                }

                return result;
            }

            public static void ResetLocalCurrency()
            {
                string sql = @"
UPDATE [dbo].[T_Currency]
SET [LocalCurrency] = 0;";

                xPort5.DAL.SqlHelper.Default.ExecuteNonQuery(CommandType.Text, sql);
            }

            public static string GetCurrencyName(string currencyCode)
            {
                string result = String.Empty;

                T_Currency cny = T_Currency.LoadWhere(String.Format("CurrencyCode = '{0}'", currencyCode));
                if (cny != null)
                {
                    result = cny.CurrencyName;
                }

                return result;
            }
        }

        public class CheckedComboBox
        {
            public static void LoadColor(xPort5.Controls.CheckedComboBox comboBox)
            {
                // 2010.08.08 paulus: 改為 T_Color
                //                string sql = @"
                //SELECT DISTINCT [AgeGradingId], [AgeGradingName]
                //FROM [dbo].[T_AgeGrading]
                //ORDER BY [AgeGradingName]
                //";
                string sql = @"
SELECT DISTINCT [ColorId], [ColorName]
FROM [dbo].[T_Color]
ORDER BY [ColorName]
";
                SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, sql);
                comboBox.Items.Clear();

                while (reader.Read())
                {
                    comboBox.AddString(reader.GetString(1), false);
                }
            }
        }

        public class ComboBox
        {
            public static void LoadViews(ref Gizmox.WebGUI.Forms.ComboBox comboBox)
            {
                nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

                comboBox.Items.Clear();
                comboBox.Items.Add("");
                comboBox.Items.Add(oDict.GetWord("last_7_days"));
                comboBox.Items.Add(oDict.GetWord("last_30_days"));
                comboBox.Items.Add(oDict.GetWord("last_60_days"));
                comboBox.Items.Add(oDict.GetWord("last_90_days"));
                comboBox.SelectedIndex = 0;
            }

            public static void LoadMeasurement(ref Gizmox.WebGUI.Forms.ComboBox comboBox)
            {
                comboBox.Items.Clear();
                comboBox.Items.Add("CBM");
                comboBox.Items.Add("CUFT");
                comboBox.SelectedIndex = 1;
            }

            public static void LoadInputMask(ref Gizmox.WebGUI.Forms.ComboBox comboBox)
            {
                comboBox.Items.Clear();
                comboBox.Items.Add("1");
                comboBox.Items.Add("2");
                comboBox.Items.Add("3");
                comboBox.Items.Add("4");
                comboBox.Items.Add("5");
                comboBox.SelectedIndex = 4;
            }

            public static void LoadContainerSize(ref Gizmox.WebGUI.Forms.ComboBox comboBox)
            {
                comboBox.Items.Clear();
                comboBox.Items.Add("20'");
                comboBox.Items.Add("40'");
                comboBox.Items.Add("40' HQ");
                comboBox.Items.Add("45'");

                comboBox.SelectedIndex = 1;
            }

            public static void LoadProductPacking(ref Gizmox.WebGUI.Forms.ComboBox comboBox, Guid productId)
            {
                string where = String.Format("ArticleId = '{0}'", productId.ToString());
                string[] orderby = { "DefaultRec" };
                ArticlePackageCollection prodPackings = ArticlePackage.LoadCollection(where, orderby, false);
                if (prodPackings.Count > 0)
                {
                    comboBox.DataSource = null;
                    comboBox.Items.Clear();

                    string sql = String.Empty;
                    ComboList itemlist = new ComboList();
                    foreach (ArticlePackage prodPacking in prodPackings)
                    {
                        T_Package packing = T_Package.Load(prodPacking.PackageId);
                        if (packing != null)
                        {
                            itemlist.Add(new ComboItem(packing.PackageName, packing.PackageId));
                        }
                    }

                    comboBox.DataSource = itemlist;
                    comboBox.DisplayMember = "Code";
                    comboBox.ValueMember = "Id";
                    comboBox.SelectedIndex = 0;
                }
            }

            public static void LoadProductSupplier(ref Gizmox.WebGUI.Forms.ComboBox comboBox, Guid productId)
            {
                string where = String.Format("ArticleId = '{0}'", productId.ToString());
                string[] orderby = { "DefaultRec" };
                ArticleSupplierCollection prodSuppliers = ArticleSupplier.LoadCollection(where, orderby, false);
                if (prodSuppliers.Count > 0)
                {
                    comboBox.DataSource = null;
                    comboBox.Items.Clear();

                    string sql = String.Empty;
                    ComboList itemlist = new ComboList();
                    foreach (ArticleSupplier prodSupplier in prodSuppliers)
                    {
                        xPort5.DAL.Supplier supplier = xPort5.DAL.Supplier.Load(prodSupplier.SupplierId);
                        if (supplier != null)
                        {
                            itemlist.Add(new ComboItem(supplier.SupplierName, supplier.SupplierId));
                        }
                    }

                    comboBox.DataSource = itemlist;
                    comboBox.DisplayMember = "Code";
                    comboBox.ValueMember = "Id";
                    comboBox.SelectedIndex = 0;
                }
            }

            /// <summary>
            /// 2010.08.17 paulus: 第三代 T_Category (沒有 T_Dept 或 T_Class)
            /// </summary>
            /// <param name="comboBox"></param>
            public static void LoadCategory(ref Gizmox.WebGUI.Forms.ComboBox comboBox)
            {
                string where = "DeptId IS NULL AND ClassId IS NULL";
                string[] orderby = { "CategoryName" };
                T_CategoryCollection category = T_Category.LoadCollection(where, orderby, true);

                if (category.Count > 0)
                {
                    comboBox.Items.Clear();

                    string sql = String.Empty;
                    ComboList itemlist = new ComboList();
                    foreach (T_Category item in category)
                    {
                        itemlist.Add(new ComboItem(item.CategoryName, item.CategoryId));
                    }

                    comboBox.DataSource = itemlist;
                    comboBox.DisplayMember = "Code";
                    comboBox.ValueMember = "Id";
                    comboBox.SelectedIndex = 0;
                }
            }

            #region ComboBox Binding List
            public class ComboItem
            {
                private string _code = string.Empty;
                private Guid _id = Guid.Empty;

                public ComboItem(string code, Guid id)
                {
                    _code = code;
                    _id = id;
                }

                public string Code
                {
                    get
                    {
                        return _code;
                    }
                    set
                    {
                        _code = value;
                    }
                }

                public Guid Id
                {
                    get
                    {
                        return _id;
                    }
                    set
                    {
                        _id = value;
                    }
                }
            }

            public class ComboList : BindingList<ComboItem>
            {
            }

            #endregion
        }

        public class Default
        {
            public static void Currency(ref Gizmox.WebGUI.Forms.ComboBox target)
            {
                string defaultValue = "USD";
                if (ConfigurationManager.AppSettings["Currency"] != null)
                {
                    defaultValue = ConfigurationManager.AppSettings["Currency"];
                }
                int index = target.FindString(defaultValue, 0);
                if (index >= 0 && index < target.Items.Count)
                {
                    target.SelectedIndex = index;
                }
            }

            public static void UoM(ref Gizmox.WebGUI.Forms.ComboBox target)
            {
                string defaultValue = "PC";
                if (ConfigurationManager.AppSettings["UoM"] != null)
                {
                    defaultValue = ConfigurationManager.AppSettings["UoM"];
                }
                int index = target.FindString(defaultValue, 0);
                if (index >= 0 && index < target.Items.Count)
                {
                    target.SelectedIndex = index;
                }
            }

            public static void Salesperson(ref Gizmox.WebGUI.Forms.ComboBox target)
            {
                string defaultValue = "Clara";
                if (ConfigurationManager.AppSettings["Salesperson"] != null)
                {
                    defaultValue = ConfigurationManager.AppSettings["Salesperson"];
                }
                int index = target.FindString(defaultValue, 0);
                if (index >= 0 && index < target.Items.Count)
                {
                    target.SelectedIndex = index;
                }
            }

            public static void Origin(ref Gizmox.WebGUI.Forms.ComboBox target)
            {
                string defaultValue = "CHINA";
                if (ConfigurationManager.AppSettings["Origin"] != null)
                {
                    defaultValue = ConfigurationManager.AppSettings["Origin"];
                }
                int index = target.FindString(defaultValue, 0);
                if (index >= 0 && index < target.Items.Count)
                {
                    target.SelectedIndex = index;
                }
            }

            public static int InputMask()
            {
                int defaultValue = 5;
                if (ConfigurationManager.AppSettings["InputMask"] != null)
                {
                    defaultValue = Convert.ToInt32(ConfigurationManager.AppSettings["InputMask"]);
                }
                return defaultValue;
            }

            public static string[] PhoneLabels4User()
            {
                string[] result = new string[5];

                string label = "1,2,3,4,5";
                if (ConfigurationManager.AppSettings["PhoneLabels4User"] != null)
                {
                    label = ConfigurationManager.AppSettings["PhoneLabels4User"];
                }
                string[] labels = label.Split(',');

                for (int i = 0; i < 5; i++)
                {
                    Z_Phone phone = Z_Phone.LoadWhere(String.Format("PhoneCode = '{0}'", labels[i]));
                    if (phone != null)
                    {
                        result.SetValue(phone.PhoneName, i);
                    }
                }

                return result;
            }

            public static Guid[] PhoneLabelIds4User()
            {
                Guid[] result = new Guid[5];

                string label = "1,2,3,4,5";
                if (ConfigurationManager.AppSettings["PhoneLabels4User"] != null)
                {
                    label = ConfigurationManager.AppSettings["PhoneLabels4User"];
                }
                string[] labels = label.Split(',');

                for (int i = 0; i < 5; i++)
                {
                    Z_Phone phone = Z_Phone.LoadWhere(String.Format("PhoneCode = '{0}'", labels[i]));
                    if (phone != null)
                    {
                        result.SetValue(phone.PhoneId, i);
                    }
                }

                return result;
            }

            public static string[] PhoneLabels4Contact()
            {
                string[] result = new string[5];

                string label = "1,2,3,4,5";
                if (ConfigurationManager.AppSettings["PhoneLabels4Contact"] != null)
                {
                    label = ConfigurationManager.AppSettings["PhoneLabels4Contact"];
                }
                string[] labels = label.Split(',');

                for (int i = 0; i < 5; i++)
                {
                    Z_Phone phone = Z_Phone.LoadWhere(String.Format("PhoneCode = '{0}'", labels[i]));
                    if (phone != null)
                    {
                        result.SetValue(phone.PhoneName, i);
                    }
                }

                return result;
            }

            public static Guid[] PhoneLabelIds4Contact()
            {
                Guid[] result = new Guid[5];

                string label = "1,2,3,4,5";
                if (ConfigurationManager.AppSettings["PhoneLabels4Contact"] != null)
                {
                    label = ConfigurationManager.AppSettings["PhoneLabels4Contact"];
                }
                string[] labels = label.Split(',');

                for (int i = 0; i < 5; i++)
                {
                    Z_Phone phone = Z_Phone.LoadWhere(String.Format("PhoneCode = '{0}'", labels[i]));
                    if (phone != null)
                    {
                        result.SetValue(phone.PhoneId, i);
                    }
                }

                return result;
            }

            public static string[] PhoneLabels4Customer()
            {
                string[] result = new string[5];

                string label = "1,2,3,4,5";
                if (ConfigurationManager.AppSettings["PhoneLabels4Customer"] != null)
                {
                    label = ConfigurationManager.AppSettings["PhoneLabels4Customer"];
                }
                string[] labels = label.Split(',');

                for (int i = 0; i < 5; i++)
                {
                    Z_Phone phone = Z_Phone.LoadWhere(String.Format("PhoneCode = '{0}'", labels[i]));
                    if (phone != null)
                    {
                        result.SetValue(phone.PhoneName, i);
                    }
                }

                return result;
            }

            public static Guid[] PhoneLabelIds4Customer()
            {
                Guid[] result = new Guid[5];

                string label = "1,2,3,4,5";
                if (ConfigurationManager.AppSettings["PhoneLabels4Customer"] != null)
                {
                    label = ConfigurationManager.AppSettings["PhoneLabels4Customer"];
                }
                string[] labels = label.Split(',');

                for (int i = 0; i < 5; i++)
                {
                    Z_Phone phone = Z_Phone.LoadWhere(String.Format("PhoneCode = '{0}'", labels[i]));
                    if (phone != null)
                    {
                        result.SetValue(phone.PhoneId, i);
                    }
                }

                return result;
            }

            public static string[] PhoneLabels4Supplier()
            {
                string[] result = new string[5];

                string label = "1,2,3,4,5";
                if (ConfigurationManager.AppSettings["PhoneLabels4Supplier"] != null)
                {
                    label = ConfigurationManager.AppSettings["PhoneLabels4Supplier"];
                }
                string[] labels = label.Split(',');

                for (int i = 0; i < 5; i++)
                {
                    Z_Phone phone = Z_Phone.LoadWhere(String.Format("PhoneCode = '{0}'", labels[i]));
                    if (phone != null)
                    {
                        result.SetValue(phone.PhoneName, i);
                    }
                }

                return result;
            }

            public static Guid[] PhoneLabelIds4Supplier()
            {
                Guid[] result = new Guid[5];

                string label = "1,2,3,4,5";
                if (ConfigurationManager.AppSettings["PhoneLabels4Supplier"] != null)
                {
                    label = ConfigurationManager.AppSettings["PhoneLabels4Supplier"];
                }
                string[] labels = label.Split(',');

                for (int i = 0; i < 5; i++)
                {
                    Z_Phone phone = Z_Phone.LoadWhere(String.Format("PhoneCode = '{0}'", labels[i]));
                    if (phone != null)
                    {
                        result.SetValue(phone.PhoneId, i);
                    }
                }

                return result;
            }

            public static Color TopPanelBackgroundColor
            {
                get
                {
                    Color color = Color.FromName("#ACC0E9");    // default color
                    if (HttpContext.Current.Request.Cookies["xPort3_TopPanelBackgroundColor"] != null)
                    {
                        color = HttpContext.Current.Request.Cookies["xPort3_TopPanelBackgroundColor"].Value == String.Empty ?
                            Color.FromName("#ACC0E9") :
                            Color.FromName(HttpContext.Current.Request.Cookies["xPort3_TopPanelBackgroundColor"].Value);
                    }
                    return color;
                }
                set
                {
                    System.Web.HttpCookie oCookie = new System.Web.HttpCookie("xPort3_TopPanelBackgroundColor");

                    if (value != null)
                    {
                        // create the cookie
                        DateTime now = DateTime.Now;

                        oCookie.Value = value.ToString();
                        oCookie.Expires = now.AddYears(1);

                        System.Web.HttpContext.Current.Response.Cookies.Add(oCookie);
                    }
                    else
                    {
                        // destory the cookie
                        DateTime now = DateTime.Now;

                        oCookie.Value = value.ToString();
                        oCookie.Expires = now.AddDays(-1);

                        System.Web.HttpContext.Current.Response.Cookies.Add(oCookie);
                    }
                }
            }

            public static String CurrentTheme
            {
                get
                {
                    String theme = "";
                    if (HttpContext.Current.Request.Cookies["xPort3_CurrentTheme"] != null)
                    {
                        theme = HttpContext.Current.Request.Cookies["xPort3_CurrentTheme"].Value;
                    }
                    return theme == String.Empty ? "Vista" : theme;
                }
                set
                {
                    System.Web.HttpCookie oCookie = new System.Web.HttpCookie("xPort3_CurrentTheme");

                    if (value != null)
                    {
                        // create the cookie
                        DateTime now = DateTime.Now;

                        oCookie.Value = value.ToString() == String.Empty ? "Vista" : value.ToString();
                        oCookie.Expires = now.AddYears(1);

                        System.Web.HttpContext.Current.Response.Cookies.Add(oCookie);
                    }
                    else
                    {
                        // destory the cookie
                        DateTime now = DateTime.Now;

                        oCookie.Value = value.ToString();
                        oCookie.Expires = now.AddDays(-1);

                        System.Web.HttpContext.Current.Response.Cookies.Add(oCookie);
                    }
                }
            }
        }

        public class Supplier
        {
            public static bool DeleteRec(Guid supplierId)
            {
                bool result = false;

                xPort5.DAL.Supplier item = xPort5.DAL.Supplier.Load(supplierId);
                if (item != null)
                {
                    switch ((int)item.Status)
                    {
                        case (int)Common.Enums.Status.Active:
                            item.Status = Convert.ToInt32(Common.Enums.Status.Inactive.ToString("d"));
                            item.Retired = true;
                            item.RetiredOn = DateTime.Now;
                            item.RetiredBy = Common.Config.CurrentUserId;
                            item.Save();

                            result = true;
                            break;

                        case (int)Common.Enums.Status.Draft:
                            xPort5.DAL.SupplierAddressCollection addresses = xPort5.DAL.SupplierAddress.LoadCollection(String.Format("SupplierId = '{0}'", supplierId.ToString()));
                            if (addresses.Count > 0)
                            {
                                foreach (xPort5.DAL.SupplierAddress address in addresses)
                                {
                                    address.Delete();
                                }
                            }
                            xPort5.DAL.SupplierContactCollection contacts = xPort5.DAL.SupplierContact.LoadCollection(String.Format("SupplierId = '{0}'", supplierId.ToString()));
                            if (contacts.Count > 0)
                            {
                                foreach (xPort5.DAL.SupplierContact contact in contacts)
                                {
                                    contact.Delete();
                                }
                            }
                            item.Delete();

                            result = true;
                            break;
                    }

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, item.ToString());
                }
                return result;
            }

            public static string NextSupplierCode()
            {
                string result = String.Empty;

                X_CounterCollection counter = X_Counter.LoadCollection();
                if (counter.Count > 0)
                {
                    result = counter[0].NextSuppCode.ToString();
                    counter[0].NextSuppCode = counter[0].NextSuppCode + 1;
                    counter[0].Save();
                }

                return result;
            }

            public static String GetSupplierName(Guid supplierId)
            {
                String result = String.Empty;

                xPort5.DAL.Supplier item = xPort5.DAL.Supplier.Load(supplierId);
                if (item != null)
                {
                    switch (xPort5.DAL.Common.Config.CurrentLanguageId)
                    {
                        case 2:
                            result = item.SupplierName_Chs;
                            break;
                        case 3:
                            result = item.SupplierName_Cht;
                            break;
                        case 1:
                        default:
                            result = item.SupplierName;
                            break;
                    }
                }

                return result;
            }
        }

        public class SupplierAddress
        {
            public static bool DeleteRec(Guid cAddressId)
            {
                bool result = true;

                xPort5.DAL.SupplierAddress item = xPort5.DAL.SupplierAddress.Load(cAddressId);
                if (item != null)
                {
                    // 2009.09.25 paulus: 直接 delete 算了
                    /*
                    Staff user = Staff.Load(item.StaffId);
                    switch ((int)user.Status)
                    {
                        case (int)Common.Enums.Status.Active:
                            item.Retired = true;
                            item.RetiredOn = DateTime.Now;
                            item.RetiredBy = Common.Config.CurrentUserId;
                            item.Save();

                            result = true;
                            break;

                        case (int)Common.Enums.Status.Draft:
                            item.Delete();
                            if (item.DefaultRec)
                            {
                                // set the next StaffAddress record to DefaultRec
                                StaffAddressCollection uAddresses = StaffAddress.LoadCollection(String.Format("StaffId = '{0}'", item.StaffId.ToString()));
                                if (uAddresses.Count > 0)
                                {
                                    StaffAddress uAddress = uAddresses[0];
                                    uAddress.DefaultRec = true;
                                    uAddress.Save();
                                }
                            }

                            result = true;
                            break;
                    }
                    */
                    item.Delete();
                    if (item.DefaultRec)
                    {
                        // set the next SupplierAddress record to DefaultRec
                        xPort5.DAL.SupplierAddressCollection uAddresses = xPort5.DAL.SupplierAddress.LoadCollection(String.Format("SupplierId = '{0}'", item.SupplierId.ToString()));
                        if (uAddresses.Count > 0)
                        {
                            xPort5.DAL.SupplierAddress uAddress = uAddresses[0];
                            uAddress.DefaultRec = true;
                            uAddress.Save();
                        }
                    }

                    result = true;
                }
                return result;
            }

            public static int Count(Guid userId)
            {
                xPort5.DAL.SupplierAddressCollection items = xPort5.DAL.SupplierAddress.LoadCollection(String.Format("SupplierId = '{0}'", userId.ToString()));
                return items.Count;
            }
        }

        public class SupplierContact
        {
            public static bool DeleteRec(Guid contactId)
            {
                bool result = true;

                xPort5.DAL.SupplierContact item = xPort5.DAL.SupplierContact.Load(contactId);
                if (item != null)
                {
                    #region 2009.09.25 paulus: 直接 delete 算了
                    /*
                    Staff user = Staff.Load(item.StaffId);
                    switch ((int)user.Status)
                    {
                        case (int)Common.Enums.Status.Active:
                            item.Retired = true;
                            item.RetiredOn = DateTime.Now;
                            item.RetiredBy = Common.Config.CurrentUserId;
                            item.Save();

                            result = true;
                            break;

                        case (int)Common.Enums.Status.Draft:
                            item.Delete();
                            if (item.DefaultRec)
                            {
                                // set the next StaffAddress record to DefaultRec
                                StaffAddressCollection uAddresses = StaffAddress.LoadCollection(String.Format("StaffId = '{0}'", item.StaffId.ToString()));
                                if (uAddresses.Count > 0)
                                {
                                    StaffAddress uAddress = uAddresses[0];
                                    uAddress.DefaultRec = true;
                                    uAddress.Save();
                                }
                            }

                            result = true;
                            break;
                    }
                    */
                    #endregion
                    item.Delete();
                    if (item.DefaultRec)
                    {
                        // set the next SupplierContact record to DefaultRec
                        xPort5.DAL.SupplierContactCollection contacts = xPort5.DAL.SupplierContact.LoadCollection(String.Format("SupplierId = '{0}'", item.SupplierId.ToString()));
                        if (contacts.Count > 0)
                        {
                            xPort5.DAL.SupplierContact contact = contacts[0];
                            contact.DefaultRec = true;
                            contact.Save();
                        }
                    }

                    // 2012.04.06 paulus: 把 login 的資料一並刪除
                    UserProfile.DelRec(contactId);

                    result = true;

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, item.ToString());
                }
                return result;
            }

            public static int Count(Guid userId)
            {
                xPort5.DAL.SupplierContactCollection items = xPort5.DAL.SupplierContact.LoadCollection(String.Format("SupplierId = '{0}'", userId.ToString()));
                return items.Count;
            }
        }

        public class Staff
        {
            public static bool IsAccessAllowed(xPort5.DAL.Common.Enums.UserGroup security)
            {
                bool result = false;

                xPort5.DAL.Staff user = xPort5.DAL.Staff.Load(Common.Config.CurrentUserId);
                if (user != null)
                {
                    T_Group group = T_Group.Load(user.GroupId);
                    if (group != null)
                    {
                        int userLevel = Convert.ToInt32(group.GroupCode);
                        if (userLevel <= (int)security)
                        {
                            result = true;
                        }
                    }
                }

                return result;
            }

            public static string NextStaffCode()
            {
                string result = String.Empty;

                X_CounterCollection counter = X_Counter.LoadCollection();
                if (counter.Count > 0)
                {
                    result = counter[0].NextStaffCode.ToString();
                    counter[0].NextStaffCode = counter[0].NextStaffCode + 1;
                    counter[0].Save();
                }

                return result;
            }

            public static bool DeleteRec(Guid userId)
            {
                bool result = false;

                xPort5.DAL.Staff staff = xPort5.DAL.Staff.Load(userId);
                if (staff != null)
                {
                    // cannot delete primary user (primary user means CreatedBy = Guid.Empty)
                    if (staff.CreatedBy != Guid.Empty)
                    {
                        switch ((int)staff.Status)
                        {
                            case (int)Common.Enums.Status.Active:
                                staff.Status = Convert.ToInt32(Common.Enums.Status.Inactive.ToString("d"));
                                staff.Retired = true;
                                staff.RetiredOn = DateTime.Now;
                                staff.RetiredBy = Common.Config.CurrentUserId;
                                staff.Save();

                                result = true;
                                break;

                            case (int)Common.Enums.Status.Draft:
                                xPort5.DAL.StaffAddressCollection addresses = xPort5.DAL.StaffAddress.LoadCollection(String.Format("StaffId = '{0}'", userId.ToString()));
                                if (addresses.Count > 0)
                                {
                                    foreach (xPort5.DAL.StaffAddress address in addresses)
                                    {
                                        address.Delete();
                                    }
                                }
                                staff.Delete();

                                result = true;
                                break;
                        }

                        // 2012.04.04 paulus: 把 login 的資料一並刪除
                        UserProfile.DelRec(staff.StaffId);
                    }

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, staff.ToString());
                }
                return result;
            }

            public static string GetAlias(Guid userId)
            {
                string result = String.Empty;
                xPort5.DAL.Staff user = xPort5.DAL.Staff.Load(userId);
                if (user != null)
                {
                    result = user.Alias;
                }
                return result;
            }

            public static Guid GetSupplierId(Guid userId)
            {
                Guid supplierId = new Guid();

                xPort5.DAL.Staff user = xPort5.DAL.Staff.Load(userId);
                if (user != null)
                {
                    string[] password = user.Password.Split('-');
                    xPort5.DAL.Supplier supplier = xPort5.DAL.Supplier.LoadWhere(String.Format("SupplierCode = '{0}'", password[1]));
                    if (supplier != null)
                    {
                        supplierId = supplier.SupplierId;
                    }
                }

                return supplierId;
            }

            public static bool IsSuperUser(Guid userId)
            {
                bool result = false;

                xPort5.DAL.Staff staff = xPort5.DAL.Staff.Load(userId);
                if (staff != null)
                {
                    if (staff.CreatedBy == Guid.Empty)
                    {
                        result = true;
                    }
                }

                return result;
            }

            public static Guid GetSuperUserId()
            {
                Guid result = Guid.Empty;

                String sql = String.Format("CreatedBy = '{0}'", Guid.Empty.ToString());
                xPort5.DAL.Staff staff = xPort5.DAL.Staff.LoadWhere(sql);
                if (staff != null)
                {
                    result = staff.StaffId;
                }
                return result;
            }
        }

        public class StaffAddress
        {
            public static bool DeleteRec(Guid userAddressId)
            {
                bool result = true;

                xPort5.DAL.StaffAddress item = xPort5.DAL.StaffAddress.Load(userAddressId);
                if (item != null)
                {
                    // 2009.09.25 paulus: 直接 delete 算了
                    /*
                    Staff user = Staff.Load(item.StaffId);
                    switch ((int)user.Status)
                    {
                        case (int)Common.Enums.Status.Active:
                            item.Retired = true;
                            item.RetiredOn = DateTime.Now;
                            item.RetiredBy = Common.Config.CurrentUserId;
                            item.Save();

                            result = true;
                            break;

                        case (int)Common.Enums.Status.Draft:
                            item.Delete();
                            if (item.DefaultRec)
                            {
                                // set the next StaffAddress record to DefaultRec
                                StaffAddressCollection uAddresses = StaffAddress.LoadCollection(String.Format("StaffId = '{0}'", item.StaffId.ToString()));
                                if (uAddresses.Count > 0)
                                {
                                    StaffAddress uAddress = uAddresses[0];
                                    uAddress.DefaultRec = true;
                                    uAddress.Save();
                                }
                            }

                            result = true;
                            break;
                    }
                    */
                    item.Delete();
                    if (item.DefaultRec)
                    {
                        // set the next StaffAddress record to DefaultRec
                        xPort5.DAL.StaffAddressCollection uAddresses = xPort5.DAL.StaffAddress.LoadCollection(String.Format("StaffId = '{0}'", item.StaffId.ToString()));
                        if (uAddresses.Count > 0)
                        {
                            xPort5.DAL.StaffAddress uAddress = uAddresses[0];
                            uAddress.DefaultRec = true;
                            uAddress.Save();
                        }
                    }

                    result = true;

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, item.ToString());
                }
                return result;
            }

            public static int Count(Guid userId)
            {
                xPort5.DAL.StaffAddressCollection items = xPort5.DAL.StaffAddress.LoadCollection(String.Format("StaffId = '{0}'", userId.ToString()));
                return items.Count;
            }
        }

        public class TreeViewControl
        {
            #region Load Customers

            /// <summary>
            /// Loads the customer by name.
            /// </summary>
            /// <param name="root">The root.</param>
            public static void LoadCustomer(TreeNodeCollection root)
            {
                LoadCustomer(root, true);
            }

            /// <summary>
            /// Loads the customer.
            /// </summary>
            /// <param name="root">The root.</param>
            /// <param name="byNameOrCode">if set to <c>true</c> [by customer name] else [by customer code].</param>
            public static void LoadCustomer(TreeNodeCollection root, bool byNameOrCode)
            {
                for (int i = 0; i <= 26; i++)
                {
                    AddNodes(root, i, byNameOrCode);
                }
            }

            private static void AddNodes(TreeNodeCollection oNodes, int row, bool byNameOrCode)
            {
                string where = String.Format("Status >= {0}", Common.Enums.Status.Active.ToString("d"));
                string[] orderby = { "CustomerName" };

                #region create the 1st alpha character
                TreeNode oNode = new TreeNode();
                oNode.Image = new IconResourceHandle("16x16.folder_close.png");
                oNode.ExpandedImage = new IconResourceHandle("16x16.folder_open.png");
                oNode.IsExpanded = false;
                switch (row)
                {
                    case 0:
                        oNode.Label = "#";
                        break;
                    default:
                        oNode.Label = ((char)(row + 64)).ToString();
                        break;
                }
                oNodes.Add(oNode);
                #endregion

                #region append the Clients with the same Alpha
                switch (row)
                {
                    case 0:
                        where = "SUBSTRING([CustomerName], 1, 1) NOT BETWEEN N'A' AND N'Z'";
                        break;
                    default:
                        where = String.Format("SUBSTRING([CustomerName], 1, 1) = N'{0}'", ((char)(row + 64)).ToString());
                        break;
                }
                xPort5.DAL.CustomerCollection oClients = xPort5.DAL.Customer.LoadCollection(where, orderby, true);
                if (oClients.Count > 0)
                {
                    oNode.Loaded = true;

                    foreach (xPort5.DAL.Customer client in oClients)
                    {
                        TreeNode endNode = new TreeNode();
                        if (byNameOrCode)
                        {
                            endNode.Label = client.CustomerName;
                        }
                        else
                        {
                            endNode.Label = client.CustomerCode;
                        }
                        endNode.Tag = client.CustomerId;
                        if (client.BlackListed)
                        {
                            endNode.Image = new IconResourceHandle("16x16.customerwarning_16.png");
                        }
                        else
                        {
                            endNode.Image = new IconResourceHandle("16x16.CustomerSingle_16.png");
                        }
                        endNode.IsExpanded = false;
                        oNode.Nodes.Add(endNode);
                    }
                }
                #endregion
            }

            #endregion

            /// <summary>
            /// Loads the specified tree view nodes.
            /// </summary>
            /// <typeparam name="T">Customer, Supplier</typeparam>
            /// <param name="root">The root.</param>
            public static void Load<T>(TreeNodeCollection root)
            {
                Load<T>(root, true);
            }

            /// <summary>
            /// Loads the specified tree view nodes.
            /// </summary>
            /// <typeparam name="T">Customer, Supplier</typeparam>
            /// <param name="byNameOrCode">if set to <c>true</c> [by customer name] else [by customer code].</param>
            public static void Load<T>(TreeNodeCollection root, bool byNameOrCode)
            {
                for (int i = 0; i <= 26; i++)
                {
                    AddNodes<T>(root, i, byNameOrCode);
                }
            }

            /// <summary>
            /// Adds the nodes.
            /// </summary>
            /// <typeparam name="T">Customer, Supplier</typeparam>
            /// <param name="oNodes">The o nodes.</param>
            /// <param name="row">The row.</param>
            /// <param name="byNameOrCode">if set to <c>true</c> [by customer name] else [by customer code].</param>
            private static void AddNodes<T>(TreeNodeCollection oNodes, int row, bool byNameOrCode)
            {
                Type classType = typeof(T);

                if (classType.Name == "Supplier" || classType.Name == "Customer")
                {
                    string fieldName = classType.Name + (byNameOrCode ? "Name" : "Code");

                    string where = String.Format("Status >= {0}", Common.Enums.Status.Active.ToString("d"));
                    string[] orderby = { fieldName };

                    #region create the 1st alpha character

                    TreeNode oNode = new TreeNode();
                    oNode.Image = new IconResourceHandle("16x16.folder_close.png");
                    oNode.ExpandedImage = new IconResourceHandle("16x16.folder_open.png");
                    oNode.IsExpanded = false;
                    switch (row)
                    {
                        case 0:
                            oNode.Label = "#";
                            break;
                        default:
                            oNode.Label = ((char)(row + 64)).ToString();
                            break;
                    }
                    oNodes.Add(oNode);

                    #endregion

                    #region append the Clients with the same Alpha

                    switch (row)
                    {
                        case 0:
                            where = "SUBSTRING([" + fieldName + "], 1, 1) NOT BETWEEN N'A' AND N'Z'";
                            break;
                        default:
                            where = String.Format("SUBSTRING([" + fieldName + "], 1, 1) = N'{0}'", ((char)(row + 64)).ToString());
                            break;
                    }

                    // 2009.12.28 david: 临时性。应该有其他方法。
                    switch (classType.Name.ToLower())
                    {
                        case "customer":
                            xPort5.DAL.CustomerCollection oClients = xPort5.DAL.Customer.LoadCollection(where, orderby, true);
                            if (oClients.Count > 0)
                            {
                                oNode.Loaded = true;

                                foreach (xPort5.DAL.Customer client in oClients)
                                {
                                    TreeNode endNode = new TreeNode();
                                    if (byNameOrCode)
                                    {
                                        endNode.Label = client.CustomerName;
                                    }
                                    else
                                    {
                                        endNode.Label = client.CustomerCode + " - " + client.CustomerName;
                                    }
                                    endNode.Tag = client.CustomerId;
                                    if (client.BlackListed)
                                    {
                                        endNode.Image = new IconResourceHandle("16x16.customerwarning_16.png");
                                    }
                                    else
                                    {
                                        endNode.Image = new IconResourceHandle("16x16.CustomerSingle_16.png");
                                    }
                                    endNode.IsExpanded = false;
                                    oNode.Nodes.Add(endNode);
                                }
                            }
                            break;
                        case "supplier":
                            xPort5.DAL.SupplierCollection oSuppliers = xPort5.DAL.Supplier.LoadCollection(where, orderby, true);
                            if (oSuppliers.Count > 0)
                            {
                                oNode.Loaded = true;

                                foreach (xPort5.DAL.Supplier supplier in oSuppliers)
                                {
                                    TreeNode endNode = new TreeNode();
                                    if (byNameOrCode)
                                    {
                                        endNode.Label = supplier.SupplierName;
                                    }
                                    else
                                    {
                                        endNode.Label = supplier.SupplierCode + " - " + supplier.SupplierName;
                                    }
                                    endNode.Tag = supplier.SupplierId;
                                    endNode.Image = new IconResourceHandle("16x16.supplierSingle_16.gif");
                                    endNode.IsExpanded = false;
                                    oNode.Nodes.Add(endNode);
                                }
                            }
                            break;
                    }

                    #endregion
                }
            }

        }

        public class OrderQT
        {
            public static bool DeleteRec(Guid quotationId)
            {
                bool result = false;

                xPort5.DAL.OrderQT order = xPort5.DAL.OrderQT.Load(quotationId);
                if (order != null)
                {
                    switch ((int)order.Status)
                    {
                        case (int)Common.Enums.Status.Active:
                            order.Status = Convert.ToInt32(Common.Enums.Status.Inactive.ToString("d"));
                            order.Retired = true;
                            order.RetiredOn = DateTime.Now;
                            order.RetiredBy = Common.Config.CurrentUserId;
                            order.Save();

                            result = true;
                            break;

                        case (int)Common.Enums.Status.Draft:
                            xPort5.DAL.OrderQTItemsCollection items = xPort5.DAL.OrderQTItems.LoadCollection(String.Format("OrderQTId = '{0}'", quotationId.ToString()));
                            if (items.Count > 0)
                            {
                                foreach (xPort5.DAL.OrderQTItems item in items)
                                {
                                    #region delete child OrderQTCustShipping
                                    xPort5.DAL.OrderQTCustShippingCollection custShipping = xPort5.DAL.OrderQTCustShipping.LoadCollection(String.Format("OrderQTItemId = '{0}'", item.OrderQTItemId.ToString()));
                                    if (custShipping.Count > 0)
                                    {
                                        foreach (xPort5.DAL.OrderQTCustShipping cust in custShipping)
                                        {
                                            cust.Delete();
                                        }
                                    }
                                    #endregion

                                    #region delete child OrderQTSuppShipping
                                    xPort5.DAL.OrderQTSuppShippingCollection suppShipping = xPort5.DAL.OrderQTSuppShipping.LoadCollection(String.Format("OrderQTItemId = '{0}'", item.OrderQTItemId.ToString()));
                                    if (suppShipping.Count > 0)
                                    {
                                        foreach (xPort5.DAL.OrderQTSuppShipping supp in suppShipping)
                                        {
                                            supp.Delete();
                                        }
                                    }
                                    #endregion

                                    #region delete child OrderQTPackage
                                    xPort5.DAL.OrderQTPackageCollection packages = xPort5.DAL.OrderQTPackage.LoadCollection(String.Format("OrderQTItemId = '{0}'", item.OrderQTItemId.ToString()));
                                    if (packages.Count > 0)
                                    {
                                        foreach (xPort5.DAL.OrderQTPackage package in packages)
                                        {
                                            package.Delete();
                                        }
                                    }
                                    #endregion

                                    #region delete child OrderQTSupplier
                                    xPort5.DAL.OrderQTSupplierCollection suppliers = xPort5.DAL.OrderQTSupplier.LoadCollection(String.Format("OrderQTItemId = '{0}'", item.OrderQTItemId.ToString()));
                                    if (suppliers.Count > 0)
                                    {
                                        foreach (xPort5.DAL.OrderQTSupplier supplier in suppliers)
                                        {
                                            supplier.Delete();
                                        }
                                    }
                                    #endregion

                                    item.Delete();
                                }
                            }
                            order.Delete();

                            result = true;
                            break;
                    }

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, order.ToString());
                }
                return result;
            }

            public static string NextQTNumber()
            {
                string result = String.Empty;

                X_CounterCollection counter = X_Counter.LoadCollection();
                if (counter.Count > 0)
                {
                    result = counter[0].NextQTNumber.ToString();
                    counter[0].NextQTNumber = counter[0].NextQTNumber + 1;
                    counter[0].Save();
                }

                return result;
            }

            public static string GetCurrencyCode(Guid quotationId)
            {
                if (quotationId != null)
                {
                    xPort5.DAL.OrderQT order = xPort5.DAL.OrderQT.Load(quotationId);
                    if (order != null)
                    {
                        return Currency.CurrencyUsed(order.CurrencyId);
                    }
                }

                return string.Empty;
            }

            public static string GetCurrencyCode_Factory(Guid qtItemId)
            {
                if (qtItemId != null)
                {
                    String sql = String.Format("OrderQtItemId = '{0}'", qtItemId.ToString());
                    xPort5.DAL.OrderQTSupplier order = xPort5.DAL.OrderQTSupplier.LoadWhere(sql);
                    if (order != null)
                    {
                        return Currency.CurrencyUsed(order.CurrencyId);
                    }
                }

                return string.Empty;
            }

            public static string GetPackingUnit(Guid qtItemId)
            {
                if (qtItemId != null)
                {
                    String sql = String.Format("OrderQtItemId = '{0}'", qtItemId.ToString());
                    xPort5.DAL.OrderQTPackage order = xPort5.DAL.OrderQTPackage.LoadWhere(sql);
                    if (order != null)
                    {
                        return order.Unit;
                    }
                }

                return string.Empty;
            }

            public static Decimal GetXchgRate_Quoted(Guid orderId)
            {
                Decimal result = 1;

                xPort5.DAL.OrderQT order = xPort5.DAL.OrderQT.Load(orderId);
                if (order != null)
                {
                    result = order.ExchangeRate;
                }
                return result;
            }

            public static Decimal GetXchgRate_Factory(Guid qtItemId)
            {
                Decimal result = 1;
                String sql = String.Format("OrderQTItemId = '{0}'", qtItemId.ToString());
                xPort5.DAL.OrderQTSupplier order = xPort5.DAL.OrderQTSupplier.LoadWhere(sql);
                if (order != null)
                {
                    xPort5.DAL.T_Currency cny = xPort5.DAL.T_Currency.Load(order.CurrencyId);
                    if (cny != null)
                        result = cny.XchgRate;
                }
                return result;
            }

            public static bool DeleteItem(Guid qtItemId)
            {
                bool result = false;
                string sql = string.Format("OrderQTItemId = '{0}'", qtItemId.ToString());

                // [David 2010-08-05]: 如果该Quotation Item已经生成咗Pre-Order的话，那么就不能删除该Item了。
                xPort5.DAL.OrderPLItemsCollection itemList = xPort5.DAL.OrderPLItems.LoadCollection(sql);
                if (itemList.Count > 0)
                {
                    result = false;
                }
                else
                {
                    xPort5.DAL.OrderQTItems orderItem = xPort5.DAL.OrderQTItems.Load(qtItemId);
                    if (orderItem != null)
                    {
                        orderItem.Delete();

                        result = true;
                    }

                    xPort5.DAL.OrderQTCustShippingCollection orderCShippingList = xPort5.DAL.OrderQTCustShipping.LoadCollection(sql);
                    foreach (xPort5.DAL.OrderQTCustShipping orderCShipping in orderCShippingList)
                    {
                        orderCShipping.Delete();
                    }

                    xPort5.DAL.OrderQTSuppShippingCollection orderSShippingList = xPort5.DAL.OrderQTSuppShipping.LoadCollection(sql);
                    foreach (xPort5.DAL.OrderQTSuppShipping orderSShipping in orderSShippingList)
                    {
                        orderSShipping.Delete();
                    }

                    xPort5.DAL.OrderQTPackageCollection orderPackageList = xPort5.DAL.OrderQTPackage.LoadCollection(sql);
                    foreach (xPort5.DAL.OrderQTPackage orderPackage in orderPackageList)
                    {
                        orderPackage.Delete();
                    }

                    xPort5.DAL.OrderQTSupplierCollection orderSupplierList = xPort5.DAL.OrderQTSupplier.LoadCollection(sql);
                    foreach (xPort5.DAL.OrderQTSupplier orderSupplier in orderSupplierList)
                    {
                        orderSupplier.Delete();
                    }
                }

                return result;
            }
        }

        public class OrderPL
        {
            public static bool DeleteRec(Guid preOrderId)
            {
                bool result = false;

                xPort5.DAL.OrderPL order = xPort5.DAL.OrderPL.Load(preOrderId);
                if (order != null)
                {
                    switch ((int)order.Status)
                    {
                        case (int)Common.Enums.Status.Active:
                            order.Status = Convert.ToInt32(Common.Enums.Status.Inactive.ToString("d"));
                            order.Retired = true;
                            order.RetiredOn = DateTime.Now;
                            order.RetiredBy = Common.Config.CurrentUserId;
                            order.Save();

                            result = true;
                            break;

                        case (int)Common.Enums.Status.Draft:
                            xPort5.DAL.OrderPLItemsCollection items = xPort5.DAL.OrderPLItems.LoadCollection(String.Format("OrderPLId = '{0}'", preOrderId.ToString()));
                            if (items.Count > 0)
                            {
                                foreach (xPort5.DAL.OrderPLItems item in items)
                                {
                                    item.Delete();
                                }
                            }
                            order.Delete();

                            result = true;
                            break;
                    }

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, order.ToString());
                }
                return result;
            }

            public static bool DeleteItem(Guid preOrderItemId)
            {
                bool result = false;

                xPort5.DAL.OrderPLItems orderItem = xPort5.DAL.OrderPLItems.Load(preOrderItemId);
                if (orderItem != null)
                {
                    orderItem.Delete();

                    result = true;
                }

                return result;
            }

            internal static string NextPLNumber()
            {
                string result = String.Empty;

                X_CounterCollection counter = X_Counter.LoadCollection();
                if (counter.Count > 0)
                {
                    result = counter[0].NextPLNumber.ToString();
                    counter[0].NextPLNumber = counter[0].NextPLNumber + 1;
                    counter[0].Save();
                }

                return result;
            }
        }

        public class OrderSC
        {
            public static bool DeleteRec(Guid contractId)
            {
                bool result = false;

                xPort5.DAL.OrderSC order = xPort5.DAL.OrderSC.Load(contractId);
                if (order != null)
                {
                    switch ((int)order.Status)
                    {
                        case (int)Common.Enums.Status.Active:
                            order.Status = Convert.ToInt32(Common.Enums.Status.Inactive.ToString("d"));
                            order.Retired = true;
                            order.RetiredOn = DateTime.Now;
                            order.RetiredBy = Common.Config.CurrentUserId;
                            order.Save();

                            result = true;
                            break;

                        case (int)Common.Enums.Status.Draft:
                            xPort5.DAL.OrderSCItemsCollection items = xPort5.DAL.OrderSCItems.LoadCollection(String.Format("OrderSCId = '{0}'", contractId.ToString()));
                            if (items.Count > 0)
                            {
                                foreach (xPort5.DAL.OrderSCItems item in items)
                                {
                                    item.Delete();
                                }
                            }
                            order.Delete();

                            result = true;
                            break;
                    }

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, order.ToString());
                }
                return result;
            }

            public static bool DeleteItem(Guid contractItemId)
            {
                bool result = false;

                xPort5.DAL.OrderSCItems orderItem = xPort5.DAL.OrderSCItems.Load(contractItemId);
                if (orderItem != null)
                {
                    orderItem.Delete();

                    result = true;
                }

                return result;
            }

            internal static string NextSCNumber()
            {
                string result = String.Empty;

                X_CounterCollection counter = X_Counter.LoadCollection();
                if (counter.Count > 0)
                {
                    result = counter[0].NextSCNumber.ToString();
                    counter[0].NextSCNumber = counter[0].NextSCNumber + 1;
                    counter[0].Save();
                }

                return result;
            }

            public static Guid OrderQTItemId(Guid orderSCItemsId)
            {
                OrderSCItems item = OrderSCItems.Load(orderSCItemsId);
                if (item != null)
                {
                    return item.OrderQTItemId;
                }

                return Guid.Empty;
            }
        }

        public class OrderPC
        {
            public static bool DeleteRec(Guid contractId)
            {
                bool result = false;

                xPort5.DAL.OrderPC order = xPort5.DAL.OrderPC.Load(contractId);
                if (order != null)
                {
                    switch ((int)order.Status)
                    {
                        case (int)Common.Enums.Status.Active:
                            order.Status = Convert.ToInt32(Common.Enums.Status.Inactive.ToString("d"));
                            order.Retired = true;
                            order.RetiredOn = DateTime.Now;
                            order.RetiredBy = Common.Config.CurrentUserId;
                            order.Save();

                            result = true;
                            break;

                        case (int)Common.Enums.Status.Draft:
                            xPort5.DAL.OrderPCItemsCollection items = xPort5.DAL.OrderPCItems.LoadCollection(String.Format("OrderPCId = '{0}'", contractId.ToString()));
                            if (items.Count > 0)
                            {
                                foreach (xPort5.DAL.OrderPCItems item in items)
                                {
                                    item.Delete();
                                }
                            }
                            order.Delete();

                            result = true;
                            break;
                    }

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, order.ToString());
                }
                return result;
            }

            public static bool DeleteItem(Guid contractItemId)
            {
                bool result = false;

                xPort5.DAL.OrderPCItems orderItem = xPort5.DAL.OrderPCItems.Load(contractItemId);
                if (orderItem != null)
                {
                    orderItem.Delete();

                    result = true;
                }

                return result;
            }

            internal static string NextPCNumber()
            {
                string result = String.Empty;

                X_CounterCollection counter = X_Counter.LoadCollection();
                if (counter.Count > 0)
                {
                    result = counter[0].NextPCNumber.ToString();
                    counter[0].NextPCNumber = counter[0].NextPCNumber + 1;
                    counter[0].Save();
                }

                return result;
            }
        }

        public class OrderIN
        {
            public static bool DeleteRec(Guid contractId)
            {
                bool result = false;

                xPort5.DAL.OrderIN order = xPort5.DAL.OrderIN.Load(contractId);
                if (order != null)
                {
                    switch ((int)order.Status)
                    {
                        case (int)Common.Enums.Status.Active:
                            order.Status = Convert.ToInt32(Common.Enums.Status.Inactive.ToString("d"));
                            order.Retired = true;
                            order.RetiredOn = DateTime.Now;
                            order.RetiredBy = Common.Config.CurrentUserId;
                            order.Save();

                            result = true;
                            break;

                        case (int)Common.Enums.Status.Draft:
                            xPort5.DAL.OrderINItemsCollection items = xPort5.DAL.OrderINItems.LoadCollection(String.Format("OrderINId = '{0}'", contractId.ToString()));
                            if (items.Count > 0)
                            {
                                foreach (xPort5.DAL.OrderINItems item in items)
                                {
                                    item.Delete();
                                }
                            }
                            order.Delete();

                            result = true;
                            break;
                    }

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, order.ToString());
                }
                return result;
            }

            public static bool DeleteItem(Guid contractItemId)
            {
                bool result = false;

                xPort5.DAL.OrderINItems orderItem = xPort5.DAL.OrderINItems.Load(contractItemId);
                if (orderItem != null)
                {
                    OrderINShipmentCollection shipmentList = OrderINShipment.LoadCollection("OrderINItemsId = '" + orderItem.OrderINItemsId.ToString() + "'");
                    foreach (OrderINShipment shipment in shipmentList)
                    {
                        shipment.Delete();
                    }

                    orderItem.Delete();

                    result = true;

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, orderItem.ToString());
                }

                return result;
            }

            internal static string NextINNumber()
            {
                string result = String.Empty;

                X_CounterCollection counter = X_Counter.LoadCollection();
                if (counter.Count > 0)
                {
                    result = counter[0].NextINNumber.ToString();
                    counter[0].NextINNumber = counter[0].NextINNumber + 1;
                    counter[0].Save();
                }

                return result;
            }

            public static Decimal TotalCharges(String invoiceNumber)
            {
                Decimal result = 0;

                String sql = String.Format("INNumber = '{0}'", invoiceNumber);
                xPort5.DAL.OrderIN invoice = xPort5.DAL.OrderIN.LoadWhere(sql);
                if (invoice != null)
                {
                    result = TotalCharges(invoice.OrderINId);
                }
                return result;
            }

            public static Decimal TotalCharges(Guid invoiceId)
            {
                Decimal result = 0;

                String sql = String.Format("OrderINId = '{0}'", invoiceId.ToString());
                xPort5.DAL.OrderINChargesCollection charges = xPort5.DAL.OrderINCharges.LoadCollection(sql);
                if (charges.Count > 0)
                {
                    for (int i = 0; i < charges.Count; i++)
                    {
                        result += charges[i].Amount;
                    }
                }

                return result;
            }

            public static String ConvertToPK(String invNumber)
            {
                String result = String.Empty;

                result = invNumber.Replace("INV", "PK");

                return result;
            }
        }

        public class OrderSP
        {
            public static bool DeleteRec(Guid contractId)
            {
                bool result = false;

                xPort5.DAL.OrderSP order = xPort5.DAL.OrderSP.Load(contractId);
                if (order != null)
                {
                    switch ((int)order.Status)
                    {
                        case (int)Common.Enums.Status.Active:
                            order.Status = Convert.ToInt32(Common.Enums.Status.Inactive.ToString("d"));
                            order.Retired = true;
                            order.RetiredOn = DateTime.Now;
                            order.RetiredBy = Common.Config.CurrentUserId;
                            order.Save();

                            result = true;
                            break;

                        case (int)Common.Enums.Status.Draft:
                            xPort5.DAL.OrderSPItemsCollection items = xPort5.DAL.OrderSPItems.LoadCollection(String.Format("OrderSPId = '{0}'", contractId.ToString()));
                            if (items.Count > 0)
                            {
                                foreach (xPort5.DAL.OrderSPItems item in items)
                                {
                                    item.Delete();
                                }
                            }
                            order.Delete();

                            result = true;
                            break;
                    }

                    // log activity
                    xPort5.Controls.Log4net.LogInfo(xPort5.Controls.Log4net.LogAction.Delete, order.ToString());
                }
                return result;
            }

            public static bool DeleteItem(Guid contractItemId)
            {
                bool result = false;

                xPort5.DAL.OrderSPItems orderItem = xPort5.DAL.OrderSPItems.Load(contractItemId);
                if (orderItem != null)
                {
                    orderItem.Delete();

                    result = true;
                }

                return result;
            }

            internal static string NextSPNumber()
            {
                string result = String.Empty;

                X_CounterCollection counter = X_Counter.LoadCollection();
                if (counter.Count > 0)
                {
                    result = counter[0].NextSPNumber.ToString();
                    counter[0].NextSPNumber = counter[0].NextSPNumber + 1;
                    counter[0].Save();
                }

                return result;
            }
        }

        public class TextBoxControl
        {
            /// <summary>
            /// Gets the back color of a read only box.
            /// </summary>
            /// <value>the back color.</value>
            public static Color ColorReadOnly
            {
                get
                {
                    return Color.LightYellow;
                }
            }

            /// <summary>
            /// Gets the back color of a mandatary box.
            /// </summary>
            /// <value>the back color.</value>
            public static Color ColorMadatary
            {
                get
                {
                    return Color.PaleTurquoise;
                }
            }

            /// <summary>
            /// Set: Textbox.BackColor = ColorReadOnly, Textbox.ReadOnly = false, and Textbox.Enable = false
            /// </summary>
            /// <param name="target"></param>
            public static void SetToReadOnly(ref TextBox target)
            {
                target.BackColor = ColorReadOnly;
                target.Enabled = false;
                target.ReadOnly = false;
            }
        }

        public class Resources
        {
            #region Image Size

            public class ImageSize
            {
                /// <summary>
                /// Gets the small size(90, 65).
                /// </summary>
                /// <value>The small.</value>
                public static Size Small
                {
                    get
                    {
                        return new Size(90, 65);
                    }
                }

                /// <summary>
                /// Gets the medium Size(120, 90).
                /// </summary>
                /// <value>The medium.</value>
                public static Size Medium
                {
                    get
                    {
                        return new Size(120, 90);
                    }
                }

                /// <summary>
                /// Gets the large Size(240, 180).
                /// </summary>
                /// <value>The large.</value>
                public static Size Large
                {
                    get
                    {
                        return new Size(240, 180);
                    }
                }

                /// <summary>
                /// Gets the X-large Size(320, 240).
                /// </summary>
                /// <value>The X large.</value>
                public static Size XLarge
                {
                    get
                    {
                        return new Size(320, 240);
                    }
                }
            }

            #endregion

            private static void SaveFile(string fileName, Guid productId)
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    xPort5.DAL.Resources resc = new xPort5.DAL.Resources();
                    resc.Keyword = Utility.Product.ProductCode(productId);
                    resc.ContentType = (int)Common.Enums.ContentType.Image;
                    resc.OriginalFileName = fileName;
                    resc.SaveAsFileId = productId.ToString();
                    resc.SaveAsFileName = fileName;
                    resc.CreatedBy = Common.Config.CurrentUserId;
                    resc.CreatedOn = DateTime.Now;
                    resc.ModifiedBy = Common.Config.CurrentUserId;
                    resc.ModifiedOn = DateTime.Now;

                    resc.Save();

                    // David 2010-08-17: 如果无Key Picture，就增加一个.
                    if (!Utility.Product.HasKeyPicture(productId))
                    {
                        Utility.Product.SaveKeyPicture(productId, resc.ResourcesId);
                    }
                }
            }

            #region Product Resource man

            public static string UploadPicture(OpenFileDialog objFileDialog, Guid productId)
            {
                string filePath = string.Empty;

                if (objFileDialog != null)
                {
                    for (int i = 0; i < objFileDialog.Files.Count; i++)
                    {
                        filePath = PictureFilePath(productId, string.Empty);

                        HttpPostedFileHandle file = objFileDialog.Files[i] as HttpPostedFileHandle;
                        if (file.ContentLength > 0)
                        {
                            string fileName = Path.GetFileName(file.PostedFileName);
                            filePath = Path.Combine(filePath, fileName);
                            file.SaveAs(filePath);

                            SaveFile(fileName, productId);
                        }
                    }
                }

                return filePath;
            }

            public static string PictureFilePath(Guid productId, string fileName)
            {
                string result = String.Empty;

                Article product = Article.Load(productId);
                if (product != null)
                {
                    result = PictureFilePath(product.ArticleCode, fileName);
                }

                return result;
            }

            /// <summary>
            /// Picture file path: ProductImage/[ProductCode]/[FileName]
            /// </summary>
            /// <param name="productCode">The product code.</param>
            /// <param name="fileName">Name of the file.</param>
            /// <returns></returns>
            public static string PictureFilePath(string productCode, string fileName)
            {
                string productCodeDir = Path.Combine(VWGContext.Current.Config.GetDirectory("ProductImage"), productCode);
                string filePath = Path.Combine(productCodeDir, fileName);

                if (!(Directory.Exists(productCodeDir))) Directory.CreateDirectory(productCodeDir);

                return filePath;
            }

            /// <summary>
            /// Deletes all the pictures belongs to the product
            /// </summary>
            /// <param name="productId">The product id.</param>
            public static void DeletePictures(Guid productId)
            {
                Article product = Article.Load(productId);
                if (product != null)
                {
                    string sql = "ArticleId = '" + product.ArticleId.ToString() + "'";

                    // delete the key picture
                    ArticleKeyPicture keyPic = ArticleKeyPicture.LoadWhere(sql);
                    if (keyPic != null)
                    {
                        keyPic.Delete();
                    }

                    sql = "Keyword = '" + Utility.Product.ProductCode(productId) + "'";
                    xPort5.DAL.ResourcesCollection resList = xPort5.DAL.Resources.LoadCollection(sql);
                    foreach (xPort5.DAL.Resources res in resList)
                    {
                        DeletePicture(product.ArticleId, res.OriginalFileName);
                    }
                }
            }

            public static bool DeletePicture(Guid productId, string fileName)
            {
                bool result = false;

                Article product = Article.Load(productId);
                if (product != null)
                {
                    result = DeletePicture(productId, product.CategoryId, fileName);
                }

                return result;
            }

            public static bool DeletePicture(Guid productId, Guid categoryId, string fileName)
            {
                bool result = false;

                string filePath = PictureFilePath(Product.ProductCode(productId), fileName);

                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                        result = true;
                    }
                    catch { }
                }

                return result;
            }

            public static void RenamePicture(Guid oldId, Guid newId)
            {
                Article product = Article.Load(newId);
                if (product != null)
                {
                    Article oldProduct = Article.Load(oldId);
                    if (oldProduct != null)
                    {
                        if (oldProduct.ArticleCode != product.ArticleCode)
                        {
                            string oldDir = Path.Combine(VWGContext.Current.Config.GetDirectory("ProductImage"), oldProduct.ArticleCode);
                            string[] fileList = Directory.GetFiles(oldDir);

                            for (int i = 0; i < fileList.Length; i++)
                            {
                                string fileName = Path.GetFileName(fileList[i]);
                                string newFilePath = PictureFilePath(product.ArticleId, fileName);
                                string oldFilePath = PictureFilePath(oldId, fileName);

                                if (File.Exists(oldFilePath))
                                {
                                    File.Copy(oldFilePath, newFilePath, true);
                                }
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// This allows us to resize the image. It prevents skewed images and
            /// also vertically long images caused by trying to maintain the aspect
            /// ratio on images who's height is larger than their width
            /// </summary>
            /// <param name="ImageFilePath"></param>
            /// <param name="NewWidth"></param>
            /// <param name="MaxHeight"></param>
            /// <param name="OnlyResizeIfWider"></param>
            /// <returns></returns>
            public static Image GetPicture(string ImageFilePath, int NewWidth, int MaxHeight, bool OnlyResizeIfWider)
            {
                System.Drawing.Image FullsizeImage = System.Drawing.Image.FromFile(ImageFilePath);

                // Prevent using images internal thumbnail
                FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
                FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

                if (OnlyResizeIfWider)
                {
                    if (FullsizeImage.Width <= NewWidth)
                    {
                        NewWidth = FullsizeImage.Width;
                    }
                }

                int NewHeight = FullsizeImage.Height * NewWidth / FullsizeImage.Width;
                if (NewHeight > MaxHeight)
                {
                    // Resize with height instead
                    NewWidth = FullsizeImage.Width * MaxHeight / FullsizeImage.Height;
                    NewHeight = MaxHeight;
                }

                System.Drawing.Image NewImage = FullsizeImage.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero);

                // Clear handle to original file so that we can overwrite it if necessary
                FullsizeImage.Dispose();

                return NewImage;
            }

            #endregion

            #region Order Resource man

            public static string UploadResource(OpenFileDialog objFileDialog, string orderNumber)
            {
                string filePath = ResourceFilePath(orderNumber, string.Empty);

                if (objFileDialog != null)
                {
                    for (int i = 0; i < objFileDialog.Files.Count; i++)
                    {
                        HttpPostedFileHandle file = objFileDialog.Files[i] as HttpPostedFileHandle;
                        if (file.ContentLength > 0)
                        {
                            filePath = Path.Combine(filePath, Path.GetFileName(file.PostedFileName));
                            file.SaveAs(filePath);
                        }
                    }
                }

                return filePath;
            }

            /// <summary>
            /// Picture file path: Attachment/[OrderNumber]/[FileName]
            /// </summary>
            /// <param name="orderNumber">The order number.</param>
            /// <param name="fileName">Name of the file.</param>
            /// <returns></returns>
            public static string ResourceFilePath(string orderNumber, string fileName)
            {
                string attchmentDir = VWGContext.Current.Config.GetDirectory("Attachment");
                string orderNumberDir = Path.Combine(attchmentDir, orderNumber);
                string filePath = Path.Combine(orderNumberDir, fileName);

                if (!(Directory.Exists(attchmentDir))) Directory.CreateDirectory(attchmentDir);
                if (!(Directory.Exists(orderNumberDir))) Directory.CreateDirectory(orderNumberDir);

                return filePath;
            }

            /// <summary>
            /// Deletes all the attachments belongs to the order
            /// </summary>
            public static void DeleteResources(Guid orderId, string orderNumber)
            {
                string sql = "SaveAsFileId = '" + orderId.ToString() + "'";
                xPort5.DAL.ResourcesCollection resList = xPort5.DAL.Resources.LoadCollection(sql);
                foreach (xPort5.DAL.Resources res in resList)
                {
                    DeleteResource(orderId, orderNumber, res.OriginalFileName);
                }
            }

            public static bool DeleteResource(Guid orderId, string orderNumber, string fileName)
            {
                bool result = false;

                string filePath = ResourceFilePath(orderNumber, fileName);

                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                        result = true;
                    }
                    catch { }
                }

                return result;
            }

            public static bool HasAttachment(string orderNumber)
            {
                bool result = false;

                string sql = "KeyWord = '" + orderNumber + "'";
                xPort5.DAL.ResourcesCollection resList = xPort5.DAL.Resources.LoadCollection(sql);
                if (resList.Count > 0)
                {
                    result = true;
                }

                return result;
            }

            public static bool HasAttachment(Guid orderId)
            {
                bool result = false;

                string sql = "SaveAsFileId = '" + orderId.ToString() + "'";
                xPort5.DAL.ResourcesCollection resList = xPort5.DAL.Resources.LoadCollection(sql);
                if (resList.Count > 0)
                {
                    result = true;
                }

                return result;
            }

            #endregion
        }

        public class ImagePanel
        {
            public enum ThumbnailSize
            {
                /// <summary>
                /// Small thumbnail size : 24x24
                /// </summary>
                Small,

                /// <summary>
                /// Medium thumbnail size : 32x32
                /// </summary>
                Medium,

                /// <summary>
                /// Large thumbnail size : 64x64
                /// </summary>
                Large
            }

            public enum CheckedType
            {
                Order,
                Product
            }

            public static List<Guid> GetCheckedItems(FlowLayoutPanel flpImageList, CheckedType checkedType)
            {
                List<Guid> checkedItems = new List<Guid>();

                for (int i = 0; i < flpImageList.Controls.Count; i++)
                {
                    Control ctrl = flpImageList.Controls[i];
                    if (ctrl is Panel)
                    {
                        for (int j = 0; j < ctrl.Controls.Count; j++)
                        {
                            Control pCtrl = ctrl.Controls[j];
                            if (pCtrl is ProductImage)
                            {
                                ProductImage imgCtrl = pCtrl as ProductImage;
                                if (imgCtrl != null)
                                {
                                    if (IsChecked(imgCtrl.Parent))
                                    {
                                        switch (checkedType)
                                        {
                                            case CheckedType.Product:
                                                checkedItems.Add(imgCtrl.ProductId);
                                                break;
                                            case CheckedType.Order:
                                            default:
                                                if (Common.Utility.IsGUID(imgCtrl.Name))
                                                {
                                                    checkedItems.Add(new Guid(imgCtrl.Name));
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return checkedItems;
            }

            private static bool IsChecked(Control panel)
            {
                for (int i = 0; i < panel.Controls.Count; i++)
                {
                    Control ctrl = panel.Controls[i];
                    if (ctrl is CheckBox)
                    {
                        CheckBox chkCtrl = ctrl as CheckBox;
                        if (chkCtrl != null)
                        {
                            if (chkCtrl.Tag != null && chkCtrl.Tag.ToString().ToLower() == "selectedimage")
                            {
                                return chkCtrl.Checked;
                            }
                        }
                    }
                }

                return false;
            }
        }

        public class SysInfo
        {
            private string ownerName = String.Empty;
            private string factoryGDocsAccount = String.Empty;
            private string factoryGDocsPassword = String.Empty;

            public SysInfo()
            {
            }

            public static SysInfo Load()
            {
                SysInfo sysInfo = new SysInfo();

                SystemInfoCollection sys = SystemInfo.LoadCollection();
                if (sys.Count > 0)
                {
                    sysInfo.OwnerName = sys[0].OwnerName;
                    sysInfo.FactoryGDocsAccount = sys[0].GetMetadata("FactoryGDocsAccount");
                    sysInfo.FactoryGDocsPassword = sys[0].GetMetadata("FactoryGDocsPassword");
                }

                return sysInfo;
            }

            public void Save()
            {
                SystemInfoCollection sys = SystemInfo.LoadCollection();
                if (sys.Count > 0)
                {
                    sys[0].OwnerName = this.OwnerName;
                    sys[0].SetMetadata("FactoryGDocsAccount", this.FactoryGDocsAccount);
                    sys[0].SetMetadata("FactoryGDocsPassword", this.FactoryGDocsPassword);
                    sys[0].Save();
                }
                else
                {
                    SystemInfo sysInfo = new SystemInfo();
                    sysInfo.OwnerName = this.OwnerName;
                    sysInfo.SetMetadata("FactoryGDocsAccount", this.FactoryGDocsAccount);
                    sysInfo.SetMetadata("FactoryGDocsPassword", this.FactoryGDocsPassword);
                    sysInfo.Save();
                }
            }

            #region public properties
            public string OwnerName
            {
                get { return ownerName; }
                set { ownerName = value; }
            }

            public string FactoryGDocsAccount
            {
                get { return factoryGDocsAccount; }
                set { factoryGDocsAccount = value; }
            }

            public string FactoryGDocsPassword
            {
                get { return factoryGDocsPassword; }
                set { factoryGDocsPassword = value; }
            }
            #endregion
        }

        public class UserProfile
        {
            public static bool SaveRec(Guid userSid, int userType, String loginName, String loginPassword, String alias)
            {
                bool result = true;

                String sql = String.Format("UserSid = '{0}'", userSid.ToString());
                xPort5.DAL.UserProfile user = xPort5.DAL.UserProfile.LoadWhere(sql);

                if (user != null)
                {
                    user.UserType = userType;
                    user.LoginName = loginName;
                    user.LoginPassword = loginPassword;
                    user.Alias = alias;
                    user.Save();

                    result = true;
                }
                else
                {
                    user = new xPort5.DAL.UserProfile();
                    user.UserSid = userSid;
                    user.UserType = userType;
                    user.LoginName = loginName;
                    user.LoginPassword = loginPassword;
                    user.Alias = alias;
                    user.Save();

                    result = true;
                }

                return result;
            }

            public static bool DelRec(Guid userSid)
            {
                bool result = false;

                String sql = String.Format("UserSid = '{0}'", userSid.ToString());
                xPort5.DAL.UserProfile user = xPort5.DAL.UserProfile.LoadWhere(sql);

                if (user != null)
                {
                    string cmd = @"
DELETE
FROM [dbo].[UserDisplayPreference]
WHERE [UserId] = '" + user.UserId.ToString() + @"'
";
                    SqlDataReader reader = SqlHelper.Default.ExecuteReader(CommandType.Text, cmd);

                    user.Delete();
                    result = true;
                }

                return result;
            }
        }

        public class NetSqlAzMan
        {
            /// <summary>
            /// Store = xPort5, Application, Operation, Identity = CurrentUserId, DateTime.Now, false
            /// </summary>
            /// <param name="operation"></param>
            /// <returns></returns>
            public static bool IsAccessAuthorized(String application, String operation)
            {
                bool result = false;
                String storeName = "xPort5";

                String cs = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["NetSqlAzManDb"].ConnectionString;

                using (IAzManStorage storage = new SqlAzManStorage(cs))
                {
                    IAzManSid userSid = new SqlAzManSID(Common.Config.CurrentUserId.ToString());    // CurrentUserId = UserProfile.UserSid，就是 AzManDBUser 的 Sid
                    IAzManDBUser dbUser = storage.GetDBUser(userSid);

                    try
                    {
                        AuthorizationType authorization = storage.CheckAccess(storeName, application, operation, dbUser, DateTime.Now, false);
                        switch (authorization)
                        {
                            case AuthorizationType.Allow:
                                result = true;
                                break;
                        }
                    }
                    catch { }
                }

                return result;
            }

            /// <summary>
            /// Store = xPort5, Store Group = Staff -> T_Group,GroupName, Customer, or Supplier
            /// </summary>
            /// <param name="sid"></param>
            /// <returns></returns>
            public static bool AddUserToGroup(xPort5.DAL.Staff staff)
            {
                bool result = false;
                String groupName = String.Empty;

                #region 找出 staff 所屬的 groupName = T_Group.GroupName
                if (staff != null)
                {
                    xPort5.DAL.T_Group staffGroup = xPort5.DAL.T_Group.Load(staff.GroupId);
                    groupName = staffGroup.GroupName;
                }
                #endregion

                RemoveUser(staff.StaffId);                                  // 首先取消原來的 Membership
                result = AddUser(staff.StaffId, groupName);                 // 然後加進新的 Membership

                return result;
            }
            public static bool AddUserToGroup(xPort5.DAL.CustomerContact customer)
            {
                bool result = false;
                String groupName = "Customer";

                RemoveUser(customer.CustomerContactId);                     // 首先取消原來的 Membership
                result = AddUser(customer.CustomerId, groupName);           // 然後加進新的 Membership

                return result;
            }
            public static bool AddUserToGroup(xPort5.DAL.SupplierContact supplier)
            {
                bool result = false;
                String groupName = "Supplier";

                RemoveUser(supplier.SupplierContactId);                     // 首先取消原來的 Membership
                result = AddUser(supplier.SupplierContactId, groupName);    // 然後加進新的 Membership

                return result;
            }

            /// <summary>
            /// add the user to the store group that it belongs to
            /// </summary>
            /// <param name="sid"></param>
            /// <returns></returns>
            public static bool AddUser(Guid sid, String groupName)
            {
                bool result = false;
                String storeName = "xPort5";

                String cs = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["NetSqlAzManDb"].ConnectionString;

                using (IAzManStorage storage = new SqlAzManStorage(cs))
                {
                    IAzManSid userSid = new SqlAzManSID(sid.ToString());
                    //IAzManDBUser dbUser = storage.GetDBUser(userSid);

                    using (IAzManStore store = storage.GetStore(storeName))
                    {
                        IAzManStoreGroup[] groups = store.GetStoreGroups();

                        foreach (IAzManStoreGroup group in groups)
                        {
                            if (groupName == group.Name)
                            {
                                group.CreateStoreGroupMember(userSid, WhereDefined.Database, true);
                            }
                        }
                    }
                }

                return result;
            }

            /// <summary>
            /// delete the user member from the Store Group
            /// </summary>
            /// <param name="sid"></param>
            /// <returns></returns>
            public static bool RemoveUser(Guid sid)
            {
                bool result = false;
                String storeName = "xPort5";

                String cs = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["NetSqlAzManDb"].ConnectionString;

                using (IAzManStorage storage = new SqlAzManStorage(cs))
                {
                    try
                    {
                        IAzManSid userSid = new SqlAzManSID(sid.ToString());
                        IAzManDBUser dbUser = storage.GetDBUser(userSid);

                        using (IAzManStore store = storage.GetStore(storeName))
                        {
                            IAzManStoreGroup[] groups = store.GetStoreGroups();

                            foreach (IAzManStoreGroup group in groups)
                            {
                                if (group.IsInGroup(dbUser))
                                {
                                    IAzManStoreGroupMember member = group.GetStoreGroupMember(userSid);
                                    member.Delete();

                                    result = true;
                                }
                            }
                        }
                    }
                    catch { }
                }

                return result;
            }
        }

        public class DisplayPreference
        {
            public static void Save(ListView lvwList)
            {
                // 把每個 ColumnHeader 的資料保存在 MetadataXml 中
                String sql = String.Format("UserSid = '{0}'", Common.Config.CurrentUserId.ToString());
                xPort5.DAL.UserProfile user = xPort5.DAL.UserProfile.LoadWhere(sql);
                if (user != null)
                {
                    sql = String.Format("UserId = '{0}' AND PreferenceObjectId = '{1}'", user.UserId.ToString(), ((Guid)lvwList.Tag).ToString());

                    UserDisplayPreference userPref = UserDisplayPreference.LoadWhere(sql);

                    if (userPref == null)
                    {
                        userPref = new UserDisplayPreference();
                        userPref.UserId = user.UserId;
                        userPref.PreferenceObjectId = (Guid)lvwList.Tag;
                    }

                    userPref.MetadataXml = new Dictionary<string, UserDisplayPreference.MetadataAttributes>();     // 首先清空舊的 Metadata.

                    foreach (ColumnHeader col in lvwList.Columns)
                    {
                        UserDisplayPreference.MetadataAttributes attrs = new UserDisplayPreference.MetadataAttributes();

                        attrs.Add(new UserDisplayPreference.MetadataAttribute("Name", col.Name));
                        //attrs.Add(new UserDisplayPreference.MetadataAttribute("Position", col.Position.ToString()));
                        attrs.Add(new UserDisplayPreference.MetadataAttribute("SortOrder", col.SortOrder.ToString()));
                        attrs.Add(new UserDisplayPreference.MetadataAttribute("SortPosition", col.SortPosition.ToString()));
                        attrs.Add(new UserDisplayPreference.MetadataAttribute("Text", col.Text));                  // 為咗方便睇 SQL 紀錄，Text 會 save 不過不需要 load
                        attrs.Add(new UserDisplayPreference.MetadataAttribute("Visible", col.Visible.ToString()));
                        attrs.Add(new UserDisplayPreference.MetadataAttribute("Width", col.Width.ToString()));
                        if (col.Image != null)
                            attrs.Add(new UserDisplayPreference.MetadataAttribute("ImageFile", col.Image.File));
                        else
                            attrs.Add(new UserDisplayPreference.MetadataAttribute("ImageFile", String.Empty));

                        userPref.SetMetadata(col.Index.ToString(), attrs);                                  // 採用 ColumnHeader.Index 作為 key
                    }

                    userPref.Save();
                }
            }

            public static void Delete(ListView lvwList)
            {
                String sql = String.Format("UserSid = '{0}'", Common.Config.CurrentUserId.ToString());
                xPort5.DAL.UserProfile user = xPort5.DAL.UserProfile.LoadWhere(sql);
                if (user != null)
                {
                    sql = String.Format("UserId = '{0}' AND PreferenceObjectId = '{1}'", user.UserId.ToString(), ((Guid)lvwList.Tag).ToString());

                    UserDisplayPreference userPref = UserDisplayPreference.LoadWhere(sql);

                    if (userPref != null)
                    {
                        userPref.Delete();
                    }
                }
            }

            public static void Load(ref ListView lvwList)
            {
                String sql = String.Format("UserSid = '{0}'", Common.Config.CurrentUserId.ToString());
                xPort5.DAL.UserProfile user = xPort5.DAL.UserProfile.LoadWhere(sql);
                if (user != null)
                {
                    // 2012.04.18 paulus:
                    // 首先用 SuperUser 個 Id 試下，搵唔到才用自己個 Id，於是 SuperUser 可以設定 ListView 的 Layout 給所有用戶
                    sql = String.Format("UserId = '{0}' AND PreferenceObjectId = '{1}'", xPort5.Controls.Utility.Staff.GetSuperUserId().ToString(), ((Guid)lvwList.Tag).ToString());
                    UserDisplayPreference userPref = UserDisplayPreference.LoadWhere(sql);
                    if (userPref == null)
                    {
                        sql = String.Format("UserId = '{0}' AND PreferenceObjectId = '{1}'", user.UserId.ToString(), ((Guid)lvwList.Tag).ToString());
                        userPref = UserDisplayPreference.LoadWhere(sql);
                    }

                    #region 搵到就根據 UserDisplayPreference 的資料更改 ColumnHeader
                    if (userPref != null)
                    {
                        Dictionary<string, xPort5.DAL.UserDisplayPreference.MetadataAttributes> metadata = userPref.MetadataXml;
                        foreach (KeyValuePair<string, xPort5.DAL.UserDisplayPreference.MetadataAttributes> col in metadata)
                        {
                            int colIndex = int.Parse(col.Key);      // col.Key 等於 ColumnHeader.Index

                            foreach (xPort5.DAL.UserDisplayPreference.MetadataAttribute item in col.Value)
                            {
                                int position = 0, sortPosition = 0, width = 0;
                                bool visible = false;

                                switch (item.Key)
                                {
                                    case "Name":
                                        lvwList.Columns[colIndex].Name = item.Value;
                                        break;
                                    case "Position":
                                        int.TryParse(item.Value, out position);
                                        //lvwList.Columns[colIndex].Position = position;
                                        break;
                                    case "SortOrder":
                                        if (item.Value == Gizmox.WebGUI.Forms.SortOrder.Ascending.ToString("g"))
                                            lvwList.Columns[colIndex].SortOrder = Gizmox.WebGUI.Forms.SortOrder.Ascending;
                                        else if (item.Value == Gizmox.WebGUI.Forms.SortOrder.Descending.ToString("g"))
                                            lvwList.Columns[colIndex].SortOrder = Gizmox.WebGUI.Forms.SortOrder.Descending;
                                        else if (item.Value == Gizmox.WebGUI.Forms.SortOrder.None.ToString("g"))
                                            lvwList.Columns[colIndex].SortOrder = Gizmox.WebGUI.Forms.SortOrder.None;
                                        break;
                                    case "SortPosition":
                                        int.TryParse(item.Value, out sortPosition);
                                        lvwList.Columns[colIndex].SortPosition = sortPosition;
                                        break;
                                    case "Visible":
                                        bool.TryParse(item.Value, out visible);
                                        lvwList.Columns[colIndex].Visible = visible;
                                        break;
                                    case "Width":
                                        int.TryParse(item.Value, out width);
                                        lvwList.Columns[colIndex].Width = width;
                                        break;
                                }
                            }
                        }
                    }
                    #endregion
                }
            }

            #region 2012.04.18 paulus: 已放棄這段 Serialization code
            // HACK: Serialize 最好的保存方法是用 SQL Data Type VARBINARY 保存
            public Byte [] Serialize(ListView lvwList)
            {
                CustomColumn[] ccArray = CustomizeListView(lvwList);

                XmlSerializer xmlSerialer = new XmlSerializer(typeof(CustomColumn));

                BinaryFormatter objBinaryFormatter = new BinaryFormatter();
                MemoryStream objMemoryStream = new MemoryStream();
                StringWriter writer = new StringWriter();

                objBinaryFormatter.Serialize(objMemoryStream, ccArray);

                objMemoryStream.Seek(0, 0);
                Byte[] content = objMemoryStream.ToArray();

                String s = System.Text.Encoding.Default.GetString(objMemoryStream.ToArray());

                return content;
            }

            // HACK: 未完成
            public void Deserialize(ListView lvwList)
            {
                CustomColumn[] ccArray = CustomizeListView(lvwList);
                BinaryFormatter objBinaryFormatter = new BinaryFormatter();
                MemoryStream objMemoryStream = new MemoryStream();
                objBinaryFormatter.Serialize(objMemoryStream, ccArray);
                objMemoryStream.Seek(0, 0);
                Byte[] content = objMemoryStream.ToArray();
            }

            [Serializable()]
            public class CustomColumn
            {
                public string ImageFile;
                public int Index;
                public string Name;
                public int Position;
                public string Text;
                public bool Visible;
                public int Width;
            }

            public CustomColumn[] CustomizeListView(ListView L)
            {
                CustomColumn[] cColArray = new CustomColumn[L.Columns.Count];

                for (int i = 0; i < L.Columns.Count; i++)
                {
                    ColumnHeader col = L.Columns[i];
                    CustomColumn cc = new CustomColumn();

                    cc.ImageFile = String.Empty;
                    if (col.Image != null)
                        cc.ImageFile = col.Image.File;
                    cc.Index = col.Index;
                    cc.Name = col.Name;
                    //cc.Position = col.Position;
                    cc.Text = col.Text;
                    cc.Visible = col.Visible;
                    cc.Width = col.Width;

                    cColArray[i] = cc;
                }

                return cColArray;
            }
            #endregion
        }

        public class ToolbarControl
        {
            /// <summary>
            /// Add: Save, Save & Close, Save & Dup, Delete buttons
            /// 如果不使用 NetSqlAzMan, param: application 和 operation 可以 null
            /// </summary>
            /// <param name="target"></param>
            /// <param name="editMode"></param>
            /// <param name="application"></param>
            /// <param name="operation"></param>
            public static void LoadAnsBaseButtons(ref ToolBar target, Common.Enums.EditMode editMode, String application, String operation)
            {
                nxStudio.BaseClass.WordDict oDict = new nxStudio.BaseClass.WordDict(Common.Config.CurrentWordDict, Common.Config.CurrentLanguageId);

                if (xPort5.DAL.Common.Config.UseNetSqlAzMan)
                {
                    #region 用 NetSqlAzMan 就要 check permissions
                    // cmdSave
                    ToolBarButton cmdSave = new ToolBarButton("Save", oDict.GetWord("Save"));
                    cmdSave.Tag = "Save";
                    cmdSave.Image = new IconResourceHandle("16x16.16_L_save.gif");

                    // cmdSaveClose
                    ToolBarButton cmdSaveClose = new ToolBarButton("Save & Close", oDict.GetWord("Save_Close").Replace("%26", "&"));
                    cmdSaveClose.Tag = "Save & Close";
                    cmdSaveClose.Image = new IconResourceHandle("16x16.16_saveClose.gif");

                    // cmdSaveDup
                    ToolBarButton cmdSaveDup = new ToolBarButton("Save & Dup", oDict.GetWord("Save_Dup").Replace("%26", "&"));
                    cmdSaveDup.Tag = "Save & Dup";
                    cmdSaveDup.Image = new IconResourceHandle("16x16.16_L_saveDup.gif");

                    // cmdDelete
                    ToolBarButton cmdDelete = new ToolBarButton("Delete", oDict.GetWord("Delete"));
                    cmdDelete.Tag = "Delete";
                    cmdDelete.Image = new IconResourceHandle("16x16.16_L_remove.gif");

                    if (editMode != Common.Enums.EditMode.Read)
                    {
                        if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized(application, operation + ".Update"))
                        {
                            target.Buttons.Add(cmdSave);
                            target.Buttons.Add(cmdSaveClose);
                            target.Buttons.Add(cmdSaveDup);
                        }

                        if (editMode != Common.Enums.EditMode.Add)
                        {
                            if (xPort5.Controls.Utility.NetSqlAzMan.IsAccessAuthorized(application, operation + ".Delete"))
                            {
                                target.Buttons.Add(cmdDelete);
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 舊方式
                    // cmdSave
                    ToolBarButton cmdSave = new ToolBarButton("Save", oDict.GetWord("Save"));
                    cmdSave.Tag = "Save";
                    cmdSave.Image = new IconResourceHandle("16x16.16_L_save.gif");

                    // cmdSaveClose
                    ToolBarButton cmdSaveClose = new ToolBarButton("Save & Close", oDict.GetWord("Save_Close").Replace("%26", "&"));
                    cmdSaveClose.Tag = "Save & Close";
                    cmdSaveClose.Image = new IconResourceHandle("16x16.16_saveClose.gif");

                    // cmdSaveDup
                    ToolBarButton cmdSaveDup = new ToolBarButton("Save & Dup", oDict.GetWord("Save_Dup").Replace("%26", "&"));
                    cmdSaveDup.Tag = "Save & Dup";
                    cmdSaveDup.Image = new IconResourceHandle("16x16.16_L_saveDup.gif");

                    // cmdDelete
                    ToolBarButton cmdDelete = new ToolBarButton("Delete", oDict.GetWord("Delete"));
                    cmdDelete.Tag = "Delete";
                    cmdDelete.Image = new IconResourceHandle("16x16.16_L_remove.gif");

                    if (editMode != Common.Enums.EditMode.Read)
                    {
                        target.Buttons.Add(cmdSave);
                        target.Buttons.Add(cmdSaveClose);
                        target.Buttons.Add(cmdSaveDup);

                        if (editMode != Common.Enums.EditMode.Add)
                        {
                            target.Buttons.Add(cmdDelete);
                        }
                    }
                    #endregion
                }
            }
        }
    }
}
