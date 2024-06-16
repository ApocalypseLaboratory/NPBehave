using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class TimeMinData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(TimeMinData).FullName; } }

        public float m_limit = 0.0f;
        public float m_randomVariation;
        public bool m_waitOnFailure = false;

        [JsonIgnore]
        private TimeMin m_timeMin;

        public TimeMinData() : base() 
        {
            m_nodeType = NodeType.Decorator;
        }

        public TimeMinData(long id) : base(id)
        {
            m_nodeType = NodeType.Decorator;
        }

        public override Decorator CreateDecorator(Node node)
        {
            m_timeMin = new TimeMin(m_limit, m_randomVariation, m_waitOnFailure, node);
            return m_timeMin;
        }

        public override Node GetNode()
        {
            return m_timeMin;
        }
    }
}