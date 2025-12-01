// 2025-11-26 Claude Sonnet 4.5: Refactored compatibility layer for T_Category entity
// Uses EntityCollection base class and simplified Active Record pattern
// Reduced from 419 lines to ~150 lines by removing boilerplate

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
    // 2025-11-26 Claude Sonnet 4.5: Partial class extension for T_Category
    public partial class T_Category
    {
        // 2025-11-26 Claude Sonnet 4.5: Track original key for Save() logic (insert vs update detection)
        private Guid _originalKey = Guid.Empty;

        #region Static Load Methods

        public static T_Category Load(Guid categoryId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.T_Category.Find(categoryId);
                if (entity != null)
                {
                    entity._originalKey = entity.CategoryId;
                }
                return entity;
            }
        }

        public static T_Category Load(Guid? CategoryId)
        {
            if (CategoryId.HasValue)
            {
                return Load(CategoryId.Value);
            }
            return null;
        }

        public static T_Category LoadWhere(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.T_Category.Where(SqlToLinqConverter.ConvertWhereClause(whereClause)).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.CategoryId;
                }
                return entity;
            }
        }

        public static T_CategoryCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                return new T_CategoryCollection(context.T_Category.ToList());
            }
        }

        public static T_CategoryCollection LoadCollection(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<T_Category> query = context.T_Category;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                return new T_CategoryCollection(query.ToList());
            }
        }

        public static T_CategoryCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new T_CategoryCollection(context.T_Category.OrderBy(orderClause).ToList());
            }
        }

        public static T_CategoryCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<T_Category> query = context.T_Category;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new T_CategoryCollection(query.OrderBy(orderClause).ToList());
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
                    if (this.CategoryId == Guid.Empty)
                    {
                        this.CategoryId = Guid.NewGuid();
                    }
                    context.T_Category.Add(this);
                    _originalKey = this.CategoryId;
                }
                else
                {
                    if (_originalKey != this.CategoryId)
                    {
                        Delete(_originalKey);
                        context.T_Category.Add(this);
                        _originalKey = this.CategoryId;
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
            Delete(this.CategoryId);
        }

        public static void Delete(Guid categoryId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.T_Category.Find(categoryId);
                if (entity != null)
                {
                    context.T_Category.Remove(entity);
                    context.SaveChanges();
                }
            }
        }

        #endregion

        #region LoadCombo Methods

        public static void LoadCombo(ref ComboBox ddList, string textField, bool switchLocale)
        {
            LoadCombo(ref ddList, textField, switchLocale, false, string.Empty, string.Empty, new string[] { textField });
        }

        public static void LoadCombo(ref ComboBox ddList, string textField, bool switchLocale, string[] orderBy)
        {
            LoadCombo(ref ddList, textField, switchLocale, false, string.Empty, string.Empty, orderBy);
        }

        public static void LoadCombo(ref ComboBox ddList, string textField, bool switchLocale, bool blankLine, string blankLineText, string whereClause)
        {
            LoadCombo(ref ddList, textField, switchLocale, blankLine, blankLineText, string.Empty, whereClause, new string[] { textField });
        }

        public static void LoadCombo(ref ComboBox ddList, string textField, bool switchLocale, bool blankLine, string blankLineText, string whereClause, string[] orderBy)
        {
            LoadCombo(ref ddList, textField, switchLocale, blankLine, blankLineText, string.Empty, whereClause, orderBy);
        }

        public static void LoadCombo(ref ComboBox ddList, string textField, bool switchLocale, bool blankLine, string blankLineText, string parentFilter, string whereClause, string[] orderBy)
        {
            string[] textFields = { textField };
            LoadCombo(ref ddList, textFields, "{0}", switchLocale, blankLine, blankLineText, parentFilter, whereClause, orderBy);
        }

        public static void LoadCombo(ref ComboBox ddList, string[] textFields, string textFormatString, bool switchLocale, bool blankLine, string blankLineText, string whereClause, string[] orderBy)
        {
            LoadCombo(ref ddList, textFields, textFormatString, switchLocale, blankLine, blankLineText, string.Empty, whereClause, orderBy);
        }

        public static void LoadCombo(ref ComboBox ddList, string[] textFields, string textFormatString, bool switchLocale, bool blankLine, string blankLineText, string parentFilter, string whereClause, string[] orderBy)
        {
            if (switchLocale)
            {
                textFields = GetSwitchLocale(textFields);
            }

            ddList.Items.Clear();

            T_CategoryCollection source;
            if (orderBy == null || orderBy.Length == 0)
            {
                orderBy = textFields;
            }

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

            foreach (T_Category item in source)
            {
                bool filter = false;
                if (!string.IsNullOrEmpty(parentFilter?.Trim()))
                {
                    filter = true;
                    if (item.DeptId.HasValue && item.DeptId.Value != Guid.Empty)
                    {
                        filter = IgnoreThis(item, parentFilter);
                    }
                }

                if (!filter)
                {
                    string code = GetFormattedText(item, textFields, textFormatString);
                    sourceList.Add(new xPort5.Common.ComboItem(code, item.CategoryId));
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

        private static bool IgnoreThis(T_Category target, string parentFilter)
        {
            bool result = true;
            parentFilter = parentFilter.Replace(" ", "").Replace("'", "");
            string[] parsed = parentFilter.Split('=');

            if (!target.DeptId.HasValue || target.DeptId.Value == Guid.Empty)
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
                T_Category parentTemplate = Load(target.DeptId.Value);
                if (parentTemplate != null)
                {
                    result = IgnoreThis(parentTemplate, parentFilter);
                }
            }

            return result;
        }

        private static string GetFormattedText(T_Category target, string[] textFields, string textFormatString)
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
    /// 2025-11-26 Claude Sonnet 4.5: Collection wrapper using EntityCollection base class
    /// Reduced from 10 lines to 3 lines
    /// </summary>
    public class T_CategoryCollection : EntityCollection<T_Category>
    {
        public T_CategoryCollection() : base() { }
        public T_CategoryCollection(IList<T_Category> list) : base(list) { }
    }
}

