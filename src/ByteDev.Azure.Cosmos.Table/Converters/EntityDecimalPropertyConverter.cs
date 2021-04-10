using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Table;

namespace ByteDev.Azure.Cosmos.Table.Converters
{
    /// <summary>
    /// Write and reads decimal properties to/from Table Storage.
    /// </summary>
    public static class EntityDecimalPropertyConverter
    {
        /// <summary>
        /// Write the entity to Table Storage.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <param name="entity">Entity.</param>
        /// <param name="properties">Properties on the entity that are already to be written.</param>
        public static void WriteEntity<TEntity>(TEntity entity, IDictionary<string, EntityProperty> properties)
        {
            var propertyInfos = entity.GetType().GetDecimalProperties();

            foreach (var pi in propertyInfos)
            {
                object value = pi.GetValue(entity);

                if (value != null)
                {
                    properties.Add(pi.Name, new EntityProperty(value.ToString()));
                }
            }
        }

        /// <summary>
        /// Read the entity from Table Storage.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <param name="entity">Entity.</param>
        /// <param name="properties">Properties on the entity that are already to be read.</param>
        public static void ReadEntity<TEntity>(TEntity entity, IDictionary<string, EntityProperty> properties)
        {
            var propertyInfos = entity.GetType().GetDecimalProperties();

            foreach (var pi in propertyInfos)
            {
                var value = properties[pi.Name].PropertyAsObject;

                if (value == null)
                {
                    continue;
                }

                if (decimal.TryParse(value.ToString(), out decimal m))
                {
                    pi.SetValue(entity, m);
                }
            }
        }
    }
}