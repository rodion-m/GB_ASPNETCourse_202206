using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Lesson.MultiThreading;

public class ThreadSafeCatalog
{
    public record Product(Guid Id, string Name);

    private readonly ConcurrentDictionary<Guid /* Id */, Product> _productsDict = new();

    public int Count => _productsDict.Count;
    
    public void Add(Product product) => _productsDict.TryAdd(product.Id, product);

    public void Remove(Product product) => _productsDict.TryRemove(product.Id, out _);
    
    public IReadOnlyCollection<Product> GetAll() => _productsDict.Values.ToArray();
}






public class ConcurrentListD<T> where T: class
{
    private readonly ConcurrentDictionary<T, T> _productsDict = new();

    public int Count => _productsDict.Count;
    
    public void Add(T item) => _productsDict.TryAdd(item, item);

    public void Remove(T item) => _productsDict.TryRemove(item, out _);
    
    public IReadOnlyCollection<T> GetAll() => _productsDict.Values.ToArray();
}