using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteDev.Azure.Cosmos.Table.Model;
using Microsoft.Azure.Cosmos.Table;

namespace ByteDev.Azure.Cosmos.Table
{
    /// <summary>
    /// Represents a repository to a table.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity the table contains.</typeparam>
    public class TableRepository<TEntity> : ITableRepository<TEntity> where TEntity : class, ITableEntity, new()
    {
        private const string PartitionKeyName = "PartitionKey";
        private const string RowKeyName = "RowKey";
        private const string Timestamp = "Timestamp";

        private readonly CloudTable _table;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.Cosmos.Table.TableRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="tableName">Table name.</param>
        /// <param name="createIfNotExists">Indicate if should create the table if it does not exist upon initialization.</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="connectionString" /> is null or empty.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="tableName" /> is null or empty.</exception>
        public TableRepository(string connectionString, string tableName, bool createIfNotExists)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));

            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var tableClient = storageAccount.CreateCloudTableClient();

            _table = tableClient.GetTableReference(tableName);

            if (createIfNotExists)
                _table.CreateIfNotExists();
        }

        #region Retrieval

        /// <summary>
        /// Determines if an entity exists based on its partition key and row key.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="rowKey">Row key.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will contain true if the entity exists; otherwise false.</returns>
        public async Task<bool> ExistsAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default)
        {
            var filter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(PartitionKeyName, QueryComparisons.Equal, partitionKey),
                TableOperators.And,
                TableQuery.GenerateFilterCondition(RowKeyName, QueryComparisons.Equal, rowKey));

            var query = new TableQuery<TEntity>()
                .Where(filter)
                .Select(new List<string>{ RowKeyName });

            var entities = await _table.ExecuteQuerySegmentedAsync(query, null, cancellationToken);

            return entities.Results.Any();
        }

        /// <summary>
        /// Retrieves all entities from the table.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will contain list of all entities.</returns>
        public Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var query = new TableQuery<TEntity>();

            return _table.ExecuteQueryAsync(query, cancellationToken);
        }

        /// <summary>
        /// Gets a count of all entities in the table.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will contain count of all entities in the table.</returns>
        public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        {
            var query = new TableQuery<TEntity>()
                .Select(new List<string> { RowKeyName });

            var entities = await _table.ExecuteQueryAsync(query, cancellationToken);

            return entities.Count;
        }

        /// <summary>
        /// Gets a count of all entities in a particular partition.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will contain count of all entities in the partition.</returns>
        public async Task<int> GetCountAsync(string partitionKey, CancellationToken cancellationToken = default)
        {
            var filter = TableQuery.GenerateFilterCondition(PartitionKeyName, QueryComparisons.Equal, partitionKey);

            var query = new TableQuery<TEntity>()
                .Where(filter)
                .Select(new List<string>{ RowKeyName });

            var entities = await _table.ExecuteQueryAsync(query, cancellationToken);

            return entities.Count;
        }

        /// <summary>
        /// Retrieve an entity by its partition key and row key. If the entity does not exist then
        /// a null result will be returned.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="rowKey">Row key.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will contain the matching entity.</returns>
        public async Task<TEntity> GetByKeysAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default)
        {
            var operation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);

            var result = await _table.ExecuteAsync(operation, cancellationToken);
            
            return result.Result as TEntity;
        }

        /// <summary>
        /// Retrieve an entity by the provided entity's partition key and row key. If the entity does not exist then
        /// a null result will be returned.
        /// </summary>
        /// <param name="entity">Table entity.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will contain the matching entity.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        public Task<TEntity> GetByKeysAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return GetByKeysAsync(entity.PartitionKey, entity.RowKey, cancellationToken);
        }

        /// <summary>
        /// Retrieve a set of entities based on a field value.
        /// </summary>
        /// <param name="fieldName">Entity field name.</param>
        /// <param name="value">Entity field value.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will contain list of matching entities.</returns>
        public Task<IList<TEntity>> FindByAsync(string fieldName, string value, CancellationToken cancellationToken = default)
        {
            var query = new TableQuery<TEntity>()
                .Where(TableQuery.GenerateFilterCondition(fieldName, QueryComparisons.Equal, value));

            return _table.ExecuteQueryAsync(query, cancellationToken);
        }

        /// <summary>
        /// Retrieve a set of entities based on a set of values for a particular field.
        /// </summary>
        /// <param name="name">Entity field name.</param>
        /// <param name="values">Set of entity field values.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will contain list of matching entities.</returns>
        public async Task<IList<TEntity>> FindInAsync(string name, IEnumerable<string> values, CancellationToken cancellationToken = default)
        {
            string combinedCondition = string.Empty;

            foreach (var val in values)
            {
                var condition = TableQuery.GenerateFilterCondition(name, QueryComparisons.Equal, val);

                combinedCondition = combinedCondition == string.Empty ? 
                    condition : 
                    TableQuery.CombineFilters(combinedCondition, TableOperators.Or, condition);
            }

            if (combinedCondition == string.Empty)
                return new List<TEntity>();

            var query = new TableQuery<TEntity>().Where(combinedCondition);

            return await _table.ExecuteQueryAsync(query, cancellationToken);
        }

        /// <summary>
        /// Query the table using a filter.
        /// </summary>
        /// <param name="filter">Filter to apply in the query.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will contain list of matching entities.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="filter" /> is null.</exception>
        public Task<IList<TEntity>> QueryAsync(Filter filter, CancellationToken cancellationToken = default)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            var filterString = FilterConverter.ToTableQueryFilter(filter);

            return _table.QueryAsync<TEntity>(filterString, cancellationToken);
        }

        #endregion

        #region Insert

        /// <summary>
        /// Insert the provided entity. If the entity already exists then an exception is thrown.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        public Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return _table.ExecuteAsync(TableOperation.Insert(entity), cancellationToken);
        }

        /// <summary>
        /// Inserts a collection of entities. If any entity already exists then an exception is thrown.
        /// </summary>
        /// <param name="entities">Entities to insert.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entities" /> is null.</exception>
        public Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return FuncUtils.WhenAllAsync(e => InsertAsync(e, cancellationToken), entities);
        }

        /// <summary>
        /// Inserts a entity or replaces it if it already exists.
        /// </summary>
        /// <param name="entity">Entity to insert or replace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        public Task InsertOrReplaceAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return _table.ExecuteAsync(TableOperation.InsertOrReplace(entity), cancellationToken);
        }

        /// <summary>
        /// Inserts a collection of entities or replaces them if any already exist.
        /// </summary>
        /// <param name="entities">Entities to insert or replace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entities" /> is null.</exception>
        public Task InsertOrReplaceAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return FuncUtils.WhenAllAsync(e => InsertOrReplaceAsync(e, cancellationToken), entities);
        }

        /// <summary>
        /// Inserts a entity or merges it if it already exists.
        /// </summary>
        /// <param name="entity">Entity to insert or replace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        public Task InsertOrMergeAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return _table.ExecuteAsync(TableOperation.InsertOrMerge(entity), cancellationToken);
        }

        /// <summary>
        /// Inserts a collection of entities or merges them if any already exist.
        /// </summary>
        /// <param name="entities">Entities to insert or merge.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entities" /> is null.</exception>
        public Task InsertOrMergeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return FuncUtils.WhenAllAsync(e => InsertOrMergeAsync(e, cancellationToken), entities);
        }

        #endregion

        #region Replace

        /// <summary>
        /// Replace an existing entity. If the entity does not exist then an exception is thrown.
        /// </summary>
        /// <param name="entity">Entity to replace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        public Task ReplaceAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return _table.ExecuteAsync(TableOperation.Replace(entity), cancellationToken);
        }

        /// <summary>
        /// Replaces a collection of entities. If any entities do not exist then an exception is thrown.
        /// </summary>
        /// <param name="entities">Entities to replace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entities" /> is null.</exception>
        public Task ReplaceAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return FuncUtils.WhenAllAsync(e => ReplaceAsync(e, cancellationToken), entities);
        }

        /// <summary>
        /// Replace an existing entity. If the entity does not exist no exception is thrown.
        /// </summary>
        /// <param name="entity">Entity to replace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        public Task ReplaceIfExistsAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return FuncUtils.SwallowNotFoundAsync(e => ReplaceAsync(e, cancellationToken), entity);
        }

        /// <summary>
        /// Replaces a collection of entities. If any entities do not exist no exception is thrown.
        /// </summary>
        /// <param name="entities">Entities to replace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entities" /> is null.</exception>
        public Task ReplaceIfExistsAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return FuncUtils.WhenAllAsync(e => ReplaceIfExistsAsync(e, cancellationToken), entities);
        }

        #endregion

        #region Merge

        /// <summary>
        /// Merge an existing entity. If the entity does not exist then an exception is thrown.
        /// </summary>
        /// <param name="entity">Entity to merge.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        public Task MergeAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return _table.ExecuteAsync(TableOperation.Merge(entity), cancellationToken);
        }

        /// <summary>
        /// Merges a collection of entities. If any entities do not exist then an exception is thrown.
        /// </summary>
        /// <param name="entities">Entities to merge.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entities" /> is null.</exception>
        public Task MergeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return FuncUtils.WhenAllAsync(e => MergeAsync(e, cancellationToken), entities);
        }

        /// <summary>
        /// Merge an existing entity. If the entity does not exist no exception is thrown.
        /// </summary>
        /// <param name="entity">Entity to merge.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        public Task MergeIfExistsAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return FuncUtils.SwallowNotFoundAsync(e => MergeAsync(e, cancellationToken), entity);
        }

        /// <summary>
        /// Merge a collection of entities. If any entities do not exist no exception is thrown.
        /// </summary>
        /// <param name="entities">Entities to merge.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entities" /> is null.</exception>
        public Task MergeIfExistsAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return FuncUtils.WhenAllAsync(e => MergeIfExistsAsync(e, cancellationToken), entities);
        }

        #endregion
        
        #region Delete

        /// <summary>
        /// Delete all entities in the table.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task DeleteAllAsync(CancellationToken cancellationToken = default)
        {
            var query = new TableQuery<TEntity>();

            var entities = await _table.ExecuteQueryAsync(query, cancellationToken);

            await FuncUtils.WhenAllAsync(e => DeleteIfExistsAsync(e, cancellationToken), entities);
        }

        /// <summary>
        /// Delete an entity. If the entity does not exist then an exception will be thrown.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return _table.DeleteAsync(entity, cancellationToken);
        }

        /// <summary>
        /// Delete an entity based on it's keys. If the entity does not exist then no exception will be thrown.
        /// </summary>
        /// <param name="partitionKey">Entity to delete partition key.</param>
        /// <param name="rowKey">Entity to delete row key.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task DeleteIfExistsAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default)
        {
            var entity = await GetByKeysAsync(partitionKey, rowKey, cancellationToken);
            
            if (entity == null)
                return;

            await FuncUtils.SwallowNotFoundAsync(e => _table.DeleteAsync(e, cancellationToken), entity);
        }

        /// <summary>
        /// Delete an entity. If the entity does not exist then no exception will be thrown.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entity" /> is null.</exception>
        public Task DeleteIfExistsAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return FuncUtils.SwallowNotFoundAsync(e => _table.DeleteAsync(e, cancellationToken), entity);
        }

        /// <summary>
        /// Delete a collection of entities. If any entity does not exist then no exception will be thrown.
        /// </summary>
        /// <param name="entities">Entities to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entities" /> is null.</exception>
        public Task DeleteIfExistsAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return FuncUtils.WhenAllAsync(e => DeleteIfExistsAsync(e, cancellationToken), entities);
        }

        /// <summary>
        /// Deletes all entities older than the supplied DateTime using an entity Timestamp.
        /// </summary>
        /// <param name="dateTime">Entities older than this DateTime will be deleted.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task DeleteIfOlderThanAsync(DateTime dateTime, CancellationToken cancellationToken = default)
        {
            var filter = TableQuery.GenerateFilterConditionForDate(Timestamp, QueryComparisons.LessThan, dateTime);

            var query = new TableQuery<TEntity>()
                .Where(filter);

            var entities = await _table.ExecuteQueryAsync(query, cancellationToken);

            await FuncUtils.WhenAllAsync(e => DeleteIfExistsAsync(e, cancellationToken), entities);
        }

        #endregion
    }
}