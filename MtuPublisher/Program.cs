using MtuPublisher;
using MtuEventBus;
using MtuEventBus.Consumers;
using MtuEventBus.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Mtu bus configs
builder.Services.AddMtuBus(builder.Configuration, sectionName:"RabbitMq");



builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(otp => otp.SwaggerEndpoint("/openapi/v1.json", "MtuPublisher"));
}

app.UseHttpsRedirection();


app.MapPost("publish/evt", async (IIntegrationEventDispatcher eventDispatcher) =>
{
    await eventDispatcher.PublishAsync(new TestIntegrationEvent("test user", Guid.NewGuid(), DateTime.Now,
            Guid.NewGuid()));      
       
    await eventDispatcher.PublishAsync(new Test2IntegrationEvent("test 2 user", Guid.NewGuid(), DateTime.Now,
            Guid.NewGuid()));      
});

app.Run();