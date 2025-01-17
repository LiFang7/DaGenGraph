using System;

namespace DaGenGraph.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeViewTypeAttribute: Attribute
    {
        public NodeViewTypeAttribute(Type baseViewNode)
        {
            ViewType = baseViewNode;
        }
        public Type ViewType;
    }
}