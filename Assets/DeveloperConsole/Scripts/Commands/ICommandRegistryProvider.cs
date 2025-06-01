using System.Collections.Generic;

namespace DeveloperConsole
{
    public interface ICommandRegistryProvider
    {
        public List<string> GetBaseCommandNames();
        public bool TryGetCommand(string fullyQualifiedName, out ICommand command);
        public string GetDescription(string fullyQualifiedName);
    }
}