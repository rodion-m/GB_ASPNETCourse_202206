using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lesson.MultiThreading.ThreadSafeLists;

[DebuggerDisplay("Count = {Count}")]
[Serializable]
public class ThreadSafeList<T> : IList<T>, IList, IReadOnlyList<T>
{
    private const int DefaultCapacity = 4;

    internal T[] _items; // Do not rename (binary serialization)
    internal volatile int _size; // Do not rename (binary serialization)
    private long _version; // Do not rename (binary serialization)

#pragma warning disable CA1825 // avoid the extra generic instantiation for Array.Empty<T>()
    private static readonly T[] s_emptyArray = Array.Empty<T>();
#pragma warning restore CA1825

    // Constructs a List. The list is initially empty and has a capacity
    // of zero. Upon adding the first element to the list the capacity is
    // increased to DefaultCapacity, and then increased in multiples of two
    // as required.
    public ThreadSafeList()
    {
        _items = s_emptyArray;
    }

    // Constructs a List with a given initial capacity. The list is
    // initially empty, but will have room for the given number of elements
    // before any reallocations are required.
    //
    public ThreadSafeList(int capacity)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        if (capacity == 0)
            _items = s_emptyArray;
        else
            _items = new T[capacity];
    }

    // Constructs a List, copying the contents of the given collection. The
    // size and capacity of the new list will both be equal to the size of the
    // given collection.
    //
    public ThreadSafeList(IEnumerable<T> collection)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (collection is ICollection<T> c)
        {
            int count = c.Count;
            if (count == 0)
            {
                _items = s_emptyArray;
            }
            else
            {
                _items = new T[count];
                c.CopyTo(_items, 0);
                _size = count;
            }
        }
        else
        {
            _items = s_emptyArray;
            using (IEnumerator<T> en = collection!.GetEnumerator())
            {
                while (en.MoveNext())
                {
                    Add(en.Current);
                }
            }
        }
    }

    // Gets and sets the capacity of this list.  The capacity is the size of
    // the internal array used to hold items.  When set, the internal
    // array of the list is reallocated to the given capacity.
    //
    public int Capacity
    {
        get => _items.Length;
        set
        {
            if (value < _size)
            {
                throw new InvalidOperationException();
            }

            if (value != _items.Length)
            {
                if (value > 0)
                {
                    T[] newItems = new T[value];
                    if (_size > 0)
                    {
                        Array.Copy(_items, newItems, _size);
                    }

                    _items = newItems;
                }
                else
                {
                    _items = s_emptyArray;
                }
            }
        }
    }

    // Read-only property describing how many elements are in the List.
    public int Count => _size;

    bool IList.IsFixedSize => false;

    // Is this List read-only?
    bool ICollection<T>.IsReadOnly => false;

    bool IList.IsReadOnly => false;

    // Is this List synchronized (thread-safe)?
    bool ICollection.IsSynchronized => true;

    // Synchronization root for this object.
    object ICollection.SyncRoot => this;

    // Sets or Gets the element at the given index.
    public T this[int index]
    {
        get
        {
            // Following trick can reduce the range check by one
            if ((uint)index >= (uint)_size)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _items[index];
        }

        set
        {
            if ((uint)index >= (uint)_size)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            _items[index] = value;
            _version++;
        }
    }

    private static bool IsCompatibleObject(object? value)
    {
        // Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
        // Note that default(T) is not equal to null for value types except when T is Nullable<U>.
        return (value is T) || (value == null && default(T) == null);
    }

    object? IList.this[int index]
    {
        get
        {
            return this[index];
        }
        set
        {
            this[index] = (T)value!;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private long IncrementVersion() => Interlocked.Increment(ref _version);

    // Adds the given object to the end of this list. The size of the list is
    // increased by one. If required, the capacity of the list is doubled
    // before adding the new element.
    //
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        var spinWait = new SpinWait();
        do
        {
            var savedVersion = IncrementVersion();
            var array = _items;
            var size = _size;
            if ((uint)size < (uint)array.Length)
            {
                array[size + 1] = item;
            }
            // else
            // {
            //     AddWithResize(item);
            // }
            if (savedVersion == _version)
            {
                Interlocked.Increment(ref _size);
                return;
            }

            spinWait.SpinOnce();
        } while (true);
    }

    // Non-inline from List.Add to improve its code quality as uncommon path
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddWithResize(T item)
    {
        Debug.Assert(_size == _items.Length);
        int size = _size;
        Grow(size + 1);
        _size = size + 1;
        _items[size] = item;
    }

    int IList.Add(object? item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        try
        {
            Add((T)item!);
        }
        catch (InvalidCastException)
        {
            throw new ArgumentException(nameof(item));
        }

        return Count - 1;
    }

    public ReadOnlyCollection<T> AsReadOnly()
        => new ReadOnlyCollection<T>(this);

    // Clears the contents of List.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        _version++;
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            int size = _size;
            _size = 0;
            if (size > 0)
            {
                Array.Clear(_items, 0, size); // Clear the elements so that the gc can reclaim the references.
            }
        }
        else
        {
            _size = 0;
        }
    }

    // Contains returns true if the specified element is in the List.
    // It does a linear, O(n) search.  Equality is determined by calling
    // EqualityComparer<T>.Default.Equals().
    //
    public bool Contains(T item)
    {
        // PERF: IndexOf calls Array.IndexOf, which internally
        // calls EqualityComparer<T>.Default.IndexOf, which
        // is specialized for different types. This
        // boosts performance since instead of making a
        // virtual method call each iteration of the loop,
        // via EqualityComparer<T>.Default.Equals, we
        // only make one virtual call to EqualityComparer.IndexOf.

        return _size != 0 && IndexOf(item) != -1;
    }

    bool IList.Contains(object? item)
    {
        if (IsCompatibleObject(item))
        {
            return Contains((T)item!);
        }

        return false;
    }

    // Copies this List into array, which must be of a
    // compatible array type.
    public void CopyTo(T[] array)
        => CopyTo(array, 0);

    // Copies this List into array, which must be of a
    // compatible array type.
    void ICollection.CopyTo(Array array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        Array.Copy(_items, 0, array!, arrayIndex, _size);
    }

    // Copies a section of this list to the given array at the given index.
    //
    // The method uses the Array.Copy method to copy the elements.
    //
    public void CopyTo(int index, T[] array, int arrayIndex, int count)
    {
        if (_size - index < count)
        {
            throw new ArgumentException();
        }

        // Delegate rest of error checking to Array.Copy.
        Array.Copy(_items, index, array, arrayIndex, count);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        // Delegate rest of error checking to Array.Copy.
        Array.Copy(_items, 0, array, arrayIndex, _size);
    }

    /// <summary>
    /// Ensures that the capacity of this list is at least the specified <paramref name="capacity"/>.
    /// If the current capacity of the list is less than specified <paramref name="capacity"/>,
    /// the capacity is increased by continuously twice current capacity until it is at least the specified <paramref name="capacity"/>.
    /// </summary>
    /// <param name="capacity">The minimum capacity to ensure.</param>
    public int EnsureCapacity(int capacity)
    {
        if (capacity < 0)
        {
        }

        if (_items.Length < capacity)
        {
            Grow(capacity);
            _version++;
        }

        return _items.Length;
    }

    /// <summary>
    /// Increase the capacity of this list to at least the specified <paramref name="capacity"/>.
    /// </summary>
    /// <param name="capacity">The minimum capacity to ensure.</param>
    private void Grow(int capacity)
    {
        Debug.Assert(_items.Length < capacity);

        int newcapacity = _items.Length == 0 ? DefaultCapacity : 2 * _items.Length;

        // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
        // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
        if ((uint)newcapacity > Array.MaxLength) newcapacity = Array.MaxLength;

        // If the computed capacity is still less than specified, set to the original argument.
        // Capacities exceeding Array.MaxLength will be surfaced as OutOfMemoryException by Array.Resize.
        if (newcapacity < capacity) newcapacity = capacity;

        Capacity = newcapacity;
    }

    //
    // public T? Find(Predicate<T> match)
    // {
    //     if (match == null)
    //     {
    //         ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
    //     }
    //
    //     for (int i = 0; i < _size; i++)
    //     {
    //         if (match(_items[i]))
    //         {
    //             return _items[i];
    //         }
    //     }
    //
    //     return default;
    // }
    //
    // public List<T> FindAll(Predicate<T> match)
    // {
    //     if (match == null)
    //     {
    //         ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
    //     }
    //
    //     List<T> list = new List<T>();
    //     for (int i = 0; i < _size; i++)
    //     {
    //         if (match(_items[i]))
    //         {
    //             list.Add(_items[i]);
    //         }
    //     }
    //
    //     return list;
    // }
    //
    // public int FindIndex(Predicate<T> match)
    //     => FindIndex(0, _size, match);
    //
    // public int FindIndex(int startIndex, Predicate<T> match)
    //     => FindIndex(startIndex, _size - startIndex, match);
    //
    // public int FindIndex(int startIndex, int count, Predicate<T> match)
    // {
    //     if ((uint)startIndex > (uint)_size)
    //     {
    //         ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_Index();
    //     }
    //
    //     if (count < 0 || startIndex > _size - count)
    //     {
    //         ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count();
    //     }
    //
    //     if (match == null)
    //     {
    //         ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
    //     }
    //
    //     int endIndex = startIndex + count;
    //     for (int i = startIndex; i < endIndex; i++)
    //     {
    //         if (match(_items[i])) return i;
    //     }
    //
    //     return -1;
    // }
    //
    // public T? FindLast(Predicate<T> match)
    // {
    //     if (match == null)
    //     {
    //         ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
    //     }
    //
    //     for (int i = _size - 1; i >= 0; i--)
    //     {
    //         if (match(_items[i]))
    //         {
    //             return _items[i];
    //         }
    //     }
    //
    //     return default;
    // }
    //
    // public int FindLastIndex(Predicate<T> match)
    //     => FindLastIndex(_size - 1, _size, match);
    //
    // public int FindLastIndex(int startIndex, Predicate<T> match)
    //     => FindLastIndex(startIndex, startIndex + 1, match);
    //
    // public int FindLastIndex(int startIndex, int count, Predicate<T> match)
    // {
    //     if (match == null)
    //     {
    //         ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
    //     }
    //
    //     if (_size == 0)
    //     {
    //         // Special case for 0 length List
    //         if (startIndex != -1)
    //         {
    //             ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_Index();
    //         }
    //     }
    //     else
    //     {
    //         // Make sure we're not out of range
    //         if ((uint)startIndex >= (uint)_size)
    //         {
    //             ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_Index();
    //         }
    //     }
    //
    //     // 2nd have of this also catches when startIndex == MAXINT, so MAXINT - 0 + 1 == -1, which is < 0.
    //     if (count < 0 || startIndex - count + 1 < 0)
    //     {
    //         ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count();
    //     }
    //
    //     int endIndex = startIndex - count;
    //     for (int i = startIndex; i > endIndex; i--)
    //     {
    //         if (match(_items[i]))
    //         {
    //             return i;
    //         }
    //     }
    //
    //     return -1;
    // }
    //
    // public void ForEach(Action<T> action)
    // {
    //     if (action == null)
    //     {
    //         ThrowHelper.ThrowArgumentNullException(ExceptionArgument.action);
    //     }
    //
    //     int version = _version;
    //
    //     for (int i = 0; i < _size; i++)
    //     {
    //         if (version != _version)
    //         {
    //             break;
    //         }
    //
    //         action(_items[i]);
    //     }
    //
    //     if (version != _version)
    //         ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
    // }

    // Returns an enumerator for this list with the given
    // permission for removal of elements. If modifications made to the list
    // while an enumeration is in progress, the MoveNext and
    // GetObject methods of the enumerator will throw an exception.
    //
    public Enumerator GetEnumerator()
        => new Enumerator(this);

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
        => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator()
        => new Enumerator(this);

    // Returns the index of the first occurrence of a given value in a range of
    // this list. The list is searched forwards from beginning to end.
    // The elements of the list are compared to the given value using the
    // Object.Equals method.
    //
    // This method uses the Array.IndexOf method to perform the
    // search.
    //
    public int IndexOf(T item)
        => Array.IndexOf(_items, item, 0, _size);

    public void Insert(int index, T item)
    {
        throw new NotImplementedException();
    }

    int IList.IndexOf(object? item)
    {
        if (IsCompatibleObject(item))
        {
            return IndexOf((T)item!);
        }

        return -1;
    }

    public void Insert(int index, object? value)
    {
        throw new NotImplementedException();
    }

    // Removes the element at the given index. The size of the list is
    // decreased by one.
    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    void IList.Remove(object? item)
    {
        if (IsCompatibleObject(item))
        {
            Remove((T)item!);
        }
    }

    // Removes the element at the given index. The size of the list is
    // decreased by one.
    public void RemoveAt(int index)
    {
        if ((uint)index >= (uint)_size)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        _size--;
        if (index < _size)
        {
            Array.Copy(_items, index + 1, _items, index, _size - index);
        }

        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            _items[_size] = default!;
        }

        _version++;
    }


    // Sorts the elements in a section of this list. The sort compares the
    // elements to each other using the given IComparer interface. If
    // comparer is null, the elements are compared to each other using
    // the IComparable interface, which in that case must be implemented by all
    // elements of the list.
    //
    // This method uses the Array.Sort method to sort the elements.
    //

    // ToArray returns an array containing the contents of the List.
    // This requires copying the List, which is an O(n) operation.
    public T[] ToArray()
    {
        if (_size == 0)
        {
            return s_emptyArray;
        }

        T[] array = new T[_size];
        Array.Copy(_items, array, _size);
        return array;
    }

    // Sets the capacity of this list to the size of the list. This method can
    // be used to minimize a list's memory overhead once it is known that no
    // new elements will be added to the list. To completely clear a list and
    // release all memory referenced by the list, execute the following
    // statements:
    //
    // list.Clear();
    // list.TrimExcess();
    //
    public void TrimExcess()
    {
        int threshold = (int)(((double)_items.Length) * 0.9);
        if (_size < threshold)
        {
            Capacity = _size;
        }
    }


    public struct Enumerator : IEnumerator<T>, IEnumerator
    {
        private readonly ThreadSafeList<T> _list;
        private int _index;
        private T? _current;

        internal Enumerator(ThreadSafeList<T> list)
        {
            _list = list;
            _index = 0;
            _current = default;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            ThreadSafeList<T> localList = _list;
            if (_index >= localList._items.Length)
                return false;
            
            _current = localList._items[_index];
            _index++;
            return true;
        }


        public T Current => _current!;

        object? IEnumerator.Current
        {
            get
            {
                if (_index == 0 || _index == _list._size + 1)
                {
                    throw new InvalidOperationException();
                }

                return Current;
            }
        }

        void IEnumerator.Reset()
        {
            _index = 0;
            _current = default;
        }
    }
}