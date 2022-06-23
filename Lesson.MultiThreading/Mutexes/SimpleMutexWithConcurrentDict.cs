using System.Collections.Concurrent;

namespace Lesson.MultiThreading.Mutexes;

//TODO async lock на TaskCompletionSource с колбеком в Unlock
public class SimpleMutexWithConcurrentDict
{
    /// <summary>
    /// Выполняющийся поток и потоки в очереди хранятся тут
    /// </summary>
    private readonly ConcurrentDictionary<int, bool> _threads = new();
    private volatile int _processing;


    /// <summary>
    /// Значит, в данный момент какой-то поток выполняет работу
    /// </summary>
    public bool IsProcessing => _processing == 1;

    public void Lock()
    {
        var threadId = Environment.CurrentManagedThreadId;
        if (!_threads.TryAdd(threadId, true))
        {
            return; //один и тот же поток спокойно может вызвать лок повторно
        }

        var savedThreadsCount = _threads.Count;
        while (true)
        {
            var currentThreadsCount = _threads.Count;
            if(currentThreadsCount > 1
               && currentThreadsCount == savedThreadsCount
              && _processing == 1)
                continue;
            
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
        Interlocked.Exchange(ref _processing, 0);

        while (!_threads.TryRemove(threadId, out _))
        {
        }

    }
}