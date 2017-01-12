using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
        protected JsonSerializerSettings JsonSettings { get; set; }

        public JsonProtocol()
        {
            JsonSettings = new JsonSerializerSettings();
            //JsonSettings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            JsonSettings.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            JsonSettings.TypeNameHandling = TypeNameHandling.Objects;
            //JsonSettings.SerializationBinder = new CustomBinder();
        }

        public PacketBase Deserialize(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);

            return (PacketBase)JsonConvert.DeserializeObject(json, JsonSettings);
        }

        public byte[] Serialize<T>(T packet)
            where T : PacketBase
        {
            var json = JsonConvert.SerializeObject(packet, JsonSettings);

            return Encoding.UTF8.GetBytes(json);
        }
    }
}
