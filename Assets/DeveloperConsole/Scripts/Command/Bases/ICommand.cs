using System.Threading.Tasks;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Command interface.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Defines the asynchronous execution logic for this command.
        /// </summary>
        /// <param name="context">The context of this execution.</param>
        /// <returns>The command's output.</returns>
        public Task<CommandOutput> ExecuteAsync(CommandContext context);
        
        
        /// <summary>
        /// Registers any type parsers the command needs.
        /// </summary>
        public void RegisterTypeParsers();
    }
    

    /// <summary>
    /// Base class for all commands.
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        public virtual void RegisterTypeParsers() { }
        public abstract Task<CommandOutput> ExecuteAsync(CommandContext context);
    }
}