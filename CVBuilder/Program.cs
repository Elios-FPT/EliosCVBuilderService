using CVBuilder.Core.Interfaces;
using CVBuilder.Infrastructure.DataContext;
using CVBuilder.Infrastructure.Implementations;
using CVBuilder.Infrastructure.Kafka;
using Elios.CVBuilder.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CVBuilder API",
        Version = "v1",
        Description = "API utility operations"
    });
    c.AddServer(new OpenApiServer { Url = "/" });
});

// Add services to the container.
builder.Services.AddScoped<IAppConfiguration, AppConfiguration>();
builder.Services.AddScoped<ICombinedTransaction, CombinedTransaction>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IKafkaProducerRepository<>), typeof(KafkaProducerRepository<>));
builder.Services.AddScoped(typeof(IKafkaConsumerRepository<>), typeof(KafkaConsumerRepository<>));
builder.Services.AddScoped(typeof(IKafkaConsumerFactory<>), typeof(KafkaConsumerFactory<>));
builder.Services.AddScoped(typeof(IKafkaResponseHandler<>), typeof(KafkaResponseHandler<>));
builder.Services.AddScoped<IKafkaTransaction, KafkaTransaction>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// Database Context
builder.Services.AddDbContext<CVBuilderDataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CVBuilder.Core.AssemblyReference).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CVBuilder.Contract.AssemblyReference).Assembly);
});

// Kafka Consumers
var sourceServices = builder.Configuration.GetSection("Kafka:SourceServices").Get<string[]>() ?? [];

Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] Registering {sourceServices.Length} Kafka consumers for sources: [{string.Join(", ", sourceServices)}]");

foreach (var sourceService in sourceServices)
{
    var currentSource = sourceService;

    builder.Services.AddSingleton<IHostedService>(sp =>
    {
        var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();

        return ActivatorUtilities.CreateInstance<KafkaConsumerHostedService<UserCv>>(
            sp,
            scopeFactory,
            currentSource
        );
    });
}


var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var appConfiguration = scope.ServiceProvider.GetRequiredService<IAppConfiguration>();
    KafkaResponseConsumer.Initialize(appConfiguration);
    Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] KafkaResponseConsumer initialized");
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CVBuilder API v1");
    c.DocumentTitle = "CVBuilder API Documentation";
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();
app.MapControllers();

var currentService = builder.Configuration["KafkaCommunication:CurrentService"];
Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] {currentService} Service Started!");
Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] Kafka Consumers registered for: [{string.Join(", ", sourceServices)}]");

app.Run();