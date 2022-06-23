using System.Collections.Concurrent;
using System.Diagnostics;
using Lesson.MultiThreading.Exercises;
using Lesson.MultiThreading.Mutexes;
using Lesson.MultiThreading.ThreadSafeLists;

for (int i = 0; i < 10_000_000; i++)
{
   new TwoTasksXY().RunWithSpinLock();
   Console.WriteLine();
}
Console.WriteLine("Done");
return;

ThreadPool.GetAvailableThreads(out var t, out var asThreads);
Console.WriteLine($"workerThreads {t}" + $" async threads {asThreads}");
_ = Task.Run(async () => await Task.Run(async () => await File.WriteAllBytesAsync("file.txt", new byte[100])));
//await Task.Delay(100);
ThreadPool.GetAvailableThreads(out t, out asThreads);
Console.WriteLine($"workerThreads {t}" + $" async threads {asThreads}");

var sw = Stopwatch.StartNew();
var list = new ThreadSafeList<int>();
Parallel.For(0, 1000_000, (i) =>
{
   list.Add(i);
});


Console.WriteLine(list.Count);
Console.WriteLine(sw.Elapsed);
return;

ThreadSafeListTest.RunCustomBench();

MutexTests.TestSimpleSpinLock();
var stopwatch = Stopwatch.StartNew();
MutexTests.TestSimpleSpinLock();
Console.WriteLine(stopwatch.Elapsed);
var stopwatch2 = Stopwatch.StartNew();
MutexTests.TestSpinLock();
Console.WriteLine(stopwatch2.Elapsed);

//RaceConditionDemo.CreateIncorrectModel();

//NonBackgroundExample.DoWork();

// using System.Diagnostics;
// using static Lesson.MultiThreading.FastCalculator;
//
// var nums = Enumerable.Repeat(0, 50_000_000)
//     .Select(_ => Random.Shared.Next(50))
//     .ToArray();
//
// CalculateInParallel(nums);
// Measure(nameof(Calculate), () => Calculate(nums));
//
// Measure(nameof(CalculateInParallel), 
//     () => CalculateInParallel(nums));
//
//
// //Todo benchmark
// void Measure(string name, Action action)
// {
//     var stopwatch = Stopwatch.StartNew();
//     action();
//     stopwatch.Stop();
//     Console.WriteLine($"{name} completed in: {stopwatch.Elapsed}");
// }