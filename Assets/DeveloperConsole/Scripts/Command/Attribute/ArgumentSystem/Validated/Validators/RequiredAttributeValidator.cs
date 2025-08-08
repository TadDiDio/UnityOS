namespace DeveloperConsole.Command
{
    public class RequiredAttributeValidator : IAttributeValidator
    {
        private string _name;
        private bool _wasSet;

        public bool Validate(ArgumentSpecification spec)
        {
            _name = spec?.Name;
            return _wasSet;
        }

        public void Record(RecordingContext context) => _wasSet = true;

        public string ErrorMessage() => $"Argument '{_name ?? "unknown"}' must be explicitly set.";
    }
}
