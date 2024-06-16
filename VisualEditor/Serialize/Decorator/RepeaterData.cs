using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class RepeaterData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(RepeaterData).FullName; } }

        public int m_loopCount = -1;

        [JsonIgnore]
        private Repeater m_repeater;

        public RepeaterData() : base() 
        {
            m_nodeType = NodeType.Decorator;
        }

        public RepeaterData(long id) : base(id)
        {
            m_nodeType = NodeType.Decorator;
        }

        public override Decorator CreateDecorator(Node node)
        {
            m_repeater = new Repeater(m_loopCount, node);
            return m_repeater;
        }

        public override Node GetNode()
        {
            return m_repeater;
        }
    }
}