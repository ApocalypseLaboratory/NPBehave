using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class TimeMaxData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(TimeMaxData).FullName; } }

        public float m_limit = 0.0f;
        public float m_randomVariation;
        public bool waitForChildButFailOnLimitReached = false;

        [JsonIgnore]
        private TimeMax m_timeMax;

        public TimeMaxData() : base()
        {
            m_nodeType = NodeType.Decorator;
        }

        public TimeMaxData(long id) : base(id)
        {
            m_nodeType = NodeType.Decorator;
        }

        public override Decorator CreateDecorator(Node node)
        {
            m_timeMax = new TimeMax(m_limit, m_randomVariation, waitForChildButFailOnLimitReached, node);
            return m_timeMax;
        }

        public override Node GetNode()
        {
            return m_timeMax;
        }
    }
}