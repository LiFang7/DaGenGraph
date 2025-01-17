using DaGenGraph.Editor;
using UnityEngine;

namespace DaGenGraph.Example
{
    [NodeViewType(typeof(ExampleNodeView))]
    public class ExampleNode: NodeBase
    {
        public string Text;
        public override void AddDefaultPorts()
        {
            AddOutputPort("DefaultOutputName", EdgeMode.Multiple, true, true);
        }
    }
}