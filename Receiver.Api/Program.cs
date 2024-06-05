using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQDemo.Core;
using Receiver.Api.Models;
using Receiver.Api.Models.Entities;
using Receiver.Api.Models.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMassTransit(x => //Setup RabbitMQ configuration using Mass transient library
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<ProductCreatedConsumer>(); //This line will take of setting up infra,so Messagign can function correctky
    x.AddConsumer<ProductViewedConsumer>();

    x.UsingRabbitMq((context, config) =>
    {//using MassTransinet we dont need to manage infra ,give control of infra to library
        config.Host(new Uri("rabbitmq://localhost"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        config.ConfigureEndpoints(context);
        //config.ReceiveEndpoint(queueName:"send-demo", configureEndpoint: e => //convert configs to require Topolgy on broker,//will create exchanges,queues and bind them to messges i am sending to my app
        //{ 
        //    e.Consumer<SenderDemo>(); 
        //});
    });

    //Add Consumer
    //x.AddConsumer<Prouctc>();
});
builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
    options.StartTimeout = TimeSpan.FromSeconds(30);
    options.StopTimeout = TimeSpan.FromMinutes(1);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
