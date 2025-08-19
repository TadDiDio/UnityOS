using System.Collections.Generic;
using DeveloperConsole.Command.Execution;
using DeveloperConsole.Scripts.Command.Execution;

namespace DeveloperConsole.Parsing.Graph
{
    public class GraphBuilder
    {
        private GraphNode _root = new();
        private List<AnnotationEdge> _annotations = new();

        private GraphNode _parallelAnchor;
        private GraphNode _serialAnchor;
        private GraphNode _serialParent;

        private bool _lastWasSerial;

        public GraphBuilder()
        {
            _parallelAnchor = _root;
            _serialAnchor = _root;
            _serialParent = _root;
        }

        public GraphBuilder Then(GraphNode node)
        {
            if (_lastWasSerial) _serialParent = _serialAnchor;

            _lastWasSerial = true;
            _serialAnchor.Outgoing.Add(new GraphEdge(node));
            _serialAnchor = node;

            return this;
        }

        public GraphBuilder ThenOnSuccess(GraphNode node)
        {
            if (_lastWasSerial) _serialParent = _serialAnchor;

            _lastWasSerial = true;
            _serialAnchor.Outgoing.Add(new GraphEdge(node));
            _serialAnchor = node;

            node.Condition = ExecutionCondition.OnSuccess;

            return this;
        }

        public GraphBuilder ThenOnFailure(GraphNode node)
        {
            if (_lastWasSerial) _serialParent = _serialAnchor;

            _lastWasSerial = true;
            _serialAnchor.Outgoing.Add(new GraphEdge(node));
            _serialAnchor = node;

            node.Condition = ExecutionCondition.OnFailure;

            return this;
        }

        public GraphBuilder And(GraphNode node)
        {
            if (_lastWasSerial) _parallelAnchor = _serialParent;

            _lastWasSerial = false;
            _parallelAnchor.Outgoing.Add(new GraphEdge(node));
            _serialAnchor = node;

            return this;
        }

        public GraphBuilder AndPipeTo(GraphNode node)
        {
            if (_lastWasSerial) _parallelAnchor = _serialParent;

            _lastWasSerial = false;
            _parallelAnchor.Outgoing.Add(new GraphEdge(node));
            _serialAnchor = node;

            _annotations.Add(new PipeAnnotation(_serialAnchor, node));

            return this;
        }

        public CommandGraph Build()
        {
            return new CommandGraph(_root, _annotations);
        }
    }
}
