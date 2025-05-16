using Confluent.Kafka;
using Microsoft.Extensions.Hosting;

namespace Kafka.Producer
{
    internal class MessageProccessingJob(IProducer<string, string> kafkaProducer)
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

                DeliveryResult<string, string>? result =
                    await kafkaProducer.ProduceAsync("topic_1", message, stoppingToken);

                await Task.Delay(millisecondsDelay: 1000, stoppingToken);
            }
        }


    }

}
