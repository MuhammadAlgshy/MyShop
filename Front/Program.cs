using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Front.Data;
using Front.Areas.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("FrontContextConnection") ?? throw new InvalidOperationException("Connection string 'FrontContextConnection' not found.");

builder.Services.AddDbContext<FrontContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<FrontUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<FrontContext>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication().AddJwtBearer(
    options =>
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("V%$rt $ecery (not)!"));
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = key

        };

    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.Run();
