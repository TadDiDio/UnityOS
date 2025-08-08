using System;
using UnityEngine;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Rules;
using DeveloperConsole.Parsing.TypeAdapting;
using DeveloperConsole.Parsing.TypeAdapting.Types;

namespace DeveloperConsole
{
    public class DefaultParseRuleInstaller : IConsoleInstaller
    {
        public void Install()
        {
            ConsoleAPI.Parsing.AddRule(typeof(InvertedBoolLongSwitchParseRule));
            ConsoleAPI.Parsing.AddRule(typeof(InvertedBoolShortSwitchParseRule));
            ConsoleAPI.Parsing.AddRule(typeof(BoolLongSwitchParseRule));
            ConsoleAPI.Parsing.AddRule(typeof(BoolShortSwitchParseRule));
            ConsoleAPI.Parsing.AddRule(typeof(GroupedShortBoolSwitchRule));
            ConsoleAPI.Parsing.AddRule(typeof(LongSwitchParseRule));
            ConsoleAPI.Parsing.AddRule(typeof(ShortSwitchParseRule));
            ConsoleAPI.Parsing.AddRule(typeof(PositionalParseRule));
            ConsoleAPI.Parsing.AddRule(typeof(OptionalParseRule));
            ConsoleAPI.Parsing.AddRule(typeof(VariadicParseRule));
        }
    }

    public class DefaultTypeAdapterInstaller : IConsoleInstaller
    {
        public void Install()
        {
            ConsoleAPI.Parsing.RegisterTypeParser<int>(new IntAdapter());
            ConsoleAPI.Parsing.RegisterTypeParser<bool>(new BoolAdapter());
            ConsoleAPI.Parsing.RegisterTypeParser<float>(new FloatAdapter());
            ConsoleAPI.Parsing.RegisterTypeParser<Color>(new ColorAdapter());
            ConsoleAPI.Parsing.RegisterTypeParser<Color>(new AlphaColorAdapter());
            ConsoleAPI.Parsing.RegisterTypeParser<string>(new StringAdapter());
            ConsoleAPI.Parsing.RegisterTypeParser<Type>(new TypeAdapter());
            ConsoleAPI.Parsing.RegisterTypeParser<Vector2>(new Vector2Adapter());
            ConsoleAPI.Parsing.RegisterTypeParser<Vector3>(new Vector3Adapter());
            ConsoleAPI.Parsing.RegisterTypeParser<ConfirmationResult>(new ConfirmationResultAdapter());
            ConsoleAPI.Parsing.RegisterTypeParser<ICommandResolver>(new TextCommandResolverAdapter());
            ConsoleAPI.Parsing.RegisterTypeParser<CommandBatch>(new CommandBatchTypeAdapter());
        }
    }

    public class DefaultTerminalInstaller : IConsoleInstaller
    {
        public void Install()
        {
            var terminal = new TerminalClient();
            ConsoleAPI.Shell.CreateShellSession(terminal);
            ConsoleAPI.Windowing.RegisterWindow(terminal);
        }
    }
}
