namespace Lesson.MultiThreading.Mutexes;

/// <summary>
/// НЕрекурсивный Mutex
/// </summary>
public class SimpleSpinLock
{
    private volatile int _processing;
    
    /// <summary>
    /// Значит, в данный момент какой-то поток выполняет работу
    /// </summary>
    public bool IsProcessing => _processing == 1;

    public void Lock()
    {
        //SpinWait spinWait = new();
        while (true)
        {
            var wasProcessing = Interlocked.CompareExchange(ref _processing, 1, 0);
            if (wasProcessing == 0 && _processing == 1)
            {
                break;
            }
            //spinWait.SpinOnce(); //долго
            Thread.Sleep(1);
        }
    }

    public void Unlock()
    {
        _processing = 0;
    }
}