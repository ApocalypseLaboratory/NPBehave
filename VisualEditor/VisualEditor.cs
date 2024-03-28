using NPSerialization;
using NPVisualEditor;
using System.Collections.Generic;
using System.IO;
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

        NodeGraphicView = rootVisualElement.Q<GraphicView>("GraphicView");

        var openBtn = rootVisualElement.Q<Button>("open");
        openBtn.RegisterCallback<MouseUpEvent>((evt) => Open());
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
            nodeDataTree.CreateTreeByNodeData();
            CreateNodeGraphByData(nodeDataTree);
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

        //GraphicUtils.OptimizeTreeLayout(NodeGraphicView.RootNode);
    }

    public Dictionary<long, GraphNode> ID2GraphNode { get; private set; } = new();
    public VisualElement BlackboardPanel { get; private set; }
    public GraphicView NodeGraphicView { get; private set; }
}
