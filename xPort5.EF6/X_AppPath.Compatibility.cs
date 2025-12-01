// 2025-11-26 Claude Sonnet 4.5: Compatibility layer for X_AppPath entity
// Pattern: Copy from T_Category template, adjust property names (CategoryId → X_AppPathId)

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
    public partial class X_AppPath
    {
        private Guid _originalKey = Guid.Empty;

        #region Static Load Methods

        public static X_AppPath Load(Guid X_AppPathId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.X_AppPath.Find(X_AppPathId);
                if (entity != null)
                {
                    entity._originalKey = entity.AppPathId;
                }
                return entity;
            }
        }

        public static X_AppPath LoadWhere(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.X_AppPath.Where(SqlToLinqConverter.ConvertWhereClause(whereClause)).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.AppPathId;
                }
                return entity;
            }
        }

        public static X_AppPathCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                return new X_AppPathCollection(context.X_AppPath.ToList());
            }
        }

        public static X_AppPathCollection LoadCollection(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<X_AppPath> query = context.X_AppPath;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                return new X_AppPathCollection(query.ToList());
            }
        }

        public static X_AppPathCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new X_AppPathCollection(context.X_AppPath.OrderBy(orderClause).ToList());
            }
        }

        public static X_AppPathCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<X_AppPath> query = context.X_AppPath;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new X_AppPathCollection(query.OrderBy(orderClause).ToList());
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
                    if (this.AppPathId == Guid.Empty)
                    {
                        this.AppPathId = Guid.NewGuid();
                    }
                    context.X_AppPath.Add(this);
                    _originalKey = this.AppPathId;
                }
                else
                {
                    if (_originalKey != this.AppPathId)
                    {
                        Delete(_originalKey);
                        context.X_AppPath.Add(this);
                        _originalKey = this.AppPathId;
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
            Delete(this.AppPathId);
        }

        public static void Delete(Guid X_AppPathId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.X_AppPath.Find(X_AppPathId);
                if (entity != null)
                {
                    context.X_AppPath.Remove(entity);
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

            X_AppPathCollection source = !string.IsNullOrEmpty(whereClause) 
                ? LoadCollection(whereClause, orderBy, true) 
                : LoadCollection(orderBy, true);

            var sourceList = new xPort5.Common.ComboList();

            if (blankLine)
            {
                sourceList.Add(new xPort5.Common.ComboItem(blankLineText, Guid.Empty));
            }

            foreach (X_AppPath item in source)
            {
                string code = GetFormattedText(item, textFields, textFormatString);
                sourceList.Add(new xPort5.Common.ComboItem(code, item.AppPathId));
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

        private static string GetFormattedText(X_AppPath target, string[] textFields, string textFormatString)
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
    public class X_AppPathCollection : EntityCollection<X_AppPath>
    {
        public X_AppPathCollection() : base() { }
        public X_AppPathCollection(IList<X_AppPath> list) : base(list) { }
    }
}


