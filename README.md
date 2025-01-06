# Keycloak.Auth.Api

This project is an ASP.NET Core Web API that integrates with Keycloak for authentication and authorization.  
Code from following Milan Jovanovic video: https://www.youtube.com/watch?v=Blrn5JyAl6E 

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Keycloak](https://www.keycloak.org/downloads)

### Configuration

Update the `appsettings.json` file with your Keycloak configuration:

```json
{
  "Keycloak": {
    "AuthorizationUrl": "http://localhost:8080/realms/demo-realm/protocol/openid-connect/auth"
  },
  "Authentication": {
    "MetadataAddress": "http://localhost:8080/realms/demo-realm/.well-known/openid-configuration",
    "ValidIssuer": "http://localhost:8080/realms/demo-realm",
    "Audience": "account"
  }
}
```

### Running the Application

1. Start Keycloak and create a realm and client as per your requirements.
2. Run the application using the following command:

```sh
dotnet run
```

### API Endpoints

- `GET /users/me`: Returns the claims of the authenticated user. Requires authorization.

### Swagger

Swagger is enabled in development mode. You can access it at `/swagger`.

## License

This project is licensed under the MIT License.
