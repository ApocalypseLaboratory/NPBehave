using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System;
using UnityEngine;
using UnityEditor;

namespace NPSerialization
{
    /// <summary>
    /// See https://stackoverflow.com/questions/8030538/how-to-implement-custom-jsonconverter-in-json-net
    /// </summary>
    public class NodeDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return System.Attribute.GetCustomAttributes(objectType).Any(v => v is KnownTypeAttribute);
        }

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // load the object 
            JObject jObject = JObject.Load(reader);

            // take custom attributes on the type
            Attribute[] attrs = Attribute.GetCustomAttributes(objectType);

            // take the names of elements from json data
            HashSet<string> jObjectKeys = GetKeys(jObject);

            JToken valueToken = jObject.GetValue("TYPE_NAME_FOR_SERIALIZATION");

            if (valueToken != null && valueToken.Type == JTokenType.String)
            {
                string typeName = valueToken.Value<string>();
                Type nodeDataType = null;

                // trying to find the right type
                foreach (var attr in attrs.OfType<KnownTypeAttribute>())
                {
                    Type knownType = attr.Type;
                    if (!objectType.IsAssignableFrom(knownType))
                        continue;

                    if (knownType.FullName == typeName)
                    {
                        nodeDataType = knownType;
                        break;
                    }
                }
                if (nodeDataType != null)
                {
                    object target = Activator.CreateInstance(nodeDataType);
                    using (JsonReader jObjectReader = CopyReaderForObject(reader, jObject))
                    {
                        serializer.Populate(jObjectReader, target);
                    }
                    return target;
                }
            }
            throw new SerializationException($"Could not serialize to KnownTypes and assign to base class {objectType} reference");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private HashSet<string> GetKeys(JObject obj)
        {
            return new HashSet<string>(((IEnumerable<KeyValuePair<string, JToken>>)obj).Select(k => k.Key));
        }

        public static JsonReader CopyReaderForObject(JsonReader reader, JObject jObject)
        {
            JsonReader jObjectReader = jObject.CreateReader();
            jObjectReader.Culture = reader.Culture;
            jObjectReader.DateFormatString = reader.DateFormatString;
            jObjectReader.DateParseHandling = reader.DateParseHandling;
            jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
            jObjectReader.FloatParseHandling = reader.FloatParseHandling;
            jObjectReader.MaxDepth = reader.MaxDepth;
            jObjectReader.SupportMultipleContent = reader.SupportMultipleContent;
            return jObjectReader;
        }
    }

    // Solutions to prevent serialization errors. Seen in https://forum.unity.com/threads/jsonserializationexception-self-referencing-loop-detected.1264253/
    // Newtonsoft struggles serializing structs like Vector3 because it has a property .normalized
    // that references Vector3, and thus entering a self-reference loop throwing circular reference error.
    // Add the class to BootstrapJsonParser
    public class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            JObject obj = new JObject() { ["r"] = value.r, ["g"] = value.g, ["b"] = value.b, ["a"] = value.a };
            obj.WriteTo(writer);
        }
        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            return new Color((float)obj.GetValue("r"), (float)obj.GetValue("g"), (float)obj.GetValue("b"), (float)obj.GetValue("a"));
        }
    }
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            JObject obj = new JObject() { ["x"] = value.x, ["y"] = value.y };
            obj.WriteTo(writer);
        }
        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            return new Vector2((float)obj.GetValue("x"), (float)obj.GetValue("y"));
        }
    }
    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            JObject obj = new JObject() { ["x"] = value.x, ["y"] = value.y, ["z"] = value.z };
            obj.WriteTo(writer);
        }
        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            return new Vector3((float)obj.GetValue("x"), (float)obj.GetValue("y"), (float)obj.GetValue("z"));
        }
    }
    public class Vector4Converter : JsonConverter<Vector4>
    {
        public override void WriteJson(JsonWriter writer, Vector4 value, JsonSerializer serializer)
        {
            JObject obj = new JObject() { ["x"] = value.x, ["y"] = value.y, ["z"] = value.z, ["w"] = value.w };
            obj.WriteTo(writer);
        }
        public override Vector4 ReadJson(JsonReader reader, Type objectType, Vector4 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            return new Vector4((float)obj.GetValue("x"), (float)obj.GetValue("y"), (float)obj.GetValue("z"), (float)obj.GetValue("w"));
        }
    }
}