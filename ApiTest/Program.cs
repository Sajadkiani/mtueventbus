using ApiTest;
using ApiTest.Consumers;
using MtuEventBus;
using MtuEventBus.Consumers;
using MtuEventBus.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Mtu bus configs
builder.Services.AddScoped<MtuConsumer, Test2Consumer>();
builder.Services.AddScoped<MtuConsumer, TestConsumer>();
builder.Services.AddMtuBus(builder.Configuration, sectionName:"RabbitMq");



builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Map("publish/evt", async (IIntegrationEventDispatcher eventDispatcher) =>
{
    await eventDispatcher.PublishAsync(nameof(TestIntegrationEvent),
        new TestIntegrationEvent("test user", Guid.NewGuid(), DateTime.Now,
            Guid.NewGuid()));      
       
    await eventDispatcher.PublishAsync(nameof(Test2IntegrationEvent),
        new Test2IntegrationEvent("test 2 user", Guid.NewGuid(), DateTime.Now,
            Guid.NewGuid()));      
});

app.Run();