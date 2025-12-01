// 2025-11-26 Claude Sonnet 4.5: Generic base class for entity collections
// Provides BindingList<T> wrapper for compatibility with VWG forms

using System.Collections.Generic;
using System.ComponentModel;

namespace xPort5.EF6.Base
{
    /// <summary>
    /// Base class for entity collection wrappers
    /// Provides BindingList functionality for data binding in VWG forms
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public class EntityCollection<T> : BindingList<T> where T : class
    {
        /// <summary>
        /// Creates an empty collection
        /// </summary>
        public EntityCollection() : base()
        {
        }

        /// <summary>
        /// Creates a collection from an existing list
        /// </summary>
        /// <param name="list">The source list</param>
        public EntityCollection(IList<T> list) : base(list)
        {
        }
    }
}
