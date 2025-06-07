using finance_api.Data;
using finance_api.Dtos;
using finance_api.Enums;
using finance_api.Profiles;
using finance_api.Services;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"))
    );

builder.Services.AddAutoMapper(typeof(MappingProfile));

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

app.MapGet("/getTransactions", async (ITransactionsService service) =>
{
    var transactions = await service.GetAllTransactions();
    return Results.Ok(transactions);
})
.WithName("GetAllData")
.Produces<List<TransactionDtoResponse>>(StatusCodes.Status200OK)
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

app.Run();
