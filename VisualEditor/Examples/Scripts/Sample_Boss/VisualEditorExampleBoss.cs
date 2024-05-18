using NPSerialization;
using UnityEngine;

namespace NPVisualEditor_Example
{
    public class VisualEditorExampleBoss : MonoBehaviour
    {
        private void Start()
        {
            CreateBoss();
            LoadNodeDataTree();
        }

        private void CreateBoss()
        {
            NPCModel model = new(1001, 100, 20);
            Boss boss = new(model);

            InstanceContext.Instance.RegisterReference(boss, 1001);
        }

        private void LoadNodeDataTree()
        {

        }

        private NodeDataTree m_nodeDataTree;
    }
}