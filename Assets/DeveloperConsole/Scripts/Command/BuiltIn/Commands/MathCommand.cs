namespace DeveloperConsole
{
    public class MathCommand : Command
    {
        [PositionalArg(0)] public float OperandA;
        [PositionalArg(1)] public float OperandB;

        [RequiredArg][SwitchArg("--operation", 'o')] public string Operation;
        protected override string Name() => "math";
        protected override string Description() => "Tests math operations.";

        protected override CommandResult Execute(CommandArgs args)
        {
            return Operation switch
            {
                "add" => new((OperandA + OperandB).ToString()),
                "subtract" => new((OperandA - OperandB).ToString()),
                "multiply" => new((OperandA * OperandB).ToString()),
                "divide" when OperandB == 0 => new("Cannot divide by zero."),
                "divide" => new((OperandA / OperandB).ToString()),
                _ => new("Invalid operation.")
            };
        }
    }
}