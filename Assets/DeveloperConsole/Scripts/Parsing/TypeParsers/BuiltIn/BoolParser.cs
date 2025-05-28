namespace DeveloperConsole
{
    public class BoolParser : BaseTypeParser
    {
        public override int TokenCount() => 1;

        protected override bool TryParseType(TokenStream tokenSubSteam, out object obj)
        {
            bool success = bool.TryParse(tokenSubSteam.Next(), out bool typed);

            obj = typed;
            return success;
        }
    }
}