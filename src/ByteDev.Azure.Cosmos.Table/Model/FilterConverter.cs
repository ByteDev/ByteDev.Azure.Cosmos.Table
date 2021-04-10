using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Table;

namespace ByteDev.Azure.Cosmos.Table.Model
{
    public static class FilterConverter
    {
        public static string ToTableQueryFilter(Filter filter)
        {
            if (filter == null)
                return string.Empty;

            string combinedConditions = string.Empty;

            for (var i=0; i < filter.Parts.Count; i++)
            {
                if (combinedConditions == string.Empty)
                {
                    var sta = (Statement)filter.Parts[i];

                    combinedConditions = TableQuery.GenerateFilterCondition(sta.FieldName, QueryComparisonConverter.ToTableString(sta.Comparison), sta.FieldValue);
                    continue;
                }

                if (IsLastElement(filter.Parts, i))
                    continue;

                var op = (QueryOperator)filter.Parts[i];
                var s2 = (Statement)filter.Parts[i+1];

                var condition = TableQuery.GenerateFilterCondition(s2.FieldName, QueryComparisonConverter.ToTableString(s2.Comparison), s2.FieldValue);

                combinedConditions = TableQuery.CombineFilters(combinedConditions, QueryOperatorConverter.ToTableString(op), condition);
                i++;
            }

            return combinedConditions;
        }

        private static bool IsLastElement<T>(ICollection<T> list, int i)
        {
            return i == list.Count - 1;
        }
    }
}