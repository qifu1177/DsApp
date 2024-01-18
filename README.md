# SolutionName: DsApp
This Project includes a .net 6 api and a angular app.
## Projects:
### Data 
    - .Net 6.0 Class Library handles every DataAccess also HTTPDataAccess for assets, Uses RepositoryPattern and UnitOfWork Pattern
### Web 
    - .Net 6.0 Kestrel Host, Hosts the static content as well as a WebAPI for the Frontend
### Domain 
    – .Net 6.0 Class Library Containing Business Logic, Validators, Interfaces and Models
## The Api should have the following actions:
### POST for Creating an Object 
    – returning an 200 on successful creation of the object and the url were the object can be called
### GET with id parameter 
    – to ask for an object by id
### PUT 
    – to update the object with the given id
### DELETE 
    – to delete the object with the given id
## Rules:
- The WebApi accepts and returns application/json data.
- The object and the properties should be validated by fluentValidation
- If the object is invalid ( on post and put ) – return 400 and an information what property does not fullyfy the requirements and which requirement is not fullyfied.
- Describe the API with swagger therefore use Swashbuckle host the swaggerUI under [localhost]/swagger.
- Provide example data in the SwaggerUI, so when someone click on try it out there is already useful valid data in the object that can be posted.
- For all strings, use localization and a Jsonfile as resource file.
- Use autofac for dependency injection
- Write the log to a serilog rolling file sink the name needs to be setable in the applicationsettings.json file. 
## Frontend:
The including Form must be an Angular Application which uses the API to Post Data AND Validate all the inputs with the exact same parameters as the API does.
- use Typescript


## Run the app with docker
### Installation
For the project shoud be installed docker and nodejs
- Docker installation, go to https://www.docker.com/get-started
- nodejs installation, go to https://nodejs.org/en/download/

### Build for production
- Go to folder "DsApp\web", then run `npm install`, then run `npm run build`
### Run 
- Go to folder "DsApp", then run `docker-compose up`, then open `http://localhost:5000` 

