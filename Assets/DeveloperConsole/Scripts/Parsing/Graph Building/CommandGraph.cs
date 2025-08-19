using System.Collections.Generic;
using DeveloperConsole.Parsing.Graph;

namespace DeveloperConsole.Command.Execution
{
    public class CommandGraph
    {
        public readonly GraphNode Root;
        public readonly List<AnnotationEdge> Annotations;

        public CommandGraph(GraphNode root, List<AnnotationEdge> annotations)
        {
            Root = root;
            Annotations = annotations;
        }

        public void Print()
        {
            if (Root == null) return;
            Log.Info("[Root]");
            foreach (var edge in Root.Outgoing)
            {
                PrintNode(edge.Target, "", new HashSet<GraphNode>(), true);
            }
        }

        private void PrintNode(GraphNode node, string indent, HashSet<GraphNode> visited, bool isLast)
        {
            if (node == null) return;

            // Print current node
            Log.Info($"{indent}{(isLast ? "└── " : "├── ")}{node.ToString() ?? "[null]"}");

            // Prevent infinite loops on cycles
            if (!visited.Add(node))
            {
                Log.Info($"{indent}    (already visited)");
                return;
            }

            // Prepare indent for children
            string childIndent = indent + (isLast ? "    " : "│   ");
            for (int i = 0; i < node.Outgoing.Count; i++)
            {
                var edge = node.Outgoing[i];
                bool lastChild = (i == node.Outgoing.Count - 1);
                PrintNode(edge.Target, childIndent, visited, lastChild);
            }
        }
    }

    public static class GraphIterator
    {
        // Depth-first traversal
        public static IEnumerable<GraphNode> TraverseDFS(GraphNode root)
        {
            if (root == null) yield break;

            var visited = new HashSet<GraphNode>();
            var stack = new Stack<GraphNode>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                var node = stack.Pop();
                if (!visited.Add(node))
                    continue;

                yield return node;

                // Push children in reverse order if you want left-to-right traversal
                for (int i = node.Outgoing.Count - 1; i >= 0; i--)
                {
                    stack.Push(node.Outgoing[i].Target);
                }
            }
        }

        // Breadth-first traversal
        public static IEnumerable<GraphNode> TraverseBFS(GraphNode root)
        {
            if (root == null) yield break;

            var visited = new HashSet<GraphNode>();
            var queue = new Queue<GraphNode>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (!visited.Add(node))
                    continue;

                yield return node;

                foreach (var edge in node.Outgoing)
                {
                    queue.Enqueue(edge.Target);
                }
            }
        }
    }
}
