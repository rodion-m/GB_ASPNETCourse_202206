using System.Reflection;
using Lesson.DomainEventsWithMediatR;
using Lesson.DomainEventsWithMediatR.DomainEvents.Events;
using Lesson.DomainEventsWithMediatR.DomainEvents.Handlers;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ScopedDependency>();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

var app = builder.Build();
app.MapGet("/", (IMediator mediator) =>
{
    mediator.Publish(new IndexPageOpened(DateTimeOffset.Now));
    return "Hello World!";
});

app.Run();