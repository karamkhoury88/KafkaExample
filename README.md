# .NET Aspire Apache Kafka Example

This repository provides a simple example of using Apache Kafka with a .NET Aspire project. It includes a Kafka producer and consumer implementation.

## 📌 Overview

This project demonstrates:
- **Kafka Producer**: Sends messages to a Kafka topic.
- **Kafka Consumer**: Listens for messages from a Kafka topic.
- **AppHost**: Manages service dependencies within .NET Aspire.

## 🚀 Getting Started

### Prerequisites
Ensure you have the following installed:
- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker Desktop](https://docs.docker.com/desktop/setup/install/windows-install/)

### 📦 Installation
Clone the repository:
```sh
git clone https://github.com/karamkhoury88/KafkaExample
cd KafkaExample
```

## ⚙️ Project Setup

Modify the `AppHost` to define Kafka dependencies:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddKafka("kafka")
    .WithDataVolume(isReadOnly: false)
    .WithKafkaUI();

var kafkaProducer = builder.AddProject<Projects.Kafka_Producer>("kafka-producer")
    .WithReference(kafka)
    .WaitFor(kafka);

builder.AddProject<Projects.Kafka_Consumer>("kafka-consumer")
    .WithReference(kafka)
    .WaitFor(kafka)
    .WaitFor(kafkaProducer);

builder.Build().Run();
```

## 🎭 Producer Implementation

The Kafka producer publishes messages to the `topic_1` topic.

### `Program.cs`:
```csharp
using Aspire.Confluent.Kafka;
using Confluent.Kafka;
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

builder.Services.AddHostedService<MessageProcessingJob>();

var host = builder.Build();
host.Run();
```

### Background Job:
```csharp
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;

namespace Kafka.Producer
{
    internal class MessageProcessingJob(IProducer<string, string> kafkaProducer)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = new Message<string, string>()
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = $"The UTC time now is {DateTime.UtcNow}"
                };

                await kafkaProducer.ProduceAsync("topic_1", message, stoppingToken);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
```

## 🎭 Consumer Implementation

The Kafka consumer listens for messages in `topic_1`.

### `Program.cs`:
```csharp
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

builder.Services.AddHostedService<MessageProcessingJob>();

var host = builder.Build();
host.Run();
```

### Background Job:
```csharp
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;

namespace Kafka.Consumer
{
    internal class MessageProcessingJob(IConsumer<string, string> kafkaConsumer)
        : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            kafkaConsumer.Subscribe("topic_1");

            while (!stoppingToken.IsCancellationRequested)
            {
                var deliveryResult = kafkaConsumer.Consume(TimeSpan.FromSeconds(10));
                if (deliveryResult == null) continue;

                Console.WriteLine($"Key: {deliveryResult.Message.Key}, Value: {deliveryResult.Message.Value}");
                kafkaConsumer.Commit(deliveryResult);
            }

            return Task.CompletedTask;
        }
    }
}
```

## 🏃 Running the Application

Since you're using .NET Aspire, simply run the application using:

 Start the Aspire environment:
   ```sh
   dotnet run --project AppHost
   ```

---

Happy coding! 🚀
