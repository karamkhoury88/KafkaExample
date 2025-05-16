using Confluent.Kafka;
using Kafka.Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddKafkaConsumer<string, string>("kafka", options =>
{
    options.Config.GroupId = "my-consumer-group";
    options.Config.AutoOffsetReset = AutoOffsetReset.Earliest;
    options.Config.EnableAutoCommit = false;
});

builder.Services.AddHostedService<MessageProccessingJob>();

var host = builder.Build();
host.Run();
