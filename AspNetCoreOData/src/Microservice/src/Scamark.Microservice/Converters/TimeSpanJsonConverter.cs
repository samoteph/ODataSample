using System.Text.Json;
using System.Text.Json.Serialization;

namespace Scamark.Microservice.Converters;

public class TimeSpanJsonConverter : JsonConverter<TimeSpan>
{
    /////// <summary>
    /////// Format: Days.Hours:Minutes:Seconds:Milliseconds
    /////// </summary>
    ////public const string TimeSpanFormatString = @"d\.hh\:mm\:ss\:FFF";

    public TimeSpanJsonConverter()
    {
    }

    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        ////if (TimeSpan.TryParseExact(value, TimeSpanFormatString, null, out var parsedTimeSpan) == true)
        ////{
        ////    return parsedTimeSpan;
        ////}

        return System.Xml.XmlConvert.ToTimeSpan(value);
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        var str = System.Xml.XmlConvert.ToString(value);
        writer.WriteStringValue(str);
    }
}
