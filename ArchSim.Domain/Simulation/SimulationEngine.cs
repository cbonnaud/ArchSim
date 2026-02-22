namespace ArchSim.Domain.Simulation;

public class SimulationEngine
{
    private readonly SimulationGraph _graph;

    public SimulationEngine(SimulationGraph graph)
    {
        _graph = graph;
    }

    public SimulationResult Run(double load)
    {
        var currentNode = GetEntryPoint();
        var totalLatency = 0.0;

        while (currentNode is not null)
        {
            var processingResult = currentNode.Process(load);

            // Add crossed node latency 
            totalLatency += processingResult.Latency;

            // Add network latency
            var outgoingConnection = _graph.Connections.First(conn => conn.From == currentNode);
            totalLatency += outgoingConnection.NetworkLatency;

            // if (totalLatency) TODO :  finish GetDownStreamLatency logic

        }



    }

    private double GetDownStreamLatency(Connection connection)
    {
        return connection.NetworkLatency;
    }

    private SimulatedNode GetEntryPoint()
    {
        return _graph.Nodes.FirstOrDefault(node => !_graph.Connections.Any(conn => conn.To == node))
            ?? throw new InvalidOperationException("No entry point found in the simulation graph.");
    }

    // public SimulationResult Run(double load)
    // {
    //     return _nodes.Aggregate(new SimulationResult(0, false, 0, 0), (result, node) =>
    //     {
    //         var currentResult = node.Process(load);
    //         return new SimulationResult(
    //             result.TotalLatency + currentResult.Latency,
    //             result.HasErrors || currentResult.HasTimedOut,
    //             result.TotalMonthlyCost + node.MonthlyCost,
    //             result.CostPerRequest + node.MonthlyCost / ((decimal)load * SecondsPerMonth)
    //         );
    //     });
    // }
}