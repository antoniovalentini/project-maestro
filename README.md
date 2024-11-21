# Project Maestro

Prototype of a backend for a workflow engine that will define, manage, and execute workflows, automating tasks, handling human interactions, and ensuring successful completion of processes. For further info, take a look at the [Design document](./Design%20document.pdf). Visit the [Future Improvements](#future-improvements) section to get an idea of what features are missing.

The solution is made of the following projects:
- `ApiGateway`: main interface to interact with the Engine via REST API
- `EngineCore`: domain logic for workflows and tasks management
- `TaskWorkerService`: executes tasks picked from the queue
- `TaskNotificationWorkerService`: process tasks completion notifications

The web service and the background workers communicate with each other using queues. [EasyNetQ](https://easynetq.com/) is used to reduce queues management complexity.

The workflow definitions are based on a structured format (JSON) which get deserialized using .NET polymorphic deserialization. You can read more about _Polymorphic type discriminators_ [here](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism?pivots=dotnet-7-0#polymorphic-type-discriminators).

Data is stored using relation databases. Everything is handled by Entity Framework using the PostgreSQL provider ([Npgsql](https://www.npgsql.org/)).

## How to run it

Requirements:
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker Engine](https://docs.docker.com/engine/install/)
- [Docker Compose](https://docs.docker.com/compose/install/)

### Dependencies
Clone the project on your local machine, navigate to the solution directory, locate the `docker-compose.yml` file and run the following command:
```
$ docker-compose up -d
```
Make sure you got 3 running containers: `postgres`, `rabbitmq`, `pgadmin`.

### Database seed
Run the following command to install the `dotnet-ef` tool:
```
$ dotnet tool install --global dotnet-ef
```
Then navigate inside the `EngineCore` project folder and run:
```
$ dotnet ef database update
```
This should run the database migrations to setup the schema. In case of errors, try running it again.

### Web API and background workers
Navigate to the `src` folder, and run the `APIGateway`, the `TaskWorkerService` and `TaskNotificationWorkerService` in 3 separate terminals, using the following command (replace the name of the project with the one you want to run):
```
$ dotnet run --project ./src/ApiGateway/ApiGateway.csproj
```

### Test the application
Open your browser and navigate to:
```
http://localhost:5261/swagger/index.html
```
The application will pre-populate the database with a test workflow definition (`order_fulfillment_1`).
Use the Swagger UI to send a POST request to the `/api/Workflow/start` endpoint, with the following payload:

```JSON
{
    "id": "order_fulfillment_1",
    "version": "1"
}
```
You should receive a 201 response with a location header like `location: api/workflow/status/{guid}`.

Use pgAdmin (http://localhost:8080/) or any DB management tool to connect to the PostgreSQL container and make sure you have a COMPLETED workflow inside the `WorkflowInstances` table, and 4 COMPLETED tasks inside the `TasksInstances` table.

Note:<br>
if you're using pgAdmin, remember to use `myapp-postgres` as host instead of localhost when adding a new server. You can find the credentials (for both pgAdmin and postgres) inside the [docker-compose.yml](./docker-compose.yml) file.

## Future improvements

Not in a particular order:
- Database transactionality
- `/workflow/{id}/status` endpoint logic
- human interaction logic
- parallel execution of tasks
  - database locking
- timers
- workflow definitions management (insert, modify, delete definitions)
- caching
- metrics and observability
- authentication and authorization
- ... ?

## Tests
Well, there isn't much to say here. The solution only has 1 integration test which helped me during the early phases of development, and to have a quick feedback whenever I changed the DI structure.
