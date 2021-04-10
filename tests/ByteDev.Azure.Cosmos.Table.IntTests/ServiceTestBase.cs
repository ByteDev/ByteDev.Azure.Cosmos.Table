using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using NUnit.Framework;

namespace ByteDev.Azure.Cosmos.Table.IntTests
{
    public abstract class ServiceTestBase
    {
        private readonly ConcurrentBag<TableEntity> TrackedEntities = new ConcurrentBag<TableEntity>();

        private readonly ITableRepository<TableEntity> _repo;

        protected ServiceTestBase()
        {
            _repo = TableRepositoryFactory.Create<TableEntity>();
        }

        [TearDown]
        public async Task TearDown()
        {
            await CleanUpEntities();
        }

        private async Task CleanUpEntities()
        {
            var tasks = new List<Task>();

            foreach (var entity in TrackedEntities)
            {
                tasks.Add(_repo.DeleteIfExistsAsync(entity.PartitionKey, entity.RowKey));
            }

            await Task.WhenAll(tasks);

            TrackedEntities.Clear();
        }

        protected void Track(params TableEntity[] entities)
        {
            foreach (var entity in entities)
            {
                if(TrackedEntities.All(e => e.RowKey != entity.RowKey))
                    TrackedEntities.Add(entity);
            }
        }

        protected async Task InsertAsync(params TableEntity[] entities)
        {
            var tasks = new List<Task>();

            foreach (var entity in entities)
            {
                tasks.Add(_repo.InsertAsync(entity));
                Track(entity);
            }

            await Task.WhenAll(tasks);
        }
    }
}