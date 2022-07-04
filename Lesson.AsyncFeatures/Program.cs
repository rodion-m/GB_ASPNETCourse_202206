// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using Lesson.AsyncFeatures;

await Task.Factory.StartNew(async () =>
    {
        Console.WriteLine(Environment.CurrentManagedThreadId);
        Console.WriteLine(Thread.CurrentThread.IsThreadPoolThread); //False
        var lines = await File.ReadAllBytesAsync(@"C:\Users\rodio\YandexDisk\Уроки\SP 2022\Files\1.txt");
        Console.WriteLine(Environment.CurrentManagedThreadId);
        Console.WriteLine(Thread.CurrentThread.IsThreadPoolThread); //True
        for (var i = 0; i < lines.Length; i++)
        {
            lines[i] = (byte)(lines[i] ^ 10);
        }
        Console.WriteLine("Done");
    }, TaskCreationOptions.LongRunning);

return;

//ThreadPool.SetMinThreads(100, 100);

RealThreadsCountDemo.Run(true);

return;

TaskScheduler.UnobservedTaskException += (sender, eventArgs) => Console.WriteLine(eventArgs.Exception);

{
    _ = Throw();
}
// Thread.Sleep(1000);
// GC.Collect();
// GC.WaitForFullGCComplete();
// GC.WaitForPendingFinalizers();
Thread.Sleep(10000);

async Task<string> Throw()
{
    await Task.Delay(100);
    throw new Exception("EXCEPTION");
    return "asd";
}

return;

Console.WriteLine("Starting");

//ThreadPool.SetMinThreads(1000, 1000);
await RealThreadsCountDemo.RunAsync();

Console.WriteLine("Done");
