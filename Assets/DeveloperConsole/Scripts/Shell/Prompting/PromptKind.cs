namespace DeveloperConsole.Core.Shell
{
    public enum PromptKind
    {
        Command,
        General,
        Choice,
        Confirmation
    }

    public static class PromptMetaKeys
    {
        public const string Choices = "choices";
    }

    public class PromptChoice
    {
        public string Label;
        public object Value;

        public PromptChoice(string label, object value)
        {
            Label = label;
            Value = value;
        }
    }
}
