#if Full || ConsoleX

using System;
using System.Reflection;

namespace Netnr;

/// <summary>
/// About
/// </summary>
public partial class MenuItemService
{
    [Display(Name = "Exit", Description = "退出", GroupName = "About", AutoGenerateFilter = true)]
    public static void Exit() => Environment.Exit(0);

    [Display(Name = "View version", Description = "查看版本", GroupName = "About",
        ShortName = "version", Prompt = "version")]
    public static void ViewVersion()
    {
        ConsoleTo.LogColor(BaseTo.Version);

        var assembly = Assembly.GetEntryAssembly();
        var copyAttrName = nameof(AssemblyCopyrightAttribute);
        var copyInfo = assembly.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == copyAttrName)?.ConstructorArguments.FirstOrDefault().Value?.ToString();
        if (!string.IsNullOrWhiteSpace(copyInfo))
        {
            ConsoleTo.LogColor(copyInfo);
        }
    }

    [Display(Name = "Console encoding", Description = "控制台编码", GroupName = "About")]
    public static void ConsoleEncoding()
    {
        ConsoleTo.LogColor($"Current: {Console.OutputEncoding.EncodingName}");

        //选择编码
        var cri2 = ConsoleXTo.ConsoleReadItem("Choose an encoding", "Unicode,UTF-8".Split(','), 1);
        switch (cri2)
        {
            case 1: Console.OutputEncoding = Encoding.Unicode; break;
            case 2: Console.OutputEncoding = Encoding.UTF8; break;
        }
    }

    [Display(Name = "GC", Description = "清理", GroupName = "About")]
    public static async Task GarbageCleanup()
    {
        ConsoleTo.LogColor($"Use Physical Memory: {ParsingTo.FormatByte(Environment.WorkingSet)}");
        GC.Collect();
        await Task.Delay(1000);
        GC.Collect();
        ConsoleTo.LogColor($"Use Physical Memory: {ParsingTo.FormatByte(Environment.WorkingSet)}");
    }

    [Display(Name = "Open Base Directory", Description = "打开根目录", GroupName = "About", AutoGenerateFilter = true,
        ShortName = "basedir", Prompt = "basedir")]
    public static void OpenBaseDirectory()
    {
        ConsoleTo.LogColor($"\r\n{AppContext.BaseDirectory}\r\n");

        if (CmdTo.IsWindows)
        {
            CmdTo.Execute($"start {AppContext.BaseDirectory}");
        }
        else
        {
            var cr = CmdTo.Execute($"ls {AppContext.BaseDirectory} -lh");
            ConsoleTo.LogColor(cr.CrOutput);
        }
    }

    [Display(Name = "Try Color", Description = "颜色", GroupName = "About")]
    public static void TryColor()
    {
        var values = Enum.GetValues(typeof(ConsoleColor));
        foreach (ConsoleColor value in values)
        {
            ConsoleTo.LogColor($"{value}", value);
        }
    }

    [Display(Name = "Try Directory", Description = "路径信息", GroupName = "About")]
    public static void TryDirectory()
    {
        ConsoleTo.LogColor($"应用程序位置");
        ConsoleTo.LogColor($"BaseTo.{nameof(BaseTo.ProjectPath)}: {BaseTo.ProjectPath}");
        ConsoleTo.LogColor($"AppContext.BaseDirectory: {AppContext.BaseDirectory}");
        ConsoleTo.LogColor($"Environment.ProcessPath: {Environment.ProcessPath}");
        ConsoleTo.LogColor($"");

        ConsoleTo.LogColor($"命令行位置");
        ConsoleTo.LogColor($"Environment.CurrentDirectory: {Environment.CurrentDirectory}");
        ConsoleTo.LogColor($"Directory.GetCurrentDirectory(): {Directory.GetCurrentDirectory()}");
        ConsoleTo.LogColor($"");

        ConsoleTo.LogColor($"Environment.GetFolderPath(Environment.SpecialFolder.*)");
        foreach (Environment.SpecialFolder folder in Enum.GetValues(typeof(Environment.SpecialFolder)))
        {
            ConsoleColor? cc = null;
            if (folder == Environment.SpecialFolder.UserProfile)
            {
                cc = ConsoleColor.Cyan;
            }

            var path = Environment.GetFolderPath(folder);
            if (CmdTo.IsWindows || !string.IsNullOrEmpty(path))
            {
                ConsoleTo.LogColor($"{folder}: {path}", cc);
            }
        }
    }

    [Display(Name = "Try Assembly", Description = "程序集", GroupName = "About")]
    public static void TryAssembly()
    {
        // 获取当前程序集的信息
        var assembly = Assembly.GetEntryAssembly();

        var dict = new Dictionary<string, IEnumerable<object>>();
        assembly.CustomAttributes.ForEach(attr =>
        {
            dict.TryAdd(attr.AttributeType.Name, attr.ConstructorArguments.Select(x => x.Value));
        });
        ConsoleTo.LogColor(dict.ToJson(true));
    }

    [Display(Name = "Try Tmp", Description = "临时", GroupName = "About")]
    public static void TryTmp()
    {

    }

}

#endif