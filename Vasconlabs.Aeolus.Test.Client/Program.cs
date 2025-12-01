using Scalar.AspNetCore;
using Vasconlabs.Aeolus.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAeolusClient(opts =>
{
    opts.BaseUrl = "https://localhost:7246";
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
