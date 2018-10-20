using System;
using Newtonsoft.Json;

namespace LauncherFinal.JsonSerializer
{
    /// <summary>
    /// Конвертирует интерфейс в конкретный тип
    /// </summary>
    /// <typeparam name="TConcrete">Реализация интерфейса</typeparam>
    public class ConcreteTypeConverter<TConcrete> : JsonConverter
        where TConcrete : new()
    {
        public override bool CanConvert(Type objectType)
        {
            //assume we can convert to anything for now
            return objectType == typeof(TConcrete);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            //explicitly specify the concrete type we want to create
            return serializer.Deserialize<TConcrete>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            //use the default serialization - it works fine
            serializer.Serialize(writer, value);
        }
    }
}
