using DotNet.Testcontainers;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using FluentAssertions;
using Testcontainers.MsSql;

namespace Testcontainers.Flyway.Tests.Migrations;

public class MsSqlMigrationTest : IAsyncLifetime
{
    private const string FlywayDirectory = "../../../../../flyway/sqlserver";
    private const string SqlPassword = "yourStrong(!)Password";
    private const string SqlDbName = "TestDb";

    private readonly INetwork _network;
    private readonly MsSqlContainer _msSqlContainer;
    private FlywayContainer _flywayContainer = null!;

    public MsSqlMigrationTest()
    {
        _network = new NetworkBuilder()
            .Build();
        _msSqlContainer = new MsSqlBuilder()
            .WithNetwork(_network)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        await _msSqlContainer.ExecScriptAsync($"CREATE DATABASE {SqlDbName}");
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
        await _flywayContainer.DisposeAsync();
        await _network.DisposeAsync();
    }

    [Fact]
    public async Task ShouldMigrateMsSqlDb()
    {
        _flywayContainer = new FlywayBuilder()
            .WithNetwork(_network)
            .DependsOn(_msSqlContainer)
            .WithResourceMapping(FlywayDirectory, "/flyway/sql")
            .WithFlywayCommand(FlywayCommand.Migrate)
            .WithDbUser("sa")
            .WithDbPassword(SqlPassword)
            .WithJdbcUrl($"jdbc:sqlserver://{_msSqlContainer.Name.TrimStart('/')}:1433;encrypt=true;trustServerCertificate=true;databaseName={SqlDbName}")
            .Build();

        await _flywayContainer.StartAsync();
        var exitCode = await _flywayContainer.GetExitCodeAsync();
        exitCode.Should().Be(0);

        var execResult = await _msSqlContainer.ExecScriptAsync($"USE {SqlDbName}\n SELECT COUNT(Id) FROM dbo.Testing");
        execResult.ExitCode.Should().Be(0);
        execResult.Stdout.Should().Contain("1 row");
    }

    private string[] GetSqlCommand(MsSqlContainer container)
    {
        string[] sqlCommand =
        [
            "-S", $"{container.Name.TrimStart('/')},1433",
            "-U", "SA",
            "-P", SqlPassword,
            "-d", "master",
            "-Q", $"CREATE DATABASE {SqlDbName}"
        ];
        return sqlCommand;
    }
}