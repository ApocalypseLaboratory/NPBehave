using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace NPSerialization
{
    public class JsonStream : IStream
    {
        public Func<string, string> ReadLocator;
        public Action<string, string> WriteLocator;

        public bool Load<T>(string path, out T obj)
        {
            string jsonString;
            if (ReadLocator != null)
            {
                jsonString = ReadLocator(path);
            }
            else
            {
                jsonString = File.ReadAllText(path);
            }
            obj = JsonConvert.DeserializeObject<T>(jsonString);

            return obj != null;
        }

        public void Save<T>(T obj, string path)
        {
            string jsonString = JsonConvert.SerializeObject(obj); 
            if (WriteLocator != null)
            {
                WriteLocator(path, jsonString);
            }
            else
            {
                File.WriteAllText(path, jsonString);
            }
        }
    }
}
