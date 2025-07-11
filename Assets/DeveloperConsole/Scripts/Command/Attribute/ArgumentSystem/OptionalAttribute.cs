namespace DeveloperConsole.Command
{
    public class OptionalAttribute : InformativeAttribute
    {
        /// <summary>
        /// The index in the command.
        /// </summary>
        public int Index { get; }
        
        
        /// <summary>
        /// Creates a positional arg.
        /// </summary>
        /// <param name="index">The index this arg must appear at relative to other optionals.</param>
        /// <param name="description">The arg's description.</param>
        /// <param name="overrideName">Overrides the field name.</param>
        public OptionalAttribute(int index, string description, string overrideName = null) : base(description, overrideName)
        {
            Index = index;
        }

        public override string ToString()
        {
            return $"{Name ?? "Positional"} ({Index}): {Description}";
        }
    }
}