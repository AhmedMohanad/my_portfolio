using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Portofolio.Data;
using Portofolio.Services.ExperienceServices;
using Portofolio.Services.ImageServices;
using Portofolio.Services.ProfileServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// service injection
builder.Services.AddScoped<IImageServices, ImageServices>();
builder.Services.AddScoped<IProfileServices, ProfileServices>();
builder.Services.AddScoped<IExperienceServices, ExperienceServices>();


var app = builder.Build();


// Database connection



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Program.cs
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        var ex = contextFeature?.Error;

        context.Response.ContentType = "application/json";

        context.Response.StatusCode = ex switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        await context.Response.WriteAsJsonAsync(new
        {
            statusCode = context.Response.StatusCode,
            message = ex?.Message ?? "An unexpected error occurred."
        });
    });
});


app.Run();
