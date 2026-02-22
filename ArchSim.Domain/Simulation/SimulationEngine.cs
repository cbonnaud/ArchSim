namespace ArchSim.Domain.Simulation;

public class SimulationEngine
{
    private readonly SimulationGraph _graph;
    private const int SecondPerMonth = 30 * 24 * 60 * 60;

    public SimulationEngine(SimulationGraph graph)
    {
        _graph = graph;
    }

    public SimulationResult Run(double load)
    {
        var entry = GetEntryPoint();

        var result = SimulateNode(entry, load);

        var costPerRequest = CalculateCostPerRequest(result.TotalCost, load);

        return new SimulationResult(
            result.TotalLatency,
            result.HasErrors,
            result.TotalCost,
            costPerRequest
        );
    }

    private decimal CalculateCostPerRequest(decimal totalCost, double load)
    {
        if (load <= 0)
            throw new ArgumentException("Load must be greater than zero to calculate cost per request.");

        return totalCost / ((decimal)load * SecondPerMonth);
    }

    private TraversalResult SimulateNode(
        SimulatedNode node,
        double load)
    {
        // 1️⃣ Process current node
        var processing = node.Process(load);

        var nodeLatency = processing.Latency;
        var nodeCost = node.MonthlyCost;
        var hasErrors = processing.HasTimedOut;

        // 2️⃣ Get outgoing connections
        var outgoing = _graph.Connections
            .Where(c => c.From == node)
            .ToList();

        // 3️⃣ Leaf node → no downstream
        if (!outgoing.Any())
        {
            return new TraversalResult(
                nodeLatency,
                hasErrors,
                nodeCost
            );
        }

        // 4️⃣ Evaluate downstream branches
        var branchLatencies = new List<double>();
        var totalCost = nodeCost;

        foreach (var connection in outgoing)
        {
            var downstream = SimulateNode(connection.To, load);

            var segmentLatency =
                connection.NetworkLatency
                + downstream.TotalLatency;

            // Timeout evaluation (local to connection)
            if (segmentLatency >= connection.Timeout)
            {
                hasErrors = true;
            }

            branchLatencies.Add(segmentLatency);

            totalCost += downstream.TotalCost;

            if (downstream.HasErrors)
            {
                hasErrors = true;
            }
        }

        var downstreamLatency = branchLatencies.Max();

        var totalLatency = nodeLatency + downstreamLatency;

        return new TraversalResult(
            totalLatency,
            hasErrors,
            totalCost
        );
    }

    private sealed record TraversalResult(
        double TotalLatency,
        bool HasErrors,
        decimal TotalCost
    );

    private SimulatedNode GetEntryPoint()
    {
        return _graph.Nodes.FirstOrDefault(node => !_graph.Connections.Any(conn => conn.To == node))
            ?? throw new InvalidOperationException("No entry point found in the simulation graph.");
    }
}