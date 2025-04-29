# About this project!
This projects is a school project for our final exams. 
The projects is a "Too good to go" replica.
This repo contains the backend of the projects, which uses a `microservice` architecture.

## How To run the project
### Requirments
* MSSQL Server installed
* Visual Studio 2022 (Rider is able to run the project as well, however you'll be on you own when setting up the startup)

### Let's get started!
1. Clone or download the repo
2. Open the solution in Visual Studio 2022
3. Go to `appsettings.json` and change the connection string to your local MSSQL server
4. In the toolbar select `Project` and in the dropdown select `Configure startup projects`
5. In the window that opens, select `Multiple startup projects` and set the `Action` to `Start` for all projects, expect for tests
6. (Optional) Rename the startup configuration to your liking. 
7. Press `F5` to start the project (`ctrl + F5` to start without debugging))

## Architeture
The project is built using a microservice architecture. Each service has its own database, ensuring if one of the services goes down, the others will still be able to run.
The services are:
* `UserService` - Handles all user related actions, such as login, registration and user profile management.
* `MealService` - Handles all meal related actions, such as adding, updating and deleting meals.
* `OrderService` - Handles all order related actions, such as creating, updating and deleting orders.
* `VendorService` - Handles all vendor related actions, such as login, registration and vendor profile management.

All these services uses `Entity Framework` to communicate with the database.
The are behind a Gateway, which is a Microsoft YARP proxy. The gateway is responsible for routing the requests to the correct service.
The gateway is also responsible for authentication and authorization. The gateway uses `JWT` tokens to authenticate the users and vendors.
This projects utelizes `Code First`, which means that the database is created from the code. To do that the migrations will have to run, but they are run automatically when the project is starting