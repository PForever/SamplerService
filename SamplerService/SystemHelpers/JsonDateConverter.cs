#nullable disable
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SamplerService.SystemHelpers;

public class JsonDateConverter : JsonConverter<DateTime>
{
    private const string Format = "dd.MM.yyyy";
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        DateTime.ParseExact(reader.GetString(), Format, CultureInfo.InvariantCulture, DateTimeStyles.None);
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
}