using backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy allowing credentials
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:3000")  // your frontend
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // <-- THIS IS CRUCIAL
    });
});

// Add DB context with MySQL connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 32))));

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add session support and in-memory distributed cache
builder.Services.AddDistributedMemoryCache();  // Add in-memory distributed cache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Session timeout setting
    options.Cookie.HttpOnly = true;  // Set cookie properties
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS policy
app.UseCors("AllowLocalhost");

app.UseStaticFiles(); // allow serving /uploads files
app.UseSession();  // Enables session middleware
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers(); // Enable controller routing

app.Run();
