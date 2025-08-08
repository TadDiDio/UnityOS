
namespace DeveloperConsole.Command
{
    public enum CommandCondition
    {
        Always,
        OnPreviousSuccess
    }

    public static class CommandConditionExtensions
    {
        public static bool AllowsStatus(this CommandCondition condition, Status status)
        {
            return condition switch
            {
                CommandCondition.Always => true,
                CommandCondition.OnPreviousSuccess => status is Status.Success,
                _ => false
            };
        }
    }
}
