using System.Threading.Tasks;
using UnityEngine;

namespace DeveloperConsole
{
    public class ConsoleApplication : ShellApplication, IWindow
    {
        private ConsoleInput _consoleInput = new();
        private ConsoleOutput _consoleOutput;
        private ConsoleState _consoleState;
        
        private Vector2 _scrollPosition = Vector2.zero;
        
        public ConsoleApplication(ITokenizationManager tokenizationManager, IConsoleParser parser) 
            : base(tokenizationManager, parser)
        {
            // TODO: Move this or inject it or a factory.
            _consoleState = JsonFileManager.Load();
            _consoleOutput = new(_consoleState.OutputBuffer);
            
            InputManager.RegisterInputSource(_consoleInput);
            OutputManager.RegisterOutputSink(_consoleOutput);
        }
        
        public void OnGUI(Rect areaRect)
        {
            GUILayout.BeginArea(areaRect);
            
            _consoleOutput.OnGUI(ref _scrollPosition);
            _consoleInput.OnGUI();
            
            GUILayout.EndArea();
            
            InputManager.OnEventOccured(Event.current);
        }

        protected override void OnBeforeInputProcessed(string rawInput)
        {
            _scrollPosition.y = float.MaxValue;
        }

        protected override async Task OnInput(string rawInput, ConsoleState consoleState = null)
        {
            await base.OnInput(rawInput, _consoleState);
        }

        protected override void OnAfterInputProcessed(CommandResult result)
        {
            // TODO: Update to support file per console
            JsonFileManager.Save(_consoleState);
        }
    }
}