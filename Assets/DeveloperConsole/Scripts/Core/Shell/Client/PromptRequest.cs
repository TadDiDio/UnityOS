namespace DeveloperConsole.Core.Shell
{
    public class PromptRequest
    {
        public string Message;

        public PromptRequest(string message)
        {
            Message = message;
        }
    }

    public class ChoicePrompt : PromptRequest
    {
        public object[] Options;

        public ChoicePrompt(string message, object[] options) : base(message)
        {
            Options = options;
        }
    }

    public class ConfirmationPrompt : ChoicePrompt
    {
        public ConfirmationPrompt(string message) : base(message, null)
        {
            Options = new object[] { "y", "n" };
        }
    }
}
