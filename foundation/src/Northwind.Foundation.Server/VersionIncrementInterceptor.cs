using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Northwind.Foundation.Server;

public class VersionIncrementInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateVersion(eventData);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        UpdateVersion(eventData);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    void UpdateVersion(DbContextEventData eventData)
    {
        if (eventData.Context is null)
            return;
        
        foreach (var versionedEntity in eventData.Context.ChangeTracker.Entries().Where(e => e.Entity is IVersionable).Select(e => e.Entity as IVersionable))
        {
            if (versionedEntity is not null)
                versionedEntity.Version += 1;
        }
    }
}