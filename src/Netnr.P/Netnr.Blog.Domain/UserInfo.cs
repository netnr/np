using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(UserName), Name = "UserInfo_UserName", IsUnique = true)]
    public partial class UserInfo
    {
        [Key]
        public int UserId { get; set; }
        [StringLength(50)]
        public string UserName { get; set; }
        public int? UserNameChange { get; set; }
        [StringLength(50)]
        public string UserPwd { get; set; }
        [StringLength(50)]
        public string OpenId1 { get; set; }
        [StringLength(50)]
        public string OpenId2 { get; set; }
        [StringLength(50)]
        public string OpenId3 { get; set; }
        [StringLength(50)]
        public string OpenId4 { get; set; }
        [StringLength(50)]
        public string OpenId5 { get; set; }
        [StringLength(50)]
        public string OpenId6 { get; set; }
        [StringLength(50)]
        public string OpenId7 { get; set; }
        [StringLength(50)]
        public string OpenId8 { get; set; }
        [StringLength(50)]
        public string OpenId9 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UserCreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UserLoginTime { get; set; }
        public int? LoginLimit { get; set; }
        [StringLength(30)]
        public string UserSign { get; set; }
        [StringLength(50)]
        public string Nickname { get; set; }
        [StringLength(200)]
        public string UserPhoto { get; set; }
        public int? UserSex { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UserBirthday { get; set; }
        [StringLength(20)]
        public string UserPhone { get; set; }
        [StringLength(50)]
        public string UserMail { get; set; }
        public int? UserMailValid { get; set; }
        [StringLength(100)]
        public string UserUrl { get; set; }
        [StringLength(200)]
        public string UserSay { get; set; }
        [StringLength(50)]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        public string Spare3 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UserAddTime { get; set; }
    }
}
