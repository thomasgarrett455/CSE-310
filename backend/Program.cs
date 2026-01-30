using JournalApi.Data;
using JournalApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

builder.Services.AddScoped<IJournalService, JournalService>();

builder.Services
  .AddIdentity<ApplicationUser, IdentityRole>()
  .AddEntityFrameworkStores<JournalDbContext>()
  .AddDefaultTokenProviders();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  var key = builder.Configuration["Jwt:Key"]!;

  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = 
    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
  };
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowFrontend");
app.MapControllers();
app.Run();