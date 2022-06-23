namespace Lesson.AsyncFeatures;

public class AtomicLong
{
    private long i = 0;

    public long Value => Interlocked.Read(ref i);
    public long Increment() => Interlocked.Increment(ref i);

    public static AtomicLong operator ++(AtomicLong val)
    {
        val.Increment();
        return val;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}