using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace NPSerialization
{
    public static class JsonCustomSettings
    {
        public static void ConfigureJsonInternal()
        {
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new ColorConverter());
                settings.Converters.Add(new Vector2Converter());
                settings.Converters.Add(new Vector3Converter());
                settings.Converters.Add(new Vector4Converter());
                return settings;
            };
        }
    }
    // this must be inside an Editor/ folder
    public static class EditorJsonSettings
    {
        [InitializeOnLoadMethod]
        public static void ApplyCustomConverters()
        {
            JsonCustomSettings.ConfigureJsonInternal();
        }
    }
    public static class RuntimeJsonSettings
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void ApplyCustomConverters()
        {
            JsonCustomSettings.ConfigureJsonInternal();
        }
    }
}