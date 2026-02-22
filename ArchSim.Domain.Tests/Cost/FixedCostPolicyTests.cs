using ArchSim.Domain.Simulation;
using ArchSim.Domain.Simulation.Cost;
using FluentAssertions;
using Xunit;

namespace ArchSim.Domain.Tests.Cost;

public class FixedCostPolicyTests
{
    [Fact]
    public void FixedCostPolicy_Returns_Fixed_Value()
    {
        var policy = new FixedCostPolicy(100m);

        var cost = policy.CalculateMonthlyCost(
            load: 500,
            capacity: 100,
            isSaturated: true);

        cost.Should().Be(100m);
    }

    [Fact]
    public void Node_Delegates_Cost_To_Policy()
    {
        var fakePolicy = new FakePolicy();

        var node = new SimulatedNode(
            "api",
            baseLatency: 10,
            capacity: 100,
            timeout: 1000,
            monthlyCost: 0,
            costPolicy: fakePolicy);

        var cost = node.CalculateMonthlyCost(load: 200);

        cost.Should().Be(42m);
        fakePolicy.ReceivedLoad.Should().Be(200);
        fakePolicy.ReceivedCapacity.Should().Be(100);
        fakePolicy.ReceivedSaturation.Should().BeTrue();
    }
}
