// 2025-11-26 Claude Sonnet 4.5: Compatibility layer for X_EventLog entity
// Pattern: Copy from T_Category template, adjust property names (CategoryId → X_EventLogId)

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
    public partial class X_EventLog
    {
        private Guid _originalKey = Guid.Empty;

        #region Static Load Methods

        public static X_EventLog Load(Guid X_EventLogId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.X_EventLog.Find(X_EventLogId);
                if (entity != null)
                {
                    entity._originalKey = entity.EventId;
                }
                return entity;
            }
        }

        public static X_EventLog LoadWhere(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.X_EventLog.Where(SqlToLinqConverter.ConvertWhereClause(whereClause)).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.EventId;
                }
                return entity;
            }
        }

        public static X_EventLogCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                return new X_EventLogCollection(context.X_EventLog.ToList());
            }
        }

        public static X_EventLogCollection LoadCollection(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<X_EventLog> query = context.X_EventLog;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                return new X_EventLogCollection(query.ToList());
            }
        }

        public static X_EventLogCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new X_EventLogCollection(context.X_EventLog.OrderBy(orderClause).ToList());
            }
        }

        public static X_EventLogCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<X_EventLog> query = context.X_EventLog;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new X_EventLogCollection(query.OrderBy(orderClause).ToList());
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
                    if (this.EventId == Guid.Empty)
                    {
                        this.EventId = Guid.NewGuid();
                    }
                    context.X_EventLog.Add(this);
                    _originalKey = this.EventId;
                }
                else
                {
                    if (_originalKey != this.EventId)
                    {
                        Delete(_originalKey);
                        context.X_EventLog.Add(this);
                        _originalKey = this.EventId;
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
            Delete(this.EventId);
        }

        public static void Delete(Guid X_EventLogId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.X_EventLog.Find(X_EventLogId);
                if (entity != null)
                {
                    context.X_EventLog.Remove(entity);
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

            X_EventLogCollection source = !string.IsNullOrEmpty(whereClause) 
                ? LoadCollection(whereClause, orderBy, true) 
                : LoadCollection(orderBy, true);

            var sourceList = new xPort5.Common.ComboList();

            if (blankLine)
            {
                sourceList.Add(new xPort5.Common.ComboItem(blankLineText, Guid.Empty));
            }

            foreach (X_EventLog item in source)
            {
                string code = GetFormattedText(item, textFields, textFormatString);
                sourceList.Add(new xPort5.Common.ComboItem(code, item.EventId));
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

        private static string GetFormattedText(X_EventLog target, string[] textFields, string textFormatString)
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
    public class X_EventLogCollection : EntityCollection<X_EventLog>
    {
        public X_EventLogCollection() : base() { }
        public X_EventLogCollection(IList<X_EventLog> list) : base(list) { }
    }
}


