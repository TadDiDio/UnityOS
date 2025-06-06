using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeveloperConsole
{
    public class BufferedInputManager : IInputManager
    {
        public List<IInputSource> InputSources { get; } = new();
        private Queue<string> _buffer = new();
        private bool _waiting;
       

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

        public bool TryGetInput(out string input)
        {
            input = null;
            if (_waiting) return false;
            return _buffer.TryDequeue(out input);
        }
        
        public void ClearBuffer() => _buffer.Clear();
        public async Task<string> WaitForInput()
        {
            try
            {
                _waiting = true;
                while (true)
                {
                    if (_buffer.TryDequeue(out string input))
                    {
                        return input;
                    }
                    await Task.Yield();
                }
            }
            catch (Exception)
            {
                return "";
            }
            finally
            {
                _waiting = false;
            }
        }
    }
}