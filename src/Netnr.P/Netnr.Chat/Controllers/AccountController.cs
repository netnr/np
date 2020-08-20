using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Netnr.Chat.Application.ViewModel;

namespace Netnr.Chat.Controllers
{
    /// <summary>
    /// 账号
    /// </summary>
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly Data.ContextBase db;

        public AccountController(Data.ContextBase _db)
        {
            db = _db;
        }

        /// <summary>
        /// 授权获取token
        /// </summary>
        /// <param name="chatLogin">登录信息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResultVM Token([FromBody] ChatLoginVM chatLogin)
        {
            var vm = new ActionResultVM();

            try
            {
                Domain.NChatUser uo = null;

                //有账号、密码
                if (!string.IsNullOrWhiteSpace(chatLogin.UserName) && !string.IsNullOrWhiteSpace(chatLogin.Password))
                {
                    var pw = Core.CalcTo.MD5(chatLogin.Password);

                    uo = db.NChatUser.FirstOrDefault(x => x.CuUserName == chatLogin.UserName && x.CuPassword == pw);
                    if (uo == null)
                    {
                        vm.Set(ARTag.unauthorized);
                        vm.Msg = "账号或密码错误";
                        return vm;
                    }
                }
                //启用来宾用户
                else if (GlobalTo.GetValue<bool>("NetnrChat:EnableGuestUsers"))
                {
                    //新增
                    if (string.IsNullOrWhiteSpace(chatLogin.GuestId))
                    {
                        var uid = Core.UniqueTo.LongId();

                        uo = new Domain.NChatUser
                        {
                            CuUserId = "G_" + uid,
                            CuUserName = "Guest-" + uid,
                            CuPassword = Core.CalcTo.MD5("Guest"),
                            CuUserNickname = "Guest",
                            CuCreateTime = DateTime.Now,
                            CuStatus = 1                            
                        };
                    }
                    else
                    {

                    }
                }
                else
                {
                    vm.Set(ARTag.invalid);
                    vm.Msg = "账号或密码不能为空";
                    return vm;
                }

                //token带的用户信息
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, uo.CuUserId),
                    new Claim(ClaimTypes.Name, uo.CuUserName),
                    new Claim(ClaimTypes.UserData, new { chatLogin.Device, chatLogin.Sign }.ToJson())
                };

                var expireDate = GlobalTo.GetValue<int>("TokenManagement:AccessExpiration");

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GlobalTo.GetValue("TokenManagement:Secret")));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var jwtToken = new JwtSecurityToken(GlobalTo.GetValue("TokenManagement:Issuer"), GlobalTo.GetValue("TokenManagement:Audience"), claims, expires: DateTime.Now.AddSeconds(expireDate), signingCredentials: credentials);

                vm.Data = new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expireDate,
                    userId = uo.CuUserId,
                    userName = uo.CuUserName,
                    userPhoto = uo.CuUserPhoto
                };
                vm.Set(ARTag.success);
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
    }
}