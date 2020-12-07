using Microsoft.EntityFrameworkCore;
using Netnr.Chat.Domain;

#nullable disable

namespace Netnr.Chat.Data
{
    public partial class ContextBase : DbContext
    {
        public ContextBase()
        {
        }

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
            modelBuilder.Entity<NChatBuddy>(entity =>
            {
                entity.HasKey(e => e.CbId)
                    .HasName("PK_NCHATBUDDY");

                entity.HasComment("用户好友");

                entity.Property(e => e.CbId).IsUnicode(false);

                entity.Property(e => e.CbCreateTime).HasComment("创建时间");

                entity.Property(e => e.CbUserId)
                    .IsUnicode(false)
                    .HasComment("好友ID");

                entity.Property(e => e.CcId)
                    .IsUnicode(false)
                    .HasComment("归类ID（1：默认组，其它为引用）");

                entity.Property(e => e.CuUserId)
                    .IsUnicode(false)
                    .HasComment("用户ID");
            });

            modelBuilder.Entity<NChatClassify>(entity =>
            {
                entity.HasKey(e => e.CcId)
                    .HasName("PK_NCHATCLASSIFY");

                entity.HasComment("用户、组归类");

                entity.Property(e => e.CcId).IsUnicode(false);

                entity.Property(e => e.CcName).HasComment("归类名称");

                entity.Property(e => e.CcOrder).HasComment("归类排序");

                entity.Property(e => e.CcType).HasComment("归类类型（1：用户好友，2：用户组）");

                entity.Property(e => e.CuUserId)
                    .IsUnicode(false)
                    .HasComment("用户ID");
            });

            modelBuilder.Entity<NChatFile>(entity =>
            {
                entity.HasKey(e => e.CfId)
                    .HasName("PK_NCHATFILE");

                entity.HasComment("文件");

                entity.Property(e => e.CfId).IsUnicode(false);

                entity.Property(e => e.CfCreateTime).HasComment("创建时间");

                entity.Property(e => e.CfExt)
                    .IsUnicode(false)
                    .HasComment("文件扩展名");

                entity.Property(e => e.CfFileName).HasComment("文件名");

                entity.Property(e => e.CfFullPath).HasComment("文件全路径");

                entity.Property(e => e.CfSize).HasComment("文件大小");

                entity.Property(e => e.CfStatus).HasComment("状态（1：正常，2：限制）");

                entity.Property(e => e.CfType)
                    .IsUnicode(false)
                    .HasComment("文件类型");

                entity.Property(e => e.CuUserId)
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

                entity.Property(e => e.CgId).IsUnicode(false);

                entity.Property(e => e.CcId)
                    .IsUnicode(false)
                    .HasComment("归类ID（1：默认组，其它为引用）");

                entity.Property(e => e.CgCreateTime).HasComment("创建时间");

                entity.Property(e => e.CgName).HasComment("组名");

                entity.Property(e => e.CgOwnerId)
                    .IsUnicode(false)
                    .HasComment("所属用户ID");

                entity.Property(e => e.CgStatus).HasComment("状态（1：正常）");
            });

            modelBuilder.Entity<NChatGroupMember>(entity =>
            {
                entity.HasKey(e => e.CgmId)
                    .HasName("PK_NCHATGROUPMEMBER");

                entity.HasComment("组成员");

                entity.Property(e => e.CgmId).IsUnicode(false);

                entity.Property(e => e.CgId)
                    .IsUnicode(false)
                    .HasComment("组ID");

                entity.Property(e => e.CgmCreateTime).HasComment("创建时间");

                entity.Property(e => e.CgmStatus).HasComment("状态（1：正常，2：禁言）");

                entity.Property(e => e.CuMark).HasComment("标记");

                entity.Property(e => e.CuUserId)
                    .IsUnicode(false)
                    .HasComment("用户ID");
            });

            modelBuilder.Entity<NChatMessageGroupPull>(entity =>
            {
                entity.HasKey(e => e.CuUserId)
                    .HasName("PK_NCHATMESSAGEGROUPPULL");

                entity.HasComment("用户消息组接收记录");

                entity.Property(e => e.CuUserId)
                    .IsUnicode(false)
                    .HasComment("接收用户ID");

                entity.Property(e => e.CgGroupId)
                    .IsUnicode(false)
                    .HasComment("组ID");

                entity.Property(e => e.CmgId)
                    .IsUnicode(false)
                    .HasComment("组消息ID");

                entity.Property(e => e.GpId).IsUnicode(false);

                entity.Property(e => e.GpUpdateTime).HasComment("更新时间");
            });

            modelBuilder.Entity<NChatMessageToGroup>(entity =>
            {
                entity.HasKey(e => e.CmgId)
                    .HasName("PK_NCHATMESSAGETOGROUP");

                entity.HasComment("组消息");

                entity.Property(e => e.CmgId).IsUnicode(false);

                entity.Property(e => e.CmgContent).HasComment("发送消息内容");

                entity.Property(e => e.CmgCreateTime).HasComment("创建时间");

                entity.Property(e => e.CmgPullGroupId)
                    .IsUnicode(false)
                    .HasComment("接收组ID");

                entity.Property(e => e.CmgPushType).HasComment("消息类型");

                entity.Property(e => e.CmgPushUserDevice)
                    .IsUnicode(false)
                    .HasComment("发送用户设备");

                entity.Property(e => e.CmgPushUserId)
                    .IsUnicode(false)
                    .HasComment("发送用户ID");

                entity.Property(e => e.CmgPushUserSign)
                    .IsUnicode(false)
                    .HasComment("发送用户标识");

                entity.Property(e => e.CmgPushWhich).HasComment("发送哪种");
            });

            modelBuilder.Entity<NChatMessageToUser>(entity =>
            {
                entity.HasKey(e => e.CmuId)
                    .HasName("PK_NCHATMESSAGETOUSER");

                entity.HasComment("用户消息");

                entity.Property(e => e.CmuId).IsUnicode(false);

                entity.Property(e => e.CmuContent).HasComment("发送消息内容");

                entity.Property(e => e.CmuCreateTime).HasComment("创建时间");

                entity.Property(e => e.CmuPullUserId)
                    .IsUnicode(false)
                    .HasComment("接收用户ID");

                entity.Property(e => e.CmuPushType).HasComment("消息类型");

                entity.Property(e => e.CmuPushUserDevice)
                    .IsUnicode(false)
                    .HasComment("发送用户设备");

                entity.Property(e => e.CmuPushUserId)
                    .IsUnicode(false)
                    .HasComment("发送用户ID");

                entity.Property(e => e.CmuPushUserSign)
                    .IsUnicode(false)
                    .HasComment("发送用户标识");

                entity.Property(e => e.CmuPushWhich).HasComment("发送哪种");

                entity.Property(e => e.CmuStatus).HasComment("消息状态（-1：撤回，1：待推送，2：已推送，3：已接收，4：已读）");
            });

            modelBuilder.Entity<NChatNotice>(entity =>
            {
                entity.HasKey(e => e.CnId)
                    .HasName("PK_NCHATNOTICE");

                entity.HasComment("通知信息");

                entity.Property(e => e.CnId).IsUnicode(false);

                entity.Property(e => e.CnCreateTime).HasComment("创建时间");

                entity.Property(e => e.CnFromId)
                    .IsUnicode(false)
                    .HasComment("发送用户ID");

                entity.Property(e => e.CnNotice1).HasComment("发送通知");

                entity.Property(e => e.CnNotice2).HasComment("发送通知");

                entity.Property(e => e.CnReason).HasComment("接收用户处理事由");

                entity.Property(e => e.CnResult).HasComment("接收用户处理结果（1：通过，2：拒绝）");

                entity.Property(e => e.CnStatus).HasComment("状态（1：正常，2：删除）");

                entity.Property(e => e.CnType).HasComment("通知类型（1：好友申请，2：组申请，3：离开组）");

                entity.Property(e => e.CuUserId)
                    .IsUnicode(false)
                    .HasComment("接收用户ID");
            });

            modelBuilder.Entity<NChatUser>(entity =>
            {
                entity.HasKey(e => e.CuUserId)
                    .HasName("PK_NCHATUSER");

                entity.HasComment("用户表");

                entity.Property(e => e.CuUserId)
                    .IsUnicode(false)
                    .HasComment("用户ID");

                entity.Property(e => e.CuCreateTime).HasComment("创建时间");

                entity.Property(e => e.CuPassword)
                    .IsUnicode(false)
                    .HasComment("密码");

                entity.Property(e => e.CuStatus).HasComment("状态（1：正常，2：限制登录）");

                entity.Property(e => e.CuUserName).HasComment("账号");

                entity.Property(e => e.CuUserNickname).HasComment("昵称");

                entity.Property(e => e.CuUserPhoto).HasComment("头像");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
