using Infoseed.MessagingPortal.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.Migrations.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly MessagingPortalDbContext _context;

        public InitialHostDbBuilder(MessagingPortalDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
            new DefaultTemplateMessagePurposesCreator(_context).Create();

            new DefaultServiceTypesCreator(_context).Create();
            new DefaultServiceStatusesCreator(_context).Create();
            new DefaultServiceFrequenciesCreator(_context).Create();
            new DefaultInfoSeedServicesCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
