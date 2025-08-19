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
            // TODO: Make a pipe between source and target
        }
    }
}
