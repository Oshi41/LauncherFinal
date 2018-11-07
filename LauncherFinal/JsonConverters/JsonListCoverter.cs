using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LauncherFinal.JsonConverters
{
    class JsonListCoverter<TBase, TDerived> : JsonConverter
        where TDerived : TBase, new()
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //use the default serialization - it works fine
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var list = new List<TBase>();

            if (reader.TokenType != JsonToken.StartArray)
                return list;

            reader.Read();

            while (reader.TokenType != JsonToken.EndArray)
            {
                var obj = JObject.Load(reader);

                var element = obj.ToObject<TDerived>();

                list.Add(element);

                reader.Read();
            }

            return list;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IList).IsAssignableFrom(objectType);
        }
    }
}

