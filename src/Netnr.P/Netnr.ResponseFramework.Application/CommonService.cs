using Microsoft.AspNetCore.Http;
using Netnr.ResponseFramework.Data;
using Netnr.ResponseFramework.Domain;
using Netnr.ResponseFramework.Application.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Netnr.ResponseFramework.Application
{
    /// <summary>
    /// 公共、常用方法
    /// </summary>
    public class CommonService
    {
        #region 字典  

        private static Dictionary<string, string> _dicSqlRelation;
        /// <summary>
        /// 数据库查询条件关系符
        /// </summary>
        public static Dictionary<string, string> DicSqlRelation
        {
            get
            {
                if (_dicSqlRelation == null)
                {
                    var ts = @"
                                Equal: '{0} = {1}',
                                NotEqual: '{0} != {1}',
                                LessThan: '{0} < {1}',
                                GreaterThan: '{0} > {1}',
                                LessThanOrEqual: '{0} <= {1}',
                                GreaterThanOrEqual: '{0} >= {1}',
                                BetweenAnd: '{0} >= {1} AND {0} <= {2}',
                                Contains: '%{0}%',
                                StartsWith: '{0}%',
                                EndsWith: '%{0}',
                                In: 'IN',
                                NotIn: 'NOT IN'
                              ".Split(',').ToList();
                    var dic = new Dictionary<string, string>();
                    foreach (var item in ts)
                    {
                        var ms = item.Split(':').ToList();
                        dic.Add(ms.FirstOrDefault().Trim(), ms.LastOrDefault().Trim().Replace("'", ""));
                    }
                    _dicSqlRelation = dic;
                }
                return _dicSqlRelation;
            }
            set
            {
                _dicSqlRelation = value;
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 查询拼接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="ivm"></param>
        /// <param name="db"></param>
        /// <param name="ovm"></param>
        public static void QueryJoin<T>(IQueryable<T> query, QueryDataInputVM ivm, ContextBase db, ref QueryDataOutputVM ovm)
        {
            //条件
            query = QueryWhere(query, ivm);

            //总条数
            ovm.Total = query.Count();

            //排序
            if (!string.IsNullOrWhiteSpace(ivm.Sort))
            {
                query = Fast.QueryableTo.OrderBy(query, ivm.Sort, ivm.Order);
            }

            //分页
            if (ivm.Pagination == 1)
            {
                query = query.Skip((Math.Max(ivm.Page, 1) - 1) * ivm.Rows).Take(ivm.Rows);
            }

            //数据
            var data = query.ToList();
            ovm.Data = data;
            //导出时，存储数据表格
            if (ivm.HandleType == "export")
            {
                ovm.Table = ToDataTableForString(data);
            }

            //列
            if (ivm.ColumnsExists != 1)
            {
                ovm.Columns = db.SysTableConfig.Where(x => x.TableName == ivm.TableName).OrderBy(x => x.ColOrder).ToList();
            }
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
                var whereItems = JArray.Parse(ivm.Wheres);
                foreach (var item in whereItems)
                {
                    //关系符
                    var relation = item["relation"].ToStringOrEmpty();
                    string rel = DicSqlRelation[relation];

                    //字段
                    var field = item["field"].ToStringOrEmpty();
                    //值
                    var value = item["value"];

                    //值引号
                    var vqm = "\"";

                    switch (relation)
                    {
                        case "Equal":
                        case "NotEqual":
                        case "LessThan":
                        case "GreaterThan":
                        case "LessThanOrEqual":
                        case "GreaterThanOrEqual":
                            {
                                string val = vqm + value.ToStringOrEmpty() + vqm;
                                query = query.Where(string.Format(rel, field, val));
                            }
                            break;
                        case "Contains":
                        case "StartsWith":
                        case "EndsWith":
                            {
                                query = query.Where(field + "." + relation + "(@0)", value.ToStringOrEmpty());
                            }
                            break;
                        case "BetweenAnd":
                            if (value.Count() == 2)
                            {
                                var v1 = vqm + value[0].ToString() + vqm;
                                var v2 = vqm + value[1].ToString() + vqm;

                                query = query.Where(string.Format(rel, field, v1, v2));
                            }
                            break;
                    }
                }
            }
            return query;
        }

        /// <summary>
        /// 查询条件（IEnumerable,仅支持部分）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="ivm"></param>
        /// <returns></returns>
        public static IEnumerable<T> QueryWhere<T>(IEnumerable<T> query, QueryDataInputVM ivm)
        {
            //条件
            if (!string.IsNullOrWhiteSpace(ivm.Wheres))
            {
                var whereItems = JArray.Parse(ivm.Wheres);
                foreach (var item in whereItems)
                {
                    //关系符
                    var relation = item["relation"].ToStringOrEmpty();
                    string rel = DicSqlRelation[relation];

                    //字段
                    var field = item["field"].ToStringOrEmpty();
                    //值
                    var value = item["value"].ToString().ToLower();

                    switch (relation)
                    {
                        case "Equal":
                            query = query.Where(x => x.GetType().GetProperty(field).GetValue(x, null).ToString().ToLower() == value);
                            break;
                        case "NotEqual":
                            query = query.Where(x => x.GetType().GetProperty(field).GetValue(x, null).ToString().ToLower() != value);
                            break;
                        case "Contains":
                            query = query.Where(x => x.GetType().GetProperty(field).GetValue(x, null).ToString().ToLower().Contains(value));
                            break;
                        case "StartsWith":
                            query = query.Where(x => x.GetType().GetProperty(field).GetValue(x, null).ToString().ToLower().StartsWith(value));
                            break;
                        case "EndsWith":
                            query = query.Where(x => x.GetType().GetProperty(field).GetValue(x, null).ToString().ToLower().EndsWith(value));
                            break;
                    }
                }
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

        #region 获取登录用户信息

        /// <summary>
        /// 获取登录用户信息
        /// </summary>
        /// <returns></returns>
        public static LoginUserVM GetLoginUserInfo(HttpContext context)
        {
            var user = context.User;

            if (user.Identity.IsAuthenticated)
            {
                return new LoginUserVM
                {
                    UserId = context.User.FindFirst(ClaimTypes.PrimarySid)?.Value,
                    UserName = context.User.FindFirst(ClaimTypes.Name)?.Value,
                    Nickname = context.User.FindFirst(ClaimTypes.GivenName)?.Value,
                    RoleId = context.User.FindFirst(ClaimTypes.Role)?.Value
                };
            }
            else
            {
                var token = context.Request.Headers["Authorization"].ToString();
                var mo = TokenValid(token);
                if (mo == null)
                {
                    mo = new LoginUserVM();
                }
                return mo;
            }
        }

        /// <summary>
        /// 获取登录用户角色信息
        /// </summary>
        /// <param name="context"></param>
        public static SysRole LoginUserRoleInfo(HttpContext context)
        {
            var lui = GetLoginUserInfo(context);
            if (!string.IsNullOrWhiteSpace(lui.RoleId))
            {
                return QuerySysRoleEntity(x => x.SrId == lui.RoleId);
            }
            return null;
        }

        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="mo">授权用户信息</param>
        /// <returns></returns>
        public static string TokenMake(LoginUserVM mo)
        {
            var key = GlobalTo.GetValue("VerifyCode:Key");

            var token = Core.CalcTo.EnDES(new
            {
                mo,
                expired = DateTime.Now.AddDays(10).ToTimestamp()
            }.ToJson(), key);

            return token;
        }

        /// <summary>
        /// 验证Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static LoginUserVM TokenValid(string token)
        {
            LoginUserVM mo = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    var key = GlobalTo.GetValue("VerifyCode:Key");

                    var jo = Core.CalcTo.DeDES(token, key).ToJObject();

                    if (DateTime.Now.ToTimestamp() < long.Parse(jo["expired"].ToString()))
                    {
                        mo = jo["mo"].ToString().ToEntity<LoginUserVM>();
                    }
                }
            }
            catch (Exception)
            {

            }

            return mo;
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
            public const string SysMenu = "GlobalSysMenu";

            /// <summary>
            /// 角色缓存KEY
            /// </summary>
            public const string SysRole = "GlobalSysRole";

            /// <summary>
            /// 按钮缓存KEY
            /// </summary>
            public const string SysButton = "GlobalSysButton";
        }

        /// <summary>
        /// 清空全局缓存
        /// </summary>
        public static void GlobalCacheRmove()
        {
            Core.CacheTo.Remove(GlobalCacheKey.SysMenu);
            Core.CacheTo.Remove(GlobalCacheKey.SysRole);
            Core.CacheTo.Remove(GlobalCacheKey.SysButton);
        }

        #endregion

        #region 查询

        /// <summary>
        /// 查询配置信息
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<SysTableConfig> QuerySysTableConfigList(Expression<Func<SysTableConfig, bool>> predicate)
        {
            using var db = ContextBaseFactory.CreateDbContext();
            var list = db.SysTableConfig.Where(predicate).OrderBy(x => x.ColOrder).ToList();
            return list;
        }

        /// <summary>
        /// 查询菜单
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static List<SysMenu> QuerySysMenuList(Func<SysMenu, bool> predicate, bool cache = true)
        {
            if (!cache || !(Core.CacheTo.Get(GlobalCacheKey.SysMenu) is List<SysMenu> list))
            {
                using var db = ContextBaseFactory.CreateDbContext();
                list = db.SysMenu.OrderBy(x => x.SmOrder).ToList();
                Core.CacheTo.Set(GlobalCacheKey.SysMenu, list, 300, false);
            }
            list = list.Where(predicate).ToList();
            return list;
        }

        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static List<SysButton> QuerySysButtonList(Func<SysButton, bool> predicate, bool cache = true)
        {
            if (!cache || !(Core.CacheTo.Get(GlobalCacheKey.SysButton) is List<SysButton> list))
            {
                using var db = ContextBaseFactory.CreateDbContext();
                list = db.SysButton.OrderBy(x => x.SbBtnOrder).ToList();
                Core.CacheTo.Set(GlobalCacheKey.SysButton, list, 300, false);
            }
            list = list.Where(predicate).ToList();
            return list;
        }

        /// <summary>
        /// 查询角色信息
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static SysRole QuerySysRoleEntity(Func<SysRole, bool> predicate, bool cache = true)
        {
            if (!cache || !(Core.CacheTo.Get(GlobalCacheKey.SysRole) is List<SysRole> list))
            {
                using var db = ContextBaseFactory.CreateDbContext();
                list = db.SysRole.ToList();
                Core.CacheTo.Set(GlobalCacheKey.SysRole, list, 300, false);
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
            var menuMo = QuerySysMenuList(x => x.SmUrl?.ToLower() == url.ToLower()).FirstOrDefault();
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