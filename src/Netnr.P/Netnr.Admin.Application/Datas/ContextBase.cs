using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Netnr.Admin.Domain.Entities;

namespace Netnr.Admin.Application.Datas
{
    public partial class ContextBase : DbContext
    {
        public ContextBase(DbContextOptions<ContextBase> options)
            : base(options)
        {
        }

        public virtual DbSet<BaseButton> BaseButton { get; set; }
        public virtual DbSet<BaseDictionary> BaseDictionary { get; set; }
        public virtual DbSet<BaseLog> BaseLog { get; set; }
        public virtual DbSet<BaseMenu> BaseMenu { get; set; }
        public virtual DbSet<BaseOrg> BaseOrg { get; set; }
        public virtual DbSet<BaseRole> BaseRole { get; set; }
        public virtual DbSet<BaseUser> BaseUser { get; set; }
        public virtual DbSet<WorkInstance> WorkInstance { get; set; }
        public virtual DbSet<WorkInstanceData> WorkInstanceData { get; set; }
        public virtual DbSet<WorkInstanceStep> WorkInstanceStep { get; set; }
        public virtual DbSet<WorkNode> WorkNode { get; set; }
        public virtual DbSet<WorkNodeForm> WorkNodeForm { get; set; }
        public virtual DbSet<WorkTemplate> WorkTemplate { get; set; }
        public virtual DbSet<WorkTemplateNode> WorkTemplateNode { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaseButton>(entity =>
            {
                entity.HasKey(e => e.ButtonId)
                    .HasName("PK_BASE_BUTTON");

                entity.ToTable("base_button");

                entity.HasComment("按钮");

                entity.Property(e => e.ButtonId)
                    .ValueGeneratedNever()
                    .HasColumnName("button_id")
                    .HasComment("主键，唯一");

                entity.Property(e => e.ButtonClass)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("button_class")
                    .HasComment("按钮类名");

                entity.Property(e => e.ButtonGroup)
                    .HasColumnName("button_group")
                    .HasComment("按钮分组（1：默认）");

                entity.Property(e => e.ButtonIcon)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("button_icon")
                    .HasComment("按钮图标");

                entity.Property(e => e.ButtonKey)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("button_key")
                    .HasComment("按钮标识");

                entity.Property(e => e.ButtonOrder)
                    .HasColumnName("button_order")
                    .HasComment("按钮排序");

                entity.Property(e => e.ButtonRemark)
                    .HasMaxLength(200)
                    .HasColumnName("button_remark")
                    .HasComment("按钮备注");

                entity.Property(e => e.ButtonText)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("button_text")
                    .HasComment("按钮文本");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("状态（1：启用；0：停用）");
            });

            modelBuilder.Entity<BaseDictionary>(entity =>
            {
                entity.HasKey(e => e.DictId)
                    .HasName("PK_BASE_DICTIONARY");

                entity.ToTable("base_dictionary");

                entity.HasComment("字典");

                entity.Property(e => e.DictId)
                    .ValueGeneratedNever()
                    .HasColumnName("dict_id")
                    .HasComment("字典ID，唯一");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.DictKey)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("dict_key")
                    .HasComment("字典键");

                entity.Property(e => e.DictOrder)
                    .HasColumnName("dict_order")
                    .HasComment("字典排序");

                entity.Property(e => e.DictPid)
                    .HasColumnName("dict_pid")
                    .HasComment("字典父级ID，无父级为0");

                entity.Property(e => e.DictRemark)
                    .HasMaxLength(200)
                    .HasColumnName("dict_remark")
                    .HasComment("字典备注");

                entity.Property(e => e.DictType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("dict_type")
                    .HasComment("字典类别");

                entity.Property(e => e.DictValue)
                    .HasMaxLength(50)
                    .HasColumnName("dict_value")
                    .HasComment("字典值");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("状态（1：启用；0：停用）");
            });

            modelBuilder.Entity<BaseLog>(entity =>
            {
                entity.HasKey(e => e.LogId)
                    .HasName("PK_BASE_LOG");

                entity.ToTable("base_log");

                entity.HasComment("日志");

                entity.Property(e => e.LogId)
                    .ValueGeneratedNever()
                    .HasColumnName("log_id")
                    .HasComment("日志ID，唯一");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.LogAction)
                    .HasMaxLength(50)
                    .HasColumnName("log_action")
                    .HasComment("动作");

                entity.Property(e => e.LogBrowser)
                    .HasMaxLength(50)
                    .HasColumnName("log_browser")
                    .HasComment("浏览器");

                entity.Property(e => e.LogContent)
                    .HasMaxLength(4000)
                    .HasColumnName("log_content")
                    .HasComment("内容");

                entity.Property(e => e.LogIp)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("log_ip")
                    .HasComment("IP");

                entity.Property(e => e.LogNickname)
                    .HasMaxLength(50)
                    .HasColumnName("log_nickname")
                    .HasComment("昵称");

                entity.Property(e => e.LogRemark)
                    .HasMaxLength(200)
                    .HasColumnName("log_remark")
                    .HasComment("备注");

                entity.Property(e => e.LogSql)
                    .HasMaxLength(1000)
                    .HasColumnName("log_sql")
                    .HasComment("执行 SQL");

                entity.Property(e => e.LogSystem)
                    .HasMaxLength(50)
                    .HasColumnName("log_system")
                    .HasComment("系统");

                entity.Property(e => e.LogTimeCost)
                    .HasColumnName("log_time_cost")
                    .HasComment("用时（毫秒）");

                entity.Property(e => e.LogType)
                    .HasColumnName("log_type")
                    .HasComment("类型（-1：异常；1：默认；2：登录；）");

                entity.Property(e => e.LogUrl)
                    .HasMaxLength(4000)
                    .HasColumnName("log_url")
                    .HasComment("链接");

                entity.Property(e => e.LogUser)
                    .HasMaxLength(50)
                    .HasColumnName("log_user")
                    .HasComment("关联用户");

                entity.Property(e => e.LogUserAgent)
                    .HasMaxLength(500)
                    .HasColumnName("log_user_agent")
                    .HasComment("User-Agent");
            });

            modelBuilder.Entity<BaseMenu>(entity =>
            {
                entity.HasKey(e => e.MenuId)
                    .HasName("PK_BASE_MENU");

                entity.ToTable("base_menu");

                entity.HasComment("菜单");

                entity.Property(e => e.MenuId)
                    .ValueGeneratedNever()
                    .HasColumnName("menu_id")
                    .HasComment("菜单ID，唯一");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.MenuGroup)
                    .HasColumnName("menu_group")
                    .HasComment("菜单分组（1：PC；2：Mobile）");

                entity.Property(e => e.MenuIcon)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("menu_icon")
                    .HasComment("菜单图标");

                entity.Property(e => e.MenuName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("menu_name")
                    .HasComment("菜单名称");

                entity.Property(e => e.MenuOrder)
                    .HasColumnName("menu_order")
                    .HasComment("菜单排序");

                entity.Property(e => e.MenuPid)
                    .HasColumnName("menu_pid")
                    .HasComment("菜单父级ID，无父级为0");

                entity.Property(e => e.MenuRemark)
                    .HasMaxLength(200)
                    .HasColumnName("menu_remark")
                    .HasComment("菜单备注");

                entity.Property(e => e.MenuUrl)
                    .HasMaxLength(200)
                    .HasColumnName("menu_url")
                    .HasComment("菜单链接");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("状态（1：启用；0：停用）");
            });

            modelBuilder.Entity<BaseOrg>(entity =>
            {
                entity.HasKey(e => e.OrgId)
                    .HasName("PK_BASE_ORG");

                entity.ToTable("base_org");

                entity.HasComment("组织架构");

                entity.Property(e => e.OrgId)
                    .ValueGeneratedNever()
                    .HasColumnName("org_id")
                    .HasComment("组织架构ID，唯一");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.OrgCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("org_code")
                    .HasComment("组织代码");

                entity.Property(e => e.OrgName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("org_name")
                    .HasComment("组织名称");

                entity.Property(e => e.OrgPid)
                    .HasColumnName("org_pid")
                    .HasComment("组织架构父级ID（无父级为0）");

                entity.Property(e => e.OrgRemark)
                    .HasMaxLength(200)
                    .HasColumnName("org_remark")
                    .HasComment("组织备注");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("状态（1：启用；0：停用）");
            });

            modelBuilder.Entity<BaseRole>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .HasName("PK_BASE_ROLE");

                entity.ToTable("base_role");

                entity.HasComment("用户角色");

                entity.Property(e => e.RoleId)
                    .ValueGeneratedNever()
                    .HasColumnName("role_id")
                    .HasComment("角色ID，唯一");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.RoleButtons)
                    .HasMaxLength(2000)
                    .IsUnicode(false)
                    .HasColumnName("role_buttons")
                    .HasComment("关联按钮（JSON）");

                entity.Property(e => e.RoleGroup)
                    .HasColumnName("role_group")
                    .HasComment("角色分组（默认1）");

                entity.Property(e => e.RoleMenus)
                    .HasMaxLength(2000)
                    .IsUnicode(false)
                    .HasColumnName("role_menus")
                    .HasComment("关联菜单（逗号分割）");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("role_name")
                    .HasComment("角色名称");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("状态（1：启用；0：停用）");
            });

            modelBuilder.Entity<BaseUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_BASE_USER");

                entity.ToTable("base_user");

                entity.HasComment("用户信息");

                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasColumnName("user_id")
                    .HasComment("用户ID，唯一");

                entity.Property(e => e.Account)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("account")
                    .HasComment("账号，唯一");

                entity.Property(e => e.ActualName)
                    .HasMaxLength(50)
                    .HasColumnName("actual_name")
                    .HasComment("真实姓名");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email")
                    .HasComment("邮箱");

                entity.Property(e => e.MobilePhone)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("mobile_phone")
                    .HasComment("手机号码");

                entity.Property(e => e.Nickname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("nickname")
                    .HasComment("昵称");

                entity.Property(e => e.OrgId)
                    .HasColumnName("org_id")
                    .HasComment("关联组织架构ID");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password")
                    .HasComment("密码");

                entity.Property(e => e.RolesId)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("roles_id")
                    .HasComment("关联角色ID（多选逗号分割）");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("状态（1：正常；0：停用；2：禁止登录）");
            });

            modelBuilder.Entity<WorkInstance>(entity =>
            {
                entity.HasKey(e => e.InstanceId)
                    .HasName("PK_WORK_INSTANCE");

                entity.ToTable("work_instance");

                entity.HasComment("工作流程实例");

                entity.Property(e => e.InstanceId)
                    .ValueGeneratedNever()
                    .HasColumnName("instance_id")
                    .HasComment("实例ID，唯一");

                entity.Property(e => e.BeginTime)
                    .HasColumnType("datetime")
                    .HasColumnName("begin_time")
                    .HasComment("开始时间");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.EndTime)
                    .HasColumnType("datetime")
                    .HasColumnName("end_time")
                    .HasComment("结束时间");

                entity.Property(e => e.InstanceRemark)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("instance_remark")
                    .HasComment("实例备注");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("状态（）");

                entity.Property(e => e.TemplateId)
                    .HasColumnName("template_id")
                    .HasComment("关联模版ID");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasComment("发起人");
            });

            modelBuilder.Entity<WorkInstanceData>(entity =>
            {
                entity.HasKey(e => e.DataId)
                    .HasName("PK_WORK_INSTANCE_DATA");

                entity.ToTable("work_instance_data");

                entity.HasComment("工作流程实例数据");

                entity.Property(e => e.DataId)
                    .ValueGeneratedNever()
                    .HasColumnName("data_id")
                    .HasComment("数据ID，唯一");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.DataValue)
                    .HasMaxLength(200)
                    .HasColumnName("data_value")
                    .HasComment("表单值");

                entity.Property(e => e.FormId)
                    .HasColumnName("form_id")
                    .HasComment("关联表单ID");

                entity.Property(e => e.InstanceId)
                    .HasColumnName("instance_id")
                    .HasComment("关联实例ID");

                entity.Property(e => e.NodeId)
                    .HasColumnName("node_id")
                    .HasComment("关联节点ID");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("状态（）");

                entity.Property(e => e.StepId)
                    .HasColumnName("step_id")
                    .HasComment("关联步骤ID");

                entity.Property(e => e.TemplateId)
                    .HasColumnName("template_id")
                    .HasComment("关联模版ID");

                entity.Property(e => e.TnodeId)
                    .HasColumnName("tnode_id")
                    .HasComment("关联模板节点ID");
            });

            modelBuilder.Entity<WorkInstanceStep>(entity =>
            {
                entity.HasKey(e => e.StepId)
                    .HasName("PK_WORK_INSTANCE_STEP");

                entity.ToTable("work_instance_step");

                entity.HasComment("工作流程实例步骤");

                entity.Property(e => e.StepId)
                    .ValueGeneratedNever()
                    .HasColumnName("step_id")
                    .HasComment("步骤ID，唯一");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.InstanceId)
                    .HasColumnName("instance_id")
                    .HasComment("关联实例ID");

                entity.Property(e => e.NodeId)
                    .HasColumnName("node_id")
                    .HasComment("关联节点ID");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("状态（）");

                entity.Property(e => e.TemplateId)
                    .HasColumnName("template_id")
                    .HasComment("关联模版ID");

                entity.Property(e => e.TnodeId)
                    .HasColumnName("tnode_id")
                    .HasComment("关联模板节点ID");
            });

            modelBuilder.Entity<WorkNode>(entity =>
            {
                entity.HasKey(e => e.NodeId)
                    .HasName("PK_WORK_NODE");

                entity.ToTable("work_node");

                entity.HasComment("工作流程节点");

                entity.Property(e => e.NodeId)
                    .ValueGeneratedNever()
                    .HasColumnName("node_id")
                    .HasComment("节点ID，唯一");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.NodeName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("node_name")
                    .HasComment("节点名称");

                entity.Property(e => e.NodeType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("node_type")
                    .HasComment("节点类型（关联字典 work_node_type）");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("状态（1：启用；0：停用）");
            });

            modelBuilder.Entity<WorkNodeForm>(entity =>
            {
                entity.HasKey(e => e.FormId)
                    .HasName("PK_WORK_NODE_FORM");

                entity.ToTable("work_node_form");

                entity.HasComment("工作流程节点表单");

                entity.Property(e => e.FormId)
                    .ValueGeneratedNever()
                    .HasColumnName("form_id")
                    .HasComment("表单ID，唯一");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.FormKey)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("form_key")
                    .HasComment("表单KEY");

                entity.Property(e => e.FormName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("form_name")
                    .HasComment("表单名称");

                entity.Property(e => e.FormOrder)
                    .HasColumnName("form_order")
                    .HasComment("表单排序");

                entity.Property(e => e.FormRemark)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("form_remark")
                    .HasComment("表单备注");

                entity.Property(e => e.FormType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("form_type")
                    .HasComment("表单类型");

                entity.Property(e => e.NodeId)
                    .HasColumnName("node_id")
                    .HasComment("关联节点ID");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("状态（1：启用；0：停用）");
            });

            modelBuilder.Entity<WorkTemplate>(entity =>
            {
                entity.HasKey(e => e.TemplateId)
                    .HasName("PK_WORK_TEMPLATE");

                entity.ToTable("work_template");

                entity.HasComment("工作流程模板");

                entity.Property(e => e.TemplateId)
                    .ValueGeneratedNever()
                    .HasColumnName("template_id")
                    .HasComment("模板ID，唯一");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("状态（1：启用；0：停用）");

                entity.Property(e => e.TemplateName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("template_name")
                    .HasComment("模板名称");

                entity.Property(e => e.TemplateType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("template_type")
                    .HasComment("模版类型（关联字典 work_template_type）");
            });

            modelBuilder.Entity<WorkTemplateNode>(entity =>
            {
                entity.HasKey(e => e.TnodeId)
                    .HasName("PK_WORK_TEMPLATE_NODE");

                entity.ToTable("work_template_node");

                entity.HasComment("工作流程模版节点");

                entity.Property(e => e.TnodeId)
                    .ValueGeneratedNever()
                    .HasColumnName("tnode_id")
                    .HasComment("模板节点ID，唯一");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.NodeId)
                    .HasColumnName("node_id")
                    .HasComment("关联节点ID");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("状态（1：启用；0：停用）");

                entity.Property(e => e.TemplateId)
                    .HasColumnName("template_id")
                    .HasComment("关联模板ID");

                entity.Property(e => e.TnodeName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("tnode_name")
                    .HasComment("模板节点名称（默认继承节点名称）");

                entity.Property(e => e.TnodeNextIds)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("tnode_next_ids")
                    .HasComment("下一模版节点ID，多选");

                entity.Property(e => e.TnodePerson)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("tnode_person")
                    .HasComment("模版节点关联人ID");

                entity.Property(e => e.TnodeRequest)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("tnode_request")
                    .HasComment("模版节点要求");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
