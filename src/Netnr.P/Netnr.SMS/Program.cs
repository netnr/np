using Netnr;
using Netnr.SMS.Application;
using Netnr.SMS.Domain;

Console.Title = "Netnr SMS";
Console.WriteLine("\r\n  Start Netnr SMS , Enter \"exit\" stop\r\n");

//启动作业
FluentScheduler.JobManager.Initialize();
FluentScheduler.JobManager.AddJob(() =>
{
    var vm = WorkService.WorkAliyun().ToJson(true);
    Console.WriteLine($"\r\n{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    Console.WriteLine(vm);
    Netnr.Core.ConsoleTo.Log(vm);
}, s =>
{
    s.WithName("Job_Work_Aliyun");

    var ci = new ConfigInit();
    var ws = ci.GetValue<int>("Work:WaitSeconds");
    s.ToRunEvery(ws).Seconds();
});

//等待退出
while (Console.ReadLine() != "exit")
{

}