using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public interface ICommand
    {
        public Task<CommandResult> ExecuteAsync(CommandArgsBase args);
        public void RegisterTypeParsers();
    }

    #region COMMAND BASES
    public abstract class CommandBase : ICommand
    {
        public virtual void RegisterTypeParsers() { }

        protected void Print(object obj)
        {
            Debug.Log(obj.ToString());
        }
        
        public abstract Task<CommandResult> ExecuteAsync(CommandArgsBase args);
    }
    public abstract class ConsoleCommand : CommandBase
    {
        public override async Task<CommandResult> ExecuteAsync(CommandArgsBase args)
        {
            if (args is ConsoleCommandArgs commandArgs) return await Task.FromResult(Execute(commandArgs));
            
            Debug.LogError($"Invalid casting from type {args.GetType()} to BuiltInCommandArgs. Ensure " +
                           $"that this command inherits from {typeof(ConsoleCommand)} or use " +
                           $"{typeof(SimpleCommand)} if you don't need to modify console state."); 
            return null;
        }
        protected abstract CommandResult Execute(ConsoleCommandArgs args);
    }
    public abstract class SimpleCommand : CommandBase
    {
        public override async Task<CommandResult> ExecuteAsync(CommandArgsBase args)
        {
            if (args is CommandArgs commandArgs) return await Task.FromResult(Execute(commandArgs));
            
            Debug.LogError($"Invalid casting from type {args.GetType()} to CommandArgs. Ensure " +
                           $"that this command inherits from {typeof(SimpleCommand)} or use " +
                           $"{typeof(ConsoleCommand)} if you wish to modify console state."); 
            return null;
        }
        protected abstract CommandResult Execute(CommandArgs args);
    }
    public abstract class AsyncCommand : CommandBase
    {
        public override async Task<CommandResult> ExecuteAsync(CommandArgsBase args)
        {
            if (args is CommandArgs commandArgs) return await ExecuteAsync(commandArgs);
            
            Debug.LogError($"Invalid casting from type {args.GetType()} to CommandArgs. Ensure " +
                           $"that this command inherits from {typeof(SimpleCommand)} or use " +
                           $"{typeof(ConsoleCommand)} if you wish to modify console state."); 
            return null;
        }
        public abstract Task<CommandResult> ExecuteAsync(CommandArgs args);
    }
    public abstract class TerminalCommand : SimpleCommand, ITerminalApplication
    {
        public abstract void OnInputRecieved(string input);
        public abstract void ReceiveOutput(string message);
        public abstract void OnGUI(GUIContext context);
    }
    public abstract class WindowedCommand : SimpleCommand, IGraphical
    {
        // TODO: Need to register with window command
        
        public abstract void OnGUI(GUIContext context);
    }
    #endregion

    public abstract class CommandArgsBase
    {
        public List<string> Tokens;

    }
    public class CommandArgs : CommandArgsBase { }
    public class ConsoleCommandArgs : CommandArgsBase
    {
        public ConsoleState ConsoleState;
    }
    public class CommandResult
    {
        public string Message;

        public CommandResult()
        {
            Message = "";
        }

        public CommandResult(string message)
        {
            Message = message;
        }
    }
}