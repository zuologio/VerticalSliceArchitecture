using GameCharacterVsaCqrs.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCharacterVsaCqrs.Tests.Testing;

public static class DataContextFactory
{
    public static DataContext CreateInMemoryContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        var context = new DataContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
