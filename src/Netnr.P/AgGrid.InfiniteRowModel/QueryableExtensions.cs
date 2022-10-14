using System;
using System.Linq;

namespace AgGrid.InfiniteRowModel
{
    public static class QueryableExtensions
    {
        public static InfiniteRowModelResult<T> GetInfiniteRowModelBlock<T>(this IQueryable<T> queryable, string getRowsParamsJson, InfiniteRowModelOptions options = null, Action<IQueryable<T>> queryCall = null)
            => GetInfiniteRowModelBlock(queryable, InfiniteScroll.DeserializeGetRowsParams(getRowsParamsJson), options, queryCall);

        public static InfiniteRowModelResult<T> GetInfiniteRowModelBlock<T>(this IQueryable<T> queryable, GetRowsParams getRowsParams, InfiniteRowModelOptions options = null, Action<IQueryable<T>> queryCall = null)
        {
            var query = InfiniteScroll.ToQueryableRows(queryable, getRowsParams, options, out IQueryable<T> queryableCount);
            queryCall?.Invoke(query);

            var rows = query.ToList();
            return InfiniteScroll.ToRowModelResult(getRowsParams, rows, queryableCount);
        }
    }
}
