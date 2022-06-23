namespace Lesson.MultiThreading.Mutexes;

public class SimpleMutexNotSafe
{
    private readonly HashSet<int> _workingThreads = new(128);
    private int _victim = -1;
    
    public void Lock()
    {
        var threadId = Environment.CurrentManagedThreadId;
        if(_workingThreads.Contains(threadId)) return;
        _workingThreads.Add(threadId);
        _victim = threadId;
        while (_workingThreads.Count > 1 && _victim == threadId)
        {
        }
    }

    public void Unlock()
    {
        var threadId = Environment.CurrentManagedThreadId;
        _workingThreads.Remove(threadId);
    }
}