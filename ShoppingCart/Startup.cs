using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShoppingCart.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc.Razor;
using ShoppingCart.Resources;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace ShoppingCart
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            }).AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews().AddNewtonsoftJson(x =>
            {
                x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddRazorPages();

            services.AddAuthentication().AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false,
                    ValidateAudience = false,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddLocalization();
            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        return factory.Create(typeof(SharedResource));
                    };
                });

            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider service,IConfiguration config)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            var supportedCultures = new[] { "en-US", "th-TH" };
            var localizationOption = new RequestLocalizationOptions()
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOption);

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });


            Setup(service, config);
        }

        public void Setup(IServiceProvider service, IConfiguration config)
        {

            var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();

            //create admin user if not exist
            var adminUser = userManager.FindByNameAsync("admin").GetAwaiter().GetResult();
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = config["Admin:Username"],
                    Email = config["Admin:Email"]
                };
                var result = userManager.CreateAsync(adminUser, config["Admin:Password"]).GetAwaiter().GetResult();

                if (!result.Succeeded) throw new Exception();
            }

            //create admin role if not exist
            var adminRole = roleManager.FindByNameAsync("Admin").GetAwaiter().GetResult();
            if (adminRole == null)
            {
                adminRole = new IdentityRole("Admin");
                var result = roleManager.CreateAsync(adminRole).GetAwaiter().GetResult();
                if (!result.Succeeded) throw new Exception();
            }

            //assign admin role if not exist
            var isAdmin = userManager.IsInRoleAsync(adminUser, adminRole.Name).GetAwaiter().GetResult();
            if (!isAdmin)
            {
                var result = userManager.AddToRoleAsync(adminUser, adminRole.Name).GetAwaiter().GetResult();
                if (!result.Succeeded) throw new Exception();
            }

            //create customer role if not exist
            var customerRole = roleManager.FindByNameAsync("Customer").GetAwaiter().GetResult();
            if (customerRole == null)
            {
                customerRole = new IdentityRole("Customer");
                var result = roleManager.CreateAsync(customerRole).GetAwaiter().GetResult();
                if (!result.Succeeded) throw new Exception();
            }
        }

    }
}
