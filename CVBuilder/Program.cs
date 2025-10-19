using CVBuilder.Core.Interfaces;
using CVBuilder.Infrastructure.DataContext;
using CVBuilder.Infrastructure.Implementations;
using CVBuilder.Infrastructure.Kafka;
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
builder.Services.AddScoped<ICombinedTransaction, CombinedTransaction>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();
builder.Services.AddScoped(typeof(IKafkaRepository<>), typeof(KafkaRepository<>));
builder.Services.AddScoped<IKafkaTransaction, KafkaTransaction>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

builder.Services.AddHostedService<KafkaConsumerHostedService<CVBuilder.Domain.Entities.Notification>>();

builder.Services.AddDbContext<CVBuilderDataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CVBuilder.Core.AssemblyReference).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CVBuilder.Contract.AssemblyReference).Assembly);
});


//builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CVBuilder API v1");
        c.DocumentTitle = "CVBuilder API Documentation";
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
