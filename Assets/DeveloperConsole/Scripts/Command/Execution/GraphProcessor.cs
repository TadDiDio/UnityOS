using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command.Execution;
using DeveloperConsole.Parsing.Graph;
using DeveloperConsole.Scripts.Command.Execution;
using DeveloperConsole.Windowing;

namespace DeveloperConsole.Core.Shell
{
    public class GraphProcessor
    {
        private IShell _shell;
        private IOContext _defaultIOContext;
        private ShellSession _session;

        public GraphProcessor(IShell shell, ShellSession session, IOContext defaultIOContext)
        {
            _session = session;
            _defaultIOContext = defaultIOContext;
            _shell = shell;
        }

        /// <summary>
        /// Submits a batch of commands to be run in the context of the session.
        /// </summary>
        /// <param name="graph">The graph to run.</param>
        /// <param name="token">A token to cancel the operation</param>
        public async Task ProcessCommandGraphAsync(CommandGraph graph, CancellationToken token)
        {

            foreach (var node in GraphIterator.TraverseBFS(graph.Root))
            {
                bool windowed = node.ExecutionContext.Windowed;

                if (windowed)
                {
                    var windowConfig = WindowConfigFactory.CommandWindow();
                    var commandWindow = new CommandWindow(windowConfig);
                    node.ExecutionContext.IoContext = IOContext.CreateFromClient(commandWindow, _session);
                    node.ExecutionContext.IoContext.Prompt.InitializePromptHeader(ShellSession.PromptEnd);
                    node.ExecutionContext.CommandWindow = commandWindow;
                }
                else node.ExecutionContext.IoContext = _defaultIOContext.Clone();
            }

            foreach (var annotation in graph.Annotations)
            {
                annotation.Setup();
            }

            await RunNodeAndChildrenAsync(graph.Root, Status.Success, token);
        }

        private async Task RunNodeAndChildrenAsync(GraphNode node, Status parentStatus, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            // Decide whether to execute node
            bool shouldRun = node.Condition switch
            {
                ExecutionCondition.Always => true,
                ExecutionCondition.OnSuccess => parentStatus == Status.Success,
                ExecutionCondition.OnFailure => parentStatus == Status.Fail,
                _ => true
            };

            Status currentStatus;

            if (shouldRun)
            {
                currentStatus = await RunNodeOnMainThread(node, token);
            }
            else
            {
                // Dummy status based on condition
                currentStatus = node.Condition switch
                {
                    ExecutionCondition.OnSuccess => Status.Fail,
                    ExecutionCondition.OnFailure => Status.Success,
                    _ => Status.Success
                };
            }

            if (node.Outgoing.Count == 1)
            {
                // Sequential execution
                await RunNodeAndChildrenAsync(node.Outgoing[0].Target, currentStatus, token);
            }
            else if (node.Outgoing.Count > 1)
            {
                // Parallel execution (conceptual, still main thread)
                var tasks = new List<Task>();
                foreach (var edge in node.Outgoing)
                {
                    // Dispatch each child to the main thread and await it
                    tasks.Add(RunNodeAndChildrenAsync(edge.Target, currentStatus, token));
                }
                await Task.WhenAll(tasks);
            }
        }

        // Runs a single node on the Unity main thread safely
        private Task<Status> RunNodeOnMainThread(GraphNode node, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<Status>();

            UnityMainThreadDispatcher.Instance.Enqueue(async () =>
            {
                try
                {
                    var result = await ExecuteNode(node, token);
                    tcs.SetResult(result);
                }
                catch (System.Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }

        private async Task<Status> ExecuteNode(GraphNode node, CancellationToken token)
        {
            // Sentinel root node
            if (node.Command == null) return Status.Success;

            var request = new ShellRequest
            {
                Command = node.Command,
                Session = _session,
                Window = node.ExecutionContext.CommandWindow
            };

            return await _shell.HandleCommandRequestAsync(request, node.ExecutionContext.IoContext, token);
        }
    }
}
