#if Full || Base

namespace Netnr;

/// <summary>
/// 程序管理脚本
/// </summary>
public partial class PMScriptTo
{
    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        if (BaseTo.CommandLineArgs.Contains("--zz"))
        {
            var clKeys = BaseTo.CommandLineKeys();
            var platfrom = clKeys.FirstOrDefault(x => x.Key == "--zz").Value;
            var urls = clKeys.FirstOrDefault(x => x.Key == "--urls").Value;
            GeneratePMScript(platfrom, urls);

            Environment.Exit(0);
        }
    }

    /// <summary>
    /// 生成脚本
    /// </summary>
    /// <param name="platform"></param>
    /// <param name="urls"></param>
    public static void GeneratePMScript(string platform, string urls)
    {
        if (string.IsNullOrWhiteSpace(platform))
        {
            platform = CmdTo.IsWindows ? "win" : "linux";
        }

        string processName = AppDomain.CurrentDomain.FriendlyName;
        string processPath = AppDomain.CurrentDomain.BaseDirectory;
        var processPort = urls?.Split(':').Last().TrimEnd('/');

        string scriptAction = "runlater|start|stop|restart|status";
        string scriptContent;
        string ext;
        var qmark = '"';

        if (platform.StartsWith("win"))
        {
            ext = "bat";
            processName += ".exe";
            scriptAction = scriptAction.Replace("|", "^|");

            string processWait = "timeout /t 2 >nul";
            string processStart = $"start /D {qmark}{processPath}{qmark} {processName}";
            string processStatus = $"echo. && tasklist | findstr \"{processName}\"";
            if (!string.IsNullOrWhiteSpace(urls))
            {
                processStart += $" --urls {qmark}{urls}{qmark}";
                processStatus += $" && echo. && netstat -ano | findstr LISTEN | findstr \":{processPort}\"";
            }

            scriptContent = $"""
@echo off
set choice=%1
if "%choice%"=="" (
    goto ask
)
goto start

:ask
set /p choice=Choose an option ({scriptAction}): 

:start
if "%choice%"=="runlater" (
    echo Start program after 30 seconds
    timeout /t 30 >nul
    {processStart}
    {processWait}
    {processStatus}
) else if "%choice%"=="start" (
    echo Starting the program...
    {processStart}
    {processWait}
    {processStatus}
) else if "%choice%"=="stop" (
    echo Stopping the program...
    taskkill /F /IM "{processName}"
    {processStatus}
) else if "%choice%"=="restart" (
    echo Restarting the program...
    {processStatus}
    taskkill /F /IM "{processName}"
    {processWait}
    {processStatus}
    {processStart}
    {processWait}
    {processStatus}
) else if "%choice%"=="status" (
    echo Program status
    {processStatus}
) else (
    echo Invalid argument. Usage: %~nx0 {scriptAction}
)

""";
        }
        else
        {
            ext = "sh";
            processPath = processPath.Replace('\\', '/');

            string processWait = "sleep 2";
            string processStart = $"cd {qmark}{processPath}{qmark} && chmod +x {processName} && nohup ./{processName}";
            string processStatus = $"echo && ps aux | grep \"{processName}\"";
            if (!string.IsNullOrWhiteSpace(urls))
            {
                processStart += $" --urls {qmark}{urls}{qmark}";
                processStatus += $" && echo && netstat -tunlp | grep \":{processPort}\"";
            }
            processStart += $" >> ./nohup.log 2>&1 &";

            string processStop = string.IsNullOrWhiteSpace(urls) ?
                $"kill $(pidof {processName})" :
                $"kill $(lsof -t -i:{processPort})";

            var bb = "#!/bin/bash";

            scriptContent = $"""
{bb}

if [ -z "$1" ]
then
    echo "Choose an option ({scriptAction}):"
    read option
else
    option=$1
fi

case $option in
    runlater)
        echo "Start program after 30 seconds"
        sleep 30
        {processStart}
        {processWait}
        {processStatus}
        ;;
    start)
        echo "Starting the program..."
        {processStart}
        {processWait}
        {processStatus}
        ;;
    stop)
        echo "Stopping the program..."
        {processStop}
        {processWait}
        {processStatus}
        ;;
    restart)
        echo "Restarting the program..."
        {processStatus}
        {processStop}
        {processWait}
        {processStart}
        {processWait}
        {processStatus}
        ;;
    status)
        echo "Program status"
        {processStatus}
        ;;
    *)
        echo "Invalid argument. Usage: $0 {scriptAction}"
        exit 1
esac

""";
            scriptContent = scriptContent.Replace("\r\n", "\n");
        }

        var path = Path.Combine(processPath, $"zz.{ext}");
        File.WriteAllText(path, scriptContent);
        ConsoleTo.WriteCard(nameof(GeneratePMScript), path);

        if (!CmdTo.IsWindows)
        {
            CmdTo.Execute($"chmod +x {path}");
        }
    }
}

#endif