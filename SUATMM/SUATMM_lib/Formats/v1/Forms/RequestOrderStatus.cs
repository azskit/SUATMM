using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuatmmApi.Serialize;

namespace SuatmmApi.Formats.v1.Forms
{
    /// <summary>
    /// Получение статуса заказа
    /// </summary>
    public class RequestOrderStatus : IPacketContent
    {
        public string OrderId { get; set; } //идентификатор заказа на стороне продавца


        #region IPacketContent implementation

        public IEnumerable<KeyValuePair<string, string>> GetProperties()
        {
            yield return new KeyValuePair<string, string>("order_id", OrderId);
        }

        public void SetProperty(string property, string value)
        {
            switch (property)
            {
                case "order_id": OrderId = value; break;
                default:
                    break;
            }
        }
        #endregion
    }
}
