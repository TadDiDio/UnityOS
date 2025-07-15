using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeveloperConsole.Command;
using DeveloperConsole.IO;
using UnityEngine;

namespace DeveloperConsole.Core.Shell
{

    /// <summary>
    /// A delegate representing a shell signal handler.
    /// </summary>
    public delegate void ShellSignalHandler(ShellSignal signal);

    /// <summary>
    /// Owns the interaction and state context for a shell session.
    /// </summary>
    public class ShellSession : IDisposable
    {
        private IPromptResponder _promptResponder;
        private IShellApplication _shell;
        private CompositeOutputChannel _output;

        private ShellSignalHandler _onShellSignal;


        /// <summary>
        /// Creates a new shell session for a human interface.
        /// </summary>
        /// <param name="shell">The shell.</param>
        /// <param name="humanInterface">The human interface.</param>
        public ShellSession(IShellApplication shell, IHumanInterface humanInterface)
        {
            Initialize(shell, humanInterface, new List<IOutputChannel> {humanInterface});
        }


        /// <summary>
        /// Creates a new shell session.
        /// </summary>
        /// <param name="shell">The shell.</param>
        /// <param name="responder">The prompt responder.</param>
        /// <param name="outputs">0 or more output channels.</param>
        public ShellSession(
            IShellApplication shell,
            IPromptResponder responder,
            List<IOutputChannel> outputs = null)
        {
            Initialize(shell, responder, outputs);
        }

        private void Initialize(IShellApplication shell, IPromptResponder responder,
            List<IOutputChannel> outputs = null)
        {
            _shell = shell;
            _promptResponder = responder;
            _onShellSignal += responder.GetSignalHandler();
            _output = new CompositeOutputChannel(outputs);

            // TODO: May need to move this to some sort of OnSystemReady action.
            _ = CommandPromptLoop();
        }


        private async Task CommandPromptLoop()
        {
            while (true)
            {
                var resolver = await PromptAsync<ICommandResolver>(Shell.Prompt.Command());

                // TODO: Set windowed properly here
                var request = new ShellRequest { Session = this, Windowed = false, CommandResolver = resolver };
                var output = await _shell.HandleCommandRequestAsync(request);

                string message = output.Status is Status.Success ? output.CommandOutput.Message : output.ErrorMessage;
                WriteLine(message);
            }
            // ReSharper disable once FunctionNeverReturns
        }


        public async Task<T> PromptAsync<T>(Prompt prompt)
        {
            try
            {
                while (true)
                {
                    var response = await _promptResponder.HandlePrompt(prompt);

                    if (response is T typed)
                    {
                        if (prompt.Validator.Invoke(typed)) return typed;

                        WriteLine(MessageFormatter.Error("Invalid response entered."));
                        continue;
                    }

                    if (response is not string stringResponse) throw new InvalidOperationException($"{_promptResponder.GetType().Name} responded with {response.GetType().Name}" + $"when asked for {prompt.RequestedType.Name}!");

                    if (!ConsoleAPI.Parsing.CanAdaptType(typeof(T)))
                    {
                        throw new InvalidOperationException($"No type adapter registered for type {typeof(T).Name}");
                    }

                    var result = ConsoleAPI.Parsing.AdaptTypeFromString(typeof(T), stringResponse);

                    if (!result.Success)
                    {
                        WriteLine(MessageFormatter.Error("Invalid response entered."));
                        continue;
                    }

                    if (prompt.Validator.Invoke(result.As<T>())) return result.As<T>();
                    WriteLine(MessageFormatter.Error("Invalid response entered."));
                }
            }
            catch (Exception e)
            {
                WriteLine(MessageFormatter.Error(e.Message));
                throw;
            }
        }

        public void Write(object message)
        {
            _output.Write(message.ToString());
        }

        public void WriteLine(object line)
        {
            _output.WriteLine(line.ToString());
        }

        public void Signal(ShellSignal signal)
        {
            _onShellSignal?.Invoke(signal);
        }

        public void Dispose()
        {
            _onShellSignal = null;
        }
    }
}
