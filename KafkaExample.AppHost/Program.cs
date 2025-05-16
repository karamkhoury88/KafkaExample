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
