// See https://aka.ms/new-console-template for more information

using Lesson.AsyncFeatures;

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
