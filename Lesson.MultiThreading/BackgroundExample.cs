namespace Lesson.MultiThreading;

public class BackgroundExample
{
    public static void DoWorkInThread()
    {
        Console.WriteLine("Begin");
        Thread thread = new Thread(PrintNumbers)
        {
            IsBackground = true
        };
        thread.Start();
        Thread.Sleep(5000);
        Console.WriteLine("End");
        

        void PrintNumbers()
        {
            for (int i = 0; i < 7; i++)
            {
                Console.WriteLine($"{i}");
                Thread.Sleep(1000);
            }
        }
    }

}