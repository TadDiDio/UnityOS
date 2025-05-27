using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DeveloperConsole
{
    public interface ICommand
    {
        public string GetName();
        public string GetDescription();
        public Task<CommandResult> ExecuteAsync(CommandArgsBase args);
    }

    #region COMMAND BASES
    public abstract class CommandBase : ICommand
    {
        protected abstract string Name();
        protected abstract string Description();

        public string GetName()
        {
            string name = Name();
            if (string.IsNullOrEmpty(name))
            {
                Debug.Log($"Name of {GetType()} was null or empty. This is not allowed.");
                return "";
            }

            name = name.Trim().ToLower();
            return Regex.Replace(name, @"[^a-zA-Z]+", "");
        }
        
        public string GetDescription()
        {
            string description = Description();
            if (string.IsNullOrEmpty(description))
            {
                Debug.Log($"Description of {GetType()} was null or empty. This is not allowed.");
                return "";
            }

            description = description.Trim();
            description = char.ToUpper(description[0]) + description.Substring(1);
            
            char last = description[^1];
            if (last != '.' && last != '!' && last != '?')
            {
                description += ".";
            }

            return description;
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
                           $"{typeof(Command)} if you don't need to modify console state."); 
            return null;
        }
        protected abstract CommandResult Execute(ConsoleCommandArgs args);
    }
    public abstract class Command : CommandBase
    {
        public override async Task<CommandResult> ExecuteAsync(CommandArgsBase args)
        {
            if (args is CommandArgs commandArgs) return await Task.FromResult(Execute(commandArgs));
            
            Debug.LogError($"Invalid casting from type {args.GetType()} to CommandArgs. Ensure " +
                           $"that this command inherits from {typeof(Command)} or use " +
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
                           $"that this command inherits from {typeof(Command)} or use " +
                           $"{typeof(ConsoleCommand)} if you wish to modify console state."); 
            return null;
        }
        public abstract Task<CommandResult> ExecuteAsync(CommandArgs args);
    }
    public abstract class TerminalCommand : Command, ITerminalApplication
    {
        public abstract void OnInput(string input);
        public abstract void ReceiveOutput(string message);
        public abstract void OnGUI(GUIContext context);
    }
    public abstract class WindowedCommand : Command, IGraphical
    {
        protected WindowedCommand()
        {
            GraphicsManager.Register(this);
        }

        ~WindowedCommand()
        {
            GraphicsManager.Unregister(this);
        }
        
        public abstract void OnGUI(GUIContext context);
    }
    #endregion
    public abstract class CommandArgsBase { }
    public class CommandArgs : CommandArgsBase
    {
        public List<string> Tokens;
    }
    public class ConsoleCommandArgs : CommandArgsBase
    {
        public List<string> Tokens;
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