using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Assignment3.Models;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Assignment3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<Assignment3Context>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("Assignment3Context") ??
                    throw new InvalidOperationException(
                        "Connection string 'Assignment3Context' not found.")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Add SignalR
            builder.Services.AddSignalR();

            //Add ReferenceHandler.IgnoreCycles
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            //Session
            builder.Services.AddDistributedMemoryCache();

            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //session
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                // pattern: "{controller=Home}/{action=Index}/{id?}");
                pattern: "{controller=Posts}/{action=Index}/{id?}");

            //signalR server
            app.MapHub<SignalrServer>("signalrServer");

            app.Run();
        }
    }
}