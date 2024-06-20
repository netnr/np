using Xunit;

namespace Netnr.Test
{
    public class TestNetnrCompile
    {
        [Fact]
        public void Demo()
        {
            var code = @"
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class DynamicCompile
{
    public void Main()
    {
        Console.WriteLine(DateTime.Now);
        Console.WriteLine(Environment.OSVersion);
        Console.WriteLine(Environment.SystemDirectory);
        Console.WriteLine(Environment.Version);
        Console.WriteLine(RuntimeInformation.FrameworkDescription);
        Console.WriteLine(RuntimeInformation.OSDescription);
    }
}
";
            var ce = CompileTo.Executing(code, "System.Runtime.InteropServices.RuntimeInformation.dll".Split(",")).Split(Environment.NewLine);
            foreach (var item in ce)
            {
                Debug.WriteLine(item);
            }
            Assert.True(ce.Length > 0);
        }
    }
}
