namespace DeveloperConsole.Core.Shell.Prompting
{
    public class PromptResult<T>
    {
        public bool Success;
        public string ErrorMessage;
        public T Value;

        private PromptResult(string errorMessage)
        {
            Success = false;
            ErrorMessage = errorMessage;
            Value = default;
        }

        private PromptResult(T value)
        {
            Success = true;
            ErrorMessage = null;
            Value = value;
        }

        public static PromptResult<T> Successful(T value)
        {
            return new PromptResult<T>(value);
        }

        public static PromptResult<T> Failed(string errorMessage)
        {
            return new PromptResult<T>(errorMessage);
        }
    }
}
