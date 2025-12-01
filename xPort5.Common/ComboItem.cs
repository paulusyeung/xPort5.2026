using System;
using System.Collections.Generic;

namespace xPort5.Common
{
    /// <summary>
    /// Represents a single item in a combo box or dropdown list.
    /// </summary>
    public class ComboItem
    {
        /// <summary>
        /// Gets or sets the unique identifier for this item.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the display text for this item.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets an optional code or abbreviation.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Initializes a new instance of the ComboItem class.
        /// </summary>
        public ComboItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ComboItem class with Id and Text.
        /// </summary>
        public ComboItem(Guid id, string text)
        {
            Id = id;
            Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the ComboItem class with Id, Text, and Code.
        /// </summary>
        public ComboItem(Guid id, string text, string code)
        {
            Id = id;
            Text = text;
            Code = code;
        }

        /// <summary>
        /// Legacy constructor for backward compatibility with xPort5.DAL.Common.
        /// Parameter order: (code, id) - matches legacy signature.
        /// </summary>
        public ComboItem(string code, Guid id)
        {
            Code = code;
            Id = id;
            Text = code; // Default Text to Code for compatibility
        }

        public override string ToString()
        {
            return Text ?? string.Empty;
        }
    }

    /// <summary>
    /// A strongly-typed collection of ComboItem objects, bound to UI controls.
    /// </summary>
    public class ComboList : List<ComboItem>
    {
        /// <summary>
        /// Finds a ComboItem by its Id.
        /// </summary>
        public ComboItem FindById(Guid id)
        {
            return this.Find(item => item.Id == id);
        }

        /// <summary>
        /// Finds a ComboItem by its Text.
        /// </summary>
        public ComboItem FindByText(string text)
        {
            return this.Find(item => item.Text == text);
        }

        /// <summary>
        /// Finds a ComboItem by its Code.
        /// </summary>
        public ComboItem FindByCode(string code)
        {
            return this.Find(item => item.Code == code);
        }
    }
}