using NPSerialization;
using NPVisualEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class VisualEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/VisualEditor")]
    public static void ShowExample()
    {
        VisualEditor wnd = GetWindow<VisualEditor>();
        wnd.titleContent = new GUIContent("VisualEditor");
    }

    public void CreateGUI()
    {
        VisualTreeAsset visualEditor = Resources.Load<VisualTreeAsset>("VisualEditor");
        TemplateContainer editorInstance = visualEditor.CloneTree();
        editorInstance.StretchToParentSize();
        rootVisualElement.Add(editorInstance);

        BlackboardPanel = rootVisualElement.Q<VisualElement>("Blackboard");
        VisualTreeAsset blackboardViewTree = Resources.Load<VisualTreeAsset>("BlackboardPanel");
        TemplateContainer blackboardViewInstance = blackboardViewTree.CloneTree();
        BlackboardPanel.Add(blackboardViewInstance);

         InspectorView = rootVisualElement.Q<ScrollView>("Inspector");

        NodeGraphicView = rootVisualElement.Q<GraphicView>("GraphicView");
        NodeGraphicView.ManualAddNode += OnManualAddNode;
        NodeGraphicView.ManualRemoveNodes += OnManualRemoveNodes;

        var openBtn = rootVisualElement.Q<Button>("open");
        openBtn.RegisterCallback<MouseUpEvent>((evt) => Open());
    }

    private void OnManualRemoveNodes(IList<GraphNode> list)
    {
        if (m_tmpNodeDataTree == null)
            return;

        foreach (GraphNode node in list)
        {
            if (node.Data != null && m_tmpNodeDataTree.m_nodeDataDict.ContainsKey(node.Data.m_ID))
            {
                m_tmpNodeDataTree.m_nodeDataDict.Remove(node.Data.m_ID);
            }
        }
    }

    private void OnManualAddNode(GraphNode node)
    {
        if (m_tmpNodeDataTree == null || node.Data == null)
            return;

        node.Data.m_ID = GenerateID();
        node.ID = node.Data.m_ID;
        node.SelectedCB += OnSelectedNode;

        m_tmpNodeDataTree.m_nodeDataDict[node.Data.m_ID] = node.Data;
    }

    private void Open()
    {
        string path = EditorUtility.OpenFilePanel("Select", Application.dataPath, "");
        string extension = Path.GetExtension(path);
        NodeDataTree nodeDataTree = null;
        switch (extension)
        {
            case ".json":
                var jsonStream = new JsonStream();
                jsonStream.Load<NodeDataTree>(path, out nodeDataTree);
                break;
        }

        if (nodeDataTree != null)
        {
            CreateNodeGraphByData(nodeDataTree);
            m_tmpNodeDataTree = nodeDataTree;
        }
    }

    private void CreateNodeGraphByData(NodeDataTree nodeDataTree)
    {
        if (nodeDataTree == null || NodeGraphicView == null)
            return;

        NodeGraphicView.ClearGraphNodes();
        ID2GraphNode.Clear();

        long rootID = nodeDataTree.m_rootID;

        Queue<long> q = new();
        q.Enqueue(rootID);

        while (q.Count > 0)
        {
            var id = q.Dequeue();
            var node = NodeGraphicView.CreateNode(nodeDataTree.m_nodeDataDict[id].m_position);

            node.Data = nodeDataTree.m_nodeDataDict[id];
            GraphicUtils.UpdateGraphNode(node);
            node.SelectedCB += OnSelectedNode;
            ID2GraphNode.Add(id, node);

            IList<long> linkedNodeIDs = nodeDataTree.m_nodeDataDict[id].m_linkedNodeIDs;
            foreach (long childID in linkedNodeIDs)
            {
                q.Enqueue(childID);
            }
        }

        foreach (var nodeData in nodeDataTree.m_nodeDataDict.Values)
        {
            long parentID = nodeData.m_parentID;
            if (parentID != 0)
            {
                NodeGraphicView.CreateEdge(ID2GraphNode[parentID].Q<Port>("Children"), ID2GraphNode[nodeData.m_ID].Q<Port>("Parent"));
            }
        }

        NodeGraphicView.RootNode = ID2GraphNode[nodeDataTree.m_rootID];
        GraphicUtils.OptimizeTreeLayout(NodeGraphicView.RootNode);
    }

    private void OnSelectedNode(object sender, EventArgs args)
    {
        InspectorView.Clear();
        GraphNode node = (GraphNode)sender;
        if (node != null)
        {
            long id = node.ID;
            if (m_tmpNodeDataTree.m_nodeDataDict.TryGetValue(id, out NodeData nodeData))
            {
                var elements = UIFactory.CreateElements(node, nodeData);
                foreach (var element in elements)
                {
                    InspectorView.Add(element);
                }
            }
        }
    }

    private long GenerateID()
    {
        if (m_tmpNodeDataTree == null)
            return 0;

        long maxID = m_tmpNodeDataTree.m_nodeDataDict.Keys.Max();

        return maxID + 1;
    }

    public Dictionary<long, GraphNode> ID2GraphNode { get; private set; } = new();
    public VisualElement BlackboardPanel { get; private set; }
    public GraphicView NodeGraphicView { get; private set; }
    public ScrollView InspectorView { get; private set; }
    private NodeDataTree m_tmpNodeDataTree = null;
}
