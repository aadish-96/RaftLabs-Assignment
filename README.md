# RaftLabs .NET API Integration Component

## 🚀 Overview

This is a .NET 8.0 service component that integrates with the public [reqres.in](https://reqres.in) API to fetch and process user data. The solution follows Clean Architecture principles and demonstrates resilient HTTP handling with Polly, in-memory caching, and test-driven development.

---

## ✅ Key Features

- Asynchronous service methods
- Configurable Polly policies (retry, timeout, circuit breaker)
- In-memory caching with expiration
- AutoMapper DTO-to-model conversion
- Domain-driven architecture with service abstractions
- CLI and IDE-based launch support
- Strong unit test coverage with mock support

---

## 🧱 Architecture

The solution adopts a Clean Architecture pattern:

```
├── Domain         → Core business models and interfaces
├── Application    → DTOs and abstraction contracts
├── Infrastructure → Service implementations, Polly policies, caching, mapping
├── ConsoleApp     → Entry-point to demonstrate functionality
├── Tests          → Unit tests covering service behavior
```

---

## 🔧 Technologies Used

- **.NET 8.0**
- **HttpClientFactory** + **Polly** for HTTP resiliency
- **AutoMapper** for DTO to domain model conversion
- **IMemoryCache** for caching API responses
- **IOptions Pattern** for configuration binding
- **xUnit** and **Moq** for unit testing

---

## ⚙️ Configuration

Found in `RaftLabs.ConsoleApp/appsettings.json`:

```json
{
  "ExternalApiSettings": {
    "BaseUrl": "https://reqres.in/api/",
    "ApiKey": "x-api-key",
    "ApiValue": "reqres-free-v1"
  },
  "HttpResilienceSettings": {
    "RetrySettings": {
      "RetryAttemptCount": 3,
      "ExponentialBaseDigit": 2
    },
    "CircuitBreakerSettings": {
      "NumberOfEventsAllowedBeforeBreaking": 2,
      "OpenCircuitDurationAllowedInSecs": 30
    },
    "ResponseTimeoutInMins": 10
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

## 🛠️ Build & Run

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download)

### Restore & Build

```bash
dotnet restore
dotnet build
```

### 🚀 Run Options

#### 🧪 Run from Visual Studio 2022
1. Open the solution in Visual Studio 2022.
2. Set `RaftLabs.ConsoleApp` as the startup project.
3. Press **F5** (Debug) or **Ctrl + F5** (Run).
   - The app uses `launchSettings.json` with the default argument `0` (fetch all users).

#### 💻 Run from Command Line
```bash
# To fetch all users
dotnet run --project src/RaftLabs.ConsoleApp -- 0

# To fetch a specific user by ID
dotnet run --project src/RaftLabs.ConsoleApp -- 3
```

#### 📦 Publish and Run
```bash
dotnet publish src/RaftLabs.ConsoleApp -c Release -o ./publish
./publish/RaftLabs.ConsoleApp.exe 5   # Fetch user with ID 5
```

---

## 🧪 Running Tests

```bash
dotnet test
```

Covers:
- Pagination and full user list retrieval
- Individual user fetch with caching
- Mapping correctness and error handling

---

## 🖥️ Sample Output

Example console output when fetching user ID 2:

```json
info: RaftLabs.ConsoleApp.Demo.LaunchDemo[0]
      User with ID 2:
      {
        "id": 2,
        "email": "janet.weaver@reqres.in",
        "first_name": "Janet",
        "last_name": "Weaver",
        "avatar": "https://reqres.in/img/faces/2-image.jpg"
      }
```

---

## 🛑 Error Handling Behavior

- Invalid user ID (e.g. `dotnet run -- 99`) logs:
```
error: RaftLabs.ConsoleApp.Demo.LaunchDemo[0]
       UserNotFoundException: User with ID 99 not found.
```
- Network failure or API error logs:
```
error: ExternalServiceException: Failed to connect to external service.
```

---

## 🎯 Design Decisions

| Area                 | Decision & Justification                                                                 |
|----------------------|-------------------------------------------------------------------------------------------|
| **Architecture**     | Adopted Clean Architecture to ensure modularity, scalability, and clear separation of concerns. |
| **HttpClient usage** | Used `IHttpClientFactory` to avoid socket exhaustion and to integrate Polly handlers effectively. |
| **Resilience**       | Applied Polly policies — retry (with exponential backoff), circuit breaker, and timeout — to handle transient API failures gracefully. |
| **Caching**          | Used `IMemoryCache` with a 10-minute sliding expiration to minimize redundant API calls while keeping data fresh. |
| **Configuration**    | Externalized API and resilience settings using the `IOptions<T>` pattern to allow future changes without code edits. |
| **Mapping**          | Used AutoMapper to abstract repetitive DTO → domain conversions and configured it globally for maintainability. |
| **Snake_case Support** | Implemented a custom `SnakeCaseNamingPolicy` to ensure compatibility with the API's JSON format. |
| **Testability**      | All services and external dependencies (e.g., API client) were injected via interfaces, enabling mocking in unit tests. |
| **CLI Integration**  | Used `launchSettings.json` and `commandLineArgs` to simplify run/debug configuration for developers. |

---

## 📽️ Video Demo Link

🎥 [Watch the walkthrough demo here](https://1drv.ms/v/c/db96032a15e8bebe/EeE3VF09j-ZOsUA8AezpQvkBpgvHEwEH-GpMEQgQB-HZow?e=hGV0uj)

---

## 📄 Credits

This solution was developed as part of the **[RaftLabs .NET Developer Contractor Assignment (2025)](https://drive.google.com/file/d/1sBW0fx5QJhu7ZGnX7DtIZulpWBz3BP7S/view?usp=sharing)** and utilizes the [reqres.in](https://reqres.in) API for demonstration purposes only.

---

## ⚖️ License

For evaluation and demonstration purposes only.
