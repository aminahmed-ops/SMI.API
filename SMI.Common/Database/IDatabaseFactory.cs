using Microsoft.EntityFrameworkCore;

namespace SMI.Common.Database
{
    public interface IDatabaseFactory<T> : IDisposable where T : DbContext
    {
        T GetDatabase();
    }
}
