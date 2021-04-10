using System.Threading.Tasks;
using ByteDev.Azure.Cosmos.Table.IntTests.Entities;
using NUnit.Framework;

namespace ByteDev.Azure.Cosmos.Table.IntTests
{
    [TestFixture]
    public class CustomTableEntityTests
    {
        [TestFixture]
        public class DecimalPropertyTests : CustomTableEntityTests
        {
            private ITableRepository<PersonWithDecimalEntity> _sut;

            [SetUp]
            public async Task SetUp()
            {
                _sut = TableRepositoryFactory.Create<PersonWithDecimalEntity>();

                await _sut.DeleteAllAsync();
            }

            [Test]
            public async Task WhenEntityHasDecimal_ThenSaveAndRetrieve()
            {
                var entity = PersonWithDecimalTestBuilder.InMemory.Build();

                await _sut.InsertAsync(entity);

                var result = await _sut.GetByKeysAsync(entity);

                Assert.That(result.Money, Is.EqualTo(entity.Money));
            }
        }

        [TestFixture]
        public class EnumPropertyTests : CustomTableEntityTests
        {
            private ITableRepository<PersonWithEnumEntity> _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = TableRepositoryFactory.Create<PersonWithEnumEntity>();
            }

            [OneTimeTearDown]
            public async Task ClassTearDown()
            {
                await _sut.DeleteAllAsync();
            }

            [Test]
            public async Task WhenEntityHasEnums_ThenSaveAndRetrieve()
            {
                var entity = PersonWithEnumTestBuilder.InMemory.Build();

                await _sut.InsertAsync(entity);

                var result = await _sut.GetByKeysAsync(entity);

                Assert.That(result.ResidentCountry, Is.EqualTo(entity.ResidentCountry));
                Assert.That(result.MovingCountry, Is.EqualTo(entity.MovingCountry));
            }
        }
    }
}