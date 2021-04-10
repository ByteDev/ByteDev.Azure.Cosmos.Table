using System;
using ByteDev.Common.Creation;

namespace ByteDev.Azure.Cosmos.Table.IntTests.Entities
{
    public class PersonWithEnumTestBuilder : Builder<PersonWithEnumTestBuilder, PersonWithEnumEntity>
    {
        public static PersonWithEnumTestBuilder InMemory => new PersonWithEnumTestBuilder();

        public PersonWithEnumTestBuilder()
        {
            WithResidentCountry(Country.Uk);
            WithMovingCountry(Country.France);
        }

        public PersonWithEnumTestBuilder WithResidentCountry(Country country)
        {
            return With(entity => entity.ResidentCountry = country);
        }

        public PersonWithEnumTestBuilder WithMovingCountry(Country country)
        {
            return With(entity => entity.MovingCountry = country);
        }

        protected override PersonWithEnumEntity CreateEntity()
        {
            var id = Guid.NewGuid();

            return new PersonWithEnumEntity
            {
                PartitionKey = id.ToString(),
                RowKey = id.ToString(),
            };
        }
    }
}