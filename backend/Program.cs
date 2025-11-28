using driver_api.Repository.IRepo;
using driver_api.Repository.Repo;
using Microsoft.AspNetCore.Rewrite;
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
            .WithOrigins("http://localhost:3000", "http://localhost:5272", "http://localhost:7009", "null")
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

var rewriteOption = new RewriteOptions()
    .AddRedirect("(.*)/$", "$1");

app.UseRewriter(rewriteOption);

// app.Use(async (context, next) =>
// {
//     var path = context.Request.Path.Value;

//     // Only process if:
//     // 1. Path is not root "/"
//     // 2. Path doesn't already have an extension (like .js, .css, .png)
//     if (!string.IsNullOrEmpty(path) && path != "/" && !path.Contains('.'))
//     {
//         // Get the physical path of the wwwroot folder
//         var webRootPath = app.Environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

//         // Construct the potential file path (e.g., /app/wwwroot/about.html)
//         var htmlFilePath = Path.Combine(webRootPath, path.TrimStart('/') + ".html");

//         // Check if the .html file actually exists
//         if (File.Exists(htmlFilePath))
//         {
//             // If it exists, rewrite the internal request path to include .html
//             // The user still sees the original URL in the browser.
//             context.Request.Path = path + ".html";
//         }
//     }

//     await next();
// });

app.Use(async (context, next) =>
{
    string path = context.Request.Path.Value ?? "";

    // Only rewrite paths without extension
    if (path != "/" && !Path.HasExtension(path))
    {
        string wwwroot = app.Environment.WebRootPath!;
        string target = Path.Combine(wwwroot, path.TrimStart('/') + ".html");

        if (File.Exists(target))
        {
            context.Request.Path = path + ".html";
        }
    }

    await next();
});


// Serves index.html for the root "/"
app.UseDefaultFiles();

// Serves the actual static files
app.UseStaticFiles();

app.UseRouting();

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
