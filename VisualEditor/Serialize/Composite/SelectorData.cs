using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class SelectorData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(SelectorData).FullName; } }

        [JsonIgnore]
        private Selector m_selector;

        public SelectorData() : base()
        {
            m_nodeType = NodeType.Composite;
        }

        public SelectorData(long id) : base(id)
        {
            m_nodeType = NodeType.Composite;
        }

        public override Composite CreateComposite(Node[] nodes)
        {
            m_selector = new Selector(nodes);
            return m_selector;
        }

        public override Node GetNode()
        {
            return m_selector;
        }
    }
}
