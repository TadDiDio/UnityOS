using DeveloperConsole.Command;

[Command("lobby", "Manages lobby interactions")]
[Restrict(UnityEnvironment.Runtime)]
public class LobbyCommand : SimpleCommand
{
    [Bind] private ILobbyTest lobby;
    [Bind(tag: "Player")] private Tester tester;
    protected override CommandOutput Execute(CommandContext context)
    {
        lobby.Join();
        return new CommandOutput(tester.SayIt());
    }
}
