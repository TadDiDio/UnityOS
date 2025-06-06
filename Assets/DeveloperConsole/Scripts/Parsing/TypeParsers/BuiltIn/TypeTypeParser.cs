namespace DeveloperConsole
{
    public class TypeTypeParser : BaseTypeParser
    {
        public override int TokenCount() => 1;

        protected override bool TryParseType(TokenStream tokenSubSteam, out object obj)
        {
            obj = null;
            string typeName = tokenSubSteam.Next();

            obj = TypeFriendlyNames.NameToType(typeName);

            return obj != null;
        }
    }
}