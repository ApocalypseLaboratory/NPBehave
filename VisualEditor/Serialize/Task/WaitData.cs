using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class WaitData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(WaitData).FullName; } }

        [JsonIgnore]
        public Wait m_wait;
        public float m_seconds;

        public WaitData() : base()
        {
            m_nodeType = NodeType.Task;
        }

        public WaitData(long id) : base(id)
        {
            m_nodeType = NodeType.Task;
        }

        public override Task CreateTask()
        {
            m_wait = new Wait(m_seconds);
            return m_wait;
        }

        public override Node GetNode()
        {
            return m_wait;
        }
    }
}
