using System;
using Microsoft.Azure.Cosmos.Table;

namespace ByteDev.Azure.Cosmos.Table.Model
{
    internal static class QueryOperatorConverter
    {
        public static string ToTableString(QueryOperator op)
        {
            switch (op)
            {
                case QueryOperator.And:
                    return TableOperators.And;
                case QueryOperator.Or:
                    return TableOperators.Or;
                default:
                    throw new InvalidOperationException($"Unhandled enum value: {op}.");
            }
        }
    }
}