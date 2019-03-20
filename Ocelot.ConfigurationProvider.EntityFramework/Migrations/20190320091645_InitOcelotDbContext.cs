using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ocelot.ConfigurationProvider.EntityFramework.Migrations
{
    public partial class InitOcelotDbContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OcelotConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DisplayName = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcelotConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OcelotGlobalConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConfigurationId = table.Column<int>(nullable: false),
                    RequestIdKey = table.Column<string>(nullable: true),
                    BaseUrl = table.Column<string>(nullable: true),
                    DownstreamScheme = table.Column<string>(nullable: true),
                    HttpHandlerOption = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcelotGlobalConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OcelotReRoutes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConfigurationId = table.Column<int>(nullable: false),
                    DownstreamPathTemplate = table.Column<string>(nullable: true),
                    UpstreamPathTemplate = table.Column<string>(nullable: true),
                    UpstreamHttpMethod = table.Column<string>(nullable: true),
                    AddHeadersToRequest = table.Column<string>(nullable: true),
                    UpstreamHeaderTransform = table.Column<string>(nullable: true),
                    DownstreamHeaderTransform = table.Column<string>(nullable: true),
                    AddClaimsToRequest = table.Column<string>(nullable: true),
                    RouteClaimsRequirement = table.Column<string>(nullable: true),
                    AddQueriesToRequest = table.Column<string>(nullable: true),
                    RequestIdKey = table.Column<string>(nullable: true),
                    CacheOption = table.Column<string>(nullable: true),
                    ReRouteIsCaseSensitive = table.Column<bool>(nullable: false),
                    ServiceName = table.Column<string>(nullable: true),
                    DownstreamScheme = table.Column<string>(nullable: true),
                    QoSOption = table.Column<string>(nullable: true),
                    LoadBalancerOption = table.Column<string>(nullable: true),
                    RateLimitOption = table.Column<string>(nullable: true),
                    AuthenticationOption = table.Column<string>(nullable: true),
                    HttpHandlerOption = table.Column<string>(nullable: true),
                    DownstreamHostAndPorts = table.Column<string>(nullable: true),
                    UpstreamHost = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    DelegatingHandler = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    Timeout = table.Column<int>(nullable: false),
                    DangerousAcceptAnyServerCertificateValidator = table.Column<bool>(nullable: false),
                    SecurityOption = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcelotReRoutes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OcelotConfigurations");

            migrationBuilder.DropTable(
                name: "OcelotGlobalConfigurations");

            migrationBuilder.DropTable(
                name: "OcelotReRoutes");
        }
    }
}
