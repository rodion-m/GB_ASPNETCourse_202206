namespace Lesson.MultiThreading;

public class InterlockedExamples
{
    private long i;
    private Product _product = new();
    private DateTime _dateTime = new();
    
    public void Examples()
    {
        Interlocked.Increment(ref i); //атомарный инкремент
        Interlocked.Decrement(ref i); //атомарный декремент
        Interlocked.Add(ref i, 100); //атомарное сложение
        Interlocked.Add(ref i, -100); //атомарное вычитание
        long val = Interlocked.Read(ref i); //чтение с барьером памяти
        Interlocked.Exchange(ref i, 10); //запись с барьером памяти
        
        //Interlocked.Increment(ref _product); //нет такой перегрузки
        //Interlocked.Exchange(ref _dateTime, _dateTime); //имеет ограничение T: class?
    }
}