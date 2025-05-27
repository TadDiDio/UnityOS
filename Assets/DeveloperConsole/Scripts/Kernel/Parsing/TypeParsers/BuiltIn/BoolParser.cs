namespace DeveloperConsole
{
    public class BoolParser : BaseTypeParser<bool>
    {
        public override int TokenCount() => 1;

        protected override bool TryParseType(TokenStream tokenSubSteam, out bool obj)
        {
            return bool.TryParse(tokenSubSteam.Next(), out obj);
        }
    }
}