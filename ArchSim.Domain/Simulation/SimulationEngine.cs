using ArchSim.Domain.Simulation.Cost;

namespace ArchSim.Domain.Simulation;

public class SimulationEngine
{
    private readonly SimulationGraph _graph;
    private const int SecondPerMonth = 30 * 24 * 60 * 60;
    private readonly ICostModel _costModel;

    public SimulationEngine(SimulationGraph graph, ICostModel costModel)
    {
        _graph = graph;
        _costModel = costModel;
    }

    public SimulationResult Run(double load)
    {
        var memo = new Dictionary<ISimulatedNode, TraversalResult>();

        var entry = GetEntryPoint();

        var result = SimulateNode(entry, load, memo);

        var totalCost = _costModel.CalculateTotalMonthlyCost(
            _graph.Nodes,
            load);

        var costPerRequest = _costModel.CalculateCostPerRequest(
            totalCost,
            load);

        return new SimulationResult(
            result.TotalLatency,
            result.HasErrors,
            totalCost,
            costPerRequest
        );
    }

    private TraversalResult SimulateNode(
        ISimulatedNode node,
        double load,
        Dictionary<ISimulatedNode, TraversalResult> memo)
    {
        if (memo.TryGetValue(node, out var cached))
        {
            return cached;
        }

        var nodeResult = node.Process(load);

        var outgoingConnections = _graph.GetOutgoingConnections(node);

        if (!outgoingConnections.Any())
        {
            var leafResult = new TraversalResult(
                nodeResult.Latency,
                nodeResult.HasTimedOut);

            memo[node] = leafResult;
            return leafResult;
        }

        double maxBranchLatency = 0;
        bool hasTimedOut = nodeResult.HasTimedOut;

        foreach (var connection in outgoingConnections)
        {
            var downstream = SimulateNode(connection.To, load, memo);

            double segmentLatency = connection.NetworkLatency + downstream.TotalLatency;

            bool connectionTimeout = segmentLatency >= connection.Timeout;

            hasTimedOut |= connectionTimeout || downstream.HasErrors;

            maxBranchLatency = Math.Max(maxBranchLatency, segmentLatency);
        }

        var result = new TraversalResult(
            nodeResult.Latency + maxBranchLatency,
            hasTimedOut);

        memo[node] = result;
        return result;
    }

    private sealed record TraversalResult(
        double TotalLatency,
        bool HasErrors
    );

    private ISimulatedNode GetEntryPoint()
    {
        return _graph.Nodes.FirstOrDefault(node => !_graph.Connections.Any(conn => conn.To == node))
            ?? throw new InvalidOperationException("No entry point found in the simulation graph.");
    }
}