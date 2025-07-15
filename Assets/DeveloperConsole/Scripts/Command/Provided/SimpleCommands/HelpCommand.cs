using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("help", "Gets information on any command.")]
    public class HelpCommand : SimpleCommand
    {
        [Optional(0, "The command to get help on.")]
        private string commandName = "!!!";

        [Variadic("Nested commands under the main command.")]
        private List<string> subcommands;

        [Switch('v', "Gets detailed information about the command.")]
        private bool verbose;

        private List<ArgumentSpecification> _positionals;
        private List<ArgumentSpecification> _optionals;
        private List<ArgumentSpecification> _switches;
        private ArgumentSpecification _variadic;

        protected override CommandOutput Execute(CommandContext context)
        {
            if (commandName == "!!!") return new CommandOutput(Describe());

            string fullCommand = subcommands == null ? commandName : $"{commandName}.{string.Join('.', subcommands)}";

            return new CommandOutput(GetHelpOnCommand(fullCommand));
        }

        private string Describe()
        {
            return "This will be more helpful later!";
        }

        private string GetHelpOnCommand(string fullyQualifiedName)
        {
            bool exists = ConsoleAPI.Commands.TryResolveCommandSchema(fullyQualifiedName, out var schema);
            if (!exists) return $"Could not find a command matching {fullyQualifiedName}";

            StringBuilder builder = new();

            // General information
            string name = fullyQualifiedName.Replace(".", " ");
            builder.AppendLine($"Name: {MessageFormatter.AddColor(name, MessageFormatter.Blue)}");
            builder.AppendLine($"Description: {schema.Description}");
            builder.AppendLine(MessageFormatter.Bar);

            // Usages
            builder.AppendLine($"Usage: {GetCommandUsage(name, schema)}");

            if (verbose) builder.AppendLine(VerboseDescription());

            return builder.ToString();
        }

        private string GetCommandUsage(string fqn, CommandSchema schema)
        {
            StringBuilder builder = new();

            builder.Append(MessageFormatter.AddColor(fqn, MessageFormatter.Blue));

            _positionals = schema.ArgumentSpecifications
                .Where(spec => spec.Attributes.OfType<PositionalAttribute>().Any())
                .OrderBy(spec => spec.Attributes.OfType<PositionalAttribute>().First().Index)
                .ToList();

            foreach (var positional in _positionals)
            {
                builder.Append($" <{positional.Name}: {TypeFriendlyNames.TypeToName(positional.FieldInfo.FieldType)}>");
            }

            _optionals = schema.ArgumentSpecifications
                .Where(spec => spec.Attributes.OfType<OptionalAttribute>().Any())
                .OrderBy(spec => spec.Attributes.OfType<OptionalAttribute>().First().Index)
                .ToList();

            foreach (var optional in _optionals)
            {
                builder.Append($" [{optional.Name}: {TypeFriendlyNames.TypeToName(optional.FieldInfo.FieldType)}]");
            }

            _switches = schema.ArgumentSpecifications
                .Where(spec => spec.Attributes.OfType<SwitchAttribute>().Any())
                .OrderBy(spec => spec.Attributes.OfType<SwitchAttribute>().First().Name)
                .ToList();

            foreach (var s in _switches)
            {
                builder.Append($" [--{s.Name}: {TypeFriendlyNames.TypeToName(s.FieldInfo.FieldType)}]");
            }

            _variadic = schema.ArgumentSpecifications.FirstOrDefault(spec => spec.Attributes.OfType<VariadicAttribute>().Any());
            if (_variadic != null)
            {
                var type = _variadic.FieldInfo.FieldType;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    type = type.GetGenericArguments()[0];
                }

                builder.Append($" [{_variadic.Name}: List of {TypeFriendlyNames.TypeToName(type)}s]");
            }

            return builder.ToString();
        }

        private string VerboseDescription()
        {
            StringBuilder builder = new();

            if (_positionals.Count > 0)
            {
                builder.AppendLine("Positional Arguments (required).");
                List<string> positionals = new();
                foreach (var positional in _positionals)
                {
                    positionals.Add($"{positional.Name}({TypeFriendlyNames.TypeToName(positional.FieldInfo.FieldType)}): {positional.Description}");
                }

                builder.AppendLine(MessageFormatter.PadFirstWordRight(positionals));
            }

            if (_optionals.Count > 0)
            {
                builder.AppendLine("Optional Positional Arguments (optional).");
                List<string> optionals = new();
                foreach (var optional in _optionals)
                {
                    optionals.Add($"{optional.Name}({TypeFriendlyNames.TypeToName(optional.FieldInfo.FieldType)}): {optional.Description}");
                }

                builder.AppendLine(MessageFormatter.PadFirstWordRight(optionals));
            }

            if (_switches.Count > 0)
            {
                builder.AppendLine("Switch Arguments (optional).");
                List<string> switches = new();
                foreach (var s in _switches)
                {
                    switches.Add($"--{s.Name}(-{s.Attributes.OfType<SwitchAttribute>().FirstOrDefault()!.Alias})({TypeFriendlyNames.TypeToName(s.FieldInfo.FieldType)}): {s.Description}");
                }

                builder.AppendLine(MessageFormatter.PadFirstWordRight(switches));
            }

            if (_variadic != null)
            {
                builder.AppendLine("Variadic Arguments (provide as many as you want) (optional).");

                string type = $"list of {_variadic.FieldInfo.FieldType.GetGenericArguments()[0]}";
                builder.AppendLine($"{_variadic.Name}({type}): {_variadic.Description}");
            }

            return builder.ToString();
        }
    }
}
