using System.Threading.Tasks;

namespace DeveloperConsole
{
    [Command("repl", "A test repl command", true)]
    public class TestREPL : ReplCommand
    {
        protected override async Task<string> HandleInput(CommandContext context, string input)
        {
            var result = await context.Shell.RunInput(input);
            return context.Shell.GetOutputStringFromResult(result);
        }
    }
}