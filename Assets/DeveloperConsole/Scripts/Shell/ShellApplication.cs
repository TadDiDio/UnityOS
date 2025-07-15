using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeveloperConsole.Command;
using DeveloperConsole.IO;

namespace DeveloperConsole.Core.Shell
{
    /// <summary>
    /// Routes input to the command executor and execution results to the output manager.
    /// </summary>
    public sealed class ShellApplication : IShellApplication
    {
        private ICommandExecutor _executor;

        private Dictionary<Guid, ShellSession> _sessions = new();


        /// <summary>
        /// Creates a new shell application.
        /// </summary>
        /// <param name="executor">The command executor.</param>
        public ShellApplication(ICommandExecutor executor)
        {
            _executor = executor;
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
            ShellSession session = new(this, promptResponder, outputs);
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
            ShellSession session = new(this, humanInterface);
            _sessions.Add(sessionId, session);
            return sessionId;
        }

        public ShellSession GetSession(Guid sessionId)
        {
            return _sessions.GetValueOrDefault(sessionId);
        }


        public async Task<CommandExecutionResult> HandleCommandRequestAsync(ShellRequest request)
        {
            try
            {
                // TODO: Handle windowed here.

                CommandExecutionRequest executionRequest = new()
                {
                    Shell = this,
                    Resolver = request.CommandResolver,
                    ShellSession = request.Session
                };

                return await _executor.ExecuteCommand(executionRequest);
            }
            catch (Exception e)
            {
                string error = $"Shell had an unexpected error while executing: {e}";
                Log.Error(error);
                return CommandExecutionResult.Fail(error);
            }
        }

        public void Tick() { /* TODO */ }

        public void Dispose()
        {
            foreach (var session in _sessions.Values)
            {
                session.Dispose();
            }
        }
    }
}
