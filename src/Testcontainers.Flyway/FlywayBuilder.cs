using Docker.DotNet.Models;
using DotNet.Testcontainers;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

namespace Testcontainers.Flyway;

public sealed class FlywayBuilder : ContainerBuilder<FlywayBuilder, FlywayContainer, FlywayConfiguration>
{
    public const string FlywayImage = "flyway/flyway:10-alpine";
    public const string EntryPoint = "flyway";
    public const FlywayCommand DefaultCommand = FlywayCommand.Validate;
    public const string DefaultUrl = "jdbc:postgres://db:5432/postgres";
    public const string DefaultUser = "postgres";
    public const string DefaultPassword = "postgres";

    /// <summary>
    /// Initializes a new instance of the <see cref="FlywayBuilder" /> class.
    /// </summary>
    public FlywayBuilder() : this(new FlywayConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }
    
    private FlywayBuilder(FlywayConfiguration dockerResourceConfiguration) : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
    }

    protected override FlywayConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override FlywayContainer Build()
    {
        Validate();
        var config = WithCommand([
            $"-url={DockerResourceConfiguration.JdbcUrl ?? DefaultUrl}",
            $"-user={DockerResourceConfiguration.DbUsername ?? DefaultUser}",
            $"-password={DockerResourceConfiguration.DbPassword ?? DefaultPassword}",
            (DockerResourceConfiguration.FlywayCommand ?? DefaultCommand).ToString().ToLower()
        ]).DockerResourceConfiguration;
        return new FlywayContainer(config);
    }

    protected override void Validate()
    {
        base.Validate();
        
        _ = Guard.Argument(DockerResourceConfiguration.JdbcUrl, nameof(DockerResourceConfiguration.JdbcUrl))
            .NotNull()
            .NotEmpty();
        
        _ = Guard.Argument(DockerResourceConfiguration.DbPassword, nameof(DockerResourceConfiguration.DbPassword))
            .NotNull()
            .NotEmpty();
    }

    public FlywayBuilder WithJdbcUrl(string jdbcUrl)
    {
        return Merge(DockerResourceConfiguration, new FlywayConfiguration(jdbcUrl: jdbcUrl));
    }

    public FlywayBuilder WithDbUser(string dbUser)
    {
        return Merge(DockerResourceConfiguration, new FlywayConfiguration(dbUsername: dbUser));
    }
    
    public FlywayBuilder WithDbPassword(string dbPassword)
    {
        return Merge(DockerResourceConfiguration, new FlywayConfiguration(dbPassword: dbPassword));
    }

    public FlywayBuilder WithFlywayCommand(FlywayCommand flywayCommand)
    {
        return Merge(DockerResourceConfiguration, new FlywayConfiguration(flywayCommand: flywayCommand));
    }
    
    public FlywayBuilder WithAdditionalArgs(string[] args)
    {
        return Merge(DockerResourceConfiguration, new FlywayConfiguration(additionalCommandArgs: args));
    }

    /// <inheritdoc />
    protected override FlywayBuilder Init()
    {
        return base.Init()
            .WithImage(FlywayImage)
            .WithEntrypoint(EntryPoint);
    }

    /// <inheritdoc />
    protected override FlywayBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new FlywayConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override FlywayBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new FlywayConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override FlywayBuilder Merge(FlywayConfiguration oldValue, FlywayConfiguration newValue)
    {
        return new FlywayBuilder(new FlywayConfiguration(oldValue, newValue));
    }
}