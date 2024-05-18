using NPSerialization;
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NPVisualEditor
{
    public class GraphNode : Node
    {
        public new class UxmlFactory : UxmlFactory<GraphNode, UxmlTraits>
        {
        }

        public GraphNode() : base( AssetDatabase.GetAssetPath(Resources.Load<VisualTreeAsset>("GraphNode")))
        {
            m_descLabel = this.Q<Label>("description-label");
            m_subTitleLabel = this.Q<Label>("subtitle-label");

            m_parentPort = InstantiateGraphNodePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(Node));
            m_parentPort.name = "Parent";
            inputContainer.Add(m_parentPort);

            m_childrenPort = InstantiateGraphNodePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(Node));
            m_childrenPort.name = "Children";
            outputContainer.Add(m_childrenPort);

            m_parentPort.ConnectNode += OnConnectNode;
            m_parentPort.DisconnectNode += OnDisconnectNode;

            RefreshExpandedState();
            RefreshPorts();
        }

        private void OnConnectNode(Edge edge)
        {
            var outputPort = edge.output;
            var inputPort = edge.input;
            
            if (outputPort == m_parentPort)
            {
                var node = inputPort.node;
                NodeDataUtils.AddChild(((GraphNode)node).Data, Data);
            }
            else if (inputPort == m_parentPort)
            {
                var node = outputPort.node;
                NodeDataUtils.AddChild(((GraphNode)node).Data, Data);
            }
            else if (outputPort == m_childrenPort)
            {
                var node = inputPort.node;
                NodeDataUtils.AddChild(Data, ((GraphNode)node).Data);
            }
            else if (inputPort == m_childrenPort)
            {
                var node = outputPort.node;
                NodeDataUtils.AddChild(Data, ((GraphNode)node).Data);
            }
        }

        private void OnDisconnectNode(Edge edge)
        {
            var outputPort = edge.output;
            var inputPort = edge.input;

            if (outputPort == m_parentPort)
            {
                var node = inputPort.node;
                NodeDataUtils.RemoveChild(((GraphNode)node).Data, Data);
            }
            else if (inputPort == m_parentPort)
            {
                var node = outputPort.node;
                NodeDataUtils.RemoveChild(((GraphNode)node).Data, Data);
            }
            else if (outputPort == m_childrenPort)
            {
                var node = inputPort.node;
                NodeDataUtils.RemoveChild(Data, ((GraphNode)node).Data);
            }
            else if (inputPort == m_childrenPort)
            {
                var node = outputPort.node;
                NodeDataUtils.RemoveChild(Data, ((GraphNode)node).Data);
            }
        }

        public string Description
        {
            get
            {
                return (m_descLabel != null) ? m_descLabel.text : string.Empty;
            }
            set
            {
                if (m_descLabel != null)
                {
                    m_descLabel.text = value;
                }
            }
        }

        public string SubTitle
        {
            get
            {
                return (m_subTitleLabel != null) ? m_subTitleLabel.text : string.Empty;
            }
            set
            {
                if (m_subTitleLabel != null)
                {
                    m_subTitleLabel.text = value;
                }
            }
        }

        public GraphNodePort InstantiateGraphNodePort(Orientation orientation, Direction direction, Port.Capacity capacity, Type type)
        {
            return GraphNodePort.Create<Edge>(orientation, direction, capacity, type);
        }

        public long ID { get; set; }

        public override void OnSelected()
        {
            base.OnSelected();
            SelectedCB?.Invoke(this, EventArgs.Empty);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            UnselectedCB?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnPortRemoved(Port port)
        {
            base.OnPortRemoved(port);
            PortRemovedCB.Invoke(this, port);
        }

        GraphNodePort m_parentPort;
        GraphNodePort m_childrenPort;

        private readonly Label m_descLabel;
        private readonly Label m_subTitleLabel;

        public event EventHandler SelectedCB;
        public event EventHandler UnselectedCB;

        public event EventHandler<Port> PortRemovedCB;
        public event EventHandler<Port> PortConnectedCB;

        public NodeData Data { get; set; }
    }
}