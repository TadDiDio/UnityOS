using System.Threading.Tasks;

namespace DeveloperConsole
{
    public interface ICommandParser
    {
        public Task<ParseResult> Parse(TokenStream stream, string parentName = "");
    }
}