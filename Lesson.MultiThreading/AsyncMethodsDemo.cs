namespace Lesson.MultiThreading;

public class AsyncMethodsDemo
{
    public static async Task AsyncMethod()
    {
        Console.WriteLine();
        PrintThreadInfo("Before delay current", Thread.CurrentThread);
        
        await Task.Delay(1);
        var thread = Thread.CurrentThread;
        StartMonitorThread(thread);
        
        Console.WriteLine("Delay...");
        await Task.Delay(1000);
        Console.WriteLine("Delay finished");
        
        Console.WriteLine("Sleep...");
        Thread.Sleep(1000);
        Console.WriteLine("Sleep finished");
        
        PrintThreadInfo("Saved ", thread);
        PrintThreadInfo("Current", Thread.CurrentThread);
        
        Console.WriteLine("Start imitate work");
        ImitateHeavyWork();
        Console.WriteLine("Work completed");
        
        Console.WriteLine(nameof(AsyncMethod) + " is completed");
    }

    private static void StartMonitorThread(Thread thread)
    {
        var savedState = thread.ThreadState;
        PrintThreadInfo("Start monitor for", thread);
        Task.Run(() =>
        {
            while (thread.IsAlive)
            {
                var currentState = thread.ThreadState;
                if (savedState != currentState)
                {
                    savedState = currentState;
                    PrintThreadInfo("State changed", thread, currentState);
                }
                Task.Yield();
            }
        });
    }

    private static void PrintThreadInfo(string caption, Thread thread, ThreadState? state = null)
    {
        Console.WriteLine(
            $"{caption} Thread ID: {thread.ManagedThreadId} State: {state ?? thread.ThreadState}");
    }

    public static void CreateBgThreadAndPrint()
    {
        var thread = new Thread(() => ImitateHeavyWork())
        {
            IsBackground = true
        };
        PrintThreadInfo("Before start", thread); //Background, Unstarted
        thread.Start();
        PrintThreadInfo("After started", thread); //Background
    }

    private static void ImitateHeavyWork(int iterCount = 10_000_000)
    {
        for (int i = 0; i < iterCount; i++)
        {
        }
    }
}