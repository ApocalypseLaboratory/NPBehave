using NPBehave;
using NPSerialization;
using UnityEngine;

namespace NPVisualEditor_Example
{
    public partial class FunctionsCase : IBehavioral
    {
        public Root GetRoot()
        {
            return m_root;
        }

        public void LoadBehaviorNodes()
        {
            var jsonStream = new JsonStream();
            string path = "sample_functions";
            jsonStream.ReadLocator = (string _path) =>
            {
                var textAsset = Resources.Load(_path) as TextAsset;
                return textAsset.text;
            };
            jsonStream.Load(path, out NodeDataTree m_nodeDataTree);
            m_nodeDataTree.CreateTreeByNodeData();

            m_rootData = m_nodeDataTree.m_nodeDataDict[m_nodeDataTree.m_rootID] as RootData;
            m_root = m_rootData.GetNode() as Root;
            m_blackboard = m_root.Blackboard;
        }

        public void StartBehavior()
        {
            m_root.Start();
        }

        public void StopBehavior()
        {
            m_root.Stop();
        }

        public void UpdateBlackBoard()
        {
        }

        private Root m_root;
        private Blackboard m_blackboard;
        private NodeDataTree m_nodeDataTree;
        private RootData m_rootData;
    }
}