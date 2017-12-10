

using System;
using System.IO;
using SuatmmApi.Formats;

namespace SuatmmApi.Serialize
{
    public class JsonPacketSerializer : PacketSerializer
    {
        public override Packet<T> Deserialize<T>(Stream stream)
        {
            throw new NotImplementedException();
        }

        public override void Serialize<T>(Packet<T> packet, Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}