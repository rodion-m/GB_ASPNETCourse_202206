using System.Collections.Concurrent;
using System.Diagnostics;

namespace Lesson.MultiThreading.ThreadSafeLists;

public class ThreadSafeListTest
{
    public static void RunCustomBench()
    {
        // С попыткой получения всех айтемов в цикле (GetItemsInCycle) работает примерно в 2 раза быстрее, чем ConcurrentDictionary
        // Без попытки получения всех айтемов в списке работает примерно в 3 раза быстрее, чем ConcurrentDictionary
        var list = new ThreadSafeListWithThreadStaticAndRWLockAndVersions<int>();
        var stopwatch = Stopwatch.StartNew();
        Parallel.Invoke(AddItemsInParallel, GetItemsInCycle, GetAndRemoveItem);
        Console.WriteLine($"Elapsed: {stopwatch.Elapsed}");
        var items = list.GetAll();
        Console.WriteLine();
        Console.WriteLine($"Result count: {items.Count}");

        void AddItemsInParallel()
        {
            Parallel.For(0, 100_001, i =>
            {
                list.Add(i);
            });
        }
        void GetItemsInCycle()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(list.GetAll().Count);
            }
        }
        
        void GetAndRemoveItem()
        {
            Thread.Sleep(1);
            var item = 500;
            if (list.Remove(item))
            {
                Console.WriteLine($"Removed item: {item}");
            }
            else
            {
                Console.WriteLine($"ITEM NOT REMOVED: {item}");
            }
        }
    }


    public static void RunConcDictBench()
    {
        var dict = new ConcurrentDictionary<int, int>();
        var stopwatch = Stopwatch.StartNew();
        Parallel.Invoke(AddItemsInParallel, GetItemsInCycle, GetAndRemoveItem);
        Console.WriteLine($"Elapsed: {stopwatch.Elapsed}");
        Console.WriteLine();
        Console.WriteLine($"Result count: {dict.Count}");

        void AddItemsInParallel()
        {
            Parallel.For(0, 100_001, i =>
            {
                dict.TryAdd(i, i);
            });
        }
        void GetItemsInCycle()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(dict.ToArray());
            }
        }
        
        void GetAndRemoveItem()
        {
            Thread.Sleep(1);
            var item = 500;
            if (dict.Remove(item, out _))
            {
                Console.WriteLine($"Removed item: {item}");
            }
            else
            {
                Console.WriteLine($"ITEM NOT REMOVED: {item}");
            }
        }
    }

}