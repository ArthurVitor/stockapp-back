using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using StockApp.API.Context;
using StockApp.API.Repositories;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.ServiceFactory;
using StockApp.API.ServiceFactory.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.API.Services;
using StockApp.API.Middlewares;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

Console.WriteLine(Environment.GetEnvironmentVariable("varaivel_top") + "####################");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information);
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Product
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

//Subcategory
builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();

//EntryNote
builder.Services.AddScoped<IEntryNoteRepository, EntryNoteRepository>();
builder.Services.AddScoped<IEntryNoteService, EntryNoteService>();

//ProductSubcategory
builder.Services.AddScoped<IProductSubCategoryRepository, ProductSubcategoryRepository>();
builder.Services.AddScoped<IProductSubCategoryService, ProductSubCategoryService>();

//Batch
builder.Services.AddScoped<IBatchRepository, BatchRepository>();
builder.Services.AddScoped<IBatchService, BatchService>();

//Inventory
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

//Parameters
builder.Services.AddScoped<IParametersRepository, ParametersRepository>();
builder.Services.AddScoped<IParameterService, ParameterService>();

//Import file
builder.Services.AddScoped<IImportFileService, ImportFileService>();

//ExitNote
builder.Services.AddScoped<IExitNoteService, ExitNoteService>();
builder.Services.AddScoped<IExitNoteRepository, ExitNoteRepository>();

//ExitNoteBatch
builder.Services.AddScoped<IExitNoteBatchRepository, ExitNoteBatchRepository>();
builder.Services.AddScoped<IExitNoteBatchService, ExitNoteBatchService>();

//Transactions
builder.Services.AddScoped<ITransactionsRepository, TransactionsRepository>();
builder.Services.AddScoped<ITransactionsService, TransactionsService>();
builder.Services.AddSingleton<ITransactionsServiceFactory, TransactionsServiceFactory>();

//Admin Route
builder.Services.AddScoped<IAdminRouteRepository, AdminRouteRepository>();
builder.Services.AddScoped<IAdminRouteService, AdminRouteService>();

//ReverseTransaction
builder.Services.AddScoped<IReverseEntryTransacion, ReverseEntryTransaction>();
builder.Services.AddScoped<IReverseExitTransaction, ReverseExitTransaction>();

//Override
builder.Services.AddScoped<IOverrideRepository, OverrideRepository>();
builder.Services.AddScoped<IOverrideService, OverrideService>();

//Middleware
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.Run();
