namespace ByteDev.Azure.Cosmos.Table.Model
{
    /// <summary>
    /// Represents a comparison statement.
    /// </summary>
    public class Statement
    {
        /// <summary>
        /// Field name.
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// Query comparison type.
        /// </summary>
        public QueryComparison Comparison { get; }

        /// <summary>
        /// Field value.
        /// </summary>
        public string FieldValue { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.Cosmos.Table.Model.Statement" /> class.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <param name="comparison">Query comparison type.</param>
        /// <param name="fieldValue">Field value.</param>
        public Statement(string fieldName, QueryComparison comparison, string fieldValue)
        {
            FieldName = fieldName;
            Comparison = comparison;
            FieldValue = fieldValue;
        }
    }
}