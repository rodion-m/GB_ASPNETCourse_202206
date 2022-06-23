using System.Collections.Concurrent;

namespace Lesson.MultiThreading.Mutexes;

public class SimpleMutexForTwoThreads
{
    private readonly ConcurrentDictionary<int, bool> _workingThreads = new();
    
    private volatile int _victim = -1;
    
    public void Lock()
    {
        var threadId = Environment.CurrentManagedThreadId;
        if (!_workingThreads.TryAdd(threadId, true))
        {
            return; //один и тот же поток спокойно может вызвать лок повторно
        }
        _victim = threadId;
        while (_workingThreads.Count > 1 && _victim == threadId)
        {
        }
    }

    public void Unlock()
    {
        var threadId = Environment.CurrentManagedThreadId;
        while (!_workingThreads.TryRemove(threadId, out _))
        {
        }
    }
}