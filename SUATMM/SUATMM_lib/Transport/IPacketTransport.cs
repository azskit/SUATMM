using System;
using SuatmmApi.Serialize;

namespace SuatmmApi.Transport
{
    public interface IPacketTransport
    {
        TResp SendRequest<TReq, TResp>(TReq content) where TReq : IPacketContent where TResp : IPacketContent;
    }

    public class PacketTransportException : Exception
    {
        public PacketTransportException(string message, Exception innerException) : base(message, innerException) { }
    }
}