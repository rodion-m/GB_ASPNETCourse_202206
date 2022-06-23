using System.Collections.Immutable;

namespace Lesson.MultiThreading.ThreadSafeLists;

// В будущем прогнать через тесты для ConcurrentBag
// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Collections.Concurrent/tests/ConcurrentBagTests.cs
public class ThreadSafeListWithThreadStaticAndRWLockAndVersions<T>
{
    private readonly ThreadLocal<List<T>> _localList = new(true);
    private long _version;
    private readonly object _listCreatingLock = new();
    private readonly ReaderWriterLockSlim _removeReaderWriterLock = new();

    // public IReadOnlyList<T> GetSorted(delegate) 
    //     => GetAll().OrderBy(delegate);

    public IReadOnlyList<T> GetAll()
    {
        _removeReaderWriterLock.EnterReadLock();
        var spinWait = new SpinWait();
        // Пытаемся прочитать все данные до тех пор, пока они не будут соответствовать текущей версии
        // (если запись происходит постоянно без пауз, то будет дедлок)
        do
        {
            var snapshotVersion = Volatile.Read(ref _version);
            try
            {
                var snapshot = _localList.Values
                    .SelectMany(it => it)
                    .ToImmutableList();
                // if version is not changed while iterating
                if (snapshotVersion == Volatile.Read(ref _version))
                {
                    _removeReaderWriterLock.ExitReadLock();
                    return snapshot;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            spinWait.SpinOnce();
        } while (true);
    }

    public void Add(T item)
    {
        _removeReaderWriterLock.EnterReadLock();
        IncrementVersion();
        if (!_localList.IsValueCreated)
        {
            CreateLocalList();
        }
        _localList.Value!.Add(item);
        _removeReaderWriterLock.ExitReadLock();
    }
    public bool Remove(T item)
    {
        _removeReaderWriterLock.EnterWriteLock();
        IncrementVersion();
        var result = false;
        foreach (var list in _localList.Values)
        {
            result = list.Remove(item);
            if(result) 
                break;
        }
        _removeReaderWriterLock.ExitWriteLock();
        return result;
    }

    private void CreateLocalList()
    {
        lock (_listCreatingLock)
        {
            if (!_localList.IsValueCreated)
            {
                _localList.Value = new List<T>();
            }
        }
    }

    private void IncrementVersion() 
        => Interlocked.Increment(ref _version);
}