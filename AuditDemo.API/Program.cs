using AuditDemo.API.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using System.Threading.Channels;
using UsersTasks.API.Data;
using UsersTasks.API.Endpoints.Configuration;
using UsersTasks.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<AppDBContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
	);

builder.Services.AddSingleton(Channel.CreateUnbounded<AuditingChannelRequest>(new UnboundedChannelOptions
{
	AllowSynchronousContinuations = false,
}));
builder.Services.AddHostedService<AuditingPersistProcessor>();

builder.Services.AddOpenApi();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TaskService>();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();

app.RegisterEndpoints();

app.Run();

