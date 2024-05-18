using NPBehave;
using NPSerialization;
using System.IO;
using UnityEngine;

namespace NPVisualEditor_Example
{
    public partial class Boss : ISampleNPC, IBehavoiral
    {
        public NPCModel NPCModel { get; set; }

        public Root GetRoot()
        {
            return m_root;
        }

        public void LoadBehavoirNodes()
        {
            var jsonStream = new JsonStream();
            string path = "sample_boss";
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

        public void StartBehavoir()
        {
            m_root.Start();
        }

        public void StopBehavoir()
        {
            m_root.Stop();
        }

        public void UpdateBlackBoard()
        {
            m_blackboard["I_HP"] = m_npc.HP;
            m_blackboard["I_POWER"] = m_npc.Power;

            m_blackboard["B_NEAR_CHARACTER"] = false;
            var characterGO = GameObject.FindGameObjectWithTag("CHARACTER");
            if (characterGO != null)
            {
                var model = characterGO.GetComponent<NPCModel>();
                if (model != m_npc)
                {
                    if (Vector3.Distance(model.Position, m_npc.Position) < 5)
                    {
                        m_blackboard["F_LAST_FOUND_CHARACTER"] = Time.time;
                        m_blackboard["B_NEAR_CHARACTER"] = true;
                    }
                }
            }
        }

        private Root m_root;
        private Blackboard m_blackboard;
        private NodeDataTree m_nodeDataTree;
        private RootData m_rootData;
    }
}