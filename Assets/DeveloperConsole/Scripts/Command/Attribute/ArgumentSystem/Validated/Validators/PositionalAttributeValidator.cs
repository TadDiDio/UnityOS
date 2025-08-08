namespace DeveloperConsole.Command
{
    public class PositionalAttributeValidator : IAttributeValidator
    {
        private bool _wasSet;
        private string _name;

        public bool Validate(ArgumentSpecification argSpec)
        {
            _name = argSpec.Name;
            return _wasSet;
        }

        public void Record(RecordingContext context)
        {
            _wasSet = true;
        }

        public string ErrorMessage()
        {
            return $"Missing value for positional argument: '{_name ?? "unknown"}'.";
        }
    }
}
