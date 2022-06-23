namespace Lesson.MultiThreading;

public class FastCalculator
{
    public static long Calculate(params int[] nums)
        => nums.Sum();
    
    public static long CalculateInParallel(params int[] nums)
    {
        return nums.AsParallel().Sum();
    }
    
}