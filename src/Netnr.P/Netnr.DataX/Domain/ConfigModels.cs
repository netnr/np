using System.Reflection;

namespace Netnr.DataX.Domain;

/// <summary>
/// 配置初始化
/// </summary>
public class ConfigInit
{
    /// <summary>
    /// 构造
    /// </summary>
    public ConfigInit()
    {
        DXHub = Path.Combine(BaseTo.ProjectRootPath, "ud/hub");
        if (!Directory.Exists(DXHub))
        {
            Directory.CreateDirectory(DXHub);
        }

        var configPath = Path.Combine(BaseTo.ProjectRootPath, "ud/config.json");
        if (File.Exists(configPath))
        {
            DXConfig = File.ReadAllText(configPath).DeJson<ConfigOption>();

            //设置连接对象以深拷贝构建新实例
            DXConfig.ListConnectionInfo.ForEach(item => item.DeepCopyNewInstance = true);
        }
    }

    /// <summary>
    /// 简称
    /// </summary>
    public const string ShortName = "NDX";

    /// <summary>
    /// 枢纽
    /// </summary>
    public string DXHub { get; set; }

    public ConfigOption DXConfig { get; set; }
}

/// <summary>
/// 配置
/// </summary>
public class ConfigOption
{
    /// <summary>
    /// 映射匹配模式（读写 表、列）：Same（相同） Similar（相似）
    /// </summary>
    public string MapingMatchPattern { get; set; }

    /// <summary>
    /// 数据库连接信息
    /// </summary>
    public List<DbKitConnectionOption> ListConnectionInfo { get; set; }

    /// <summary>
    /// 作业
    /// </summary>
    public JsonNode Works { get; set; }
}

/// <summary>
/// 方法对象
/// </summary>
public class MethodModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string GroupName { get; set; }
    public string NewLine { get; set; } = "";
    public string ShortName { get; set; }
    public string Action { get; set; }
    public string Prompt { get; set; }
    public MethodInfo Method { get; set; }

    /// <summary>
    /// 显示名称
    /// </summary>
    /// <returns></returns>
    public string GetViewName()
    {
        var list = new List<string>();

        if (!string.IsNullOrWhiteSpace(GroupName))
        {
            list.Add(GroupName);
        }
        if (!string.IsNullOrWhiteSpace(Action))
        {
            list.Add($"<{Action}>");
        }
        if (!string.IsNullOrWhiteSpace(Name))
        {
            list.Add(Name);
        }
        if (!string.IsNullOrWhiteSpace(Description))
        {
            list.Add(Description);
        }

        return string.Join(" ", list) + NewLine;
    }

    /// <summary>
    /// 运行提示
    /// </summary>
    /// <returns></returns>
    public void GetRunPrompt()
    {
        if (!string.IsNullOrWhiteSpace(Action))
        {
            var result = $"ndx {Action} {(Description.Length > 50 ? "\r\n" : "")}### {Description}\r\nformat: ndx {ShortName}\r\ndemo: {string.Join("\r\ndemo: ", Prompt.Split(Environment.NewLine))}\r\n";
            DXService.Log(result);
        }
    }
}