using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DeveloperConsole.Core.Shell.CommandGraph
{
    public class CommandGraphExecutor
    {
        public async Task ExecuteGraphAsync(CommandGraph graph, IShellApplication shell, CancellationToken token)
        {
            await Task.Delay(1, token);
            var visited = new HashSet<Guid>();

            CommandNode node = graph.EntryPoint;

            if (!visited.Add(node.Id)) return;

        }
    }
}
