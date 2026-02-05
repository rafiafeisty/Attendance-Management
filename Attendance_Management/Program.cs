using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Attendance_Management;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("jwt");
var key=Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication("JwtAuth")
    .AddJwtBearer("JwtAuth",options => {
         options.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidateLifetime = true,
             ValidateIssuerSigningKey = true,
             ValidIssuer = jwtSettings["Issuer"],
             ValidAudience = jwtSettings["Audience"],
             IssuerSigningKey = new SymmetricSecurityKey(key)
         };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.Redirect("/Login/LoginDashboard");
                return Task.CompletedTask;
            }
        };
     });
                                             


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseMiddleware<jwt>();


app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=LoginDashboard}/{id?}");

app.Run();
