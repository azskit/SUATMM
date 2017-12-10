using System;
using System.Collections.Generic;
using System.IO;
using SuatmmApi.Formats;

namespace SuatmmApi.Serialize
{
    public abstract class PacketSerializer
    {
        public abstract Packet<TPacketContent> Deserialize<TPacketContent>(Stream stream) where TPacketContent : IPacketContent;
        public abstract void Serialize<TPacketContent>(Packet<TPacketContent> packet, Stream stream) where TPacketContent : IPacketContent;

        private static Dictionary<string, PacketSerializer> instances = new Dictionary<string, PacketSerializer>();

        public static PacketSerializer Create(string contentType)
        {
            if (instances.ContainsKey(contentType))
                return instances[contentType];
            
            switch (contentType)
            {
                case "application/xml":
                    instances[contentType] = new XmlPacketSerializer();
                    break;
                case "application/json":
                    instances[contentType] = new JsonPacketSerializer();
                    break;
                default:
                    return null;
            }

            return instances[contentType];

        }
    }
}