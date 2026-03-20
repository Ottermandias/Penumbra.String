using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Penumbra.String.Classes;

internal static class SystemConverter
{
    internal sealed class Utf8GamePath : System.Text.Json.Serialization.JsonConverter<Classes.Utf8GamePath>
    {
        public override Classes.Utf8GamePath Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (!reader.TryReadUtf8String(out var text) || !Classes.Utf8GamePath.FromByteString(text, out var gp))
                throw new JsonException($"Could not read {nameof(Classes.Utf8GamePath)}.");

            return gp;
        }

        public override void Write(Utf8JsonWriter writer, Classes.Utf8GamePath value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.Path.Span);
    }

    internal sealed class Utf8RelPath : System.Text.Json.Serialization.JsonConverter<Classes.Utf8RelPath>
    {
        public override Classes.Utf8RelPath Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (!reader.TryReadUtf8String(out var text) || !Classes.Utf8GamePath.FromByteString(text, out var gp))
                throw new JsonException($"Could not read {nameof(Classes.Utf8RelPath)}.");

            return new Classes.Utf8RelPath(gp);
        }

        public override void Write(Utf8JsonWriter writer, Classes.Utf8RelPath value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.Path.Span);
    }

    internal sealed class FullPath : System.Text.Json.Serialization.JsonConverter<Classes.FullPath>
    {
        public override Classes.FullPath Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => new(reader.GetString() ?? throw new JsonException($"Could not read {nameof(Classes.FullPath)}."));

        public override void Write(Utf8JsonWriter writer, Classes.FullPath value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());
    }

    /// <summary> Read the UTF8 string at the current token, unescaped, into an UTF8 string without re-encoding. </summary>
    /// <param name="reader"> The JSON reader. </param>
    /// <param name="text"> On success, the UTF8 string. </param>
    /// <returns> True on success, false if the current token is not a string. </returns>
    public static bool TryReadUtf8String(this ref Utf8JsonReader reader, [NotNullWhen(true)] out CiByteString? text)
    {
        if (reader.TokenType is not JsonTokenType.String)
        {
            text = null;
            return false;
        }

        if (!reader.HasValueSequence)
        {
            text = new CiByteString(reader.ValueSpan, MetaDataComputation.All).Clone();
            return true;
        }

        var seq    = reader.ValueSequence;
        var length = 1;
        foreach (var span in seq)
            length += span.Length;

        var ret = ArrayPool<byte>.Shared.Rent(length);
        try
        {
            ret[^1] = 0;
            var tmp = ret.AsMemory();
            foreach (var span in seq)
            {
                span.CopyTo(tmp);
                tmp = tmp[span.Length..];
            }

            text = new CiByteString(ret.AsSpan(..^1), MetaDataComputation.All).Clone();
            return true;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(ret);
        }
    }
}
