using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace SsoServer.Data.Seeding
{
    public static class IdentityServerPersistedGrantDbSeeder
    {
        /// <summary>
        /// Seed the Identity Server persisted grant db (code, token, etc.)
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="autoMigrateDatabase"></param>
        public static void EnsureIdentityServerPersistedGrantDbIsSeeded(this IApplicationBuilder builder, bool autoMigrateDatabase)
        {
            using (var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<PersistedGrantDbContext>();

                if (autoMigrateDatabase)
                {
                    dbContext.Database.Migrate();
                }

                // This is mostly operational data, so no seeding required.
                // Seed any data here if necessary
            }
        }
    }
}
