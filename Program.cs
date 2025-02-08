using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Data;
using OnlineMarket.Models;
using OnlineMarket.Services;


class Program
{
    static async Task Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // // Add DbContext with MySQL
        // var conectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        // builder.Services.AddDbContext<ApplicationDbContext>(options =>
        //     options.UseMySql(
        //        conectionString,
        //         ServerVersion.AutoDetect(conectionString))
        //     );


        // Add DbContext with SQLite
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("SqlLiteConnection")));


        builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Auth/Login";
            options.AccessDeniedPath = "/Auth/AccessDenied";
        });

        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<OrderService>();
        builder.Services.AddScoped<CategoryService>();
        builder.Services.AddScoped<CartService>();

        var app = builder.Build();


        await SeedRoles.Initialize(app.Services);

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Product}/{action=Index}/{Id?}");

        app.Run();

    }
}