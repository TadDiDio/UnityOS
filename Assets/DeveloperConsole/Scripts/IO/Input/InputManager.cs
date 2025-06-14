using System;
using System.Collections.Generic;
using DeveloperConsole.Command;
using DeveloperConsole.Core;

namespace DeveloperConsole.IO
{
    /// <summary>
    /// An input manager for coalescing inputs for the system.
    /// </summary>
    public class InputManager : IInputManager
    {
        public List<IInputSource> InputSources { get; } = new();
        public Action<CommandRequest> OnCommandInput { get; set; }

        public void RegisterInputSource(IInputSource inputSource)
        {
            if (InputSources.Contains(inputSource)) return;
            InputSources.Add(inputSource);
            inputSource.InputSubmitted += OnSourceInput;
        }

        public void UnregisterInputSource(IInputSource inputSource)
        {
            if (!InputSources.Contains(inputSource)) return;
            InputSources.Remove(inputSource);
            inputSource.InputSubmitted -= OnSourceInput;
        }

        public void UnregisterAllInputSources() => InputSources.Clear();

        private void OnSourceInput(IInput input)
        {
            ShellSession session = input.ShellSession;

            if (session == null)
            {
                Log.Error("No shell session found");
                return;
            }
            
            // If a session is waiting for input, send it if it is text, otherwise ignore it.
            if (session.WaitingForInput())
            {
                if (input is TextInput textInput)
                {
                    session.ReceiveInput(textInput);
                }
                else
                {
                    // TODO: May need to tweak this to be supressable
                    Log.Warning($"Ignored input of type {input.GetType().Name} while waiting for text input.");
                }
                return;
            }

            var commandRequest = input.GetCommandRequest();
            if (commandRequest != null)
            {
                OnCommandInput?.Invoke(commandRequest);
            }
            else
            {
                Log.Warning($"Input of type {input.GetType().Name} returned null when getting command request.");
            }
        }
    }
}