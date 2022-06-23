using System.Runtime.CompilerServices;

namespace Lesson.MultiThreading;

public class LockDemo
{
    public void CallSyncedMethod()
    {
        SyncedWithAttrib();
    }
    
    public static int i;
    
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void SyncedWithAttrib()
    {
        i++;
    }

    public static void InterlockedExample()
    {
        Interlocked.Increment(ref i);
        //todo asm code
    }
}