using System;
using ByteDev.Common.Creation;

namespace ByteDev.Azure.Cosmos.Table.IntTests.Entities
{
    public class PersonWithDecimalTestBuilder : Builder<PersonWithDecimalTestBuilder, PersonWithDecimalEntity>
    {
        public static PersonWithDecimalTestBuilder InMemory => new PersonWithDecimalTestBuilder();

        public PersonWithDecimalTestBuilder()
        {
            WithMoney(100.5013M);
        }

        public PersonWithDecimalTestBuilder WithMoney(decimal money)
        {
            return With(entity => entity.Money = money);
        }

        protected override PersonWithDecimalEntity CreateEntity()
        {
            var id = Guid.NewGuid();

            return new PersonWithDecimalEntity
            {
                PartitionKey = id.ToString(),
                RowKey = id.ToString(),
            };
        }
    }
}