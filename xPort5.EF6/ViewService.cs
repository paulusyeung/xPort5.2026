using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using xPort5.EF6.Base;

namespace xPort5.EF6
{
    /// <summary>
    /// Service class for replacing database Views with LINQ queries
    /// </summary>
    public class ViewService
    {
        /// <summary>
        /// Singleton instance for convenient access
        /// </summary>
        public static ViewService Default { get; } = new ViewService();

        /// <summary>
        /// Private constructor to enforce singleton pattern
        /// </summary>
        private ViewService()
        {
        }

        /// <summary>
        /// Gets a new DbContext instance for EF6 data access
        /// </summary>
        /// <returns>A new xPort5Entities DbContext instance</returns>
        public xPort5Entities GetDbContext()
        {
            return new xPort5Entities();
        }

        #region Date Range Filtering

        /// <summary>
        /// Filters a queryable by date range using a date property name
        /// </summary>
        public IQueryable<T> FilterByDateRange<T>(
            IQueryable<T> query,
            string datePropertyName,
            DateTime? fromDate,
            DateTime? toDate)
        {
            if (string.IsNullOrWhiteSpace(datePropertyName))
            {
                return query;
            }

            if (fromDate.HasValue)
            {
                query = query.Where($"{datePropertyName} >= @0", fromDate.Value);
            }

            if (toDate.HasValue)
            {
                // Include the entire day for toDate
                var toDateEnd = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where($"{datePropertyName} <= @0", toDateEnd);
            }

            return query;
        }

        /// <summary>
        /// Parses a date string parameter (common format: "yyyy-MM-dd" or empty string)
        /// </summary>
        public DateTime? ParseDateParameter(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return null;
            }

            DateTime result;
            if (DateTime.TryParse(dateString, out result))
            {
                return result;
            }

            return null;
        }

        #endregion

        #region ID Parsing Helpers

        /// <summary>
        /// Parses a comma-separated string of GUIDs (format: 'guid1','guid2','guid3')
        /// </summary>
        public List<Guid> ParseCustomerIdArray(string customerIdArray)
        {
            var result = new List<Guid>();

            if (string.IsNullOrWhiteSpace(customerIdArray))
            {
                return result;
            }

            // Remove single quotes and split by comma
            var parts = customerIdArray.Replace("'", "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                Guid guid;
                if (Guid.TryParse(trimmed, out guid))
                {
                    result.Add(guid);
                }
            }

            return result;
        }

        /// <summary>
        /// Filters a queryable by customer IDs from a comma-separated GUID string
        /// </summary>
        public IQueryable<T> FilterByCustomerIdArray<T>(
            IQueryable<T> query,
            string customerIdArray) where T : class
        {
            var customerIds = ParseCustomerIdArray(customerIdArray);

            if (customerIds.Count == 0)
            {
                return query;
            }

            // Use standard LINQ Contains which EF6 translates to SQL IN clause
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = typeof(T).GetProperty("CustomerId", BindingFlags.Public | BindingFlags.Instance);
            
            if (property == null)
            {
                return query;
            }

            var propertyAccess = Expression.Property(parameter, property);
            var constant = Expression.Constant(customerIds);
            var containsMethod = typeof(List<Guid>).GetMethod("Contains", new[] { typeof(Guid) });
            var containsCall = Expression.Call(constant, containsMethod, propertyAccess);
            var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);

            return query.Where(lambda);
        }

        #endregion

        #region Product Views

        /// <summary>
        /// Retrieves the product list, replacing vwProductList
        /// </summary>
        public DataSet GetProductList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View uses INNER JOIN for Category and Origin, LEFT JOIN for AgeGrading and Color
                // Staff joins for CreatedBy and ModifiedBy are INNER JOINs
                var query = from a in context.Article
                            join c in context.T_Category on a.CategoryId equals c.CategoryId // INNER JOIN
                            join o in context.T_Origin on a.OriginId equals o.OriginId // INNER JOIN
                            join s in context.Staff on a.CreatedBy equals s.StaffId // INNER JOIN for CreatedBy
                            join s1 in context.Staff on a.ModifiedBy equals s1.StaffId // INNER JOIN for ModifiedBy
                            join ag in context.T_AgeGrading on a.AgeGradingId equals ag.AgeGradingId into agGroup
                            from ag in agGroup.DefaultIfEmpty() // LEFT JOIN
                            join cl in context.T_Color on a.ColorId equals cl.ColorId into clGroup
                            from cl in clGroup.DefaultIfEmpty() // LEFT JOIN
                            select new
                            {
                                a.ArticleId,
                                a.SKU,
                                a.ArticleCode,
                                a.ArticleName,
                                a.ArticleName_Chs,
                                a.ArticleName_Cht,
                                c.CategoryId,
                                c.CategoryCode,
                                c.CategoryName,
                                c.CategoryName_Chs,
                                c.CategoryName_Cht,
                                AgeGradingId = ag != null ? (Guid?)ag.AgeGradingId : (Guid?)null,
                                AgeGradingCode = ag != null ? ag.AgeGradingCode : (string)null,
                                AgeGradingName = ag != null ? ag.AgeGradingName : (string)null,
                                AgeGradingName_Chs = ag != null ? ag.AgeGradingName_Chs : (string)null,
                                AgeGradingName_Cht = ag != null ? ag.AgeGradingName_Cht : (string)null,
                                o.OriginId,
                                o.OriginCode,
                                o.OriginName,
                                o.OriginName_Chs,
                                o.OriginName_Cht,
                                a.Remarks,
                                a.ColorPattern,
                                a.Barcode,
                                a.Status,
                                a.CreatedOn,
                                CreatedBy = s.Alias,
                                a.ModifiedOn,
                                ModifiedBy = s1.Alias,
                                ColorId = cl != null ? (Guid?)cl.ColorId : (Guid?)null,
                                ColorCode = cl != null ? cl.ColorCode : (string)null,
                                ColorName = cl != null ? cl.ColorName : (string)null,
                                ColorName_Chs = cl != null ? cl.ColorName_Chs : (string)null,
                                ColorName_Cht = cl != null ? cl.ColorName_Cht : (string)null
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        // Fallback order
                        filteredQuery = filteredQuery.OrderBy(x => x.ArticleCode);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.ArticleCode);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        #endregion

        #region Category Views

        /// <summary>
        /// Retrieves the category list, replacing vwCategoryList
        /// Note: View has WHERE clause filtering out records where both DeptId and ClassId are NULL
        /// </summary>
        public DataSet GetCategoryList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View uses RIGHT OUTER JOIN for T_Class and LEFT OUTER JOIN for T_Dept
                var query = from c in context.T_Category
                            join cl in context.T_Class on c.ClassId equals cl.ClassId into clGroup
                            from cl in clGroup.DefaultIfEmpty() // LEFT JOIN (RIGHT JOIN in SQL = LEFT JOIN in LINQ when starting from Category)
                            join d in context.T_Dept on c.DeptId equals d.DeptId into dGroup
                            from d in dGroup.DefaultIfEmpty() // LEFT JOIN
                            select new
                            {
                                DeptId = d != null ? (Guid?)d.DeptId : null,
                                DeptCode = d != null ? d.DeptCode : (string)null,
                                DeptName = d != null ? d.DeptName : (string)null,
                                DeptName_Chs = d != null ? d.DeptName_Chs : (string)null,
                                DeptName_Cht = d != null ? d.DeptName_Cht : (string)null,
                                ClassId = cl != null ? (Guid?)cl.ClassId : null,
                                ClassCode = cl != null ? cl.ClassCode : (string)null,
                                ClassName = cl != null ? cl.ClassName : (string)null,
                                ClassName_Chs = cl != null ? cl.ClassName_Chs : (string)null,
                                ClassName_Cht = cl != null ? cl.ClassName_Cht : (string)null,
                                c.CategoryId,
                                c.CategoryCode,
                                c.CategoryName,
                                c.CategoryName_Chs,
                                c.CategoryName_Cht,
                                c.CostingMethod,
                                c.InventoryMethod,
                                c.TaxMethod
                            };

                // View has WHERE clause: (dbo.T_Dept.DeptId IS NULL) AND (dbo.T_Class.ClassId IS NULL)
                // This filters to show only categories where both Dept and Class are NULL
                var filteredQuery = query.Where(x => x.DeptId == null && x.ClassId == null);

                // Apply additional dynamic filters if provided
                filteredQuery = ApplyDynamicWhere(filteredQuery, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                         filteredQuery = filteredQuery.OrderBy(x => x.CategoryCode);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.CategoryCode);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        #endregion

        #region Invoice Views

        /// <summary>
        /// Retrieves the invoice list, replacing vwInvoiceList
        /// </summary>
        public DataSet GetInvoiceList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View uses LEFT OUTER JOINs for all joins
                var query = from i in context.OrderIN
                            join c in context.Customer on i.CustomerId equals c.CustomerId into cGroup
                            from c in cGroup.DefaultIfEmpty()
                            join s in context.Staff on i.StaffId equals s.StaffId into sGroup
                            from s in sGroup.DefaultIfEmpty()
                            join s1 in context.Staff on i.CreatedBy equals s1.StaffId into s1Group
                            from s1 in s1Group.DefaultIfEmpty()
                            join s2 in context.Staff on i.ModifiedBy equals s2.StaffId into s2Group
                            from s2 in s2Group.DefaultIfEmpty()
                            select new
                            {
                                i.OrderINId,
                                i.INDate,
                                i.InUse,
                                i.Status,
                                i.INNumber,
                                CustomerName = c != null ? c.CustomerName : (string)null,
                                i.Remarks,
                                i.CreatedOn,
                                CreatedBy = s1 != null ? s1.Alias : (string)null,
                                i.ModifiedOn,
                                ModifiedBy = s2 != null ? s2.Alias : (string)null,
                                i.Revision,
                                i.SendFrom,
                                i.SendTo,
                                CustomerId = c != null ? (Guid?)c.CustomerId : null
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                         filteredQuery = filteredQuery.OrderBy(x => x.INNumber);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.INNumber);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the invoice item list, replacing vwInvoiceItemList
        /// This is a complex view with many INNER JOINs and a subquery for SCNumber
        /// View structure: OrderIN INNER JOIN OrderINItems LEFT JOIN OrderSCItems LEFT JOIN OrderQTItems
        /// Then INNER JOINs on Article, ArticleSupplier, Supplier, T_Currency, T_Package, OrderQTPackage
        /// </summary>
        public DataSet GetInvoiceItemList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // Start from OrderINItems and work through the chain
                // View structure: OrderIN INNER JOIN OrderINItems LEFT JOIN OrderSCItems LEFT JOIN OrderQTItems
                // Then INNER JOINs on Article, ArticleSupplier, Supplier, T_Currency, T_Package, OrderQTPackage
                // Challenge: When sci is null from LEFT JOIN, we can't reference sci.OrderQTItemId in next join
                // Solution: Use a let clause to compute nullable key, then join on that
                // Note: let clauses can be used before joins in LINQ
                var query = from i in context.OrderINItems
                            join m in context.OrderIN on i.OrderINId equals m.OrderINId // INNER JOIN
                            // LEFT JOIN OrderSCItems
                            join sci in context.OrderSCItems on i.OrderSCItemsId equals sci.OrderSCItemsId into sciGroup
                            from sci in sciGroup.DefaultIfEmpty() // LEFT JOIN
                            // Compute nullable key for next join - this avoids accessing sci.OrderQTItemId when sci is null
                            let sciOrderQTItemId = sci != null ? (Guid?)sci.OrderQTItemId : (Guid?)null
                            // LEFT JOIN OrderQTItems using the computed nullable key
                            join qi in context.OrderQTItems on sciOrderQTItemId equals (Guid?)qi.OrderQTItemId into qiGroup
                            from qi in qiGroup.DefaultIfEmpty() // LEFT JOIN
                            // Compute keys for subsequent joins
                            let qiArticleId = qi != null ? (Guid?)qi.ArticleId : (Guid?)null
                            let qiSupplierId = qi != null ? (Guid?)qi.SupplierId : (Guid?)null
                            let qiPackageId = qi != null ? qi.PackageId : (Guid?)null
                            let qiOrderQTItemId = qi != null ? (Guid?)qi.OrderQTItemId : (Guid?)null
                            // Join Article
                            join a in context.Article on qiArticleId equals (Guid?)a.ArticleId into aGroup
                            from a in aGroup.DefaultIfEmpty()
                            // ArticleSupplier join - need both ArticleId and SupplierId
                            join ps in context.ArticleSupplier on 
                                new { ArticleId = qiArticleId, SupplierId = qiSupplierId }
                                equals new { ArticleId = (Guid?)ps.ArticleId, SupplierId = (Guid?)ps.SupplierId } into psGroup
                            from ps in psGroup.DefaultIfEmpty()
                            // Compute keys for remaining joins
                            let psSupplierId = ps != null ? (Guid?)ps.SupplierId : (Guid?)null
                            let psCurrencyId = ps != null ? ps.CurrencyId : (Guid?)null
                            // Join Supplier
                            join s in context.Supplier on psSupplierId equals (Guid?)s.SupplierId into sGroup
                            from s in sGroup.DefaultIfEmpty()
                            // Join Currency
                            join c in context.T_Currency on psCurrencyId equals (Guid?)c.CurrencyId into cGroup
                            from c in cGroup.DefaultIfEmpty()
                            // Join Package
                            join p in context.T_Package on qiPackageId equals (Guid?)p.PackageId into pGroup
                            from p in pGroup.DefaultIfEmpty()
                            // Join OrderQTPackage
                            join ap in context.OrderQTPackage on qiOrderQTItemId equals (Guid?)ap.OrderQTItemId into apGroup
                            from ap in apGroup.DefaultIfEmpty()
                            // Subquery for SCNumber
                            let scNumber = (from sc in context.OrderSC
                                           where sci != null && sc.OrderSCId == sci.OrderSCId
                                           select sc.SCNumber).FirstOrDefault()
                            select new
                            {
                                m.OrderINId,
                                m.INNumber,
                                m.INDate,
                                i.OrderINItemsId,
                                i.LineNumber,
                                ArticleId = a != null ? (Guid?)a.ArticleId : null,
                                SKU = a != null ? a.SKU : (string)null,
                                ArticleCode = a != null ? a.ArticleCode : (string)null,
                                ArticleName = a != null ? a.ArticleName : (string)null,
                                ArticleName_Chs = a != null ? a.ArticleName_Chs : (string)null,
                                ArticleName_Cht = a != null ? a.ArticleName_Cht : (string)null,
                                PackageId = p != null ? (Guid?)p.PackageId : null,
                                PackageCode = p != null ? p.PackageCode : (string)null,
                                PackageName = p != null ? p.PackageName : (string)null,
                                PackageName_Chs = p != null ? p.PackageName_Chs : (string)null,
                                PackageName_Cht = p != null ? p.PackageName_Cht : (string)null,
                                SupplierId = s != null ? (Guid?)s.SupplierId : null,
                                SupplierCode = s != null ? s.SupplierCode : (string)null,
                                SupplierName = s != null ? s.SupplierName : (string)null,
                                SupplierName_Chs = s != null ? s.SupplierName_Chs : (string)null,
                                SupplierName_Cht = s != null ? s.SupplierName_Cht : (string)null,
                                Particular = qi != null ? qi.Particular : (string)null,
                                CustRef = qi != null ? qi.CustRef : (string)null,
                                PriceType = qi != null ? (int?)qi.PriceType : null,
                                FactoryCost = qi != null ? (decimal?)qi.FactoryCost : null,
                                Margin = qi != null ? (decimal?)qi.Margin : null,
                                FCL = qi != null ? (decimal?)qi.FCL : null,
                                LCL = qi != null ? (decimal?)qi.LCL : null,
                                SampleQty = qi != null ? (decimal?)qi.SampleQty : null,
                                Qty = qi != null ? (decimal?)qi.Qty : null,
                                Unit = qi != null ? qi.Unit : (string)null,
                                Amount = qi != null ? (decimal?)qi.Amount : null,
                                Carton = qi != null ? (decimal?)qi.Carton : null,
                                GLAccount = qi != null ? qi.GLAccount : (string)null,
                                RefDocNo = qi != null ? qi.RefDocNo : (string)null,
                                ShippingMark = qi != null ? qi.ShippingMark : (string)null,
                                QtyIN = qi != null ? (decimal?)qi.QtyIN : null,
                                QtyOUT = qi != null ? (decimal?)qi.QtyOUT : null,
                                SuppRef = ps != null ? ps.SuppRef : (string)null,
                                CurrencyCode = c != null ? c.CurrencyCode : (string)null,
                                FCLCost = ps != null ? (decimal?)ps.FCLCost : null,
                                LCLCost = ps != null ? (decimal?)ps.LCLCost : null,
                                Inv_Qty = i.Qty,
                                SCNUmber = scNumber, // Subquery result
                                InnerBox = ap != null ? (decimal?)ap.InnerBox : null,
                                OuterBox = ap != null ? (decimal?)ap.OuterBox : null,
                                CUFT = ap != null ? (decimal?)ap.CUFT : null
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort - View orders by INNumber, LineNumber DESC
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                         filteredQuery = filteredQuery.OrderBy(x => x.INNumber).ThenByDescending(x => x.LineNumber);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.INNumber).ThenByDescending(x => x.LineNumber);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        #endregion

        #region Quotation Views

        /// <summary>
        /// Retrieves the quotation list, replacing vwOrderQTList
        /// </summary>
        public DataSet GetQuotationList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View uses LEFT OUTER JOINs for all joins
                var query = from q in context.OrderQT
                            join c in context.Customer on q.CustomerId equals c.CustomerId into cGroup
                            from c in cGroup.DefaultIfEmpty()
                            join s in context.Staff on q.StaffId equals s.StaffId into sGroup
                            from s in sGroup.DefaultIfEmpty()
                            join s1 in context.Staff on q.CreatedBy equals s1.StaffId into s1Group
                            from s1 in s1Group.DefaultIfEmpty()
                            join s2 in context.Staff on q.ModifiedBy equals s2.StaffId into s2Group
                            from s2 in s2Group.DefaultIfEmpty()
                            join s3 in context.Staff on q.AccessedBy equals s3.StaffId into s3Group
                            from s3 in s3Group.DefaultIfEmpty()
                            join tc in context.T_Currency on q.CurrencyId equals tc.CurrencyId into tcGroup
                            from tc in tcGroup.DefaultIfEmpty()
                            join tp in context.T_PaymentTerms on q.TermsId equals tp.TermsId into tpGroup
                            from tp in tpGroup.DefaultIfEmpty()
                            select new
                            {
                                q.OrderQTId,
                                q.QTNumber,
                                q.QTDate,
                                CustomerId = c != null ? (Guid?)c.CustomerId : null,
                                CustomerCode = c != null ? c.CustomerCode : (string)null,
                                CustomerName = c != null ? c.CustomerName : (string)null,
                                CustomerName_Chs = c != null ? c.CustomerName_Chs : (string)null,
                                CustomerName_Cht = c != null ? c.CustomerName_Cht : (string)null,
                                StaffId = s != null ? (Guid?)s.StaffId : null,
                                SalePerson = s != null ? s.Alias : (string)null,
                                q.PriceMethod,
                                q.FCLFactor,
                                q.LCLFactor,
                                q.Unit,
                                q.Measurement,
                                q.SampleQty,
                                q.InputMask,
                                CurrencyCode = tc != null ? tc.CurrencyCode : (string)null,
                                q.ExchangeRate,
                                TermsName = tp != null ? tp.TermsName : (string)null,
                                q.Repayment,
                                q.SendFrom,
                                q.SendTo,
                                q.TotalQty,
                                q.TotalQtyIN,
                                q.TotalQtyOUT,
                                q.TotalAmount,
                                q.Remarks,
                                q.Remarks2,
                                q.Remarks3,
                                q.Revision,
                                q.InUse,
                                q.Status,
                                q.CreatedOn,
                                CreatedBy = s1 != null ? s1.Alias : (string)null,
                                q.ModifiedOn,
                                ModifiedBy = s2 != null ? s2.Alias : (string)null,
                                q.AccessedOn,
                                AccessedBy = s3 != null ? s3.Alias : (string)null,
                                q.Retired
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort - View orders by QTNumber
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                         filteredQuery = filteredQuery.OrderBy(x => x.QTNumber);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.QTNumber);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the quotation item list, replacing vwOrderQTItemList
        /// </summary>
        public DataSet GetQuotationItemList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View uses INNER JOINs for all joins
                var query = from i in context.OrderQTItems
                            join m in context.OrderQT on i.OrderQTId equals m.OrderQTId // INNER JOIN
                            join a in context.Article on i.ArticleId equals a.ArticleId // INNER JOIN
                            // Handle nullable SupplierId for join - cast both sides to nullable Guid for type matching
                            join ps in context.ArticleSupplier on
                                new { ArticleId = i.ArticleId, SupplierId = (Guid?)i.SupplierId }
                                equals new { ArticleId = ps.ArticleId, SupplierId = (Guid?)ps.SupplierId } // INNER JOIN
                            join s in context.Supplier on ps.SupplierId equals s.SupplierId // INNER JOIN
                            join c in context.T_Currency on ps.CurrencyId equals c.CurrencyId // INNER JOIN
                            join p in context.T_Package on i.PackageId equals p.PackageId // INNER JOIN
                            join ap in context.OrderQTPackage on i.OrderQTItemId equals ap.OrderQTItemId // INNER JOIN
                            select new
                            {
                                m.OrderQTId,
                                m.QTNumber,
                                m.QTDate,
                                i.OrderQTItemId,
                                i.LineNumber,
                                a.ArticleId,
                                a.SKU,
                                a.ArticleCode,
                                a.ArticleName,
                                a.ArticleName_Chs,
                                a.ArticleName_Cht,
                                p.PackageId,
                                p.PackageCode,
                                p.PackageName,
                                p.PackageName_Chs,
                                p.PackageName_Cht,
                                s.SupplierId,
                                s.SupplierCode,
                                s.SupplierName,
                                s.SupplierName_Chs,
                                s.SupplierName_Cht,
                                i.Particular,
                                i.CustRef,
                                i.PriceType,
                                i.FactoryCost,
                                i.Margin,
                                i.FCL,
                                i.LCL,
                                i.SampleQty,
                                i.Qty,
                                i.Unit,
                                i.Amount,
                                i.Carton,
                                i.GLAccount,
                                i.RefDocNo,
                                i.ShippingMark,
                                i.QtyIN,
                                i.QtyOUT,
                                ps.SuppRef,
                                c.CurrencyCode,
                                ps.FCLCost,
                                ps.LCLCost,
                                ap.InnerBox,
                                ap.OuterBox,
                                ap.CUFT
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort - View orders by QTNumber, LineNumber DESC
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        // Fallback: order by QTNumber ascending, then LineNumber descending
                        // Dynamic LINQ uses ThenBy with " descending" suffix, not ThenByDescending
                        filteredQuery = filteredQuery.OrderBy("QTNumber").ThenBy("LineNumber descending");
                    }
                }
                else
                {
                    // Default: order by QTNumber ascending, then LineNumber descending
                    // Dynamic LINQ uses ThenBy with " descending" suffix, not ThenByDescending
                    filteredQuery = filteredQuery.OrderBy("QTNumber").ThenBy("LineNumber descending");
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        #endregion

        #region PreOrder Views

        /// <summary>
        /// Retrieves the pre-order list, replacing vwPreOrderList
        /// </summary>
        public DataSet GetPreOrderList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View uses LEFT OUTER JOINs for all joins
                var query = from pl in context.OrderPL
                            join c in context.Customer on pl.CustomerId equals c.CustomerId into cGroup
                            from c in cGroup.DefaultIfEmpty()
                            join s in context.Staff on pl.StaffId equals s.StaffId into sGroup
                            from s in sGroup.DefaultIfEmpty()
                            join s1 in context.Staff on pl.CreatedBy equals s1.StaffId into s1Group
                            from s1 in s1Group.DefaultIfEmpty()
                            join s2 in context.Staff on pl.ModifiedBy equals s2.StaffId into s2Group
                            from s2 in s2Group.DefaultIfEmpty()
                            select new
                            {
                                pl.OrderPLId,
                                pl.PLDate,
                                pl.InUse,
                                pl.Status,
                                pl.PLNumber,
                                CustomerName = c != null ? c.CustomerName : (string)null,
                                pl.Remarks,
                                pl.CreatedOn,
                                CreatedBy = s1 != null ? s1.Alias : (string)null,
                                pl.ModifiedOn,
                                ModifiedBy = s2 != null ? s2.Alias : (string)null,
                                pl.Revision,
                                pl.TotalQty,
                                pl.TotalQtyIN,
                                pl.TotalQtyOUT,
                                pl.TotalAmount,
                                pl.SendFrom,
                                pl.SendTo,
                                CustomerId = c != null ? (Guid?)c.CustomerId : null
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.PLNumber);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.PLNumber);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the pre-order item list, replacing vwPreOrderItemList
        /// </summary>
        public DataSet GetPreOrderItemList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View structure: OrderPL INNER JOIN OrderPLItems LEFT JOIN OrderQTItems
                // Then INNER JOINs on Article, ArticleSupplier, Supplier, T_Currency, T_Package, OrderQTPackage
                var query = from i in context.OrderPLItems
                            join m in context.OrderPL on i.OrderPLId equals m.OrderPLId // INNER JOIN
                            // LEFT JOIN OrderQTItems
                            let qiOrderQTItemId = i.OrderQTItemId != Guid.Empty ? (Guid?)i.OrderQTItemId : (Guid?)null
                            join qi in context.OrderQTItems on qiOrderQTItemId equals (Guid?)qi.OrderQTItemId into qiGroup
                            from qi in qiGroup.DefaultIfEmpty() // LEFT JOIN
                            // Compute keys for subsequent joins
                            let qiArticleId = qi != null ? (Guid?)qi.ArticleId : (Guid?)null
                            let qiSupplierId = qi != null ? (Guid?)qi.SupplierId : (Guid?)null
                            let qiPackageId = qi != null ? qi.PackageId : (Guid?)null
                            let qiOrderQTItemId2 = qi != null ? (Guid?)qi.OrderQTItemId : (Guid?)null
                            // Join Article
                            join a in context.Article on qiArticleId equals (Guid?)a.ArticleId into aGroup
                            from a in aGroup.DefaultIfEmpty()
                            // ArticleSupplier join
                            join ps in context.ArticleSupplier on
                                new { ArticleId = qiArticleId, SupplierId = qiSupplierId }
                                equals new { ArticleId = (Guid?)ps.ArticleId, SupplierId = (Guid?)ps.SupplierId } into psGroup
                            from ps in psGroup.DefaultIfEmpty()
                            // Compute keys for remaining joins
                            let psSupplierId = ps != null ? (Guid?)ps.SupplierId : (Guid?)null
                            let psCurrencyId = ps != null ? ps.CurrencyId : (Guid?)null
                            // Join Supplier
                            join s in context.Supplier on psSupplierId equals (Guid?)s.SupplierId into sGroup
                            from s in sGroup.DefaultIfEmpty()
                            // Join Currency
                            join c in context.T_Currency on psCurrencyId equals (Guid?)c.CurrencyId into cGroup
                            from c in cGroup.DefaultIfEmpty()
                            // Join Package
                            join p in context.T_Package on qiPackageId equals (Guid?)p.PackageId into pGroup
                            from p in pGroup.DefaultIfEmpty()
                            // Join OrderQTPackage
                            join ap in context.OrderQTPackage on qiOrderQTItemId2 equals (Guid?)ap.OrderQTItemId into apGroup
                            from ap in apGroup.DefaultIfEmpty()
                            select new
                            {
                                m.OrderPLId,
                                m.PLNumber,
                                m.PLDate,
                                i.OrderPLItemsId,
                                i.LineNumber,
                                ArticleId = a != null ? (Guid?)a.ArticleId : null,
                                SKU = a != null ? a.SKU : (string)null,
                                ArticleCode = a != null ? a.ArticleCode : (string)null,
                                ArticleName = a != null ? a.ArticleName : (string)null,
                                ArticleName_Chs = a != null ? a.ArticleName_Chs : (string)null,
                                ArticleName_Cht = a != null ? a.ArticleName_Cht : (string)null,
                                PackageId = p != null ? (Guid?)p.PackageId : null,
                                PackageCode = p != null ? p.PackageCode : (string)null,
                                PackageName = p != null ? p.PackageName : (string)null,
                                PackageName_Chs = p != null ? p.PackageName_Chs : (string)null,
                                PackageName_Cht = p != null ? p.PackageName_Cht : (string)null,
                                SupplierId = s != null ? (Guid?)s.SupplierId : null,
                                SupplierCode = s != null ? s.SupplierCode : (string)null,
                                SupplierName = s != null ? s.SupplierName : (string)null,
                                SupplierName_Chs = s != null ? s.SupplierName_Chs : (string)null,
                                SupplierName_Cht = s != null ? s.SupplierName_Cht : (string)null,
                                Particular = qi != null ? qi.Particular : (string)null,
                                CustRef = qi != null ? qi.CustRef : (string)null,
                                PriceType = qi != null ? (int?)qi.PriceType : null,
                                FactoryCost = qi != null ? (decimal?)qi.FactoryCost : null,
                                Margin = qi != null ? (decimal?)qi.Margin : null,
                                FCL = qi != null ? (decimal?)qi.FCL : null,
                                LCL = qi != null ? (decimal?)qi.LCL : null,
                                SampleQty = qi != null ? (decimal?)qi.SampleQty : null,
                                Qty = qi != null ? (decimal?)qi.Qty : null,
                                Unit = qi != null ? qi.Unit : (string)null,
                                Amount = qi != null ? (decimal?)qi.Amount : null,
                                Carton = qi != null ? (decimal?)qi.Carton : null,
                                GLAccount = qi != null ? qi.GLAccount : (string)null,
                                RefDocNo = qi != null ? qi.RefDocNo : (string)null,
                                ShippingMark = qi != null ? qi.ShippingMark : (string)null,
                                QtyIN = qi != null ? (decimal?)qi.QtyIN : null,
                                QtyOUT = qi != null ? (decimal?)qi.QtyOUT : null,
                                SuppRef = ps != null ? ps.SuppRef : (string)null,
                                CurrencyCode = c != null ? c.CurrencyCode : (string)null,
                                FCLCost = ps != null ? (decimal?)ps.FCLCost : null,
                                LCLCost = ps != null ? (decimal?)ps.LCLCost : null,
                                InnerBox = ap != null ? (decimal?)ap.InnerBox : null,
                                OuterBox = ap != null ? (decimal?)ap.OuterBox : null,
                                CUFT = ap != null ? (decimal?)ap.CUFT : null
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort - View orders by PLNumber, LineNumber DESC
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.PLNumber).ThenByDescending(x => x.LineNumber);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.PLNumber).ThenByDescending(x => x.LineNumber);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        #endregion

        #region SalesContract Views

        /// <summary>
        /// Retrieves the sales contract list, replacing vwSalesContractList
        /// </summary>
        public DataSet GetSalesContractList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View uses LEFT OUTER JOINs for all joins
                var query = from sc in context.OrderSC
                            join c in context.Customer on sc.CustomerId equals c.CustomerId into cGroup
                            from c in cGroup.DefaultIfEmpty()
                            join s in context.Staff on sc.StaffId equals s.StaffId into sGroup
                            from s in sGroup.DefaultIfEmpty()
                            join s1 in context.Staff on sc.CreatedBy equals s1.StaffId into s1Group
                            from s1 in s1Group.DefaultIfEmpty()
                            join s2 in context.Staff on sc.ModifiedBy equals s2.StaffId into s2Group
                            from s2 in s2Group.DefaultIfEmpty()
                            join s3 in context.Staff on sc.AccessedBy equals s3.StaffId into s3Group
                            from s3 in s3Group.DefaultIfEmpty()
                            select new
                            {
                                sc.OrderSCId,
                                sc.SCNumber,
                                sc.SCDate,
                                CustomerId = c != null ? (Guid?)c.CustomerId : null,
                                CustomerCode = c != null ? c.CustomerCode : (string)null,
                                CustomerName = c != null ? c.CustomerName : (string)null,
                                CustomerName_Chs = c != null ? c.CustomerName_Chs : (string)null,
                                CustomerName_Cht = c != null ? c.CustomerName_Cht : (string)null,
                                StaffId = s != null ? (Guid?)s.StaffId : null,
                                SalePerson = s != null ? s.Alias : (string)null,
                                sc.SendFrom,
                                sc.SendTo,
                                sc.Remarks,
                                sc.Remarks2,
                                sc.Remarks3,
                                sc.Revision,
                                sc.InUse,
                                sc.Status,
                                sc.CreatedOn,
                                CreatedBy = s1 != null ? s1.Alias : (string)null,
                                sc.ModifiedOn,
                                ModifiedBy = s2 != null ? s2.Alias : (string)null,
                                sc.AccessedOn,
                                AccessedBy = s3 != null ? s3.Alias : (string)null,
                                sc.Retired
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort - View orders by SCNumber
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.SCNumber);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.SCNumber);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the sales contract item list, replacing vwSalesContractItemList
        /// </summary>
        public DataSet GetSalesContractItemList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View structure: OrderSC INNER JOIN OrderSCItems LEFT JOIN OrderQTItems
                // Then INNER JOINs on Article, ArticleSupplier, Supplier, T_Currency, T_Package, OrderQTPackage
                var query = from i in context.OrderSCItems
                            join m in context.OrderSC on i.OrderSCId equals m.OrderSCId // INNER JOIN
                            // LEFT JOIN OrderQTItems
                            let qiOrderQTItemId = i.OrderQTItemId != Guid.Empty ? (Guid?)i.OrderQTItemId : (Guid?)null
                            join qi in context.OrderQTItems on qiOrderQTItemId equals (Guid?)qi.OrderQTItemId into qiGroup
                            from qi in qiGroup.DefaultIfEmpty() // LEFT JOIN
                            // Compute keys for subsequent joins
                            let qiArticleId = qi != null ? (Guid?)qi.ArticleId : (Guid?)null
                            let qiSupplierId = qi != null ? (Guid?)qi.SupplierId : (Guid?)null
                            let qiPackageId = qi != null ? qi.PackageId : (Guid?)null
                            let qiOrderQTItemId2 = qi != null ? (Guid?)qi.OrderQTItemId : (Guid?)null
                            // Join Article
                            join a in context.Article on qiArticleId equals (Guid?)a.ArticleId into aGroup
                            from a in aGroup.DefaultIfEmpty()
                            // ArticleSupplier join
                            join ps in context.ArticleSupplier on
                                new { ArticleId = qiArticleId, SupplierId = qiSupplierId }
                                equals new { ArticleId = (Guid?)ps.ArticleId, SupplierId = (Guid?)ps.SupplierId } into psGroup
                            from ps in psGroup.DefaultIfEmpty()
                            // Compute keys for remaining joins
                            let psSupplierId = ps != null ? (Guid?)ps.SupplierId : (Guid?)null
                            let psCurrencyId = ps != null ? ps.CurrencyId : (Guid?)null
                            // Join Supplier
                            join s in context.Supplier on psSupplierId equals (Guid?)s.SupplierId into sGroup
                            from s in sGroup.DefaultIfEmpty()
                            // Join Currency
                            join c in context.T_Currency on psCurrencyId equals (Guid?)c.CurrencyId into cGroup
                            from c in cGroup.DefaultIfEmpty()
                            // Join Package
                            join p in context.T_Package on qiPackageId equals (Guid?)p.PackageId into pGroup
                            from p in pGroup.DefaultIfEmpty()
                            // Join OrderQTPackage
                            join ap in context.OrderQTPackage on qiOrderQTItemId2 equals (Guid?)ap.OrderQTItemId into apGroup
                            from ap in apGroup.DefaultIfEmpty()
                            select new
                            {
                                m.OrderSCId,
                                m.SCNumber,
                                m.SCDate,
                                i.OrderSCItemsId,
                                i.LineNumber,
                                ArticleId = a != null ? (Guid?)a.ArticleId : null,
                                SKU = a != null ? a.SKU : (string)null,
                                ArticleCode = a != null ? a.ArticleCode : (string)null,
                                ArticleName = a != null ? a.ArticleName : (string)null,
                                ArticleName_Chs = a != null ? a.ArticleName_Chs : (string)null,
                                ArticleName_Cht = a != null ? a.ArticleName_Cht : (string)null,
                                PackageId = p != null ? (Guid?)p.PackageId : null,
                                PackageCode = p != null ? p.PackageCode : (string)null,
                                PackageName = p != null ? p.PackageName : (string)null,
                                PackageName_Chs = p != null ? p.PackageName_Chs : (string)null,
                                PackageName_Cht = p != null ? p.PackageName_Cht : (string)null,
                                SupplierId = s != null ? (Guid?)s.SupplierId : null,
                                SupplierCode = s != null ? s.SupplierCode : (string)null,
                                SupplierName = s != null ? s.SupplierName : (string)null,
                                SupplierName_Chs = s != null ? s.SupplierName_Chs : (string)null,
                                SupplierName_Cht = s != null ? s.SupplierName_Cht : (string)null,
                                Particular = qi != null ? qi.Particular : (string)null,
                                CustRef = qi != null ? qi.CustRef : (string)null,
                                PriceType = qi != null ? (int?)qi.PriceType : null,
                                FactoryCost = qi != null ? (decimal?)qi.FactoryCost : null,
                                Margin = qi != null ? (decimal?)qi.Margin : null,
                                FCL = qi != null ? (decimal?)qi.FCL : null,
                                LCL = qi != null ? (decimal?)qi.LCL : null,
                                SampleQty = qi != null ? (decimal?)qi.SampleQty : null,
                                Qty = qi != null ? (decimal?)qi.Qty : null,
                                Unit = qi != null ? qi.Unit : (string)null,
                                Amount = qi != null ? (decimal?)qi.Amount : null,
                                Carton = qi != null ? (decimal?)qi.Carton : null,
                                GLAccount = qi != null ? qi.GLAccount : (string)null,
                                RefDocNo = qi != null ? qi.RefDocNo : (string)null,
                                ShippingMark = qi != null ? qi.ShippingMark : (string)null,
                                QtyIN = qi != null ? (decimal?)qi.QtyIN : null,
                                QtyOUT = qi != null ? (decimal?)qi.QtyOUT : null,
                                SuppRef = ps != null ? ps.SuppRef : (string)null,
                                CurrencyCode = c != null ? c.CurrencyCode : (string)null,
                                FCLCost = ps != null ? (decimal?)ps.FCLCost : null,
                                LCLCost = ps != null ? (decimal?)ps.LCLCost : null,
                                OrderQTItemId = qiOrderQTItemId2,
                                InnerBox = ap != null ? (decimal?)ap.InnerBox : null,
                                OuterBox = ap != null ? (decimal?)ap.OuterBox : null,
                                CUFT = ap != null ? (decimal?)ap.CUFT : null
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort - View orders by SCNumber, LineNumber DESC
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.SCNumber).ThenByDescending(x => x.LineNumber);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.SCNumber).ThenByDescending(x => x.LineNumber);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        #endregion

        #region PurchaseContract Views

        /// <summary>
        /// Retrieves the purchase contract list, replacing vwPurchaseContractList
        /// Note: View uses DISTINCT and complex RIGHT OUTER JOINs with ISNULL
        /// </summary>
        public DataSet GetPurchaseContractList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View structure: OrderQT -> OrderQTItems -> OrderSCItems -> OrderPCItems -> OrderPC
                // Uses RIGHT OUTER JOINs, which in LINQ we handle by starting from OrderPC and LEFT JOINing backwards
                var query = from pc in context.OrderPC
                            join pci in context.OrderPCItems on pc.OrderPCId equals pci.OrderPCId into pciGroup
                            from pci in pciGroup.DefaultIfEmpty()
                            // Continue the chain backwards
                            let sciOrderSCItemsId = pci != null ? (Guid?)pci.OrderSCItemsId : (Guid?)null
                            join sci in context.OrderSCItems on sciOrderSCItemsId equals (Guid?)sci.OrderSCItemsId into sciGroup
                            from sci in sciGroup.DefaultIfEmpty()
                            let qtiOrderQTItemId = sci != null ? (Guid?)sci.OrderQTItemId : (Guid?)null
                            join qti in context.OrderQTItems on qtiOrderQTItemId equals (Guid?)qti.OrderQTItemId into qtiGroup
                            from qti in qtiGroup.DefaultIfEmpty()
                            let qtOrderQTId = qti != null ? (Guid?)qti.OrderQTId : (Guid?)null
                            join qt in context.OrderQT on qtOrderQTId equals (Guid?)qt.OrderQTId into qtGroup
                            from qt in qtGroup.DefaultIfEmpty()
                            // Now join the other tables
                            join supp in context.Supplier on pc.SupplierId equals supp.SupplierId into suppGroup
                            from supp in suppGroup.DefaultIfEmpty()
                            join s in context.Staff on pc.StaffId equals s.StaffId into sGroup
                            from s in sGroup.DefaultIfEmpty()
                            join s1 in context.Staff on pc.CreatedBy equals s1.StaffId into s1Group
                            from s1 in s1Group.DefaultIfEmpty()
                            join s2 in context.Staff on pc.ModifiedBy equals s2.StaffId into s2Group
                            from s2 in s2Group.DefaultIfEmpty()
                            join s3 in context.Staff on pc.AccessedBy equals s3.StaffId into s3Group
                            from s3 in s3Group.DefaultIfEmpty()
                            select new
                            {
                                pc.OrderPCId,
                                pc.PCNumber,
                                pc.PCDate,
                                SupplierId = supp != null ? (Guid?)supp.SupplierId : null,
                                SupplierCode = supp != null ? supp.SupplierCode : (string)null,
                                SupplierName = supp != null ? supp.SupplierName : (string)null,
                                SupplierName_Chs = supp != null ? supp.SupplierName_Chs : (string)null,
                                SupplierName_Cht = supp != null ? supp.SupplierName_Cht : (string)null,
                                StaffId = s != null ? (Guid?)s.StaffId : null,
                                SalePerson = s != null ? s.Alias : (string)null,
                                pc.Remarks,
                                pc.Remarks2,
                                pc.Remarks3,
                                pc.Revision,
                                pc.InUse,
                                pc.Status,
                                pc.CreatedOn,
                                CreatedBy = s1 != null ? s1.Alias : (string)null,
                                pc.ModifiedOn,
                                ModifiedBy = s2 != null ? s2.Alias : (string)null,
                                pc.AccessedOn,
                                AccessedBy = s3 != null ? s3.Alias : (string)null,
                                pc.Retired,
                                SampleQty = qt != null ? (decimal?)qt.SampleQty : 0m
                            };

                // Apply DISTINCT (View uses DISTINCT)
                var distinctQuery = query.Distinct();

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(distinctQuery, whereClause);

                // Apply dynamic sort - View orders by PCNumber
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.PCNumber);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.PCNumber);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the purchase contract item list, replacing vwPurchaseContractItemList
        /// </summary>
        public DataSet GetPurchaseContractItemList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // Similar structure to other item lists
                var query = from i in context.OrderPCItems
                            join m in context.OrderPC on i.OrderPCId equals m.OrderPCId // INNER JOIN
                            // Continue with similar pattern to other item lists
                            let sciOrderSCItemsId = i.OrderSCItemsId != Guid.Empty ? (Guid?)i.OrderSCItemsId : (Guid?)null
                            join sci in context.OrderSCItems on sciOrderSCItemsId equals (Guid?)sci.OrderSCItemsId into sciGroup
                            from sci in sciGroup.DefaultIfEmpty()
                            let qiOrderQTItemId = sci != null ? (Guid?)sci.OrderQTItemId : (Guid?)null
                            join qi in context.OrderQTItems on qiOrderQTItemId equals (Guid?)qi.OrderQTItemId into qiGroup
                            from qi in qiGroup.DefaultIfEmpty()
                            // Compute keys for subsequent joins
                            let qiArticleId = qi != null ? (Guid?)qi.ArticleId : (Guid?)null
                            let qiSupplierId = qi != null ? (Guid?)qi.SupplierId : (Guid?)null
                            let qiPackageId = qi != null ? qi.PackageId : (Guid?)null
                            let qiOrderQTItemId2 = qi != null ? (Guid?)qi.OrderQTItemId : (Guid?)null
                            // Join Article
                            join a in context.Article on qiArticleId equals (Guid?)a.ArticleId into aGroup
                            from a in aGroup.DefaultIfEmpty()
                            // ArticleSupplier join
                            join ps in context.ArticleSupplier on
                                new { ArticleId = qiArticleId, SupplierId = qiSupplierId }
                                equals new { ArticleId = (Guid?)ps.ArticleId, SupplierId = (Guid?)ps.SupplierId } into psGroup
                            from ps in psGroup.DefaultIfEmpty()
                            // Compute keys for remaining joins
                            let psSupplierId = ps != null ? (Guid?)ps.SupplierId : (Guid?)null
                            let psCurrencyId = ps != null ? ps.CurrencyId : (Guid?)null
                            // Join Supplier
                            join s in context.Supplier on psSupplierId equals (Guid?)s.SupplierId into sGroup
                            from s in sGroup.DefaultIfEmpty()
                            // Join Currency
                            join c in context.T_Currency on psCurrencyId equals (Guid?)c.CurrencyId into cGroup
                            from c in cGroup.DefaultIfEmpty()
                            // Join Package
                            join p in context.T_Package on qiPackageId equals (Guid?)p.PackageId into pGroup
                            from p in pGroup.DefaultIfEmpty()
                            // Join OrderQTPackage
                            join ap in context.OrderQTPackage on qiOrderQTItemId2 equals (Guid?)ap.OrderQTItemId into apGroup
                            from ap in apGroup.DefaultIfEmpty()
                            select new
                            {
                                m.OrderPCId,
                                m.PCNumber,
                                m.PCDate,
                                i.OrderPCItemsId,
                                i.LineNumber,
                                ArticleId = a != null ? (Guid?)a.ArticleId : null,
                                SKU = a != null ? a.SKU : (string)null,
                                ArticleCode = a != null ? a.ArticleCode : (string)null,
                                ArticleName = a != null ? a.ArticleName : (string)null,
                                ArticleName_Chs = a != null ? a.ArticleName_Chs : (string)null,
                                ArticleName_Cht = a != null ? a.ArticleName_Cht : (string)null,
                                PackageId = p != null ? (Guid?)p.PackageId : null,
                                PackageCode = p != null ? p.PackageCode : (string)null,
                                PackageName = p != null ? p.PackageName : (string)null,
                                PackageName_Chs = p != null ? p.PackageName_Chs : (string)null,
                                PackageName_Cht = p != null ? p.PackageName_Cht : (string)null,
                                SupplierId = s != null ? (Guid?)s.SupplierId : null,
                                SupplierCode = s != null ? s.SupplierCode : (string)null,
                                SupplierName = s != null ? s.SupplierName : (string)null,
                                SupplierName_Chs = s != null ? s.SupplierName_Chs : (string)null,
                                SupplierName_Cht = s != null ? s.SupplierName_Cht : (string)null,
                                Particular = qi != null ? qi.Particular : (string)null,
                                CustRef = qi != null ? qi.CustRef : (string)null,
                                PriceType = qi != null ? (int?)qi.PriceType : null,
                                FactoryCost = qi != null ? (decimal?)qi.FactoryCost : null,
                                Margin = qi != null ? (decimal?)qi.Margin : null,
                                FCL = qi != null ? (decimal?)qi.FCL : null,
                                LCL = qi != null ? (decimal?)qi.LCL : null,
                                SampleQty = qi != null ? (decimal?)qi.SampleQty : null,
                                Qty = qi != null ? (decimal?)qi.Qty : null,
                                Unit = qi != null ? qi.Unit : (string)null,
                                Amount = qi != null ? (decimal?)qi.Amount : null,
                                Carton = qi != null ? (decimal?)qi.Carton : null,
                                GLAccount = qi != null ? qi.GLAccount : (string)null,
                                RefDocNo = qi != null ? qi.RefDocNo : (string)null,
                                ShippingMark = qi != null ? qi.ShippingMark : (string)null,
                                QtyIN = qi != null ? (decimal?)qi.QtyIN : null,
                                QtyOUT = qi != null ? (decimal?)qi.QtyOUT : null,
                                SuppRef = ps != null ? ps.SuppRef : (string)null,
                                CurrencyCode = c != null ? c.CurrencyCode : (string)null,
                                FCLCost = ps != null ? (decimal?)ps.FCLCost : null,
                                LCLCost = ps != null ? (decimal?)ps.LCLCost : null,
                                InnerBox = ap != null ? (decimal?)ap.InnerBox : null,
                                OuterBox = ap != null ? (decimal?)ap.OuterBox : null,
                                CUFT = ap != null ? (decimal?)ap.CUFT : null
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.PCNumber).ThenByDescending(x => x.LineNumber);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.PCNumber).ThenByDescending(x => x.LineNumber);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        #endregion

        #region Sample Views

        /// <summary>
        /// Retrieves the sample list, replacing vwOrderSPList
        /// </summary>
        public DataSet GetSampleList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View uses LEFT OUTER JOINs for all joins
                var query = from sp in context.OrderSP
                            join c in context.Customer on sp.CustomerId equals c.CustomerId into cGroup
                            from c in cGroup.DefaultIfEmpty()
                            join s in context.Staff on sp.StaffId equals s.StaffId into sGroup
                            from s in sGroup.DefaultIfEmpty()
                            join s1 in context.Staff on sp.CreatedBy equals s1.StaffId into s1Group
                            from s1 in s1Group.DefaultIfEmpty()
                            join s2 in context.Staff on sp.ModifiedBy equals s2.StaffId into s2Group
                            from s2 in s2Group.DefaultIfEmpty()
                            select new
                            {
                                sp.OrderSPId,
                                sp.SPDate,
                                sp.InUse,
                                sp.Status,
                                sp.SPNumber,
                                CustomerName = c != null ? c.CustomerName : (string)null,
                                Salesperson = s != null ? s.Alias : (string)null,
                                sp.Remarks,
                                sp.CreatedOn,
                                CreatedBy = s1 != null ? s1.Alias : (string)null,
                                sp.ModifiedOn,
                                ModifiedBy = s2 != null ? s2.Alias : (string)null,
                                sp.Revision,
                                sp.SendFrom,
                                sp.SendTo,
                                CustomerId = c != null ? (Guid?)c.CustomerId : null,
                                CustomerCode = c != null ? c.CustomerCode : (string)null,
                                CustomerName_Chs = c != null ? c.CustomerName_Chs : (string)null,
                                CustomerName_Cht = c != null ? c.CustomerName_Cht : (string)null
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.SPNumber);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.SPNumber);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the sample item list, replacing vwOrderSPItemList
        /// </summary>
        public DataSet GetSampleItemList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View structure: OrderSP INNER JOIN OrderSPItems LEFT JOIN OrderQTItems
                // Then INNER JOINs on Article, ArticlePackage, T_Package, ArticleSupplier, Supplier, T_Currency, OrderQT
                var query = from i in context.OrderSPItems
                            join m in context.OrderSP on i.OrderSPId equals m.OrderSPId // INNER JOIN
                            // LEFT JOIN OrderQTItems
                            let qiOrderQTItemId = i.OrderQTItemId != Guid.Empty ? (Guid?)i.OrderQTItemId : (Guid?)null
                            join qi in context.OrderQTItems on qiOrderQTItemId equals (Guid?)qi.OrderQTItemId into qiGroup
                            from qi in qiGroup.DefaultIfEmpty() // LEFT JOIN
                            // Compute keys for subsequent joins
                            let qiArticleId = qi != null ? (Guid?)qi.ArticleId : (Guid?)null
                            let qiSupplierId = qi != null ? (Guid?)qi.SupplierId : (Guid?)null
                            let qiPackageId = qi != null ? qi.PackageId : (Guid?)null
                            let qiOrderQTId = qi != null ? (Guid?)qi.OrderQTId : (Guid?)null
                            // Join Article
                            join a in context.Article on qiArticleId equals (Guid?)a.ArticleId into aGroup
                            from a in aGroup.DefaultIfEmpty()
                            // Join ArticlePackage (requires both ArticleId and PackageId)
                            join ap in context.ArticlePackage on
                                new { ArticleId = qiArticleId, PackageId = qiPackageId }
                                equals new { ArticleId = (Guid?)ap.ArticleId, PackageId = (Guid?)ap.PackageId } into apGroup
                            from ap in apGroup.DefaultIfEmpty()
                            // Join T_Package through ArticlePackage
                            let apPackageId = ap != null ? (Guid?)ap.PackageId : null
                            join p in context.T_Package on apPackageId equals (Guid?)p.PackageId into pGroup
                            from p in pGroup.DefaultIfEmpty()
                            // ArticleSupplier join
                            join ps in context.ArticleSupplier on
                                new { ArticleId = qiArticleId, SupplierId = qiSupplierId }
                                equals new { ArticleId = (Guid?)ps.ArticleId, SupplierId = (Guid?)ps.SupplierId } into psGroup
                            from ps in psGroup.DefaultIfEmpty()
                            // Compute keys for remaining joins
                            let psSupplierId = ps != null ? (Guid?)ps.SupplierId : (Guid?)null
                            let psCurrencyId = ps != null ? ps.CurrencyId : (Guid?)null
                            // Join Supplier
                            join s in context.Supplier on psSupplierId equals (Guid?)s.SupplierId into sGroup
                            from s in sGroup.DefaultIfEmpty()
                            // Join Currency
                            join c in context.T_Currency on psCurrencyId equals (Guid?)c.CurrencyId into cGroup
                            from c in cGroup.DefaultIfEmpty()
                            // Join OrderQT for QTNumber subquery
                            join qt in context.OrderQT on qiOrderQTId equals (Guid?)qt.OrderQTId into qtGroup
                            from qt in qtGroup.DefaultIfEmpty()
                            select new
                            {
                                m.OrderSPId,
                                m.SPNumber,
                                m.SPDate,
                                i.OrderSPItemsId,
                                i.LineNumber,
                                ArticleId = a != null ? (Guid?)a.ArticleId : null,
                                SKU = a != null ? a.SKU : (string)null,
                                ArticleCode = a != null ? a.ArticleCode : (string)null,
                                ArticleName = a != null ? a.ArticleName : (string)null,
                                ArticleName_Chs = a != null ? a.ArticleName_Chs : (string)null,
                                ArticleName_Cht = a != null ? a.ArticleName_Cht : (string)null,
                                PackageId = p != null ? (Guid?)p.PackageId : null,
                                PackageCode = p != null ? p.PackageCode : (string)null,
                                PackageName = p != null ? p.PackageName : (string)null,
                                PackageName_Chs = p != null ? p.PackageName_Chs : (string)null,
                                PackageName_Cht = p != null ? p.PackageName_Cht : (string)null,
                                SupplierId = s != null ? (Guid?)s.SupplierId : null,
                                SupplierCode = s != null ? s.SupplierCode : (string)null,
                                SupplierName = s != null ? s.SupplierName : (string)null,
                                SupplierName_Chs = s != null ? s.SupplierName_Chs : (string)null,
                                SupplierName_Cht = s != null ? s.SupplierName_Cht : (string)null,
                                Particular = qi != null ? qi.Particular : (string)null,
                                CustRef = qi != null ? qi.CustRef : (string)null,
                                PriceType = qi != null ? (int?)qi.PriceType : null,
                                FactoryCost = qi != null ? (decimal?)qi.FactoryCost : null,
                                Margin = qi != null ? (decimal?)qi.Margin : null,
                                FCL = qi != null ? (decimal?)qi.FCL : null,
                                LCL = qi != null ? (decimal?)qi.LCL : null,
                                SampleQty = qi != null ? (decimal?)qi.SampleQty : null,
                                Qty = i.Qty,
                                Unit = i.Unit,
                                Amount = qi != null ? (decimal?)qi.Amount : null,
                                Carton = qi != null ? (decimal?)qi.Carton : null,
                                GLAccount = qi != null ? qi.GLAccount : (string)null,
                                RefDocNo = qi != null ? qi.RefDocNo : (string)null,
                                ShippingMark = qi != null ? qi.ShippingMark : (string)null,
                                QtyIN = qi != null ? (decimal?)qi.QtyIN : null,
                                QtyOUT = qi != null ? (decimal?)qi.QtyOUT : null,
                                SuppRef = ps != null ? ps.SuppRef : (string)null,
                                CurrencyCode = c != null ? c.CurrencyCode : (string)null,
                                FCLCost = ps != null ? (decimal?)ps.FCLCost : null,
                                LCLCost = ps != null ? (decimal?)ps.LCLCost : null,
                                InnerBox = ap != null ? (decimal?)ap.InnerBox : null,
                                OuterBox = ap != null ? (decimal?)ap.OuterBox : null,
                                CUFT = ap != null ? (decimal?)ap.CUFT : null,
                                QTLineNumber = qi != null ? (int?)qi.LineNumber : null,
                                QTNumber = qt != null ? qt.QTNumber : (string)null
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort - View orders by SPNumber, LineNumber DESC
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.SPNumber).ThenByDescending(x => x.LineNumber);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.SPNumber).ThenByDescending(x => x.LineNumber);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the sample item record, replacing vwOSSample
        /// </summary>
        public DataSet GetSampleItemRecord(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View structure: Complex RIGHT OUTER JOINs starting from T_Package
                // We'll restructure to start from OrderQTItems and LEFT JOIN
                var query = from qi in context.OrderQTItems
                            where qi.OrderQTItemId != Guid.Empty // View has WHERE OrderQTItemId IS NOT NULL
                            join a in context.Article on qi.ArticleId equals a.ArticleId into aGroup
                            from a in aGroup.DefaultIfEmpty()
                            join p in context.T_Package on qi.PackageId equals p.PackageId into pGroup
                            from p in pGroup.DefaultIfEmpty()
                            join qt in context.OrderQT on qi.OrderQTId equals qt.OrderQTId into qtGroup
                            from qt in qtGroup.DefaultIfEmpty()
                            join s in context.Supplier on qi.SupplierId equals s.SupplierId into sGroup
                            from s in sGroup.DefaultIfEmpty()
                            join c in context.Customer on (qt != null ? qt.CustomerId : Guid.Empty) equals c.CustomerId into cGroup
                            from c in cGroup.DefaultIfEmpty()
                            select new
                            {
                                CustomerId = c != null ? (Guid?)c.CustomerId : null,
                                CustName = c != null ? c.CustomerName : (string)null,
                                CustRef = qi.CustRef,
                                ArticleCode = a != null ? a.ArticleCode : (string)null,
                                ArtName = a != null ? a.ArticleName : (string)null,
                                Amount = qi.Amount,
                                SampleQty = qi.SampleQty,
                                Unit = qt != null ? qt.Unit : (string)null,
                                SupplierName = s != null ? s.SupplierName : (string)null,
                                QTNumber = qt != null ? qt.QTNumber : (string)null,
                                QtyOUT = qi.QtyOUT,
                                PackageCode = p != null ? p.PackageCode : (string)null,
                                PackageName = p != null ? p.PackageName : (string)null,
                                SupplierCode = s != null ? s.SupplierCode : (string)null,
                                QTDate = qt != null ? (DateTime?)qt.QTDate : null,
                                OrderQTItemId = qi.OrderQTItemId
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.QTNumber);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.QTNumber);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        #endregion

        #region History Views

        /// <summary>
        /// Retrieves the quote history, replacing vwQuoteHistory
        /// </summary>
        public DataSet GetQuoteHistory(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View uses LEFT JOINs and subqueries for CurrencyCode
                var query = from qi in context.OrderQTItems
                            join qt in context.OrderQT on qi.OrderQTId equals qt.OrderQTId into qtGroup
                            from qt in qtGroup.DefaultIfEmpty()
                            join c in context.Customer on (qt != null ? qt.CustomerId : Guid.Empty) equals c.CustomerId into cGroup
                            from c in cGroup.DefaultIfEmpty()
                            join qts in context.OrderQTSupplier on qi.OrderQTItemId equals qts.OrderQTItemId into qtsGroup
                            from qts in qtsGroup.DefaultIfEmpty()
                            join qtp in context.OrderQTPackage on (qts != null ? qts.OrderQTItemId : Guid.Empty) equals qtp.OrderQTItemId into qtpGroup
                            from qtp in qtpGroup.DefaultIfEmpty()
                            join a in context.Article on qi.ArticleId equals a.ArticleId into aGroup
                            from a in aGroup.DefaultIfEmpty()
                            join p in context.T_Package on qi.PackageId equals p.PackageId into pGroup
                            from p in pGroup.DefaultIfEmpty()
                            join s in context.Supplier on qi.SupplierId equals s.SupplierId into sGroup
                            from s in sGroup.DefaultIfEmpty()
                            // Subqueries for CurrencyCode
                            let currencyCode = (from curr in context.T_Currency
                                               where qt != null && curr.CurrencyId == qt.CurrencyId
                                               select curr.CurrencyCode).FirstOrDefault()
                            let currencyUsed = (from curr in context.T_Currency
                                               where qts != null && curr.CurrencyId == qts.CurrencyId
                                               select curr.CurrencyCode).FirstOrDefault()
                            select new
                            {
                                OrderQTItemId = qi.OrderQTItemId,
                                ArticleCode = a != null ? a.ArticleCode : "",
                                SupplierCode = s != null ? s.SupplierCode : "",
                                PackageCode = p != null ? p.PackageCode : "",
                                CustomerName = c != null ? c.CustomerName : "",
                                CustRef = qi.CustRef ?? "",
                                QTDate = qt != null ? (DateTime?)qt.QTDate : null,
                                QTNumber = qt != null ? qt.QTNumber : "",
                                Margin = qi.Margin,
                                PriceType = qi.PriceType == 0 ? "C" : (qi.PriceType == 1 ? "F" : (qi.PriceType == 2 ? "L" : "")),
                                Amount = qi.Amount,
                                CurrencyCode = currencyCode ?? "",
                                FactoryCost = qi.FactoryCost,
                                CurrencyUsed = currencyUsed ?? "",
                                InnerBox = qtp != null ? (decimal?)qtp.InnerBox : 0m,
                                OuterBox = qtp != null ? (decimal?)qtp.OuterBox : 0m,
                                CUFT = qtp != null ? (decimal?)qtp.CUFT : 0m,
                                Unit = qi.Unit ?? "",
                                SKU = a != null ? a.SKU : ""
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.QTNumber);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.QTNumber);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the purchase history, replacing vwPurchaseHistory
        /// Note: This View has very complex nested LEFT JOINs
        /// </summary>
        public DataSet GetPurchaseHistory(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View structure is very complex with nested LEFT JOINs
                // Starting from OrderQTSuppShipping and working backwards
                var query = from qts in context.OrderQTSuppShipping
                            join qi in context.OrderQTItems on qts.OrderQTItemId equals qi.OrderQTItemId into qiGroup
                            from qi in qiGroup.DefaultIfEmpty()
                            join qtp in context.OrderQTPackage on (qi != null ? qi.OrderQTItemId : Guid.Empty) equals qtp.OrderQTItemId into qtpGroup
                            from qtp in qtpGroup.DefaultIfEmpty()
                            join qtsupp in context.OrderQTSupplier on (qtp != null ? qtp.OrderQTItemId : Guid.Empty) equals qtsupp.OrderQTItemId into qtsuppGroup
                            from qtsupp in qtsuppGroup.DefaultIfEmpty()
                            join s in context.Supplier on (qi != null ? qi.SupplierId : Guid.Empty) equals s.SupplierId into sGroup
                            from s in sGroup.DefaultIfEmpty()
                            join sci in context.OrderSCItems on (qi != null ? qi.OrderQTItemId : Guid.Empty) equals sci.OrderQTItemId into sciGroup
                            from sci in sciGroup.DefaultIfEmpty()
                            join pci in context.OrderPCItems on (sci != null ? sci.OrderSCItemsId : Guid.Empty) equals pci.OrderSCItemsId into pciGroup
                            from pci in pciGroup.DefaultIfEmpty()
                            join pc in context.OrderPC on (pci != null ? pci.OrderPCId : Guid.Empty) equals pc.OrderPCId into pcGroup
                            from pc in pcGroup.DefaultIfEmpty()
                            join a in context.Article on (qi != null ? qi.ArticleId : Guid.Empty) equals a.ArticleId into aGroup
                            from a in aGroup.DefaultIfEmpty()
                            join qt in context.OrderQT on (qi != null ? qi.OrderQTId : Guid.Empty) equals qt.OrderQTId into qtGroup
                            from qt in qtGroup.DefaultIfEmpty()
                            join c in context.Customer on (qt != null ? qt.CustomerId : Guid.Empty) equals c.CustomerId into cGroup
                            from c in cGroup.DefaultIfEmpty()
                            join p in context.T_Package on (qi != null ? qi.PackageId : Guid.Empty) equals p.PackageId into pGroup
                            from p in pGroup.DefaultIfEmpty()
                            // Subqueries for CurrencyCode
                            let orderedCny = (from curr in context.T_Currency
                                             where qt != null && curr.CurrencyId == qt.CurrencyId
                                             select curr.CurrencyCode).FirstOrDefault()
                            let factoryCny = (from curr in context.T_Currency
                                             where qtsupp != null && curr.CurrencyId == qtsupp.CurrencyId
                                             select curr.CurrencyCode).FirstOrDefault()
                            select new
                            {
                                CustomerId = c != null ? (Guid?)c.CustomerId : null,
                                SupplierId = s != null ? (Guid?)s.SupplierId : null,
                                SuppName = s != null ? s.SupplierName : (string)null,
                                PCNumber = pc != null ? pc.PCNumber : (string)null,
                                CustName = c != null ? c.CustomerName : (string)null,
                                CustRef = qi != null ? qi.CustRef : (string)null,
                                ArticleCode = a != null ? a.ArticleCode : (string)null,
                                ArtName = a != null ? a.ArticleName : (string)null,
                                Packing = (p != null ? p.PackageName : "") + " " +
                                         (qtp != null ? qtp.InnerBox.ToString() : "0") + " " +
                                         (qtp != null ? qtp.Unit : "") + "/ " +
                                         (qtp != null ? qtp.OuterBox.ToString() : "0") + " " +
                                         (qtp != null ? qtp.Unit : "") + "/ " +
                                         (qtp != null ? qtp.CUFT.ToString() : "0") + " CUFT.",
                                ScheduledQty = qts.QtyOrdered,
                                OrderedCny = orderedCny ?? "",
                                OrderedPrice = qi != null ? (decimal?)qi.Amount : null,
                                OrderedUnit = qi != null ? qi.Unit : (string)null,
                                FactoryCny = factoryCny ?? "",
                                FactoryCost = qi != null ? (decimal?)qi.FactoryCost : null,
                                FactoryUnit = qtp != null ? qtp.Unit : (string)null,
                                ScheduledShipmentDate = qts.DateShipped
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.PCNumber);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.PCNumber);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the shipment history, replacing vwShipmentHistory
        /// </summary>
        public DataSet GetShipmentHistory(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View structure similar to purchase history but for customer shipments
                var query = from qtc in context.OrderQTCustShipping
                            join qi in context.OrderQTItems on qtc.OrderQTItemId equals qi.OrderQTItemId into qiGroup
                            from qi in qiGroup.DefaultIfEmpty()
                            join a in context.Article on (qi != null ? qi.ArticleId : Guid.Empty) equals a.ArticleId into aGroup
                            from a in aGroup.DefaultIfEmpty()
                            join qt in context.OrderQT on (qi != null ? qi.OrderQTId : Guid.Empty) equals qt.OrderQTId into qtGroup
                            from qt in qtGroup.DefaultIfEmpty()
                            join c in context.Customer on (qt != null ? qt.CustomerId : Guid.Empty) equals c.CustomerId into cGroup
                            from c in cGroup.DefaultIfEmpty()
                            join s in context.Supplier on (qi != null ? qi.SupplierId : Guid.Empty) equals s.SupplierId into sGroup
                            from s in sGroup.DefaultIfEmpty()
                            // Subquery for CurrencyCode
                            let currencyCode = (from curr in context.T_Currency
                                               where qt != null && curr.CurrencyId == qt.CurrencyId
                                               select curr.CurrencyCode).FirstOrDefault()
                            select new
                            {
                                CustomerId = c != null ? (Guid?)c.CustomerId : null,
                                SupplierId = s != null ? (Guid?)s.SupplierId : null,
                                CustName = c != null ? c.CustomerName : (string)null,
                                CustRef = qi != null ? qi.CustRef : (string)null,
                                ArticleCode = a != null ? a.ArticleCode : (string)null,
                                ArtName = a != null ? a.ArticleName : (string)null,
                                CurrencyCode = currencyCode ?? "",
                                Amount = qi != null ? (decimal?)qi.Amount : null,
                                Qty = qi != null ? (decimal?)qi.Qty : null,
                                Unit = qi != null ? qi.Unit : (string)null,
                                ShipmentDate = qtc.ShippedOn,
                                QtyOrdered = qtc.QtyOrdered,
                                QtyShipped = qtc.QtyShipped,
                                OSQty = qtc.QtyOrdered - qtc.QtyShipped,
                                OSAmount = (qtc.QtyOrdered - qtc.QtyShipped) * (qi != null ? qi.Amount : 0m)
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.ShipmentDate);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.ShipmentDate);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        #endregion

        #region Master Data Views

        /// <summary>
        /// Retrieves the user list, replacing vwUserList
        /// Note: View uses UNION of 4 queries
        /// </summary>
        public DataSet GetUserList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // Query 1: Staff with CreatedBy = empty GUID
                var query1 = from u in context.UserProfile
                            join s in context.Staff on u.UserSid equals s.StaffId
                            join s2 in context.Staff on s.ModifiedBy equals s2.StaffId into s2Group
                            from s2 in s2Group.DefaultIfEmpty()
                            where s.CreatedBy == Guid.Empty
                            select new
                            {
                                u.UserSid,
                                u.UserType,
                                u.Alias,
                                u.LoginName,
                                u.LoginPassword,
                                FullName = s.FullName,
                                CreatedOn = s.CreatedOn,
                                CreatedBy = (string)null,
                                ModifiedOn = s.ModifiedOn,
                                ModifiedBy = s2 != null ? s2.Alias : (string)null,
                                Status = s.Status
                            };

                // Query 2: Staff with CreatedBy != empty GUID
                var query2 = from u in context.UserProfile
                            join s in context.Staff on u.UserSid equals s.StaffId
                            join s1 in context.Staff on s.CreatedBy equals s1.StaffId
                            join s2 in context.Staff on s.ModifiedBy equals s2.StaffId
                            where s.CreatedBy != Guid.Empty
                            select new
                            {
                                u.UserSid,
                                u.UserType,
                                u.Alias,
                                u.LoginName,
                                u.LoginPassword,
                                FullName = s.FullName,
                                CreatedOn = s.CreatedOn,
                                CreatedBy = s1.Alias,
                                ModifiedOn = s.ModifiedOn,
                                ModifiedBy = s2.Alias,
                                Status = s.Status
                            };

                // Query 3: SupplierContact
                var query3 = from u in context.UserProfile
                            join sc in context.SupplierContact on u.UserSid equals sc.SupplierContactId
                            join s in context.Supplier on sc.SupplierId equals s.SupplierId
                            join s1 in context.Staff on sc.CreatedBy equals s1.StaffId
                            join s2 in context.Staff on sc.ModifiedBy equals s2.StaffId
                            select new
                            {
                                u.UserSid,
                                u.UserType,
                                u.Alias,
                                u.LoginName,
                                u.LoginPassword,
                                FullName = sc.FullName,
                                CreatedOn = sc.CreatedOn,
                                CreatedBy = s1.Alias,
                                ModifiedOn = sc.ModifiedOn,
                                ModifiedBy = s2.Alias,
                                Status = s.Status
                            };

                // Query 4: CustomerContact
                var query4 = from u in context.UserProfile
                            join cc in context.CustomerContact on u.UserSid equals cc.CustomerContactId
                            join c in context.Customer on cc.CustomerId equals c.CustomerId
                            join s1 in context.Staff on cc.CreatedBy equals s1.StaffId
                            join s2 in context.Staff on cc.ModifiedBy equals s2.StaffId
                            select new
                            {
                                u.UserSid,
                                u.UserType,
                                u.Alias,
                                u.LoginName,
                                u.LoginPassword,
                                FullName = cc.FullName,
                                CreatedOn = cc.CreatedOn,
                                CreatedBy = s1.Alias,
                                ModifiedOn = cc.ModifiedOn,
                                ModifiedBy = s2.Alias,
                                Status = c.Status
                            };

                // Union all queries
                var unionQuery = query1.Union(query2).Union(query3).Union(query4);

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(unionQuery, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.Alias);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.Alias);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the city list, replacing vwCityList
        /// </summary>
        public DataSet GetCityList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                var query = from ci in context.T_City
                            join co in context.T_Country on ci.CountryId equals co.CountryId // INNER JOIN
                            join pr in context.T_Province on ci.ProvinceId equals pr.ProvinceId into prGroup
                            from pr in prGroup.DefaultIfEmpty() // LEFT JOIN
                            select new
                            {
                                co.CountryId,
                                co.CountryCode,
                                co.CountryName,
                                co.CountryName_Chs,
                                co.CountryName_Cht,
                                co.CountryPhoneCode,
                                ProvinceId = pr != null ? (Guid?)pr.ProvinceId : null,
                                ProvinceCode = pr != null ? pr.ProvinceCode : (string)null,
                                ProvinceName = pr != null ? pr.ProvinceName : (string)null,
                                ProvinceName_Chs = pr != null ? pr.ProvinceName_Chs : (string)null,
                                ProvinceName_Cht = pr != null ? pr.ProvinceName_Cht : (string)null,
                                ci.CityId,
                                ci.CityCode,
                                ci.CityPhoneCode,
                                ci.CityName,
                                ci.CityName_Chs,
                                ci.CityName_Cht
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort - View orders by CountryName, ProvinceName, CityName
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.CountryName).ThenBy(x => x.ProvinceName).ThenBy(x => x.CityName);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.CountryName).ThenBy(x => x.ProvinceName).ThenBy(x => x.CityName);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the customer list, replacing vwCustomerList
        /// </summary>
        public DataSet GetCustomerList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View uses INNER JOINs for all joins
                var query = from c in context.Customer
                            join r in context.T_Region on c.RegionId equals r.RegionId // INNER JOIN
                            join t in context.T_PaymentTerms on c.TermsId equals t.TermsId // INNER JOIN
                            join cny in context.T_Currency on c.CurrencyId equals cny.CurrencyId // INNER JOIN
                            join s1 in context.Staff on c.CreatedBy equals s1.StaffId // INNER JOIN
                            join s2 in context.Staff on c.ModifiedBy equals s2.StaffId // INNER JOIN
                            select new
                            {
                                c.CustomerId,
                                c.CustomerCode,
                                c.ACNumber,
                                c.CustomerName,
                                c.CustomerName_Chs,
                                c.CustomerName_Cht,
                                c.RegionId,
                                RegionName = r.RegionName,
                                c.TermsId,
                                TermsName = t.TermsName,
                                c.CurrencyId,
                                CurrencyName = cny.CurrencyName,
                                c.CreditLimit,
                                c.ProfitMargin,
                                c.BlackListed,
                                c.Remarks,
                                c.Status,
                                c.CreatedOn,
                                CreatedBy = s1.Alias,
                                c.ModifiedOn,
                                ModifiedBy = s2.Alias,
                                c.Retired
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort - View orders by CustomerName
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.CustomerName);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.CustomerName);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the customer address list, replacing vwCustomerAddressList
        /// </summary>
        public DataSet GetCustomerAddressList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View uses INNER JOINs for all joins (including 5 phone labels)
                var query = from ca in context.CustomerAddress
                            join a in context.Z_Address on ca.AddressId equals a.AddressId // INNER JOIN
                            join p1 in context.Z_Phone on ca.Phone1_Label equals p1.PhoneId // INNER JOIN
                            join p2 in context.Z_Phone on ca.Phone2_Label equals p2.PhoneId // INNER JOIN
                            join p3 in context.Z_Phone on ca.Phone3_Label equals p3.PhoneId // INNER JOIN
                            join p4 in context.Z_Phone on ca.Phone4_Label equals p4.PhoneId // INNER JOIN
                            join p5 in context.Z_Phone on ca.Phone5_Label equals p5.PhoneId // INNER JOIN
                            join s1 in context.Staff on ca.CreatedBy equals s1.StaffId // INNER JOIN
                            join s2 in context.Staff on ca.ModifiedBy equals s2.StaffId // INNER JOIN
                            select new
                            {
                                ca.CustomerAddressId,
                                ca.CustomerId,
                                AddressName = a.AddressName,
                                ca.DefaultRec,
                                ca.AddrText,
                                ca.AddrIsMailing,
                                PhoneLabel1 = p1.PhoneName,
                                ca.Phone1_Text,
                                PhoneLabel2 = p2.PhoneName,
                                ca.Phone2_Text,
                                PhoneLabel3 = p3.PhoneName,
                                ca.Phone3_Text,
                                PhoneLabel4 = p4.PhoneName,
                                ca.Phone4_Text,
                                PhoneLabel5 = p5.PhoneName,
                                ca.Phone5_Text,
                                ca.Notes,
                                ca.CreatedOn,
                                CreatedBy = s1.Alias,
                                ca.ModifiedOn,
                                ModifiedBy = s2.Alias,
                                ca.Retired
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.CustomerAddressId);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.CustomerAddressId);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the customer contact list, replacing vwCustomerContactList
        /// </summary>
        public DataSet GetCustomerContactList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View uses INNER JOINs for all joins
                var query = from cc in context.CustomerContact
                            join zs in context.Z_Salutation on cc.SalutationId equals zs.SalutationId // INNER JOIN
                            join zj in context.Z_JobTitle on cc.JobTitleId equals zj.JobTitleId // INNER JOIN
                            join p1 in context.Z_Phone on cc.Phone1_Label equals p1.PhoneId // INNER JOIN
                            join p2 in context.Z_Phone on cc.Phone2_Label equals p2.PhoneId // INNER JOIN
                            join p3 in context.Z_Phone on cc.Phone3_Label equals p3.PhoneId // INNER JOIN
                            join p4 in context.Z_Phone on cc.Phone4_Label equals p4.PhoneId // INNER JOIN
                            join p5 in context.Z_Phone on cc.Phone5_Label equals p5.PhoneId // INNER JOIN
                            join s1 in context.Staff on cc.CreatedBy equals s1.StaffId // INNER JOIN
                            join s2 in context.Staff on cc.ModifiedBy equals s2.StaffId // INNER JOIN
                            select new
                            {
                                cc.CustomerContactId,
                                cc.CustomerId,
                                cc.DefaultRec,
                                SalutationName = zs.SalutationName,
                                cc.FullName,
                                cc.FirstName,
                                cc.LastName,
                                JobTitleName = zj.JobTitleName,
                                PhoneLabel1 = p1.PhoneName,
                                cc.Phone1_Text,
                                PhoneLabel2 = p2.PhoneName,
                                cc.Phone2_Text,
                                PhoneLabel3 = p3.PhoneName,
                                cc.Phone3_Text,
                                PhoneLabel4 = p4.PhoneName,
                                cc.Phone4_Text,
                                PhoneLabel5 = p5.PhoneName,
                                cc.Phone5_Text,
                                cc.Notes,
                                cc.CreatedOn,
                                CreatedBy = s1.Alias,
                                cc.ModifiedOn,
                                ModifiedBy = s2.Alias,
                                cc.Retired
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.CustomerContactId);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.CustomerContactId);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the supplier list, replacing vwSupplierList
        /// </summary>
        public DataSet GetSupplierList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View uses INNER JOINs for all joins
                var query = from s in context.Supplier
                            join r in context.T_Region on s.RegionId equals r.RegionId // INNER JOIN
                            join p in context.T_PaymentTerms on s.TermsId equals p.TermsId // INNER JOIN
                            join s1 in context.Staff on s.CreatedBy equals s1.StaffId // INNER JOIN
                            join s2 in context.Staff on s.ModifiedBy equals s2.StaffId // INNER JOIN
                            select new
                            {
                                s.SupplierId,
                                s.SupplierCode,
                                s.SupplierName,
                                s.SupplierName_Chs,
                                s.SupplierName_Cht,
                                s.ACNumber,
                                RegionName = r.RegionName,
                                TermsName = p.TermsName,
                                s.Remarks,
                                s.Status,
                                s.CreatedOn,
                                CreatedBy = s1.Alias,
                                s.ModifiedOn,
                                ModifiedBy = s2.Alias,
                                s.Retired
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.SupplierName);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.SupplierName);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the supplier address list, replacing vwSupplierAddressList
        /// </summary>
        public DataSet GetSupplierAddressList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // Similar structure to CustomerAddressList
                var query = from sa in context.SupplierAddress
                            join a in context.Z_Address on sa.AddressId equals a.AddressId // INNER JOIN
                            join p1 in context.Z_Phone on sa.Phone1_Label equals p1.PhoneId // INNER JOIN
                            join p2 in context.Z_Phone on sa.Phone2_Label equals p2.PhoneId // INNER JOIN
                            join p3 in context.Z_Phone on sa.Phone3_Label equals p3.PhoneId // INNER JOIN
                            join p4 in context.Z_Phone on sa.Phone4_Label equals p4.PhoneId // INNER JOIN
                            join p5 in context.Z_Phone on sa.Phone5_Label equals p5.PhoneId // INNER JOIN
                            join s1 in context.Staff on sa.CreatedBy equals s1.StaffId // INNER JOIN
                            join s2 in context.Staff on sa.ModifiedBy equals s2.StaffId // INNER JOIN
                            select new
                            {
                                sa.SupplierAddressId,
                                sa.SupplierId,
                                sa.DefaultRec,
                                AddressName = a.AddressName,
                                sa.AddrText,
                                sa.AddrIsMailing,
                                PhoneLabel1 = p1.PhoneName,
                                sa.Phone1_Text,
                                PhoneLabel2 = p2.PhoneName,
                                sa.Phone2_Text,
                                PhoneLabel3 = p3.PhoneName,
                                sa.Phone3_Text,
                                PhoneLabel4 = p4.PhoneName,
                                sa.Phone4_Text,
                                PhoneLabel5 = p5.PhoneName,
                                sa.Phone5_Text,
                                sa.Notes,
                                sa.CreatedOn,
                                CreatedBy = s1.Alias,
                                sa.ModifiedOn,
                                ModifiedBy = s2.Alias,
                                sa.Retired
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.SupplierAddressId);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.SupplierAddressId);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the supplier contact list, replacing vwSupplierContactList
        /// </summary>
        public DataSet GetSupplierContactList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // Similar structure to CustomerContactList
                var query = from sc in context.SupplierContact
                            join zs in context.Z_Salutation on sc.SalutationId equals zs.SalutationId // INNER JOIN
                            join zj in context.Z_JobTitle on sc.JobTitleId equals zj.JobTitleId // INNER JOIN
                            join p1 in context.Z_Phone on sc.Phone1_Label equals p1.PhoneId // INNER JOIN
                            join p2 in context.Z_Phone on sc.Phone2_Label equals p2.PhoneId // INNER JOIN
                            join p3 in context.Z_Phone on sc.Phone3_Label equals p3.PhoneId // INNER JOIN
                            join p4 in context.Z_Phone on sc.Phone4_Label equals p4.PhoneId // INNER JOIN
                            join p5 in context.Z_Phone on sc.Phone5_Label equals p5.PhoneId // INNER JOIN
                            join s1 in context.Staff on sc.CreatedBy equals s1.StaffId // INNER JOIN
                            join s2 in context.Staff on sc.ModifiedBy equals s2.StaffId // INNER JOIN
                            select new
                            {
                                sc.SupplierContactId,
                                sc.SupplierId,
                                sc.DefaultRec,
                                SalutationName = zs.SalutationName,
                                sc.FullName,
                                sc.FirstName,
                                sc.LastName,
                                JobTitleName = zj.JobTitleName,
                                PhoneLabel1 = p1.PhoneName,
                                sc.Phone1_Text,
                                PhoneLabel2 = p2.PhoneName,
                                sc.Phone2_Text,
                                PhoneLabel3 = p3.PhoneName,
                                sc.Phone3_Text,
                                PhoneLabel4 = p4.PhoneName,
                                sc.Phone4_Text,
                                PhoneLabel5 = p5.PhoneName,
                                sc.Phone5_Text,
                                sc.Notes,
                                sc.CreatedOn,
                                CreatedBy = s1.Alias,
                                sc.ModifiedOn,
                                ModifiedBy = s2.Alias,
                                sc.Retired
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.SupplierContactId);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.SupplierContactId);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the staff list, replacing vwStaffList
        /// </summary>
        public DataSet GetStaffList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View uses INNER JOINs for Division and Group, LEFT JOINs for Staff (CreatedBy/ModifiedBy)
                var query = from u in context.Staff
                            join d in context.T_Division on u.DivisionId equals d.DivisionId // INNER JOIN
                            join g in context.T_Group on u.GroupId equals g.GroupId // INNER JOIN
                            join s1 in context.Staff on u.CreatedBy equals s1.StaffId into s1Group
                            from s1 in s1Group.DefaultIfEmpty() // LEFT JOIN
                            join s2 in context.Staff on u.ModifiedBy equals s2.StaffId into s2Group
                            from s2 in s2Group.DefaultIfEmpty() // LEFT JOIN
                            select new
                            {
                                DivisionId = d.DivisionId,
                                DivisionCode = d.DivisionCode,
                                DivisionName = d.DivisionName,
                                DivisionName_Chs = d.DivisionName_Chs,
                                DivisionName_Cht = d.DivisionName_Cht,
                                GroupId = g.GroupId,
                                GroupCode = g.GroupCode,
                                GroupName = g.GroupName,
                                GroupName_Chs = g.GroupName_Chs,
                                GroupName_Cht = g.GroupName_Cht,
                                StaffId = u.StaffId,
                                StaffCode = u.StaffCode,
                                FullName = u.FullName,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                Alias = u.Alias,
                                Login = u.Login,
                                Password = u.Password,
                                Remarks = u.Remarks,
                                Status = u.Status,
                                CreatedOn = u.CreatedOn,
                                CreatedBy = s1 != null ? s1.Alias : (string)null,
                                ModifiedOn = u.ModifiedOn,
                                ModifiedBy = s2 != null ? s2.Alias : (string)null
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.Alias);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.Alias);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Retrieves the staff address list, replacing vwStaffAddressList
        /// </summary>
        public DataSet GetStaffAddressList(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View uses INNER JOINs for phones, LEFT JOINs for Address and Staff
                var query = from sa in context.StaffAddress
                            join p1 in context.Z_Phone on sa.Phone1_Label equals p1.PhoneId // INNER JOIN
                            join p2 in context.Z_Phone on sa.Phone2_Label equals p2.PhoneId // INNER JOIN
                            join p3 in context.Z_Phone on sa.Phone3_Label equals p3.PhoneId // INNER JOIN
                            join p4 in context.Z_Phone on sa.Phone4_Label equals p4.PhoneId // INNER JOIN
                            join p5 in context.Z_Phone on sa.Phone5_Label equals p5.PhoneId // INNER JOIN
                            join za in context.Z_Address on sa.AddressId equals za.AddressId into zaGroup
                            from za in zaGroup.DefaultIfEmpty() // LEFT JOIN
                            join s2 in context.Staff on sa.ModifiedBy equals s2.StaffId into s2Group
                            from s2 in s2Group.DefaultIfEmpty() // LEFT JOIN
                            join s1 in context.Staff on sa.CreatedBy equals s1.StaffId into s1Group
                            from s1 in s1Group.DefaultIfEmpty() // LEFT JOIN
                            select new
                            {
                                StaffId = sa.StaffId,
                                StaffAddressId = sa.StaffAddressId,
                                sa.DefaultRec,
                                AddressName = za != null ? za.AddressName : (string)null,
                                sa.AddrText,
                                sa.AddrIsMailing,
                                PhoneLable1 = p1.PhoneName, // Note: View has typo "PhoneLable1"
                                sa.Phone1_Text,
                                PhoneLabel2 = p2.PhoneName,
                                sa.Phone2_Text,
                                PhoneLabel3 = p3.PhoneName,
                                sa.Phone3_Text,
                                PhoneLabel4 = p4.PhoneName,
                                sa.Phone4_Text,
                                PhoneLabel5 = p5.PhoneName,
                                sa.Phone5_Text,
                                Notes = sa.Notes ?? "",
                                sa.CreatedOn,
                                CreatedBy = s1 != null ? s1.Alias : (string)null,
                                sa.ModifiedOn,
                                ModifiedBy = s2 != null ? s2.Alias : (string)null,
                                sa.Retired
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.StaffAddressId);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.StaffAddressId);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        #endregion

        #region Reporting Views

        // Note: Reporting Views (vwRpt*) are typically complex and may require specific implementations
        // For now, we'll add placeholder methods that can be implemented based on actual usage patterns
        // These can be expanded as needed when updating the reporting code

        /// <summary>
        /// Retrieves report data for sales contract list, replacing vwRptSalesContractList
        /// </summary>
        public DataSet GetRptSalesContractList(string whereClause = "", string orderBy = "")
        {
            // Implementation similar to GetSalesContractList but may have additional fields for reporting
            return GetSalesContractList(whereClause, orderBy);
        }

        /// <summary>
        /// Retrieves report data for purchase contract list, replacing vwRptPurchaseContractList
        /// </summary>
        public DataSet GetRptPurchaseContractList(string whereClause = "", string orderBy = "")
        {
            // Implementation similar to GetPurchaseContractList but may have additional fields for reporting
            return GetPurchaseContractList(whereClause, orderBy);
        }

        /// <summary>
        /// Retrieves report data for invoice list, replacing vwRptInvoiceList
        /// </summary>
        public DataSet GetRptInvoiceList(string whereClause = "", string orderBy = "")
        {
            // Implementation similar to GetInvoiceList but may have additional fields for reporting
            return GetInvoiceList(whereClause, orderBy);
        }

        /// <summary>
        /// Retrieves report data for proforma invoice list, replacing vwRptProformaInvoiceList
        /// </summary>
        public DataSet GetRptProformaInvoiceList(string whereClause = "", string orderBy = "")
        {
            // Similar to invoice list - may need to check actual View definition
            return GetInvoiceList(whereClause, orderBy);
        }

        /// <summary>
        /// Retrieves report data for price list, replacing vwRptPriceList
        /// </summary>
        public DataSet GetRptPriceList(string whereClause = "", string orderBy = "")
        {
            // May need to check actual View definition
            return GetQuotationItemList(whereClause, orderBy);
        }

        /// <summary>
        /// Retrieves report data for pre-order list, replacing vwRptPreOrderList
        /// </summary>
        public DataSet GetRptPreOrderList(string whereClause = "", string orderBy = "")
        {
            return GetPreOrderList(whereClause, orderBy);
        }

        /// <summary>
        /// Retrieves report data for shipment advise list, replacing vwRptShipmentAdviseList
        /// </summary>
        public DataSet GetRptShipmentAdviseList(string whereClause = "", string orderBy = "")
        {
            // May need to check actual View definition
            return GetShipmentHistory(whereClause, orderBy);
        }

        /// <summary>
        /// Retrieves report data for purchase contract shipment list, replacing vwRptPurchaseContractShipmentList
        /// </summary>
        public DataSet GetRptPurchaseContractShipmentList(string whereClause = "", string orderBy = "")
        {
            // May need to check actual View definition
            return GetPurchaseHistory(whereClause, orderBy);
        }

        /// <summary>
        /// Retrieves report data for sales contract shipment list, replacing vwRptSalesContractShipmentList
        /// </summary>
        public DataSet GetRptSalesContractShipmentList(string whereClause = "", string orderBy = "")
        {
            // May need to check actual View definition
            return GetShipmentHistory(whereClause, orderBy);
        }

        /// <summary>
        /// Retrieves report data for pre-order list customer shipment, replacing vwRptPreOrderList_CustShipment
        /// </summary>
        public DataSet GetRptPreOrderListCustShipment(string whereClause = "", string orderBy = "")
        {
            // May need to check actual View definition
            return GetShipmentHistory(whereClause, orderBy);
        }

        /// <summary>
        /// Retrieves report data for pre-order list supplier shipment, replacing vwRptPreOrderList_SuppShipment
        /// </summary>
        public DataSet GetRptPreOrderListSuppShipment(string whereClause = "", string orderBy = "")
        {
            // May need to check actual View definition
            return GetPurchaseHistory(whereClause, orderBy);
        }

        /// <summary>
        /// Retrieves invoice charges, replacing vwRptInvoice_Charges
        /// </summary>
        public DataSet GetRptInvoiceCharges(string whereClause = "", string orderBy = "")
        {
            using (var context = GetDbContext())
            {
                // View uses INNER JOIN
                var query = from oin in context.OrderIN
                            join oic in context.OrderINCharges on oin.OrderINId equals oic.OrderINId // INNER JOIN
                            select new
                            {
                                oin.OrderINId,
                                oin.INNumber,
                                oic.OrderINChargeId,
                                oic.ChargeId,
                                oic.Description,
                                oic.Amount
                            };

                // Apply dynamic filters
                var filteredQuery = ApplyDynamicWhere(query, whereClause);

                // Apply dynamic sort
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    try
                    {
                        filteredQuery = filteredQuery.OrderBy(orderBy);
                    }
                    catch
                    {
                        filteredQuery = filteredQuery.OrderBy(x => x.INNumber);
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderBy(x => x.INNumber);
                }

                return filteredQuery.ToDataSet("Table");
            }
        }

        #endregion

        #region Print Views

        /// <summary>
        /// Retrieves print data for price list, replacing vwPrint_PriceList
        /// </summary>
        public DataSet GetPrintPriceList(string whereClause = "", string orderBy = "")
        {
            // May need to check actual View definition - likely similar to quotation item list
            return GetQuotationItemList(whereClause, orderBy);
        }

        /// <summary>
        /// Retrieves print data for completed price list, replacing vwPrint_CompletedPriceList
        /// </summary>
        public DataSet GetPrintCompletedPriceList(string whereClause = "", string orderBy = "")
        {
            // May need to check actual View definition
            return GetQuotationItemList(whereClause, orderBy);
        }

        /// <summary>
        /// Retrieves price detail list, replacing vwPriceDetailList
        /// </summary>
        public DataSet GetPriceDetailList(string whereClause = "", string orderBy = "")
        {
            // May need to check actual View definition
            return GetQuotationItemList(whereClause, orderBy);
        }

        /// <summary>
        /// Retrieves print data for quotation list, replacing vwPrint_QuotationList
        /// </summary>
        public DataSet GetPrintQuotationList(string whereClause = "", string orderBy = "")
        {
            return GetQuotationList(whereClause, orderBy);
        }

        #endregion

        #region Bot Views

        // Note: vwClientList and vwClientUserList were identified in xPort5.Bot code
        // but are not present in the main database script. These may be in a different database
        // or schema. Placeholder methods are provided for when they are identified.

        /// <summary>
        /// Retrieves client list, replacing vwClientList (if applicable)
        /// Note: This View was not found in the main database script - may be in a different schema
        /// </summary>
        public DataSet GetClientList(string whereClause = "", string orderBy = "")
        {
            // TODO: Implement when View definition is identified
            // This View is used in xPort5.Bot but not found in xPort5_MSSQL_script.sql
            throw new NotImplementedException("vwClientList View definition not found in database script. Please provide the View definition.");
        }

        /// <summary>
        /// Retrieves client user list, replacing vwClientUserList (if applicable)
        /// Note: This View was not found in the main database script - may be in a different schema
        /// </summary>
        public DataSet GetClientUserList(string whereClause = "", string orderBy = "")
        {
            // TODO: Implement when View definition is identified
            // This View is used in xPort5.Bot but not found in xPort5_MSSQL_script.sql
            throw new NotImplementedException("vwClientUserList View definition not found in database script. Please provide the View definition.");
        }

        #endregion

        #region Dynamic WHERE Clause Helper

        /// <summary>
        /// Applies a dynamic WHERE clause based on legacy SQL syntax
        /// </summary>
        public IQueryable<T> ApplyDynamicWhere<T>(IQueryable<T> query, string sqlWhereClause)
        {
            if (string.IsNullOrWhiteSpace(sqlWhereClause))
            {
                return query;
            }

            // Remove "WHERE" keyword if present at the start
            var whereClause = sqlWhereClause;
            if (whereClause.TrimStart().StartsWith("WHERE ", StringComparison.OrdinalIgnoreCase))
            {
                whereClause = whereClause.Substring(whereClause.IndexOf("WHERE ", StringComparison.OrdinalIgnoreCase) + 6);
            }

            // Use SqlToLinqConverter to transform SQL syntax to Dynamic LINQ syntax
            var linqExpression = SqlToLinqConverter.ConvertWhereClause(whereClause);
            
            try 
            {
                return query.Where(linqExpression);
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                System.Diagnostics.Debug.WriteLine($"ViewService.ApplyDynamicWhere - Failed to apply dynamic filter");
                System.Diagnostics.Debug.WriteLine($"  Converted LINQ Expression: {linqExpression}");
                System.Diagnostics.Debug.WriteLine($"  Original SQL WHERE clause: {sqlWhereClause}");
                System.Diagnostics.Debug.WriteLine($"  Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"  Inner Exception: {ex.InnerException.Message}");
                }
                System.Diagnostics.Debug.WriteLine($"  Stack Trace: {ex.StackTrace}");
                
                // Re-throw with more context
                throw new InvalidOperationException(
                    string.Format("Failed to apply dynamic filter: {0} (Original: {1})", linqExpression, sqlWhereClause), 
                    ex);
            }
        }

        #endregion
    }
}
