using System.Collections;

namespace HW2_ConcurrentList;

public class ConcurrentList<T> : IEnumerable<T>
{
    private readonly List<T> _items = new();
    private readonly object _syncObj = new();

    public T this[int index]
    {
        get
        {
            lock (_syncObj)
            {
                return _items[index];
            }
        }
        set
        {
            lock (_syncObj)
            {
                _items[index] = value;
            }
        }
    }

    public void Add(T item)
    {
        lock (_syncObj)
        {
            _items.Add(item);
        }
    }

    public void Remove(T item)
    {
        lock (_syncObj)
        {
            _items.Remove(item);
        }
    }

    public IReadOnlyList<T> GetAll()
    {
        lock (_syncObj)
        {
            //Создаем копию списка (снэпшот)
            return _items.ToList();
        }
    }

    public IEnumerator<T> GetEnumerator() => GetAll().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}