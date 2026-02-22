using ArchSim.Application.Abstractions;
using ArchSim.Application.Contracts;
using ArchSim.Application.Orchestration;
using ArchSim.Azure;
using FluentAssertions;

namespace ArchSim.Application.Tests;

public class SimulationOrchestratorTests
{
    [Fact]
    public void Should_simulate_simple_app_to_sql_architecture()
    {
        var request = new SimulationRequest
        {
            Provider = CloudProvider.Azure,
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
                new ConnectionDefinition
                {
                    From = "App",
                    To = "Sql",
                    NetworkLatency = 5
                }
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
            Provider = CloudProvider.Azure,
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

        Assert.Throws<ArgumentException>(() =>
            orchestrator.Run(request));
    }

    [Fact]
    public void Should_throw_when_connection_references_unknown_resource()
    {
        var request = new SimulationRequest
        {
            Provider = CloudProvider.Azure,
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
                new ConnectionDefinition
                {
                    From = "App",
                    To = "Sql", // not defined
                    NetworkLatency = 5
                }
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
            Provider = CloudProvider.Azure,
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