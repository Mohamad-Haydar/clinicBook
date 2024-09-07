using System.Text;
using api.Data;
using api.BusinessLogic.DataAccess;
using api.Helper;
using api.Internal.DataAccess;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Middlewares;
using Serilog;
//using Microsoft.AspNetCore.Hosting;
using Serilog.Events;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
           .CreateLogger();

builder.Host.UseSerilog();

//Serilog.ILogger logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console().CreateLogger();
//builder.Services.AddSingleton(logger);

// TODO: Create Custom DateOnly and TimeOnly Model Binder

// Add services to the container.
var appConnectionString = builder.Configuration.GetConnectionString("AppDbConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityDbConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(appConnectionString));
builder.Services.AddDbContext<IdentityAppDbContext>(options =>
        options.UseNpgsql(identityConnectionString));

builder.Services.AddIdentity<UserModel, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ابتثجحخدذرزسشصضطظعغفقكلمنهويىأ";
}).AddEntityFrameworkStores<IdentityAppDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
        options.DefaultAuthenticateScheme = "JwtBearer";
        options.DefaultChallengeScheme = "JwtBearer";
}).AddJwtBearer("JwtBearer", jwtBearerOptions =>
{
    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("loreamipsumissomesecreatekeytobeused")),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(0)
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowLocalhost",
        policy =>
        {
            policy
            .WithOrigins("https://localhost:3000", "http://localhost:3000", "https://192.168.1.17:3000", "http://192.168.1.17:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddScoped<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddScoped<SecretaryData>();
builder.Services.AddScoped<ITokenData, TokenData>();
builder.Services.AddScoped<IDoctorManagementData, DoctorManagementData>();
builder.Services.AddScoped<IDoctorAvailabilityData, DoctorAvailabilityData>();
builder.Services.AddScoped<IReservationData, ReservationData>();
builder.Services.AddScoped<IAuthenticationData, AuthenticationData>();
builder.Services.AddScoped<ICategoryData, CategoryData>();
builder.Services.AddScoped<IBackupService, BackupService>();
builder.Services.AddScoped<IServiceData, ServiceData>();

builder.Services.AddHostedService<DailyWorker>();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "jwtToken_Auth_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Here Enter the JWT Token like the following: beater[space] <your token>"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme
            {
                Reference=new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer",
                }
            },
            new string[]
            {

            }
        }
    });
    c.MapType<TimeSpan>(() => new OpenApiSchema
    {
        Type = "string",
        Example = new OpenApiString("08:00:00")
    });
    c.MapType<DateOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Example = new OpenApiString("2024-01-01")
    });
});



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
        app.UseSwagger();
        app.UseSwaggerUI();
}
app.UseStaticFiles();
//Add support to logging request with SERILOG
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost");

app.UseTokenMiddleware();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
