using Newtonsoft.Json;
using System;

public class SoundSourceConverter : JsonConverter {
    public override bool CanConvert(Type objectType) {
        return objectType == typeof(SoundManager.SoundSource);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
        SoundManager.SoundSource source = (SoundManager.SoundSource)value;
        writer.WriteValue(source.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
        string value = reader.Value?.ToString();
        if (Enum.TryParse(typeof(SoundManager.SoundSource), value, true, out object result)) {
            return result;
        }
        throw new JsonSerializationException($"Unknown fruit value: {value}");
    }
}
