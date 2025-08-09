using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;
using DeveloperConsole.IO;
using DeveloperConsole.Windowing;

namespace DeveloperConsole.Core.Shell
{
    /// <summary>
    /// Routes input to the command executor and execution results to the output manager.
    /// </summary>
    public sealed class ShellApplication : IShellApplication
    {
        private ICommandExecutor _executor;
        private IWindowManager _windowManager;

        private Dictionary<Guid, ShellSession> _sessions = new();


        /// <summary>
        /// Creates a new shell application.
        /// </summary>
        /// <param name="executor">The command executor.</param>
        /// <param name="windowManager">The window manager.</param>
        public ShellApplication(ICommandExecutor executor, IWindowManager windowManager)
        {
            _executor = executor;
            _windowManager = windowManager;
        }


        /// <summary>
        /// Creates a new shell session.
        /// </summary>
        /// <param name="promptResponder">The prompt responder.</param>
        /// <param name="outputs">0 or more outputs associated with this session.</param>
        /// <returns>The session id.</returns>
        public Guid CreateSession(IPromptResponder promptResponder, List<IOutputChannel> outputs = null)
        {
            Guid sessionId = Guid.NewGuid();
            ShellSession session = new(this, promptResponder, sessionId, outputs);
            _sessions.Add(sessionId, session);
            return sessionId;
        }


        /// <summary>
        /// Creates a new shell session for a human interface.
        /// </summary>
        /// <param name="humanInterface">The human interface.</param>
        /// <returns></returns>
        public Guid CreateSession(IHumanInterface humanInterface)
        {
            Guid sessionId = Guid.NewGuid();
            ShellSession session = new(this, humanInterface, sessionId);
            _sessions.Add(sessionId, session);
            return sessionId;
        }

        public ShellSession GetSession(Guid sessionId)
        {
            return _sessions.GetValueOrDefault(sessionId);
        }

        public async Task<CommandExecutionResult> HandleCommandRequestAsync(
            ShellRequest request,
            CancellationToken userToken,
            UserInterface defaultUserInterface)
        {
            try
            {
                request.Shell = this;

                if (request.Windowed)
                {
                    _ = RunWindowedCommand(request);
                    return CommandExecutionResult.Success();
                }

                return await _executor.ExecuteCommand(request, defaultUserInterface, userToken);
            }
            catch (OperationCanceledException)
            {
                return CommandExecutionResult.Fail("Command execution was canceled.");
            }
            catch (Exception e)
            {
                string error = $"Shell had an unexpected error while executing: {e}";
                Log.Error(error);
                return CommandExecutionResult.Fail(error);
            }
        }

        private async Task RunWindowedCommand(ShellRequest request)
        {
            var config = WindowConfigFactory.CommandWindow();

            var commandWindow = new CommandWindow(config);
            _windowManager.RegisterWindow(commandWindow);

            await _executor.ExecuteCommand(request, commandWindow.GetInterface(), commandWindow.GetCommandCancellationToken());
        }

        public void Tick() { /* TODO */ }

        public void Dispose() { /* TODO */ }
    }
}
