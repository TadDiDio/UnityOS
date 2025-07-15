using System.Threading.Tasks;
using DeveloperConsole.Command;
using DeveloperConsole.Core.Shell;

namespace DeveloperConsole.Scripts.Command
{
    [Command("prompt", "Tests the prompt mechanics.")]
    public class TestPromptCommand : PromptCommand
    {
        private enum Animal
        {
            Dog,
            Cat,
            Fish
        }
        protected override async Task<CommandOutput> Execute(CommandContext context)
        {
            if (!await ConfirmAsync("Do you want to proceed?")) return new("Execution cancelled.");

            int number = await PromptAsync<int>("Enter your favorite integer");

            context.Session.WriteLine($"You picked {number}");

            Animal animal = await PromptWithChoicesAsync<Animal>("Pick your favorite animal", new[]
            {
                new PromptChoice("Dog", Animal.Dog),
                new PromptChoice("Cat", Animal.Cat),
                new PromptChoice("Fish", Animal.Fish),
            });

            context.Session.WriteLine("Doing a random countdown.");

            int i = 3;
            while (i > 0)
            {
                context.Session.WriteLine(i--);
                await Task.Delay(1000);
            }

            return new CommandOutput("Success!");
        }
    }
}
