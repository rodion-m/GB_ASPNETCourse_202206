using Microsoft.Win32.SafeHandles;

namespace Lesson.AsyncFeatures;

public class AsyncDisposableDemo
{
    public static async Task Run()
    {
        var fn = "file.txt";
        var options = new FileStreamOptions()
        {
            Options = FileOptions.Asynchronous,
            Mode = FileMode.OpenOrCreate
        };
        var fs = File.Open(fn, options);
        await fs.WriteAsync(new byte[100]);
        await fs.DisposeAsync();
    }
}