using finance_api.Data;
using finance_api.Profiles;
using finance_api.Services;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Going.Plaid;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string AllowedOrigins = "AllowLocalHost";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowedOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173", "https://iankernohan.github.io", "https://www.thunderclient.com")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"))
        );
}
else
{
    Env.Load();
    string ConnectionString = System.Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "";
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(ConnectionString)
        );
}
builder.Services.AddAuthorization();

var supabaseJwt = System.Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET") ?? builder.Configuration["Authentication:JwtSecret"];
var issuer = System.Environment.GetEnvironmentVariable("SUPABASE_VALID_ISSUER") ?? builder.Configuration["Authentication:ValidIssuer"];
var audience = "authenticated";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(supabaseJwt ?? "")),
          ValidateIssuer = true,
          ValidIssuer = issuer,
          ValidateAudience = true,
          ValidAudience = audience,
          ValidateLifetime = true,
          ClockSkew = TimeSpan.FromMinutes(2)
      };
  });

builder.Services.Configure<PlaidOptions>(
    builder.Configuration.GetSection("Plaid"));
builder.Services.AddSingleton<PlaidClient>();
builder.Services.AddSingleton<EncryptionService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<ICategoryRulesService, CategoryRulesService>();
builder.Services.AddScoped<ICategoryRulesApplier, CategoryRulesApplier>();
builder.Services.AddScoped<ITransactionsService, TransactionsService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPlaidService, PlaidService>();

var app = builder.Build();

// app.UseHttpsRedirection();
app.UseCors(AllowedOrigins);
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();