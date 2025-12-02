// 2025-01-XX Composer: OLAP Service for converting stored procedures to LINQ queries
// Provides infrastructure for OLAP reporting functionality

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace xPort5.EF6
{
    /// <summary>
    /// Service class for OLAP reporting queries using LINQ instead of stored procedures
    /// </summary>
    public class OlapService
    {
        /// <summary>
        /// Singleton instance for convenient access
        /// </summary>
        public static OlapService Default { get; } = new OlapService();

        /// <summary>
        /// Private constructor to enforce singleton pattern
        /// </summary>
        private OlapService()
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
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="query">The queryable to filter</param>
        /// <param name="datePropertyName">Name of the date property (e.g., "INDate", "ShipmentDate")</param>
        /// <param name="fromDate">Start date (inclusive), null to ignore</param>
        /// <param name="toDate">End date (inclusive), null to ignore</param>
        /// <returns>Filtered queryable</returns>
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
        /// <param name="dateString">Date string to parse</param>
        /// <returns>Parsed DateTime or null if empty/invalid</returns>
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

        #region Customer ID Array Parsing

        /// <summary>
        /// Parses a comma-separated string of GUIDs (format: 'guid1','guid2','guid3')
        /// </summary>
        /// <param name="customerIdArray">Comma-separated GUID string</param>
        /// <returns>List of parsed GUIDs, empty list if input is null/empty</returns>
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
        /// Uses Contains method which translates to SQL IN clause
        /// </summary>
        /// <typeparam name="T">Entity type with CustomerId property</typeparam>
        /// <param name="query">The queryable to filter</param>
        /// <param name="customerIdArray">Comma-separated GUID string</param>
        /// <returns>Filtered queryable, or original query if customerIdArray is empty</returns>
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
            // Build expression: x => customerIds.Contains(x.CustomerId)
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

        #region Date Grouping Helpers

        /// <summary>
        /// Gets the year from a nullable DateTime
        /// </summary>
        public int? GetYear(DateTime? date)
        {
            return date?.Year;
        }

        /// <summary>
        /// Gets the month (1-12) from a nullable DateTime
        /// </summary>
        public int? GetMonth(DateTime? date)
        {
            return date?.Month;
        }

        /// <summary>
        /// Gets the quarter (1-4) from a nullable DateTime
        /// </summary>
        public int? GetQuarter(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return (date.Value.Month - 1) / 3 + 1;
        }

        /// <summary>
        /// Groups by year and month, returning a key like "2024-01"
        /// </summary>
        public string GetYearMonthKey(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.ToString("yyyy-MM");
        }

        /// <summary>
        /// Groups by year and quarter, returning a key like "2024-Q1"
        /// </summary>
        public string GetYearQuarterKey(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            var quarter = GetQuarter(date);
            return $"{date.Value.Year}-Q{quarter}";
        }

        #endregion

        #region Currency Conversion

        /// <summary>
        /// Converts an amount from one currency to another using exchange rates
        /// Note: This is a placeholder - actual implementation will depend on your currency conversion logic
        /// </summary>
        /// <param name="amount">Amount to convert</param>
        /// <param name="fromCurrencyId">Source currency GUID</param>
        /// <param name="toCurrencyId">Target currency GUID</param>
        /// <param name="exchangeRate">Exchange rate (if known)</param>
        /// <returns>Converted amount</returns>
        public decimal ConvertCurrency(decimal amount, Guid? fromCurrencyId, Guid? toCurrencyId, decimal? exchangeRate = null)
        {
            // If same currency or no conversion needed, return original
            if (fromCurrencyId == toCurrencyId || !fromCurrencyId.HasValue || !toCurrencyId.HasValue)
            {
                return amount;
            }

            // If exchange rate provided, use it
            if (exchangeRate.HasValue && exchangeRate.Value != 0)
            {
                return amount * exchangeRate.Value;
            }

            // TODO: Implement actual currency conversion logic from T_Currency table
            // For now, return original amount
            return amount;
        }

        /// <summary>
        /// Gets exchange rate between two currencies from the database
        /// </summary>
        /// <param name="fromCurrencyId">Source currency GUID</param>
        /// <param name="toCurrencyId">Target currency GUID</param>
        /// <param name="context">DbContext instance</param>
        /// <returns>Exchange rate, or 1.0 if currencies are the same or not found</returns>
        public decimal GetExchangeRate(Guid? fromCurrencyId, Guid? toCurrencyId, xPort5Entities context)
        {
            if (fromCurrencyId == toCurrencyId || !fromCurrencyId.HasValue || !toCurrencyId.HasValue)
            {
                return 1.0m;
            }

            // TODO: Implement actual exchange rate lookup from T_Currency table
            // This will depend on your currency table structure
            // For now, return 1.0 as placeholder
            return 1.0m;
        }

        #endregion

        #region OLAP Report Methods

        /// <summary>
        /// Gets invoice summary data matching olap_InvoiceSummary stored procedure
        /// Returns invoice items and charges grouped by month (last 12 months + backlog)
        /// </summary>
        /// <param name="customerIdArray">Comma-separated GUID string of customer IDs (format: 'guid1','guid2')</param>
        /// <returns>DataSet with columns: Region, CustName, INDate, INNumber, ExchangeRate, INQty, UnitAmount, ExtAmount, ExtHKDAmount, BackLogAmt, Amt1-12, Total</returns>
        public DataSet GetInvoiceSummary(string customerIdArray)
        {
            using (var context = GetDbContext())
            {
                var customerIds = ParseCustomerIdArray(customerIdArray);
                if (customerIds.Count == 0)
                {
                    return new DataSet();
                }

                var now = DateTime.Now;
                var currentMonth = new DateTime(now.Year, now.Month, 1);

                // Calculate month boundaries for the 12 months (current month - 11 through current month)
                var monthBoundaries = new List<DateTime>();
                for (int i = 11; i >= 0; i--)
                {
                    monthBoundaries.Add(currentMonth.AddMonths(-i));
                }

                // Query 1: Invoice Items (OrderINItems -> OrderIN -> OrderSCItems -> OrderQTItems -> OrderQT)
                var itemsQuery = from item in context.OrderINItems
                                 join invoice in context.OrderIN on item.OrderINId equals invoice.OrderINId
                                 join scItem in context.OrderSCItems on item.OrderSCItemsId equals scItem.OrderSCItemsId
                                 join qtItem in context.OrderQTItems on scItem.OrderQTItemId equals qtItem.OrderQTItemId
                                 join qt in context.OrderQT on qtItem.OrderQTId equals qt.OrderQTId
                                 join customer in context.Customer on invoice.CustomerId equals customer.CustomerId
                                 join region in context.T_Region on customer.RegionId equals region.RegionId into regionGroup
                                 from region in regionGroup.DefaultIfEmpty()
                                 where customerIds.Contains(invoice.CustomerId)
                                 select new
                                 {
                                     Region = region != null ? region.RegionName : (string)null,
                                     CustName = customer.CustomerName,
                                     INDate = invoice.INDate,
                                     INNumber = invoice.INNumber,
                                     ExchangeRate = qt.ExchangeRate,
                                     INQty = item.Qty,
                                     UnitAmount = qtItem.Amount,
                                     ExtAmount = item.Qty * qtItem.Amount,
                                     ExtHKDAmount = item.Qty * qtItem.Amount * qt.ExchangeRate,
                                     HKDAmount = item.Qty * qtItem.Amount * qt.ExchangeRate
                                 };

                // Query 2: Invoice Charges (OrderINCharges -> OrderIN -> Customer -> T_Currency)
                var chargesQuery = from charge in context.OrderINCharges
                                   join invoice in context.OrderIN on charge.OrderINId equals invoice.OrderINId
                                   join customer in context.Customer on invoice.CustomerId equals customer.CustomerId
                                   join currency in context.T_Currency on customer.CurrencyId equals currency.CurrencyId into currencyGroup
                                   from currency in currencyGroup.DefaultIfEmpty()
                                   join region in context.T_Region on customer.RegionId equals region.RegionId into regionGroup
                                   from region in regionGroup.DefaultIfEmpty()
                                   where customerIds.Contains(invoice.CustomerId)
                                   select new
                                   {
                                       Region = region != null ? region.RegionName : (string)null,
                                       CustName = customer.CustomerName,
                                       INDate = invoice.INDate,
                                       INNumber = invoice.INNumber,
                                       ExchangeRate = currency != null ? currency.XchgRate : 0m,
                                       INQty = 1m,
                                       UnitAmount = charge.Amount,
                                       ExtAmount = charge.Amount,
                                       ExtHKDAmount = charge.Amount * (currency != null ? currency.XchgRate : 0m),
                                       HKDAmount = charge.Amount * (currency != null ? currency.XchgRate : 0m)
                                   };

                // Combine both queries (UNION ALL equivalent)
                var combinedQuery = itemsQuery.Concat(chargesQuery);

                // Execute query and calculate month-based amounts
                var results = combinedQuery.ToList();

                // Get year-month strings for comparison (matching SQL CONVERT(VARCHAR(7), date, 120))
                var currentYearMonth = now.ToString("yyyy-MM");
                var monthStrings = new List<string>();
                for (int i = 11; i >= 0; i--)
                {
                    var monthDate = currentMonth.AddMonths(-i);
                    monthStrings.Add(monthDate.ToString("yyyy-MM"));
                }

                var summaryResults = results.Select(r =>
                {
                    var invoiceDate = r.INDate.HasValue ? r.INDate.Value : DateTime.MinValue;
                    var invoiceYearMonth = invoiceDate.ToString("yyyy-MM");
                    var hkdAmount = r.HKDAmount;

                    // Calculate BackLogAmt (amounts from months before the 12-month window)
                    // SQL: CONVERT(VARCHAR(7),OrderIN.INDate,120) < CONVERT(VARCHAR(7),DATEADD(MONTH,-11,GETDATE()),120)
                    var backLogAmt = invoiceYearMonth.CompareTo(monthStrings[0]) < 0 ? hkdAmount : 0m;

                    // Calculate Amt1-12 (amounts for each of the 12 months)
                    // SQL: CONVERT(VARCHAR(7),OrderIN.INDate,120) = CONVERT(VARCHAR(7),DATEADD(MONTH,-11,GETDATE()),120)
                    var amt1 = invoiceYearMonth == monthStrings[0] ? hkdAmount : 0m;
                    var amt2 = invoiceYearMonth == monthStrings[1] ? hkdAmount : 0m;
                    var amt3 = invoiceYearMonth == monthStrings[2] ? hkdAmount : 0m;
                    var amt4 = invoiceYearMonth == monthStrings[3] ? hkdAmount : 0m;
                    var amt5 = invoiceYearMonth == monthStrings[4] ? hkdAmount : 0m;
                    var amt6 = invoiceYearMonth == monthStrings[5] ? hkdAmount : 0m;
                    var amt7 = invoiceYearMonth == monthStrings[6] ? hkdAmount : 0m;
                    var amt8 = invoiceYearMonth == monthStrings[7] ? hkdAmount : 0m;
                    var amt9 = invoiceYearMonth == monthStrings[8] ? hkdAmount : 0m;
                    var amt10 = invoiceYearMonth == monthStrings[9] ? hkdAmount : 0m;
                    var amt11 = invoiceYearMonth == monthStrings[10] ? hkdAmount : 0m;
                    var amt12 = invoiceYearMonth == monthStrings[11] ? hkdAmount : 0m;

                    var total = backLogAmt + amt1 + amt2 + amt3 + amt4 + amt5 + amt6 + amt7 + amt8 + amt9 + amt10 + amt11 + amt12;

                    return new
                    {
                        Region = r.Region ?? string.Empty,
                        CustName = r.CustName ?? string.Empty,
                        INDate = r.INDate,
                        INNumber = r.INNumber ?? string.Empty,
                        ExchangeRate = r.ExchangeRate,
                        INQty = r.INQty,
                        UnitAmount = r.UnitAmount,
                        ExtAmount = r.ExtAmount,
                        ExtHKDAmount = r.ExtHKDAmount,
                        BackLogAmt = backLogAmt,
                        Amt1 = amt1,
                        Amt2 = amt2,
                        Amt3 = amt3,
                        Amt4 = amt4,
                        Amt5 = amt5,
                        Amt6 = amt6,
                        Amt7 = amt7,
                        Amt8 = amt8,
                        Amt9 = amt9,
                        Amt10 = amt10,
                        Amt11 = amt11,
                        Amt12 = amt12,
                        Total = total
                    };
                })
                .OrderBy(r => r.Region)
                .ThenBy(r => r.CustName)
                .ThenBy(r => r.INDate)
                .ThenBy(r => r.INNumber)
                .ToList();

                // Convert to DataSet
                return summaryResults.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Gets monthly invoice summary data matching olap_MonthlyInvoiceSummary stored procedure
        /// Similar to GetInvoiceSummary but with date range filtering
        /// </summary>
        /// <param name="customerIdArray">Comma-separated GUID string of customer IDs</param>
        /// <param name="fromDate">Start date string (format: "yyyy-MM-dd HH:mm:ss")</param>
        /// <param name="toDate">End date string (format: "yyyy-MM-dd HH:mm:ss")</param>
        /// <returns>DataSet with invoice summary data</returns>
        public DataSet GetMonthlyInvoiceSummary(string customerIdArray, string fromDate, string toDate)
        {
            using (var context = GetDbContext())
            {
                var customerIds = ParseCustomerIdArray(customerIdArray);
                if (customerIds.Count == 0)
                {
                    return new DataSet();
                }

                var fromDateParsed = ParseDateParameter(fromDate);
                var toDateParsed = ParseDateParameter(toDate);
                
                if (!fromDateParsed.HasValue || !toDateParsed.HasValue)
                {
                    return new DataSet();
                }

                // Calculate month boundaries based on ToDate (not current date)
                var toDateValue = toDateParsed.Value;
                var toDateMonth = new DateTime(toDateValue.Year, toDateValue.Month, 1);
                
                var monthStrings = new List<string>();
                for (int i = 11; i >= 0; i--)
                {
                    var monthDate = toDateMonth.AddMonths(-i);
                    monthStrings.Add(monthDate.ToString("yyyy-MM"));
                }

                // Query 1: Invoice Items
                var itemsQuery = from item in context.OrderINItems
                                 join invoice in context.OrderIN on item.OrderINId equals invoice.OrderINId
                                 join scItem in context.OrderSCItems on item.OrderSCItemsId equals scItem.OrderSCItemsId
                                 join qtItem in context.OrderQTItems on scItem.OrderQTItemId equals qtItem.OrderQTItemId
                                 join qt in context.OrderQT on qtItem.OrderQTId equals qt.OrderQTId
                                 join customer in context.Customer on invoice.CustomerId equals customer.CustomerId
                                 join region in context.T_Region on customer.RegionId equals region.RegionId into regionGroup
                                 from region in regionGroup.DefaultIfEmpty()
                                 where customerIds.Contains(invoice.CustomerId)
                                    && invoice.INDate >= fromDateParsed.Value
                                    && invoice.INDate <= toDateParsed.Value
                                 select new
                                 {
                                     Region = region != null ? region.RegionName : (string)null,
                                     CustName = customer.CustomerName,
                                     INDate = invoice.INDate,
                                     INNumber = invoice.INNumber,
                                     ExchangeRate = qt.ExchangeRate,
                                     INQty = item.Qty,
                                     UnitAmount = qtItem.Amount,
                                     ExtAmount = item.Qty * qtItem.Amount,
                                     ExtHKDAmount = item.Qty * qtItem.Amount * qt.ExchangeRate,
                                     HKDAmount = item.Qty * qtItem.Amount * qt.ExchangeRate
                                 };

                // Query 2: Invoice Charges
                var chargesQuery = from charge in context.OrderINCharges
                                   join invoice in context.OrderIN on charge.OrderINId equals invoice.OrderINId
                                   join customer in context.Customer on invoice.CustomerId equals customer.CustomerId
                                   join currency in context.T_Currency on customer.CurrencyId equals currency.CurrencyId into currencyGroup
                                   from currency in currencyGroup.DefaultIfEmpty()
                                   join region in context.T_Region on customer.RegionId equals region.RegionId into regionGroup
                                   from region in regionGroup.DefaultIfEmpty()
                                   where customerIds.Contains(invoice.CustomerId)
                                      && invoice.INDate >= fromDateParsed.Value
                                      && invoice.INDate <= toDateParsed.Value
                                   select new
                                   {
                                       Region = region != null ? region.RegionName : (string)null,
                                       CustName = customer.CustomerName,
                                       INDate = invoice.INDate,
                                       INNumber = invoice.INNumber,
                                       ExchangeRate = currency != null ? currency.XchgRate : 0m,
                                       INQty = 1m,
                                       UnitAmount = charge.Amount,
                                       ExtAmount = charge.Amount,
                                       ExtHKDAmount = charge.Amount * (currency != null ? currency.XchgRate : 0m),
                                       HKDAmount = charge.Amount * (currency != null ? currency.XchgRate : 0m)
                                   };

                var combinedQuery = itemsQuery.Concat(chargesQuery);
                var results = combinedQuery.ToList();

                var summaryResults = results.Select(r =>
                {
                    var invoiceDate = r.INDate.HasValue ? r.INDate.Value : DateTime.MinValue;
                    var invoiceYearMonth = invoiceDate.ToString("yyyy-MM");
                    var hkdAmount = r.HKDAmount;

                    var backLogAmt = invoiceYearMonth.CompareTo(monthStrings[0]) < 0 ? hkdAmount : 0m;
                    var amt1 = invoiceYearMonth == monthStrings[0] ? hkdAmount : 0m;
                    var amt2 = invoiceYearMonth == monthStrings[1] ? hkdAmount : 0m;
                    var amt3 = invoiceYearMonth == monthStrings[2] ? hkdAmount : 0m;
                    var amt4 = invoiceYearMonth == monthStrings[3] ? hkdAmount : 0m;
                    var amt5 = invoiceYearMonth == monthStrings[4] ? hkdAmount : 0m;
                    var amt6 = invoiceYearMonth == monthStrings[5] ? hkdAmount : 0m;
                    var amt7 = invoiceYearMonth == monthStrings[6] ? hkdAmount : 0m;
                    var amt8 = invoiceYearMonth == monthStrings[7] ? hkdAmount : 0m;
                    var amt9 = invoiceYearMonth == monthStrings[8] ? hkdAmount : 0m;
                    var amt10 = invoiceYearMonth == monthStrings[9] ? hkdAmount : 0m;
                    var amt11 = invoiceYearMonth == monthStrings[10] ? hkdAmount : 0m;
                    var amt12 = invoiceYearMonth == monthStrings[11] ? hkdAmount : 0m;
                    var total = backLogAmt + amt1 + amt2 + amt3 + amt4 + amt5 + amt6 + amt7 + amt8 + amt9 + amt10 + amt11 + amt12;

                    return new
                    {
                        Region = r.Region ?? string.Empty,
                        CustName = r.CustName ?? string.Empty,
                        INDate = r.INDate,
                        INNumber = r.INNumber ?? string.Empty,
                        ExchangeRate = r.ExchangeRate,
                        INQty = r.INQty,
                        UnitAmount = r.UnitAmount,
                        ExtAmount = r.ExtAmount,
                        ExtHKDAmount = r.ExtHKDAmount,
                        BackLogAmt = backLogAmt,
                        Amt1 = amt1,
                        Amt2 = amt2,
                        Amt3 = amt3,
                        Amt4 = amt4,
                        Amt5 = amt5,
                        Amt6 = amt6,
                        Amt7 = amt7,
                        Amt8 = amt8,
                        Amt9 = amt9,
                        Amt10 = amt10,
                        Amt11 = amt11,
                        Amt12 = amt12,
                        Total = total
                    };
                })
                .OrderBy(r => r.Region)
                .ThenBy(r => r.CustName)
                .ThenBy(r => r.INDate)
                .ThenBy(r => r.INNumber)
                .ToList();

                return summaryResults.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Gets outstanding order summary matching olap_OSOrder stored procedure
        /// </summary>
        public DataSet GetOutstandingOrderSummary(string customerIdArray)
        {
            using (var context = GetDbContext())
            {
                var customerIds = ParseCustomerIdArray(customerIdArray);
                if (customerIds.Count == 0)
                {
                    return new DataSet();
                }

                var now = DateTime.Now;
                var currentMonth = new DateTime(now.Year, now.Month, 1);
                var monthStrings = new List<string>();
                for (int i = 11; i >= 0; i--)
                {
                    monthStrings.Add(currentMonth.AddMonths(-i).ToString("yyyy-MM"));
                }

                var query = from custShipping in context.OrderQTCustShipping
                            join qtItem in context.OrderQTItems on custShipping.OrderQTItemId equals qtItem.OrderQTItemId
                            join qt in context.OrderQT on qtItem.OrderQTId equals qt.OrderQTId
                            join scItem in context.OrderSCItems on qtItem.OrderQTItemId equals scItem.OrderQTItemId
                            join sc in context.OrderSC on scItem.OrderSCId equals sc.OrderSCId
                            join customer in context.Customer on sc.CustomerId equals customer.CustomerId
                            join region in context.T_Region on customer.RegionId equals region.RegionId into regionGroup
                            from region in regionGroup.DefaultIfEmpty()
                            join currency in context.T_Currency on customer.CurrencyId equals currency.CurrencyId into currencyGroup
                            from currency in currencyGroup.DefaultIfEmpty()
                            join article in context.Article on qtItem.ArticleId equals article.ArticleId into articleGroup
                            from article in articleGroup.DefaultIfEmpty()
                            join qtSupplier in context.OrderQTSupplier on qtItem.OrderQTItemId equals qtSupplier.OrderQTItemId into qtSupplierGroup
                            from qtSupplier in qtSupplierGroup.DefaultIfEmpty()
                            join supplierCurrency in context.T_Currency on qtSupplier.CurrencyId equals supplierCurrency.CurrencyId into supplierCurrencyGroup
                            from supplierCurrency in supplierCurrencyGroup.DefaultIfEmpty()
                            join qtPackage in context.OrderQTPackage on qtItem.OrderQTItemId equals qtPackage.OrderQTItemId into qtPackageGroup
                            from qtPackage in qtPackageGroup.DefaultIfEmpty()
                            where customerIds.Contains(sc.CustomerId)
                                && (custShipping.QtyOrdered - custShipping.QtyShipped) > 0
                            select new
                            {
                                Region = region != null ? region.RegionName : string.Empty,
                                CustName = customer.CustomerName ?? string.Empty,
                                ShipmentDate = custShipping.ShippedOn,
                                SCNumber = sc.SCNumber ?? string.Empty,
                                CustRef = qtItem.CustRef ?? string.Empty,
                                ArticleCode = article != null ? article.ArticleCode : string.Empty,
                                ExchangeRate = qt.ExchangeRate,
                                OrderedCny = currency != null ? currency.CurrencyCode : string.Empty,
                                OrderedUnitAmt = qtItem.Amount,
                                OrderedQty = qtItem.Qty,
                                OrderedUnit = qtItem.Unit ?? string.Empty,
                                ScheduledQty = custShipping.QtyOrdered,
                                ShippedQty = custShipping.QtyShipped,
                                FactoryCost = supplierCurrency != null && supplierCurrency.LocalCurrency ? qtItem.FactoryCost : qtItem.FactoryCost * qt.ExchangeRate,
                                FactoryCny = supplierCurrency != null ? supplierCurrency.CurrencyCode : string.Empty,
                                LocalCurrency = currency != null ? currency.LocalCurrency : false,
                                FactoryUnit = qtPackage != null ? qtPackage.Unit : string.Empty,
                                OutstandingQty = custShipping.QtyOrdered - custShipping.QtyShipped,
                                HKDAmount = currency != null && currency.LocalCurrency 
                                    ? (custShipping.QtyOrdered - custShipping.QtyShipped) * qtItem.Amount * qt.ExchangeRate 
                                    : 0m
                            };

                var results = query.ToList();
                var summaryResults = results.Select(r =>
                {
                    var shipmentDate = r.ShipmentDate.HasValue ? r.ShipmentDate.Value : DateTime.MinValue;
                    var shipmentYearMonth = shipmentDate.ToString("yyyy-MM");
                    var hkdAmount = r.HKDAmount;

                    var backLogAmt = shipmentYearMonth.CompareTo(monthStrings[0]) < 0 ? hkdAmount : 0m;
                    var amt1 = shipmentYearMonth == monthStrings[0] ? hkdAmount : 0m;
                    var amt2 = shipmentYearMonth == monthStrings[1] ? hkdAmount : 0m;
                    var amt3 = shipmentYearMonth == monthStrings[2] ? hkdAmount : 0m;
                    var amt4 = shipmentYearMonth == monthStrings[3] ? hkdAmount : 0m;
                    var amt5 = shipmentYearMonth == monthStrings[4] ? hkdAmount : 0m;
                    var amt6 = shipmentYearMonth == monthStrings[5] ? hkdAmount : 0m;
                    var amt7 = shipmentYearMonth == monthStrings[6] ? hkdAmount : 0m;
                    var amt8 = shipmentYearMonth == monthStrings[7] ? hkdAmount : 0m;
                    var amt9 = shipmentYearMonth == monthStrings[8] ? hkdAmount : 0m;
                    var amt10 = shipmentYearMonth == monthStrings[9] ? hkdAmount : 0m;
                    var amt11 = shipmentYearMonth == monthStrings[10] ? hkdAmount : 0m;
                    var amt12 = shipmentYearMonth == monthStrings[11] ? hkdAmount : 0m;
                    var total = backLogAmt + amt1 + amt2 + amt3 + amt4 + amt5 + amt6 + amt7 + amt8 + amt9 + amt10 + amt11 + amt12;

                    return new
                    {
                        Region = r.Region,
                        CustName = r.CustName,
                        ShipmentDate = r.ShipmentDate,
                        SCNumber = r.SCNumber,
                        CustRef = r.CustRef,
                        ArticleCode = r.ArticleCode,
                        ExchangeRate = r.ExchangeRate,
                        OrderedCny = r.OrderedCny,
                        OrderedUnitAmt = r.OrderedUnitAmt,
                        OrderedQty = r.OrderedQty,
                        OrderedUnit = r.OrderedUnit,
                        ScheduledQty = r.ScheduledQty,
                        ShippedQty = r.ShippedQty,
                        FactoryCost = r.FactoryCost,
                        FactoryCny = r.FactoryCny,
                        LocalCurrency = r.LocalCurrency,
                        FactoryUnit = r.FactoryUnit,
                        ExtAmount = r.LocalCurrency ? r.OutstandingQty * r.OrderedUnitAmt : 0m,
                        ExtHKDAmount = r.HKDAmount,
                        BackLogAmt = backLogAmt,
                        Amt1 = amt1,
                        Amt2 = amt2,
                        Amt3 = amt3,
                        Amt4 = amt4,
                        Amt5 = amt5,
                        Amt6 = amt6,
                        Amt7 = amt7,
                        Amt8 = amt8,
                        Amt9 = amt9,
                        Amt10 = amt10,
                        Amt11 = amt11,
                        Amt12 = amt12,
                        Total = total
                    };
                })
                .OrderBy(r => r.Region)
                .ThenBy(r => r.CustName)
                .ThenBy(r => r.ShipmentDate)
                .ThenBy(r => r.SCNumber)
                .ToList();

                return summaryResults.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Gets outstanding profit summary matching olap_OSProfit stored procedure
        /// Similar to GetOutstandingOrderSummary but calculates profit (Amount - FactoryCost)
        /// </summary>
        public DataSet GetOutstandingProfitSummary(string customerIdArray)
        {
            using (var context = GetDbContext())
            {
                var customerIds = ParseCustomerIdArray(customerIdArray);
                if (customerIds.Count == 0)
                {
                    return new DataSet();
                }

                var now = DateTime.Now;
                var currentMonth = new DateTime(now.Year, now.Month, 1);
                var monthStrings = new List<string>();
                for (int i = 11; i >= 0; i--)
                {
                    monthStrings.Add(currentMonth.AddMonths(-i).ToString("yyyy-MM"));
                }

                var query = from custShipping in context.OrderQTCustShipping
                            join qtItem in context.OrderQTItems on custShipping.OrderQTItemId equals qtItem.OrderQTItemId
                            join qt in context.OrderQT on qtItem.OrderQTId equals qt.OrderQTId
                            join scItem in context.OrderSCItems on qtItem.OrderQTItemId equals scItem.OrderQTItemId
                            join sc in context.OrderSC on scItem.OrderSCId equals sc.OrderSCId
                            join customer in context.Customer on sc.CustomerId equals customer.CustomerId
                            join region in context.T_Region on customer.RegionId equals region.RegionId into regionGroup
                            from region in regionGroup.DefaultIfEmpty()
                            join currency in context.T_Currency on customer.CurrencyId equals currency.CurrencyId into currencyGroup
                            from currency in currencyGroup.DefaultIfEmpty()
                            join article in context.Article on qtItem.ArticleId equals article.ArticleId into articleGroup
                            from article in articleGroup.DefaultIfEmpty()
                            join qtSupplier in context.OrderQTSupplier on qtItem.OrderQTItemId equals qtSupplier.OrderQTItemId into qtSupplierGroup
                            from qtSupplier in qtSupplierGroup.DefaultIfEmpty()
                            join supplierCurrency in context.T_Currency on qtSupplier.CurrencyId equals supplierCurrency.CurrencyId into supplierCurrencyGroup
                            from supplierCurrency in supplierCurrencyGroup.DefaultIfEmpty()
                            join qtPackage in context.OrderQTPackage on qtItem.OrderQTItemId equals qtPackage.OrderQTItemId into qtPackageGroup
                            from qtPackage in qtPackageGroup.DefaultIfEmpty()
                            where customerIds.Contains(sc.CustomerId)
                                && (custShipping.QtyOrdered - custShipping.QtyShipped) > 0
                            select new
                            {
                                Region = region != null ? region.RegionName : string.Empty,
                                CustName = customer.CustomerName ?? string.Empty,
                                ShipmentDate = custShipping.ShippedOn,
                                SCNumber = sc.SCNumber ?? string.Empty,
                                CustRef = qtItem.CustRef ?? string.Empty,
                                ArticleCode = article != null ? article.ArticleCode : string.Empty,
                                ExchangeRate = qt.ExchangeRate,
                                CurrencyCode = currency != null ? currency.CurrencyCode : string.Empty,
                                OrderedUnitAmt = qtItem.Amount,
                                OrderedQty = qtItem.Qty,
                                OrderedUnit = qtItem.Unit ?? string.Empty,
                                ScheduledQty = custShipping.QtyOrdered,
                                ShippedQty = custShipping.QtyShipped,
                                FactoryCost = supplierCurrency != null && !supplierCurrency.LocalCurrency 
                                    ? qtItem.FactoryCost * qt.ExchangeRate 
                                    : qtItem.FactoryCost,
                                FactoryCny = supplierCurrency != null ? supplierCurrency.CurrencyCode : string.Empty,
                                LocalCurrency = currency != null ? currency.LocalCurrency : false,
                                FactoryUnit = qtPackage != null ? qtPackage.Unit : string.Empty,
                                OutstandingQty = custShipping.QtyOrdered - custShipping.QtyShipped,
                                // Profit calculation: (Amount * ExchangeRate) - FactoryCost
                                ProfitHKDAmount = (custShipping.QtyOrdered - custShipping.QtyShipped) * 
                                    (qtItem.Amount * qt.ExchangeRate - 
                                    (supplierCurrency != null && !supplierCurrency.LocalCurrency 
                                        ? qtItem.FactoryCost * qt.ExchangeRate 
                                        : qtItem.FactoryCost))
                            };

                var results = query.ToList();
                var summaryResults = results.Select(r =>
                {
                    var shipmentDate = r.ShipmentDate.HasValue ? r.ShipmentDate.Value : DateTime.MinValue;
                    var shipmentYearMonth = shipmentDate.ToString("yyyy-MM");
                    var profitAmount = r.ProfitHKDAmount;

                    var backLogAmt = shipmentYearMonth.CompareTo(monthStrings[0]) < 0 ? profitAmount : 0m;
                    var amt1 = shipmentYearMonth == monthStrings[0] ? profitAmount : 0m;
                    var amt2 = shipmentYearMonth == monthStrings[1] ? profitAmount : 0m;
                    var amt3 = shipmentYearMonth == monthStrings[2] ? profitAmount : 0m;
                    var amt4 = shipmentYearMonth == monthStrings[3] ? profitAmount : 0m;
                    var amt5 = shipmentYearMonth == monthStrings[4] ? profitAmount : 0m;
                    var amt6 = shipmentYearMonth == monthStrings[5] ? profitAmount : 0m;
                    var amt7 = shipmentYearMonth == monthStrings[6] ? profitAmount : 0m;
                    var amt8 = shipmentYearMonth == monthStrings[7] ? profitAmount : 0m;
                    var amt9 = shipmentYearMonth == monthStrings[8] ? profitAmount : 0m;
                    var amt10 = shipmentYearMonth == monthStrings[9] ? profitAmount : 0m;
                    var amt11 = shipmentYearMonth == monthStrings[10] ? profitAmount : 0m;
                    var amt12 = shipmentYearMonth == monthStrings[11] ? profitAmount : 0m;
                    var total = backLogAmt + amt1 + amt2 + amt3 + amt4 + amt5 + amt6 + amt7 + amt8 + amt9 + amt10 + amt11 + amt12;

                    return new
                    {
                        Region = r.Region,
                        CustName = r.CustName,
                        ShipmentDate = r.ShipmentDate,
                        SCNumber = r.SCNumber,
                        CustRef = r.CustRef,
                        ArticleCode = r.ArticleCode,
                        ExchangeRate = r.ExchangeRate,
                        CurrencyCode = r.CurrencyCode,
                        OrderedUnitAmt = r.OrderedUnitAmt,
                        OrderedQty = r.OrderedQty,
                        OrderedUnit = r.OrderedUnit,
                        ScheduledQty = r.ScheduledQty,
                        ShippedQty = r.ShippedQty,
                        FactoryCost = r.FactoryCost,
                        FactoryCny = r.FactoryCny,
                        LocalCurrency = r.LocalCurrency,
                        FactoryUnit = r.FactoryUnit,
                        ExtAmount = r.OutstandingQty * r.OrderedUnitAmt,
                        ExtHKDAmount = r.ProfitHKDAmount,
                        BackLogAmt = backLogAmt,
                        Amt1 = amt1,
                        Amt2 = amt2,
                        Amt3 = amt3,
                        Amt4 = amt4,
                        Amt5 = amt5,
                        Amt6 = amt6,
                        Amt7 = amt7,
                        Amt8 = amt8,
                        Amt9 = amt9,
                        Amt10 = amt10,
                        Amt11 = amt11,
                        Amt12 = amt12,
                        Total = total
                    };
                })
                .OrderBy(r => r.Region)
                .ThenBy(r => r.CustName)
                .ThenBy(r => r.ShipmentDate)
                .ThenBy(r => r.SCNumber)
                .ToList();

                return summaryResults.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Gets sales contract summary matching olap_SalesContract stored procedure
        /// </summary>
        public DataSet GetSalesContractSummary(string customerIdArray, string fromDate, string toDate)
        {
            using (var context = GetDbContext())
            {
                var customerIds = ParseCustomerIdArray(customerIdArray);
                if (customerIds.Count == 0)
                {
                    return new DataSet();
                }

                var fromDateParsed = ParseDateParameter(fromDate);
                var toDateParsed = ParseDateParameter(toDate);
                if (!fromDateParsed.HasValue || !toDateParsed.HasValue)
                {
                    return new DataSet();
                }

                var query = from scItem in context.OrderSCItems
                            join sc in context.OrderSC on scItem.OrderSCId equals sc.OrderSCId
                            join customer in context.Customer on sc.CustomerId equals customer.CustomerId
                            join region in context.T_Region on customer.RegionId equals region.RegionId into regionGroup
                            from region in regionGroup.DefaultIfEmpty()
                            join custShipping in context.OrderQTCustShipping on scItem.OrderQTItemId equals custShipping.OrderQTItemId
                            join qtItem in context.OrderQTItems on custShipping.OrderQTItemId equals qtItem.OrderQTItemId
                            join qt in context.OrderQT on qtItem.OrderQTId equals qt.OrderQTId
                            join article in context.Article on qtItem.ArticleId equals article.ArticleId into articleGroup
                            from article in articleGroup.DefaultIfEmpty()
                            join currency in context.T_Currency on qt.CurrencyId equals currency.CurrencyId into currencyGroup
                            from currency in currencyGroup.DefaultIfEmpty()
                            where customerIds.Contains(sc.CustomerId)
                                && sc.SCDate >= fromDateParsed.Value
                                && sc.SCDate <= toDateParsed.Value
                            select new
                            {
                                Region = region != null ? region.RegionName : string.Empty,
                                CustName = customer.CustomerName ?? string.Empty,
                                SCDate = sc.SCDate,
                                SCNumber = sc.SCNumber ?? string.Empty,
                                CurrencyCode = currency != null ? currency.CurrencyCode : string.Empty,
                                ArticleCode = article != null ? article.ArticleCode : string.Empty,
                                ExtAmount = qtItem.Qty * qtItem.Amount,
                                ExtHKDAmount = qtItem.Qty * qtItem.Amount * qt.ExchangeRate
                            };

                var results = query.OrderBy(r => r.Region).ThenBy(r => r.CustName).ThenBy(r => r.SCDate).ThenBy(r => r.SCNumber).ToList();
                return results.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Gets sales turnover summary matching olap_SalesTurnover stored procedure
        /// </summary>
        public DataSet GetSalesTurnover(string customerIdArray, string fromDate, string toDate)
        {
            using (var context = GetDbContext())
            {
                var customerIds = ParseCustomerIdArray(customerIdArray);
                if (customerIds.Count == 0)
                {
                    return new DataSet();
                }

                var fromDateParsed = ParseDateParameter(fromDate);
                var toDateParsed = ParseDateParameter(toDate);
                if (!fromDateParsed.HasValue || !toDateParsed.HasValue)
                {
                    return new DataSet();
                }

                // Query 1: Invoice Items
                var itemsQuery = from item in context.OrderINItems
                                 join invoice in context.OrderIN on item.OrderINId equals invoice.OrderINId
                                 join scItem in context.OrderSCItems on item.OrderSCItemsId equals scItem.OrderSCItemsId
                                 join qtItem in context.OrderQTItems on scItem.OrderQTItemId equals qtItem.OrderQTItemId
                                 join qt in context.OrderQT on qtItem.OrderQTId equals qt.OrderQTId
                                 join customer in context.Customer on invoice.CustomerId equals customer.CustomerId
                                 join region in context.T_Region on customer.RegionId equals region.RegionId into regionGroup
                                 from region in regionGroup.DefaultIfEmpty()
                                 join paymentTerms in context.T_PaymentTerms on invoice.PricingTerms equals paymentTerms.TermsId into paymentTermsGroup
                                 from paymentTerms in paymentTermsGroup.DefaultIfEmpty()
                                 join currency in context.T_Currency on qt.CurrencyId equals currency.CurrencyId into currencyGroup
                                 from currency in currencyGroup.DefaultIfEmpty()
                                 where customerIds.Contains(invoice.CustomerId)
                                    && invoice.INDate >= fromDateParsed.Value
                                    && invoice.INDate <= toDateParsed.Value
                                 select new
                                 {
                                     Region = region != null ? region.RegionName : string.Empty,
                                     CustName = customer.CustomerName ?? string.Empty,
                                     PricingTerms = paymentTerms != null ? paymentTerms.TermsName : string.Empty,
                                     INDate = invoice.INDate,
                                     INNumber = invoice.INNumber ?? string.Empty,
                                     Currency = currency != null ? currency.CurrencyCode : string.Empty,
                                     ExtAmount = item.Qty * qtItem.Amount,
                                     ExtHKDAmount = item.Qty * qtItem.Amount * qt.ExchangeRate
                                 };

                // Query 2: Invoice Charges
                var chargesQuery = from charge in context.OrderINCharges
                                   join invoice in context.OrderIN on charge.OrderINId equals invoice.OrderINId
                                   join customer in context.Customer on invoice.CustomerId equals customer.CustomerId
                                   join region in context.T_Region on customer.RegionId equals region.RegionId into regionGroup
                                   from region in regionGroup.DefaultIfEmpty()
                                   join currency in context.T_Currency on customer.CurrencyId equals currency.CurrencyId into currencyGroup
                                   from currency in currencyGroup.DefaultIfEmpty()
                                   join paymentTerms in context.T_PaymentTerms on invoice.PricingTerms equals paymentTerms.TermsId into paymentTermsGroup
                                   from paymentTerms in paymentTermsGroup.DefaultIfEmpty()
                                   where customerIds.Contains(invoice.CustomerId)
                                      && invoice.INDate >= fromDateParsed.Value
                                      && invoice.INDate <= toDateParsed.Value
                                   select new
                                   {
                                       Region = region != null ? region.RegionName : string.Empty,
                                       CustName = customer.CustomerName ?? string.Empty,
                                       PricingTerms = paymentTerms != null ? paymentTerms.TermsName : string.Empty,
                                       INDate = invoice.INDate,
                                       INNumber = invoice.INNumber ?? string.Empty,
                                       Currency = currency != null ? currency.CurrencyCode : string.Empty,
                                       ExtAmount = charge.Amount,
                                       ExtHKDAmount = charge.Amount * (currency != null ? currency.XchgRate : 0m)
                                   };

                var combinedQuery = itemsQuery.Concat(chargesQuery);
                var results = combinedQuery
                    .OrderBy(r => r.Region)
                    .ThenBy(r => r.CustName)
                    .ThenBy(r => r.INDate)
                    .ThenBy(r => r.INNumber)
                    .ToList();

                return results.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Gets shipment summary matching olap_ShipmentSummary stored procedure
        /// </summary>
        public DataSet GetShipmentSummary(string customerIdArray, string fromDate, string currencyCode)
        {
            using (var context = GetDbContext())
            {
                var customerIds = ParseCustomerIdArray(customerIdArray);
                if (customerIds.Count == 0)
                {
                    return new DataSet();
                }

                var fromDateParsed = ParseDateParameter(fromDate);
                var now = DateTime.Now;
                var currentMonth = new DateTime(now.Year, now.Month, 1);
                
                // ShipmentSummary uses current month + 1 through + 11 (not -11 through 0)
                var monthStrings = new List<string>();
                monthStrings.Add(currentMonth.ToString("yyyy-MM")); // Amt1 = current month
                for (int i = 1; i <= 11; i++)
                {
                    monthStrings.Add(currentMonth.AddMonths(i).ToString("yyyy-MM"));
                }

                var query = from scItem in context.OrderSCItems
                            join sc in context.OrderSC on scItem.OrderSCId equals sc.OrderSCId
                            join customer in context.Customer on sc.CustomerId equals customer.CustomerId
                            join custShipping in context.OrderQTCustShipping on scItem.OrderQTItemId equals custShipping.OrderQTItemId
                            join qtItem in context.OrderQTItems on custShipping.OrderQTItemId equals qtItem.OrderQTItemId
                            join qt in context.OrderQT on qtItem.OrderQTId equals qt.OrderQTId
                            join region in context.T_Region on customer.RegionId equals region.RegionId into regionGroup
                            from region in regionGroup.DefaultIfEmpty()
                            join qtSupplier in context.OrderQTSupplier on qtItem.OrderQTItemId equals qtSupplier.OrderQTItemId into qtSupplierGroup
                            from qtSupplier in qtSupplierGroup.DefaultIfEmpty()
                            join supplierCurrency in context.T_Currency on qtSupplier.CurrencyId equals supplierCurrency.CurrencyId into supplierCurrencyGroup
                            from supplierCurrency in supplierCurrencyGroup.DefaultIfEmpty()
                            join qtPackage in context.OrderQTPackage on qtItem.OrderQTItemId equals qtPackage.OrderQTItemId into qtPackageGroup
                            from qtPackage in qtPackageGroup.DefaultIfEmpty()
                            join article in context.Article on qtItem.ArticleId equals article.ArticleId into articleGroup
                            from article in articleGroup.DefaultIfEmpty()
                            where customerIds.Contains(sc.CustomerId)
                                && (!fromDateParsed.HasValue || custShipping.ShippedOn >= fromDateParsed.Value)
                                && (custShipping.QtyShipped < custShipping.QtyOrdered || custShipping.QtyShipped == null)
                                && sc.SCNumber != null
                                && (string.IsNullOrEmpty(currencyCode) || supplierCurrency.CurrencyCode == currencyCode)
                            select new
                            {
                                Region = region != null ? region.RegionName : string.Empty,
                                CustName = customer.CustomerName ?? string.Empty,
                                ShipmentDate = custShipping.ShippedOn,
                                SCNumber = sc.SCNumber ?? string.Empty,
                                ArticleCode = article != null ? article.ArticleCode : string.Empty,
                                ExchangeRate = qt.ExchangeRate,
                                CurrencyCode = supplierCurrency != null ? supplierCurrency.CurrencyCode : string.Empty,
                                OrderedUnitAmt = qtItem.Amount,
                                OrderedQty = qtItem.Qty,
                                OrderedUnit = qtItem.Unit ?? string.Empty,
                                ScheduledQty = custShipping.QtyOrdered,
                                ShippedQty = custShipping.QtyShipped,
                                FactoryCost = supplierCurrency != null && !supplierCurrency.LocalCurrency 
                                    ? qtItem.FactoryCost * qt.ExchangeRate 
                                    : qtItem.FactoryCost,
                                FactoryCny = supplierCurrency != null ? supplierCurrency.CurrencyCode : string.Empty,
                                LocalCurrency = supplierCurrency != null ? supplierCurrency.LocalCurrency : false,
                                FactoryUnit = qtPackage != null ? qtPackage.Unit : string.Empty,
                                CustRef = qtItem.CustRef ?? string.Empty,
                                OutstandingQty = custShipping.QtyOrdered - custShipping.QtyShipped,
                                HKDAmount = (custShipping.QtyOrdered - custShipping.QtyShipped) * qtItem.Amount * qt.ExchangeRate
                            };

                var results = query.ToList();
                var summaryResults = results.Select(r =>
                {
                    var shipmentDate = r.ShipmentDate.HasValue ? r.ShipmentDate.Value : DateTime.MinValue;
                    var shipmentYearMonth = shipmentDate.ToString("yyyy-MM");
                    var hkdAmount = r.HKDAmount;

                    var backLogAmt = shipmentYearMonth.CompareTo(monthStrings[0]) < 0 ? hkdAmount : 0m;
                    var amt1 = shipmentYearMonth == monthStrings[0] ? hkdAmount : 0m;
                    var amt2 = shipmentYearMonth == monthStrings[1] ? hkdAmount : 0m;
                    var amt3 = shipmentYearMonth == monthStrings[2] ? hkdAmount : 0m;
                    var amt4 = shipmentYearMonth == monthStrings[3] ? hkdAmount : 0m;
                    var amt5 = shipmentYearMonth == monthStrings[4] ? hkdAmount : 0m;
                    var amt6 = shipmentYearMonth == monthStrings[5] ? hkdAmount : 0m;
                    var amt7 = shipmentYearMonth == monthStrings[6] ? hkdAmount : 0m;
                    var amt8 = shipmentYearMonth == monthStrings[7] ? hkdAmount : 0m;
                    var amt9 = shipmentYearMonth == monthStrings[8] ? hkdAmount : 0m;
                    var amt10 = shipmentYearMonth == monthStrings[9] ? hkdAmount : 0m;
                    var amt11 = shipmentYearMonth == monthStrings[10] ? hkdAmount : 0m;
                    var amt12 = shipmentYearMonth == monthStrings[11] ? hkdAmount : 0m;
                    var total = backLogAmt + amt1 + amt2 + amt3 + amt4 + amt5 + amt6 + amt7 + amt8 + amt9 + amt10 + amt11 + amt12;

                    return new
                    {
                        Region = r.Region,
                        CustName = r.CustName,
                        ShipmentDate = r.ShipmentDate,
                        SCNumber = r.SCNumber,
                        ArticleCode = r.ArticleCode,
                        ExchangeRate = r.ExchangeRate,
                        CurrencyCode = r.CurrencyCode,
                        OrderedUnitAmt = r.OrderedUnitAmt,
                        OrderedQty = r.OrderedQty,
                        OrderedUnit = r.OrderedUnit,
                        ScheduledQty = r.ScheduledQty,
                        ShippedQty = r.ShippedQty,
                        FactoryCost = r.FactoryCost,
                        FactoryCny = r.FactoryCny,
                        LocalCurrency = r.LocalCurrency,
                        FactoryUnit = r.FactoryUnit,
                        CustRef = r.CustRef,
                        ExtAmount = r.OutstandingQty * r.OrderedUnitAmt,
                        ExtHKDAmount = r.HKDAmount,
                        BackLogAmt = backLogAmt,
                        Amt1 = amt1,
                        Amt2 = amt2,
                        Amt3 = amt3,
                        Amt4 = amt4,
                        Amt5 = amt5,
                        Amt6 = amt6,
                        Amt7 = amt7,
                        Amt8 = amt8,
                        Amt9 = amt9,
                        Amt10 = amt10,
                        Amt11 = amt11,
                        Amt12 = amt12,
                        Total = total
                    };
                })
                .OrderBy(r => r.Region)
                .ThenBy(r => r.CustName)
                .ThenBy(r => r.ShipmentDate)
                .ThenBy(r => r.SCNumber)
                .ThenBy(r => r.ArticleCode)
                .ToList();

                return summaryResults.ToDataSet("Table");
            }
        }

        /// <summary>
        /// Gets top 10 suppliers matching olap_Top10Supplier stored procedure
        /// </summary>
        public DataSet GetTop10Suppliers(string supplierIdArray, string fromDate, string toDate)
        {
            using (var context = GetDbContext())
            {
                var supplierIds = ParseCustomerIdArray(supplierIdArray); // Reuse same parsing logic (same format)
                if (supplierIds.Count == 0)
                {
                    return new DataSet();
                }

                var fromDateParsed = ParseDateParameter(fromDate);
                var toDateParsed = ParseDateParameter(toDate);
                if (!fromDateParsed.HasValue || !toDateParsed.HasValue)
                {
                    return new DataSet();
                }

                var query = from pcItem in context.OrderPCItems
                            join pc in context.OrderPC on pcItem.OrderPCId equals pc.OrderPCId
                            join supplier in context.Supplier on pc.SupplierId equals supplier.SupplierId
                            join region in context.T_Region on supplier.RegionId equals region.RegionId into regionGroup
                            from region in regionGroup.DefaultIfEmpty()
                            join scItem in context.OrderSCItems on pcItem.OrderSCItemsId equals scItem.OrderSCItemsId
                            join qtItem in context.OrderQTItems on scItem.OrderQTItemId equals qtItem.OrderQTItemId
                            join qtSupplier in context.OrderQTSupplier on qtItem.OrderQTItemId equals qtSupplier.OrderQTItemId into qtSupplierGroup
                            from qtSupplier in qtSupplierGroup.DefaultIfEmpty()
                            join qtPackage in context.OrderQTPackage on qtItem.OrderQTItemId equals qtPackage.OrderQTItemId into qtPackageGroup
                            from qtPackage in qtPackageGroup.DefaultIfEmpty()
                            join qt in context.OrderQT on qtItem.OrderQTId equals qt.OrderQTId
                            join currency in context.T_Currency on qtSupplier.CurrencyId equals currency.CurrencyId into currencyGroup
                            from currency in currencyGroup.DefaultIfEmpty()
                            join paymentTerms in context.T_PaymentTerms on pc.PricingTerms equals paymentTerms.TermsId into paymentTermsGroup
                            from paymentTerms in paymentTermsGroup.DefaultIfEmpty()
                            where supplierIds.Contains(pc.SupplierId)
                                && pc.PCDate >= fromDateParsed.Value
                                && pc.PCDate <= toDateParsed.Value
                            select new
                            {
                                Region = region != null ? region.RegionName : string.Empty,
                                SuppName = supplier.SupplierName ?? string.Empty,
                                PricingTerms = paymentTerms != null ? paymentTerms.TermsName : string.Empty,
                                PCDate = pc.PCDate,
                                PCNumber = pc.PCNumber ?? string.Empty,
                                CurrencyCode = currency != null ? currency.CurrencyCode : string.Empty,
                                LocalCurrency = currency != null ? currency.LocalCurrency : false,
                                ExchangeRate = qt.ExchangeRate,
                                Qty = qtItem.Qty,
                                OrderedUnit = qtItem.Unit ?? string.Empty,
                                FactoryCost = qtItem.FactoryCost,
                                FactoryUnit = qtPackage != null ? qtPackage.Unit : string.Empty,
                                ExtAmount = qtItem.Qty * qtItem.FactoryCost,
                                ExtHKDAmount = currency != null && currency.LocalCurrency 
                                    ? qtItem.Qty * qtItem.FactoryCost 
                                    : qtItem.Qty * qtItem.FactoryCost * qt.ExchangeRate
                            };

                // Group by supplier and sum amounts, then take top 10
                var groupedResults = query
                    .GroupBy(r => new { r.Region, r.SuppName, r.PricingTerms, r.PCDate, r.PCNumber, r.CurrencyCode, r.LocalCurrency, r.ExchangeRate, r.OrderedUnit, r.FactoryCost, r.FactoryUnit })
                    .Select(g => new
                    {
                        Region = g.Key.Region,
                        SuppName = g.Key.SuppName,
                        PricingTerms = g.Key.PricingTerms,
                        PCDate = g.Key.PCDate,
                        PCNumber = g.Key.PCNumber,
                        CurrencyCode = g.Key.CurrencyCode,
                        LocalCurrency = g.Key.LocalCurrency,
                        ExchangeRate = g.Key.ExchangeRate,
                        Qty = g.Sum(x => x.Qty),
                        OrderedUnit = g.Key.OrderedUnit,
                        FactoryCost = g.Key.FactoryCost,
                        FactoryUnit = g.Key.FactoryUnit,
                        ExtAmount = g.Sum(x => x.ExtAmount),
                        ExtHKDAmount = g.Sum(x => x.ExtHKDAmount)
                    })
                    .OrderByDescending(r => r.ExtHKDAmount)
                    .Take(10)
                    .OrderBy(r => r.Region)
                    .ThenBy(r => r.SuppName)
                    .ToList();

                return groupedResults.ToDataSet("Table");
            }
        }

        #endregion
    }

    /// <summary>
    /// Extension methods for converting LINQ query results to DataSet
    /// </summary>
    public static class OlapServiceExtensions
    {
        /// <summary>
        /// Converts a LINQ query result to a DataSet with a single DataTable
        /// </summary>
        /// <typeparam name="T">Entity or anonymous type</typeparam>
        /// <param name="query">LINQ query result</param>
        /// <param name="tableName">Name for the DataTable (default: "Table")</param>
        /// <returns>DataSet containing the query results</returns>
        public static DataSet ToDataSet<T>(this IQueryable<T> query, string tableName = "Table")
        {
            var dataSet = new DataSet();
            var dataTable = new DataTable(tableName);

            // Execute query to get results
            var results = query.ToList();

            if (results.Count == 0)
            {
                dataSet.Tables.Add(dataTable);
                return dataSet;
            }

            // Get properties from first item (works for both entity types and anonymous types)
            var firstItem = results[0];
            var properties = firstItem.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Create columns
            foreach (var property in properties)
            {
                var columnType = property.PropertyType;
                
                // Handle nullable types
                if (columnType.IsGenericType && columnType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    columnType = Nullable.GetUnderlyingType(columnType);
                }

                dataTable.Columns.Add(property.Name, columnType ?? typeof(object));
            }

            // Add rows
            foreach (var item in results)
            {
                var row = dataTable.NewRow();
                foreach (var property in properties)
                {
                    var value = property.GetValue(item);
                    if (value == null)
                    {
                        row[property.Name] = DBNull.Value;
                    }
                    else
                    {
                        row[property.Name] = value;
                    }
                }
                dataTable.Rows.Add(row);
            }

            dataSet.Tables.Add(dataTable);
            return dataSet;
        }

        /// <summary>
        /// Converts a list of entities to a DataSet with a single DataTable
        /// </summary>
        /// <typeparam name="T">Entity or anonymous type</typeparam>
        /// <param name="list">List of entities</param>
        /// <param name="tableName">Name for the DataTable (default: "Table")</param>
        /// <returns>DataSet containing the list items</returns>
        public static DataSet ToDataSet<T>(this IEnumerable<T> list, string tableName = "Table")
        {
            return list.AsQueryable().ToDataSet(tableName);
        }
    }
}
