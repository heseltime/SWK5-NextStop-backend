using System.Text.Json;
using System.Text.Json.Serialization;

public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Ensure the token is an object
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject token");
        }

        int hour = 0, minute = 0;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();

                reader.Read(); // Move to the value token
                switch (propertyName)
                {
                    case "hour":
                        hour = reader.GetInt32();
                        break;
                    case "minute":
                        minute = reader.GetInt32();
                        break;
                }
            }
        }

        return new TimeOnly(hour, minute);
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        // Serialize TimeOnly as an object
        writer.WriteStartObject();
        writer.WriteNumber("hour", value.Hour);
        writer.WriteNumber("minute", value.Minute);
        writer.WriteEndObject();
    }
}