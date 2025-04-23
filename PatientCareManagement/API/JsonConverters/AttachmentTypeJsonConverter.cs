using System.Text.Json;
using System.Text.Json.Serialization;
using PatientCareManagement.Core.Enums;

namespace PatientCareManagement.API.JsonConverters
{
    public class AttachmentTypeJsonConverter : JsonConverter<AttachmentType>
    {
        public override AttachmentType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? stringValue = reader.GetString();
                if (Enum.TryParse<AttachmentType>(stringValue, true, out var result))
                {
                    return result;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                int intValue = reader.GetInt32();
                if (Enum.IsDefined(typeof(AttachmentType), intValue))
                {
                    return (AttachmentType)intValue;
                }
            }
            
            throw new JsonException($"Unable to convert \"{reader.GetString()}\" to {typeof(AttachmentType)}");
        }

        public override void Write(Utf8JsonWriter writer, AttachmentType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}