using NPBehave;
using NPSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NPVisualEditor
{
    public static class GraphicUtils
    {
        public static void UpdateGraphNode(GraphNode node)
        {
            NodeData nodeData = node.Data as NodeData;
            if (nodeData == null)
                return;

            node.ID = nodeData.m_ID;
            node.title = nodeData.GetType().Name.Replace("Data", "");
            node.Description = nodeData.m_description;
            node.SubTitle = NodeDataUtils.GetSubTitle(nodeData);
        }

        public static void OptimizeTreeLayout(GraphNode node)
        {
            IList<IList<GraphNode>> nodeLayers;

            ResetNodePosition(node);
            InitLayer(node, out nodeLayers);
            LayoutChild(node);
            LayoutOverlaps(nodeLayers);
        }

        private static void ResetNodePosition(GraphNode node)
        {
            Queue<GraphNode> queue = new Queue<GraphNode>();
            queue.Enqueue(node);

            while (queue.Count > 0)
            {
                var curNode = queue.Dequeue();
                SetPosition(curNode, new Vector2(0, 0));

                foreach (var child in GetChildren(curNode))
                {
                    queue.Enqueue(child);
                }
            }
        }

        private static void InitLayer(GraphNode node, out IList<IList<GraphNode>> nodeLayers)
        {
            nodeLayers = new List<IList<GraphNode>>();
            Queue<GraphNode> queue = new Queue<GraphNode>();
            queue.Enqueue(node);
            float y = GetPosition(node).y;

            while (queue.Count > 0)
            {
                List<GraphNode> nodeLayer = new();
                while (queue.Count > 0)
                {
                    var curNode = queue.Dequeue();
                    var pos = GetPosition(curNode);
                    SetPosition(curNode, new Vector2(0, y));
                    nodeLayer.Add(curNode);
                }
                nodeLayers.Add(nodeLayer);

                foreach (var layerNode in nodeLayer)
                {
                    foreach (var child in GetChildren(layerNode))
                    {
                        queue.Enqueue(child);
                    }
                }

                y += IntervalY;
            }
        }

        private static void LayoutChild(GraphNode node)
        {
            IList<GraphNode> children = GetChildren(node);

            if (children.Count == 0)
                return;

            float start = GetPosition(node).x - (children.Count - 1) * IntervalX / 2.0f;

            int i = 0;
            foreach (var child in children)
            {
                float x = start + i * IntervalX;
                TranslateTree(child, x);
                LayoutChild(child);
                i++;
            }
        }

        private static void LayoutOverlaps(IList<IList<GraphNode>> nodeLayers)
        {
            for (int i = nodeLayers.Count - 1; i >= 0; --i)
            {
                IList<GraphNode> layer = nodeLayers[i];
                for (int j = 0; j < layer.Count - 1; ++j)
                {
                    if (IsOverlaps(layer[j], layer[j + 1]))
                    {
                        float dx = GetPosition(layer[j]).x - GetPosition(layer[j + 1]).x + IntervalX;
                        GraphNode commonParent = FindCommonParentNode(layer[j], layer[j + 1]);
                        TranslateTree(commonParent, GetPosition(commonParent).x + dx);
                        CenterChildren(GetParent(commonParent));

                        i = nodeLayers.Count;
                    }
                }
            }
        }

        public static IList<GraphNode> GetChildren(GraphNode node)
        {
            List<GraphNode> children = new();
            Port port = node.Q<Port>("Children");
            foreach (var edge in port.connections)
            {
                GraphNode connectedNode = edge.input.node as GraphNode;
                children.Add(connectedNode);
            }

            return children;
        }

        public static GraphNode GetParent(GraphNode node)
        {
            Port port = node.Q<Port>("Parent");
            if (port == null || port.connections.Count() == 0)
                return null;

            var edge = port.connections.First();
            GraphNode connectedNode = edge.output.node as GraphNode;
            return connectedNode;
        }

        /// <summary>
        /// Find the ancestor node of node2 that is sibling to a certain ancestor node of node1
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        public static GraphNode FindCommonParentNode(GraphNode node1, GraphNode node2)
        {
            GraphNode node1Parent = GetParent(node1);
            GraphNode node2Parent = GetParent(node2);
            if (node1Parent == node2Parent)
                return node2;
            else
                return FindCommonParentNode(node1Parent, node2Parent);
        }

        private static void TranslateTree(GraphNode node, float x)
        {
            var pos = GetPosition(node);
            float dx = x - pos.x;
            SetPosition(node, new Vector2(x, pos.y));

            foreach (var childNode in GetChildren(node))
            {
                TranslateTree(childNode, GetPosition(childNode).x + dx);
            }
        }

        private static void CenterChildren(GraphNode node)
        {
            float dx = 0;

            if (node == null)
                return;

            IList<GraphNode> children = GetChildren(node);
            if (children.Count == 1)
            {
                dx = GetPosition(node).x - GetPosition(children[0]).x;
            }

            if (children.Count > 1)
            {
                dx = GetPosition(node).x - (GetPosition(children[0]).x + (GetPosition(children[children.Count - 1]).x - GetPosition(children[0]).x) / 2.0f);
            }

            if (Mathf.Abs(dx) > float.Epsilon)
            {
                foreach (var childNode in children)
                {
                    TranslateTree(childNode, GetPosition(childNode).x + dx);
                }
            }
        }

        private static bool IsOverlaps(GraphNode node1, GraphNode node2)
        {
            return (GetPosition(node1).x - GetPosition(node2).x > 0) || (GetPosition(node2).x - GetPosition(node1).x < IntervalX);
        }

        public static Vector2 GetPosition(GraphNode node)
        {
            // Error value if you try to GetPosition(may be updated neext frame). Fuck unity
            Vector2 position = new(node.style.left.value.value, node.style.top.value.value);
            return position;
        }

        // Set the position of a node
        public static void SetPosition(GraphNode node, Vector2 position)
        {
            Rect rect = new(position, DEFAULT_NODE_SIZE);
            node.SetPosition(rect);
            node.MarkDirtyRepaint();
        }

        public static float IntervalX = 300f;
        public static float IntervalY = 200f;

        public static Vector2 DEFAULT_NODE_SIZE = new(150, 200);
    }
}