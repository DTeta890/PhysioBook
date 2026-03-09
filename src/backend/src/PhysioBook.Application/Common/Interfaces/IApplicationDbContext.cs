using Microsoft.EntityFrameworkCore;

namespace PhysioBook.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
