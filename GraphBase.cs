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

        public Dictionary<string, NodeBase> nodes = new();

        public NodeBase GetStartNode()
        {
            return FindNode(startNodeId);
        }

        public NodeBase FindNode(string nodeId)
        {
            if (!string.IsNullOrEmpty(nodeId) && nodes.TryGetValue(nodeId, out var node))
            {
                return node;
            }

            return null;
        }
        
        public NodeBase CreateNode<T>(Vector2 pos, string nodeName = "Node", bool isRoot = false) where T: NodeBase
        {
            var node = Activator.CreateInstance<T>();
            node.InitNode(WorldToGridPosition(pos), nodeName);
            if (!nodes.ContainsKey(node.id))
            {
                nodes.Add(node.id, node);
                if ((isRoot || string.IsNullOrEmpty(startNodeId)) && nodes.Count>0)
                {
                    startNodeId = nodes.First().Key;
                }
            }
            node.AddDefaultPorts();
            return node;
        }
        
        public Vector2 WorldToGridPosition(Vector2 worldPosition)
        {
            return (worldPosition - currentPanOffset) / currentZoom;
        }
    }
}