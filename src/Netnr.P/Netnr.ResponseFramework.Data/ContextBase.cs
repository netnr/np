using Microsoft.EntityFrameworkCore;
using Netnr.ResponseFramework.Domain;

namespace Netnr.ResponseFramework.Data
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

        public virtual DbSet<SysButton> SysButton { get; set; }
        public virtual DbSet<SysDictionary> SysDictionary { get; set; }
        public virtual DbSet<SysLog> SysLog { get; set; }
        public virtual DbSet<SysMenu> SysMenu { get; set; }
        public virtual DbSet<SysRole> SysRole { get; set; }
        public virtual DbSet<SysTableConfig> SysTableConfig { get; set; }
        public virtual DbSet<SysUser> SysUser { get; set; }
        public virtual DbSet<TempExample> TempExample { get; set; }
        public virtual DbSet<TempInvoiceDetail> TempInvoiceDetail { get; set; }
        public virtual DbSet<TempInvoiceMain> TempInvoiceMain { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SysButton>(entity =>
            {
                entity.HasKey(e => e.SbId)
                    .HasName("SysButton_SbId_PK");

                entity.HasComment("系统按钮表");

                entity.Property(e => e.SbId).IsUnicode(false);

                entity.Property(e => e.SbBtnClass)
                    .IsUnicode(false)
                    .HasComment("按钮类");

                entity.Property(e => e.SbBtnGroup).HasComment("分组");

                entity.Property(e => e.SbBtnHide).HasComment("隐藏，1隐藏");

                entity.Property(e => e.SbBtnIcon)
                    .IsUnicode(false)
                    .HasComment("按钮图标");

                entity.Property(e => e.SbBtnId)
                    .IsUnicode(false)
                    .HasComment("按钮ID");

                entity.Property(e => e.SbBtnOrder).HasComment("排序");

                entity.Property(e => e.SbBtnText).HasComment("按钮文本");

                entity.Property(e => e.SbDescribe).HasComment("描述");

                entity.Property(e => e.SbPid).IsUnicode(false);

                entity.Property(e => e.SbStatus).HasComment("状态，1启用");
            });

            modelBuilder.Entity<SysDictionary>(entity =>
            {
                entity.HasKey(e => e.SdId)
                    .HasName("SysDictionary_SdId_PK")
                    .IsClustered(false);

                entity.HasComment("系统字典表");

                entity.HasIndex(e => e.SdType, "SysDictionary_SdType")
                    .IsClustered();

                entity.Property(e => e.SdId).IsUnicode(false);

                entity.Property(e => e.SdAttribute1)
                    .IsUnicode(false)
                    .HasComment("特性");

                entity.Property(e => e.SdAttribute2)
                    .IsUnicode(false)
                    .HasComment("特性");

                entity.Property(e => e.SdAttribute3)
                    .IsUnicode(false)
                    .HasComment("特性");

                entity.Property(e => e.SdKey)
                    .IsUnicode(false)
                    .HasComment("键");

                entity.Property(e => e.SdOrder).HasComment("排序");

                entity.Property(e => e.SdPid)
                    .IsUnicode(false)
                    .HasComment("上级ID");

                entity.Property(e => e.SdRemark).HasComment("备注");

                entity.Property(e => e.SdStatus).HasComment("状态：1正常，-1删除，2停用");

                entity.Property(e => e.SdType)
                    .IsUnicode(false)
                    .HasComment("字典类别");

                entity.Property(e => e.SdValue).HasComment("值");
            });

            modelBuilder.Entity<SysLog>(entity =>
            {
                entity.HasKey(e => e.LogId)
                    .HasName("SysLog_LogId_PK")
                    .IsClustered(false);

                entity.HasComment("系统日志表");

                entity.HasIndex(e => e.LogCreateTime, "SysLog_LogCreateTime")
                    .IsClustered();

                entity.Property(e => e.LogId)
                    .IsUnicode(false)
                    .HasComment("");

                entity.Property(e => e.LogAction).HasComment("动作");

                entity.Property(e => e.LogArea)
                    .IsUnicode(false)
                    .HasComment("IP归属地");

                entity.Property(e => e.LogBrowserName).HasComment("浏览器");

                entity.Property(e => e.LogContent).HasComment("内容");

                entity.Property(e => e.LogCreateTime).HasComment("创建时间");

                entity.Property(e => e.LogGroup).HasComment("分组（1：默认；2：爬虫）");

                entity.Property(e => e.LogIp)
                    .IsUnicode(false)
                    .HasComment("IP");

                entity.Property(e => e.LogLevel)
                    .IsUnicode(false)
                    .HasComment("级别（F： Fatal；E：Error；W：Warn；I：Info；D：Debug；A：All）");

                entity.Property(e => e.LogRemark).HasComment("备注");

                entity.Property(e => e.LogSystemName).HasComment("客户端操作系统");

                entity.Property(e => e.LogUrl)
                    .IsUnicode(false)
                    .HasComment("链接");

                entity.Property(e => e.LogUserAgent)
                    .IsUnicode(false)
                    .HasComment("User-Agent");

                entity.Property(e => e.SuName).HasComment("用户名");

                entity.Property(e => e.SuNickname).HasComment("昵称");
            });

            modelBuilder.Entity<SysMenu>(entity =>
            {
                entity.HasKey(e => e.SmId)
                    .HasName("SysMenu_SmId_PK");

                entity.HasComment("系统菜单表");

                entity.Property(e => e.SmId).IsUnicode(false);

                entity.Property(e => e.SmGroup).HasComment("分组，默认1，比如移动端为2");

                entity.Property(e => e.SmIcon)
                    .IsUnicode(false)
                    .HasComment("图标");

                entity.Property(e => e.SmName).HasComment("名称");

                entity.Property(e => e.SmOrder).HasComment("排序");

                entity.Property(e => e.SmPid).IsUnicode(false);

                entity.Property(e => e.SmStatus).HasComment("状态，1启用");

                entity.Property(e => e.SmUrl)
                    .IsUnicode(false)
                    .HasComment("链接");
            });

            modelBuilder.Entity<SysRole>(entity =>
            {
                entity.HasKey(e => e.SrId)
                    .HasName("SysRole_SrId_PK");

                entity.HasComment("系统角色表");

                entity.Property(e => e.SrId).IsUnicode(false);

                entity.Property(e => e.SrButtons).HasComment("按钮");

                entity.Property(e => e.SrCreateTime).HasComment("创建时间");

                entity.Property(e => e.SrDescribe).HasComment("描述");

                entity.Property(e => e.SrGroup).HasComment("分组");

                entity.Property(e => e.SrMenus).HasComment("菜单");

                entity.Property(e => e.SrName).HasComment("名称");

                entity.Property(e => e.SrStatus)
                    .HasDefaultValueSql("((0))")
                    .HasComment("状态，1启用");
            });

            modelBuilder.Entity<SysTableConfig>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("SysTableConfig_Id_PK")
                    .IsClustered(false);

                entity.HasComment("表配置");

                entity.HasIndex(e => e.TableName, "SysTableConfig_TableName")
                    .IsClustered();

                entity.Property(e => e.Id).IsUnicode(false);

                entity.Property(e => e.ColAlign).HasComment("对齐方式 1左，2中，3右");

                entity.Property(e => e.ColExport)
                    .HasDefaultValueSql("((0))")
                    .HasComment("1导出");

                entity.Property(e => e.ColField)
                    .IsUnicode(false)
                    .HasComment("列键");

                entity.Property(e => e.ColFormat).HasComment("格式化");

                entity.Property(e => e.ColFrozen)
                    .HasDefaultValueSql("((0))")
                    .HasComment("1冻结");

                entity.Property(e => e.ColHide).HasComment("1隐藏");

                entity.Property(e => e.ColOrder)
                    .HasDefaultValueSql("((0))")
                    .HasComment("排序");

                entity.Property(e => e.ColQuery)
                    .HasDefaultValueSql("((0))")
                    .HasComment("1查询");

                entity.Property(e => e.ColRelation)
                    .IsUnicode(false)
                    .HasComment("查询关系符");

                entity.Property(e => e.ColSort)
                    .HasDefaultValueSql("((0))")
                    .HasComment("1启用点击排序");

                entity.Property(e => e.ColTitle).HasComment("列标题");

                entity.Property(e => e.ColWidth).HasComment("列宽");

                entity.Property(e => e.DvTitle).HasComment("默认列标题");

                entity.Property(e => e.FormArea).HasComment("区域");

                entity.Property(e => e.FormHide).HasComment("1隐藏");

                entity.Property(e => e.FormMaxlength).HasComment("最大长度");

                entity.Property(e => e.FormOrder).HasComment("排序");

                entity.Property(e => e.FormPlaceholder).HasComment("输入框提示");

                entity.Property(e => e.FormRequired)
                    .HasDefaultValueSql("((0))")
                    .HasComment("1必填");

                entity.Property(e => e.FormSpan).HasComment("跨列");

                entity.Property(e => e.FormText).HasComment("显示文本");

                entity.Property(e => e.FormType)
                    .IsUnicode(false)
                    .HasComment("输入类型");

                entity.Property(e => e.FormUrl).HasComment("来源");

                entity.Property(e => e.FormValue).HasComment("初始值");

                entity.Property(e => e.TableName)
                    .IsUnicode(false)
                    .HasComment("（虚）表名");
            });

            modelBuilder.Entity<SysUser>(entity =>
            {
                entity.HasKey(e => e.SuId)
                    .HasName("SysUser_SuId_PK");

                entity.HasComment("系统用户表");

                entity.Property(e => e.SuId).IsUnicode(false);

                entity.Property(e => e.SrId)
                    .IsUnicode(false)
                    .HasComment("角色");

                entity.Property(e => e.SuCreateTime).HasComment("创建时间");

                entity.Property(e => e.SuGroup).HasComment("分组");

                entity.Property(e => e.SuName).HasComment("账号");

                entity.Property(e => e.SuNickname).HasComment("昵称");

                entity.Property(e => e.SuPwd).HasComment("密码");

                entity.Property(e => e.SuSign)
                    .IsUnicode(false)
                    .HasComment("登录标识");

                entity.Property(e => e.SuStatus)
                    .HasDefaultValueSql("((0))")
                    .HasComment("状态，1正常");
            });

            modelBuilder.Entity<TempExample>(entity =>
            {
                entity.HasComment("示例表，请删除");

                entity.Property(e => e.Id).IsUnicode(false);

                entity.Property(e => e.ColAlign).HasComment("对齐方式 1左，2中，3右");

                entity.Property(e => e.ColExport).HasComment("1导出");

                entity.Property(e => e.ColField)
                    .IsUnicode(false)
                    .HasComment("列键");

                entity.Property(e => e.ColFormat).HasComment("格式化");

                entity.Property(e => e.ColFrozen).HasComment("1冻结");

                entity.Property(e => e.ColHide).HasComment("1隐藏");

                entity.Property(e => e.ColOrder).HasComment("排序");

                entity.Property(e => e.ColQuery).HasComment("1查询");

                entity.Property(e => e.ColSort).HasComment("1启用点击排序");

                entity.Property(e => e.ColTitle).HasComment("列标题");

                entity.Property(e => e.ColWidth).HasComment("列宽");

                entity.Property(e => e.DvTitle).HasComment("默认列标题");

                entity.Property(e => e.FormArea).HasComment("区域");

                entity.Property(e => e.FormHide).HasComment("1隐藏");

                entity.Property(e => e.FormOrder).HasComment("排序");

                entity.Property(e => e.FormPlaceholder).HasComment("输入框提示");

                entity.Property(e => e.FormRequired).HasComment("1必填");

                entity.Property(e => e.FormSpan).HasComment("跨列");

                entity.Property(e => e.FormText).HasComment("显示文本");

                entity.Property(e => e.FormType)
                    .IsUnicode(false)
                    .HasComment("输入类型");

                entity.Property(e => e.FormUrl).HasComment("来源");

                entity.Property(e => e.FormValue).HasComment("初始值");

                entity.Property(e => e.TableName)
                    .IsUnicode(false)
                    .HasComment("（虚）表名");
            });

            modelBuilder.Entity<TempInvoiceDetail>(entity =>
            {
                entity.HasKey(e => e.TidId)
                    .HasName("TempInvoiceDetail_TidId_PK");

                entity.HasComment("单据明细");

                entity.Property(e => e.TidId).IsUnicode(false);

                entity.Property(e => e.GoodsCost).HasComment("商品成本");

                entity.Property(e => e.GoodsCount).HasComment("商品数量");

                entity.Property(e => e.GoodsId)
                    .IsUnicode(false)
                    .HasComment("商品ID");

                entity.Property(e => e.GoodsPrice).HasComment("商品售价");

                entity.Property(e => e.Spare1)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.TidOrder).HasComment("排序");

                entity.Property(e => e.TimId)
                    .IsUnicode(false)
                    .HasComment("单据主表ID");

                entity.Property(e => e.TimNo)
                    .IsUnicode(false)
                    .HasComment("单据号");
            });

            modelBuilder.Entity<TempInvoiceMain>(entity =>
            {
                entity.HasKey(e => e.TimId)
                    .HasName("TempInvoiceMain_TimId_PK");

                entity.HasComment("单据主表");

                entity.Property(e => e.TimId).IsUnicode(false);

                entity.Property(e => e.Spare1)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare2)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.Spare3)
                    .IsUnicode(false)
                    .HasComment("备用");

                entity.Property(e => e.TimCreateTime).HasComment("创建时间");

                entity.Property(e => e.TimDate).HasComment("单据日期");

                entity.Property(e => e.TimNo)
                    .IsUnicode(false)
                    .HasComment("单据号");

                entity.Property(e => e.TimOwnerId)
                    .IsUnicode(false)
                    .HasComment("制单人");

                entity.Property(e => e.TimOwnerName).HasComment("制单人");

                entity.Property(e => e.TimRemark).HasComment("备注");

                entity.Property(e => e.TimStatus).HasComment("状态，1默认，2已审核，3未通过，4作废");

                entity.Property(e => e.TimStore)
                    .IsUnicode(false)
                    .HasComment("门店");

                entity.Property(e => e.TimSupplier)
                    .IsUnicode(false)
                    .HasComment("供应商");

                entity.Property(e => e.TimType).HasComment("采购类型");

                entity.Property(e => e.TimUser)
                    .IsUnicode(false)
                    .HasComment("采购员");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}