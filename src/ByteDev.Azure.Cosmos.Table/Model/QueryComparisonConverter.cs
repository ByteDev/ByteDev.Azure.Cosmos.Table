using System;
using Microsoft.Azure.Cosmos.Table;

namespace ByteDev.Azure.Cosmos.Table.Model
{
    internal static class QueryComparisonConverter
    {
        public static string ToTableString(QueryComparison qc)
        {
            switch (qc)
            {
                case QueryComparison.Equal:
                    return QueryComparisons.Equal;
                case QueryComparison.NotEqual:
                    return QueryComparisons.NotEqual;
                case QueryComparison.GreaterThan:
                    return QueryComparisons.GreaterThan;
                case QueryComparison.GreaterThanOrEqual:
                    return QueryComparisons.GreaterThanOrEqual;
                case QueryComparison.LessThan:
                    return QueryComparisons.LessThan;
                case QueryComparison.LessThanOrEqual:
                    return QueryComparisons.LessThanOrEqual;
                default:
                    throw new InvalidOperationException($"Unhandled enum value: {qc}.");
            }
        }
    }
}