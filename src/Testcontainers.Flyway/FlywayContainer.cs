using DotNet.Testcontainers.Containers;

namespace Testcontainers.Flyway;

public class FlywayContainer : DockerContainer
{
    private readonly FlywayConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="FlywayContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public FlywayContainer(FlywayConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }
}