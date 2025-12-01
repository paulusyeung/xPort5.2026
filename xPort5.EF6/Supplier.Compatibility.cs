// 2025-11-26 Claude Sonnet 4.5: Compatibility layer for Supplier entity
// Pattern: Copy from T_Category template, adjust property names (CategoryId → SupplierId)

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
    public partial class Supplier
    {
        private Guid _originalKey = Guid.Empty;

        #region Static Load Methods

        public static Supplier Load(Guid SupplierId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.Supplier.Find(SupplierId);
                if (entity != null)
                {
                    entity._originalKey = entity.SupplierId;
                }
                return entity;
            }
        }

        public static Supplier Load(Guid? SupplierId)
        {
            if (SupplierId.HasValue)
            {
                return Load(SupplierId.Value);
            }
            return null;
        }

        public static Supplier LoadWhere(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.Supplier.Where(SqlToLinqConverter.ConvertWhereClause(whereClause)).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.SupplierId;
                }
                return entity;
            }
        }

        public static SupplierCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                return new SupplierCollection(context.Supplier.ToList());
            }
        }

        public static SupplierCollection LoadCollection(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<Supplier> query = context.Supplier;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                return new SupplierCollection(query.ToList());
            }
        }

        public static SupplierCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new SupplierCollection(context.Supplier.OrderBy(orderClause).ToList());
            }
        }

        public static SupplierCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<Supplier> query = context.Supplier;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new SupplierCollection(query.OrderBy(orderClause).ToList());
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
                    if (this.SupplierId == Guid.Empty)
                    {
                        this.SupplierId = Guid.NewGuid();
                    }
                    context.Supplier.Add(this);
                    _originalKey = this.SupplierId;
                }
                else
                {
                    if (_originalKey != this.SupplierId)
                    {
                        Delete(_originalKey);
                        context.Supplier.Add(this);
                        _originalKey = this.SupplierId;
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
            Delete(this.SupplierId);
        }

        public static void Delete(Guid SupplierId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.Supplier.Find(SupplierId);
                if (entity != null)
                {
                    context.Supplier.Remove(entity);
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

            SupplierCollection source = !string.IsNullOrEmpty(whereClause) 
                ? LoadCollection(whereClause, orderBy, true) 
                : LoadCollection(orderBy, true);

            var sourceList = new xPort5.Common.ComboList();

            if (blankLine)
            {
                sourceList.Add(new xPort5.Common.ComboItem(blankLineText, Guid.Empty));
            }

            foreach (Supplier item in source)
            {
                string code = GetFormattedText(item, textFields, textFormatString);
                sourceList.Add(new xPort5.Common.ComboItem(code, item.SupplierId));
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

        private static string GetFormattedText(Supplier target, string[] textFields, string textFormatString)
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
    public class SupplierCollection : EntityCollection<Supplier>
    {
        public SupplierCollection() : base() { }
        public SupplierCollection(IList<Supplier> list) : base(list) { }
    }
}


