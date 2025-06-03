using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeveloperConsole
{
    public class BufferedInputManager : IInputManager
    {
        public List<IInputSource> InputSources { get; } = new();

        private Queue<string> _buffer = new();

       

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

        private void OnInputSubmittedFromSource(string input) => _buffer.Enqueue(input);
        public bool TryGetInput(out string input) => _buffer.TryDequeue(out input);
        
        public void ClearBuffer() => _buffer.Clear();
        public async Task<string> WaitForInput()
        {
            try
            {
                while (true)
                {
                    if (TryGetInput(out var input)) return input;
                    await Task.Yield();
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}