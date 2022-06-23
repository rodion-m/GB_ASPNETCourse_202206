using System.Collections.Concurrent;

namespace Lesson.MultiThreading.Mutexes;

//Не работает
public class SimpleMutexWithArrayAsync
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

    private readonly ConcurrentQueue<TaskCompletionSource> _taskCompletionSources = new();

    public Task LockAsync()
    {
        var threadId = Environment.CurrentManagedThreadId;
        if (_threads[threadId] == 1)
        {
            return Task.CompletedTask; //один и тот же поток спокойно может вызвать лок повторно
        }

        Interlocked.Increment(ref _threadsCount);
        Interlocked.Exchange(ref _threads[threadId], 1);
        var taskCompletionSource = new TaskCompletionSource();
        
        _taskCompletionSources.Enqueue(taskCompletionSource);
        RaiseUnlocked();

        return taskCompletionSource.Task;
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

        Interlocked.Exchange(ref _processing, 0);
        Interlocked.Decrement(ref _threadsCount);
        RaiseUnlocked();
    }

    private void RaiseUnlocked()
    {
        if(_threadsCount > 1 && _processing == 1)
            return;
            
        var wasProcessing = Interlocked.CompareExchange(ref _processing, 1, 0);
        if (wasProcessing == 0)
        {
            if (_taskCompletionSources.TryDequeue(out var taskCompletionSource))
            {
                taskCompletionSource.SetResult();
            }
        }
        else
        {
            RaiseUnlocked();
        }
    }
}