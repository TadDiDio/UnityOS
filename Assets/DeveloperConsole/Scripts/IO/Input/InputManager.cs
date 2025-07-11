using System;
using System.Collections.Generic;
using DeveloperConsole.Command;
using DeveloperConsole.Core;
using DeveloperConsole.Core.Shell;

namespace DeveloperConsole.IO
{
    /// <summary>
    /// An input manager for coalescing inputs for the system.
    /// </summary>
    public class InputManager : IInputManager
    {
        public List<IInputSource> InputSources { get; } = new();
        public Action<CommandRequest> OnCommandInput { get; set; }

        private IOutputManager _outputManager;
        
        public InputManager(IOutputManager outputManager)
        {
            _outputManager = outputManager;
        }
        
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
            
            if (session.WaitingForInput)
            {
                SendToSession(input, session);
                return;
            }

            SendToShell(input, session);
        }

        private void SendToSession(IInput input, ShellSession session)
        {
            Type requestedType = session.RequestedInputType;
            if (!typeof(IInput).IsAssignableFrom(requestedType))
            {
                Log.Error($"Session is waiting on {requestedType} which is not an input type.");
                return;
            }
            
            if (input.GetType() == requestedType)
            {
                session.ReceiveInput(input);
            }
        }
        
        
        private void SendToShell(IInput input, ShellSession session)
        {
            var commandRequest = input.GetCommandRequest();
            if (commandRequest != null)
            {
                if (input is TextInput textInput)
                {
                    _outputManager.Emit(new InputMirrorMessage(
                        session,
                        session.CurrentPrompt,
                        textInput.Input)
                    );
                }
                
                OnCommandInput?.Invoke(commandRequest);
            }
            else
            {
                Log.Warning($"Input of type {input.GetType().Name} returned null when getting command request.");
            }
        }
    }
}