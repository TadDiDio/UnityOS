using DeveloperConsole;

public interface ILobbyTest
{
    public void Join();
}

public class Lobby : ILobbyTest
{
    public void Join()
    {
        Log.Info("Joined");
    }
}
