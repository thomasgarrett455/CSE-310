using JournalApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<JournalDbContext>(options => options.UseSqlite("Data Source=journal.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
      policy =>
      {
        policy
          .AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
      });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.MapControllers();
app.Run();