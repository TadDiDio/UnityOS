using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;

namespace DeveloperConsole.Core.Shell
{
    public class CommandSubmitter
    {
        private IShellApplication _shell;
        private IOContext _defaultIOContext;
        private ShellSession _session;

        public CommandSubmitter(IShellApplication shell, ShellSession session, IOContext defaultIOContext)
        {
            _session = session;
            _defaultIOContext = defaultIOContext;
            _shell = shell;
        }

        /// TODO: REPLACE ALL THIS WITH GRAPH EXECUTION
        /// <summary>
        /// Submits a batch of commands to be run in the context of the session.
        /// </summary>
        /// <param name="batch">The batch to run.</param>
        /// <param name="ui">The interface this submission should communicate with.</param>
        /// <param name="token">A token to cancel the operation</param>
        /// <param name="ioContext">The IO context this command should run with.</param>
        public async Task SubmitBatch(CommandBatch batch, CancellationToken token, IOContext ioContext = null)
        {
            try
            {
                var aliasExpanded = false;
                var previousStatus = Status.Success;

                var requests = new List<CommandRequest>(batch.Requests);

                for (int i = 0; i < requests.Count; i++)
                {
                    var request = requests[i];
                    if (token.IsCancellationRequested) return;
                    if (!request.Condition.AllowsStatus(previousStatus)) continue;

                    var shellRequest = new ShellRequest
                    {
                        Session = _session,
                        ExpandAliases = !aliasExpanded,
                        Windowed = request.Windowed,
                        CommandResolver = request.Resolver
                    };

                    aliasExpanded = false;

                    var output = await _shell.HandleCommandRequestAsync(shellRequest, token, ioContext ?? _defaultIOContext);

                    switch (output.Status)
                    {
                        case CommandResolutionStatus.Success:
                            previousStatus = Status.Success;
                            break;

                        case CommandResolutionStatus.AliasExpansion:
                            aliasExpanded = true;
                            var batcher = new DefaultCommandBatcher();
                            var insertBatch = batcher.GetBatchFromTokens(output.Tokens);
                            requests.InsertRange(i + 1, insertBatch.Requests);
                            break;
                        case CommandResolutionStatus.Cancelled:
                            previousStatus = Status.Fail;
                            // TODO: Need to output cancelled message but maybe elsewhere
                            break;
                        case CommandResolutionStatus.Fail:
                            previousStatus = Status.Fail;
                            // TODO: Was putting command fail to parse messages here but now nowhere
                            break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ignored: This case happens when a user cancels a command request prompt which is not an issue
            }
        }
    }
}
