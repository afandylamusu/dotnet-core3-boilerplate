# Kafka Stream with DotNet Core C#

install package Moonlay.Confluent.Kafka

```bash
dotnet add package Moonlay.Confluent.Kafka
```

## Topics

A **Topic** is a category/feed name to which messages are stored and published. Messages are byte arrays that can store any object in any format. As said before, all Kafka messages are organized into topics. If you wish to send a message you send it to a specific topic and if you wish to read a message you read it from a specific topic. Producer applications write data to topics and consumer applications read from topics. Messages published to the cluster will stay in the cluster until a configurable retention period has passed by. Kafka retains all messages for a set amount of time, and therefore, consumers are responsible to track their location.

| Create a Standard Lib Project then add package *Confluent.SchemaRegistry.Serdes*

| Create Key type of Avro Message:
MessageHeader.avsc

```json
{
  "name": "MessageHeader",
  "type": "record",
  "namespace": "Moonlay.Topics",
  "fields": [
    {
      "name": "Token",
      "type": "string"
    },
    {
      "name": "CurrentUser",
      "type": "string"
    },
    {
      "name": "IsCurrentUserDemo",
      "type": "boolean"
    },
    {
      "name": "AppOrigin",
      "type": "string"
    },
    {
      "name": "Timestamp",
      "type": "string"
    }
  ]
}
```

| Create Value type of Avro Message file: Customer_NewCustomer.avsc

```json
{
  "name": "NewCustomerTopic",
  "type": "record",
  "namespace": "Moonlay.Topics.Customers",
  "fields": [
    {
      "name": "FirstName",
      "type": "string"
    },
    {
      "name": "LastName",
      "type": "string"
    }
  ]
}
```

| Serialize all avro messages to C# code.
Install AvroGen Tools

```bash
dotnet tool install --global Confluent.Apache.Avro.AvroGen
```
Usage:

```
avrogen -s your_schema.avsc .
```

## Producer

| Register Moonlay.Confluent.Kafka.IKafkaProducer in Startup class

```csharp
...
services.AddSingleton(c => new SchemaRegistryConfig {
 Url = Configuration.GetSection("Kafka:SchemaRegistryUrl").Value,
  RequestTimeoutMs = 5000,
  MaxCachedSchemas = 10
});

services.AddSingleton < ISchemaRegistryClient > (c => new CachedSchemaRegistryClient(c.GetRequiredService < SchemaRegistryConfig > ()));
services.AddSingleton(c => new ProducerConfig() {
 BootstrapServers = Configuration.GetSection("Kafka:BootstrapServers").Value
});

services.AddScoped<IKafkaProducer, KafkaProducer>();
...
```

| Add kafka config in appsettings.json

```json
...
"Kafka": {
    "BootstrapServers": "<Kafka Broker Hosts, use comma for more brokers>",
    "SchemaRegistryUrl": "<Kafka Schema Registry Host>"
  },
...
```

| Usage IKafkaProducer

Here's the sample of using IKafkaProducer in Razor Page.
You can find in *Moonlay.MasterData.WebApp/Pages/Customer/Create.cshtml.cs*

Add IKafkaProducer as param on the constructor.

```cs
public class CreateModel: PageModel {
...
 public CreateModel(IKafkaProducer producer, ISignInService signIn) {
  _producer = producer;
  _signIn = signIn;
 }
...
}

```
Send Topic through *IKafkaProducer.Publish(key,value)*

```cs
...
 public async Task < IActionResult > OnPostAsync() {
  if (!ModelState.IsValid) {
   return Page();
  }

  await _producer.Publish("new-customer-topic2", _signIn.GenMessageHeader(),
   new NewCustomerTopic {
    FirstName = this.Form.FirstName,
     LastName = this.Form.LastName
   });

  return RedirectToPage("./Index");
 }
...
```

## Consumer
| Create a Consumer class which extend from KafkaConsumer<TKey, TValue>.  ex. to handle NewCustomerTopic


```cs
public interface INewCustomerConsumer: IKafkaConsumer < MessageHeader, NewCustomerTopic > {}

public class NewCustomerConsumer: KafkaConsumer < MessageHeader, NewCustomerTopic > , INewCustomerConsumer {
  private readonly ICustomerUseCase _service;

  public NewCustomerConsumer(ILogger < NewCustomerConsumer > logger, ISchemaRegistryClient schemaRegistryClient, ConsumerConfig config, ICustomerUseCase service): base(logger, schemaRegistryClient, config) {
   _service = service;
  }

  public override string TopicName => "new-customer-topic2";

  protected override async Task ConsumeMessages(ConsumeResult < MessageHeader, NewCustomerTopic > message) {
   await _service.NewCustomerAsync(message.Value.FirstName, message.Value.LastName, c => {
    c.CreatedBy = message.Key.CurrentUser;
    c.UpdatedBy = message.Key.CurrentUser;
    c.Tested = message.Key.IsCurrentUserDemo;
   });
  }
```

| Register Consumers in Startup class

```csharp
...
services.AddHostedService < MasterDataHostConsumers > ();

services.AddSingleton(c => new SchemaRegistryConfig {
 Url = Configuration.GetSection("Kafka:SchemaRegistryUrl").Value,
  RequestTimeoutMs = 5000,
  MaxCachedSchemas = 10
});

services.AddSingleton < ISchemaRegistryClient > (c => new CachedSchemaRegistryClient(c.GetRequiredService < SchemaRegistryConfig > ()));

services.AddSingleton(c => new ConsumerConfig {
 GroupId = "mdm-consumer-group",
  BootstrapServers = Configuration.GetSection("Kafka:BootstrapServers").Value,
  AutoOffsetReset = AutoOffsetReset.Earliest
});
...
```

Register consumers by scope

```cs
...
services.AddScoped < INewCustomerConsumer, NewCustomerConsumer > ();
services.AddScoped < IUpdateCustomerConsumer, UpdateCustomerConsumer > ();
...
```

| Add kafka config in appsettings.json

```json
...
"Kafka": {
    "BootstrapServers": "<Kafka Broker Hosts, use comma for more brokers>",
    "SchemaRegistryUrl": "<Kafka Schema Registry Host>"
  },
...
```