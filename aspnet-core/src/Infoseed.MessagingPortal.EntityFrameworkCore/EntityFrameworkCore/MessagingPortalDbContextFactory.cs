using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Infoseed.MessagingPortal.Configuration;
using Infoseed.MessagingPortal.Web;

namespace Infoseed.MessagingPortal.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class MessagingPortalDbContextFactory : IDesignTimeDbContextFactory<MessagingPortalDbContext>
    {
        public MessagingPortalDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<MessagingPortalDbContext>();
            var configuration = AppConfigurations.Get(
                WebContentDirectoryFinder.CalculateContentRootFolder(),
                addUserSecrets: true
            );

            MessagingPortalDbContextConfigurer.Configure(builder, configuration.GetConnectionString(MessagingPortalConsts.ConnectionStringName));

            return new MessagingPortalDbContext(builder.Options);
        }
    }
}