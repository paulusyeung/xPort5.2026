// 2025-11-26 Claude Sonnet 4.5: Compatibility layer for OrderINCharges entity
// Pattern: Copy from T_Category template, adjust property names (CategoryId → OrderINChargesId)

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
    public partial class OrderINCharges
    {
        private Guid _originalKey = Guid.Empty;

        #region Static Load Methods

        public static OrderINCharges Load(Guid OrderINChargesId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.OrderINCharges.Find(OrderINChargesId);
                if (entity != null)
                {
                    entity._originalKey = entity.OrderINChargeId;
                }
                return entity;
            }
        }

        public static OrderINCharges LoadWhere(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.OrderINCharges.Where(SqlToLinqConverter.ConvertWhereClause(whereClause)).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.OrderINChargeId;
                }
                return entity;
            }
        }

        public static OrderINChargesCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                return new OrderINChargesCollection(context.OrderINCharges.ToList());
            }
        }

        public static OrderINChargesCollection LoadCollection(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<OrderINCharges> query = context.OrderINCharges;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                return new OrderINChargesCollection(query.ToList());
            }
        }

        public static OrderINChargesCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new OrderINChargesCollection(context.OrderINCharges.OrderBy(orderClause).ToList());
            }
        }

        public static OrderINChargesCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<OrderINCharges> query = context.OrderINCharges;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new OrderINChargesCollection(query.OrderBy(orderClause).ToList());
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
                    if (this.OrderINChargeId == Guid.Empty)
                    {
                        this.OrderINChargeId = Guid.NewGuid();
                    }
                    context.OrderINCharges.Add(this);
                    _originalKey = this.OrderINChargeId;
                }
                else
                {
                    if (_originalKey != this.OrderINChargeId)
                    {
                        Delete(_originalKey);
                        context.OrderINCharges.Add(this);
                        _originalKey = this.OrderINChargeId;
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
            Delete(this.OrderINChargeId);
        }

        public static void Delete(Guid OrderINChargesId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.OrderINCharges.Find(OrderINChargesId);
                if (entity != null)
                {
                    context.OrderINCharges.Remove(entity);
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

            OrderINChargesCollection source = !string.IsNullOrEmpty(whereClause) 
                ? LoadCollection(whereClause, orderBy, true) 
                : LoadCollection(orderBy, true);

            var sourceList = new xPort5.Common.ComboList();

            if (blankLine)
            {
                sourceList.Add(new xPort5.Common.ComboItem(blankLineText, Guid.Empty));
            }

            foreach (OrderINCharges item in source)
            {
                string code = GetFormattedText(item, textFields, textFormatString);
                sourceList.Add(new xPort5.Common.ComboItem(code, item.OrderINChargeId));
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

        private static string GetFormattedText(OrderINCharges target, string[] textFields, string textFormatString)
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
    public class OrderINChargesCollection : EntityCollection<OrderINCharges>
    {
        public OrderINChargesCollection() : base() { }
        public OrderINChargesCollection(IList<OrderINCharges> list) : base(list) { }
    }
}


