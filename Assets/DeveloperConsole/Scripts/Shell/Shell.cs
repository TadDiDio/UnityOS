using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;
using DeveloperConsole.Windowing;

namespace DeveloperConsole.Core.Shell
{
    /// <summary>
    /// Routes input to the command executor and execution results to the output manager.
    /// </summary>
    public sealed class Shell : IShell
    {
        private ICommandExecutor _executor;
        private IWindowManager _windowManager;

        private Dictionary<Guid, ShellSession> _sessions = new();


        /// <summary>
        /// Creates a new shell application.
        /// </summary>
        /// <param name="executor">The command executor.</param>
        /// <param name="windowManager">The window manager.</param>
        public Shell(ICommandExecutor executor, IWindowManager windowManager)
        {
            _executor = executor;
            _windowManager = windowManager;
        }

        public void CreateSession(IShellClient client)
        {
            Guid sessionId = Guid.NewGuid();
            _sessions[sessionId] = new ShellSession(this, client);
        }

        public ShellSession GetSession(Guid sessionId)
        {
            return _sessions.GetValueOrDefault(sessionId);
        }

        public async Task<Status> HandleCommandRequestAsync(ShellRequest request, IOContext ioContext, CancellationToken userToken)
        {
            try
            {
                if (request.Window != null)
                {
                    _ = RunWindowedCommand(request, ioContext);
                    return Status.Success;
                }

                return await _executor.ExecuteCommand(request, ioContext, userToken);
            }
            catch (OperationCanceledException)
            {
                ioContext.Output.WriteLine("Command execution was cancelled.");
                return Status.Fail;
            }
            catch (Exception e)
            {
                string error = $"Shell threw an exception: {e}";
                Log.Error(error);
                ioContext.Output.WriteLine(error);
                return Status.Fail;
            }
        }

        private async Task RunWindowedCommand(ShellRequest request, IOContext ioContext)
        {
            _windowManager.RegisterWindow(request.Window);
            await _executor.ExecuteCommand(request, ioContext, request.Window.GetPromptCancellationToken());
        }
    }
}
