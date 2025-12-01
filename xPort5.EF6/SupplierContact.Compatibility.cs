// 2025-11-26 Claude Sonnet 4.5: Compatibility layer for SupplierContact entity
// Pattern: Copy from T_Category template, adjust property names (CategoryId → SupplierContactId)

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
    public partial class SupplierContact
    {
        private Guid _originalKey = Guid.Empty;

        #region Static Load Methods

        public static SupplierContact Load(Guid SupplierContactId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.SupplierContact.Find(SupplierContactId);
                if (entity != null)
                {
                    entity._originalKey = entity.SupplierContactId;
                }
                return entity;
            }
        }

        public static SupplierContact LoadWhere(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.SupplierContact.Where(SqlToLinqConverter.ConvertWhereClause(whereClause)).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.SupplierContactId;
                }
                return entity;
            }
        }

        public static SupplierContactCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                return new SupplierContactCollection(context.SupplierContact.ToList());
            }
        }

        public static SupplierContactCollection LoadCollection(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<SupplierContact> query = context.SupplierContact;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                return new SupplierContactCollection(query.ToList());
            }
        }

        public static SupplierContactCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new SupplierContactCollection(context.SupplierContact.OrderBy(orderClause).ToList());
            }
        }

        public static SupplierContactCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<SupplierContact> query = context.SupplierContact;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new SupplierContactCollection(query.OrderBy(orderClause).ToList());
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
                    if (this.SupplierContactId == Guid.Empty)
                    {
                        this.SupplierContactId = Guid.NewGuid();
                    }
                    context.SupplierContact.Add(this);
                    _originalKey = this.SupplierContactId;
                }
                else
                {
                    if (_originalKey != this.SupplierContactId)
                    {
                        Delete(_originalKey);
                        context.SupplierContact.Add(this);
                        _originalKey = this.SupplierContactId;
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
            Delete(this.SupplierContactId);
        }

        public static void Delete(Guid SupplierContactId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.SupplierContact.Find(SupplierContactId);
                if (entity != null)
                {
                    context.SupplierContact.Remove(entity);
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

            SupplierContactCollection source = !string.IsNullOrEmpty(whereClause) 
                ? LoadCollection(whereClause, orderBy, true) 
                : LoadCollection(orderBy, true);

            var sourceList = new xPort5.Common.ComboList();

            if (blankLine)
            {
                sourceList.Add(new xPort5.Common.ComboItem(blankLineText, Guid.Empty));
            }

            foreach (SupplierContact item in source)
            {
                string code = GetFormattedText(item, textFields, textFormatString);
                sourceList.Add(new xPort5.Common.ComboItem(code, item.SupplierContactId));
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

        private static string GetFormattedText(SupplierContact target, string[] textFields, string textFormatString)
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
    public class SupplierContactCollection : EntityCollection<SupplierContact>
    {
        public SupplierContactCollection() : base() { }
        public SupplierContactCollection(IList<SupplierContact> list) : base(list) { }
    }
}


