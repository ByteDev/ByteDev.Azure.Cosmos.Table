using System;
using Microsoft.Azure.Cosmos.Table;

namespace ByteDev.Azure.Cosmos.Table
{
    /// <summary>
    /// Extension methods for <see cref="T:Microsoft.Azure.Cosmos.Table.StorageException" />.
    /// </summary>
    public static class StorageExceptionExtensions
    {
        /// <summary>
        /// Indicates if the exception is for entitiy not found.
        /// </summary>
        /// <param name="source">Exception to check.</param>
        /// <returns>True the exception is entity not found; otherwise false.</returns>
        public static bool IsNotFound(this StorageException source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Message == "Not Found";
        }
    }
}