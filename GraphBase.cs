using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DaGenGraph
{
    [Serializable]
    public class GraphBase
    {
        public Vector2 currentPanOffset = Vector2.zero;
        public float currentZoom = 1f;
        public int windowID;
        public NodeBase startNode;
        public bool leftInRightOut;
        public string guid = Guid.NewGuid().ToString();
        public Dictionary<string, NodeBase> nodes { private set; get; } = new();

        public virtual void Save()
        {
            //序列化为文件储存
        }
    }
}