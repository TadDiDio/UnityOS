using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;
using DeveloperConsole.Core.Shell;

namespace DeveloperConsole.Scripts.Command
{
    [Command("prompt", "Tests the prompt mechanics.")]
    public class TestPromptCommand : PromptCommand
    {
        protected override async Task<CommandOutput> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            if (!await ConfirmAsync("Do you want to proceed?", cancellationToken))
                return new CommandOutput("Execution canceled.");

            int number = await PromptAsync<int>("Enter your favorite integer", cancellationToken);

            WriteLine($"You picked {number}");

            await PromptWithChoicesAsync<string>("Pick your favorite animal", new[]
            {
                new PromptChoice("Dog", "Dog"),
                new PromptChoice("Cat", "Cat"),
                new PromptChoice("Fish", "Fiss"),
            }, cancellationToken);

            WriteLine("Doing a random countdown.");

            int i = 3;
            while (i > 0)
            {
                WriteLine(i--);
                await Task.Delay(1000, cancellationToken);
            }

            return new CommandOutput("Success!");
        }
    }
}
