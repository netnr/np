using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Netnr.Blog.Domain
{
    [Index(nameof(UserName), Name = "IDXUserInfo_UserName", IsUnique = true)]
    public partial class UserInfo
    {
        [Key]
        public int UserId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string UserName { get; set; }
        public int? UserNameChange { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string UserPwd { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string OpenId1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string OpenId2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string OpenId3 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string OpenId4 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string OpenId5 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string OpenId6 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string OpenId7 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string OpenId8 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string OpenId9 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UserCreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UserLoginTime { get; set; }
        public int? LoginLimit { get; set; }
        [Column(TypeName = "varchar(30)")]
        public string UserSign { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Nickname { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string UserPhoto { get; set; }
        public int? UserSex { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UserBirthday { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string UserPhone { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string UserMail { get; set; }
        public int? UserMailValid { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string UserUrl { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string UserSay { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare1 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare2 { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Spare3 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UserAddTime { get; set; }
    }
}
