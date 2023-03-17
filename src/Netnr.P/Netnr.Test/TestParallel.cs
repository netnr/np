using Xunit;

namespace Netnr.Test
{
    public class TestParallel
    {
        public List<int> list = Enumerable.Range(0, 999).ToList();

        [Fact]
        public void For_1()
        {
            Debug.WriteLine("For_1 start");
            Parallel.For(0, list.Count, i =>
            {
                Debug.WriteLine($"item: {list[i]}, index: {i}, task: {Task.CurrentId}");
            });
            Debug.WriteLine("For_1 end");
        }

        [Fact]
        public void ForEach_2()
        {
            Debug.WriteLine("ForEach_2 start");
            Parallel.ForEach(list, item =>
            {
                Console.WriteLine(item);
                Debug.WriteLine($"item: {item}, task: {Task.CurrentId}");
            });
            Debug.WriteLine("ForEach_2 end");
        }

        [Fact]
        public async void ForEachAsync_3()
        {
            Debug.WriteLine("ForEachAsync start");
            await Parallel.ForEachAsync(list, async (item, token) =>
            {
                await Task.Delay(RandomTo.Instance.Next(10, 500), token);
                Debug.WriteLine($"item: {item}");
            });
            Debug.WriteLine("ForEachAsync end");
        }
    }
}