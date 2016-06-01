# Serilog.Sinks.Stackify

A Serilog sink that writes events to stackify. [Stackify](http://www.stackify.com) is a cloud hosted solution to capture log messages. Register for an account at their website and use the provided GUID in the configuration for Serilog.

**Package** - [Serilog.Sinks.Stackify](http://nuget.org/packages/serilog.sinks.Stackify)
| **Platforms** - .NET 4.5

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Stackify()
    .CreateLogger();
```

The sink captures all levels, but respect the minimum level configured on LoggerConfiguration. Serilog properties are converted to stackify's jsondata property.

Refer to Stackify's documentation [Here](https://github.com/stackify/stackify-api-dotnet/) for configuration.
