namespace DeveloperConsole.Command
{
    /// <summary>
    /// The context that the command executes in.
    /// </summary>
    public interface ICommandContext
    {
        /// <summary>
        /// The write permissions this command has.
        /// </summary>
        public IWriteContext Writing { get; }

        /// <summary>
        /// The prompt permissions this command has.
        /// </summary>
        public IPromptContext Prompting { get; }

        /// <summary>
        /// The signaling permissions this command has.
        /// </summary>
        public ISignalContext Signaling { get; }

        /// <summary>
        /// The session permissions this command has.
        /// </summary>
        public ISessionContext Session { get; }

        /// <summary>
        /// The current environment.
        /// </summary>
        public UnityEnvironment Environment { get; }
    }


    /**
     * The theory behind these contexts is to only provide a context option
     * when a service is limited, not when its available. When available, we will
     * prefer (only allow) access through base class command methods which act as
     * helpers to reduce confusion and keep the context object light weight.
     */

    public class FullCommandContext : ICommandContext
    {
        public WriteContext Writing { get; }
        public PromptContext Prompting { get; }
        public SignalContext Signaling { get; }
        public FullSessionContext Session { get; }
        public UnityEnvironment Environment { get; }
        IWriteContext ICommandContext.Writing => Writing;
        IPromptContext ICommandContext.Prompting => Prompting;
        ISignalContext ICommandContext.Signaling => Signaling;
        ISessionContext ICommandContext.Session => Session;

        public FullCommandContext
        (
            WriteContext write,
            PromptContext prompt,
            SignalContext signal,
            FullSessionContext session,
            UnityEnvironment env
        )
        {
            Writing = write;
            Prompting = prompt;
            Signaling = signal;
            Session = session;
            Environment = env;
        }
    }

    public class AsyncCommandContext : ICommandContext
    {
        public UnityEnvironment Environment { get; }

        IWriteContext ICommandContext.Writing => null;
        IPromptContext ICommandContext.Prompting => null;
        ISignalContext ICommandContext.Signaling => null;
        ISessionContext ICommandContext.Session => null;

        public AsyncCommandContext
        (
            UnityEnvironment env
        )
        {
            Environment = env;
        }
    }

    public class SimpleContext : ICommandContext
    {
        public RestrictedSessionContext Session { get; }
        public UnityEnvironment Environment { get; }

        ISessionContext ICommandContext.Session => Session;
        IWriteContext ICommandContext.Writing => null;
        IPromptContext ICommandContext.Prompting => null;
        ISignalContext ICommandContext.Signaling => null;

        public SimpleContext
        (
            RestrictedSessionContext session,
            UnityEnvironment env
        )
        {
            Session = session;
            Environment = env;
        }
    }
}
