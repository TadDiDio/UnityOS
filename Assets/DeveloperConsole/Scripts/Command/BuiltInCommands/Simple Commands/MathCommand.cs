using System.Collections.Generic;
using UnityEngine;

namespace DeveloperConsole
{
    public enum MathCommandOperation
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }
    [Command("math", "Performs math operations.")]
    public class MathCommand : SimpleCommand
    {
        [InRange(-5, 5)]
        [PositionalArg(0)]
        private float operandA;
        
        [PositionalArg(1)]
        private float operandB;
        
        [RequiredArg]
        [SwitchArg("operation", 'o')] 
        private MathCommandOperation operation;
        
        [SwitchArg("message", 'm')] 
        private string Message;

        [SwitchArg("flag", 'f')] 
        private bool Flag;
        
        [VariadicArgs]
        private List<Vector2> variadicArgs;
        
        public override void RegisterTypeParsers()
        {
            ConsoleAPI.Parser.RegisterTypeParser<MathCommandOperation>(new EnumParser<MathCommandOperation>());
        }

        protected override CommandResult Execute(CommandArgs args)
        {
            string result = operation switch
            {
                MathCommandOperation.Add => (operandA + operandB).ToString(),
                MathCommandOperation.Subtract => (operandA - operandB).ToString(),
                MathCommandOperation.Multiply => (operandA * operandB).ToString(),
                MathCommandOperation.Divide when operandB == 0 => "Cannot divide by zero.",
                MathCommandOperation.Divide => (operandA / operandB).ToString(),
                _ => $"Invalid math operation: {operation}"
            };

            string variadic = string.Join(" ", variadicArgs);
            
            return new CommandResult($"{result}, flag is {Flag}, message is {Message}, {variadic}");
        }
    }
}