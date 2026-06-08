using Microsoft.Data.SqlClient;
using System.Data;
using PayrollApp.Repos;
using PayrollApp.Services;

Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddScoped<IEmployeeRepo, EmployeeRepo>();
builder.Services.AddScoped<IPayrollRepo, PayrollRepo>();

builder.Services.AddScoped<IPayrollService, PayrollService>();

builder.Services.AddScoped<IDbConnection>(sp =>
{
    var conn = new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnectionString"));
    conn.Open();
    return conn;

});


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
using var connection = new SqlConnection(connectionString);
connection.Open();

foreach (var file in new[] { "schema.sql", "dump.sql", "sp_RunPayroll.sql", "sp_GetAllEmployees.sql", "sp_GetPayrollByMonthYear.sql", "sp_GetPayrollSlip.sql" })
{
    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SQL", file);
    var sqlText = File.ReadAllText(path);
    using var cmd = new SqlCommand(sqlText, connection);
    cmd.ExecuteNonQuery();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseWhen(ctx => !ctx.Request.Path.StartsWithSegments("/index.html"), appBuilder =>
{
    appBuilder.UseStaticFiles();
});
app.UseStaticFiles();

//app.UseStaticFiles();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();




app.Run();


