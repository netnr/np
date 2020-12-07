using Microsoft.EntityFrameworkCore;
using Netnr.Blog.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace Netnr.Blog.Data
{
    /// <summary>
    /// ContextBase 自动生成（Scaffold-DbContext命令）
    /// </summary>
    public partial class ContextBase : DbContext
    {
        public ContextBase(DbContextOptions<ContextBase> options)
            : base(options)
        {
        }

        public virtual DbSet<DocSet> DocSet { get; set; }
        public virtual DbSet<DocSetDetail> DocSetDetail { get; set; }
        public virtual DbSet<Draw> Draw { get; set; }
        public virtual DbSet<GiftRecord> GiftRecord { get; set; }
        public virtual DbSet<GiftRecordDetail> GiftRecordDetail { get; set; }
        public virtual DbSet<Gist> Gist { get; set; }
        public virtual DbSet<GistSync> GistSync { get; set; }
        public virtual DbSet<GuffRecord> GuffRecord { get; set; }
        public virtual DbSet<KeyValueSynonym> KeyValueSynonym { get; set; }
        public virtual DbSet<KeyValues> KeyValues { get; set; }
        public virtual DbSet<Notepad> Notepad { get; set; }
        public virtual DbSet<OperationRecord> OperationRecord { get; set; }
        public virtual DbSet<Run> Run { get; set; }
        public virtual DbSet<Tags> Tags { get; set; }
        public virtual DbSet<UserConnection> UserConnection { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<UserMessage> UserMessage { get; set; }
        public virtual DbSet<UserReply> UserReply { get; set; }
        public virtual DbSet<UserWriting> UserWriting { get; set; }
        public virtual DbSet<UserWritingTags> UserWritingTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocSet>(entity =>
            {
                entity.HasKey(e => e.DsCode)
                    .HasName("DocSet_DsCode_PK");

                entity.HasComment("文档");

                entity.Property(e => e.DsCode).HasComment("唯一编码");

                entity.Property(e => e.DsCreateTime).HasComment("创建时间");

                entity.Property(e => e.DsName).HasComment("主题");

                entity.Property(e => e.DsOpen).HasComment("公开1");

                entity.Property(e => e.DsRemark).HasComment("备注");

                entity.Property(e => e.DsStatus).HasComment("状态1正常");

                entity.Property(e => e.Spare1)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Uid).HasComment("所属用户");
            });

            modelBuilder.Entity<DocSetDetail>(entity =>
            {
                entity.HasKey(e => e.DsdId)
                    .HasName("PK_DOCSETDETAIL")
                    .IsClustered(false);

                entity.HasComment("文档明细");

                entity.HasIndex(e => e.DsCode, "DocSetDetail_DsCode")
                    .IsClustered();

                entity.Property(e => e.DsCode).HasComment("文档集唯一编码");

                entity.Property(e => e.DsdContent).HasComment("内容");

                entity.Property(e => e.DsdContentHtml).HasComment("内容Html");

                entity.Property(e => e.DsdContentMd).HasComment("内容Markdown");

                entity.Property(e => e.DsdCreateTime).HasComment("创建时间");

                entity.Property(e => e.DsdLetest).HasComment("1是最新");

                entity.Property(e => e.DsdOrder).HasComment("排序");

                entity.Property(e => e.DsdPid).HasComment("父ID");

                entity.Property(e => e.DsdTitle).HasComment("标题");

                entity.Property(e => e.DsdUpdateTime).HasComment("修改时间");

                entity.Property(e => e.Spare1)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Uid).HasComment("所属用户");
            });

            modelBuilder.Entity<Draw>(entity =>
            {
                entity.HasKey(e => e.DrId)
                    .HasName("Draw_DrId_PK");

                entity.HasComment("绘制");

                entity.Property(e => e.DrId).IsUnicode(false);

                entity.Property(e => e.DrCategory).HasComment("类别");

                entity.Property(e => e.DrContent).HasComment("内容");

                entity.Property(e => e.DrCreateTime).HasComment("创建时间");

                entity.Property(e => e.DrName).HasComment("分类：Draw、Mind");

                entity.Property(e => e.DrOpen).HasComment("公开：1公开，2私有");

                entity.Property(e => e.DrOrder).HasComment("排序");

                entity.Property(e => e.DrRemark).HasComment("备注");

                entity.Property(e => e.DrStatus).HasComment("状态：1正常，-1删除");

                entity.Property(e => e.DrType)
                    .IsUnicode(false)
                    .HasComment("名称");

                entity.Property(e => e.Spare1)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Uid).HasComment("创建用户");
            });

            modelBuilder.Entity<GiftRecord>(entity =>
            {
                entity.HasKey(e => e.GrId)
                    .HasName("GiftRecord_GrId_PK");

                entity.HasComment("礼薄");

                entity.Property(e => e.ActionTime).HasComment("活动时间");

                entity.Property(e => e.CreateTime).HasComment("创建时间");

                entity.Property(e => e.Describe).HasComment("描述");

                entity.Property(e => e.GrActionTime).HasComment("活动时间");

                entity.Property(e => e.GrCreateTime).HasComment("创建时间");

                entity.Property(e => e.GrDescription).HasComment("描述");

                entity.Property(e => e.GrName1).HasComment("涉及人员");

                entity.Property(e => e.GrName2).HasComment("涉及人员");

                entity.Property(e => e.GrName3).HasComment("涉及人员");

                entity.Property(e => e.GrName4).HasComment("涉及人员");

                entity.Property(e => e.GrRemark).HasComment("备注");

                entity.Property(e => e.GrTheme).HasComment("主题");

                entity.Property(e => e.GrType).HasComment("分类");

                entity.Property(e => e.Name1).HasComment("涉及人员");

                entity.Property(e => e.Name2).HasComment("涉及人员");

                entity.Property(e => e.Name3).HasComment("涉及人员");

                entity.Property(e => e.Name4).HasComment("涉及人员");

                entity.Property(e => e.Remark).HasComment("备注");

                entity.Property(e => e.Spare1)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Theme).HasComment("主题");

                entity.Property(e => e.Type).HasComment("分类");
            });

            modelBuilder.Entity<GiftRecordDetail>(entity =>
            {
                entity.HasKey(e => e.GrdId)
                    .HasName("GiftRecordDetail_GrdId_PK");

                entity.HasComment("礼薄明细");

                entity.Property(e => e.CreateTime).HasComment("时间");

                entity.Property(e => e.Gid).HasComment("主表ID");

                entity.Property(e => e.GiftCash).HasComment("礼金");

                entity.Property(e => e.GiftGoods).HasComment("礼物");

                entity.Property(e => e.GiverName).HasComment("送礼人");

                entity.Property(e => e.GrId).HasComment("主表ID");

                entity.Property(e => e.GrdCash).HasComment("礼金");

                entity.Property(e => e.GrdCreateTime).HasComment("时间");

                entity.Property(e => e.GrdGiverName).HasComment("送礼人");

                entity.Property(e => e.GrdGoods).HasComment("礼物");

                entity.Property(e => e.Spare1)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");
            });

            modelBuilder.Entity<Gist>(entity =>
            {
                entity.HasComment("代码片段");

                entity.Property(e => e.GistCode).HasComment("唯一编码");

                entity.Property(e => e.GistContent).HasComment("内容");

                entity.Property(e => e.GistContentPreview).HasComment("预览内容，前10行");

                entity.Property(e => e.GistCreateTime).HasComment("创建时间");

                entity.Property(e => e.GistFilename).HasComment("文件名");

                entity.Property(e => e.GistLanguage).HasComment("语言");

                entity.Property(e => e.GistOpen).HasComment("1公开，2私有");

                entity.Property(e => e.GistRemark).HasComment("备注");

                entity.Property(e => e.GistRow).HasComment("行数");

                entity.Property(e => e.GistStatus).HasComment("状态 1正常");

                entity.Property(e => e.GistTags).HasComment("标签");

                entity.Property(e => e.GistTheme).HasComment("主题");

                entity.Property(e => e.GistUpdateTime).HasComment("修改时间");

                entity.Property(e => e.Spare1)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Uid).HasComment("所属用户");
            });

            modelBuilder.Entity<GistSync>(entity =>
            {
                entity.HasKey(e => e.GistCode)
                    .HasName("GistSync_GitCode_PK");

                entity.HasComment("代码片段同步");

                entity.Property(e => e.GistFilename).HasComment("文件名");

                entity.Property(e => e.GsGitHubId).HasComment("GitHub的ID");

                entity.Property(e => e.GsGitHubTime).HasComment("GitHub最后同步时间，对应修改时间");

                entity.Property(e => e.GsGiteeId).HasComment("Gitee的ID");

                entity.Property(e => e.GsGiteeTime).HasComment("Gitee最后同步时间");

                entity.Property(e => e.Spare1)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Uid).HasComment("所属用户");
            });

            modelBuilder.Entity<GuffRecord>(entity =>
            {
                entity.HasKey(e => e.GrId)
                    .IsClustered(false);

                entity.HasComment("尬服列表");

                entity.HasIndex(e => e.GrCreateTime, "GuffRecord_GrCreateTime")
                    .IsClustered();

                entity.Property(e => e.GrId).IsUnicode(false);

                entity.Property(e => e.GrAudio).HasComment("音频，多个逗号分割");

                entity.Property(e => e.GrContent).HasComment("内容");

                entity.Property(e => e.GrContentMd).HasComment("内容Markdown");

                entity.Property(e => e.GrCreateTime).HasComment("初始发布时间");

                entity.Property(e => e.GrFile).HasComment("文件，多个逗号分割");

                entity.Property(e => e.GrImage).HasComment("图片，多个逗号分割");

                entity.Property(e => e.GrLaud).HasComment("点赞数");

                entity.Property(e => e.GrMark).HasComment("收藏数");

                entity.Property(e => e.GrObject).HasComment("对象，多个逗号分割，如主播姓名");

                entity.Property(e => e.GrOpen).HasComment("回复数");

                entity.Property(e => e.GrReadNum).HasComment("阅读量");

                entity.Property(e => e.GrRemark).HasComment("结束语");

                entity.Property(e => e.GrReplyNum).HasComment("1公开，2私有");

                entity.Property(e => e.GrStatus).HasComment("状态，1正常，2block，-1只读");

                entity.Property(e => e.GrTag).HasComment("标签，多个逗号分割");

                entity.Property(e => e.GrTypeName).HasComment("分类，直播、名人、书、音乐等");

                entity.Property(e => e.GrTypeValue).HasComment("分类值，如分类为斗鱼，值可为房间号");

                entity.Property(e => e.GrUpdateTime).HasComment("更新时间");

                entity.Property(e => e.GrVideo).HasComment("视频，多个逗号分割");

                entity.Property(e => e.Spare1).HasComment("备用");

                entity.Property(e => e.Spare2).HasComment("备用");

                entity.Property(e => e.Spare3).HasComment("备用");

                entity.Property(e => e.Uid).HasComment("创建用户");
            });

            modelBuilder.Entity<KeyValueSynonym>(entity =>
            {
                entity.HasKey(e => e.KsId)
                    .IsClustered(false);

                entity.HasComment("键值同义词");

                entity.HasIndex(e => e.KsName, "KeyValueSynonym_KsName")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.KeyName).HasComment("键名");

                entity.Property(e => e.KsName).HasComment("键名 同义词");

                entity.Property(e => e.Spare1).HasComment("备用");

                entity.Property(e => e.Spare2).HasComment("备用");

                entity.Property(e => e.Spare3).HasComment("备用");
            });

            modelBuilder.Entity<KeyValues>(entity =>
            {
                entity.HasKey(e => e.KeyId)
                    .IsClustered(false);

                entity.HasComment("键值");

                entity.HasIndex(e => e.KeyName, "KeyValues_KeyName")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.KeyName).HasComment("键名");

                entity.Property(e => e.KeyRemark).HasComment("备注");

                entity.Property(e => e.KeyType).HasComment("分类");

                entity.Property(e => e.KeyValue).HasComment("键值");

                entity.Property(e => e.KeyValue1).HasComment("键值");

                entity.Property(e => e.KeyValue2).HasComment("键值");

                entity.Property(e => e.KeyValue3).HasComment("键值");

                entity.Property(e => e.KeyValue4).HasComment("键值");

                entity.Property(e => e.KeyValue5).HasComment("键值");

                entity.Property(e => e.Spare1).HasComment("备用");

                entity.Property(e => e.Spare2).HasComment("备用");

                entity.Property(e => e.Spare3).HasComment("备用");
            });

            modelBuilder.Entity<Notepad>(entity =>
            {
                entity.HasKey(e => e.NoteId)
                    .IsClustered(false);

                entity.HasComment("记事本");

                entity.HasIndex(e => e.Uid, "Notepad_Uid")
                    .IsClustered();

                entity.Property(e => e.NoteContent).HasComment("SQL_Latin1_General_CP1_CI_AS");

                entity.Property(e => e.NoteCreateTime).HasComment("创建时间");

                entity.Property(e => e.NoteTitle).HasComment("SQL_Latin1_General_CP1_CI_AS");

                entity.Property(e => e.Spare1)
                    .IsUnicode(false)
                    .HasComment("SQL_Latin1_General_CP1_CI_AS");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("SQL_Latin1_General_CP1_CI_AS");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("SQL_Latin1_General_CP1_CI_AS");

                entity.Property(e => e.Uid).HasComment("所属用户ID");
            });

            modelBuilder.Entity<OperationRecord>(entity =>
            {
                entity.HasKey(e => e.OrId)
                    .HasName("PK_OPERATIONRECORD");

                entity.HasComment("操作记录");

                entity.Property(e => e.OrId).IsUnicode(false);

                entity.Property(e => e.OrAction)
                    .IsUnicode(false)
                    .HasComment("动作，具体的增删改等");

                entity.Property(e => e.OrCreateTime).HasComment("时间");

                entity.Property(e => e.OrMark).HasComment("标记");

                entity.Property(e => e.OrRemark).HasComment("备注");

                entity.Property(e => e.OrSource).HasComment("源");

                entity.Property(e => e.OrType)
                    .IsUnicode(false)
                    .HasComment("操作分类，推荐虚拟表名");

                entity.Property(e => e.Spare1).HasComment("备用");

                entity.Property(e => e.Spare2).HasComment("备用");

                entity.Property(e => e.Spare3).HasComment("备用");
            });

            modelBuilder.Entity<Run>(entity =>
            {
                entity.HasComment("运行");

                entity.Property(e => e.RunCode).HasComment("唯一编码");

                entity.Property(e => e.RunContent1).HasComment("内容 html");

                entity.Property(e => e.RunContent2).HasComment("内容 js");

                entity.Property(e => e.RunContent3).HasComment("内容 css");

                entity.Property(e => e.RunContent4).HasComment("内容");

                entity.Property(e => e.RunContent5).HasComment("内容");

                entity.Property(e => e.RunCreateTime).HasComment("创建时间");

                entity.Property(e => e.RunOpen).HasComment("公开1");

                entity.Property(e => e.RunRemark).HasComment("备注");

                entity.Property(e => e.RunStatus).HasComment("状态 1正常");

                entity.Property(e => e.RunTags).HasComment("标签");

                entity.Property(e => e.RunTheme).HasComment("主题");

                entity.Property(e => e.Spare1)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Uid).HasComment("所属用户");
            });

            modelBuilder.Entity<Tags>(entity =>
            {
                entity.HasKey(e => e.TagId)
                    .IsClustered(false);

                entity.HasComment("标签");

                entity.HasIndex(e => e.TagName, "Tags_TagName")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.Spare1)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.TagCode).HasComment("标签码");

                entity.Property(e => e.TagHot).HasComment("热度");

                entity.Property(e => e.TagIcon).HasComment("标签图标");

                entity.Property(e => e.TagName).HasComment("标签名");

                entity.Property(e => e.TagOrder).HasComment("排序");

                entity.Property(e => e.TagOwner).HasComment("创建用户UID，系统标签为0");

                entity.Property(e => e.TagPid).HasComment("Pid");

                entity.Property(e => e.TagState).HasComment("状态 1启用");

                entity.Property(e => e.TagStatus).HasComment("状态 1启用");
            });

            modelBuilder.Entity<UserConnection>(entity =>
            {
                entity.HasKey(e => e.UconnId)
                    .IsClustered(false);

                entity.HasComment("用户关联");

                entity.HasIndex(e => e.Uid, "UserConnection_Uid")
                    .IsClustered();

                entity.Property(e => e.UconnId).IsUnicode(false);

                entity.Property(e => e.Spare1)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.UconnAction).HasComment("1点赞，2收藏，3关注");

                entity.Property(e => e.UconnCreateTime).HasComment("创建时间");

                entity.Property(e => e.UconnTargetId)
                    .IsUnicode(false)
                    .HasComment("关联目标ID");

                entity.Property(e => e.UconnTargetType)
                    .IsUnicode(false)
                    .HasComment("关联分类");
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("UserInfo_UserId_PK");

                entity.HasComment("用户");

                entity.Property(e => e.LoginLimit).HasComment("登录限制 1限制 2补齐信息");

                entity.Property(e => e.Nickname).HasComment("昵称");

                entity.Property(e => e.OpenId1).HasComment("第三方登录，QQ");

                entity.Property(e => e.OpenId2).HasComment("第三方登录，Sina");

                entity.Property(e => e.OpenId3).HasComment("第三方登录，GitHub");

                entity.Property(e => e.OpenId4).HasComment("第三方登录，Taobao");

                entity.Property(e => e.OpenId5).HasComment("第三方登录，Microsoft");

                entity.Property(e => e.OpenId6).HasComment("第三方登录");

                entity.Property(e => e.OpenId7).HasComment("第三方登录");

                entity.Property(e => e.OpenId8).HasComment("第三方登录");

                entity.Property(e => e.OpenId9).HasComment("第三方登录");

                entity.Property(e => e.Spare1)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.UserAddTime).HasComment("注册时间");

                entity.Property(e => e.UserBirthday).HasComment("生日");

                entity.Property(e => e.UserCreateTime).HasComment("注册时间");

                entity.Property(e => e.UserLoginTime).HasComment("最后登录时间");

                entity.Property(e => e.UserMail).HasComment("邮箱");

                entity.Property(e => e.UserMailValid).HasComment("邮箱是否验证，1验证");

                entity.Property(e => e.UserName).HasComment("登录帐号");

                entity.Property(e => e.UserNameChange).HasComment("账号变更，1已经更改");

                entity.Property(e => e.UserPhone).HasComment("手机");

                entity.Property(e => e.UserPhoto).HasComment("头像");

                entity.Property(e => e.UserPwd).HasComment("登录密码");

                entity.Property(e => e.UserSay).HasComment("说");

                entity.Property(e => e.UserSex).HasComment("性别，1男，2女");

                entity.Property(e => e.UserSign).HasComment("登录标记");

                entity.Property(e => e.UserUrl).HasComment("网址");
            });

            modelBuilder.Entity<UserMessage>(entity =>
            {
                entity.HasKey(e => e.UmId)
                    .IsClustered(false);

                entity.HasComment("用户消息");

                entity.HasIndex(e => new { e.Uid, e.UmType, e.UmCreateTime }, "UserMessage_TypeAndUid")
                    .IsClustered();

                entity.Property(e => e.Spare1)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Uid).HasComment("接收用户");

                entity.Property(e => e.UmAction).HasComment("消息标记，1系统，2回复，3私信，4点赞，5收藏，6关注");

                entity.Property(e => e.UmContent).HasComment("消息内容");

                entity.Property(e => e.UmCreateTime).HasComment("创建时间");

                entity.Property(e => e.UmStatus).HasComment("状态，1未读，2已读");

                entity.Property(e => e.UmTargetId)
                    .IsUnicode(false)
                    .HasComment("消息目标ID");

                entity.Property(e => e.UmTargetIndex).HasComment("消息定向索引");

                entity.Property(e => e.UmTriggerUid).HasComment("触发用户ID");

                entity.Property(e => e.UmType)
                    .IsUnicode(false)
                    .HasComment("消息分类");
            });

            modelBuilder.Entity<UserReply>(entity =>
            {
                entity.HasKey(e => e.UrId)
                    .IsClustered(false);

                entity.HasComment("用户回复");

                entity.HasIndex(e => new { e.UrTargetType, e.UrTargetId }, "UserReply_TypeAndId")
                    .IsClustered();

                entity.Property(e => e.Spare1).HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Uid).HasComment("登录用户ID，匿名用户为0");

                entity.Property(e => e.UrAnonymousLink).HasComment("匿名链接");

                entity.Property(e => e.UrAnonymousMail).HasComment("匿名邮箱");

                entity.Property(e => e.UrAnonymousName).HasComment("匿名用户");

                entity.Property(e => e.UrContent).HasComment("回复内容");

                entity.Property(e => e.UrContentMd).HasComment("回复内容");

                entity.Property(e => e.UrCreateTime).HasComment("回复时间");

                entity.Property(e => e.UrStatus).HasComment("状态，1正常，2block");

                entity.Property(e => e.UrTargetId)
                    .IsUnicode(false)
                    .HasComment("目标ID");

                entity.Property(e => e.UrTargetPid).HasComment("目标PID");

                entity.Property(e => e.UrTargetType)
                    .IsUnicode(false)
                    .HasComment("目标分类");
            });

            modelBuilder.Entity<UserWriting>(entity =>
            {
                entity.HasKey(e => e.UwId)
                    .HasName("Writing_UwId_PK");

                entity.HasComment("用户写作");

                entity.Property(e => e.Spare1).HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.UwCategory).HasComment("所属分类");

                entity.Property(e => e.UwContent).HasComment("内容");

                entity.Property(e => e.UwContentMd).HasComment("内容Markdown");

                entity.Property(e => e.UwCreateTime).HasComment("初始发布时间");

                entity.Property(e => e.UwLastDate).HasComment("最后回复时间");

                entity.Property(e => e.UwLastUid).HasComment("最后回复人");

                entity.Property(e => e.UwLaud).HasComment("点赞数");

                entity.Property(e => e.UwMark).HasComment("点赞数");

                entity.Property(e => e.UwOpen).HasComment("1公开，2私有");

                entity.Property(e => e.UwReadNum).HasComment("阅读量");

                entity.Property(e => e.UwReplyNum).HasComment("回复数量");

                entity.Property(e => e.UwStatus).HasComment("状态，1正常，2block，-1只读");

                entity.Property(e => e.UwTitle).HasComment("标题");

                entity.Property(e => e.UwUpdateTime).HasComment("更新时间");
            });

            modelBuilder.Entity<UserWritingTags>(entity =>
            {
                entity.HasKey(e => e.UwtId)
                    .IsClustered(false);

                entity.HasComment("写作标签关联");

                entity.HasIndex(e => e.UwId, "UserWritingTags_UwId")
                    .IsClustered();

                entity.Property(e => e.Spare1).HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.TagCode).HasComment("标签编码");

                entity.Property(e => e.TagId).HasComment("标签表ID");

                entity.Property(e => e.TagName).HasComment("标签名");

                entity.Property(e => e.UwId).HasComment("写作表ID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}