using System;
using ByteDev.Common.Creation;

namespace ByteDev.Azure.Cosmos.Table.IntTests.Entities
{
    public class PersonTestBuilder : Builder<PersonTestBuilder, PersonEntity>
    {
        public static PersonTestBuilder InMemory => new PersonTestBuilder();

        public PersonTestBuilder()
        {
            WithName("John");
            WithAge("50");
        }

        public PersonTestBuilder WithName(string name)
        {
            return With(entity => entity.Name = name);
        }

        public PersonTestBuilder WithAge(string age)
        {
            return With(entity => entity.Age = age);
        }

        protected override PersonEntity CreateEntity()
        {
            var id = Guid.NewGuid();

            return new PersonEntity
            {
                PartitionKey = id.ToString(),
                RowKey = id.ToString(),
            };
        }
    }
}