# Bet.Azure

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/kdcllc/Bet.Azure/master/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Bet.Azure.Messaging.svg)](https://www.nuget.org/packages?q=Bet.Azure.Messaging)
![Nuget](https://img.shields.io/nuget/dt/Bet.Azure.Messaging)


![Stand With Israel](./img/IStandWithIsrael.png)

> The second letter in the Hebrew alphabet is the ב bet/beit. Its meaning is "house". In the ancient pictographic Hebrew it was a symbol resembling a tent on a landscape.

The goal of this repo is to provide with a reusable functionality for developing with Azure Cloud.

The libraries are closely integrate with [Azure SDK](https://azure.microsoft.com/en-us/downloads/).

## Hire me

Please send [email](mailto:info@kingdavidconsulting@.com) if you consider to **hire me**.

[![buymeacoffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/vyve0og)

## Give a Star! :star:

If you like or are using this project to learn or start your solution, please give it a star. Thanks!

## `Bet.Azure.Messaging` library

This library provides ability to handle messages thru Handlers.

- [`Bet.Azure.Messaging`](./src/Bet.Azure.Messaging/)
- [`Bet.Azure.Messaging.Sample`](./src/Bet.Azure.Messaging.Sample/)

## Features

- **Azure Service Bus Integration**: Provides seamless integration with Azure Service Bus for queue and topic-based messaging.
- **Dependency Injection**: Built-in support for Dependency Injection to enhance testability and maintainability.
- **JSON Serialization**: Ensures interoperability by using JSON for message serialization.
- **Asynchronous Programming**: Leverages asynchronous programming for improved performance and scalability.

## Usage Examples

### Sending a Message

```csharp
using Bet.Azure.Messaging;

var options = new AzureServiceBusOptions
{
    ConnectionString = "<Your Connection String>",
    QueueName = "sample-queue"
};

IAzureServiceBusConnection serviceBusConnection = new AzureServiceBusConnection(options);
await serviceBusConnection.SendMessageAsync("{ \"key\": \"value\" }");
```

### Receiving a Message

```csharp
using Bet.Azure.Messaging;

var options = new AzureServiceBusOptions
{
    ConnectionString = "<Your Connection String>",
    QueueName = "sample-queue"
};

IAzureServiceBusConnection serviceBusConnection = new AzureServiceBusConnection(options);
var message = await serviceBusConnection.ReceiveMessageAsync();
Console.WriteLine(message);
```

### Handling Large Messages

```csharp
using Bet.Azure.Messaging;

var message = "<Your Large Message>";
if (message.Length > 256 * 1024) // Example max size
{
    throw new MessageTooLargeException();
}
```

## Documentation

For detailed documentation, refer to the [Azure Messaging Schema Specification](./spec/schema-azure-messaging.md).
