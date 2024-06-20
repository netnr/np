namespace Netnr.Blog.Web.Controllers
{
    /// <summary>
    /// 主
    /// </summary>
    public class WebController : Controller
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="content"></param>
        internal void LogContent(string content) => HttpContext?.Items.Add("LogContent", content);

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="content"></param>
        internal void LogSql(string content) => HttpContext?.Items.Add("LogSql", content);

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="paramsJson"></param>
        /// <returns></returns>
        internal async Task<ResultVM> GetQuery<T>(IQueryable<T> query, string paramsJson = null)
        {
            var vm = new ResultVM();

            try
            {
                //默认分页
                paramsJson ??= "{\"startRow\":0,\"endRow\":30}";

                vm.Data = await query.GetInfiniteRowModelBlock(paramsJson, queryCall: query =>
                {
                    LogSql(query.ToQueryString());
                    return query.ToListAsync();
                });

                vm.Set(RCodeTypes.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
    }
}
