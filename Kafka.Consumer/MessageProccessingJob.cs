using Confluent.Kafka;
using Microsoft.Extensions.Hosting;

namespace Kafka.Consumer
{
    internal class MessageProccessingJob(IConsumer<string, string> kafkaConsumer)
        : BackgroundService
    {  
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ConsumeResult<string, string>? deliveryResult = null;
            kafkaConsumer.Subscribe(topic: "topic_1");

            while (!stoppingToken.IsCancellationRequested)
            {

                deliveryResult = kafkaConsumer.Consume(TimeSpan.FromSeconds(10));
                if(deliveryResult == null)
                {
                    continue;
                }
                Console.WriteLine($"Key: {deliveryResult.Message.Key}, Value: {deliveryResult.Message.Value}");
                kafkaConsumer.Commit(deliveryResult);

            }
            return Task.CompletedTask;
        }      
    }
}
