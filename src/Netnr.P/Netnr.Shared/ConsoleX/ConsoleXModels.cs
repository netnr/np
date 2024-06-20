#if Full || ConsoleX

using System.Reflection;

namespace Netnr;

/// <summary>
/// 方法对象
/// </summary>
public class ConsoleXMethodModel
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
            list.Add($"[{GroupName}]");
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
            var result = $"{Action} {(Description.Length > 50 ? "\r\n" : "")}### {Description}\r\nformat: {ShortName}\r\ndemo: {string.Join("\r\ndemo: ", Prompt.Split(Environment.NewLine))}\r\n";
            ConsoleTo.LogColor(result);
        }
    }
}

#endif