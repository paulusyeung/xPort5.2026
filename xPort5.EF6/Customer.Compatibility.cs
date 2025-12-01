// 2025-11-26 Claude Sonnet 4.5: Prototype compatibility layer for Customer entity
// This partial class extends the EF6-generated Customer with Active Record methods
// to maintain API compatibility with the legacy xPort5.DAL implementation

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using xPort5.EF6.Base;
using System.Linq.Dynamic.Core;
using Gizmox.WebGUI.Forms;
using System.Reflection;

namespace xPort5.EF6
{
    // 2025-11-26 Claude Sonnet 4.5: Partial class extension for Customer
    public partial class Customer
    {
        // 2025-11-26 Claude Sonnet 4.5: Track original key for Save() logic (insert vs update detection)
        private Guid _originalKey = Guid.Empty;

        /// <summary>
        /// Loads a Customer object from the database using the given CustomerId
        /// </summary>
        /// <param name="customerId">The primary key value</param>
        /// <returns>A Customer object or null if not found</returns>
        public static Customer Load(Guid customerId)
        {
            // 2025-11-26 Claude Sonnet 4.5: Use EF6 DbContext to find entity by primary key
            using (var context = new xPort5Entities())
            {
                var entity = context.Customer.Find(customerId);
                if (entity != null)
                {
                    // 2025-11-26 Claude Sonnet 4.5: Set original key for Save() logic
                    entity._originalKey = entity.CustomerId;
                }
                return entity;
            }
        }
        public static Customer Load(Guid? CustomerId)
        {
            if (CustomerId.HasValue)
            {
                return Load(CustomerId.Value);
            }
            return null;
        }


        /// <summary>
        /// Loads a Customer object from the database using the given where clause
        /// </summary>
        /// <param name="whereClause">The filter expression for the query</param>
        /// <returns>A Customer object or null if not found</returns>
        public static Customer LoadWhere(string whereClause)
        {
            // 2025-11-26 Claude Sonnet 4.5: Use Dynamic LINQ to support string-based where clauses
            using (var context = new xPort5Entities())
            {
                var entity = context.Customer.Where(SqlToLinqConverter.ConvertWhereClause(whereClause)).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.CustomerId;
                }
                return entity;
            }
        }

        /// <summary>
        /// Loads a collection of Customer objects from the database
        /// </summary>
        /// <returns>A collection containing all Customer objects</returns>
        public static CustomerCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                var entities = context.Customer.ToList();
                return new CustomerCollection(entities);
            }
        }

        /// <summary>
        /// Loads a collection of Customer objects from the database with where clause
        /// </summary>
        /// <param name="whereClause">The filter expression for the query</param>
        /// <returns>A collection of Customer objects</returns>
        public static CustomerCollection LoadCollection(string whereClause)
        {
            // 2025-11-26 Claude Sonnet 4.5: Use Dynamic LINQ for string-based where clauses
            using (var context = new xPort5Entities())
            {
                IQueryable<Customer> query = context.Customer;
                
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                
                var entities = query.ToList();
                return new CustomerCollection(entities);
            }
        }

        /// <summary>
        /// Loads a collection of Customer objects ordered by specified columns
        /// </summary>
        /// <param name="orderByColumns">Array of column names to order by</param>
        /// <param name="ascending">True for ascending, false for descending</param>
        /// <returns>A collection of Customer objects</returns>
        public static CustomerCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            // 2025-11-26 Claude Sonnet 4.5: Build Dynamic LINQ order by clause
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending)
                {
                    orderClause += " DESC";
                }

                var entities = context.Customer.OrderBy(orderClause).ToList();
                return new CustomerCollection(entities);
            }
        }

        /// <summary>
        /// Loads a collection of Customer objects with where clause and ordering
        /// </summary>
        /// <param name="whereClause">The filter expression</param>
        /// <param name="orderByColumns">Array of column names to order by</param>
        /// <param name="ascending">True for ascending, false for descending</param>
        /// <returns>A collection of Customer objects</returns>
        public static CustomerCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            // 2025-11-26 Claude Sonnet 4.5: Combine where clause and order by using Dynamic LINQ
            using (var context = new xPort5Entities())
            {
                IQueryable<Customer> query = context.Customer;
                
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }

                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending)
                {
                    orderClause += " DESC";
                }

                var entities = query.OrderBy(orderClause).ToList();
                return new CustomerCollection(entities);
            }
        }

        /// <summary>
        /// Deletes a Customer object from the database
        /// </summary>
        /// <param name="customerId">The primary key value</param>
        public static void Delete(Guid customerId)
        {
            // 2025-11-26 Claude Sonnet 4.5: Use EF6 to find and remove entity
            using (var context = new xPort5Entities())
            {
                var entity = context.Customer.Find(customerId);
                if (entity != null)
                {
                    context.Customer.Remove(entity);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Deletes this Customer object from the database
        /// </summary>
        public void Delete()
        {
            Delete(this.CustomerId);
        }

        /// <summary>
        /// Saves this Customer object to the database (insert or update)
        /// </summary>
        public void Save()
        {
            // 2025-11-26 Claude Sonnet 4.5: Mimic DAL logic - if _originalKey is empty, insert; otherwise update
            using (var context = new xPort5Entities())
            {
                if (_originalKey == Guid.Empty)
                {
                    // 2025-11-26 Claude Sonnet 4.5: New entity - insert
                    if (this.CustomerId == Guid.Empty)
                    {
                        this.CustomerId = Guid.NewGuid();
                    }
                    
                    // 2025-11-26 Claude Sonnet 4.5: Set audit fields for new record
                    if (this.CreatedOn == DateTime.MinValue || this.CreatedOn == DateTime.Parse("1900-1-1"))
                    {
                        this.CreatedOn = DateTime.Now;
                    }
                    if (this.ModifiedOn == DateTime.MinValue || this.ModifiedOn == DateTime.Parse("1900-1-1"))
                    {
                        this.ModifiedOn = DateTime.Now;
                    }
                    
                    context.Customer.Add(this);
                    _originalKey = this.CustomerId;
                }
                else
                {
                    // 2025-11-26 Claude Sonnet 4.5: Existing entity - update
                    if (_originalKey != this.CustomerId)
                    {
                        // 2025-11-26 Claude Sonnet 4.5: Primary key changed - delete old and insert new
                        Delete(_originalKey);
                        context.Customer.Add(this);
                        _originalKey = this.CustomerId;
                    }
                    else
                    {
                        // 2025-11-26 Claude Sonnet 4.5: Normal update - set ModifiedOn
                        this.ModifiedOn = DateTime.Now;
                        context.Entry(this).State = EntityState.Modified;
                    }
                }
                
                context.SaveChanges();
            }
        }

        #region LoadCombo Methods

        /// <summary>
        /// Loads a ComboBox with Customer data
        /// </summary>
        public static void LoadCombo(ref ComboBox ddList, string textField, bool switchLocale)
        {
            LoadCombo(ref ddList, textField, switchLocale, false, string.Empty, string.Empty, new string[] { textField });
        }

        /// <summary>
        /// Loads a ComboBox with Customer data and custom ordering
        /// </summary>
        public static void LoadCombo(ref ComboBox ddList, string textField, bool switchLocale, string[] orderBy)
        {
            LoadCombo(ref ddList, textField, switchLocale, false, string.Empty, string.Empty, orderBy);
        }

        /// <summary>
        /// Loads a ComboBox with Customer data, blank line option, and where clause
        /// </summary>
        public static void LoadCombo(ref ComboBox ddList, string textField, bool switchLocale, bool blankLine, string blankLineText, string whereClause)
        {
            LoadCombo(ref ddList, textField, switchLocale, blankLine, blankLineText, string.Empty, whereClause, new string[] { textField });
        }

        /// <summary>
        /// Loads a ComboBox with Customer data, blank line, where clause, and ordering
        /// </summary>
        public static void LoadCombo(ref ComboBox ddList, string textField, bool switchLocale, bool blankLine, string blankLineText, string whereClause, string[] orderBy)
        {
            LoadCombo(ref ddList, textField, switchLocale, blankLine, blankLineText, string.Empty, whereClause, orderBy);
        }

        /// <summary>
        /// Loads a ComboBox with Customer data (full parameter version)
        /// </summary>
        public static void LoadCombo(ref ComboBox ddList, string textField, bool switchLocale, bool blankLine, string blankLineText, string parentFilter, string whereClause, string[] orderBy)
        {
            string[] textFields = { textField };
            LoadCombo(ref ddList, textFields, "{0}", switchLocale, blankLine, blankLineText, parentFilter, whereClause, orderBy);
        }

        /// <summary>
        /// Loads a ComboBox with Customer data using multiple text fields
        /// </summary>
        public static void LoadCombo(ref ComboBox ddList, string[] textFields, string textFormatString, bool switchLocale, bool blankLine, string blankLineText, string whereClause, string[] orderBy)
        {
            LoadCombo(ref ddList, textFields, textFormatString, switchLocale, blankLine, blankLineText, string.Empty, whereClause, orderBy);
        }

        /// <summary>
        /// Loads a ComboBox with Customer data (master method with all parameters)
        /// </summary>
        public static void LoadCombo(ref ComboBox ddList, string[] textFields, string textFormatString, bool switchLocale, bool blankLine, string blankLineText, string parentFilter, string whereClause, string[] orderBy)
        {
            // 2025-11-26 Claude Sonnet 4.5: Implement ComboBox loading with locale support and retired filtering
            if (switchLocale)
            {
                textFields = GetSwitchLocale(textFields);
            }

            ddList.Items.Clear();

            if (orderBy == null || orderBy.Length == 0)
            {
                orderBy = textFields;
            }

            // 2025-11-26 Claude Sonnet 4.5: Filter retired records (important difference from T_Category)
            if (!string.IsNullOrEmpty(whereClause))
            {
                whereClause += " AND Retired = false";
            }
            else
            {
                whereClause = "Retired = false";
            }

            // 2025-11-26 Claude Sonnet 4.5: Load collection with where clause and ordering
            CustomerCollection source;
            if (!string.IsNullOrEmpty(whereClause))
            {
                source = LoadCollection(whereClause, orderBy, true);
            }
            else
            {
                source = LoadCollection(orderBy, true);
            }

            var sourceList = new xPort5.Common.ComboList();

            if (blankLine)
            {
                sourceList.Add(new xPort5.Common.ComboItem(blankLineText, Guid.Empty));
            }

            foreach (Customer item in source)
            {
                bool filter = false;
                if (!string.IsNullOrEmpty(parentFilter?.Trim()))
                {
                    filter = true;
                    if (item.RegionId.HasValue && item.RegionId.Value != Guid.Empty)
                    {
                        filter = IgnoreThis(item, parentFilter);
                    }
                }

                if (!filter)
                {
                    string code = GetFormattedText(item, textFields, textFormatString);
                    sourceList.Add(new xPort5.Common.ComboItem(code, item.CustomerId));
                }
            }

            ddList.DataSource = sourceList;
            ddList.DisplayMember = "Code";
            ddList.ValueMember = "Id";

            if (ddList.Items.Count > 0)
            {
                ddList.SelectedIndex = 0;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Determines if an item should be filtered based on parent filter
        /// </summary>
        private static bool IgnoreThis(Customer target, string parentFilter)
        {
            // 2025-11-26 Claude Sonnet 4.5: Port parent filtering logic from DAL
            bool result = true;
            parentFilter = parentFilter.Replace(" ", "").Replace("'", "");
            string[] parsed = parentFilter.Split('=');

            if (!target.RegionId.HasValue || target.RegionId.Value == Guid.Empty)
            {
                PropertyInfo pi = target.GetType().GetProperty(parsed[0]);
                if (pi != null)
                {
                    string filterField = pi.GetValue(target, null)?.ToString() ?? string.Empty;
                    if (filterField.Equals(parsed[1], StringComparison.OrdinalIgnoreCase))
                    {
                        result = false;
                    }
                }
            }
            else
            {
                Customer parentTemplate = Load(target.RegionId.Value);
                if (parentTemplate != null)
                {
                    result = IgnoreThis(parentTemplate, parentFilter);
                }
            }

            return result;
        }

        /// <summary>
        /// Formats text using property values and format string
        /// </summary>
        private static string GetFormattedText(Customer target, string[] textFields, string textFormatString)
        {
            // 2025-11-26 Claude Sonnet 4.5: Port text formatting logic from DAL
            for (int i = 0; i < textFields.Length; i++)
            {
                PropertyInfo pi = target.GetType().GetProperty(textFields[i]);
                string value = pi != null ? (pi.GetValue(target, null)?.ToString() ?? string.Empty) : string.Empty;
                textFormatString = textFormatString.Replace("{" + i.ToString() + "}", value);
            }
            return textFormatString;
        }

        /// <summary>
        /// Switches field names to locale-specific versions
        /// </summary>
        private static string[] GetSwitchLocale(string[] source)
        {
            // 2025-11-26 Claude Sonnet 4.5: Port locale switching logic from DAL
            switch (xPort5.Common.Config.CurrentLanguageId)
            {
                case 2:
                    source[source.Length - 1] += "_Chs";
                    break;
                case 3:
                    source[source.Length - 1] += "_Cht";
                    break;
            }
            return source;
        }

        #endregion
    }

    /// <summary>
    /// Represents a collection of Customer objects
    /// </summary>
    public class CustomerCollection : BindingList<Customer>
    {
        // 2025-11-26 Claude Sonnet 4.5: Collection wrapper for BindingList compatibility
        public CustomerCollection() : base()
        {
        }

        public CustomerCollection(IList<Customer> list) : base(list)
        {
        }
    }
}

