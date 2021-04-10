using System;
using NUnit.Framework;

namespace ByteDev.Azure.Cosmos.Table.UnitTests
{
    [TestFixture]
    public class TableRepositoryTests
    {
        private const string ConnString = "DefaultEndpointsProtocol=https;AccountName=myAccount;AccountKey=AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==;EndpointSuffix=core.windows.net";
        private const string TableName = "MyTable";

        public class TestEntity : CustomTableEntity
        {
        }

        private TableRepository<TestEntity> CreateSut()
        {
            return new TableRepository<TestEntity>(ConnString, TableName, false);
        }

        [TestFixture]
        public class Constructor : TableRepositoryTests
        {
            [TestCase(null)]
            [TestCase("")]
            public void WhenConnectionStringIsNullOrEmpty_ThenThrowException(string connString)
            {
                Assert.Throws<ArgumentException>(() => _ = new TableRepository<TestEntity>(connString, TableName, false));
            }

            [TestCase(null)]
            [TestCase("")]
            public void WhenTableNameIsNullOrEmpty_ThenThrowException(string tableName)
            {
                Assert.Throws<ArgumentException>(() => _ = new TableRepository<TestEntity>(ConnString, tableName, false));
            }
        }
    }
}