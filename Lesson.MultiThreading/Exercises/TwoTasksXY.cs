namespace Lesson.MultiThreading.Exercises;

public class TwoTasksXY
{
    private int x;
    private int y;

    public void Run()
    {
        var t1 = Task.Run(() =>
        {
            x = 1;
            Console.Write(y);
        });
        var t2 = Task.Run(() =>
        {
            y = 1;
            Console.Write(x);
        });
        
        Task.WaitAll(t1, t2);
    }
    
    public void RunWithSpinLock() //always 10
    {
        var spinLock = new SpinLock();
        var t1 = Task.Run(() =>
        {
            var lockTaken = false;
            spinLock.Enter(ref lockTaken);
            x = 1;
            Console.Write(y);
            spinLock.Exit();
        });
        var t2 = Task.Run(() =>
        {
            var lockTaken = false;
            spinLock.Enter(ref lockTaken);
            y = 1;
            Console.Write(x);
            spinLock.Exit();
        });
        
        Task.WaitAll(t1, t2);
    }
    
    public void RunWithMemoryBarriers()
    {
        var t1 = Task.Run(() =>
        {
            x = 1;
            Interlocked.MemoryBarrier();
            Console.Write(y);
        });
        var t2 = Task.Run(() =>
        {
            y = 1;
            Interlocked.MemoryBarrier();
            Console.Write(x);
        });
        
        Task.WaitAll(t1, t2);
    }
    
    public void RunWithInterlocked()
    {
        var t1 = Task.Run(() =>
        {
            Interlocked.Exchange(ref x, 1);
            Console.Write(y);
        });
        var t2 = Task.Run(() =>
        {
            Interlocked.Exchange(ref y, 1);
            Console.Write(x);
        });
        
        Task.WaitAll(t1, t2);
    }
    
    public void RunWithVolatileClass()
    {
        var t1 = Task.Run(() =>
        {
            Volatile.Write(ref x, 1);
            Console.Write(Volatile.Read(ref y));
        });
        var t2 = Task.Run(() =>
        {
            Volatile.Write(ref y, 1);
            Console.Write(Volatile.Read(ref x));
        });
        
        Task.WaitAll(t1, t2);
    }
}