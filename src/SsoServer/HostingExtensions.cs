using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SsoServer.Data;
using SsoServer.Data.Seeding;
using SsoServer.Models;
using System.Reflection;

namespace SsoServer
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            // The UI pages are in the Pages folder
            builder.Services.AddRazorPages();

            // Setup Controllers support for API endpoints
            builder.Services.AddControllers();

            // The call to MigrationsAssembly(…) later tells Entity Framework that the host project will contain the migrations. This is necessary since the host project is in a different assembly than the one that contains the DbContext classes.
            var migrationsAssembly = typeof(Program).Assembly.GetName().Name;
            string dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // ASP.NET Core Identity 
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(dbConnectionString));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Identity Server
            builder.Services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                    options.EmitStaticAudienceClaim = true;
                })
                // Store configuration data: resources and clients, etc.
                .AddConfigurationStore(options => 
                {
                    options.ConfigureDbContext = b => b.UseSqlite(dbConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // Store operational data that IdentityServer produces: tokens, codes, and consents, etc.
                .AddOperationalStore(options => 
                {
                    options.ConfigureDbContext = b => b.UseSqlite(dbConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // Adds the integration layer to allow IdentityServer to access the user data for the ASP.NET Core Identity user database (configured above). This is needed when IdentityServer must add claims for the users into tokens.
                .AddAspNetIdentity<ApplicationUser>();

            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to https://localhost:5001/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });

            // Use mediator design pattern to reduce coupling between classes while allowing communication between them.
            builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Automatically apply db migrations and seed development databases for development purpose.
                // In production, this should either be done manually, or done through a UI application.
                app.EnsureAspNetCoreIdentityDatabaseIsSeeded(true);
                app.EnsureIdentityServerConfigurationDbIsSeeded(true);
                app.EnsureIdentityServerPersistedGrantDbIsSeeded(true);
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapRazorPages()
                .RequireAuthorization();

            // Map API endpoints following MVC Controller convention
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            return app;
        }

    }
}