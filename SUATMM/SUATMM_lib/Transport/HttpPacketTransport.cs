using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using SuatmmApi.Serialize;

namespace SuatmmApi.Transport
{
    public class HttpPacketTransport : IPacketTransport
    {
        private Uri endpoint;
        private string contentType;

        public HttpPacketTransport(Uri endpoint, string contentType = "application/xml")
        {
            this.endpoint = endpoint;
            this.contentType = contentType;
        }


        public TResp SendRequest<TReq, TResp>(TReq content)
            where TReq : IPacketContent
            where TResp : IPacketContent
        {
            HttpWebRequest request = WebRequest.Create(endpoint) as HttpWebRequest;

            request.Method = "POST";
            request.ContentType = contentType;
            request.Timeout = 3000;

            PacketSerializer serializer = PacketSerializer.Create(request.ContentType);

            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(new Packet<TReq>(content) { Version = "v1" }, stream);

                    request.ContentLength = stream.Length;

                    stream.Position = 0;

                    stream.CopyTo(request.GetRequestStream());
                }

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                Stream responseStream = response.GetResponseStream();

                Packet<TResp> responsePacket = serializer.Deserialize<TResp>(responseStream);

                return responsePacket.Content;
            }
            catch (WebException ex)
            {
                throw new PacketTransportException("Ошибка соединения с сервером", ex);
            }
        }
    }
}
