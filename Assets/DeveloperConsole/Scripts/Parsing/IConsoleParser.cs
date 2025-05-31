namespace DeveloperConsole
{
    public interface IConsoleParser
    {
        public ParseResult Parse(TokenStream stream, string parentName = "");
    }
}