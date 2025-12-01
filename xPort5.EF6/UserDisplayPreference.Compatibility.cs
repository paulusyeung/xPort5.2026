// 2025-11-26 Claude Sonnet 4.5: Compatibility layer for UserDisplayPreference entity
// Pattern: Copy from T_Category template, adjust property names (CategoryId → UserDisplayPreferenceId)

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
    public partial class UserDisplayPreference
    {
        private Guid _originalKey = Guid.Empty;
        private Dictionary<string, MetadataAttributes> _metadataDict = new Dictionary<string, MetadataAttributes>();
        private bool _metadataParsed = false;

        #region Static Load Methods

        public static UserDisplayPreference Load(Guid UserDisplayPreferenceId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.UserDisplayPreference.Find(UserDisplayPreferenceId);
                if (entity != null)
                {
                    entity._originalKey = entity.PreferenceId;
                }
                return entity;
            }
        }

        public static UserDisplayPreference LoadWhere(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.UserDisplayPreference.Where(SqlToLinqConverter.ConvertWhereClause(whereClause)).FirstOrDefault();
                if (entity != null)
                {
                    entity._originalKey = entity.PreferenceId;
                }
                return entity;
            }
        }

        public static UserDisplayPreferenceCollection LoadCollection()
        {
            using (var context = new xPort5Entities())
            {
                return new UserDisplayPreferenceCollection(context.UserDisplayPreference.ToList());
            }
        }

        public static UserDisplayPreferenceCollection LoadCollection(string whereClause)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<UserDisplayPreference> query = context.UserDisplayPreference;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                return new UserDisplayPreferenceCollection(query.ToList());
            }
        }

        public static UserDisplayPreferenceCollection LoadCollection(string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new UserDisplayPreferenceCollection(context.UserDisplayPreference.OrderBy(orderClause).ToList());
            }
        }

        public static UserDisplayPreferenceCollection LoadCollection(string whereClause, string[] orderByColumns, bool ascending)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<UserDisplayPreference> query = context.UserDisplayPreference;
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(SqlToLinqConverter.ConvertWhereClause(whereClause));
                }
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending) orderClause += " DESC";
                return new UserDisplayPreferenceCollection(query.OrderBy(orderClause).ToList());
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
                    if (this.PreferenceId == Guid.Empty)
                    {
                        this.PreferenceId = Guid.NewGuid();
                    }
                    context.UserDisplayPreference.Add(this);
                    _originalKey = this.PreferenceId;
                }
                else
                {
                    if (_originalKey != this.PreferenceId)
                    {
                        Delete(_originalKey);
                        context.UserDisplayPreference.Add(this);
                        _originalKey = this.PreferenceId;
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
            Delete(this.PreferenceId);
        }

        public static void Delete(Guid UserDisplayPreferenceId)
        {
            using (var context = new xPort5Entities())
            {
                var entity = context.UserDisplayPreference.Find(UserDisplayPreferenceId);
                if (entity != null)
                {
                    context.UserDisplayPreference.Remove(entity);
                    context.SaveChanges();
                }
            }
        }

        #endregion

        #region Metadata Support

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

        /// <summary>
        /// Get MetadataXml as Dictionary (parses XML string)
        /// </summary>
        public Dictionary<string, MetadataAttributes> GetMetadataXmlDictionary()
        {
            // Parse XML string into Dictionary if not already parsed
            if (!_metadataParsed)
            {
                ParseMetadataXml();
                _metadataParsed = true;
            }
            return _metadataDict;
        }

        /// <summary>
        /// Set MetadataXml from Dictionary (serializes to XML string)
        /// </summary>
        public void SetMetadataXmlDictionary(Dictionary<string, MetadataAttributes> value)
        {
            _metadataDict = value ?? new Dictionary<string, MetadataAttributes>();
            // Serialize Dictionary to XML string and set property
            this.MetadataXml = GenerateXml(_metadataDict);
            _metadataParsed = true;
        }

        private void ParseMetadataXml()
        {
            _metadataDict.Clear();
            string xmlString = this.MetadataXml;
            if (string.IsNullOrEmpty(xmlString))
                return;

            try
            {
                System.Xml.XmlDocument metadata = new System.Xml.XmlDocument();
                metadata.LoadXml(xmlString);

                System.Xml.XmlNodeList dataList = metadata.SelectNodes("//data");
                if (dataList != null && dataList.Count > 0)
                {
                    MetadataAttributes attributes = new MetadataAttributes();
                    foreach (System.Xml.XmlNode node in dataList)
                    {
                        if (node.ChildNodes.Count >= 2)
                        {
                            string key = node.ChildNodes[0].InnerText;
                            string value = node.ChildNodes[1].InnerText;
                            attributes.Add(new MetadataAttribute(key, value));
                        }
                    }
                    if (attributes.Count > 0)
                    {
                        _metadataDict.Add("data", attributes);
                    }
                }
                else
                {
                    System.Xml.XmlNodeList targetNode = metadata.SelectNodes("//Metadata");
                    if (targetNode != null)
                    {
                        foreach (System.Xml.XmlNode node in targetNode)
                        {
                            if (node.HasChildNodes)
                            {
                                ProcessMetadataNodes(node);
                            }
                        }
                    }
                }
            }
            catch
            {
                // If parsing fails, leave dictionary empty
            }
        }

        private void ProcessMetadataNodes(System.Xml.XmlNode node)
        {
            foreach (System.Xml.XmlNode child in node)
            {
                MetadataAttributes attributes = new MetadataAttributes();
                string metadataKey = string.Empty;
                System.Xml.XmlAttributeCollection attrList = child.Attributes;
                if (attrList != null)
                {
                    foreach (System.Xml.XmlAttribute attr in attrList)
                    {
                        if (attr.Name == "id")
                        {
                            metadataKey = attr.Value;
                        }
                        else
                        {
                            attributes.Add(new MetadataAttribute(attr.Name, attr.Value));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(metadataKey))
                {
                    _metadataDict[metadataKey] = attributes;
                }
            }
        }

        private string GenerateXml(Dictionary<string, MetadataAttributes> metadataDict)
        {
            if (metadataDict == null || metadataDict.Count == 0)
                return string.Empty;

            System.Xml.XmlDocument metadata = new System.Xml.XmlDocument();
            System.Xml.XmlNode node = metadata.AppendChild(metadata.CreateElement("Metadata"));

            foreach (KeyValuePair<string, MetadataAttributes> kvp in metadataDict)
            {
                System.Xml.XmlElement element = metadata.CreateElement("record");
                element.SetAttribute("id", kvp.Key);

                foreach (MetadataAttribute attr in kvp.Value)
                {
                    element.SetAttribute(attr.Key, attr.Value);
                }

                node.AppendChild(element);
            }

            return metadata.OuterXml;
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

            UserDisplayPreferenceCollection source = !string.IsNullOrEmpty(whereClause) 
                ? LoadCollection(whereClause, orderBy, true) 
                : LoadCollection(orderBy, true);

            var sourceList = new xPort5.Common.ComboList();

            if (blankLine)
            {
                sourceList.Add(new xPort5.Common.ComboItem(blankLineText, Guid.Empty));
            }

            foreach (UserDisplayPreference item in source)
            {
                string code = GetFormattedText(item, textFields, textFormatString);
                sourceList.Add(new xPort5.Common.ComboItem(code, item.PreferenceId));
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

        private static string GetFormattedText(UserDisplayPreference target, string[] textFields, string textFormatString)
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
    public class UserDisplayPreferenceCollection : EntityCollection<UserDisplayPreference>
    {
        public UserDisplayPreferenceCollection() : base() { }
        public UserDisplayPreferenceCollection(IList<UserDisplayPreference> list) : base(list) { }
    }
}
