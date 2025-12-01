// 2025-11-26 Claude Sonnet 4.5: Compatibility layer for X_ErrorLog entity
// Pattern: Copy from T_Category template, adjust property names (CategoryId → X_ErrorLogId)

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
    public partial class X_ErrorLog
    {
        private Guid _originalKey = Guid.Empty;

        #region Static Load Methods

        public static X_ErrorLog Load(Guid X_ErrorLogId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.X_ErrorLog.Find(X_ErrorLogId);
                if (entity != null)
                {
                    entity._originalKey = entity.ErrorLogId;
                }
                return entity;
            }
        }

        public static X_ErrorLog LoadWhere(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.X_ErrorLog.Where(SqlToLinqConverter.ConvertWhereClause(whereClause)).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.ErrorLogId;
                }
                return entity;
            }
        }

        public static X_ErrorLogCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                return new X_ErrorLogCollection(context.X_ErrorLog.ToList());
            }
        }

        public static X_ErrorLogCollection LoadCollection(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<X_ErrorLog> query = context.X_ErrorLog;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                return new X_ErrorLogCollection(query.ToList());
            }
        }

        public static X_ErrorLogCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new X_ErrorLogCollection(context.X_ErrorLog.OrderBy(orderClause).ToList());
            }
        }

        public static X_ErrorLogCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<X_ErrorLog> query = context.X_ErrorLog;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new X_ErrorLogCollection(query.OrderBy(orderClause).ToList());
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
                    if (this.ErrorLogId == Guid.Empty)
                    {
                        this.ErrorLogId = Guid.NewGuid();
                    }
                    context.X_ErrorLog.Add(this);
                    _originalKey = this.ErrorLogId;
                }
                else
                {
                    if (_originalKey != this.ErrorLogId)
                    {
                        Delete(_originalKey);
                        context.X_ErrorLog.Add(this);
                        _originalKey = this.ErrorLogId;
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
            Delete(this.ErrorLogId);
        }

        public static void Delete(Guid X_ErrorLogId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.X_ErrorLog.Find(X_ErrorLogId);
                if (entity != null)
                {
                    context.X_ErrorLog.Remove(entity);
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

            X_ErrorLogCollection source = !string.IsNullOrEmpty(whereClause) 
                ? LoadCollection(whereClause, orderBy, true) 
                : LoadCollection(orderBy, true);

            var sourceList = new xPort5.Common.ComboList();

            if (blankLine)
            {
                sourceList.Add(new xPort5.Common.ComboItem(blankLineText, Guid.Empty));
            }

            foreach (X_ErrorLog item in source)
            {
                string code = GetFormattedText(item, textFields, textFormatString);
                sourceList.Add(new xPort5.Common.ComboItem(code, item.ErrorLogId));
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

        private static string GetFormattedText(X_ErrorLog target, string[] textFields, string textFormatString)
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
    public class X_ErrorLogCollection : EntityCollection<X_ErrorLog>
    {
        public X_ErrorLogCollection() : base() { }
        public X_ErrorLogCollection(IList<X_ErrorLog> list) : base(list) { }
    }
}


