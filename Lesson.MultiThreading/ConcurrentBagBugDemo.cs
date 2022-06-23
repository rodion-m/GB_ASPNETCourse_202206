using System.Collections.Concurrent;

namespace Lesson.MultiThreading;

public class ConcurrentBagBugDemo
{
    private ConcurrentBag<int> Values = new();

    public void Add(int value)
    {
        Values.Add(value);
    }

    public void Remove(int value)
    {
        var enumerable = Values.Where(it => it != value);
        var newValues = new ConcurrentBag<int>(enumerable);
        Values = newValues;
    }
}