namespace DeveloperConsole.Command
{
    public class FrontEndCommandRequest
    {
        public ICommandResolver Resolver;
        public CommandCondition Condition;
        public bool Windowed;
    }
}
