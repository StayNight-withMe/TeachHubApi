using Core.Common.Types.HashId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace infrastructure.Utils.HashIdConverter
{
    public class HashidJsonConverter : JsonConverter<Hashid>
    {
        public override Hashid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            return new Hashid(HashidsHelper.Decode(stringValue));
        }

        public override void Write(Utf8JsonWriter writer, Hashid value, JsonSerializerOptions options)
        {
            //writer.WriteStringValue(HashidsHelper.Encode(value.Value));
            var encoded = HashidsHelper.Encode(value.Value);
            writer.WriteStringValue(encoded);
        }


        public override Hashid ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            return string.IsNullOrEmpty(stringValue) ? new Hashid(0) : new Hashid(HashidsHelper.Decode(stringValue));
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, Hashid value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(HashidsHelper.Encode(value.Value));
        }

    }
}
