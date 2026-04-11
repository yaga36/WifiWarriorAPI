using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;
using WifiWarriorAPI;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo{ Title = "WifiWarriorAPI", Version = "v1"});
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Description = "JWT Authorization using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer")] = []
    });
});
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddEntityFrameworkNpgsql().AddDbContext<ApiDbContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgresDbConnection")));

builder.Services.AddAuthentication();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CanSubmit", policy => 
        policy.RequireRole(nameof(Role.User), nameof(Role.Moderator), nameof(Role.Administrator)))
    .AddPolicy("CanEdit", policy => 
        policy.RequireRole(nameof(Role.Moderator), nameof(Role.Administrator)))
    .AddPolicy("CanDelete", policy => 
        policy.RequireRole(nameof(Role.Administrator)));

builder.Services.ConfigureIdentity();
builder.Services.ConfigureJwt(builder.Configuration);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
if (allowedOrigins is null || !allowedOrigins.Any())
    throw new InvalidOperationException("No allowed origins set");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IConnectionInformationService, ConnectionInformationService>();
builder.Services.AddScoped<IConnectionTypeService, ConnectionTypeService>();
builder.Services.AddScoped<IWifiDetailsService, WifiDetailsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program {}
