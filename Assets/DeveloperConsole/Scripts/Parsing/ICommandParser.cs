namespace DeveloperConsole
{
    public interface ICommandParser
    {
        public ParseResult Parse(TokenStream stream, string parentName = "");
    }
}