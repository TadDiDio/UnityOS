namespace DeveloperConsole.Command
{
    public class ArgumentDeclarationAttribute : ArgumentAttribute
    {
        /// <summary>
        /// The override for an argument name. If not set, the field's name will be used.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The argument's description.
        /// </summary>
        public  string Description => CommandMetaProcessor.Description(_rawDescription);
        private string _rawDescription;


        protected ArgumentDeclarationAttribute(string description, string overrideName = null)
        {
            Name = overrideName;
            _rawDescription = description;
        }
    }
}
