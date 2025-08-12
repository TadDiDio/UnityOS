using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("help", "Gets information on any command.")]
    public class HelpCommand : SimpleCommand
    {
        [Variadic("The command to get help on.")]
        private List<string> command;

        [Switch('v', "Gets detailed information about the command.")]
        private bool verbose;

        private const int IndentAmount = 2;
        private List<ArgumentSpecification> _positionals;
        private List<ArgumentSpecification> _optionals;
        private List<ArgumentSpecification> _switches;
        private ArgumentSpecification _variadic;

        protected override CommandOutput Execute(CommandContext context)
        {
            if (command.Count == 0) return new CommandOutput(Describe());

            return new CommandOutput(GetHelpOnCommand(string.Join('.', command)));
        }

        private string Describe()
        {
            return "Type 'help <command name>' for information on any command. To see a list of commands, type 'reg'.";
        }

        private string GetHelpOnCommand(string fullyQualifiedName)
        {
            bool exists = ConsoleAPI.Commands.TryResolveCommandSchema(fullyQualifiedName, out var schema);
            if (!exists) return $"Could not find a command matching {fullyQualifiedName}";
            string name = fullyQualifiedName.Replace(".", " ");

            StringBuilder builder = new();

            builder.AppendLine();
            builder.AppendLine($"Command: {MessageFormatter.AddColor(name, MessageFormatter.Blue)}");
            builder.AppendLine();

            builder.AppendLine(schema.Description);
            builder.AppendLine();

            builder.AppendLine("Usage:");
            builder.AppendLine(MessageFormatter.IndentLines(GetCommandUsage(name, schema, true), IndentAmount));
            foreach (var subcommand in schema.Subcommands)
            {
                builder.AppendLine(MessageFormatter.IndentLines(GetCommandUsage($"{name} {subcommand.Name}", subcommand, false), IndentAmount));
            }

            if (verbose)
            {
                builder.Append(VerboseDescription());
            }

            return builder.ToString();
        }

        private string GetCommandUsage(string fqn, CommandSchema schema, bool baseCommand)
        {
            StringBuilder builder = new();

            builder.Append(MessageFormatter.AddColor(fqn, MessageFormatter.Blue));

            var positionals = schema.ArgumentSpecifications
                .Where(spec => spec.Attributes.OfType<PositionalAttribute>().Any())
                .OrderBy(spec => spec.Attributes.OfType<PositionalAttribute>().First().Index)
                .ToList();

            if (baseCommand) _positionals = positionals;

            foreach (var positional in positionals)
            {
                string line = $" <{positional.Name}>";
                builder.Append(line);
            }

            var optionals = schema.ArgumentSpecifications
                .Where(spec => spec.Attributes.OfType<OptionalAttribute>().Any())
                .OrderBy(spec => spec.Attributes.OfType<OptionalAttribute>().First().Index)
                .ToList();

            if (baseCommand) _optionals = optionals;

            foreach (var optional in optionals)
            {
                string line = $" [{optional.Name}]";
                builder.Append(line);
            }

            var switches = schema.ArgumentSpecifications
                .Where(spec => spec.Attributes.OfType<SwitchAttribute>().Any())
                .OrderBy(spec => spec.Attributes.OfType<SwitchAttribute>().First().Name)
                .ToList();

            if (baseCommand) _switches = switches;

            foreach (var s in switches)
            {
                var switchInfo = s.FieldInfo.FieldType != typeof(bool) ? $"--{s.Name} <value>" : $"--{s.Name}";
                string line = $" [{switchInfo}]";
                builder.Append(line);
            }

            var variadic = schema.ArgumentSpecifications.FirstOrDefault(spec => spec.Attributes.OfType<VariadicAttribute>().Any());

            if (baseCommand) _variadic = variadic;

            if (variadic != null)
            {
                string line = $" [{variadic.Name}...]";
                builder.Append(line);
            }

            return builder.ToString();
        }

        private string VerboseDescription()
        {
            StringBuilder builder = new();

            int argIndex = 0;
            if (_positionals.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("Positional Arguments (required):");

                List<(string, string)> positionals = new();
                foreach (var positional in _positionals)
                {
                    positionals.Add(($"({argIndex++}) <{positional.Name} ({TypeFriendlyNames.TypeToName(positional.FieldInfo.FieldType)})>:", $"{positional.Description}"));
                }

                builder.AppendLine(MessageFormatter.IndentLines(MessageFormatter.PadLeft(positionals), IndentAmount));
            }

            if (_optionals.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("Optional Arguments (optional):");
                List<(string, string)> optionals = new();
                foreach (var optional in _optionals)
                {
                    optionals.Add(($"({argIndex++}) [{optional.Name} ({TypeFriendlyNames.TypeToName(optional.FieldInfo.FieldType)})]:", $"{optional.Description}"));
                }

                builder.AppendLine(MessageFormatter.IndentLines(MessageFormatter.PadLeft(optionals), IndentAmount));
            }

            if (_switches.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("Switch Arguments (optional):");
                List<(string, string)> switches = new();
                foreach (var s in _switches)
                {
                    string value = s.FieldInfo.FieldType == typeof(bool) ? "" : " <value>";
                    switches.Add(($"[--{s.Name} or -{s.Attributes.OfType<SwitchAttribute>().FirstOrDefault()!.Alias}{value} ({TypeFriendlyNames.TypeToName(s.FieldInfo.FieldType)})]:", $"{s.Description}"));
                }

                builder.AppendLine(MessageFormatter.IndentLines(MessageFormatter.PadLeft(switches), IndentAmount));
            }

            if (_variadic != null)
            {
                builder.AppendLine();
                builder.AppendLine("Variadic Arguments (optional):");

                string type = $"0 or more {TypeFriendlyNames.TypeToName(_variadic.FieldInfo.FieldType.GetGenericArguments()[0])}s";
                builder.AppendLine(MessageFormatter.IndentLines($"[{_variadic.Name} ({type})]: {_variadic.Description}", IndentAmount));
            }

            return builder.ToString();
        }
    }
}
