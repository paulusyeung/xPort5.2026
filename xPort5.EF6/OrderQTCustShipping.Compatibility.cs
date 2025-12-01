// 2025-11-26 Claude Sonnet 4.5: Compatibility layer for OrderQTCustShipping entity
// Pattern: Copy from T_Category template, adjust property names (CategoryId → OrderQTCustShippingId)

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
    public partial class OrderQTCustShipping
    {
        private Guid _originalKey = Guid.Empty;

        #region Static Load Methods

        public static OrderQTCustShipping Load(Guid OrderQTCustShippingId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.OrderQTCustShipping.Find(OrderQTCustShippingId);
                if (entity != null)
                {
                    entity._originalKey = entity.OrderQTCustShippingId;
                }
                return entity;
            }
        }

        public static OrderQTCustShipping LoadWhere(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.OrderQTCustShipping.Where(SqlToLinqConverter.ConvertWhereClause(whereClause)).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.OrderQTCustShippingId;
                }
                return entity;
            }
        }

        public static OrderQTCustShippingCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                return new OrderQTCustShippingCollection(context.OrderQTCustShipping.ToList());
            }
        }

        public static OrderQTCustShippingCollection LoadCollection(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<OrderQTCustShipping> query = context.OrderQTCustShipping;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                return new OrderQTCustShippingCollection(query.ToList());
            }
        }

        public static OrderQTCustShippingCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new OrderQTCustShippingCollection(context.OrderQTCustShipping.OrderBy(orderClause).ToList());
            }
        }

        public static OrderQTCustShippingCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<OrderQTCustShipping> query = context.OrderQTCustShipping;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new OrderQTCustShippingCollection(query.OrderBy(orderClause).ToList());
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
                    if (this.OrderQTCustShippingId == Guid.Empty)
                    {
                        this.OrderQTCustShippingId = Guid.NewGuid();
                    }
                    context.OrderQTCustShipping.Add(this);
                    _originalKey = this.OrderQTCustShippingId;
                }
                else
                {
                    if (_originalKey != this.OrderQTCustShippingId)
                    {
                        Delete(_originalKey);
                        context.OrderQTCustShipping.Add(this);
                        _originalKey = this.OrderQTCustShippingId;
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
            Delete(this.OrderQTCustShippingId);
        }

        public static void Delete(Guid OrderQTCustShippingId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.OrderQTCustShipping.Find(OrderQTCustShippingId);
                if (entity != null)
                {
                    context.OrderQTCustShipping.Remove(entity);
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

            OrderQTCustShippingCollection source = !string.IsNullOrEmpty(whereClause) 
                ? LoadCollection(whereClause, orderBy, true) 
                : LoadCollection(orderBy, true);

            var sourceList = new xPort5.Common.ComboList();

            if (blankLine)
            {
                sourceList.Add(new xPort5.Common.ComboItem(blankLineText, Guid.Empty));
            }

            foreach (OrderQTCustShipping item in source)
            {
                string code = GetFormattedText(item, textFields, textFormatString);
                sourceList.Add(new xPort5.Common.ComboItem(code, item.OrderQTCustShippingId));
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

        private static string GetFormattedText(OrderQTCustShipping target, string[] textFields, string textFormatString)
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
    public class OrderQTCustShippingCollection : EntityCollection<OrderQTCustShipping>
    {
        public OrderQTCustShippingCollection() : base() { }
        public OrderQTCustShippingCollection(IList<OrderQTCustShipping> list) : base(list) { }
    }
}


