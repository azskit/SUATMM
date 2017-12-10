using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SuatmmApi.Formats;
using SuatmmApi.Serialize;

namespace SuatmmServer.Processing
{
    public abstract class PacketHandler
    {
        public TextWriter Out { get; set; } = TextWriter.Null;

        public abstract Packet<IPacketContent> Handle(Packet<IPacketContent> incoming);

        private static Dictionary<string, PacketHandler> instances = new Dictionary<string, PacketHandler>();

        public static PacketHandler Create(string version)
        {
            if (!instances.ContainsKey(version))
            {
                switch (version)
                {
                    case "v1":
                        instances[version] = new SuatmmServer.Processing.v1.V1Processing();
                        break;
                    default:
                        throw new NotSupportedException("Unsupported version");
                }
            }

            return instances[version];
        }
    }
}
