using Microsoft.EntityFrameworkCore;
using Netnr.Chat.Domain;

namespace Netnr.Chat.Data
{
    public partial class ContextBase : DbContext
    {
        public ContextBase(DbContextOptions<ContextBase> options)
            : base(options)
        {
        }

        public virtual DbSet<NChatBuddy> NChatBuddy { get; set; }
        public virtual DbSet<NChatClassify> NChatClassify { get; set; }
        public virtual DbSet<NChatFile> NChatFile { get; set; }
        public virtual DbSet<NChatGroup> NChatGroup { get; set; }
        public virtual DbSet<NChatGroupMember> NChatGroupMember { get; set; }
        public virtual DbSet<NChatMessageGroupPull> NChatMessageGroupPull { get; set; }
        public virtual DbSet<NChatMessageToGroup> NChatMessageToGroup { get; set; }
        public virtual DbSet<NChatMessageToUser> NChatMessageToUser { get; set; }
        public virtual DbSet<NChatNotice> NChatNotice { get; set; }
        public virtual DbSet<NChatUser> NChatUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_PRC_CI_AS");

            modelBuilder.Entity<NChatBuddy>(entity =>
            {
                entity.HasKey(e => e.CbId)
                    .HasName("PK_NCHATBUDDY");

                entity.HasComment("用户好友");

                entity.HasIndex(e => e.CuUserId, "Index_1");

                entity.Property(e => e.CbId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CbCreateTime)
                    .HasColumnType("datetime")
                    .HasComment("创建时间");

                entity.Property(e => e.CbUserId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("好友ID");

                entity.Property(e => e.CcId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("归类ID（1：默认组，其它为引用）");

                entity.Property(e => e.CuUserId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("用户ID");
            });

            modelBuilder.Entity<NChatClassify>(entity =>
            {
                entity.HasKey(e => e.CcId)
                    .HasName("PK_NCHATCLASSIFY");

                entity.HasComment("用户、组归类");

                entity.HasIndex(e => e.CuUserId, "Index_1");

                entity.Property(e => e.CcId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CcName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("归类名称");

                entity.Property(e => e.CcOrder).HasComment("归类排序");

                entity.Property(e => e.CcType).HasComment("归类类型（1：用户好友，2：用户组）");

                entity.Property(e => e.CuUserId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("用户ID");
            });

            modelBuilder.Entity<NChatFile>(entity =>
            {
                entity.HasKey(e => e.CfId)
                    .HasName("PK_NCHATFILE");

                entity.HasComment("文件");

                entity.Property(e => e.CfId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CfCreateTime)
                    .HasColumnType("datetime")
                    .HasComment("创建时间");

                entity.Property(e => e.CfExt)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasComment("文件扩展名");

                entity.Property(e => e.CfFileName)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasComment("文件名");

                entity.Property(e => e.CfFullPath)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .HasComment("文件全路径");

                entity.Property(e => e.CfSize).HasComment("文件大小");

                entity.Property(e => e.CfStatus).HasComment("状态（1：正常，2：限制）");

                entity.Property(e => e.CfType)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("文件类型");

                entity.Property(e => e.CuUserId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("用户ID");
            });

            modelBuilder.Entity<NChatGroup>(entity =>
            {
                entity.HasKey(e => e.CgId)
                    .HasName("PK_NCHATGROUP")
                    .IsClustered(false);

                entity.HasComment("用户组");

                entity.HasIndex(e => e.CgOwnerId, "Index_1")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.CgId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CcId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("归类ID（1：默认组，其它为引用）");

                entity.Property(e => e.CgCreateTime)
                    .HasColumnType("datetime")
                    .HasComment("创建时间");

                entity.Property(e => e.CgName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("组名");

                entity.Property(e => e.CgOwnerId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("所属用户ID");

                entity.Property(e => e.CgStatus).HasComment("状态（1：正常）");
            });

            modelBuilder.Entity<NChatGroupMember>(entity =>
            {
                entity.HasKey(e => e.CgmId)
                    .HasName("PK_NCHATGROUPMEMBER");

                entity.HasComment("组成员");

                entity.HasIndex(e => e.CgId, "Index_1");

                entity.HasIndex(e => e.CuUserId, "Index_2");

                entity.Property(e => e.CgmId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CgId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("组ID");

                entity.Property(e => e.CgmCreateTime)
                    .HasColumnType("datetime")
                    .HasComment("创建时间");

                entity.Property(e => e.CgmStatus).HasComment("状态（1：正常，2：禁言）");

                entity.Property(e => e.CuMark)
                    .HasMaxLength(50)
                    .HasComment("标记");

                entity.Property(e => e.CuUserId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("用户ID");
            });

            modelBuilder.Entity<NChatMessageGroupPull>(entity =>
            {
                entity.HasKey(e => e.CuUserId)
                    .HasName("PK_NCHATMESSAGEGROUPPULL");

                entity.HasComment("用户消息组接收记录");

                entity.Property(e => e.CuUserId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("接收用户ID");

                entity.Property(e => e.CgGroupId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("组ID");

                entity.Property(e => e.CmgId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("组消息ID");

                entity.Property(e => e.GpId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GpUpdateTime)
                    .HasColumnType("datetime")
                    .HasComment("更新时间");
            });

            modelBuilder.Entity<NChatMessageToGroup>(entity =>
            {
                entity.HasKey(e => e.CmgId)
                    .HasName("PK_NCHATMESSAGETOGROUP");

                entity.HasComment("群组消息");

                entity.HasIndex(e => e.CmgPushUserId, "Index_1");

                entity.HasIndex(e => e.CmgPullGroupId, "Index_2");

                entity.Property(e => e.CmgId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CmgContent)
                    .IsRequired()
                    .HasComment("发送消息内容");

                entity.Property(e => e.CmgCreateTime)
                    .HasColumnType("datetime")
                    .HasComment("创建时间");

                entity.Property(e => e.CmgPullGroupId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("接收群组ID");

                entity.Property(e => e.CmgPushType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("类型 枚举MessageType");

                entity.Property(e => e.CmgPushUserDevice)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasComment("发送用户设备");

                entity.Property(e => e.CmgPushUserId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("发送用户ID");

                entity.Property(e => e.CmgPushUserSign)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("发送用户标识");

                entity.Property(e => e.CmgPushWhich)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("发送哪种");
            });

            modelBuilder.Entity<NChatMessageToUser>(entity =>
            {
                entity.HasKey(e => e.CmuId)
                    .HasName("PK_NCHATMESSAGETOUSER");

                entity.HasComment("用户消息");

                entity.HasIndex(e => e.CmuPushUserId, "Index_1");

                entity.HasIndex(e => e.CmuPullUserId, "Index_2");

                entity.Property(e => e.CmuId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CmuContent)
                    .IsRequired()
                    .HasComment("发送消息内容");

                entity.Property(e => e.CmuCreateTime)
                    .HasColumnType("datetime")
                    .HasComment("创建时间");

                entity.Property(e => e.CmuPullUserId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("接收用户ID");

                entity.Property(e => e.CmuPushType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("消息类型 枚举MessageType");

                entity.Property(e => e.CmuPushUserDevice)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasComment("发送用户设备");

                entity.Property(e => e.CmuPushUserId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("发送用户ID");

                entity.Property(e => e.CmuPushUserSign)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("发送用户标识");

                entity.Property(e => e.CmuPushWhich)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("发送哪种");

                entity.Property(e => e.CmuStatus).HasComment("消息状态（-1：撤回，1：待推送，2：已推送，3：已接收，4：已读）");
            });

            modelBuilder.Entity<NChatNotice>(entity =>
            {
                entity.HasKey(e => e.CnId)
                    .HasName("PK_NCHATNOTICE");

                entity.HasComment("通知信息");

                entity.Property(e => e.CnId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CnCreateTime)
                    .HasColumnType("datetime")
                    .HasComment("创建时间");

                entity.Property(e => e.CnFromId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("发送用户ID");

                entity.Property(e => e.CnNotice1)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasComment("发送通知");

                entity.Property(e => e.CnNotice2)
                    .HasMaxLength(200)
                    .HasComment("发送通知");

                entity.Property(e => e.CnReason)
                    .HasMaxLength(50)
                    .HasComment("接收用户处理事由");

                entity.Property(e => e.CnResult).HasComment("接收用户处理结果（1：通过，2：拒绝）");

                entity.Property(e => e.CnStatus).HasComment("状态（1：正常，2：删除）");

                entity.Property(e => e.CnType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("类型 枚举MessageType");

                entity.Property(e => e.CuUserId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("接收用户ID");
            });

            modelBuilder.Entity<NChatUser>(entity =>
            {
                entity.HasKey(e => e.CuUserId)
                    .HasName("PK_NCHATUSER");

                entity.HasComment("用户表");

                entity.HasIndex(e => e.CuUserName, "Index_1");

                entity.Property(e => e.CuUserId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("用户ID");

                entity.Property(e => e.CuCreateTime)
                    .HasColumnType("datetime")
                    .HasComment("创建时间");

                entity.Property(e => e.CuPassword)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("密码");

                entity.Property(e => e.CuStatus).HasComment("状态（1：正常，2：限制登录）");

                entity.Property(e => e.CuUserName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("账号");

                entity.Property(e => e.CuUserNickname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("昵称");

                entity.Property(e => e.CuUserPhoto)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasComment("头像");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
