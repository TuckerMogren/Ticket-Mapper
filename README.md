# Ticket-Mapper-Dev Project

## Description
Ticket-Mapper is a robust .NET application that was created to streamline the process of creating ticket for the [Lake George Alumni Association (LGAA)](https://lgalumni.com) raffle that took place in the fall of 2023. This project is structured with a clear separation of concerns, dividing the application into domain, application, infrastructure, and presentation layers, each with dedicated test projects to ensure stability and performance.

---

## Installation and Setup

### Prerequisites:
- .NET [8.0]
- 

### Setting up the environment:
1. Clone the repository to your local machine.
2. Navigate to the project's root directory.
3. Fill out the application secrets.
4. Set the Web API project as startup project.
5. Run and communcaite with API using swagger.

---

## Running the Project

### Building the Project:
1. Open the `TicketMapper.sln` in your preferred IDE.
2. Restore the NuGet packages.
3. Build the solution to verify everything is set up correctly.

### Running the Application:
1. Set `TicketMapper.WebApi` as the startup project.
2. Run the application. This should start the web server and make the API accessible.

---

## Testing

### Running the Tests:
- Navigate to the test directories (`TicketMapper.Test.Application`, `TicketMapper.Test.Domain`, `TicketMapper.Test.WebApi`).
- Use the test runner in your IDE or a CLI tool to run the tests.
- Ensure all tests pass to verify system integrity.

---

## Project Structure Overview

- **TicketMapper.Application**: Contains application logic and orchestrates domain and infrastructure interactions.
- **TicketMapper.Domain**: Defines the business models and rules.
- **TicketMapper.Infrastructure**: 
  - **FileIO**: Manages file input/output operations.
  - **Document**: Handles document processing tasks.
- **TicketMapper.WebApi**: Exposes the application's functionality over HTTP.
- **Test Projects**: Ensure the reliability and correctness of the application through automated tests.

---

## Contribution Guidelines

To contribute to Ticket-Mapper:
1. Fork the repository.
2. Create a feature branch.
3. Make your changes and write tests to ensure they work and don't break existing features.
4. Submit a pull request with a clear list of what you've done.


---

## Docker Commands

To run APIs via Docker
1. Navigate to src folder
1. run command `docker build -t ticketmapperwebapi .`
1. Then run command `docker run -d -p 8080:80 --name myticketmappercontainer ticketmapperwebapi` to run container


---

## License

This project is licensed under the [License Name] - see the `LICENSE` file for details.
