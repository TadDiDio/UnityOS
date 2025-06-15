namespace DeveloperConsole.Command
{
    public class RequiredAttribute : ValidatedAttribute
    {
        private string _name;
        private bool _wasSet;
        
        public override bool Validate(ArgumentSpecification spec)
        {
            _name = spec?.Name;
            return _wasSet;
        }

        public override void Record(RecordingContext context) => _wasSet = true;
        public override string ErrorMessage() => $"Argument '{_name ?? "unknown"}' must be explicitly set.";
    }
}