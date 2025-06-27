---
title: Azure Messaging Schema Specification
version: 1.0
date_created: 2025-06-27
last_updated: 2025-06-27
owner: kdcllc
tags: ['schema', 'azure', 'messaging']
---

# Introduction

This specification defines the schema and requirements for the Azure Messaging components implemented in the Bet.Azure repository. The goal is to provide a clear and structured definition of the messaging system, ensuring scalability, security, and maintainability.

## 1. Purpose & Scope

The purpose of this specification is to document the schema and interfaces for the Azure Messaging system. It is intended for developers and architects working on the Bet.Azure project. Assumptions include familiarity with Azure Service Bus and .NET 8.

## 2. Definitions

- **Azure Service Bus**: A fully managed enterprise message broker.
- **SAS Token**: Shared Access Signature token used for authentication.
- **Consumer Pool**: A set of consumers handling messages from queues or topics.
- **Producer Pool**: A set of producers sending messages to queues or topics.

## 3. Requirements, Constraints & Guidelines

- **REQ-001**: The system must support both queue and topic-based messaging.
- **REQ-002**: Messages must be serialized using JSON.
- **SEC-001**: SAS tokens must be securely generated and stored.
- **CON-001**: The system must use Dependency Injection for all services.
- **GUD-001**: Follow SOLID principles in the design of messaging components.
- **PAT-001**: Use asynchronous programming for message handling.

## 4. Interfaces & Data Contracts

### AzureServiceBusOptions

```csharp
public class AzureServiceBusOptions
{
    public string ConnectionString { get; set; }
    public string QueueName { get; set; }
    public string TopicName { get; set; }
}
```

### IAzureServiceBusConnection

```csharp
public interface IAzureServiceBusConnection
{
    Task SendMessageAsync(string message);
    Task<string> ReceiveMessageAsync();
}
```

## 5. Rationale & Context

The Azure Messaging system is designed to leverage Azure Service Bus for reliable and scalable messaging. JSON serialization ensures interoperability, and Dependency Injection enhances testability and maintainability.

## 6. Examples & Edge Cases

### Example: Sending a Message

```csharp
await serviceBusConnection.SendMessageAsync("{ \"key\": \"value\" }");
```

### Edge Case: Handling Large Messages

```csharp
if (message.Length > maxSize)
{
    throw new MessageTooLargeException();
}
```

## 7. Validation Criteria

- Unit tests must cover all interfaces and methods.
- Integration tests must validate message sending and receiving.
- Security tests must ensure SAS token integrity.

## 8. Related Specifications / Further Reading

- [Azure Service Bus Documentation](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview)
- [Dependency Injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [Asynchronous Programming in C#](https://learn.microsoft.com/en-us/dotnet/csharp/async)
