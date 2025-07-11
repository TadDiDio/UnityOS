using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeveloperConsole.IO;

namespace DeveloperConsole.Core.Shell
{
    /// <summary>
    /// Owns the interaction and state context for a shell session.
    /// </summary>
    public class ShellSession : IDisposable
    {
        private IShellApplication _shell;
        private IShellClient _client;
        private CompositeInputChannel _input;
        private CompositeOutputChannel _output;

        public delegate void SignalHandler(ShellSignal signal);
        private SignalHandler _onSignal;

        private Type _waitingForInputType;
        private TaskCompletionSource<IInput> _inputTaskSource;

        public ShellSession(IShellApplication shell,
                            IShellClient client,
                            List<IInputChannel> extraInputs = null,
                            List<IOutputChannel> extraOutputs = null)
        {
            _shell = shell;
            _client = client;
            _onSignal += _client.GetSignalHandler();

            var inputs = extraInputs ?? new List<IInputChannel>();
            var outputs = extraOutputs ?? new List<IOutputChannel>();

            inputs.Add(_client);
            outputs.Add(_client);

            _input = new CompositeInputChannel(inputs);
            _output = new CompositeOutputChannel(outputs);

            _input.OnInputSubmitted += HandleInput;
        }

        private void HandleInput(IInput input)
        {
            // Route to command if prompted
            if (_waitingForInputType != null && _waitingForInputType.IsAssignableFrom(input.GetType()))
            {
                _inputTaskSource.SetResult(input);
                return;
            }

            if (input is not ICommandInput commandInput) return;

            // Route to shell otherwise
            _ = SubmitInputAsync(commandInput);
        }

        private async Task SubmitInputAsync(ICommandInput input)
        {
            // TODO: Set windowed properly here
            var request = new ShellRequest
            {
                Input = input,
                Session = this,
                Windowed = false
            };

            var output = await _shell.HandleCommandRequestAsync(request);

            string message = output.Status is Status.Success ? output.CommandOutput.Message : output.ErrorMessage;
            WriteOutputLine(message);
        }

        public async Task<bool> Confirm(string message)
        {
            message += " (y/n)";
            var response = await Prompt<PromptResponseInput>(new ChoicePrompt(message, new object []{ "y", "n"}));

            string answer = response.Response.Trim().ToLowerInvariant();
            return answer is "y" or "yes";
        }

        public async Task<T> Prompt<T>(PromptRequest request) where T : IInput
        {
            if (!_client.CanHandlePrompt(request))
            {
                throw new InvalidOperationException($"Prompt request of type {request.GetType().Name} cannot be handled by {_client.GetType().Name}");
            }

            _client.PromptedFor<T>(request);
            _output.WriteLine(request.Message);

            // Only accept valid matches
            _waitingForInputType = typeof(T);
            IInput input;
            do
            {
                _inputTaskSource = new TaskCompletionSource<IInput>();
                input = await _inputTaskSource.Task;
            } while (!typeof(T).IsAssignableFrom(input.GetType()));

            _waitingForInputType = null;
            return (T)input;
        }

        public void WriteOutput(object message)
        {
            _output.Write(message.ToString());
        }

        public void WriteOutputLine(object line)
        {
            _output.WriteLine(line.ToString());
        }

        public void Signal(ShellSignal signal)
        {
            _onSignal?.Invoke(signal);
        }

        public void Dispose()
        {
            _onSignal = null;
            _input.OnInputSubmitted -= HandleInput;
            _input?.Dispose();
        }
    }
}
