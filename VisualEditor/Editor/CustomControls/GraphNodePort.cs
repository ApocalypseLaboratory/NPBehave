using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NPVisualEditor
{
    public class GraphNodePort : Port
    {
        protected GraphNodePort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
        }

        public override void Disconnect(Edge edge)
        {
            base.Disconnect(edge);
            DisconnectNode?.Invoke(edge);
        }

        public override void Connect(Edge edge)
        {
            base.Connect(edge);
            ConnectNode?.Invoke(edge);
        }

        public static new GraphNodePort Create<TEdge>(Orientation orientation, Direction direction, Capacity capacity, Type type) where TEdge : Edge, new()
        {
            DefaultEdgeConnectorListener listener = new();
            GraphNodePort port = new(orientation, direction, capacity, type)
            {
                m_EdgeConnector = new EdgeConnector<TEdge>(listener)
            };
            port.AddManipulator(port.m_EdgeConnector);
            return port;
        }

        private class DefaultEdgeConnectorListener : IEdgeConnectorListener
        {
            private GraphViewChange m_GraphViewChange;

            private List<Edge> m_EdgesToCreate;

            private List<GraphElement> m_EdgesToDelete;

            public DefaultEdgeConnectorListener()
            {
                m_EdgesToCreate = new List<Edge>();
                m_EdgesToDelete = new List<GraphElement>();
                m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
            }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                m_EdgesToCreate.Clear();
                m_EdgesToCreate.Add(edge);
                m_EdgesToDelete.Clear();
                if (edge.input.capacity == Capacity.Single)
                {
                    foreach (Edge connection in edge.input.connections)
                    {
                        if (connection != edge)
                        {
                            m_EdgesToDelete.Add(connection);
                        }
                    }
                }

                if (edge.output.capacity == Capacity.Single)
                {
                    foreach (Edge connection2 in edge.output.connections)
                    {
                        if (connection2 != edge)
                        {
                            m_EdgesToDelete.Add(connection2);
                        }
                    }
                }

                if (m_EdgesToDelete.Count > 0)
                {
                    graphView.DeleteElements(m_EdgesToDelete);
                }

                List<Edge> edgesToCreate = m_EdgesToCreate;
                if (graphView.graphViewChanged != null)
                {
                    edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
                }

                foreach (Edge item in edgesToCreate)
                {
                    graphView.AddElement(item);
                    edge.input.Connect(item);
                    edge.output.Connect(item);
                }
            }
        }

        public Action<Edge> ConnectNode;
        public Action<Edge> DisconnectNode;
    }
}