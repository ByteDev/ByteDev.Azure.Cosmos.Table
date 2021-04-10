using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace ByteDev.Azure.Cosmos.Table
{
    internal static class CloudTableExtensions
    {
        internal static Task DeleteAsync(this CloudTable source, ITableEntity entity, CancellationToken cancellationToken = default)
        {
            return source.ExecuteAsync(TableOperation.Delete(entity), cancellationToken);
        }

        internal static Task<IList<TEntity>> QueryAsync<TEntity>(this CloudTable source, string filter, CancellationToken cancellationToken = default) where TEntity : class, ITableEntity, new()
        {
            var query = new TableQuery<TEntity>().Where(filter);

            return source.ExecuteQueryAsync(query, cancellationToken);
        }

        internal static Task<PagedResult<TEntity>> QueryPagedAsync<TEntity>(this CloudTable source, string filter, string pageToken, CancellationToken cancellationToken = default) where TEntity : class, ITableEntity, new()
        {
            var query = new TableQuery<TEntity>().Where(filter);

            return source.ExecutePagedQueryAsync(query, pageToken, cancellationToken);
        }

        internal static async Task<IList<TEntity>> ExecuteQueryAsync<TEntity>(this CloudTable source, 
            TableQuery<TEntity> query, 
            CancellationToken cancellationToken = default, 
            Action<IList<TEntity>> onProgress = null) where TEntity : ITableEntity, new()
        {
            var items = new List<TEntity>();

            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<TEntity> seg = await source.ExecuteQuerySegmentedAsync<TEntity>(query, token, cancellationToken);
                token = seg.ContinuationToken;
                items.AddRange(seg.Results);
                onProgress?.Invoke(items);
            }
            while (token != null && !cancellationToken.IsCancellationRequested);

            return items;
        }

        internal static async Task<PagedResult<TEntity>> ExecutePagedQueryAsync<TEntity>(this CloudTable source, 
            TableQuery<TEntity> query, 
            string rawTokenString, 
            CancellationToken cancellationToken = default) where TEntity : ITableEntity, new()
        {
            TableContinuationToken token = null;

            if (!string.IsNullOrEmpty(rawTokenString))
            {
                token = JsonConvert.DeserializeObject<TableContinuationToken>(Encoding.UTF8.GetString(Convert.FromBase64String(rawTokenString)));
            }

            var items = new List<TEntity>();
            var taken = 0;
            var originalTakeCount = query.TakeCount;
            var nextPageToken = token;

            do
            {
                TableQuerySegment<TEntity> seg = await source.ExecuteQuerySegmentedAsync<TEntity>(query, nextPageToken, cancellationToken);
                nextPageToken = seg.ContinuationToken;
                items.AddRange(seg.Results);
                taken += seg.Results.Count;

                // In the event that the take count is not reached, set the take count to the remainder
                if (query.TakeCount != null && taken < query.TakeCount)
                {
                    query.Take(query.TakeCount - taken);
                }
            }
            while (nextPageToken != null && !cancellationToken.IsCancellationRequested && (query.TakeCount == null || taken < originalTakeCount));

            return new PagedResult<TEntity>
            {
                Items = items,
                NextPageToken = nextPageToken != null ? Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(nextPageToken))) : null
            };
        }
    }
}