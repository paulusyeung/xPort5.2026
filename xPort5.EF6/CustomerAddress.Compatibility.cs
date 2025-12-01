// 2025-11-26 Claude Sonnet 4.5: Compatibility layer for CustomerAddress entity
// Pattern: Copy from T_Category template, adjust property names (CategoryId → CustomerAddressId)

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic.Core;
using Gizmox.WebGUI.Forms;
using System.Reflection;
using xPort5.EF6.Base;

namespace xPort5.EF6
{
    public partial class CustomerAddress
    {
        private Guid _originalKey = Guid.Empty;

        #region Static Load Methods

        public static CustomerAddress Load(Guid CustomerAddressId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.CustomerAddress.Find(CustomerAddressId);
                if (entity != null)
                {
                    entity._originalKey = entity.CustomerAddressId;
                }
                return entity;
            }
        }

        public static CustomerAddress LoadWhere(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.CustomerAddress.Where(SqlToLinqConverter.ConvertWhereClause(whereClause)).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.CustomerAddressId;
                }
                return entity;
            }
        }

        public static CustomerAddressCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                return new CustomerAddressCollection(context.CustomerAddress.ToList());
            }
        }

        public static CustomerAddressCollection LoadCollection(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<CustomerAddress> query = context.CustomerAddress;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                return new CustomerAddressCollection(query.ToList());
            }
        }

        public static CustomerAddressCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new CustomerAddressCollection(context.CustomerAddress.OrderBy(orderClause).ToList());
            }
        }

        public static CustomerAddressCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<CustomerAddress> query = context.CustomerAddress;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new CustomerAddressCollection(query.OrderBy(orderClause).ToList());
            }
        }

        #endregion

        #region Save/Delete Methods

        public void Save()
        {
            using (var context = new xPort5Entities())
            {
                if (_originalKey == Guid.Empty)
                {
                    if (this.CustomerAddressId == Guid.Empty)
                    {
                        this.CustomerAddressId = Guid.NewGuid();
                    }
                    context.CustomerAddress.Add(this);
                    _originalKey = this.CustomerAddressId;
                }
                else
                {
                    if (_originalKey != this.CustomerAddressId)
                    {
                        Delete(_originalKey);
                        context.CustomerAddress.Add(this);
                        _originalKey = this.CustomerAddressId;
                    }
                    else
                    {
                        context.Entry(this).State = EntityState.Modified;
                    }
                }
                context.SaveChanges();
            }
        }

        public void Delete()
        {
            Delete(this.CustomerAddressId);
        }

        public static void Delete(Guid CustomerAddressId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.CustomerAddress.Find(CustomerAddressId);
                if (entity != null)
                {
                    context.CustomerAddress.Remove(entity);
                    context.SaveChanges();
                }
            }
        }

        #endregion

        #region LoadCombo Methods

        public static void LoadCombo(ref ComboBox ddList, string textField, bool switchLocale)
        {
            LoadCombo(ref ddList, new string[] { textField }, "{0}", switchLocale, false, string.Empty, string.Empty, new string[] { textField });
        }

        public static void LoadCombo(ref ComboBox ddList, string textField, bool switchLocale, bool blankLine, string blankLineText, string whereClause)
        {
            LoadCombo(ref ddList, new string[] { textField }, "{0}", switchLocale, blankLine, blankLineText, whereClause, new string[] { textField });
        }

        public static void LoadCombo(ref ComboBox ddList, string[] textFields, string textFormatString, bool switchLocale, bool blankLine, string blankLineText, string whereClause, string[] orderBy)
        {
            if (switchLocale)
            {
                textFields = GetSwitchLocale(textFields);
            }

            ddList.Items.Clear();

            if (orderBy == null || orderBy.Length == 0)
            {
                orderBy = textFields;
            }

            CustomerAddressCollection source = !string.IsNullOrEmpty(whereClause) 
                ? LoadCollection(whereClause, orderBy, true) 
                : LoadCollection(orderBy, true);

            var sourceList = new xPort5.Common.ComboList();

            if (blankLine)
            {
                sourceList.Add(new xPort5.Common.ComboItem(blankLineText, Guid.Empty));
            }

            foreach (CustomerAddress item in source)
            {
                string code = GetFormattedText(item, textFields, textFormatString);
                sourceList.Add(new xPort5.Common.ComboItem(code, item.CustomerAddressId));
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

        private static string GetFormattedText(CustomerAddress target, string[] textFields, string textFormatString)
        {
            for (int i = 0; i < textFields.Length; i++)
            {
                PropertyInfo pi = target.GetType().GetProperty(textFields[i]);
                string value = pi != null ? (pi.GetValue(target, null)?.ToString() ?? string.Empty) : string.Empty;
                textFormatString = textFormatString.Replace("{" + i.ToString() + "}", value);
            }
            return textFormatString;
        }

        private static string[] GetSwitchLocale(string[] source)
        {
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
    /// Collection wrapper using EntityCollection base class
    /// </summary>
    public class CustomerAddressCollection : EntityCollection<CustomerAddress>
    {
        public CustomerAddressCollection() : base() { }
        public CustomerAddressCollection(IList<CustomerAddress> list) : base(list) { }
    }
}


