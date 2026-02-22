using ArchSim.Application.Contracts;
using ArchSim.Application.Orchestration;
using ArchSim.Azure;
using ArchSim.Domain.Simulation;
using ArchSim.Domain.Simulation.Cost;
using FluentAssertions;

namespace ArchSim.Application.Tests;

public class SimulationOrchestratorTests
{
    [Fact]
    public void Should_simulate_simple_app_to_sql_architecture()
    {
        var request = new SimulationRequest
        {
            Provider = Cloud.Models.CloudProviderType.Azure,
            Load = 300,
            Resources = new()
            {
                new ResourceDefinition
                {
                    Type = "AppService",
                    Name = "App",
                    Sku = "P1V2",
                    InstanceCount = 2
                },
                new ResourceDefinition
                {
                    Type = "Sql",
                    Name = "Sql",
                    Sku = "Basic"
                }
            },
            Connections = new()
            {
                new Connection(
                    new SimulatedNode("App", 1, 1, 1, 1, new FixedCostPolicy(200)),
                    new SimulatedNode("Sql", 1, 1, 1, 1, new FixedCostPolicy(50)),
                    10,
                    5000)
            }
        };

        var orchestrator = new SimulationOrchestrator([new AzureCloudProvider()]);

        var response = orchestrator.Run(request);

        response.Should().NotBeNull();
        response.TotalLatency.Should().BeGreaterThan(0);
        response.TotalMonthlyCost.Should().BeGreaterThan(0);
        response.CostPerRequest.Should().BeGreaterThan(0);
        response.HasErrors.Should().BeFalse();
    }

    [Fact]
    public void Should_throw_when_resource_type_is_unknown()
    {
        var request = new SimulationRequest
        {
            Provider = Cloud.Models.CloudProviderType.Azure,
            Load = 100,
            Resources = new()
            {
                new ResourceDefinition
                {
                    Type = "UnknownType",
                    Name = "X",
                    Sku = "Any"
                }
            },
            Connections = new()
        };

        var orchestrator = new SimulationOrchestrator([new AzureCloudProvider()]);

        Assert.Throws<InvalidOperationException>(() => orchestrator.Run(request));
    }

    [Fact]
    public void Should_throw_when_connection_references_unknown_resource()
    {
        var request = new SimulationRequest
        {
            Provider = Cloud.Models.CloudProviderType.Azure,
            Load = 100,
            Resources = new()
            {
                new ResourceDefinition
                {
                    Type = "AppService",
                    Name = "App",
                    Sku = "P1V2",
                    InstanceCount = 1
                }
            },
            Connections = new()
            {
                new Connection(
                    new SimulatedNode("App", 1, 1, 1, 1, new FixedCostPolicy(200)),
                    new SimulatedNode("UnknownResource", 1, 1, 1, 1, new FixedCostPolicy(50)),
                    5,
                    5000)
            }
        };

        var orchestrator = new SimulationOrchestrator([new AzureCloudProvider()]);

        Assert.Throws<KeyNotFoundException>(() =>
            orchestrator.Run(request));
    }

    [Fact]
    public void Should_handle_zero_load_from_request()
    {
        var request = new SimulationRequest
        {
            Provider = Cloud.Models.CloudProviderType.Azure,
            Load = 0,
            Resources = new()
            {
                new ResourceDefinition
                {
                    Type = "AppService",
                    Name = "App",
                    Sku = "P1V2",
                    InstanceCount = 1
                }
            },
            Connections = new()
        };

        var orchestrator = new SimulationOrchestrator([new AzureCloudProvider()]);

        var response = orchestrator.Run(request);

        response.CostPerRequest.Should().Be(0);
        response.HasErrors.Should().BeFalse();
    }
}