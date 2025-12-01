// 2025-11-26 Claude Sonnet 4.5: Compatibility layer for CustomerContact entity
// Pattern: Copy from T_Category template, adjust property names (CategoryId → CustomerContactId)

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
    public partial class CustomerContact
    {
        private Guid _originalKey = Guid.Empty;

        #region Static Load Methods

        public static CustomerContact Load(Guid CustomerContactId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.CustomerContact.Find(CustomerContactId);
                if (entity != null)
                {
                    entity._originalKey = entity.CustomerContactId;
                }
                return entity;
            }
        }

        public static CustomerContact LoadWhere(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.CustomerContact.Where(SqlToLinqConverter.ConvertWhereClause(whereClause)).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.CustomerContactId;
                }
                return entity;
            }
        }

        public static CustomerContactCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                return new CustomerContactCollection(context.CustomerContact.ToList());
            }
        }

        public static CustomerContactCollection LoadCollection(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<CustomerContact> query = context.CustomerContact;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                return new CustomerContactCollection(query.ToList());
            }
        }

        public static CustomerContactCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new CustomerContactCollection(context.CustomerContact.OrderBy(orderClause).ToList());
            }
        }

        public static CustomerContactCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<CustomerContact> query = context.CustomerContact;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new CustomerContactCollection(query.OrderBy(orderClause).ToList());
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
                    if (this.CustomerContactId == Guid.Empty)
                    {
                        this.CustomerContactId = Guid.NewGuid();
                    }
                    context.CustomerContact.Add(this);
                    _originalKey = this.CustomerContactId;
                }
                else
                {
                    if (_originalKey != this.CustomerContactId)
                    {
                        Delete(_originalKey);
                        context.CustomerContact.Add(this);
                        _originalKey = this.CustomerContactId;
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
            Delete(this.CustomerContactId);
        }

        public static void Delete(Guid CustomerContactId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.CustomerContact.Find(CustomerContactId);
                if (entity != null)
                {
                    context.CustomerContact.Remove(entity);
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

            CustomerContactCollection source = !string.IsNullOrEmpty(whereClause) 
                ? LoadCollection(whereClause, orderBy, true) 
                : LoadCollection(orderBy, true);

            var sourceList = new xPort5.Common.ComboList();

            if (blankLine)
            {
                sourceList.Add(new xPort5.Common.ComboItem(blankLineText, Guid.Empty));
            }

            foreach (CustomerContact item in source)
            {
                string code = GetFormattedText(item, textFields, textFormatString);
                sourceList.Add(new xPort5.Common.ComboItem(code, item.CustomerContactId));
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

        private static string GetFormattedText(CustomerContact target, string[] textFields, string textFormatString)
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
    public class CustomerContactCollection : EntityCollection<CustomerContact>
    {
        public CustomerContactCollection() : base() { }
        public CustomerContactCollection(IList<CustomerContact> list) : base(list) { }
    }
}


