using DeveloperConsole.Core.Shell;
using DeveloperConsole.Scripts.Command.Execution;

namespace DeveloperConsole.Parsing.Graph
{
    public class GraphEdge
    {
        public readonly GraphNode Target;

        public GraphEdge(GraphNode target)
        {
            Target = target;
        }
    }

    public abstract class AnnotationEdge
    {
        public readonly GraphNode Source;
        public readonly GraphNode Target;

        public AnnotationEdge(GraphNode source, GraphNode target)
        {
            Source = source;
            Target = target;
        }

        public abstract void Setup();
    }

    public class PipeAnnotation : AnnotationEdge
    {
        public PipeAnnotation(GraphNode source, GraphNode target) : base(source, target) { }

        public override void Setup()
        {
            var pipe = new CommandPipe();

            Source.ExecutionContext.IoContext = new IOContext(
                prompt: Source.ExecutionContext.IoContext.Prompt.Promptable,
                output: pipe,
                emitter: Source.ExecutionContext.IoContext.SignalEmitter,
                session: Source.ExecutionContext.IoContext.ShellSession,
                autoRetry: true,
                name: "Source Pipe"
            );

            Target.ExecutionContext.IoContext = new IOContext(
                prompt: pipe,
                output: Target.ExecutionContext.IoContext.Output,
                emitter: Target.ExecutionContext.IoContext.SignalEmitter,
                session: Target.ExecutionContext.IoContext.ShellSession,
                autoRetry: true,
                name: "Target Pipe"
            );
        }
    }
}
