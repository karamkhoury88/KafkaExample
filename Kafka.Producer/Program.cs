using Aspire.Confluent.Kafka;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Kafka.Producer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();


builder.AddKafkaProducer<string, string>("kafka", settings =>
{
    settings.Config.Acks = Acks.All;
    settings.Config.MessageTimeoutMs = 300000;
    settings.Config.Partitioner = Partitioner.Random;
});


builder.Services.AddHostedService<MessageProccessingJob>();

var host = builder.Build();
host.Run();