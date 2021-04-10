using System;
using Microsoft.Azure.Cosmos.Table;

namespace ByteDev.Azure.Cosmos.Table
{
    /// <summary>
    /// Extension methods for <see cref="T:Microsoft.Azure.Cosmos.Table.TableEntity" />.
    /// </summary>
    public static class TableEntityExtensions
    {
        /// <summary>
        /// Apply wild card to ETag property on the entity.
        /// </summary>
        /// <param name="source">Table entity.</param>
        /// <returns>Same table entity.</returns>
        public static TableEntity WildCardETag(this TableEntity source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.ETag = "*";
            return source;
        }
    }
}