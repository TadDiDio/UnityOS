using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeveloperConsole
{
    public interface IInputManager
    {
        public List<IInputSource> InputSources { get; }
        public bool TryGetInput(out string input);
        public Task<string> WaitForInput();
        public void RegisterInputSource(IInputSource source);
        public void UnregisterInputSource(IInputSource source);
        public void UnregisterAllInputSources();
    }
}