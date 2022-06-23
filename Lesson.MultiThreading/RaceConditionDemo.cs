namespace Lesson.MultiThreading;

public class RaceConditionDemo
{
    private int i = 0;
    private int i2 = 0;
    public void DoWork()
    {
        i2++;
        i++;
    }

    private bool completed = false;
    private static TemperatureByDate original = new("Moscow", new DateOnly(2022, 7, 1), 30);
    private static TemperatureByDate copy = null!;
    public static void CreateIncorrectModel()
    {
        Task.Run(RunWatcher);
        Thread.Sleep(0);
        copy = new TemperatureByDate();
        original.CopyTo(copy);
    }

    private static void RunWatcher()
    {
        Parallel.For(0, 100_000, i =>
        {
            if (!original.Equals(copy))
            {
                Console.WriteLine(copy);
            }
        });
    }

    class TemperatureByDate
    {
        protected bool Equals(TemperatureByDate other)
        {
            return City == other.City && Date.Equals(other.Date) && Temperature.Equals(other.Temperature);
        }

        public override bool Equals(object? obj)
        {
            return Equals((TemperatureByDate)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = City.GetHashCode();
                hashCode = (hashCode * 397) ^ Date.GetHashCode();
                hashCode = (hashCode * 397) ^ Temperature.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(TemperatureByDate? left, TemperatureByDate? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TemperatureByDate? left, TemperatureByDate? right)
        {
            return !Equals(left, right);
        }

        public string City { get; set; } = null!;
        public DateOnly Date { get; set; }
        public double Temperature { get; set; }

        public TemperatureByDate() {}
        public TemperatureByDate(string city, DateOnly date, double temperature)
        {
            Date = date;
            Temperature = temperature;
            City = city;
        }

        public void CopyTo(TemperatureByDate destination)
        {
            destination.City = City;
            destination.Date = Date;
            destination.Temperature = Temperature;
        }
    }

    private static volatile int ii = 0;
    public static void RunDemo()
    {
        Parallel.For(0, 1000000, (_) =>
        {
            ii++;
        });
        Console.WriteLine(ii);
    }
}