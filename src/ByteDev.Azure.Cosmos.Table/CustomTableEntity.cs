using System.Collections.Generic;
using ByteDev.Azure.Cosmos.Table.Converters;
using Microsoft.Azure.Cosmos.Table;

namespace ByteDev.Azure.Cosmos.Table
{
    /// <summary>
    /// Table entity that when inherited from enables decimal
    /// and enum (stored as int) support in the child class.
    /// </summary>
    public abstract class CustomTableEntity : TableEntity
    {
        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var properties = base.WriteEntity(operationContext);

            EntityDecimalPropertyConverter.WriteEntity(this, properties);
            EntityEnumPropertyConverter.WriteEntity(this, properties);

            return properties;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            EntityDecimalPropertyConverter.ReadEntity(this, properties);
            EntityEnumPropertyConverter.ReadEntity(this, properties);
        }
    }
}