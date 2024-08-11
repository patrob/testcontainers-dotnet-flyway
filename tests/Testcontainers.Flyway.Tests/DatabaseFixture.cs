using DotNet.Testcontainers.Containers;

namespace Testcontainers.Flyway.Tests;

public interface IDatabaseFixture
{
    string GetConnectionString();
}

public abstract class DatabaseFixture : IDatabaseFixture, IAsyncLifetime
{
    protected IContainer DatabaseContainer = null!;
    
    public string GetConnectionString()
    {
        return (DatabaseContainer as IDatabaseContainer)!.GetConnectionString();
    }

    public virtual async Task InitializeAsync()
    {
        await DatabaseContainer.StartAsync();
    }

    public virtual async Task DisposeAsync()
    {
        await DatabaseContainer.StopAsync();
    }
}