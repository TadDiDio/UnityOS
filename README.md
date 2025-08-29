# UnityOS
A terminal and windowing interface for Unity

# Usage
With this pacakge you can author and execute arbitray commands via a bash like terminal in Unity. This system is available at runtime and during editmode, allowing for commands to seamlessly be used in both environments. Delveopers can speed up workflows, debugging, and playtesting by using one of the many built in commands or by writing their own. 

# Key Features
- Developers can write custom commands very easily.
- Strongly typed arguments are injected into commands via an attribute system meaning developers never need to write parsing logic or perform validation or type casting. Attributes can automatically handle string -> type conversions, range validation and more. 
- Write custom processors and validators to further validate arguments ONCE and use them in any command.
- Write type adapters to specify how strings convert on the CLI to custom types.
- If you want, you can write your own parser rules and add them to the system seamlessly. The console ships with a functionally complete set.
- Automatic build culling. The console is available in developer builds but automatically strips itself during release builds
 
# Installation
Simply install the package using the Import Package menu in Unity. There is no additional set up and pressing the '/' key will bring up the terminal for use. Type reg for a list of all commands and 'help <command> -v' for detailed information on a specific command
