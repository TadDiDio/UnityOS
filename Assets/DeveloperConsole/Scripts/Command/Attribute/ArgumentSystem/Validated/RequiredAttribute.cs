namespace DeveloperConsole.Command
{
    public class RequiredAttribute : ArgumentAttribute, IAttributeValidatorFactory
    {
        public IAttributeValidator CreateValidatorInstance()
        {
            return new RequiredAttributeValidator();
        }
    }
}
