using System.Security.Cryptography;

[assembly: CollectionBehavior(
    CollectionBehavior.CollectionPerClass, 
    MaxParallelThreads = 1)
]
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
        await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
    }
    
    static async Task<byte[]> GoodMd5(Stream stream)
    {
        var md5 = HashAlgorithm.Create("md5");
        var buffer = new byte[65536];
        int length;

        while ((length = await stream.ReadAsync(buffer)) > 0)
        {
            //Task.Factory.StartNew()
            await Task.Run(() =>
            {
                if (length == 65536)
                {
                    md5.TransformBlock(buffer, 0, 65536, null, 0);
                }
                else
                {
                    md5.TransformFinalBlock(buffer, 0, length);
                }
            });
        }

        return md5.Hash;
    }
}