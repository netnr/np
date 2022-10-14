namespace Netnr.Admin.Web.Controllers
{
    /// <summary>
    /// 主
    /// </summary>
    public class MainController : Controller
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="content"></param>
        internal void LogContent(string content) => HttpContext.Items.Add("LogContent", content);

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="content"></param>
        internal void LogSql(string content) => HttpContext.Items.Add("LogSql", content);

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="paramsJson"></param>
        /// <returns></returns>
        internal ResultVM GetQuery<T>(IQueryable<T> query, string paramsJson = null) => ResultVM.Try(vm =>
        {
            //默认分页
            paramsJson ??= "{\"startRow\":0,\"endRow\":30}";

            vm.Data = query.GetInfiniteRowModelBlock(paramsJson, queryCall: query => LogSql(query.ToQueryString()));

            vm.Set(EnumTo.RTag.success);

            return vm;
        });
    }
}
