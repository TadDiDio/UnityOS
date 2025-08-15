using System;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Simply a more ergonomic name for an async command.
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        public CommandSchema Schema { get; private set; }

        private FullCommandContext _context;

        protected CommandBase() { }

        public void Initialize(CommandSchema schema)
        {
            if (Schema != null) throw new InvalidOperationException("Schema already initialized.");
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
        }

        /// <summary>
        /// Executes this command.
        /// </summary>
        /// <param name="context">The unrestricted context for the command.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the command.</param>
        /// <returns>The command's output.</returns>
        public async Task<CommandOutput> ExecuteCommandAsync(FullCommandContext context, CancellationToken cancellationToken)
        {
            _context = context;
            return await ExecuteAsync(context, cancellationToken);
        }

        /// <summary>
        /// Executes this command.
        /// </summary>
        /// <param name="fullContext">The unrestricted context for the command.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the command.</param>
        /// <returns>The command's output.</returns>
        protected abstract Task<CommandOutput> ExecuteAsync(FullCommandContext fullContext, CancellationToken cancellationToken);


        /*
         * --------------------------------------
         *
         * Define helpers common to all commands
         *
         * --------------------------------------
         */


        /// <summary>
        /// Writes to the current output line.
        /// </summary>
        /// <param name="message">The message to add.</param>
        /// <param name="overwrite">True to overwrite the current output line.</param>
        protected void Write(object message, bool overwrite = false)
        {
            _context.Writing.Write(message.ToString(), overwrite);
        }

        /// <summary>
        /// Writes a line to the output channel.
        /// </summary>
        /// <param name="line">The line to write.</param>
        protected void WriteLine(object line)
        {
            _context.Writing.WriteLine(line.ToString());
        }

        /// <summary>
        /// Emits a signal.
        /// </summary>
        /// <param name="signal">The signal.</param>
        protected void Signal(ShellSignal signal)
        {
            _context.Signaling.Emitter.Signal(signal);
        }

        public virtual void Dispose() { /* No-op */ }
    }
}
