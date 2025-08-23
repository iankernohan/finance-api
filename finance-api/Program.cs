using finance_api.Data;
using finance_api.Dtos;
using finance_api.Enums;
using finance_api.Profiles;
using finance_api.Services;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string AllowedOrigins = "AllowLocalHost";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowedOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

Env.Load();
string ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(ConnectionString)
    );

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<ITransactionsService, TransactionsService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(AllowedOrigins);

app.UseHttpsRedirection();

app.MapGet("/getTransactions", async (ITransactionsService service) =>
{
    var transactions = await service.GetAllTransactions();
    return Results.Ok(transactions);
})
.WithName("GetAllData")
.Produces<List<TransactionDtoResponse>>(StatusCodes.Status200OK)
.WithOpenApi();

app.MapGet("/getTransaction", async (ITransactionsService service, int id) =>
{
    var transaction = await service.GetTransaction(id);
    return Results.Ok(transaction);
})
.WithName("GetTransactionById")
.Produces<TransactionDtoResponse>(StatusCodes.Status200OK)
.WithOpenApi();

app.MapGet("/getAllExpenses", async (ITransactionsService service) =>
{
    var transactions = await service.GetAllExpenses();
    return Results.Ok(transactions);
})
.WithName("GetAllExspenses")
.Produces<List<TransactionDtoResponse>>()
.WithOpenApi();

app.MapGet("/getAllIncome", async (ITransactionsService service) =>
{
    var transactions = await service.GetAllIncome();
    return Results.Ok(transactions);
})
.WithName("GetAllIncome")
.Produces<List<TransactionDtoResponse>>()
.WithOpenApi();

app.MapGet("/getAllCategories", async (ICategoryService service) =>
{
    var categories = await service.GetCategories();
    return Results.Ok(categories);
})
.WithName("GetAllCategories")
.Produces<List<CategoryDtoResponse>>()
.WithOpenApi();

app.MapGet("/getCategoryByName/{Name}", async (ICategoryService service, string Name) =>
{
    var category = await service.GetCategory(Name);
    return Results.Ok(category);
})
.WithName("GetCategoryByName")
.Produces<CategoryDtoResponse>()
.WithOpenApi();

app.MapGet("/getCategoryById/{Id}", async (ICategoryService service, int Id) =>
{
    var category = await service.GetCategory(Id);
    return Results.Ok(category);
})
.WithName("GetCategoryById")
.Produces<CategoryDtoResponse>()
.WithOpenApi();

app.MapPost("/AddTransaction", async (ITransactionsService service, TransactionDtoRequest transaction) =>
{
    var addedTransaction = await service.AddTransaction(transaction);
    return Results.Ok(addedTransaction);
})
.WithName("AddTransaction")
.Produces<TransactionDtoResponse>()
.WithOpenApi();

app.MapPost("/AddCategory", async (ICategoryService service, CategoryDtoRequest category) =>
{
    await service.AddCategory(category);
    return Results.Ok();
})
.WithName("AddCategory")
.WithOpenApi();

app.MapPost("AddSubCategory", async (ICategoryService service, SubCategoryDtoRequest subCategory) =>
{
    await service.AddSubCategory(subCategory);
    return Results.Ok();
})
.WithName("AddSubCategory")
.WithOpenApi();

app.MapPut("/UpdateTransaction/{id}", async (ITransactionsService service, int id, TransactionDtoRequest updatedTransaction) =>
{
    var transaction = await service.UpdateTransaction(id, updatedTransaction);
    return Results.Ok(transaction);
})
.WithName("UpdateTransaction")
.Produces<TransactionDtoResponse>()
.WithOpenApi();

app.MapDelete("/DeleteTransaction/{id}", async (ITransactionsService service, int id) =>
{
    var transaction = await service.DeleteTransaction(id);
    if (transaction is not null)
    {
        return Results.Ok(transaction);
    }
    return Results.NotFound($"No transaction found with the id {id}");
})
.WithName("DeleteTransaction")
.WithOpenApi();

app.MapPost("AddBudget", async (IBudgetService service, BudgetDtoRequest budget) =>
{
    await service.AddBudget(budget);
    return Results.Ok();
})
.WithName("AddBudget")
.WithOpenApi();

app.MapGet("GetAllBudgets", async (IBudgetService service) =>
{
    var budgets = await service.GetAllBudgets();
    return Results.Ok(budgets);
})
.WithName("GetAllBudgets")
.Produces<BudgetDtoResponse>()
.WithOpenApi();

app.MapPost("resetDatabase", async (IDatabaseService service, IWebHostEnvironment env) =>
{
    if (!env.IsDevelopment())
    {
        return Results.BadRequest("Database reset is only allowed in development environment.");
    }
    await service.ResetDatabaseAsync();
    return Results.Ok("Database reset successfully.");
})
.WithName("ResetDatabase")
.WithOpenApi();
app.Run();
