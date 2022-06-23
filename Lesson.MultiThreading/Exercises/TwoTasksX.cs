namespace Lesson.MultiThreading.Exercises;

public class TwoTasksX
{
    private int x;

    public void Run()
    {
        //"C:\Users\rodio\RiderProjects\PatternsAndMVC_Course\Lesson.MultiThreading\bin\Release\net6.0\results_xx.txt"
        var t1 = Task.Run(() =>
        {
            x = 1;
            Console.Write(x);
        });
        var t2 = Task.Run(() =>
        {
            x = 0;
            Console.Write(x);
        });
        Task.WaitAll(t1, t2);
    }
}