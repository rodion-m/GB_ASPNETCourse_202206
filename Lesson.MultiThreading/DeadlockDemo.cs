namespace Lesson.MultiThreading;

public class DeadlockDemo
{
    // На этом примере можно показывать отладку дедлока
    public static void RunDemo()
    {
        var catalog = new Catalog();
        Parallel.For(0, int.MaxValue, i =>
        {
            catalog.ClearCatalog();
            catalog.ShrinkNames(0);
            if(i % 100 == 0)
                Console.WriteLine("Working " + DateTime.Now);
        });
    }
    

    private void LockExample()
    {
        object syncObj = new();
        lock (syncObj) //syncObj - объект синхронизации
        {
            /*
              критическая секция, в которой код
              будет выполняться только на одном потоке
             */
        }
    }
    
    public void DoubleLockExample()
    {
        object syncObj = new();
        lock (syncObj)
        {
            // логика...
            lock (syncObj)
            {
                //еще логика...
            }
        }
    }

    private void MonitorExample()
    {
        object syncObj = new();
        bool lockTaken = false;
        try
        {
            Monitor.Enter(syncObj, ref lockTaken);
            /*
              критическая секция, в которой код
              будет выполняться только на одном потоке
             */
        }
        finally
        {
            if (lockTaken)
            {
                Monitor.Exit(syncObj);
            }
        }
    }
}

public class Catalog
{
    private List<Category> _categories = new();
    private List<Product> _products = new();
    private object _promoActions = new();
    
    private object _syncProducts = new object();
    private object _syncCategories = new object();
    
    public void ClearCatalog()
    {
        lock (_syncCategories)
        lock (_syncProducts)
        {
            _categories.Clear();
            _products.Clear();
        }
    }

    // Урезает названия до maxCount символов
    public void ShrinkNames(int maxCount)
    {
        lock (_syncProducts)
        lock (_syncCategories)
        {
            //тут логика
        }
    }
    
    public void AddProduct(Product product)
    {
        lock (_syncProducts)
        {
            _products.Add(product);
        }
    }
    public void AddCat(Category cat)
    {
        lock (_syncCategories)
        {
            _categories.Add(cat);
        }
    }
    
    public void RemoveProduct(Product product)
    {
        lock (_syncProducts)
        {
            _products.Remove(product);
        }
    }

    public IReadOnlyList<Product> GetProducts()
    {
        lock (_syncProducts)
        {
            return _products;
        }
    }
}

public record Category;
public record Product;