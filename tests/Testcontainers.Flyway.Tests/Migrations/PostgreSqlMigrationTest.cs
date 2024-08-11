using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using FluentAssertions;
using Testcontainers.PostgreSql;

namespace Testcontainers.Flyway.Tests.Migrations;

public class PostgreSqlMigrationTest : IAsyncLifetime
{
    private const string FlywayDirectory = "../../../../../flyway/sql";
    private readonly INetwork _network;
    private readonly PostgreSqlContainer _postgreSqlContainer;

    public PostgreSqlMigrationTest()
    {
        _network = new NetworkBuilder()
            .WithName("test-network")
            .Build();
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithNetwork(_network)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
        await _network.DisposeAsync();
    }

    [Fact]
    public async Task ShouldMigratePostgreSqlDb()
    {
        var flywayContainer = new FlywayBuilder()
            .WithNetwork(_network)
            .DependsOn(_postgreSqlContainer)
            .WithResourceMapping(FlywayDirectory, "/flyway/sql")
            .WithFlywayCommand(FlywayCommand.Migrate)
            .WithDbUser("postgres")
            .WithDbPassword("postgres")
            .WithJdbcUrl($"jdbc:postgresql://{_postgreSqlContainer.Name.TrimStart('/')}:5432/postgres")
            .Build();

        await flywayContainer.StartAsync();
        var exitCode = await flywayContainer.GetExitCodeAsync();
        exitCode.Should().Be(0);

        var execResult = await _postgreSqlContainer.ExecScriptAsync("SELECT COUNT(Id) FROM Testing");
        execResult.ExitCode.Should().Be(0);
        execResult.Stderr.Should().BeEmpty();
        execResult.Stdout.Should().Contain("1 row");
    }
}