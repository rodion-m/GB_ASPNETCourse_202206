using System.Collections.Concurrent;

namespace HW2_ConcurrentList.Test;

public class UnitTest1
{
    [Fact]
    public void TestQueuedConcurrentList()
    {
        var listQ = new ConcurrentListQ<int>();
        Parallel.For(0, 1_000_000, (i) =>
        {
            listQ.Add(i);
            //values.Add(i);
        });
    }
}