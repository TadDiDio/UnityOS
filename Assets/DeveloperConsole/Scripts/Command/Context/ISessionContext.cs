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
        public GraphProcessor GraphProcessor { get; }

        public FullSessionContext(GraphProcessor submitter, AliasManager alias) : base(alias)
        {
            GraphProcessor = submitter;
        }
    }
}
