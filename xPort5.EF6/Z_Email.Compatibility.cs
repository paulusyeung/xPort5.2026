// 2025-11-26 Claude Sonnet 4.5: Compatibility layer for Z_Email entity
// Pattern: Copy from T_Category template, adjust property names (CategoryId → Z_EmailId)

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
    public partial class Z_Email
    {
        private Guid _originalKey = Guid.Empty;

        #region Static Load Methods

        public static Z_Email Load(Guid Z_EmailId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.Z_Email.Find(Z_EmailId);
                if (entity != null)
                {
                    entity._originalKey = entity.EmailId;
                }
                return entity;
            }
        }
        public static Z_Email Load(Guid? Z_EmailId)
        {
            if (Z_EmailId.HasValue)
            {
                return Load(Z_EmailId.Value);
            }
            return null;
        }


        public static Z_Email LoadWhere(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.Z_Email.Where(SqlToLinqConverter.ConvertWhereClause(whereClause)).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.EmailId;
                }
                return entity;
            }
        }

        public static Z_EmailCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                return new Z_EmailCollection(context.Z_Email.ToList());
            }
        }

        public static Z_EmailCollection LoadCollection(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<Z_Email> query = context.Z_Email;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                return new Z_EmailCollection(query.ToList());
            }
        }

        public static Z_EmailCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new Z_EmailCollection(context.Z_Email.OrderBy(orderClause).ToList());
            }
        }

        public static Z_EmailCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<Z_Email> query = context.Z_Email;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new Z_EmailCollection(query.OrderBy(orderClause).ToList());
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
                    if (this.EmailId == Guid.Empty)
                    {
                        this.EmailId = Guid.NewGuid();
                    }
                    context.Z_Email.Add(this);
                    _originalKey = this.EmailId;
                }
                else
                {
                    if (_originalKey != this.EmailId)
                    {
                        Delete(_originalKey);
                        context.Z_Email.Add(this);
                        _originalKey = this.EmailId;
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
            Delete(this.EmailId);
        }

        public static void Delete(Guid Z_EmailId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.Z_Email.Find(Z_EmailId);
                if (entity != null)
                {
                    context.Z_Email.Remove(entity);
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

            Z_EmailCollection source = !string.IsNullOrEmpty(whereClause) 
                ? LoadCollection(whereClause, orderBy, true) 
                : LoadCollection(orderBy, true);

            var sourceList = new xPort5.Common.ComboList();

            if (blankLine)
            {
                sourceList.Add(new xPort5.Common.ComboItem(blankLineText, Guid.Empty));
            }

            foreach (Z_Email item in source)
            {
                string code = GetFormattedText(item, textFields, textFormatString);
                sourceList.Add(new xPort5.Common.ComboItem(code, item.EmailId));
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

        private static string GetFormattedText(Z_Email target, string[] textFields, string textFormatString)
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
    public class Z_EmailCollection : EntityCollection<Z_Email>
    {
        public Z_EmailCollection() : base() { }
        public Z_EmailCollection(IList<Z_Email> list) : base(list) { }
    }
}


