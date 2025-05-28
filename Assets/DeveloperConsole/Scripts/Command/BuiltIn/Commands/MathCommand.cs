namespace DeveloperConsole
{
    public class MathCommand : Command
    {
        [PositionalArg(0)] public float OperandA;
        [PositionalArg(1)] public float OperandB;
        [SwitchArg("operation", 'o')] public string Operation = "add";
        
        protected override string Name() => "math";
        protected override string Description() => "Tests math operations.";

        protected override CommandResult Execute(CommandArgs args)
        {
            return Operation switch
            {
                "add" => new CommandResult((OperandA + OperandB).ToString()),
                "subtract" => new CommandResult((OperandA - OperandB).ToString()),
                "multiply" => new CommandResult((OperandA * OperandB).ToString()),
                "divide" when OperandB == 0 => new CommandResult("Cannot divide by zero."),
                "divide" => new CommandResult((OperandA / OperandB).ToString()),
                _ => new CommandResult("Invalid math operation.")
            };
        }
    }
}