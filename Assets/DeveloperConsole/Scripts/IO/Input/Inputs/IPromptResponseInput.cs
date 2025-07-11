namespace DeveloperConsole.IO
{
    /// <summary>
    /// A string based input.
    /// </summary>
    public class PromptResponseInput : IInput
    {
        public string Response;
        
        public PromptResponseInput(string response)
        {
            Response = response;
        }
    }
}
