using Xunit;

namespace Netnr.Test
{
    public class TestLanguage
    {
        /// <summary>
        /// C# 8 Range 和 Index（范围和索引）
        /// </summary>
        [Fact]
        public void Csharp8RangeAndIndex()
        {
            var v1 = "123456789";

            Assert.Equal("789", v1[6..]);
            Assert.Equal("789", v1.Substring(6));

            Assert.Equal("456", v1[3..6]);
            Assert.Equal("456", v1.Substring(3, 3));

            Assert.Equal("123", v1[..3]);
            Assert.Equal("123", v1.Substring(0, 3));

            Assert.Equal("12345678", v1[..^1]);
            Assert.Equal("12345678", v1.Substring(0, v1.Length - 1));

            Assert.Equal("2345678", v1[1..^1]);
            Assert.Equal("2345678", v1.Substring(1, v1.Length - 2));
        }
    }
}
