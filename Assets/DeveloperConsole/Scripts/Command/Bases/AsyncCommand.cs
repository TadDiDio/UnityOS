using System;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Simply a more ergonomic name for an async command.
    /// </summary>
    public abstract class AsyncCommand : ICommand
    {
        public Guid CommandId { get; } = Guid.NewGuid();
        public CommandSchema Schema { get; private set; }

        protected ShellSession Session;

        protected AsyncCommand()
        {
            // Delete constructor to force use of creation factory
        }

        public void Initialize(CommandSchema schema)
        {
            if (Schema != null) throw new InvalidOperationException("Schema already initialized.");

            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
        }

        public async Task<CommandOutput> ExecuteCommandAsync(CommandContext context, CancellationToken cancellationToken)
        {
            Session = context.Session;
            return await ExecuteAsync(context, cancellationToken);
        }

        protected abstract Task<CommandOutput> ExecuteAsync(CommandContext context, CancellationToken cancellationToken);

        /// <summary>
        /// Writes a message to the output channel.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void Write(object message)
        {
            Session.Write(CommandId, message);
        }

        /// <summary>
        /// Overwrites the current output line on the output channel.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void OverWrite(object message)
        {
            Session.OverWrite(CommandId, message);
        }

        /// <summary>
        /// Writes a line to the output channel.
        /// </summary>
        /// <param name="line">The line.</param>
        protected void WriteLine(object line)
        {
            Session.WriteLine(CommandId, line);
        }


        public virtual void Dispose()
        {
            // No-op
        }
    }
}
