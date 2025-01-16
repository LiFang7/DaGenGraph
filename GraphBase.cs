using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DaGenGraph
{
    [Serializable]
    public abstract class GraphBase
    {
        public Vector2 currentPanOffset = Vector2.zero;
        public float currentZoom = 1f;
        public int windowID;
        public string startNodeId;

        public bool leftInRightOut;
        public string guid = Guid.NewGuid().ToString();
        public Dictionary<string, NodeBase> nodes { private set; get; } = new();

        public NodeBase GetStartNode()
        {
            if (!string.IsNullOrEmpty(startNodeId) && nodes.TryGetValue(startNodeId, out var node))
            {
                return node;
            }

            return null;
        }
    }
}