using DeveloperConsole.Parsing.Tokenizing;
using DeveloperConsole.Parsing.TypeAdapting.Types;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public class ConfirmationResultAdapter : TypeAdapter<ConfirmationResult>
    {
        protected override bool TryConsumeAndConvert(TokenStream stream, out ConfirmationResult result)
        {
            string value = stream.Next().ToLowerInvariant();

            switch (value)
            {
                case "y" or "yes":
                    result = new ConfirmationResult { Success = true };
                    return true;
                case "n" or "no":
                    result = new ConfirmationResult { Success = false };
                    return true;
                default:
                    result = null;
                    return false;
            }
        }
    }
}
