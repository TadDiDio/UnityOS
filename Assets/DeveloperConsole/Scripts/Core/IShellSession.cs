using System.Collections.Generic;
using System.Threading.Tasks;
using DeveloperConsole.IO;

namespace DeveloperConsole.Core
{
    public interface IShellSession
    {
        public bool WaitingForInput { get; }
        public Dictionary<string, List<string>> Aliases { get; }
        public void ReceiveInput(TextInput input);
        public Task<TextInput> WaitForInput();
    }
}