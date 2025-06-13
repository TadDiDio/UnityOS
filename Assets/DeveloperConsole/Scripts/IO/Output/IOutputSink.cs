namespace DeveloperConsole.IO
{
    public interface IOutputSink
    {
        public void ReceiveOutput(IOutputMessage message);
    }
}