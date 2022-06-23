namespace Lesson.MultiThreading;

public class NonBackgroundExample //Foreground
{
    public static void DoWorkInThread()
    {
        Console.WriteLine("Begin");
        Thread thread = new Thread(PrintNumbers);
        thread.Start();
        Thread.Sleep(5000);
        Console.WriteLine("End");
    }

    private static void PrintNumbers()
    {
        for (int i = 0; i < 7; i++)
        {
            Console.WriteLine(i);
            Thread.Sleep(1000);
        }

        Console.WriteLine(Thread.CurrentThread.ThreadState);
    }

    public static void DoWorkInTask()
    {
        Console.WriteLine("Begin");
        //Task task = Task.Run(PrintNumbers);
        Task task = Task.Factory.StartNew(PrintNumbers, TaskCreationOptions.LongRunning);
        Thread.Sleep(5000);
        Console.WriteLine("End");
    }
}