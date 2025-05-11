# KafkaConsumerService
KafkaConsumerService is a .NET 9 microservice that acts as a Kafka consumer. It listens to a Kafka topic, processes messages asynchronously via a decoupled message handler architecture, and persists or acts on the data using a generic repository.

The project includes:

⚙️ A Kafka consumer implemented as a hosted background service

📤 Outbox-driven Kafka producer for event publishing

🗃️ Message handling using a Strategy/Handler pattern

🧪 Testable structure with interface-based abstractions and JSON source generation

🧱 Includes SQL Server trigger-based change tracking

Use Case:

Ideal for .NET & Kafka-based event-driven systems. 
Especially useful when migrating from legacy databases, allowing the current (live) database to be mirrored into a test or development environment by transforming CRUD transactions into events. 
This is demonstrated using the example of the DISTLIST table.

Supports AOT-friendly JSON (via System.Text.Json source generators)
