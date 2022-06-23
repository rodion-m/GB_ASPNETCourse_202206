using System.Diagnostics;

namespace Lesson.AsyncFeatures;

public class ThreadPoolStarvationDemo
{
    public static void Run()
    {
        var tasks = new List<Task>();
        for (int i = 0; i < 101; i++)
        {
            var current = i;
            tasks.Add(RunSyncOverAsyncTask(current));
            //tasks.Add(RunAsyncTask(current));
            //Thread.Sleep(10);
        }

        //Task.WaitAll(tasks.ToArray());
        var sw = Stopwatch.StartNew();
        Task.Run(() => Console.WriteLine(sw.ElapsedMilliseconds)).Wait();
    }

    private static Task RunAsyncTask(int current)
    {
        return Task.Run(async () =>
        {
            if(current % 10 == 0) PrintThreadsInfo();
            await Task.Delay(1000);
        });
    }

    private static void PrintThreadsInfo()
    {
        var threadCount = ThreadPool.ThreadCount;
        var pendingItems = ThreadPool.PendingWorkItemCount;
        ThreadPool.GetAvailableThreads(out var workerThreads, out var asyncThreads);
        Console.WriteLine(
                $"threadId: {Environment.CurrentManagedThreadId}, " +
                $"threadCount: {threadCount}, " +
                $"pendingItems: {pendingItems}, " +
                $"av. workerThreads: {workerThreads}, " +
                $"av. asyncThreads: {asyncThreads}");
    }

    private static Task RunSyncOverAsyncTask(int current)
    {
        return Task.Run(() =>
        {
            if (current % 10 == 0) PrintThreadsInfo();
            Task.Delay(1000).Wait();
        });
    }
}