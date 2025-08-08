namespace DeveloperConsole.Command
{
    public class CommandRequest
    {
        public ICommandResolver Resolver;
        public CommandCondition Condition;
        public bool Windowed;
    }
}
