using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DeveloperConsole
{
    /// <summary>
    /// Command interface.
    /// </summary>
    public interface ICommand
    {
        public Task<CommandResult> ExecuteAsync(CommandContext context);
        public void RegisterTypeParsers();
    }

    /// <summary>
    /// Base class for all commands.
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        public virtual void RegisterTypeParsers() { }

        protected void print(object obj)
        {
            Debug.Log(obj.ToString());
        }
        
        public abstract Task<CommandResult> ExecuteAsync(CommandContext context);
    }

    public class CommandContext
    {
        public List<string> Tokens;
        public ShellApplication Shell;
        [CanBeNull] public ConsoleState ConsoleState;
    }
    public class CommandResult
    {
        public readonly string Message;
        public CommandResult() => Message = ""; 
        public CommandResult(string message) => Message = message;
    }
}