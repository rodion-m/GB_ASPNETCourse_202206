namespace Lesson.MultiThreading;

public class DeadlockNotVolatileDemo
{
    private static bool terminate = false;
    public static void Run()
    {
        var t1 = Task.Run(() => {       
            int x = 0;
            while (!terminate) //terminates is cached here since it's not volatile
            {
                x *= 1;
            }
        });
     
        Thread.Sleep(2000);
        terminate = true;
        t1.Wait();
        Console.WriteLine("Done.");
    }
}