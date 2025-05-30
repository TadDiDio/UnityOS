namespace DeveloperConsole
{
    public interface IWindowManager
    {
        public void Register(IGraphical window);
        public void Unregister(IGraphical window);
        public void OnGUI(GUIContext context);
    }
}