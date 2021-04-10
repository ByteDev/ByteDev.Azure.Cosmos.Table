using System;
using System.Linq;
using System.Threading.Tasks;
using ByteDev.Azure.Cosmos.Table.IntTests.Entities;
using ByteDev.Azure.Cosmos.Table.Model;
using ByteDev.Collections;
using Microsoft.Azure.Cosmos.Table;
using NUnit.Framework;

namespace ByteDev.Azure.Cosmos.Table.IntTests
{
    [TestFixture]
    [NonParallelizable]
    public class TableRepositoryTests : ServiceTestBase
    {
        private ITableRepository<PersonEntity> _sut;

        [SetUp]
        public async Task SetUp()
        {
            _sut = TableRepositoryFactory.Create<PersonEntity>();
            
            await _sut.DeleteAllAsync();
        }

        #region Retrieval

        [TestFixture]
        public class ExistsAsync : TableRepositoryTests
        {
            [Test]
            public async Task WhenDoesNotExist_ThenReturnFalse()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1);

                var result = await _sut.ExistsAsync(e1.PartitionKey, Guid.NewGuid().ToString());

                Assert.That(result, Is.False);
            }

            [Test]
            public async Task WhenExists_ThenReturnTrue()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1);

                var result = await _sut.ExistsAsync(e1.PartitionKey, e1.RowKey);

                Assert.That(result, Is.True);
            }
        }

        [TestFixture]
        public class GetAllAsync : TableRepositoryTests
        {
            [Test]
            public async Task WhenTableIsEmpty_ThenReturnEmpty()
            {
                var result = await _sut.GetAllAsync();

                Assert.That(result, Is.Empty);
            }

            [Test]
            public async Task WhenTableIsNotEmpty_ThenReturnAllEntities()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                var e2 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1);
                await InsertAsync(e2);

                var result = await _sut.GetAllAsync();

                Assert.That(result.Count, Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class GetCountAsync : TableRepositoryTests
        {
            [Test]
            public async Task WhenTableIsEmpty_ThenReturnZero()
            {
                var result = await _sut.GetCountAsync();

                Assert.That(result, Is.EqualTo(0));
            }

            [Test]
            public async Task WhenTableHasEntities_ThenReturnCount()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                var e2 = PersonTestBuilder.InMemory.Build();
                var e3 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1, e2, e3);

                var result = await _sut.GetCountAsync();

                Assert.That(result, Is.EqualTo(3));
            }
        }

        [TestFixture]
        public class GetCountAsync_ParitionKey : TableRepositoryTests
        {
            [Test]
            public async Task WhenTableHasEmptyPartition_ThenReturnZero()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1);

                var result = await _sut.GetCountAsync(Guid.NewGuid().ToString());

                Assert.That(result, Is.EqualTo(0));
            }

            [Test]
            public async Task WhenTableHasEntitiesInPartition_ThenReturnCount()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                var e2 = PersonTestBuilder.InMemory.Build();
                var e3 = PersonTestBuilder.InMemory.Build();

                e3.PartitionKey = e2.PartitionKey;

                await InsertAsync(e1, e2, e3);

                var result = await _sut.GetCountAsync(e2.PartitionKey);

                Assert.That(result, Is.EqualTo(2));
            }

            [TestCase(null)]
            [TestCase("")]
            public async Task WhenKeyIsNullOrEmpty_ThenReturnZero(string partitionKey)
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                var e2 = PersonTestBuilder.InMemory.Build();
                var e3 = PersonTestBuilder.InMemory.Build();

                e3.PartitionKey = e2.PartitionKey;

                await InsertAsync(e1, e2, e3);

                var result = await _sut.GetCountAsync(partitionKey);

                Assert.That(result, Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class GetByKeysAsync_Keys : TableRepositoryTests
        {
            [Test]
            public async Task WhenExists_ThenReturnEntity()
            {
                var entity = PersonTestBuilder.InMemory.Build();

                await InsertAsync(entity);

                var result = await _sut.GetByKeysAsync(entity.PartitionKey, entity.RowKey);

                Assert.That(result.PartitionKey, Is.EqualTo(entity.PartitionKey));
                Assert.That(result.RowKey, Is.EqualTo(entity.RowKey));
            }

            [Test]
            public async Task WhenDoesNotExist_ThenReturnNull()
            {
                var entity = PersonTestBuilder.InMemory.Build();

                var result = await _sut.GetByKeysAsync(entity.PartitionKey, entity.RowKey);

                Assert.That(result, Is.Null);
            }
        }

        [TestFixture]
        public class GetByKeysAsync_Entity : TableRepositoryTests
        {
            [Test]
            public async Task WhenEntityExists_ThenReturnEntity()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1);

                var result = await _sut.GetByKeysAsync(e1);

                Assert.That(result.RowKey, Is.EqualTo(e1.RowKey));
            }

            [Test]
            public async Task WhenDoesNotExist_ThenReturnNull()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                var result = await _sut.GetByKeysAsync(e1);

                Assert.That(result, Is.Null);
            }
        }

        [TestFixture]
        public class FindByAsync_Pair : TableRepositoryTests
        {
            [Test]
            public async Task WhenEntitysExistWithMatchingCriteria_ThenReturnEntity()
            {
                var e1 = PersonTestBuilder.InMemory.WithAge("50").Build();
                var e2 = PersonTestBuilder.InMemory.WithAge("40").Build();
                var e3 = PersonTestBuilder.InMemory.WithAge("50").Build();

                await InsertAsync(e1, e2, e3);

                var result = await _sut.FindByAsync(nameof(e1.Age), "50");

                Assert.That(result.First().RowKey, Is.EqualTo(e1.RowKey).Or.EqualTo(e3.RowKey));
                Assert.That(result.Second().RowKey, Is.EqualTo(e1.RowKey).Or.EqualTo(e3.RowKey));
            }

            [Test]
            public async Task WhenEntityDoesNotExist_ThenReturnEmpty()
            {
                var e1 = PersonTestBuilder.InMemory.WithAge("50").Build();
                var e2 = PersonTestBuilder.InMemory.WithAge("40").Build();

                await InsertAsync(e1, e2);

                var result = await _sut.FindByAsync(nameof(e1.Age), "30");

                Assert.That(result, Is.Empty);
            }
        }

        [TestFixture]
        public class FindInAsync : TableRepositoryTests
        {
            [Test]
            public async Task WhenValuesCollectionIsEmpty_ThenReturnEmpty()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1);

                var result = await _sut.FindInAsync("Age", Enumerable.Empty<string>());

                Assert.That(result, Is.Empty);
            }

            [Test]
            public async Task WhenEntitiesMatch_ThenReturnEntities()
            {
                var e1 = PersonTestBuilder.InMemory.WithAge("50").Build();
                var e2 = PersonTestBuilder.InMemory.WithAge("40").Build();
                var e3 = PersonTestBuilder.InMemory.WithAge("30").Build();

                await InsertAsync(e1, e2, e3);

                var result = await _sut.FindInAsync(nameof(e1.Age), new[] {"50", "40"});

                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result.First().RowKey, Is.EqualTo(e1.RowKey).Or.EqualTo(e2.RowKey));
                Assert.That(result.Second().RowKey, Is.EqualTo(e1.RowKey).Or.EqualTo(e2.RowKey));
            }
        }

        [TestFixture]
        public class QueryAsync : TableRepositoryTests
        {
            [Test]
            public async Task WhenMatchesOne_ThenReturnEntity()
            {
                var e1 = PersonTestBuilder.InMemory.WithAge("50").Build();
                var e2 = PersonTestBuilder.InMemory.WithAge("40").Build();
                var e3 = PersonTestBuilder.InMemory.WithAge("30").Build();

                await InsertAsync(e1, e2, e3);
                
                var filter = Filter
                    .When("Age", QueryComparison.GreaterThanOrEqual, "50")
                    .And()
                    .When("Name", QueryComparison.Equal, "John")
                    .Build();

                var result = await _sut.QueryAsync(filter);

                Assert.That(result.Count, Is.EqualTo(1));
            }

            [Test]
            public async Task WhenMatchesThree_ThenReturnEntities()
            {
                var e1 = PersonTestBuilder.InMemory.WithAge("50").Build();
                var e2 = PersonTestBuilder.InMemory.WithAge("40").Build();
                var e3 = PersonTestBuilder.InMemory.WithAge("30").Build();

                await InsertAsync(e1, e2, e3);

                var filter = Filter
                    .When("Age", QueryComparison.GreaterThanOrEqual, "50")
                    .Or()
                    .When("Name", QueryComparison.Equal, "John")
                    .Build();

                var result = await _sut.QueryAsync(filter);

                Assert.That(result.Count, Is.EqualTo(3));
            }
        }

        #endregion

        #region Insert

        [TestFixture]
        public class InsertAsync_Single: TableRepositoryTests
        {
            [Test]
            public async Task WhenEntityNotNull_ThenInsert()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                var origETag = e1.ETag;

                Track(e1);

                await _sut.InsertAsync(e1);

                Assert.That(e1.ETag, Is.Not.EqualTo(origETag));
            }

            [Test]
            public async Task WhenEntityExists_ThenThrowException()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1);

                e1.Name = "Something else";

                Assert.ThrowsAsync<StorageException>(() => _ = _sut.InsertAsync(e1));
            }
        }

        [TestFixture]
        public class InsertAsync_Multiple : TableRepositoryTests
        {
            [Test]
            public async Task WhenEntityNotNull_ThenInsert()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                var e2 = PersonTestBuilder.InMemory.Build();
                var e3 = PersonTestBuilder.InMemory.Build();
                var e4 = PersonTestBuilder.InMemory.Build();
                var e5 = PersonTestBuilder.InMemory.Build();

                Track(e1, e2, e3, e4, e5);

                await _sut.InsertAsync(new [] { e1, e2, e3, e4, e5 });

                var entities = await _sut.GetAllAsync();

                Assert.That(entities.Count, Is.EqualTo(5));
            }

            [Test]
            public async Task WhenSomeEntitiesExist_ThenThrowException()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                var e2 = PersonTestBuilder.InMemory.Build();
                var e3 = PersonTestBuilder.InMemory.Build();

                Track(e1);

                await InsertAsync(e2, e3);

                Assert.ThrowsAsync<StorageException>(() => _ = _sut.InsertAsync(new [] { e1, e2, e3 }));
            }
        }

        [TestFixture]
        public class InsertOrReplaceAsync_Single : TableRepositoryTests
        {
            [Test]
            public async Task WhenEntityDoesNotExist_ThenInsert()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                var origETag = e1.ETag;

                Track(e1);

                await _sut.InsertOrReplaceAsync(e1);

                Assert.That(e1.ETag, Is.Not.EqualTo(origETag));
            }

            [Test]
            public async Task WhenEntityExists_ThenReplace()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                await InsertAsync(e1);

                e1.Name = "Peter";

                await _sut.InsertOrReplaceAsync(e1);

                var entity = await _sut.GetByKeysAsync(e1);

                Assert.That(entity.Name, Is.EqualTo("Peter"));
            }
        }

        [TestFixture]
        public class InsertOrReplaceAsync_Multiple : TableRepositoryTests
        {
            [Test]
            public async Task WhenEntitiesExistAndDontExist_ThenReplaceOrInsert()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                var e2 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1);
                Track(e2);

                e1.Name = "Peter";
                e2.Name = "Peter";

                await _sut.InsertOrReplaceAsync(new[] { e1, e2 });

                var entities = await _sut.GetAllAsync();

                Assert.That(entities.Count, Is.EqualTo(2));
                Assert.That(entities.First().Name, Is.EqualTo("Peter"));
                Assert.That(entities.Second().Name, Is.EqualTo("Peter"));
            }
        }

        [TestFixture]
        public class InsertOrMergeAsync_Single : TableRepositoryTests
        {
            [Test]
            public async Task WhenEntityDoesNotExist_ThenInsert()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                var origETag = e1.ETag;

                Track(e1);

                await _sut.InsertOrMergeAsync(e1);

                Assert.That(e1.ETag, Is.Not.EqualTo(origETag));
            }

            [Test]
            public async Task WhenEntityExists_ThenReplace()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                await InsertAsync(e1);

                e1.Name = "Peter";

                await _sut.InsertOrMergeAsync(e1);

                var entity = await _sut.GetByKeysAsync(e1);

                Assert.That(entity.Name, Is.EqualTo("Peter"));
            }
        }

        [TestFixture]
        public class InsertOrMergeAsync_Multiple : TableRepositoryTests
        {
            [Test]
            public async Task WhenEntitiesExistAndDontExist_ThenReplaceOrInsert()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                var e2 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1);
                Track(e2);

                e1.Name = "Peter";
                e2.Name = "Peter";

                await _sut.InsertOrMergeAsync(new[] { e1, e2 });

                var entities = await _sut.GetAllAsync();

                Assert.That(entities.Count, Is.EqualTo(2));
                Assert.That(entities.First().Name, Is.EqualTo("Peter"));
                Assert.That(entities.Second().Name, Is.EqualTo("Peter"));
            }
        }

        #endregion

        #region Replace

        [TestFixture]
        public class ReplaceAsync_Single : TableRepositoryTests
        {
            [Test]
            public void WhenEntityDoesNotExist_ThenThrowException()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                e1.WildCardETag();

                var ex = Assert.ThrowsAsync<StorageException>(() => _sut.ReplaceAsync(e1));
                Assert.That(ex.IsNotFound(), Is.True);
            }

            [Test]
            public async Task WhenEntityExists_ThenReplace()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                await InsertAsync(e1);

                e1.Name = "Peter";

                await _sut.ReplaceAsync(e1);

                var entity = await _sut.GetByKeysAsync(e1);

                Assert.That(entity.Name, Is.EqualTo("Peter"));
            }
        }

        [TestFixture]
        public class ReplaceAsync_Multiple : TableRepositoryTests
        {
            [Test]
            public async Task WhenEntitiesExist_ThenReplace()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                var e2 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1);
                await InsertAsync(e2);

                e1.Name = "Peter";
                e2.Name = "Peter";

                await _sut.ReplaceAsync(new[] { e1, e2 });

                var entities = await _sut.GetAllAsync();

                Assert.That(entities.Count, Is.EqualTo(2));
                Assert.That(entities.First().Name, Is.EqualTo("Peter"));
                Assert.That(entities.Second().Name, Is.EqualTo("Peter"));
            }
        }

        [TestFixture]
        public class ReplaceIfExistsAsync_Single : TableRepositoryTests
        {
            [Test]
            public void WhenEntityDoesNotExist_ThenDoNothing()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                e1.WildCardETag();

                Assert.DoesNotThrowAsync(() => _sut.ReplaceIfExistsAsync(e1));
            }

            [Test]
            public async Task WhenEntityExists_ThenReplace()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                await InsertAsync(e1);

                e1.Name = "Peter";

                await _sut.ReplaceIfExistsAsync(e1);

                var entity = await _sut.GetByKeysAsync(e1);

                Assert.That(entity.Name, Is.EqualTo("Peter"));
            }
        }

        [TestFixture]
        public class ReplaceIfExistsAsync_Multiple : TableRepositoryTests
        {
            [Test]
            public async Task WhenEntityExists_ThenReplace()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                var e2 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1);
                Track(e2);

                e2.WildCardETag();

                e1.Name = "Peter";

                await _sut.ReplaceIfExistsAsync(new [] {e1, e2});

                var entity = await _sut.GetByKeysAsync(e1);

                Assert.That(entity.Name, Is.EqualTo("Peter"));
            }
        }

        #endregion

        #region Merge

        [TestFixture]
        public class MergeAsync_Single : TableRepositoryTests
        {
            [Test]
            public void WhenEntityDoesNotExist_ThenThrowException()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                e1.WildCardETag();

                var ex = Assert.ThrowsAsync<StorageException>(() => _sut.MergeAsync(e1));
                Assert.That(ex.IsNotFound(), Is.True);
            }

            [Test]
            public async Task WhenEntityExists_ThenMerge()
            {
                var e1 = PersonTestBuilder.InMemory.WithName("John").Build();

                await InsertAsync(e1);

                e1.Age = "99";

                await _sut.MergeAsync(e1);

                var entity = await _sut.GetByKeysAsync(e1);

                Assert.That(entity.Name, Is.EqualTo("John"));
                Assert.That(entity.Age, Is.EqualTo("99"));
            }
        }

        [TestFixture]
        public class MergeAsync_Multiple : TableRepositoryTests
        {
            [Test]
            public async Task WhenEntitiesExist_ThenReplace()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                var e2 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1);
                await InsertAsync(e2);

                e1.Name = "Peter";
                e2.Name = "Peter";

                await _sut.MergeAsync(new[] { e1, e2 });

                var entities = await _sut.GetAllAsync();

                Assert.That(entities.Count, Is.EqualTo(2));
                Assert.That(entities.First().Name, Is.EqualTo("Peter"));
                Assert.That(entities.Second().Name, Is.EqualTo("Peter"));
            }
        }

        [TestFixture]
        public class MergeIfExistsAsync_Single : TableRepositoryTests
        {
            [Test]
            public void WhenEntityDoesNotExist_ThenDoNothing()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                e1.WildCardETag();

                Assert.DoesNotThrowAsync(() => _sut.MergeIfExistsAsync(e1));
            }

            [Test]
            public async Task WhenEntityExists_ThenMerge()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                await InsertAsync(e1);

                e1.Name = "Peter";

                await _sut.MergeIfExistsAsync(e1);

                var entity = await _sut.GetByKeysAsync(e1);

                Assert.That(entity.Name, Is.EqualTo("Peter"));
            }
        }

        [TestFixture]
        public class MergeIfExistsAsync_Multiple : TableRepositoryTests
        {
            [Test]
            public async Task WhenEntityExists_ThenReplace()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                var e2 = PersonTestBuilder.InMemory.Build();
                
                await InsertAsync(e1);
                Track(e2);

                e1.Name = "Peter";
                e2.WildCardETag();
                
                await _sut.MergeIfExistsAsync(new [] { e1, e2 });

                var entity = await _sut.GetByKeysAsync(e1);

                Assert.That(entity.Name, Is.EqualTo("Peter"));
            }
        }

        #endregion

        #region Delete

        [TestFixture]
        public class DeleteAllAsync : TableRepositoryTests
        {
            [Test]
            public async Task WhenTableIsEmpty_ThenDoNothing()
            {
                await _sut.DeleteAllAsync();

                var result = await _sut.GetAllAsync();

                Assert.That(result, Is.Empty);
            }

            [Test]
            public async Task WhenTableIsNotEmpty_ThenDeleteAll()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                var e2 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1, e2);

                await _sut.DeleteAllAsync();

                var result = await _sut.GetAllAsync();

                Assert.That(result, Is.Empty);
            }

            [Test]
            public async Task WhenTableHasDifferentTypes_ThenDeleteAll()
            {
                var personDecimalRepository = TableRepositoryFactory.Create<PersonWithDecimalEntity>();
                var personEnumRepository = TableRepositoryFactory.Create<PersonWithEnumEntity>();

                var pDec = PersonWithDecimalTestBuilder.InMemory.Build();
                var pEnum = PersonWithEnumTestBuilder.InMemory.Build();

                await personDecimalRepository.InsertAsync(pDec);
                await personEnumRepository.InsertAsync(pEnum);

                await _sut.DeleteAllAsync();

                var count = await _sut.GetCountAsync();

                Assert.That(count, Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class DeleteAsync_Entity : TableRepositoryTests
        {
            [Test]
            public void WhenEntityDoesNotExist_ThenThrowException()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                e1.WildCardETag();

                var ex = Assert.ThrowsAsync<StorageException>(() => _sut.DeleteAsync(e1));
                Assert.That(ex.Message, Is.EqualTo("Not Found"));
            }

            [Test]
            public async Task WhenEntityExists_ThenDelete()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1);

                await _sut.DeleteAsync(e1);

                var entity = await _sut.GetByKeysAsync(e1.PartitionKey, e1.RowKey);

                Assert.That(entity, Is.Null);
            }
        }

        [TestFixture]
        public class DeleteIfExistsAsync_Keys : TableRepositoryTests
        {
            [Test]
            public void WhenEntityDoesNotExist_ThenDoesNothing()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                Assert.DoesNotThrowAsync(() => _sut.DeleteIfExistsAsync(e1.PartitionKey, e1.RowKey));
            }

            [Test]
            public async Task WhenEntityExists_ThenDelete()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1);

                await _sut.DeleteIfExistsAsync(e1.PartitionKey, e1.RowKey);

                var entity = await _sut.GetByKeysAsync(e1.PartitionKey, e1.RowKey);

                Assert.That(entity, Is.Null);
            }
        }

        [TestFixture]
        public class DeleteIfExistsAsync_Entity_Single : TableRepositoryTests
        {
            [Test]
            public void WhenEntityDoesNotExist_ThenDoesNothing()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                e1.WildCardETag();

                Assert.DoesNotThrowAsync(() => _sut.DeleteIfExistsAsync(e1));
            }

            [Test]
            public async Task WhenEntityExists_ThenDelete()
            {
                var e1 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1);

                await _sut.DeleteIfExistsAsync(e1);

                var entity = await _sut.GetByKeysAsync(e1.PartitionKey, e1.RowKey);

                Assert.That(entity, Is.Null);
            }
        }

        [TestFixture]
        public class DeleteIfExistsAsync_Entity_Multiple : TableRepositoryTests
        {
            [Test]
            public async Task WhenEntitiesExist_ThenDelete()
            {
                var e1 = PersonTestBuilder.InMemory.Build();
                var e2 = PersonTestBuilder.InMemory.Build();

                await InsertAsync(e1);

                e2.WildCardETag();

                await _sut.DeleteIfExistsAsync(new [] { e1, e2 });

                var entity = await _sut.GetByKeysAsync(e1.PartitionKey, e1.RowKey);

                Assert.That(entity, Is.Null);
            }
        }

        #endregion
    }
}