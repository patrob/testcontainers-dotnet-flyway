using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

namespace Testcontainers.Flyway;

public sealed class FlywayConfiguration : ContainerConfiguration
{
    public string? JdbcUrl { get; }
    public FlywayCommand? FlywayCommand { get; }

    private string InternalFlywayCommand => FlywayCommand.ToString()!.ToLower();

    public string? DbUsername { get; }
    public string? DbPassword { get; }
    public string[] AdditionalCommandArgs { get; } = [];

    /// <summary>
    /// /// Initializes a new instance of the <see cref="FlywayConfiguration" /> class.
    /// </summary>
    /// <param name="jdbcUrl">The JDBC Url.</param>
    /// <param name="flywayCommand">The Flyway command.</param>
    /// <param name="dbUsername">The database Username.</param>
    /// <param name="dbPassword">The database Password.</param>
    /// <param name="additionalCommandArgs">Additional command args</param>
    public FlywayConfiguration(
        string? jdbcUrl = null,
        FlywayCommand? flywayCommand = null,
        string? dbUsername = null,
        string? dbPassword = null,
        string[]? additionalCommandArgs = null)
    {
        JdbcUrl = jdbcUrl;
        FlywayCommand = flywayCommand;
        DbUsername = dbUsername;
        DbPassword = dbPassword;
        AdditionalCommandArgs = additionalCommandArgs ?? [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlywayConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FlywayConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlywayConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FlywayConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlywayConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FlywayConfiguration(FlywayConfiguration resourceConfiguration)
        : this(new FlywayConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlywayConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public FlywayConfiguration(FlywayConfiguration oldValue, FlywayConfiguration newValue)
        : base(oldValue, newValue)
    {
        JdbcUrl = BuildConfiguration.Combine(oldValue.JdbcUrl, newValue.JdbcUrl);
        FlywayCommand = BuildConfiguration.Combine(oldValue.FlywayCommand, newValue.FlywayCommand);
        DbUsername = BuildConfiguration.Combine(oldValue.DbUsername, newValue.DbUsername);
        DbPassword = BuildConfiguration.Combine(oldValue.DbPassword, newValue.DbPassword);
        AdditionalCommandArgs =
            BuildConfiguration.Combine(oldValue.AdditionalCommandArgs, newValue.AdditionalCommandArgs);
    }
}