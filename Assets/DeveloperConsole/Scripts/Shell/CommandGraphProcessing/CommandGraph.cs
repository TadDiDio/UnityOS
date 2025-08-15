using System;
using System.Collections.Generic;
using DeveloperConsole.Command;

namespace DeveloperConsole.Core.Shell.CommandGraph
{
    public class CommandNode
    {
        public Guid Id { get; } = Guid.NewGuid();
        public CommandRequest Request { get; private set; }
        public IOContext IOContext { get; private set; }
        public List<ExecutionEdge> Edges { get; } = new();

        public CommandNode(CommandRequest request, IOContext ioContext)
        {
            Request = request;
            IOContext = ioContext;
        }
    }

    public class ExecutionEdge
    {
        public CommandNode TargetNode { get; private set; }
        public Func<CommandExecutionResult, bool> Condition { get; private set; }

        public ExecutionEdge(CommandNode targetNode, Func<CommandExecutionResult, bool> condition)
        {
            TargetNode = targetNode;
            Condition = condition;
        }
    }

    public class CommandGraph
    {
        public CommandNode EntryPoint { get; private set; }
        public List<CommandNode> Nodes { get; } = new();

        public void AddNode(CommandNode node)
        {
            EntryPoint ??= node;
            Nodes.Add(node);
        }
    }
}
