namespace DeveloperConsole
{
    public class StringParser : BaseTypeParser<string>
    {
        public override int TokenCount() => 1;

        protected override bool TryParseType(TokenStream tokenSubSteam, out string obj)
        {
            obj = tokenSubSteam.Next();
            return true;
        }
    }
}