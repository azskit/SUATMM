using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using SuatmmApi.Formats;

namespace SuatmmApi.Serialize
{
    public class XmlPacketSerializer : PacketSerializer

    {
        private IPacketContent DeserializeContent(Type contentType, XmlReader xmlReader)
        {
            IPacketContent content = Activator.CreateInstance(contentType) as IPacketContent;

            while (xmlReader.MoveToNextAttribute())
            {
                content.SetProperty(xmlReader.Name, xmlReader.Value);
            }

            return content;
        }

        public override Packet<T> Deserialize<T>(Stream stream)
        {
            using (XmlReader xmlReader = XmlReader.Create(stream))
            {
                if (xmlReader.ReadToFollowing("packet"))
                {
                    

                    string Version = xmlReader.GetAttribute("version");

                    Format format;

                    if (!Format.SupportedFormats.TryGetValue(Version, out format))
                        throw new NotSupportedException("Not supported format");

                    xmlReader.Read();

                    Type contentType;

                    if (!format.SupportedForms.TryGetValue(xmlReader.Name, out contentType))
                        throw new InvalidDataException("Unknown packet content");

                    Packet<T> packet = new Packet<T>((T)DeserializeContent(contentType, xmlReader))
                    {
                        Version = Version,
                        ContentType = contentType
                    };

                    return packet;
                }
                throw new InvalidDataException("Element \"packet\" not found");
            }
        }

        public override void Serialize<T>(Packet<T> packet, Stream stream)
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings() { Encoding = Encoding.UTF8, CloseOutput = false }))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("packet");
                xmlWriter.WriteAttributeString("version", packet.Version);
                SerializeContent(packet.Content, xmlWriter);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();
            }
        }

        private void SerializeContent(IPacketContent message, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(message.GetType().Name);

            var simples = message.GetProperties();
            if (simples != null)
            {
                foreach (var attribute in message.GetProperties())
                {
                    xmlWriter.WriteAttributeString(attribute.Key, attribute.Value);
                }
            }
            xmlWriter.WriteEndElement();

        }

    }
}
