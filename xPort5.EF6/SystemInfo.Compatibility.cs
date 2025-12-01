// 2025-11-26 Claude Sonnet 4.5: Compatibility layer for SystemInfo entity
// Pattern: Copy from T_Category template, adjust property names (CategoryId → SystemInfoId)

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
    public partial class SystemInfo
    {
        private Guid _originalKey = Guid.Empty;

        #region Static Load Methods

        public static SystemInfo Load(Guid SystemInfoId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.SystemInfo.Find(SystemInfoId);
                if (entity != null)
                {
                    entity._originalKey = entity.SystemId;
                }
                return entity;
            }
        }

        public static SystemInfo LoadWhere(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.SystemInfo.Where(SqlToLinqConverter.ConvertWhereClause(whereClause)).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.SystemId;
                }
                return entity;
            }
        }

        public static SystemInfoCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                return new SystemInfoCollection(context.SystemInfo.ToList());
            }
        }

        public static SystemInfoCollection LoadCollection(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<SystemInfo> query = context.SystemInfo;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                return new SystemInfoCollection(query.ToList());
            }
        }

        public static SystemInfoCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new SystemInfoCollection(context.SystemInfo.OrderBy(orderClause).ToList());
            }
        }

        public static SystemInfoCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<SystemInfo> query = context.SystemInfo;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new SystemInfoCollection(query.OrderBy(orderClause).ToList());
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
                    if (this.SystemId == Guid.Empty)
                    {
                        this.SystemId = Guid.NewGuid();
                    }
                    context.SystemInfo.Add(this);
                    _originalKey = this.SystemId;
                }
                else
                {
                    if (_originalKey != this.SystemId)
                    {
                        Delete(_originalKey);
                        context.SystemInfo.Add(this);
                        _originalKey = this.SystemId;
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
            Delete(this.SystemId);
        }

        public static void Delete(Guid SystemInfoId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.SystemInfo.Find(SystemInfoId);
                if (entity != null)
                {
                    context.SystemInfo.Remove(entity);
                    context.SaveChanges();
                }
            }
        }

        #endregion

        #region Metadata Support

        private Dictionary<string, MetadataAttributes> _metadataDict = new Dictionary<string, MetadataAttributes>();

        public class MetadataAttribute
        {
            private string _key = string.Empty;
            private string _value = string.Empty;

            public MetadataAttribute(string key, string value)
            {
                _key = key;
                _value = value;
            }

            public string Key
            {
                get { return _key; }
                set { _key = value; }
            }

            public string Value
            {
                get { return _value; }
                set { _value = value; }
            }
        }

        public class MetadataAttributes : System.ComponentModel.BindingList<MetadataAttribute>
        {
        }

        public MetadataAttributes GetMetadataList(string id)
        {
            if (this._metadataDict.ContainsKey(id))
            {
                return this._metadataDict[id];
            }
            else
            {
                return new MetadataAttributes();
            }
        }

        public string GetMetadata(string key)
        {
            if (this.GetMetadataList("data").Count > 0)
            {
                MetadataAttributes attributes = this.GetMetadataList("data");
                string value = string.Empty;
                foreach (MetadataAttribute attr in attributes)
                {
                    if (attr.Key == key)
                    {
                        value = attr.Value;
                        break;
                    }
                }
                return value;
            }
            else
            {
                return string.Empty;
            }
        }

        public void SetMetadata(string key, MetadataAttributes data)
        {
            if (this._metadataDict == null)
            {
                this._metadataDict = new Dictionary<string, MetadataAttributes>();
            }

            if (this._metadataDict.ContainsKey(key))
            {
                this._metadataDict[key] = data;
            }
            else
            {
                this._metadataDict.Add(key, data);
            }
        }

        public void SetMetadata(string key, MetadataAttribute data)
        {
            MetadataAttributes attributes = this.GetMetadataList(key);

            if (!attributes.Contains(data))
            {
                attributes.Add(data);
            }

            this.SetMetadata(key, attributes);
        }

        public void SetMetadata(string key, string data)
        {
            this.SetMetadata("data", new MetadataAttribute(key, data));
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

            SystemInfoCollection source = !string.IsNullOrEmpty(whereClause) 
                ? LoadCollection(whereClause, orderBy, true) 
                : LoadCollection(orderBy, true);

            var sourceList = new xPort5.Common.ComboList();

            if (blankLine)
            {
                sourceList.Add(new xPort5.Common.ComboItem(blankLineText, Guid.Empty));
            }

            foreach (SystemInfo item in source)
            {
                string code = GetFormattedText(item, textFields, textFormatString);
                sourceList.Add(new xPort5.Common.ComboItem(code, item.SystemId));
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

        private static string GetFormattedText(SystemInfo target, string[] textFields, string textFormatString)
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
    public class SystemInfoCollection : EntityCollection<SystemInfo>
    {
        public SystemInfoCollection() : base() { }
        public SystemInfoCollection(IList<SystemInfo> list) : base(list) { }
    }
}


