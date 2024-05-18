using Newtonsoft.Json;
using NPBehave;
using NPSerialization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace NPVisualEditor_Example
{
    public class VisualEditorExampleSerialize : MonoBehaviour
    {
        private void Start()
        {
            CreateNodeDataTree();
        }

        private void CreateNodeDataTree()
        {
            m_nodeDataTree = new NodeDataTree();

            var kvData = new BlackboardKVData()
            {
                m_key = "hp",
                m_theIntValue = 100,
                m_compareType = CompareType.TInt
            };
            m_nodeDataTree.m_blackboardInitList.Add(kvData);
            
            var rootData = new RootData(ID);
            m_nodeDataTree.m_nodeDataDict[rootData.m_ID] = rootData;
            m_nodeDataTree.m_rootID = rootData.m_ID;

            var serviceData = new ServiceData(ID);
            serviceData.m_interval = 0.125f;
            serviceData.m_delegateData.SetDelegate(UpdateBlackboard);
            NodeDataUtils.AddChild(rootData, serviceData);
            m_nodeDataTree.m_nodeDataDict[serviceData.m_ID] = serviceData;

            var selectorData = new SelectorData(ID);
            NodeDataUtils.AddChild(serviceData, selectorData);
            m_nodeDataTree.m_nodeDataDict[selectorData.m_ID] = selectorData;

            var blackboardConditionData = new BlackboardConditionData(ID);
            blackboardConditionData.m_blackboardData = new BlackboardKVData()
            {
                m_key = "playerDistance",
                m_compareType = CompareType.TFloat,
                m_theFloatValue = 7.5f
            };
            blackboardConditionData.m_operator = Operator.IS_SMALLER;
            blackboardConditionData.m_stopsOnChange = Stops.IMMEDIATE_RESTART;
            NodeDataUtils.AddChild(selectorData, blackboardConditionData);
            m_nodeDataTree.m_nodeDataDict[blackboardConditionData.m_ID] = blackboardConditionData;

            var sequenceData1 = new SequenceData(ID);
            NodeDataUtils.AddChild(blackboardConditionData, sequenceData1);
            m_nodeDataTree.m_nodeDataDict[sequenceData1.m_ID] = sequenceData1;

            var actionData1 = new ActionData(ID);
            actionData1.m_actionData.SetDelegate(SetColor);
            m_nodeDataTree.m_nodeDataDict[actionData1.m_ID] = actionData1;

            var actionData2 = new ActionData(ID);
            actionData2.m_actionData.SetDelegate(Move);
            m_nodeDataTree.m_nodeDataDict[actionData2.m_ID] = actionData2;

            NodeDataUtils.AddChildren(sequenceData1, actionData1, actionData2);

            var sequenceData2 = new SequenceData(ID);
            NodeDataUtils.AddChild(selectorData, sequenceData2);
            m_nodeDataTree.m_nodeDataDict[sequenceData2.m_ID] = sequenceData2;

            var actionData3 = new ActionData(ID);
            actionData3.m_actionData.SetDelegate(SetColor);
            m_nodeDataTree.m_nodeDataDict[actionData3.m_ID] = actionData3;

            var waitUtilStoppedData = new WaitUtilStoppedData(ID);
            m_nodeDataTree.m_nodeDataDict[waitUtilStoppedData.m_ID] = waitUtilStoppedData;

            NodeDataUtils.AddChildren(sequenceData2, actionData3, waitUtilStoppedData);
        }

        private static void UpdateBlackboard()
        {

        }

        private static void SetColor()
        {

        }

        private static Action.Result Move(bool shouldCancel)
        {
            if (!shouldCancel)
            {
                return Action.Result.PROGRESS;
            }
            else
            {
                return Action.Result.FAILED;
            }
        }

        private void ShowSerializationContent()
        {
            string jsonString = JsonConvert.SerializeObject(m_nodeDataTree, Formatting.Indented);
            m_content.text = jsonString;
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_content.GetComponent<RectTransform>());
        }

        public void TestSerialize()
        {
            ShowSerializationContent();
            var jsonStream = new JsonStream();
            string path = Path.Combine(Application.dataPath, "test_tree.json");
            jsonStream.Save(m_nodeDataTree, path);
        }

        public void TestDeserialize()
        {
            var jsonStream = new JsonStream();
            string path = Path.Combine(Application.dataPath, "test_tree.json");
            jsonStream.Load(path, out NodeDataTree nodeDataTree);
            nodeDataTree.CreateTreeByNodeData();

            var rootData = nodeDataTree.m_nodeDataDict[nodeDataTree.m_rootID] as RootData;
            rootData.GetNode().Start();
        }

        private long m_id = 0;

        private long ID
        {
            get 
            {
                m_id++;
                return m_id; 
            }
        }

        private bool AlwaysTrue()
        {
            return true;
        }

        private NodeDataTree m_nodeDataTree;
        public Text m_content;
    }
}
