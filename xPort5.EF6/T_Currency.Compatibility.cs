// 2025-11-26 Claude Sonnet 4.5: Compatibility layer for T_Currency entity
// Pattern: Copy from T_Category template, adjust property names (CategoryId → CurrencyId)

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
    public partial class T_Currency
    {
        private Guid _originalKey = Guid.Empty;

        #region Static Load Methods

        public static T_Currency Load(Guid currencyId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.T_Currency.Find(currencyId);
                if (entity != null)
                {
                    entity._originalKey = entity.CurrencyId;
                }
                return entity;
            }
        }

        public static T_Currency Load(Guid? CurrencyId)
        {
            if (CurrencyId.HasValue)
            {
                return Load(CurrencyId.Value);
            }
            return null;
        }

        public static T_Currency LoadWhere(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                string linqExpression = SqlToLinqConverter.ConvertWhereClause(whereClause);
                var entity = context.T_Currency.Where(linqExpression).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.CurrencyId;
                }
                return entity;
            }
        }

        public static T_CurrencyCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                return new T_CurrencyCollection(context.T_Currency.ToList());
            }
        }

        public static T_CurrencyCollection LoadCollection(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<T_Currency> query = context.T_Currency;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    string linqExpression = SqlToLinqConverter.ConvertWhereClause(whereClause);
                    query = query.Where(linqExpression);
                }
                return new T_CurrencyCollection(query.ToList());
            }
        }

        public static T_CurrencyCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new T_CurrencyCollection(context.T_Currency.OrderBy(orderClause).ToList());
            }
        }

        public static T_CurrencyCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<T_Currency> query = context.T_Currency;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new T_CurrencyCollection(query.OrderBy(orderClause).ToList());
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
                    if (this.CurrencyId == Guid.Empty)
                    {
                        this.CurrencyId = Guid.NewGuid();
                    }
                    context.T_Currency.Add(this);
                    _originalKey = this.CurrencyId;
                }
                else
                {
                    if (_originalKey != this.CurrencyId)
                    {
                        Delete(_originalKey);
                        context.T_Currency.Add(this);
                        _originalKey = this.CurrencyId;
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
            Delete(this.CurrencyId);
        }

        public static void Delete(Guid currencyId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.T_Currency.Find(currencyId);
                if (entity != null)
                {
                    context.T_Currency.Remove(entity);
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

            T_CurrencyCollection source = !string.IsNullOrEmpty(whereClause) 
                ? LoadCollection(whereClause, orderBy, true) 
                : LoadCollection(orderBy, true);

            var sourceList = new xPort5.Common.ComboList();

            if (blankLine)
            {
                sourceList.Add(new xPort5.Common.ComboItem(blankLineText, Guid.Empty));
            }

            foreach (T_Currency item in source)
            {
                string code = GetFormattedText(item, textFields, textFormatString);
                sourceList.Add(new xPort5.Common.ComboItem(code, item.CurrencyId));
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

        private static string GetFormattedText(T_Currency target, string[] textFields, string textFormatString)
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
    public class T_CurrencyCollection : EntityCollection<T_Currency>
    {
        public T_CurrencyCollection() : base() { }
        public T_CurrencyCollection(IList<T_Currency> list) : base(list) { }
    }
}

