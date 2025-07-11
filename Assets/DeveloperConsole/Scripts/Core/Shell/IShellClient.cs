using System;
using DeveloperConsole.IO;

namespace DeveloperConsole.Core.Shell
{
    public interface IShellClient : IInputSource, IOutputSink
    {
        public Guid ShellSessionId { get; }
        public void Prompt(PromptContext context);
    }
}