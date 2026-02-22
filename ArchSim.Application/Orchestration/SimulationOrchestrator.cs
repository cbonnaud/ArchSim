using ArchSim.Application.Contracts;
using ArchSim.Cloud.Abstractions;
using ArchSim.Domain.Simulation;
using ArchSim.Domain.Simulation.Cost;

namespace ArchSim.Application.Orchestration;

public class SimulationOrchestrator
{
    private readonly IEnumerable<ICloudProvider> _providers;

    public SimulationOrchestrator(IEnumerable<ICloudProvider> providers)
    {
        _providers = providers;
    }

    public SimulationResponse Run(SimulationRequest request)
    {
        var provider = _providers.FirstOrDefault(p => p.ProviderType == request.Provider)
                    ?? throw new InvalidOperationException($"No provider registered for '{request.Provider}'.");

        var nodes = request.Resources.Select(provider.CreateNode).ToList();

        var graph = provider.BuildGraph(nodes, request.Connections);

        var engine = new SimulationEngine(graph, new LinearCostModel());

        var result = engine.Run(request.Load);

        return new SimulationResponse
        {
            TotalLatency = result.TotalLatency,
            HasErrors = result.HasErrors,
            TotalMonthlyCost = result.TotalMonthlyCost,
            CostPerRequest = result.CostPerRequest
        };
    }
}