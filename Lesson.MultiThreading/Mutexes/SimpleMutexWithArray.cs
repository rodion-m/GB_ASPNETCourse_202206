namespace Lesson.MultiThreading.Mutexes;

/// <summary>
/// Рекурсивный Mutex, т.е. один поток может повторно безболезненно вызвать lock
/// </summary>
public class SimpleMutexWithArray
{
    private const int THREADS_LIMIT = 8192; //TODO можно расширять _threads по мере увеличения ManagedThreadId
    
    /// <summary>
    /// Выполняющийся поток и потоки в очереди хранятся тут
    /// </summary>
    private readonly int[] _threads = new int[THREADS_LIMIT]; //ManagedThreadId может быть очень большим, но обычно небольшой
    private volatile int _threadsCount;
    private volatile int _processing;
    
    /// <summary>
    /// Значит, в данный момент какой-то поток выполняет работу
    /// </summary>
    public bool IsProcessing => _processing == 1;

    public void Lock()
    {
        var threadId = Environment.CurrentManagedThreadId;
        if (_threads[threadId] == 1) //recursive mutex
        {
            return; //один и тот же поток спокойно может вызвать лок повторно
        }

        Interlocked.Increment(ref _threadsCount);
        _threads[threadId] = 1;

        var savedThreadsCount = _threadsCount;
        while (true)
        {
            if (_threadsCount > 1 && _threadsCount == savedThreadsCount && _processing == 1)
            {
                Thread.Sleep(1);
                continue;
            }

            var wasProcessing = Interlocked.CompareExchange(ref _processing, 1, 0);
            if (wasProcessing == 0)
            {
                break;
            }
        }
    }

    public void Unlock()
    {
        var threadId = Environment.CurrentManagedThreadId;
        ref var threadValue = ref _threads[threadId];
        if (threadValue == 0)
        {
            return; //Повторный вызов Unlock разрешен
        }
        threadValue = 0;

        _processing = 0;
        Interlocked.Decrement(ref _threadsCount);
    }
}