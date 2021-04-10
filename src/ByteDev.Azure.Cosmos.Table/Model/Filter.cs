using System.Collections.Generic;

namespace ByteDev.Azure.Cosmos.Table.Model
{
    public interface IFilterWhen
    {
        IFilterOperator When(string fieldName, QueryComparison comparison, string fieldValue);
    }

    public interface IFilterOperator
    {
        IFilterWhen And();

        IFilterWhen Or();
        
        Filter Build();
    }

    /// <summary>
    /// Represents a query filter.
    /// </summary>
    public class Filter : IFilterWhen, IFilterOperator
    {
        internal IList<object> Parts { get; } = new List<object>();

        private Filter()
        {
        }

        /// <summary>
        /// Add a statement to the filter.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <param name="comparison">Query comparison type.</param>
        /// <param name="fieldValue">Field value.</param>
        /// <returns>Filter operator interface.</returns>
        public static IFilterOperator When(string fieldName, QueryComparison comparison, string fieldValue) // called 1st
        {
            var filter = new Filter();
            filter.Parts.Add(new Statement(fieldName, comparison, fieldValue));
            return filter;
        }

        /// <summary>
        /// Add a statement to the filter.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <param name="comparison">Query comparison type.</param>
        /// <param name="fieldValue">Field value.</param>
        /// <returns>IFilterOperator interface.</returns>
        IFilterOperator IFilterWhen.When(string fieldName, QueryComparison comparison, string fieldValue) // subsequent calls
        {
            Parts.Add(new Statement(fieldName, comparison, fieldValue));
            return this;
        }

        /// <summary>
        /// Add a AND operator to the filter to join other statements.
        /// </summary>
        /// <returns>IFilterWhen interface.</returns>
        public IFilterWhen And()
        {
            Parts.Add(QueryOperator.And);
            return this;
        }

        /// <summary>
        /// Add a OR operator to the filter to join other statements.
        /// </summary>
        /// <returns>IFilterWhen interface.</returns>
        public IFilterWhen Or()
        {
            Parts.Add(QueryOperator.Or);
            return this;
        }

        /// <summary>
        /// Return the <see cref="T:ByteDev.Azure.Cosmos.Table.Model.Filter" /> instance.
        /// </summary>
        /// <returns>The Filter instance.</returns>
        public Filter Build()
        {
            return this;
        }
    }
}