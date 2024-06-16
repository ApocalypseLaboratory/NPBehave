using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class InvertData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(InvertData).FullName; } }

        [JsonIgnore]
        private Inverter m_inverter;

        public InvertData() : base() 
        {
            m_nodeType = NodeType.Decorator;
        }

        public InvertData(long id) : base(id)
        {
            m_nodeType = NodeType.Decorator;
        }

        public override Decorator CreateDecorator(Node node)
        {
            m_inverter = new Inverter(node);
            return m_inverter;
        }

        public override Node GetNode()
        {
            return m_inverter;
        }
    }
}