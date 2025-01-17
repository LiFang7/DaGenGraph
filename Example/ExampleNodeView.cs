using DaGenGraph.Editor;
using UnityEngine;

namespace DaGenGraph.Example
{
    public class ExampleNodeView: NodeView<ExampleNode>
    {
        protected override void DrawContent()
        {
            base.DrawContent();
            node.Text = DrawPropString("测试文字", node.Text);
        }
    }
}