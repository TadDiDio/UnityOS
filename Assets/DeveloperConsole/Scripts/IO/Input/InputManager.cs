using System;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public class InputManager : IInputManager
    {
        public event Action<string> InputSubmitted;
        public List<IInputSource> InputSources { get; } = new();

        
        public void RegisterInputSource(IInputSource inputSource)
        {
            if (InputSources.Contains(inputSource)) return;
            InputSources.Add(inputSource);
            inputSource.InputSubmitted += OnInputSubmittedFromSource;
        }

        public void UnregisterInputSource(IInputSource inputSource)
        {
            if (!InputSources.Contains(inputSource)) return;
            InputSources.Remove(inputSource);
            inputSource.InputSubmitted -= OnInputSubmittedFromSource;
        }

        public void UnregisterAllInputSources() => InputSources.Clear();

        private void OnInputSubmittedFromSource(string input)
        {
            InputSubmitted?.Invoke(input);
        }
    }
}