namespace HealthBeside.Infrastructure;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class DateTimeLocalJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var utcDate = reader.GetDateTime(); 
        return utcDate.ToLocalTime();     
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var localTime = value.Kind == DateTimeKind.Utc ? value.ToLocalTime() : value;
        writer.WriteStringValue(localTime.ToString("yyyy-MM-ddTHH:mm:ss"));
    }
}
