using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace ByteDev.Azure.Cosmos.Table
{
    internal static class FuncUtils
    {
        public static async Task SwallowNotFoundAsync<TEntity>(Func<TEntity, Task> source, TEntity entity) where TEntity : class, ITableEntity, new()
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                await source(entity);
            }
            catch (StorageException ex)
            {
                if (!ex.IsNotFound())
                    throw;
            }
        }

        public static async Task WhenAllAsync<TEntity>(Func<TEntity, Task> func, IEnumerable<TEntity> entities, int maxRunningTasks = 10) where TEntity : class, ITableEntity, new()
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var tasks = new List<Task>();

            foreach (var entity in entities)
            {
                tasks.Add(func(entity));

                if (tasks.Count == maxRunningTasks)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}