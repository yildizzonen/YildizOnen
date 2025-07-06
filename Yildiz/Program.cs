using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Identity ve DbContext ekle
builder.Services.AddDbContext<Yildiz.Models.UygulamaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VarsayilanBaglanti")));
builder.Services.AddIdentity<Yildiz.Models.Kullanici, Yildiz.Models.Rol>()
    .AddEntityFrameworkStores<Yildiz.Models.UygulamaDbContext>();

var app = builder.Build();

app.UseMiddleware<Yildiz.Middleware.CustomExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Roller 
using (var scope = app.Services.CreateScope())
{
    await Yildiz.Models.SeedData.InitializeAsync(scope.ServiceProvider);
}

app.Run();
