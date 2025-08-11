using DeveloperConsole;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public string message;
    private ILobbyTest _lobby = new Lobby();
    private void Awake()
    {
        ConsoleAPI.Bindings.BindObject(typeof(ILobbyTest), _lobby);
    }

    public string SayIt() => message;
}
