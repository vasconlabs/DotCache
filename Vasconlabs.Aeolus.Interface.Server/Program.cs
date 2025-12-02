using Vasconlabs.Aeolus.Application.Cache;
using Vasconlabs.Aeolus.Application.Cache.Services;
using Vasconlabs.Aeolus.Interface.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAeolusCacheService();

builder.Services.AddHostedService<AeolusSnapshotService>();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.UseAeolusCacheService();

app.MapGrpcService<AeolusCacheService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();