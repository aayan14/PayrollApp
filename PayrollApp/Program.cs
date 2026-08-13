using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PayrollApp.Middleware;
using PayrollApp.Repos;
using PayrollApp.Service;
using PayrollApp.Services;
using PayrollApp.Validators;
using Serilog;
using System.Data;
using System.Text;

Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

// Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("C:/PaymentApp/ActivityLogs.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting Application...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddAuthorization();

    builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

    builder.Services.AddScoped<IEmployeeRepo, EmployeeRepo>();
    builder.Services.AddScoped<IPayrollRepo, PayrollRepo>();

    builder.Services.AddScoped<IPayrollService, PayrollService>();

    builder.Services.AddScoped<IAuthService, AuthService>();


    builder.Services.AddScoped<IUserRepo, UserRepo>();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
            };
        });

    builder.Services.AddScoped<IDbConnection>(sp =>
    {
        var conn = new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
        conn.Open();
        return conn;

    });


    



    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter: Bearer {your token}",
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
        });

        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
    });

    // validation
    builder.Services.AddValidatorsFromAssemblyContaining<RunRequestValidator>();
    builder.Services.AddFluentValidationAutoValidation();

    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();


    var app = builder.Build();
    app.UseMiddleware<ExecptionaHandlingMiddleware>();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    using var connection = new SqlConnection(connectionString);
    connection.Open();

    foreach (var file in new[] { "schema.sql", "dump.sql", "sp_RunPayroll.sql", "sp_GetAllEmployees.sql", "sp_GetPayrollByMonthYear.sql", "sp_GetPayrollSlip.sql", "sp_SavePayrollRun.sql", "sp_GetAttendanceForPayroll.sql" })
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SQL", file);
        var sqlText = File.ReadAllText(path);
        using var cmd = new SqlCommand(sqlText, connection);
        cmd.ExecuteNonQuery();
    }

    

    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();


    app.UseStaticFiles();

    //app.UseStaticFiles();


    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // Seed SuperAdmin user if not already present
    var checkUserSql = "SELECT COUNT(*) FROM Users WHERE Username = 'superadmin'";
    using (var checkCmd = new SqlCommand(checkUserSql, connection))
    {
        var exists = (int)checkCmd.ExecuteScalar() > 0;
        if (!exists)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin@123");
            var insertUserSql = @"
            INSERT INTO Users (Username, PasswordHash, FullName, Role, IsActive)
            VALUES ('superadmin', @PasswordHash, 'Head HR Manager', 'SuperAdmin', 1)";

            using var insertCmd = new SqlCommand(insertUserSql, connection);
            insertCmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
            insertCmd.ExecuteNonQuery();
        }
    }



    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


