# Kafka Stream with DotNet Core C#

install package Moonlay.Confluent.Kafka

```bash
dotnet add package Moonlay.Confluent.Kafka
```

## Topics

Create a Standard Lib Project then add package *Confluent.SchemaRegistry.Serdes*

Create Key type of Avro Message:
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

Create Value type of Avro Message file: Customer_NewCustomer.avsc

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

### Serialize all avro messages to C# code.
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

| appsettings.json

```json
...
"Kafka": {
    "BootstrapServers": "<Kafka Broker Hosts, use comma for more brokers>",
    "SchemaRegistryUrl": "<Kafka Schema Registry Host>"
  },
...
```

### How to Use

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

