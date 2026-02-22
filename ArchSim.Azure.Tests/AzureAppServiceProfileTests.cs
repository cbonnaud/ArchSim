using ArchSim.Azure.Builder;
using ArchSim.Azure.Profiles;
using ArchSim.Domain.Simulation;
using ArchSim.Domain.Simulation.Cost;
using FluentAssertions;

namespace ArchSim.Azure.Tests;

public class AzureAppServiceProfileTests
{
    [Fact]
    public void AppServiceProfile_Should_Create_Node_With_Correct_Capacity_And_Cost()
    {
        var profile = new AzureAppServiceProfile(
            name: "App",
            sku: "P1V2",
            instanceCount: 2);

        var node = profile.ToSimulatedNode();

        // Supposons P1V2 = (500 capacity, 200 cost)
        node.Capacity.Should().Be(1000);  // 500 * 2
        node.MonthlyCost.Should().Be(400); // 200 * 2
    }

    [Fact]
    public void AppServiceProfile_Should_Throw_When_Sku_Unknown()
    {
        var profile = new AzureAppServiceProfile(
            name: "App",
            sku: "INVALID",
            instanceCount: 1);

        Assert.Throws<ArgumentException>(() => profile.ToSimulatedNode());
    }

    [Fact]
    public void AppServiceProfile_Should_Throw_When_InstanceCount_Is_Invalid()
    {
        Assert.Throws<ArgumentException>(() => new AzureAppServiceProfile("App", "P1V2", 0));
    }

    [Fact]
    public void AzureArchitectureBuilder_Should_Create_Valid_Graph()
    {
        var app = new AzureAppServiceProfile("App", "P1V2", 1);
        var sql = new AzureSqlProfile("Sql", "Basic");

        var graph = new AzureArchitectureBuilder()
            .AddResource(app)
            .AddResource(sql)
            .Connect(app, sql, 5)
            .Build();

        graph.Nodes.Should().HaveCount(2);
        graph.Connections.Should().HaveCount(1);
    }

    [Fact]
    public void AzureArchitecture_Should_Be_Simulatable()
    {
        var app = new AzureAppServiceProfile("App", "P1V2", 1);
        var sql = new AzureSqlProfile("Sql", "Basic");

        var graph = new AzureArchitectureBuilder()
            .AddResource(app)
            .AddResource(sql)
            .Connect(app, sql, 5)
            .Build();

        var engine = new SimulationEngine(graph, new LinearCostModel());

        var result = engine.Run(200);

        result.TotalLatency.Should().BeGreaterThan(0);
        result.TotalMonthlyCost.Should().BeGreaterThan(0);
    }
}