using System;

namespace NPSerialization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SerializationIDAttribute : Attribute
    {
        public long ID { get; }

        public SerializationIDAttribute(long id)
        {
            ID = id;
        }
    }
}