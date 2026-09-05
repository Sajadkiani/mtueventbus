using System.Reflection;
using MtuEventBus;
using MtuEventBus.Consumers;
using MtuEventBus.Extensions;
using MtuSubscriber;
using MtuSubscriber.Consumers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMtuBus(builder.Configuration, Assembly.GetAssembly(typeof(TestConsumer)), sectionName:"RabbitMq");

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(otp => otp.SwaggerEndpoint("/openapi/v1.json", "subscriber"));
}

app.UseHttpsRedirection();

app.Run();