using System;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Table;

namespace ByteDev.Azure.Cosmos.Table.Converters
{
    /// <summary>
    /// Writes and reads enum properties to/from Table Storage as integers.
    /// </summary>
    public static class EntityEnumPropertyConverter
    {
        /// <summary>
        /// Write the entity to Table Storage.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <param name="entity">Entity.</param>
        /// <param name="properties">Properties on the entity that are already to be written.</param>
        public static void WriteEntity<TEntity>(TEntity entity, IDictionary<string, EntityProperty> properties)
        {
            var propertyInfos = entity.GetType().GetEnumProperties();

            foreach (var pi in propertyInfos)
            {
                object value = pi.GetValue(entity);

                var enumValue = Convert.ChangeType(value, pi.PropertyType);

                properties.Add(pi.Name, new EntityProperty((int)enumValue));
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
            var propertyInfos = entity.GetType().GetEnumProperties();

            foreach (var pi in propertyInfos)
            {
                var value = properties[pi.Name].PropertyAsObject;

                if (value == null)
                {
                    continue;
                }

                if (int.TryParse(value.ToString(), out int i))
                {
                    pi.SetValue(entity, i);
                }
            }
        }
    }
}