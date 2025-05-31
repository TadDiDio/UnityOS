using System.Collections.Generic;

namespace DeveloperConsole
{
    public enum MathCommandOperation
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }
    [Command("math", "Performs math operations.", false)]
    public class MathCommand : SimpleCommand
    {
        [InRange(-5, 5)]
        [PositionalArg(0)]
        [Description("The first operand")]
        private float operandA;
        
        [PositionalArg(1)]
        [Description("The second operand")]
        private float operandB;
        
        [RequiredArg]
        [Description("The operation to perform")]
        [SwitchArg("operation", 'o')] 
        private MathCommandOperation operation;
        
        [SwitchArg("message", 'm')] 
        [Description("A message to log")]
        private string Message;

        [SwitchArg("flag", 'f')] 
        [Description("A test flag")]
        private bool Flag;
        
        [VariadicArgs]
        [Description("A list of words to say at the end")]
        private List<string> variadicArgs;
        
        [Subcommand]
        private HelloCommand helloCommand;
        
        public override void RegisterTypeParsers()
        {
            ConsoleAPI.RegisterTypeParser<MathCommandOperation>(new EnumParser<MathCommandOperation>());
        }

        protected override CommandResult Execute(CommandContext context)
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
    
    [Command("hello", "Says hello to the world.", true)]
    public class HelloCommand : SimpleCommand
    {
        [VariadicArgs] private List<string> message;
        protected override CommandResult Execute(CommandContext context)
        {
            return new CommandResult("Hello, world!\n" + string.Join(" ", message));
        }
    }
}