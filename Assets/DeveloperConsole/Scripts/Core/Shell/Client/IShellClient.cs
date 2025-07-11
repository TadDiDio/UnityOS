using DeveloperConsole.IO;

namespace DeveloperConsole.Core.Shell
{
    public interface IShellClient : IInputChannel, IOutputChannel
    {
        /// <summary>
        /// Tells if this source can handle a prompt request. Throws on false.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool CanHandlePrompt(PromptRequest request);


        /// <summary>
        /// Notify this client that the shell requires the next input to match the prompt request. The next
        /// input given by the client or any of its peripheral input sources that matches will be consumed as
        /// a prompt response.
        /// </summary>
        /// <param name="request">The request to match</param>
        /// <typeparam name="T">The input type expected.</typeparam>
        public void PromptedFor<T>(PromptRequest request) where T : IInput;


        /// <summary>
        /// Gets the signal handler for this client.
        /// </summary>
        public ShellSession.SignalHandler GetSignalHandler();
    }
}
