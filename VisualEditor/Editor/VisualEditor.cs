using NPSerialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NPVisualEditor
{
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

            VariablesListView = BlackboardPanel.Q<ListView>("variables");
            Func<VisualElement> makeItem = () => new KVDataItem();
            VariablesListView.makeItem = makeItem;
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                (e as KVDataItem).KVData = m_blackboardKVDatas[i];
            };
            VariablesListView.bindItem = bindItem;
            VariablesListView.itemsSource = m_blackboardKVDatas;
            VariablesListView.selectionType = SelectionType.Multiple;

            InspectorView = rootVisualElement.Q<ScrollView>("Inspector");

            NodeGraphicView = rootVisualElement.Q<GraphicView>("GraphicView");
            NodeGraphicView.ManualAddNode += OnManualAddNode;
            NodeGraphicView.ManualRemoveNodes += OnManualRemoveNodes;
            NodeGraphicView.graphViewChanged += OnGraphViewChange;

            var openBtn = rootVisualElement.Q<Button>("open");
            openBtn.RegisterCallback<MouseUpEvent>((evt) => Open());

            var saveBtn = rootVisualElement.Q<Button>("save");
            saveBtn.RegisterCallback<MouseUpEvent>((evt) => Save());

            var newBtn = rootVisualElement.Q<Button>("new");
            newBtn.RegisterCallback<MouseUpEvent>((evt) => New());

            List<string> valueType = Enum.GetNames(typeof(CompareType)).ToList();
            var valueTypeDropdown = BlackboardPanel.Q<DropdownField>("variableType");
            valueTypeDropdown.choices = valueType;
            valueTypeDropdown.value = CompareType.TString.ToString();

            var addBtn = BlackboardPanel.Q<Button>("add");
            addBtn.RegisterCallback<MouseUpEvent>((evt)=> AddBlackBoardItem());

            var deleteBtn = BlackboardPanel.Q<Button>("delete");
            deleteBtn.RegisterCallback<MouseUpEvent>((evt)=> DeleteBlackBoardItem());

            OpenTmpData();
        }

        private void DeleteBlackBoardItem()
        {
            if (m_tmpNodeDataTree == null)
                return;

            BlackboardKVData kVData = VariablesListView.selectedItem as BlackboardKVData;
            if (kVData == null) 
                return;

            if (m_tmpNodeDataTree.m_blackboardInitList.Contains(kVData))
            {
                m_tmpNodeDataTree.m_blackboardInitList.Remove(kVData);
            }
            UpdateKVDatas();
        }

        private void AddBlackBoardItem()
        {
            if (m_tmpNodeDataTree == null)
                return;

            var valueTypeDropdown = BlackboardPanel.Q<DropdownField>("variableType");
            string valueType = valueTypeDropdown.value;
            CompareType compareType = (CompareType)Enum.Parse(typeof(CompareType), valueType);
            var variableText = BlackboardPanel.Q<TextField>("variableName");
            string variableName = variableText.text;

            foreach(var blackboardInitData in m_tmpNodeDataTree.m_blackboardInitList)
            {
                if (blackboardInitData.m_key == variableName)
                    return;
            }

            BlackboardKVData kVData = new();
            kVData.m_key = variableName;
            kVData.m_compareType = compareType;

            switch (compareType)
            {
                case CompareType.TString:
                    kVData.m_theStringValue = "";
                    break;
                case CompareType.TBoolean:
                    kVData.m_theBoolValue = false;
                    break;
                case CompareType.TInt:
                    kVData.m_theIntValue = 0;
                    break;
                case CompareType.TFloat:
                    kVData.m_theFloatValue = 0.0f;
                    break;
            }

            m_tmpNodeDataTree.m_blackboardInitList.Add(kVData);
            UpdateKVDatas();
        }

        private void New()
        {
            var nameField = rootVisualElement.Q<TextField>("newbtname");
            string name = nameField.text;
            if (string.IsNullOrEmpty(name))
            {
                DisplayTimedMessage("Please input the name", HelpBoxMessageType.Error, 2000);
                return;
            }
            else
            {
                rootVisualElement.Q<Label>("TreeName").text = name;
                NodeDataTree nodeDataTree = new();
                var rootData = new RootData(1);
                nodeDataTree.m_nodeDataDict[rootData.m_ID] = rootData;
                nodeDataTree.m_rootID = rootData.m_ID;

                // TODO: Check if data should be saved

                CreateNodeGraphByData(nodeDataTree);
                m_tmpNodeDataTree = nodeDataTree;
            }
            UpdateKVDatas();
        }

        private void DisplayTimedMessage(string message, HelpBoxMessageType type, float time)
        {
            var helpBox = new HelpBox(message, type);
            helpBox.name = "helpBox";
            helpBox.style.position = Position.Absolute;
            helpBox.style.left = Length.Percent(50);
            helpBox.style.top = Length.Percent(50);
            rootVisualElement.Add(helpBox);

            m_timer = new Timer(time);
            m_timer.Elapsed += DelayedRemoval;
            m_timer.AutoReset = false;
            m_timer.Start();
        }

        private void DelayedRemoval(object sender, ElapsedEventArgs e)
        {
            var helpBox = rootVisualElement.Q<HelpBox>("helpBox");
            rootVisualElement.Remove(helpBox);
        }

        private void Save()
        {
            if (CheckTree())
            {
                string fileName = rootVisualElement.Q<Label>("TreeName").text;
                int option = 0;
                switch (option)
                {
                    case 0:
                        string path = EditorUtility.SaveFilePanel("Save behavior tree as json", Application.dataPath, fileName + ".json", "json");
                        if (path.Length != 0)
                        {
                            var jsonStream = new JsonStream();
                            jsonStream.Save<NodeDataTree>(m_tmpNodeDataTree, path);
                        }
                        break;
                }
            }
        }

        private bool CheckTree()
        {
            if (m_tmpNodeDataTree == null)
                return false;

            return true;
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

        private GraphViewChange OnGraphViewChange(GraphViewChange graphViewChange)
        {
            // Adjust execution order based on location
            if (graphViewChange.movedElements != null && graphViewChange.movedElements.Count > 0)
            {
                foreach (var element in graphViewChange.movedElements)
                {
                    if (element is GraphNode node)
                    {
                        var parent = GraphicUtils.GetParent(node);
                        
                        if (parent != null)
                        {
                            NodeData parentData = parent.Data;
                            var children = GraphicUtils.GetChildren(parent).ToList();
                            children.Sort((a, b) => GraphicUtils.GetPosition(a).x.CompareTo(GraphicUtils.GetPosition(b).x));
                            
                            bool dirty = false;
                            List<long> ids = new();
                            List<Edge> newEdges = new();
                            for (int i = 0; i < children.Count; ++i)
                            {
                                if (children[i].Data.m_ID != parentData.m_linkedNodeIDs[i])
                                {
                                    dirty = true;
                                }
                                ids.Add(children[i].Data.m_ID);
                                newEdges.Add(children[i].Q<Port>("Parent").connections.First());
                            }

                            if (dirty)
                            {
                                parent.Q<Port>("Children").DisconnectAll();
                                NodeGraphicView.DeleteElements(newEdges);
                                foreach (var child in children)
                                {
                                    NodeGraphicView.CreateEdge(parent.Q<Port>("Children"), child.Q<Port>("Parent"));
                                }

                                parentData.m_linkedNodeIDs = ids;
                            }
                        }
                    }
                }
            }

            return graphViewChange;
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
            Open(path);
        }

        private void Open(string path)
        {
            string extension = Path.GetExtension(path);
            rootVisualElement.Q<Label>("TreeName").text = Path.GetFileNameWithoutExtension(path);
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
                UpdateKVDatas();
            }
        }

        private void UpdateKVDatas()
        {
            if (m_tmpNodeDataTree == null)
                return;

            m_blackboardKVDatas.Clear();
            foreach (var kv in m_tmpNodeDataTree.m_blackboardInitList)
            {
                m_blackboardKVDatas.Add(kv);
            }

            VariablesListView.Rebuild();
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
            Stack<long> ids = new();

            while (q.Count > 0)
            {
                var id = q.Dequeue();
                ids.Push(id);
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

            foreach (var id in ids)
            {
                var childIDs = ID2GraphNode[id].Data.m_linkedNodeIDs;
                if (childIDs.Count > 0)
                {
                    foreach (long childID in childIDs)
                    {
                        NodeGraphicView.CreateEdge(ID2GraphNode[id].Q<Port>("Children"), ID2GraphNode[childID].Q<Port>("Parent"));
                    }
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
                    var elements = UIFactory.CreateElements(node, nodeData, () => { OnSelectedNode(sender, args); });
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

        private void OnEnable()
        {
            EditorApplication.quitting += OnEditorQutting;
        }

        private void OnDisable()
        {
            EditorApplication.quitting -= OnEditorQutting;
            SaveTmpData();
        }

        private void OnEditorQutting()
        {
            SaveTmpData();
        }

        private void OpenTmpData()
        {
            string tmpPath = Path.Combine(Application.temporaryCachePath, "NodeTreeTmpData.json");
            if (File.Exists(tmpPath))
            {
                Open(tmpPath);
            }
        }

        private void SaveTmpData()
        {
            if (m_tmpNodeDataTree != null
               && (m_tmpNodeDataTree.m_nodeDataDict.Count > 0
               || m_tmpNodeDataTree.m_blackboardInitList.Count > 0))
            {
                string tmpPath = Path.Combine(Application.temporaryCachePath, "NodeTreeTmpData.json");
                var jsonStream = new JsonStream();
                jsonStream.Save(m_tmpNodeDataTree, tmpPath);
            }
        }

        public Dictionary<long, GraphNode> ID2GraphNode { get; private set; } = new();
        public VisualElement BlackboardPanel { get; private set; }
        public ListView VariablesListView { get; private set; }
        public GraphicView NodeGraphicView { get; private set; }
        public ScrollView InspectorView { get; private set; }
        private NodeDataTree m_tmpNodeDataTree = null;
        private Timer m_timer;
        private List<BlackboardKVData> m_blackboardKVDatas = new();
    }
}