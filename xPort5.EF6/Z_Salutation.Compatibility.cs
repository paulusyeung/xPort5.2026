// 2025-11-26 Claude Sonnet 4.5: Compatibility layer for Z_Salutation entity
// Pattern: Copy from T_Category template, adjust property names (CategoryId → Z_SalutationId)

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
    public partial class Z_Salutation
    {
        private Guid _originalKey = Guid.Empty;

        #region Static Load Methods

        public static Z_Salutation Load(Guid Z_SalutationId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.Z_Salutation.Find(Z_SalutationId);
                if (entity != null)
                {
                    entity._originalKey = entity.SalutationId;
                }
                return entity;
            }
        }
        public static Z_Salutation Load(Guid? Z_SalutationId)
        {
            if (Z_SalutationId.HasValue)
            {
                return Load(Z_SalutationId.Value);
            }
            return null;
        }


        public static Z_Salutation LoadWhere(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.Z_Salutation.Where(SqlToLinqConverter.ConvertWhereClause(whereClause)).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.SalutationId;
                }
                return entity;
            }
        }

        public static Z_SalutationCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                return new Z_SalutationCollection(context.Z_Salutation.ToList());
            }
        }

        public static Z_SalutationCollection LoadCollection(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<Z_Salutation> query = context.Z_Salutation;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                return new Z_SalutationCollection(query.ToList());
            }
        }

        public static Z_SalutationCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new Z_SalutationCollection(context.Z_Salutation.OrderBy(orderClause).ToList());
            }
        }

        public static Z_SalutationCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<Z_Salutation> query = context.Z_Salutation;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new Z_SalutationCollection(query.OrderBy(orderClause).ToList());
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
                    if (this.SalutationId == Guid.Empty)
                    {
                        this.SalutationId = Guid.NewGuid();
                    }
                    context.Z_Salutation.Add(this);
                    _originalKey = this.SalutationId;
                }
                else
                {
                    if (_originalKey != this.SalutationId)
                    {
                        Delete(_originalKey);
                        context.Z_Salutation.Add(this);
                        _originalKey = this.SalutationId;
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
            Delete(this.SalutationId);
        }

        public static void Delete(Guid Z_SalutationId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.Z_Salutation.Find(Z_SalutationId);
                if (entity != null)
                {
                    context.Z_Salutation.Remove(entity);
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

            Z_SalutationCollection source = !string.IsNullOrEmpty(whereClause) 
                ? LoadCollection(whereClause, orderBy, true) 
                : LoadCollection(orderBy, true);

            var sourceList = new xPort5.Common.ComboList();

            if (blankLine)
            {
                sourceList.Add(new xPort5.Common.ComboItem(blankLineText, Guid.Empty));
            }

            foreach (Z_Salutation item in source)
            {
                string code = GetFormattedText(item, textFields, textFormatString);
                sourceList.Add(new xPort5.Common.ComboItem(code, item.SalutationId));
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

        private static string GetFormattedText(Z_Salutation target, string[] textFields, string textFormatString)
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
    public class Z_SalutationCollection : EntityCollection<Z_Salutation>
    {
        public Z_SalutationCollection() : base() { }
        public Z_SalutationCollection(IList<Z_Salutation> list) : base(list) { }
    }
}


