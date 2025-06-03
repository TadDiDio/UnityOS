using System.Threading.Tasks;

namespace DeveloperConsole
{
    [Command("repl", "A test repl command", true)]
    public class TestREPL : REPLCommand
    {
        protected override async Task<string> OnEvaluate(CommandContext context, string input)
        {
            var result = await context.Shell.RunInput(input);
            return context.Shell.GetOutputStringFromResult(result);
        }

        public override string GetUserPrompt()
        {
            return "repl >";
        }
    }
}