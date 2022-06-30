using System.Diagnostics;
using System.Net.Mail;

namespace Lesson.AsyncFeatures;

public class RealThreadsCountDemo
{
    private static async Task SendEmailAsync()
    {
        await Task.Delay(1000);
        PrintThreadCount();
        await Task.Delay(10); //честно
        await Task.Delay(10); //честно
        PrintThreadCount();
        
        //await Task.Run(async () => await Task.Delay(1000)); //честно
    }

    public static async Task RunAsync()
    {

        var tasksStarted = new AtomicLong();
        PrintThreadCount();
        for (int i = 0; i < 1000; i++)
        {
            Task.Run(async () =>
            {
                Console.WriteLine($"Tasks Started: {tasksStarted++}, " +
                                  $"Thread Id: {Environment.CurrentManagedThreadId}");
                //Thread.Sleep(TimeSpan.FromMinutes(1));
                Task.Delay(TimeSpan.FromMinutes(1)).Wait();
            });
        }

        PrintThreadCount();
        for (int i = 0; i < 60; i++)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            PrintThreadCount();
        }
    }

    public static void Run(bool blocking)
    {
        PrintThreadCount();
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
        {
            Task.Run(() =>
            {
                Console.WriteLine($"Thread Started in {sw.ElapsedMilliseconds} ms: " 
                                  + Environment.CurrentManagedThreadId);
                if (blocking)
                {
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
                else
                {
                    //тредпул создает новые треды быстрее,
                    //т.к. таски с вэйтами оповещают тредпул о своей блокировке
                    Task.Delay(TimeSpan.FromMinutes(1)).Wait();
                }
            });
        }

        PrintThreadCount();
        for (int i = 0; i < 600; i++)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            PrintThreadCount();
        }
        
        Thread.Sleep(TimeSpan.FromMinutes(1));
        Console.WriteLine(ThreadPool.ThreadCount);
    }

    private static void PrintThreadCount()
    {
        Console.WriteLine("--- THREAD COUNT: " + ThreadPool.ThreadCount + " ---");
    }
}