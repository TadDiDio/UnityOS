using DeveloperConsole.Core.Shell;

namespace DeveloperConsole.Command
{
    public interface ISessionContext { }

    public class RestrictedSessionContext : ISessionContext
    {
        public AliasManager AliasManager { get; }
        public RestrictedSessionContext(AliasManager alias) { AliasManager = alias; }
    }

    public class FullSessionContext : RestrictedSessionContext
    {
        public CommandSubmitter CommandSubmitter { get; }

        public FullSessionContext(CommandSubmitter submitter, AliasManager alias) : base(alias)
        {
            CommandSubmitter = submitter;
        }
    }
}
