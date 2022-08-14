using Xunit;

namespace Netnr.Test
{
    public class TestNetnrBase
    {
        [Fact]
        public void Extensions_1_Json()
        {
            var vm = new ResultVM();
            var list = new List<ResultVM>()
            {
                new ResultVM(),
                new ResultVM()
            };

            var val1 = vm.ToJson();
            var val2 = list.ToJson();
            Assert.NotEmpty(val1);
            Assert.NotEmpty(val2);

            var jo2 = val1.DeJson<ResultVM>();
            var jo3 = val2.DeJson<List<ResultVM>>();
            Assert.IsType<ResultVM>(jo2);
            Assert.Equal(2, jo3.Count);
        }

        [Fact]
        public void TestTreeTo()
        {
            var list = new List<ResultVM>();
            //id:Code Pid:Data
            var vm1 = new ResultVM() { Code = 1, Msg = "一级", Data = 0 };
            var vm2 = new ResultVM() { Code = 11, Msg = "二级", Data = 1 };
            var vm3 = new ResultVM() { Code = 12, Msg = "二级", Data = 1 };
            var vm4 = new ResultVM() { Code = 121, Msg = "三级", Data = 12 };
            list.Add(vm1);
            list.Add(vm2);
            list.Add(vm3);
            list.Add(vm4);

            var result = TreeTo.ListToTree(list, "Data", "Code", new List<string> { "0" });
            Assert.NotEmpty(result);

            var nodes = TreeTo.FindToTree(list, "Data", "Code", new List<string> { "12" });
            Assert.Single(nodes);
        }
    }
}
