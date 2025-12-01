// 2025-11-26 Claude Sonnet 4.5: Compatibility layer for OrderSP entity
// Pattern: Copy from T_Category template, adjust property names (CategoryId → OrderSPId)

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
    public partial class OrderSP
    {
        private Guid _originalKey = Guid.Empty;

        #region Static Load Methods

        public static OrderSP Load(Guid OrderSPId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.OrderSP.Find(OrderSPId);
                if (entity != null)
                {
                    entity._originalKey = entity.OrderSPId;
                }
                return entity;
            }
        }
        public static OrderSP Load(Guid? OrderSPId)
        {
            if (OrderSPId.HasValue)
            {
                return Load(OrderSPId.Value);
            }
            return null;
        }


        public static OrderSP LoadWhere(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.OrderSP.Where(SqlToLinqConverter.ConvertWhereClause(whereClause)).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.OrderSPId;
                }
                return entity;
            }
        }

        public static OrderSPCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                return new OrderSPCollection(context.OrderSP.ToList());
            }
        }

        public static OrderSPCollection LoadCollection(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<OrderSP> query = context.OrderSP;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                return new OrderSPCollection(query.ToList());
            }
        }

        public static OrderSPCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new OrderSPCollection(context.OrderSP.OrderBy(orderClause).ToList());
            }
        }

        public static OrderSPCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<OrderSP> query = context.OrderSP;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new OrderSPCollection(query.OrderBy(orderClause).ToList());
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
                    if (this.OrderSPId == Guid.Empty)
                    {
                        this.OrderSPId = Guid.NewGuid();
                    }
                    context.OrderSP.Add(this);
                    _originalKey = this.OrderSPId;
                }
                else
                {
                    if (_originalKey != this.OrderSPId)
                    {
                        Delete(_originalKey);
                        context.OrderSP.Add(this);
                        _originalKey = this.OrderSPId;
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
            Delete(this.OrderSPId);
        }

        public static void Delete(Guid OrderSPId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.OrderSP.Find(OrderSPId);
                if (entity != null)
                {
                    context.OrderSP.Remove(entity);
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

            OrderSPCollection source = !string.IsNullOrEmpty(whereClause) 
                ? LoadCollection(whereClause, orderBy, true) 
                : LoadCollection(orderBy, true);

            var sourceList = new xPort5.Common.ComboList();

            if (blankLine)
            {
                sourceList.Add(new xPort5.Common.ComboItem(blankLineText, Guid.Empty));
            }

            foreach (OrderSP item in source)
            {
                string code = GetFormattedText(item, textFields, textFormatString);
                sourceList.Add(new xPort5.Common.ComboItem(code, item.OrderSPId));
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

        private static string GetFormattedText(OrderSP target, string[] textFields, string textFormatString)
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
    public class OrderSPCollection : EntityCollection<OrderSP>
    {
        public OrderSPCollection() : base() { }
        public OrderSPCollection(IList<OrderSP> list) : base(list) { }
    }
}


