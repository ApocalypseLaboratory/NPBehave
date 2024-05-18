using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class RootData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(RootData).FullName; } }

        [JsonIgnore]
        public Root m_root;

        public RootData() : base()
        {
            m_nodeType = NodeType.Decorator;
        }

        public RootData(long id) : base(id)
        {
            m_nodeType = NodeType.Decorator;
        }

        public override Decorator CreateDecorator(Node node)
        {
            m_root = new Root(node);
            return m_root;
        }

        public override Node GetNode()
        {
            return m_root;
        }
    }
}