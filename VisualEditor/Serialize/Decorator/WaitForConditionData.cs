using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class WaitForConditionData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(WaitForConditionData).FullName; } }

        public float m_checkInterval = 0.0f;
        public float m_checkVariance = 0.0f;
        public DelegateData m_conditionData = new();

        [JsonIgnore]
        private WaitForCondition m_waitForCondition;

        public WaitForConditionData() : base()
        {
            m_nodeType = NodeType.Decorator;
        }

        public WaitForConditionData(long id) : base(id)
        {
            m_nodeType = NodeType.Decorator;
        }

        public override Decorator CreateDecorator(Node node)
        {
            if (!m_conditionData.IsDelegateCreated())
                m_conditionData.CreateDelegate();

            m_waitForCondition = new WaitForCondition(m_conditionData.m_singleFrameFunc, m_checkInterval, m_checkVariance, node);
            return m_waitForCondition;
        }

        public override Node GetNode()
        {
            return m_waitForCondition;
        }
    }
}