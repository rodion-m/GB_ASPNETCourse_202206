using System.Collections;
using System.Collections.Concurrent;

namespace HW2_ConcurrentList;

/// <summary>
/// Proof of Concept ConcurrentList с очередью операций
/// </summary>
/// <typeparam name="T"></typeparam>
public class ConcurrentListQ<T> : IEnumerable<T>
{
    private readonly List<T> _items = new();
    private readonly object _syncObj = new();
    private readonly ConcurrentQueue<Action> _actionsQueue = new();

    public T this[int index]
    {
        get
        {
            WaitBeforeAllActionsCompleted();
            lock (_syncObj)
            {
                return _items[index];
            }
        }
        set
        {
            _actionsQueue.Enqueue(() => _items[index] = value);
            RunAllActionsAndWait();
        }
    }

    private void RunAllActionsAndWait()
    {
        lock (_syncObj)
        {
            while (_actionsQueue.TryDequeue(out var action))
            {
                action();
            }
        }
    }

    private void WaitBeforeAllActionsCompleted()
    {
        var spinWait = new SpinWait();
        while (!_actionsQueue.IsEmpty)
        {
            spinWait.SpinOnce();
        }
    }

    public void Add(T item)
    {
        _actionsQueue.Enqueue(() => _items.Add(item));
        RunAllActionsAndWait();
    }

    public void Remove(T item)
    {
        _actionsQueue.Enqueue(() => _items.Remove(item));
        RunAllActionsAndWait();
    }

    public IReadOnlyList<T> GetAll()
    {
        WaitBeforeAllActionsCompleted();
        lock (_syncObj)
        {
            //Создаем копию списка (снэпшот)
            return _items.ToList();
        }
    }

    public IEnumerator<T> GetEnumerator() => GetAll().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}