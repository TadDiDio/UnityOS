using System.Linq;
using System.Text.RegularExpressions;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public class CommandBatchTypeAdapter : TypeAdapter<CommandBatch>
    {
        protected override bool TryConsumeAndConvert(TokenStream stream, out CommandBatch result)
        {
            string text = string.Join(" ", stream.Read(stream.Remaining().Count()));

            // Split on ';' and '&&'
            var regex = new Regex(@"\s*(&&|;)\s*");
            var matches = regex.Split(text).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            var batch = new CommandBatch();

            string lastSep = null;

            foreach (var command in matches)
            {
                if (command is "&&" or ";")
                {
                    lastSep = command;
                }
                else
                {
                    bool windowed = command.StartsWith("!");

                    if (windowed)
                    {

                    }

                    batch.Requests.Add(new CommandRequest
                    {
                        Resolver = new TextCommandResolver(windowed ? command[1..] : command),
                        Condition = lastSep is "&&" ? CommandCondition.OnPreviousSuccess : CommandCondition.Always,
                        Windowed = windowed
                    });
                    lastSep = null;
                }
            }

            result = batch;

            return true;
        }
    }
}
