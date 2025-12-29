using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// --- 1. Keycloak (Identity Provider) ---
// Uses the "Keycloak.AuthServices.Aspire.Hosting" package you just installed.
var keycloak = builder.AddKeycloakContainer("keycloak", port: 8080)
                      .WithDataVolume();

var realm = keycloak.AddRealm("weather-shop");

// --- 2. .NET API (Resource Server) ---
var apiService = builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
    .WithReference(realm); // Injects Keycloak connection strings into the API

// --- 3. Angular Frontend (Client Application) ---
var angular = builder.AddNpmApp("angular-client", "../angular-client")
    .WithReference(apiService) // Allows Angular to find the API URL
    .WithReference(realm)      // Allows Angular to find the Keycloak URL
    .WithHttpEndpoint(env: "PORT") // Tells Aspire to listen on a random port and inject it
    .WithExternalHttpEndpoints()   // Makes it accessible from your browser
    .PublishAsDockerFile();        // (Optional) For deployment later



builder.Build().Run();