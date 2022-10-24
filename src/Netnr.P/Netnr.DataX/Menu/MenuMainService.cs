namespace Netnr.DataX.Menu;

/// <summary>
/// 主菜单
/// </summary>
public partial class MenuMainService
{
    [Display(Name = "Exit", Description = "退出")]
    public static void Exit() => Environment.Exit(0);

    [Display(Name = "Mix", Description = "综合", GroupName = "\r\n")]
    public static void Mix() => DXService.InvokeMenu(typeof(MenuMixService), false);

    [Display(Name = "Data", Description = "数据", GroupName = "\r\n")]
    public static void Data() => DXService.InvokeMenu(typeof(MenuDataService), false);

    [Display(Name = "Silent", Description = "静默", GroupName = "\r\n")]
    public static void Silent() => DXService.InvokeMenu(typeof(MenuSilenceService), false);
}