var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var messaging = builder.AddKafka("messaging");

var apiService = builder.AddProject<Projects.AspireApp1_ApiService>("apiservice")
    .WithReference(messaging);

builder.AddProject<Projects.AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
        .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
