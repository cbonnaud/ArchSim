using FluentAssertions;
using ArchSim.Domain.Simulation;

namespace ArchSim.Domain.Tests;

public class UtilizationCalculatorTests
{
    [Fact]
    public void Should_calculate_utilization_as_load_divided_by_capacity()
    {
        var load = 40;
        var capacity = 100;

        var utilization = UtilizationCalculator.Calculate(load, capacity);

        utilization.Should().Be(0.4);
    }
}