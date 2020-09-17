using Newtonsoft.Json;
using System;

namespace Volo.Abp
{
    public class CollectionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            Type t = objectType.IsNullableType()
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            if (t.IsValueType && t.IsGenericType)
            {
                return true;
            }

            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;
            string text = reader.Value.ToString();
            return JsonConvert.DeserializeObject(text, objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            var t = JsonConvert.SerializeObject(value);
            writer.WriteValue(t);
        }
    }
}
