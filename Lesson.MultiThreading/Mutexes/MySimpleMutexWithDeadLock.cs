namespace Lesson.MultiThreading.Mutexes;

public class MySimpleMutexWithDeadLock
{
    private readonly HashSet<int> _workingThreads = new();
    
    public void Lock()
    {
        var threadId = Environment.CurrentManagedThreadId;
        if(_workingThreads.Contains(threadId)) return;
        _workingThreads.Add(threadId); //несколько потоков могут прийти сюда одновременно и добавить себя, тогда будет дедлок
        while (_workingThreads.Count > 1)
        {
        }
    }

    public void Unlock()
    {
        var threadId = Environment.CurrentManagedThreadId;
        _workingThreads.Remove(threadId);
    }
}