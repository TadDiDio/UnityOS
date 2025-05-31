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

    #region COMMAND BASES
    
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
    
    /// <summary>
    /// A simple command which exhibits one-shot behavior.
    /// </summary>
    public abstract class SimpleCommand : CommandBase
    {
        public override async Task<CommandResult> ExecuteAsync(CommandContext context)
        {
            return await Task.FromResult(Execute(context));
        }
        protected abstract CommandResult Execute(CommandContext context);
    }
    
    /// <summary>
    /// Simply a more ergonomic name for an async command.
    /// </summary>
    public abstract class AsyncCommand : CommandBase { }
    #endregion

    public class CommandContext
    {
        public List<string> Tokens;
        [CanBeNull] public ConsoleState ConsoleState;
    }
    public class CommandResult
    {
        public readonly string Message;
        public CommandResult() => Message = ""; 
        public CommandResult(string message) => Message = message;
    }
}