using System.Reflection;
using Lesson.DomainEventsWithMediatR;
using Lesson.DomainEventsWithMediatR.DomainEvents.Events;
using Lesson.DomainEventsWithMediatR.DomainEvents.Handlers;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ScopedDependency>();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

var app = builder.Build();
app.MapGet("/", async (IMediator mediator) =>
{
    _ = mediator.Publish(new IndexPageOpened(DateTimeOffset.Now));
    await Task.Delay(10);
    return "Hello World!";
});

app.Run();