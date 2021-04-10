using System.Reflection;
using ByteDev.Testing;
using Microsoft.Azure.Cosmos.Table;

namespace ByteDev.Azure.Cosmos.Table.IntTests
{
    internal static class TableRepositoryFactory
    {
        public static ITableRepository<TEntity> Create<TEntity>() where TEntity : TableEntity, new()
        {
            var assembly = Assembly.GetAssembly(typeof(TableRepositoryFactory));

            var testConn = new TestConnectionString(assembly)
            {
                FilePaths = new[]
                {
                    @"Z:\Dev\ByteDev.Azure.Cosmos.Table.IntTests.connstring"
                }
            };
            
            const string tableName = "tableinitests";

            return new TableRepository<TEntity>(testConn.GetConnectionString(), tableName, true);
        }
    }
}