using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteDev.Azure.Cosmos.Table.Model;
using Microsoft.Azure.Cosmos.Table;

namespace ByteDev.Azure.Cosmos.Table
{
    /// <summary>
    /// Represents an interface to a table repository.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity the table contains.</typeparam>
    public interface ITableRepository<TEntity> where TEntity : ITableEntity, new()
    {
        #region Retrieval

        /// <summary>
        /// Determines if an entity exists based on its partition key and row key.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="rowKey">Row key.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will contain true if the entity exists; otherwise false.</returns>
        Task<bool> ExistsAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all entities from the table.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will contain list of all entities.</returns>
        Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a count of all entities in the table.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Count of all entities in the table.</returns>
        Task<int> GetCountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a count of all entities in a particular partition.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Count of all entities in the partition.</returns>
        Task<int> GetCountAsync(string partitionKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves an entity by its partition key and row key. If the entity does not exist then
        /// a null result will be returned.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="rowKey">Row key.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will contain the entity.</returns>
        Task<TEntity> GetByKeysAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves an entity by the provided entity's partition key and row key. If the entity does not exist then
        /// a null result will be returned.
        /// </summary>
        /// <param name="entity">Table entity.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will contain the found entity.</returns>
        Task<TEntity> GetByKeysAsync(TEntity entity, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Retrieve a set of entities based on a field value.
        /// </summary>
        /// <param name="fieldName">Entity field name.</param>
        /// <param name="value">Entity field value.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will contain list of matching entities.</returns>
        Task<IList<TEntity>> FindByAsync(string fieldName, string value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve a set of entities based on a set of values for a particular field.
        /// </summary>
        /// <param name="name">Entity field name.</param>
        /// <param name="values">Set of entity field values.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will contain list of matching entities.</returns>
        Task<IList<TEntity>> FindInAsync(string name, IEnumerable<string> values, CancellationToken cancellationToken = default);

        /// <summary>
        /// Query the table using a filter.
        /// </summary>
        /// <param name="filter">Filter to apply in the query.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will contain list of matching entities.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="filter" /> is null.</exception>
        Task<IList<TEntity>> QueryAsync(Filter filter, CancellationToken cancellationToken = default);

        #endregion

        #region Insert

        /// <summary>
        /// Insert the provided entity.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts a collection of entities.
        /// </summary>
        /// <param name="entities">Entities to insert.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entities" /> is null.</exception>
        Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts a entity or replaces it if it already exists.
        /// </summary>
        /// <param name="entity">Entity to insert or replace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        Task InsertOrReplaceAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts a collection of entities or replaces them if any already exist.
        /// </summary>
        /// <param name="entities">Entities to insert or replace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entities" /> is null.</exception>
        Task InsertOrReplaceAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts a entity or merges it if it already exists.
        /// </summary>
        /// <param name="entity">Entity to insert or replace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        Task InsertOrMergeAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts a collection of entities or merges them if any already exist.
        /// </summary>
        /// <param name="entities">Entities to insert or merge.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entities" /> is null.</exception>
        Task InsertOrMergeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        #endregion

        #region Replace
        
        /// <summary>
        /// Replace an existing entity.
        /// </summary>
        /// <param name="entity">Entity to replace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        Task ReplaceAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Replaces a collection of entities. If any entities do not exist then an exception is thrown.
        /// </summary>
        /// <param name="entities">Entities to replace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entities" /> is null.</exception>
        Task ReplaceAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Replace an existing entity. If the entity does not exist no exception is thrown.
        /// </summary>
        /// <param name="entity">Entity to replace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        Task ReplaceIfExistsAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Replaces a collection of entities. If any entities do not exist no exception is thrown.
        /// </summary>
        /// <param name="entities">Entities to replace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entities" /> is null.</exception>
        Task ReplaceIfExistsAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        #endregion

        #region Merge

        /// <summary>
        /// Merge an existing entity. If the entity does not exist then an exception is thrown.
        /// </summary>
        /// <param name="entity">Entity to merge.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        Task MergeAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Merges a collection of entities. If any entities do not exist then an exception is thrown.
        /// </summary>
        /// <param name="entities">Entities to merge.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entities" /> is null.</exception>
        Task MergeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Merge an existing entity. If the entity does not exist no exception is thrown.
        /// </summary>
        /// <param name="entity">Entity to merge.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        Task MergeIfExistsAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Merge a collection of entities. If any entities do not exist no exception is thrown.
        /// </summary>
        /// <param name="entities">Entities to merge.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entities" /> is null.</exception>
        Task MergeIfExistsAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        #endregion
        
        #region Delete

        /// <summary>
        /// Delete all entities in the table.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task DeleteAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete an entity. If the entity does not exist then an exception will be thrown.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete an entity based on it's keys. If the entity does not exist then no exception will be thrown.
        /// </summary>
        /// <param name="partitionKey">Entity to delete partition key.</param>
        /// <param name="rowKey">Entity to delete row key.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task DeleteIfExistsAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete an entity. If the entity does not exist then no exception will be thrown.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        Task DeleteIfExistsAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a collection of entities. If any entity does not exist then no exception will be thrown.
        /// </summary>
        /// <param name="entities">Entities to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entities" /> is null.</exception>
        Task DeleteIfExistsAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes all entities older than the supplied DateTime using an entity Timestamp.
        /// </summary>
        /// <param name="dateTime">Entities older than this DateTime will be deleted.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task DeleteIfOlderThanAsync(DateTime dateTime, CancellationToken cancellationToken = default);

        #endregion
    }
}