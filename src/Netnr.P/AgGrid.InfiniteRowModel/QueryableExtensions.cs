using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgGrid.InfiniteRowModel
{
    public static class QueryableExtensions
    {
        public static async Task<InfiniteRowModelResult<T>> GetInfiniteRowModelBlock<T>(this IQueryable<T> queryable, string getRowsParamsJson, InfiniteRowModelOptions options = null, Func<IQueryable<T>, Task<List<T>>> queryCall = null)
        {
            var getRowsParams = InfiniteScroll.DeserializeGetRowsParams(getRowsParamsJson);
            var query = InfiniteScroll.ToQueryableRows(queryable, getRowsParams, options, out IQueryable<T> queryableCount);

            var rows = await queryCall.Invoke(query);

            return InfiniteScroll.ToRowModelResult(getRowsParams, rows, queryableCount);
        }
    }
}
