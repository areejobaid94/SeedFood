using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.EntityFrameworkCore
{
    public static class MessagingPortalDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<MessagingPortalDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<MessagingPortalDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}