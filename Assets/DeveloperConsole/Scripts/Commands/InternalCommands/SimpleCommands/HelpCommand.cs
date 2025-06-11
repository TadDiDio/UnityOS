using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace DeveloperConsole
{
    [Command("help", "Gets a usage description of any other command", true)]
    public class HelpCommand : SimpleCommand
    {
        [Description("Fully qualified command name (use '.' or spaces to specify subcommands)")]
        [VariadicArgs] private List<string> CommandName; 
        
        [SwitchArg("verbose", 'v')]
        [Description("Provide a much richer help message")]
        private bool Verbose;
        
        [SwitchArg("hierarchical", 'h')]
        [Description("Show all levels of subcommands")]
        private bool Hierarchical;
        
        protected override CommandResult Execute(CommandContext context)
        {
            string message = $"{MessageFormatter.AddColor("help", MessageFormatter.Blue)} " +
                             $"is a command that can be used to look up the usage of any other command. " +
                             $"{Environment.NewLine}" +
                             $"Simply type 'help' followed by any other command to get information on it. " +
                             $"{Environment.NewLine}" +
                             $"You can use the {MessageFormatter.AddColor("reg", MessageFormatter.Blue)} " +
                             $"command to get a list of all available commands.{Environment.NewLine}" +
                             $"Finally, if the help displayed for a command is still unclear, consider " +
                             $"typing 'help -v <other_command_name>' to get a more complete help page.";
            
            if (CommandName.Count == 0) return new CommandResult($"{message}{Environment.NewLine}" +
                                                                 $"{Environment.NewLine}{GetHelp(GetType(), 0)}");
            
            string commandName = string.Join(".", CommandName);
            if (!ConsoleAPI.TryGetCommand(commandName, out ICommand command))
            {
                return new CommandResult($"Command '{commandName}' not found. Command names are type sensitive.");
            }
            
            return new CommandResult(GetHelp(command.GetType(), 0));
        }

        private string GetHelp(Type commandType, int depth, [CanBeNull] string parentPath = null)
        {
            string tab = depth == 0 ? "" : new string(' ', 2 * (depth - 1)) + "|" + "  ";
            
            CommandAttribute attribute = commandType.GetCustomAttribute<CommandAttribute>();
            if (attribute == null)
                return MessageFormatter.Error($"{commandType.Name} does not have a CommandAttribute");

            string name = attribute.Name;
            string description = attribute.Description;

            var subcommands = GetFieldAttributes<SubcommandAttribute>(commandType).ToList();
            var positionalArgs = GetFieldAttributes<PositionalArgAttribute>(commandType).ToList();
            var switchArgs = GetFieldAttributes<SwitchArgAttribute>(commandType).ToList();
            var variadicArgs = GetFieldAttributes<VariadicArgsAttribute>(commandType);

            var commandUsagePath = parentPath == null ? name : $"{parentPath} {name}";
            var commandNamePath  = parentPath == null ? name : $"{parentPath}.{name}";
            
            var regularUsageBuilder = new StringBuilder();

            // ── Usage line ─────────────────────────────
            regularUsageBuilder.Append($"{tab}{MessageFormatter.AddColor("usage:", MessageFormatter.Green)} ");
            regularUsageBuilder.Append($"{commandUsagePath}");

            foreach (var arg in positionalArgs)
            {
                regularUsageBuilder.Append($" <{arg.field.Name}>");
            }

            foreach (var sw in switchArgs)
            {
                regularUsageBuilder.Append($" [{sw.attr.Name} | {sw.attr.ShortName}]");
            }

            if (variadicArgs.Count > 0)
            {
                regularUsageBuilder.Append($" [{variadicArgs[0].field.Name}...]");
            }

            regularUsageBuilder.AppendLine();

            var subcommandUsageBuilder = new StringBuilder();
            if (subcommands.Count > 0)
            {
                subcommandUsageBuilder.AppendLine($"{tab}- or -");
                subcommandUsageBuilder.Append($"{tab}{MessageFormatter.AddColor("usage:", MessageFormatter.Green)} ");
                subcommandUsageBuilder.Append($"{commandUsagePath}");
                subcommandUsageBuilder.Append($" [");
                string subcommandsStr = string.Join(" | ", subcommands.Select(s =>
                {
                    var subType = s.field.FieldType;
                    var subCmd = subType.GetCustomAttribute<CommandAttribute>()?.Name;
                    return subCmd;
                }));
                subcommandUsageBuilder.Append($"{subcommandsStr}");
                subcommandUsageBuilder.AppendLine($"]");
            }

            if (!Verbose)
            {
                // ── Recurse into subcommands ────────────────
                if (Hierarchical)
                {
                    foreach (var subCmd in subcommands)
                    {
                        var subType = subCmd.field.FieldType;
                        subcommandUsageBuilder.AppendLine();
                        subcommandUsageBuilder.Append(GetHelp(subType, depth + 1, commandUsagePath));
                    }
                }
                
                return $"{tab}{MessageFormatter.AddColor(name, MessageFormatter.Blue)} {(parentPath == null ? "" : $"({commandNamePath})")}: " +
                    $"{description}{Environment.NewLine}{tab}{MessageFormatter.Bar}{Environment.NewLine}" +
                    $"{regularUsageBuilder}{subcommandUsageBuilder}";
            }

            // ── Description for this command ────────────
            var descriptionBuilder = new StringBuilder();
            var cmdDescription = commandType.GetCustomAttribute<DescriptionAttribute>()?.Description;
            if (!string.IsNullOrWhiteSpace(cmdDescription))
            {
                descriptionBuilder.AppendLine($"{Environment.NewLine}{tab}{cmdDescription}");
            }
            
            // ── Subcommands ─────────────────────────────
            if (subcommands.Count > 0)
            {
                int maxSubLength = subcommands
                    .Select(sc => sc.field.FieldType.GetCustomAttribute<CommandAttribute>()?.Name?.Length ?? 0)
                    .Max();

                descriptionBuilder.AppendLine($"{tab}{Environment.NewLine}{tab}{MessageFormatter.Bold("Subcommands:")}");
                foreach (var subCmd in subcommands)
                {
                    var subType = subCmd.field.FieldType;
                    var subCommandAttr = subType.GetCustomAttribute<CommandAttribute>();
                    if (subCommandAttr == null) continue;

                    var subName = subCommandAttr.Name;
                    var subDesc = subCommandAttr.Description;
                    descriptionBuilder.AppendLine($"{tab}  {PadWithAnsi(MessageFormatter.AddColor(subName, MessageFormatter.Blue), maxSubLength)}  {subDesc}");
                }
            }

            // ── Positional arguments ────────────────────
            if (positionalArgs.Count > 0)
            {
                int maxArgLength = positionalArgs.Select(pa => pa.field.Name.Length).Max();
                descriptionBuilder.AppendLine($"{tab}{Environment.NewLine}{tab}{MessageFormatter.Bold("Positional Arguments:")}");
                foreach (var arg in positionalArgs)
                {
                    var desc = arg.field.GetCustomAttribute<DescriptionAttribute>()?.Description;
                    descriptionBuilder.AppendLine($"{tab}  {arg.field.Name.PadRight(maxArgLength)}  {desc}");
                }
            }

            // ── Flags/switches ──────────────────────────
            if (switchArgs.Count > 0)
            {
                // Compute full label width: Name + possible " (required)"
                int maxLabelLength = switchArgs
                    .Select(sa => sa.attr.Name + (sa.field.GetCustomAttribute<RequiredArgAttribute>() != null ? " (required)" : ""))
                    .Select(label => label.Length)
                    .Max();

                descriptionBuilder.AppendLine($"{tab}{Environment.NewLine}{tab}{MessageFormatter.Bold("Switches:")}");
                foreach (var switchArg in switchArgs)
                {
                    var desc = switchArg.field.GetCustomAttribute<DescriptionAttribute>()?.Description;
                    var required = switchArg.field.GetCustomAttribute<RequiredArgAttribute>() != null;
                    var label = switchArg.attr.Name + (required ? " (required)" : "");
                    descriptionBuilder.AppendLine($"{tab}  {label.PadRight(maxLabelLength)}  {desc}");
                }
            }

            // ── Variadic arguments ──────────────────────
            if (variadicArgs.Count > 0)
            {
                var desc = variadicArgs[0].field.GetCustomAttribute<DescriptionAttribute>()?.Description;
                descriptionBuilder.AppendLine($"{tab}{Environment.NewLine}{tab}{MessageFormatter.Bold("Variadic Arguments:")}");
                descriptionBuilder.AppendLine($"{tab}  {variadicArgs[0].field.Name,-12} {desc}");
            }

            // ── Recurse into subcommands ────────────────
            if (Hierarchical)
            {
                foreach (var subCmd in subcommands)
                {
                    descriptionBuilder.AppendLine();
                    descriptionBuilder.AppendLine();
                    descriptionBuilder.Append(GetHelp(subCmd.field.FieldType, depth + 1, commandUsagePath));
                }
            }

            return $"{tab}{MessageFormatter.AddColor(name, MessageFormatter.Blue)} {(parentPath == null ? "" : $"({commandNamePath})")}: {description}" +
                   $"{Environment.NewLine}{tab}{MessageFormatter.Bar}{Environment.NewLine}" +
                   $"{regularUsageBuilder}{subcommandUsageBuilder}{descriptionBuilder}";
        }
        private static List<(TAttr attr, FieldInfo field)> GetFieldAttributes<TAttr>(Type type) where TAttr : Attribute
        {
            return type
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Select(field => (attr: field.GetCustomAttribute<TAttr>(), field))
                .Where(t => t.attr != null)
                .ToList()!;
        }
        
        private readonly Regex AnsiRegex = new(@"\x1B\[[0-9;]*m", RegexOptions.Compiled);
        private string PadWithAnsi(string text, int totalWidth)
        {
            var visibleLength = AnsiRegex.Replace(text, "").Length;
            var padding = Math.Max(0, totalWidth - visibleLength);
            return text + new string(' ', padding);
        }
    }
}