using Aspose.Words;
using Xunit;

namespace Netnr.Test
{
    public class TestAspose
    {
        [Fact]
        public void DocToDocx()
        {
            var path = @"D:\work\nanan\with\不假外出\陈伟_500237200002287892.doc";

            // 加载 DOC 文件
            var doc = new Document(path);

            // 将 DOC 文件保存为 DOCX 格式
            doc.Save(@"D:\tmp\na.docx", SaveFormat.Docx);
        }
    }
}