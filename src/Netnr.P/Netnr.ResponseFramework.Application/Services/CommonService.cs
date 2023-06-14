using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Netnr.ResponseFramework.Application.Services
{
    /// <summary>
    /// 公共、常用方法
    /// </summary>
    public class CommonService
    {
        #region 辅助方法

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sorts">排序字段，支持多个，逗号分割</param>
        /// <param name="orders">排序类型，支持多个，逗号分割</param>
        public static IQueryable<T> OrderBy<T>(IQueryable<T> query, string sorts, string orders = "asc")
        {
            var listSort = sorts.Split(',').ToList();
            var listOrder = orders.Split(',').ToList();

            for (int i = 0; i < listSort.Count; i++)
            {
                var sort = listSort[i];
                var order = i < listOrder.Count ? listOrder[i] : "asc";

                var property = typeof(T).GetProperties().Where(x => x.Name.Equals(sort, StringComparison.OrdinalIgnoreCase)).First();

                var parameter = Expression.Parameter(typeof(T), "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var lambda = Expression.Lambda(propertyAccess, parameter);

                var ob = i == 0 ? "OrderBy" : "ThenBy";
                if (order.ToLower() == "desc")
                {
                    ob += "Descending";
                    MethodCallExpression resultExp = Expression.Call(typeof(Queryable), ob, new Type[] { typeof(T), property.PropertyType }, query.Expression, Expression.Quote(lambda));
                    query = query.Provider.CreateQuery<T>(resultExp);
                }
                else
                {
                    MethodCallExpression resultExp = Expression.Call(typeof(Queryable), ob, new Type[] { typeof(T), property.PropertyType }, query.Expression, Expression.Quote(lambda));
                    query = query.Provider.CreateQuery<T>(resultExp);
                }
            }

            return query;
        }

        /// <summary>
        /// 查询拼接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="ivm"></param>
        /// <param name="db"></param>
        public static async Task<QueryDataOutputVM> QueryJoin<T>(IQueryable<T> query, QueryDataInputVM ivm, ContextBase db)
        {
            var ovm = new QueryDataOutputVM();

            //条件
            query = QueryWhere(query, ivm);

            //总条数
            ovm.Total = await query.CountAsync();

            //排序
            if (!string.IsNullOrWhiteSpace(ivm.Sort))
            {
                query = OrderBy(query, ivm.Sort, ivm.Order);
            }

            //分页
            if (ivm.Pagination == 1)
            {
                query = query.Skip((Math.Max(ivm.Page, 1) - 1) * ivm.Rows).Take(ivm.Rows);
            }

            //查询SQL
            ovm.QuerySql = query.ToQueryString();

            //数据
            var data = await query.ToListAsync();
            ovm.Data = data;
            //导出时，存储数据表格
            if (ivm.HandleType == "export")
            {
                ovm.Table = ToDataTableForString(data);
            }

            //列
            if (ivm.ColumnsExists != 1)
            {
                ovm.Columns = await db.SysTableConfig.Where(x => x.TableName == ivm.TableName).OrderBy(x => x.ColOrder).ToListAsync();
            }

            return ovm;
        }

        /// <summary>
        /// 查询条件（IQueryable）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="ivm"></param>
        /// <returns></returns>
        public static IQueryable<T> QueryWhere<T>(IQueryable<T> query, QueryDataInputVM ivm)
        {
            //条件
            if (!string.IsNullOrWhiteSpace(ivm.Wheres))
            {
                var whereFinal = PredicateTo.True<T>();

                var whereItems = ivm.Wheres.DeJson().EnumerateArray();
                foreach (var item in whereItems)
                {
                    //字段
                    var field = item.GetValue("field");
                    //关系符
                    var relation = item.GetValue("relation");
                    //值
                    var value = item.GetProperty("value");

                    switch (relation)
                    {
                        case "Equal":
                        case "NotEqual":
                        case "LessThan":
                        case "GreaterThan":
                        case "LessThanOrEqual":
                        case "GreaterThanOrEqual":
                            {
                                var whereItem = PredicateTo.Compare<T>(field, relation, value);
                                whereFinal = whereFinal.And(whereItem);
                            }
                            break;
                        case "Contains":
                        case "StartsWith":
                        case "EndsWith":
                            {
                                var whereItem = PredicateTo.Contains<T>(field, relation, value.ToString());
                                whereFinal = whereFinal.And(whereItem);
                            }
                            break;
                        case "BetweenAnd":
                            {
                                var arrVal = value.EnumerateArray();
                                if (arrVal.Count() == 2)
                                {
                                    var beginVal = arrVal.First().ToString();
                                    var endVal = arrVal.Last().ToString();

                                    var whereBegin = PredicateTo.Compare<T>(field, ">=", beginVal);
                                    var whereEnd = PredicateTo.Compare<T>(field, "<=", endVal);
                                    var whereItem = whereBegin.And(whereEnd);
                                    whereFinal = whereFinal.And(whereItem);
                                }
                            }
                            break;
                    }
                }

                query = query.Where(whereFinal);
            }
            return query;
        }

        /// <summary>
        /// 实体转表，类型为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTableForString<T>(List<T> list)
        {
            Type elementType = typeof(T);
            var t = new DataTable();
            elementType.GetProperties().ToList().ForEach(propInfo => t.Columns.Add(propInfo.Name, typeof(string)));
            foreach (T item in list)
            {
                var row = t.NewRow();
                elementType.GetProperties().ToList().ForEach(propInfo => row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value);
                t.Rows.Add(row);
            }
            return t;
        }

        #endregion

        #region 全局缓存

        /// <summary>
        /// 全局缓存KEY
        /// </summary>
        public class GlobalCacheKey
        {
            /// <summary>
            /// 菜单缓存KEY
            /// </summary>
            public const string SysMenu = "SysMenu";

            /// <summary>
            /// 角色缓存KEY
            /// </summary>
            public const string SysRole = "SysRole";

            /// <summary>
            /// 按钮缓存KEY
            /// </summary>
            public const string SysButton = "SysButton";
        }

        #endregion

        #region 查询

        /// <summary>
        /// 查询菜单
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cacheFirst"></param>
        /// <returns></returns>
        public static List<SysMenu> QuerySysMenuList(Func<SysMenu, bool> predicate, bool cacheFirst = true)
        {
            var list = CacheTo.Get<List<SysMenu>>(GlobalCacheKey.SysMenu);
            if (cacheFirst == false || list == null)
            {
                using var db = ContextBaseFactory.CreateDbContext();
                list = db.SysMenu.OrderBy(x => x.SmOrder).ToList();
                CacheTo.Set(GlobalCacheKey.SysMenu, list, 300, false);
            }
            list = list.Where(predicate).ToList();
            return list;
        }

        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<SysButton> QuerySysButtonList(Func<SysButton, bool> predicate)
        {
            var list = CacheTo.Get<List<SysButton>>(GlobalCacheKey.SysButton);
            if (list == null)
            {
                using var db = ContextBaseFactory.CreateDbContext();
                list = db.SysButton.OrderBy(x => x.SbBtnOrder).ToList();
                CacheTo.Set(GlobalCacheKey.SysButton, list, 300, false);
            }
            list = list.Where(predicate).ToList();
            return list;
        }

        /// <summary>
        /// 查询角色信息
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static SysRole QuerySysRoleEntity(Func<SysRole, bool> predicate)
        {
            var list = CacheTo.Get<List<SysRole>>(GlobalCacheKey.SysRole);
            if (list == null)
            {
                using var db = ContextBaseFactory.CreateDbContext();
                list = db.SysRole.ToList();
                CacheTo.Set(GlobalCacheKey.SysRole, list, 300, false);
            }
            var mo = list.FirstOrDefault(predicate);
            return mo;
        }

        /// <summary>
        /// 查询配置的菜单是否有权限访问（仅针对配置的菜单）
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="url">链接</param>
        /// <returns></returns>
        public static bool QueryMenuIsAuth(string roleId, string url)
        {
            var ia = false;

            //是配置的菜单
            var menuMo = QuerySysMenuList(x => url.Equals(x.SmUrl, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (menuMo != null)
            {
                //检测该角色是否勾选菜单
                ia = QuerySysRoleEntity(x => x.SrId == roleId)?.SrMenus.Contains(menuMo.SmId) ?? false;
            }
            else
            {
                ia = true;
            }

            return ia;
        }

        #endregion
    }
}