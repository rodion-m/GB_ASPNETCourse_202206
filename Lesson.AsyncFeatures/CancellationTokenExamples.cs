namespace Lesson.AsyncFeatures;

public class CancellationTokenExamples
{
    void Caller()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        try
        {
            DoHeavyJob(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Операция отменена");
        }
    }
    
    void DoHeavyJob(CancellationToken cancellationToken)
    {
        for (int i = 0; i < 100_000_000; i++)
        {
            NotifyUser(i);
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    private void NotifyUser(int i)
    {
    }
}