using System.Collections.Generic;

namespace ByteDev.Azure.Cosmos.Table
{
    internal class PagedResult<T>
    {
        public string PreviousPageToken { get; set; }

        public string CurrentPageToken { get; set; }

        public string NextPageToken { get; set; }

        public IEnumerable<T> Items { get; set; }

        public int? Take { get; set; }
    }
}