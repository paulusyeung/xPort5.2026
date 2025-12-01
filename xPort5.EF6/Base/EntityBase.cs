// 2025-11-26 Claude Sonnet 4.5: Abstract base class for entity compatibility layer
// Provides common Active Record methods to reduce code duplication across entities

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace xPort5.EF6.Base
{
    /// <summary>
    /// Abstract base class providing common Active Record pattern methods for EF6 entities
    /// Reduces code duplication from ~400 lines per entity to ~50 lines
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TKey">The primary key type (typically Guid)</typeparam>
    public abstract class EntityBase<TEntity, TKey> where TEntity : class
    {
        // 2025-11-26 Claude Sonnet 4.5: Track original key for Save() logic (insert vs update detection)
        protected TKey _originalKey;

        /// <summary>
        /// Gets the DbSet for this entity type from the context
        /// </summary>
        protected abstract DbSet<TEntity> GetDbSet(xPort5Entities context);

        /// <summary>
        /// Gets the primary key value of this entity
        /// </summary>
        protected abstract TKey GetKey();

        /// <summary>
        /// Sets the primary key value of this entity
        /// </summary>
        protected abstract void SetKey(TKey key);

        /// <summary>
        /// Generates a new primary key (typically Guid.NewGuid())
        /// </summary>
        protected abstract TKey GenerateNewKey();

        /// <summary>
        /// Checks if a key is empty/default (e.g., Guid.Empty for Guid keys)
        /// </summary>
        protected abstract bool IsKeyEmpty(TKey key);

        #region Static Load Methods

        /// <summary>
        /// Loads an entity from the database using the primary key
        /// </summary>
        /// <param name="id">The primary key value</param>
        /// <param name="getDbSet">Function to get the DbSet from context</param>
        /// <param name="setOriginalKey">Action to set the _originalKey on the loaded entity</param>
        /// <returns>The entity or null if not found</returns>
        protected static TEntity LoadById(TKey id, Func<xPort5Entities, DbSet<TEntity>> getDbSet, Action<TEntity, TKey> setOriginalKey)
        {
            using (var context = new xPort5Entities())
            {
                var entity = getDbSet(context).Find(id);
                if (entity != null)
                {
                    setOriginalKey(entity, id);
                }
                return entity;
            }
        }

        /// <summary>
        /// Loads an entity from the database using a where clause
        /// </summary>
        /// <param name="whereClause">The filter expression</param>
        /// <param name="getDbSet">Function to get the DbSet from context</param>
        /// <param name="setOriginalKey">Action to set the _originalKey on the loaded entity</param>
        /// <param name="getKey">Function to get the key from the entity</param>
        /// <returns>The entity or null if not found</returns>
        protected static TEntity LoadByWhere(string whereClause, Func<xPort5Entities, DbSet<TEntity>> getDbSet, Action<TEntity, TKey> setOriginalKey, Func<TEntity, TKey> getKey)
        {
            using (var context = new xPort5Entities())
            {
                var entity = getDbSet(context).Where(whereClause).FirstOrDefault();
                if (entity != null)
                {
                    setOriginalKey(entity, getKey(entity));
                }
                return entity;
            }
        }

        /// <summary>
        /// Loads all entities from the database
        /// </summary>
        protected static List<TEntity> LoadAll(Func<xPort5Entities, DbSet<TEntity>> getDbSet)
        {
            using (var context = new xPort5Entities())
            {
                return getDbSet(context).ToList();
            }
        }

        /// <summary>
        /// Loads entities with a where clause
        /// </summary>
        protected static List<TEntity> LoadWithWhere(string whereClause, Func<xPort5Entities, DbSet<TEntity>> getDbSet)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<TEntity> query = getDbSet(context);
                
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(whereClause);
                }
                
                return query.ToList();
            }
        }

        /// <summary>
        /// Loads entities with ordering
        /// </summary>
        protected static List<TEntity> LoadWithOrder(string[] orderByColumns, bool ascending, Func<xPort5Entities, DbSet<TEntity>> getDbSet)
        {
            using (var context = new xPort5Entities())
            {
                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending)
                {
                    orderClause += " DESC";
                }

                return getDbSet(context).OrderBy(orderClause).ToList();
            }
        }

        /// <summary>
        /// Loads entities with where clause and ordering
        /// </summary>
        protected static List<TEntity> LoadWithWhereAndOrder(string whereClause, string[] orderByColumns, bool ascending, Func<xPort5Entities, DbSet<TEntity>> getDbSet)
        {
            using (var context = new xPort5Entities())
            {
                IQueryable<TEntity> query = getDbSet(context);
                
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query = query.Where(whereClause);
                }

                string orderClause = string.Join(", ", orderByColumns);
                if (!ascending)
                {
                    orderClause += " DESC";
                }

                return query.OrderBy(orderClause).ToList();
            }
        }

        #endregion

        #region Instance Save/Delete Methods

        /// <summary>
        /// Saves this entity to the database (insert or update)
        /// </summary>
        public void Save()
        {
            using (var context = new xPort5Entities())
            {
                var dbSet = GetDbSet(context);
                var currentKey = GetKey();

                if (IsKeyEmpty(_originalKey))
                {
                    // 2025-11-26 Claude Sonnet 4.5: New entity - insert
                    if (IsKeyEmpty(currentKey))
                    {
                        SetKey(GenerateNewKey());
                        currentKey = GetKey();
                    }
                    dbSet.Add((TEntity)(object)this);
                    _originalKey = currentKey;
                }
                else
                {
                    // 2025-11-26 Claude Sonnet 4.5: Existing entity - update
                    if (!_originalKey.Equals(currentKey))
                    {
                        // 2025-11-26 Claude Sonnet 4.5: Primary key changed - delete old and insert new
                        DeleteById(_originalKey, GetDbSet);
                        dbSet.Add((TEntity)(object)this);
                        _originalKey = currentKey;
                    }
                    else
                    {
                        // 2025-11-26 Claude Sonnet 4.5: Normal update
                        context.Entry(this).State = EntityState.Modified;
                    }
                }
                
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes this entity from the database
        /// </summary>
        public void Delete()
        {
            DeleteById(GetKey(), GetDbSet);
        }

        #endregion

        #region Static Delete Methods

        /// <summary>
        /// Deletes an entity by primary key
        /// </summary>
        protected static void DeleteById(TKey id, Func<xPort5Entities, DbSet<TEntity>> getDbSet)
        {
            using (var context = new xPort5Entities())
            {
                var entity = getDbSet(context).Find(id);
                if (entity != null)
                {
                    getDbSet(context).Remove(entity);
                    context.SaveChanges();
                }
            }
        }

        #endregion
    }
}
