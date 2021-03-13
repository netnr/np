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
                    .HasName("PRIMARY");

                entity.HasComment("系统按钮表");

                entity.Property(e => e.SbId)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SbBtnClass)
                    .HasComment("按钮类")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SbBtnGroup).HasComment("分组");

                entity.Property(e => e.SbBtnHide).HasComment("隐藏，1隐藏");

                entity.Property(e => e.SbBtnIcon)
                    .HasComment("按钮图标")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SbBtnId)
                    .HasComment("按钮ID")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SbBtnOrder).HasComment("排序");

                entity.Property(e => e.SbBtnText)
                    .HasComment("按钮文本")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SbDescribe)
                    .HasComment("描述")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SbPid)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SbStatus).HasComment("状态，1启用");
            });

            modelBuilder.Entity<SysDictionary>(entity =>
            {
                entity.HasKey(e => e.SdId)
                    .HasName("PRIMARY");

                entity.HasComment("系统字典表");

                entity.Property(e => e.SdId)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SdAttribute1)
                    .HasComment("特性")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SdAttribute2)
                    .HasComment("特性")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SdAttribute3)
                    .HasComment("特性")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SdKey)
                    .HasComment("键")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SdOrder).HasComment("排序");

                entity.Property(e => e.SdPid)
                    .HasComment("上级ID")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SdRemark)
                    .HasComment("备注")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SdStatus).HasComment("状态：1正常，-1删除，2停用");

                entity.Property(e => e.SdType)
                    .HasComment("字典类别")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SdValue)
                    .HasComment("值")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<SysLog>(entity =>
            {
                entity.HasKey(e => e.LogId)
                    .HasName("PRIMARY");

                entity.HasComment("系统日志表");

                entity.Property(e => e.LogId)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.LogAction)
                    .HasComment("动作")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.LogArea)
                    .HasComment("IP归属地")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.LogBrowserName)
                    .HasComment("浏览器")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.LogContent)
                    .HasComment("内容")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.LogCreateTime).HasComment("创建时间");

                entity.Property(e => e.LogGroup).HasComment("分组（1：默认；2：爬虫）");

                entity.Property(e => e.LogIp)
                    .HasComment("IP")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.LogLevel)
                    .HasComment("级别（F： Fatal；E：Error；W：Warn；I：Info；D：Debug；A：All）")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.LogRemark)
                    .HasComment("备注")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.LogSystemName)
                    .HasComment("客户端操作系统")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.LogUrl)
                    .HasComment("链接")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.LogUserAgent)
                    .HasComment("User-Agent")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SuName)
                    .HasComment("用户名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SuNickname)
                    .HasComment("昵称")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<SysMenu>(entity =>
            {
                entity.HasKey(e => e.SmId)
                    .HasName("PRIMARY");

                entity.HasComment("系统菜单表");

                entity.Property(e => e.SmId)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SmGroup).HasComment("分组，默认1，比如移动端为2");

                entity.Property(e => e.SmIcon)
                    .HasComment("图标")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SmName)
                    .HasComment("名称")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SmOrder).HasComment("排序");

                entity.Property(e => e.SmPid)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SmStatus).HasComment("状态，1启用");

                entity.Property(e => e.SmUrl)
                    .HasComment("链接")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<SysRole>(entity =>
            {
                entity.HasKey(e => e.SrId)
                    .HasName("PRIMARY");

                entity.HasComment("系统角色表");

                entity.Property(e => e.SrId)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SrButtons)
                    .HasComment("按钮")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SrCreateTime).HasComment("创建时间");

                entity.Property(e => e.SrDescribe)
                    .HasComment("描述")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SrGroup).HasComment("分组");

                entity.Property(e => e.SrMenus)
                    .HasComment("菜单")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SrName)
                    .HasComment("名称")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SrStatus)
                    .HasDefaultValueSql("'0'")
                    .HasComment("状态，1启用");
            });

            modelBuilder.Entity<SysTableConfig>(entity =>
            {
                entity.HasComment("表配置");

                entity.Property(e => e.Id)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ColAlign).HasComment("对齐方式 1左，2中，3右");

                entity.Property(e => e.ColExport)
                    .HasDefaultValueSql("'0'")
                    .HasComment("1导出");

                entity.Property(e => e.ColField)
                    .HasComment("列键")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ColFormat)
                    .HasComment("格式化")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ColFrozen)
                    .HasDefaultValueSql("'0'")
                    .HasComment("1冻结");

                entity.Property(e => e.ColHide).HasComment("1隐藏");

                entity.Property(e => e.ColOrder)
                    .HasDefaultValueSql("'0'")
                    .HasComment("排序");

                entity.Property(e => e.ColQuery)
                    .HasDefaultValueSql("'0'")
                    .HasComment("1查询");

                entity.Property(e => e.ColRelation)
                    .HasComment("查询关系符")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ColSort)
                    .HasDefaultValueSql("'0'")
                    .HasComment("1启用点击排序");

                entity.Property(e => e.ColTitle)
                    .HasComment("列标题")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ColWidth).HasComment("列宽");

                entity.Property(e => e.DvTitle)
                    .HasComment("默认列标题")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.FormArea).HasComment("区域");

                entity.Property(e => e.FormHide).HasComment("1隐藏");

                entity.Property(e => e.FormMaxlength).HasComment("最大长度");

                entity.Property(e => e.FormOrder).HasComment("排序");

                entity.Property(e => e.FormPlaceholder)
                    .HasComment("输入框提示")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.FormRequired)
                    .HasDefaultValueSql("'0'")
                    .HasComment("1必填");

                entity.Property(e => e.FormSpan).HasComment("跨列");

                entity.Property(e => e.FormText)
                    .HasComment("显示文本")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.FormType)
                    .HasComment("输入类型")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.FormUrl)
                    .HasComment("来源")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.FormValue)
                    .HasComment("初始值")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TableName)
                    .HasComment("（虚）表名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<SysUser>(entity =>
            {
                entity.HasKey(e => e.SuId)
                    .HasName("PRIMARY");

                entity.HasComment("系统用户表");

                entity.Property(e => e.SuId)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SrId)
                    .HasComment("角色")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SuCreateTime).HasComment("创建时间");

                entity.Property(e => e.SuGroup).HasComment("分组");

                entity.Property(e => e.SuName)
                    .HasComment("账号")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SuNickname)
                    .HasComment("昵称")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SuPwd)
                    .HasComment("密码")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SuSign)
                    .HasComment("登录标识")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SuStatus)
                    .HasDefaultValueSql("'0'")
                    .HasComment("状态，1正常");
            });

            modelBuilder.Entity<TempExample>(entity =>
            {
                entity.HasComment("示例表，请删除");

                entity.Property(e => e.Id)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ColAlign).HasComment("对齐方式 1左，2中，3右");

                entity.Property(e => e.ColExport).HasComment("1导出");

                entity.Property(e => e.ColField)
                    .HasComment("列键")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ColFormat)
                    .HasComment("格式化")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ColFrozen).HasComment("1冻结");

                entity.Property(e => e.ColHide).HasComment("1隐藏");

                entity.Property(e => e.ColOrder).HasComment("排序");

                entity.Property(e => e.ColQuery).HasComment("1查询");

                entity.Property(e => e.ColSort).HasComment("1启用点击排序");

                entity.Property(e => e.ColTitle)
                    .HasComment("列标题")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ColWidth).HasComment("列宽");

                entity.Property(e => e.DvTitle)
                    .HasComment("默认列标题")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.FormArea).HasComment("区域");

                entity.Property(e => e.FormHide).HasComment("1隐藏");

                entity.Property(e => e.FormOrder).HasComment("排序");

                entity.Property(e => e.FormPlaceholder)
                    .HasComment("输入框提示")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.FormRequired).HasComment("1必填");

                entity.Property(e => e.FormSpan).HasComment("跨列");

                entity.Property(e => e.FormText)
                    .HasComment("显示文本")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.FormType)
                    .HasComment("输入类型")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.FormUrl)
                    .HasComment("来源")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.FormValue)
                    .HasComment("初始值")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TableName)
                    .HasComment("（虚）表名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<TempInvoiceDetail>(entity =>
            {
                entity.HasKey(e => e.TidId)
                    .HasName("PRIMARY");

                entity.HasComment("单据明细");

                entity.Property(e => e.TidId)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.GoodsCost)
                    .HasPrecision(8, 2)
                    .HasComment("商品成本");

                entity.Property(e => e.GoodsCount).HasComment("商品数量");

                entity.Property(e => e.GoodsId)
                    .HasComment("商品ID")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.GoodsPrice)
                    .HasPrecision(8, 2)
                    .HasComment("商品售价");

                entity.Property(e => e.Spare1)
                    .HasComment("备用")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Spare2)
                    .HasComment("备用")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Spare3)
                    .HasComment("备用")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TidOrder).HasComment("排序");

                entity.Property(e => e.TimId)
                    .HasComment("单据主表ID")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TimNo)
                    .HasComment("单据号")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<TempInvoiceMain>(entity =>
            {
                entity.HasKey(e => e.TimId)
                    .HasName("PRIMARY");

                entity.HasComment("单据主表");

                entity.Property(e => e.TimId)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Spare1)
                    .HasComment("备用")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Spare2)
                    .HasComment("备用")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Spare3)
                    .HasComment("备用")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TimCreateTime).HasComment("创建时间");

                entity.Property(e => e.TimDate).HasComment("单据日期");

                entity.Property(e => e.TimNo)
                    .HasComment("单据号")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TimOwnerId)
                    .HasComment("制单人")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TimOwnerName)
                    .HasComment("制单人")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TimRemark)
                    .HasComment("备注")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TimStatus).HasComment("状态，1默认，2已审核，3未通过，4作废");

                entity.Property(e => e.TimStore)
                    .HasComment("门店")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TimSupplier)
                    .HasComment("供应商")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TimType).HasComment("采购类型");

                entity.Property(e => e.TimUser)
                    .HasComment("采购员")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}