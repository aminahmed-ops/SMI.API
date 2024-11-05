using Microsoft.EntityFrameworkCore;
using SMI.Common.Database;

namespace SMI.DataAccess.Context
{
    public class ApplicationDatabaseFactory : IDatabaseFactory<ApplicationDbContext>
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        public ApplicationDatabaseFactory(DbContextOptions<ApplicationDbContext> options)
        {
            _options = options;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ApplicationDbContext GetDatabase()
        {
            var dbResult = new ApplicationDbContext(_options);
            return dbResult;
        }
    }
}
