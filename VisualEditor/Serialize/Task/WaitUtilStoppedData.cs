using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class WaitUtilStoppedData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(WaitUtilStoppedData).FullName; } }

        [JsonIgnore]
        private WaitUntilStopped m_waitUtilStopped;
        public bool sucessWhenStopped = false;

        public WaitUtilStoppedData() : base()
        {
            m_nodeType = NodeType.Task;
        }

        public WaitUtilStoppedData(long id) : base(id)
        {
            m_nodeType = NodeType.Task;
        }

        public override Task CreateTask()
        {
            m_waitUtilStopped = new WaitUntilStopped(sucessWhenStopped);
            return m_waitUtilStopped;
        }

        public override Node GetNode()
        {
            return m_waitUtilStopped;
        }
    }
}
