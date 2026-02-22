# ArchSim — Business Rules (v1 Steady State)

## 1. Scope & Model Assumptions

ArchSim v1 simulates a cloud architecture in steady state using a Directed Acyclic Graph (DAG).

The simulation is:

- Deterministic
- Stateless
- Time-agnostic
- Without dynamic scaling
- Without discrete event simulation

The objective is to compute:

- Total end-to-end latency
- Error presence (timeouts)
- Total monthly cost
- Cost per request


# 2. Graph Rules

## 2.1 Structural Constraints

A SimulationGraph must satisfy:

1. The graph must be acyclic.
2. The graph must have exactly one entry point (root).
   - A root is a node with no incoming connections.
   - If zero or more than one root exists → the graph is invalid.
3. Every Connection must reference nodes present in the graph.

Graph validation occurs at construction time.


# 3. Node Rules

A SimulatedNode represents a steady-state processing unit.

## 3.1 Valid Configuration

A node must satisfy:

- capacity > 0
- baseLatency >= 0
- timeout > 0
- monthlyCost >= 0

Invalid configurations must throw at construction time.


# 4. Load Rules

- load >= 0 is valid.
- load = 0 is allowed.

Load is interpreted as requests per second.


# 5. Latency Model

## 5.1 Utilization

utilization = load / capacity

## 5.2 Node Latency

- If utilization <= 1  
  → latency = baseLatency

- If utilization > 1  
  → latency = baseLatency * utilization

This models linear degradation under saturation.


# 6. Saturation

A node is considered saturated if:

utilization > 1

Saturation:

- Does NOT interrupt execution
- Does NOT trigger automatic errors
- Only influences latency

It is informational in v1.


# 7. Timeout Rules

Timeouts represent SLA violations.

## 7.1 Node Timeout

A node has timed out if:

latency >= node.timeout

## 7.2 Connection Timeout

For a connection:

segmentLatency = connection.networkLatency + downstream.TotalLatency

A connection has timed out if:

segmentLatency >= connection.timeout

## 7.3 Propagation Rule

Timeouts:

- Do NOT interrupt propagation.
- Do NOT stop downstream simulation.
- Only mark the branch as having errors.

The final simulation result reports:

HasErrors = true

If any node or connection in the evaluated graph timed out.


# 8. Parallel Branch Rule

For a node with multiple outgoing connections:

branchLatency = max(segmentLatency of all branches)

Total latency is:

nodeLatency + max(branchLatency)

Parallel branches represent concurrent execution paths.


# 9. Cost Model

## 9.1 Total Monthly Cost

The total monthly cost is computed using the configured ICostModel.

By default:

TotalMonthlyCost = sum(node.CalculateMonthlyCost(load))

## 9.2 Cost Per Request

If:

load <= 0

Then:

CostPerRequest = 0

Otherwise:

monthlyRequests = load * 60 * 60 * 24 * 30  
CostPerRequest = TotalMonthlyCost / monthlyRequests


# 10. Special Cases

## 10.1 Single Node Graph

A graph with a single node and no connections is valid.

## 10.2 Multiple Disconnected Nodes

Invalid (would result in multiple roots).

## 10.3 Timeout = 0

Not allowed.

If a system must effectively have no timeout, it must explicitly set a large timeout value.


# 11. Determinism Guarantee

Given:

- The same graph
- The same load

The simulation must always produce:

- The same latency
- The same error state
- The same cost results

No randomness is allowed in v1.