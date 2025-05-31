using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;

namespace DeveloperConsole.BuiltInCommands
{
    [Command("help", "Displays help for a command", false)]
    public class HelpCommand : SimpleCommand
    {
        [VariadicArgs] private List<string> CommandHierarchy; 
        
        [SwitchArg("verbose", 'v')]
        [Description("Provide a much richer help message")]
        private bool Verbose;
        
        protected override CommandResult Execute(CommandContext context)
        {
            if (CommandHierarchy.Count == 0) return new CommandResult(GetHelp(GetType()));
            
            string commandName = string.Join(".", CommandHierarchy);
            if (!ConsoleAPI.TryGetCommand(commandName, out ICommand command))
            {
                return new CommandResult($"Command '{commandName}' not found. Command names are type sensitive.");
            }
            
            return new CommandResult(GetHelp(command.GetType()));
        }

        private string GetHelp(Type commandType, [CanBeNull] string parentPath = null)
        {
            CommandAttribute attribute = commandType.GetCustomAttribute<CommandAttribute>();
            if (attribute == null) return MessageFormatter.Error($"{commandType.Name} does not have a CommandAttribute");

            string name = attribute.Name;
            string description = attribute.Description;

            var subcommands    = GetFieldAttributes<SubcommandAttribute>(commandType).ToList();
            var positionalArgs = GetFieldAttributes<PositionalArgAttribute>(commandType).ToList();
            var switchArgs     = GetFieldAttributes<SwitchArgAttribute>(commandType).ToList();
            var variadicArgs   = GetFieldAttributes<VariadicArgsAttribute>(commandType);

            var commandPath = parentPath == null ? name : $"{parentPath} {name}";

            var builder = new StringBuilder();

            // ── Usage line ─────────────────────────────
            builder.Append("usage: ");
            builder.Append(commandPath);

            if (subcommands.Count > 0)
            {
                builder.Append(" [");
                builder.Append(string.Join("|", subcommands.Select(s =>
                {
                    var subType = s.field.FieldType;
                    var subCmd = subType.GetCustomAttribute<CommandAttribute>()?.Name;
                    return subCmd;
                })));
                builder.Append("]");
            }

            foreach (var arg in positionalArgs)
            {
                builder.Append($" <{arg.field.Name}>");
            }

            foreach (var sw in switchArgs)
            {
                builder.Append($" [{sw.attr.Name} | {sw.attr.ShortName}]");
            }

            if (variadicArgs.Count > 0)
            {
                builder.Append($" [{variadicArgs[0].field.Name}...]");
            }

            builder.AppendLine();

            if (!Verbose) return $"{MessageFormatter.AddColor(name, MessageFormatter.Blue)}: " +
                               $"{description}{MessageFormatter.Bar}{builder}";
            
            // ── Description for this command ────────────
            var cmdDescription = commandType.GetCustomAttribute<DescriptionAttribute>()?.Description;
            if (!string.IsNullOrWhiteSpace(cmdDescription))
            {
                builder.AppendLine($"{Environment.NewLine}{cmdDescription}");
            }
            
            // ── Subcommands ─────────────────────────────
            if (subcommands.Count > 0)
            {
                builder.AppendLine($"{Environment.NewLine}{MessageFormatter.Bold("Subcommands:")}");
                foreach (var subCmd in subcommands)
                {
                    var subType = subCmd.field.FieldType;
                    var subCommandAttr = subType.GetCustomAttribute<CommandAttribute>();
                    if (subCommandAttr == null) continue;

                    var subName = subCommandAttr.Name;
                    var subDesc = subCommandAttr.Description;
                    builder.AppendLine($"  {subName,-12} {subDesc}");
                }
            }

            // ── Positional arguments ────────────────────
            if (positionalArgs.Count > 0)
            {
                builder.AppendLine($"{Environment.NewLine}{MessageFormatter.Bold("Positional Arguments (required):")}");
                foreach (var arg in positionalArgs)
                {
                    var desc = arg.field.GetCustomAttribute<DescriptionAttribute>()?.Description;
                    builder.AppendLine($"  {arg.field.Name,-12} {desc}");
                }
            }

            // ── Flags/switches ──────────────────────────
            if (switchArgs.Count > 0)
            {
                builder.AppendLine($"{Environment.NewLine}{MessageFormatter.Bold("Switches (optional):")}");
                foreach (var switchArg in switchArgs)
                {
                    var desc = switchArg.field.GetCustomAttribute<DescriptionAttribute>()?.Description;
                    builder.AppendLine($"  {switchArg.attr.Name,-11} {desc}");
                }
            }

            // ── Variadic arguments ──────────────────────
            if (variadicArgs.Count > 0)
            {
                var desc = variadicArgs[0].field.GetCustomAttribute<DescriptionAttribute>()?.Description;
                builder.AppendLine($"{Environment.NewLine}{MessageFormatter.Bold("Variadic Arguments (optional):")}");
                builder.AppendLine($"  {variadicArgs[0].field.Name,-12} {desc}");
            }

            // ── Recurse into subcommands ────────────────
            foreach (var subCmd in subcommands)
            {
                var subType = subCmd.field.FieldType;
                builder.AppendLine();
                builder.Append(GetHelp(subType, commandPath));
            }

            return $"{MessageFormatter.AddColor(name, MessageFormatter.Blue)}: {description}{MessageFormatter.Bar}{builder}";
        }
        private static List<(TAttr attr, FieldInfo field)> GetFieldAttributes<TAttr>(Type type) where TAttr : Attribute
        {
            return type
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Select(field => (attr: field.GetCustomAttribute<TAttr>(), field))
                .Where(t => t.attr != null)
                .ToList()!;
        }
    }
}