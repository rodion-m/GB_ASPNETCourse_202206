using Lesson.DI.Entities;

namespace Lesson.DI.DomainEvents;

public class ProductAdded : IDomainEvent
{
    public Product Product { get; }
    
    public ProductAdded(Product product)
    {
        Product = product;
    }
}