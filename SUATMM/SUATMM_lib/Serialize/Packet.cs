using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using SuatmmApi.Formats.v1;
using SuatmmApi.Serialize;

namespace SuatmmApi.Serialize
{
    public class Packet<T> where T : IPacketContent
    {
        public string Version { get; set; }
        public Type ContentType { get; set; }
        public T Content { get; set; }

        public Packet(T content)
        {
            Content = content;
            ContentType = content.GetType();
        }
    }



}
