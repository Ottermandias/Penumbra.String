using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Penumbra.String.Classes;

internal static class NewtonsoftConverter
{
    /// <summary> Conversion from and to string. </summary>
    internal sealed class Utf8GamePath : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(Classes.Utf8GamePath);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader).ToString();
            return Classes.Utf8GamePath.FromString(token, out var p)
                ? p
                : throw new JsonException($"Could not convert \"{token}\" to {nameof(Classes.Utf8GamePath)}.");
        }

        public override bool CanWrite
            => true;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is Classes.Utf8GamePath p)
                serializer.Serialize(writer, p.ToString());
        }
    }

    /// <summary> Convert from and to string. </summary>
    internal sealed class Utf8RelPath : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(Classes.Utf8RelPath);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader).ToString();
            return Classes.Utf8RelPath.FromString(token, out var p)
                ? p
                : throw new JsonException($"Could not convert \"{token}\" to {nameof(Classes.Utf8RelPath)}.");
        }

        public override bool CanWrite
            => true;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is Classes.Utf8RelPath p)
                serializer.Serialize(writer, p.ToString());
        }
    }

    /// <summary> Convert from and to string. </summary>
    internal sealed class FullPath : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(Classes.FullPath);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader).ToString();
            return new Classes.FullPath(token);
        }

        public override bool CanWrite
            => true;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is Classes.FullPath p)
                serializer.Serialize(writer, p.ToString());
        }
    }
}
