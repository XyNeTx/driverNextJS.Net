using driver_api.Repository.IRepo;
using driver_api.Repository.Repo;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocal",
        policy => policy
            .WithOrigins("http://localhost:3000","http://localhost:5272","http://localhost:7009","null")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.DictionaryKeyPolicy = null;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<WorkflowContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("WorkflowConnection"))
);

builder.Services.AddScoped<IReportOutSourceRepo, ReportOutSourceRepo>();

var app = builder.Build();
app.UseCors("AllowLocal");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
