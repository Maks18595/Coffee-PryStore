

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Security.Claims;
using System.Text;
namespace Coffee_PryStore.Models.Configurations { 
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/Account/Login";
                        options.AccessDeniedPath = "/Account/AccessDenied";
                    });

            services.AddAuthorization(options =>
            {

                options.AddPolicy("AdminEmailOnly", policy =>
                    policy.RequireAssertion(context =>
                        context.User.Identity.IsAuthenticated &&
                        context.User.FindFirst(ClaimTypes.Email)?.Value.Equals("pryimak@gmail.com", StringComparison.OrdinalIgnoreCase) == true
                    )
                );
            });

            services.AddControllersWithViews();
        }



        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseAuthentication();  
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
    

public class SharedResource
        {
        }
    }
