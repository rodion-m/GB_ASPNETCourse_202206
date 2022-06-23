namespace Lesson.MultiThreading.Mutexes;

public static class MutexTests
{
    private static int _concurrencyLevel 
        => TaskScheduler.Current.MaximumConcurrencyLevel - 1;

    public static void TestNotLocked()
    {
        var list = new List<int>();
        Parallel.For(0, 1_000_000, new ParallelOptions()
        {
            MaxDegreeOfParallelism = 3
        }, i =>
        {
            list.Add(i);
        });
        int total = 0;
        Parallel.ForEach(list, new ParallelOptions()
        {
            MaxDegreeOfParallelism = 3
        }, val =>
        {
            
            Interlocked.Add(ref total, val);
        });
        Console.WriteLine(list.Count);
    }
    
    public static void TestDeadLockedMutex()
    {
        var list = new List<int>();
        var mutex = new MySimpleMutexWithDeadLock();
        Parallel.For(0, 1_000_000, new ParallelOptions() {MaxDegreeOfParallelism = _concurrencyLevel}, i =>
        {
            mutex.Lock();
            list.Add(i);
            mutex.Unlock();
        });
        Console.WriteLine(list.Count);
    }
    
    public static void TestDeadLockedNotSafe()
    {
        var list = new List<int>();
        var mutex = new SimpleMutexNotSafe();
        Parallel.For(0, 1_000_000, new ParallelOptions() {MaxDegreeOfParallelism = _concurrencyLevel}, i =>
        {
            mutex.Lock();
            list.Add(i);
            mutex.Unlock();
        });
        Console.WriteLine(list.Count);
    }
    
    public static void TestSimpleMutexWithConcurrentDict()
    {
        var list = new List<int>();
        var mutex = new SimpleMutexWithConcurrentDict();
        Parallel.For(0, 1_000_000, new ParallelOptions() {MaxDegreeOfParallelism = _concurrencyLevel}, i =>
        {
            mutex.Lock();
            list.Add(i);
            mutex.Unlock();
        });
        Console.WriteLine(list.Count);
    }
    
    public static void TestSimpleMutexWithArray()
    {
        var list = new List<int>();
        var mutex = new SimpleMutexWithArray();
        Parallel.For(0, 1_000_000, new ParallelOptions() {MaxDegreeOfParallelism = _concurrencyLevel}, i =>
        {
            mutex.Lock();
            list.Add(i);
            mutex.Unlock();
        });
        Console.WriteLine(list.Count);
    }
    
    public static void TestSimpleSpinLock()
    {
        var list = new List<int>();
        var mutex = new SimpleSpinLock();
        Parallel.For(0, 1_000_000, new ParallelOptions() {MaxDegreeOfParallelism = _concurrencyLevel}, i =>
        {
            mutex.Lock();
            list.Add(i);
            mutex.Unlock();
        });
        Console.WriteLine(list.Count);
    }
    
    public static void TestSpinLock()
    {
        var list = new List<int>();
        var mutex = new SpinLock();
        Parallel.For(0, 1_000_000, new ParallelOptions() {MaxDegreeOfParallelism = _concurrencyLevel}, i =>
        {
            bool lockTaken = false;
            mutex.Enter(ref lockTaken);
            list.Add(i);
            mutex.Exit();
        });
        Console.WriteLine(list.Count);
    }
    
    public static void TestMonitor()
    {
        var list = new List<int>();
        Parallel.For(0, 1_000_000, new ParallelOptions() {MaxDegreeOfParallelism = _concurrencyLevel}, i =>
        {
            lock (list)
            {
                list.Add(i);
            }
        });
        Console.WriteLine(list.Count);
    }

    
    public static async Task TestSimpleMutexWithArrayAsync()
    {
        var list = new List<int>();
        var mutex = new SimpleMutexWithArrayAsync();
        await Parallel.ForEachAsync(Enumerable.Range(0, 1_000_000), new ParallelOptions() {MaxDegreeOfParallelism = _concurrencyLevel}, 
            async (i, _) =>
        {
            await mutex.LockAsync();
            list.Add(i);
            mutex.Unlock();
        });
        Console.WriteLine(list.Count);
    }
}