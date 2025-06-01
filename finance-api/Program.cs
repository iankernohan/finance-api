using finance_api.Enums;
using finance_api.Models;
using finance_api.Services;

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
                          policy.WithOrigins("http://localhost:5173");
                      });
});

builder.Services.AddScoped<ITransactionsService, TransactionsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(AllowedOrigins);

app.UseHttpsRedirection();

Category transportation = new()
{
    Id = 0,
    Name = "Transportation",
    transactionType = TransactionType.Expense,
};

app.MapGet("/getAllData", async (ITransactionsService service) =>
{
    var transactions = await service.GetAllTransactions();
    return Results.Ok(transactions);
})
.WithName("GetAllData")
.Produces<List<Transaction>>(StatusCodes.Status200OK)
.WithOpenApi();

app.MapGet("/getAllExpenses", async (ITransactionsService service) =>
{
    var transactions = await service.GetAllExpenses();
    return Results.Ok(transactions);
})
.WithName("GetAllExspenses")
.Produces<List<Transaction>>()
.WithOpenApi();

app.MapGet("/getAllIncome", async (ITransactionsService service) =>
{
    var transactions = await service.GetAllIncome();
    return Results.Ok(transactions);
})
.WithName("GetAllIncome")
.Produces<List<Transaction>>()
.WithOpenApi();

app.Run();
