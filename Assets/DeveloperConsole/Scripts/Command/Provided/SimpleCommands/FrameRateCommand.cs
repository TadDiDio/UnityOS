using UnityEngine;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Restrict(UnityEnvironment.Runtime)]
    [Command("fps", "Shows the current frame rate.")]
    public class FrameRateCommand : SimpleCommand
    {
        protected override CommandOutput Execute(SimpleContext context)
        {
            return new CommandOutput(1 / Time.deltaTime);
        }
    }
}
