using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class ActionData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(ActionData).FullName; } }

        [JsonIgnore]
        private Action m_action;

        public DelegateData m_actionData = new();

        public ActionData() : base()
        {
            m_nodeType = NodeType.Task;
        }

        public ActionData(long id) : base(id)
        {
            m_nodeType = NodeType.Task;
        }

        public override Task CreateTask()
        {
            if (!m_actionData.IsDelegateCreated())
                m_actionData.CreateDelegate();

            if (m_actionData.m_action != null)
            {
                m_action = new Action(m_actionData.m_action);
            }
            else if (m_actionData.m_singleFrameFunc != null)
            {
                m_action = new Action(m_actionData.m_singleFrameFunc);
            }
            else if (m_actionData.m_multiFrameFunc != null)
            {
                m_action = new Action(m_actionData.m_multiFrameFunc);
            }
            else if (m_actionData.m_multiFrameFunc2 != null)
            {
                m_action = new Action(m_actionData.m_multiFrameFunc2);
            }
            else
            {
                m_action = null;
            }

            return m_action;
        }

        public override Node GetNode()
        {
            return m_action;
        }
    }
}