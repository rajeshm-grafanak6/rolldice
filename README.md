# Dice App

A simple .NET-based web application that simulates rolling a dice. The application includes OpenTelemetry integration for monitoring, tracing, and logging.

## Features

- Roll a virtual dice (1-6)
- Optional player name tracking
- OpenTelemetry integration for:
  - Tracing
  - Metrics
  - Logging
- Docker support
- RESTful API endpoint

## Prerequisites

- .NET 6.0 or later
- Docker (optional)
- OpenTelemetry Collector (for full observability features)

## Getting Started

### Local Development

1. Clone the repository
2. Navigate to the project directory
3. Run the application:
   ```bash
   dotnet run
   ```
4. Access the application at `http://localhost:8889`

### Using Docker

1. Build the Docker image:
   ```bash
   docker build -t dice-app .
   ```
2. Run the container:
   ```bash
   docker run -p 8889:80 dice-app
   ```

## API Usage

### Roll Dice Endpoint

```
GET /rolldice/{player?}
```

- `player` (optional): Name of the player rolling the dice
- Returns: A number between 1 and 6

Example:
```bash
# Anonymous roll
curl http://localhost:8889/rolldice

# Roll with player name
curl http://localhost:8889/rolldice/John
```

## Observability

The application is configured with OpenTelemetry for comprehensive monitoring:

- **Tracing**: Track request flows and application behavior
- **Metrics**: Monitor application performance
- **Logging**: Structured logging with player information and roll results

The OpenTelemetry collector endpoint is configured to `http://127.0.0.1:4317` by default.

## Project Structure

- `Program.cs`: Main application code
- `Dockerfile`: Container configuration
- `appsettings.json`: Application configuration
- `config.alloy`: Additional configuration

## License

This project is open source and available under the MIT License. 