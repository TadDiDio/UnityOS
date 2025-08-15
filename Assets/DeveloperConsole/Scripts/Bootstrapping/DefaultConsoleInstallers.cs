using System;
using UnityEngine;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Rules;
using DeveloperConsole.Parsing.TypeAdapting;
using DeveloperConsole.Parsing.TypeAdapting.Types;
using DeveloperConsole.Windowing;

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
            ConsoleAPI.Parsing.RegisterTypeAdapter<int>(new IntAdapter());
            ConsoleAPI.Parsing.RegisterTypeAdapter<bool>(new BoolAdapter());
            ConsoleAPI.Parsing.RegisterTypeAdapter<float>(new FloatAdapter());
            ConsoleAPI.Parsing.RegisterTypeAdapter<Color>(new ColorAdapter());
            ConsoleAPI.Parsing.RegisterTypeAdapter<Color>(new AlphaColorAdapter());
            ConsoleAPI.Parsing.RegisterTypeAdapter<string>(new StringAdapter());
            ConsoleAPI.Parsing.RegisterTypeAdapter<Type>(new TypeAdapter());
            ConsoleAPI.Parsing.RegisterTypeAdapter<Vector2>(new Vector2Adapter());
            ConsoleAPI.Parsing.RegisterTypeAdapter<Vector3>(new Vector3Adapter());
            ConsoleAPI.Parsing.RegisterTypeAdapter<ConfirmationResult>(new ConfirmationResultAdapter());
            ConsoleAPI.Parsing.RegisterTypeAdapter<ICommandResolver>(new TokenCommandResolverAdapter());
        }
    }

    public class DefaultTerminalInstaller : IConsoleInstaller
    {
        public void Install()
        {

            var config = new WindowConfigFactory()
                .Resizeable()
                .WithMinSize(400, 300)
                .WithHeaderHeight(30)
                .FullScreen()
                .WithPadding(12)
                .WithName("Terminal")
                .Build();
            var terminal = new TerminalClient(config);
            ConsoleAPI.Shell.CreateShellSession(terminal.GetIOContext());
            ConsoleAPI.Windowing.RegisterWindow(terminal);
        }
    }
}
