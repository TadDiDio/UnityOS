using System.Collections.Generic;
using DeveloperConsole.Command;
using DeveloperConsole.Scripts.Command.Execution;

namespace DeveloperConsole.Parsing.Graph
{
    public class GraphNode
    {
        public ICommand Command;
        public ExecutionCondition Condition = ExecutionCondition.Always;
        public List<GraphEdge> Outgoing = new();
        public readonly GraphExecutionContext ExecutionContext = new();

        public override string ToString() => $"[{Command?.Schema?.Name ?? "Null"}]";
    }
}
