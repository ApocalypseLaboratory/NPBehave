using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{ 
    public class RandomData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(RandomData).FullName; } }

        public float m_probability;
        [JsonIgnore]
        private Random m_random;

        public RandomData() : base() 
        {
            m_nodeType = NodeType.Decorator;
        }

        public RandomData(long id) : base(id) 
        {
            m_nodeType = NodeType.Decorator;
        }

        public override Decorator CreateDecorator(Node node)
        {
            m_random = new Random(m_probability, node);
            return m_random;
        }

        public override Node GetNode()
        {
            return m_random;
        }
    }
}