using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JsonFx.Json;

namespace GSF.Packet.Json
{
    /*
    class CustomBinder : ISerializationBinder
    {
        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.FullName + ", " + serializedType.Assembly.FullName;
        }
        public Type BindToType(string assemblyName, string typeName)
        {
            if (typeName.StartsWith("GSF"))
                return System.Reflection.Assembly.Load(assemblyName).GetType(typeName);

            return typeof(Dictionary<string, object>);
        }
    }
    */

    public class JsonProtocol : IPacketProtocol
    {
        /*
        protected JsonSerializerSettings JsonSettings { get; set; }

        public JsonProtocol()
        {
            JsonSettings = new JsonSerializerSettings();
            //JsonSettings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            JsonSettings.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            JsonSettings.TypeNameHandling = TypeNameHandling.Objects;
            JsonSettings.Converters.Add(new MyObjectConverter());
        }
        */
        public PacketBase Deserialize(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);

            JsonReaderSettings setting = new JsonReaderSettings();
            setting.TypeHintName = "__type";
            JsonReader reader = new JsonReader(json, setting);
            return (PacketBase)reader.Deserialize();
        }

        public byte[] Serialize<T>(T packet)
            where T : PacketBase
        {
            //var json = JsonConvert.SerializeObject(packet, JsonSettings);

            StringBuilder sb = new StringBuilder();
            JsonWriter writer = new JsonWriter(sb);

            writer.TypeHintName = "__type";
            writer.Write(packet);

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
