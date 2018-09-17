
# ALTEN Challenge

The solution objective is to enable one of our partners to view the status of the connection among some vehicles on a monitoring display.

## Description
The vehicles send the status of the connection one time per minute. The status can be compared with a ping (network trace); no request from the vehicle means no connection.

<strong>Vehicle Status can be classified as below:</strong>

- <strong>Vehicle is connected</strong> This is presented as ON status in green color.
- <strong>Vehicle is disconnected</strong> This is presented as OFF status in red color.

Empty result on the screen means no vehicle gives the required status.

## Solution Architecture:

![enter image description here](https://github.com/diaakhateeb/ALTENChallenge/blob/master/VehicleStatusLiveMonitor/Resources/images/alten-architecture.PNG)
As we see here, the application consists of a front-end layer which is developed using Angular MVC ASP.NET Core (SPA), and middle-ware layer that consists of API gateway that communicates with the backend services.
   
### 1. [Front-end (FE)](https://github.com/diaakhateeb/ALTENChallenge/tree/master/VehicleStatusLiveMonitor):
The layer is developed using AngularJS and TypeScript. It has Restful services (WebAPIs) for Customer, Vehicle and Connection Status application main entities. Such services communicate with the middleware through core TypeScript service libraries [CustomerService](https://github.com/diaakhateeb/ALTENChallenge/blob/master/VehicleStatusLiveMonitor/ClientApp/services/customer-service.ts), [PingTransactionService](https://github.com/diaakhateeb/ALTENChallenge/blob/master/VehicleStatusLiveMonitor/ClientApp/services/ping-transaction-service.ts), [VehicleService](https://github.com/diaakhateeb/ALTENChallenge/blob/master/VehicleStatusLiveMonitor/ClientApp/services/vehicle-service.ts). In addition, there is a timer service ([TimerService](https://github.com/diaakhateeb/ALTENChallenge/blob/master/VehicleStatusLiveMonitor/ClientApp/services/timer-service.ts)) that calculates the next time where Vehicles will be signaled and updates the view instantly.

### 2. Middleware (MW):
It uses the BFF pattern to work in full duplex mode where it communicates the FE via WebAPi services and BE through Microservices APIs. MW layer uses [RabbitMQ](https://github.com/diaakhateeb/ALTENChallenge/blob/master/RabbitMQEventBus/MqService.cs) as a communication base for Vehicles signaling using Publisher/Subscribe technique.

### 3. Backend (BE):
It consists of [SQL Server database](https://github.com/diaakhateeb/ALTENChallenge/blob/master/VehicleStatusLiveMonitor/AppData/TextFile.txt) which stores Customer, Vehicle and Signaling status transactions.

From the ***end user perspective***, a workflow of a full signaling transaction would be as follow:

 1. Query Vehicles with signal status checking command.
 2. Wait for Vehicles to reply their statuses (On/Off).
 3. Bind result to user interface.
 4. Auto signaling command would be fired the minute after.

From the ***technical perspective***, a workflow would be as follow:
1. Based on user direction, Angular UI calls core TypeScript API services.
2. Such services create Ajax requests to WebAPi Restful services controllers.
3. The WebAPi services use repository objects to call proper backend microservices to perform CRUD operations for Customer, Vehicles and Vehicles pinging transactions in addition to creating scheduler to send/receive signal status of vehicles (RabbitMQ Event Bus).

### Unit Testing:
Every solution component has unit testing project:

 - **[ConnectionStatusUnitTest](https://github.com/diaakhateeb/ALTENChallenge/tree/master/DataDomainService.ConnectionStatus.UnitTest):** It has ping_vehicle_status_only_on() and ping_vehicle_status_only_off() unit tests.
- **[CustomerUnitTest](https://github.com/diaakhateeb/ALTENChallenge/tree/master/DataDomainService.Customer.UnitTest):** It has send_signal_to_all_customer_vehicles(), send_signal_to_non_existed_customer_vehicles_return_not_null and get_associated_vehicles_not_null() unit tests.
- **[VehicleUnitTest](https://github.com/diaakhateeb/ALTENChallenge/tree/master/DataDomainService.Vehicle.UnitTest):** It has send_signal_to_all_vehicles(), send_signal_to_non_existed_vehicle_return_not_null(), unassociate_vehicles_return_not_null() and get_associated_vehicles_not_null_if_no_result() unit tests.
- **[PatternsUnitTest](https://github.com/diaakhateeb/ALTENChallenge/tree/master/DataDomainService.Patterns.UnitTest):** It has ConnectionStatusFactoryUnitTest with create_connection_status_context_repository_object_using_factory_pattern() unit test, CustomerStatusFactoryUnitTest with create_customer_context_repository_object_using_factory_pattern() unit test and VehicleStatusFactoryUnitTest with create_vehicle_context_repository_object_using_factory_pattern unit test.
 - **[RabbitMQEventBusUnitTest](https://github.com/diaakhateeb/ALTENChallenge/tree/master/RabbitMQEventBus):** It has publish_vehicle_signal_transactions_to_rabbitmq() and subscribe_to_vehicle_signal_transactions_rabbitmq() unit tests.

The solution events including errors and exceptions are logged into a text file that is created by default at the system partition. The location can be changed when creating an object of the logging class. 

## Business functions application provides

 1. **[Customer](https://github.com/diaakhateeb/ALTENChallenge/blob/master/VehicleStatusLiveMonitor/Controllers/CustomerServiceController.cs):**
    - A- CRUD operations (On Delete, Customer must be NOT attached to a Vehicle).
    - B- Attach Vehicle.
    - C- Detach Vehicle.
    
  2. **[Vehicle](https://github.com/diaakhateeb/ALTENChallenge/blob/master/VehicleStatusLiveMonitor/Controllers/VehicleServiceController.cs):**
     - A- CRUD operations (On Delete, Vehicle must be NOT attached to a Customer). Vehicle is not physically deleted as it is the main system entity and could be attached to another Customer.
     - B- Attach Customer.

3. **[Signaling](https://github.com/diaakhateeb/ALTENChallenge/blob/master/VehicleStatusLiveMonitor/Controllers/VehicleConnectionStatusServiceController.cs):**
   - A- Ping Vehicle every 60 seconds and get status.
   - B- Filter signaling by Customer, Vehicle and specific Status.
   - C- Connection status is stored into database before binding to UI.
   - D- Signaling result is published into RabbitMQ Event Bus.
   - E- UI subscribes to the event bus and fetches the result.
 
 All the operations are developed in integrated components (services) that correlate to achieve the correct output.

## Deployment on Cloud
The application is hosted on GCP (Google Cloud Platform). You can follow one of the below pathways:
 - **Application Web Service**:
You create App Web Service and publish the application files to its directory. The cloud platform gives you capability to execute your database script through an online editor or you can connect from your local editor to the online engine.
 - **Virtual Machine**:
you can create VM over cloud and install the required perquisite software such as web server, .NET Core framework, SQL Server database engine and publishing service.

**Deployment approaches:**

 - Using Visual Studio:
   - Publish your project, whether in a folder or over cloud using cloud platform publishing service (you need to install the cloud VS plugin).
   - If you published in a folder, copy the files and past it on the cloud web folder where application is hosted.
   - Configure the settings of database connection string and .NET Core exe path (dotnet.exe).
   - Configure the web server to identify the application as a .NET Core app.
   - Run the database script to create database and tables.
   - Run the application from the browser.

- Using [Docker](https://github.com/diaakhateeb/ALTENChallenge/blob/master/Dockerfile):
  - Install docker daemon.
  - From CLI, execute "docker build -t vehiclestatuslivemonitor ."
  - Then, execute "docker run -d -p 8080:80 --name myapp vehiclestatuslivemonitor" to open the browser and fire the application.
  - execute "gcloud docker -- push vehiclestatuslivemonitor"
 
 The application is published on a [GCP VM](http://35.231.199.19:8080/) using IIS deployment service.

## Serverless Architecture
Serverless architecture (**FaaS**) is a software design pattern where application is hosted by a third-party service, eliminating the need for server software and hardware management by the developer. Application is broken up into individual functions that can be invoked and scaled individually (e.g. Microservices).
Our monitoring application can take advantage of FaaS through architecting the system functions into separate components (services) such as Customer service, Vehicle Service and ConnectionStatus service that are hosted separately and connect to each other via communication base (Event Bus).

## Technical Design Highlights
As a simple solution, it applies the following principles and  patterns.
- **Dependency Inversion Principle**: DI principle aims to decouple the high level components implementation from the low level ones and both should depends on abstraction (Interfaces). The solution here applies such principle using Microsoft Unity container IoC (Inversion of Control), and It is used through the Factory pattern class to instantiate Repository pattern class object. It injects validations objects parameters through the constructor.
- **Factory Pattern**: It is one of the creational patterns that aims to create class object without having to specify the exact class of the object that will be created. It is common pattern and widely used nowadays as it fits all software applications sizes. The solution applies it in conjunction with the IoC Unity to create Repository class object.
- **Repository Pattern**: It is widely used in the DDD, and it builds an interface between the core components and outer world. It decouples the business layer from the front layer such as providing database model functions to the MW layer. This gives us the liberality to change database or the model as a whole with no need to change the layer implementation. The solution applies it to expose the microservices functions including RqbbitMQ Publish/Subscribe transactions.
- **API Gateway Pattern**: Works as single entry points for all clients. It handles requests in one of two ways. Some requests are simply routed to the appropriate service while others handled by fanning out to multiple services.
- **Publish/Subscribe Pattern**: It is a messaging pattern where senders of messages, called publishers, do not program the messages to be sent directly to specific receivers, called subscribers, but instead categorize published messages into objects without knowledge of which subscribers. Similarly, subscribers express interest in one or more objects and only receive messages that are of interest, without knowledge of which publishers. It has three contributors: **producer** (is a user application that sends messages), **queue** (is a buffer that stores messages) and **consumer** (is a user application that receives messages).
