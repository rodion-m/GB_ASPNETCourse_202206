using UnityEngine;

namespace Lesson.MultiThreading
{
    public class UnityDeadlockDemo
    {
        public static void DoDeadlock()
        {
            UnitySynchronizationContext.InitializeSynchronizationContext();
            Console.WriteLine("Сейчас будет дедлок");
            WaitOneSec().Wait();
            Console.WriteLine("И этот текст никогда не напечатается");
        }

        private static async Task WaitOneSec()
            => await Task.Delay(TimeSpan.FromSeconds(1));
    }
}
