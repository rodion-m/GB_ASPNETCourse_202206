namespace Lesson.MultiThreading;

public class ThreadsDemo
{
    public static void CreateThreadsAndPrintIds(int threadsCount)
    {
        var threads = new List<Thread>();
        for (int i = 0; i < threadsCount; i++)
        {
            var thread = new Thread(() =>
            {
                Console.WriteLine($"Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            });
            thread.Start();
            threads.Add(thread);
        }

        threads.ForEach(it => it.Join());
        Console.WriteLine($"MaxId: {threads.MaxBy(it => it.ManagedThreadId)!.ManagedThreadId}");
    }
}