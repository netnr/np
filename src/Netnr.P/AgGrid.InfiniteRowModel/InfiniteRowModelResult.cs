using System.Collections.Generic;

namespace AgGrid.InfiniteRowModel
{
    public class InfiniteRowModelResult<T>
    {
        public IEnumerable<T> RowsThisBlock { get; set; }
        public int? LastRow { get; set; }
    }
}
