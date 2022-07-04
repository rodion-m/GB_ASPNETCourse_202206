// See https://aka.ms/new-console-template for more information

var sync = new object();

var semaphoreSlim = new SemaphoreSlim(1, 1);
Task.Run(async () =>
{
    await semaphoreSlim.WaitAsync();
    await DoWork();
    Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
    semaphoreSlim.Release();
});
Task.Run(async () =>
{
    await semaphoreSlim.WaitAsync();
    await DoWork();
    Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
    semaphoreSlim.Release();
});
await Task.Delay(3000);


async Task DoWork()
{
    await Task.Delay(2000);
}

class Product
{
    protected Product()
    {
    }
    
    public Product(string name)
    {
        Name = name;
        if (Name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }
    }

    public string Name { get; set; } = "";

}
