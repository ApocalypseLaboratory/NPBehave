namespace NPSerialization
{
    public interface IStream
    {
        public void Save<T>(T obj, string path);
        public bool Load<T>(string path, out T obj);
    }
}
