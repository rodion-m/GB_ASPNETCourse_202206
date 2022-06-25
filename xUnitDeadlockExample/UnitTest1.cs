[assembly: CollectionBehavior(CollectionBehavior.CollectionPerClass, MaxParallelThreads = 1)]
//See: MaxConcurrencySyncContext in Xunit.Sdk
//https://github.com/xunit/xunit/blob/main/src/xunit.v3.core/Sdk/MaxConcurrencySyncContext.cs

namespace xUnitDeadlockExample;

public class UnitTest1
{
    [Fact]
    public void TestDeadlock1()
    {
        WaitOneSec().Wait();
    }
    
    private async Task WaitOneSec()
    {
        await Task.Delay(TimeSpan.FromSeconds(3));
    }
}