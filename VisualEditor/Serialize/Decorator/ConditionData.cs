using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class ConditionData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(ConditionData).FullName; } }

        public float m_checkInterval;
        public float m_checkVariance;
        public Stops m_stopsOnChange;
        public DelegateData m_conditionData = new();

        [JsonIgnore]
        private Condition m_condition;

        public ConditionData() : base() 
        {
            m_nodeType = NodeType.Decorator;
        }

        public ConditionData(long id) : base(id)
        {
            m_nodeType = NodeType.Decorator;
        }

        public override Decorator CreateDecorator(Node node)
        {
            if (string.IsNullOrEmpty(m_conditionData.SingleFrameFuncString))
                return null;

            if (!m_conditionData.IsDelegateCreated())
                m_conditionData.CreateDelegate();

            m_condition = new Condition(m_conditionData.m_singleFrameFunc, m_stopsOnChange, m_checkInterval, m_checkVariance, node);
            return m_condition;
        }

        public override Node GetNode()
        {
            return m_condition;
        }
    }
}