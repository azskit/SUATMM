using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SUATMM_Server.SuatmmDatabase
{

    enum OrderStatus
    {
        Paid, //Оплачен
        Refunded, //Возвращен
        OnHold //Приостановлен
    }

    class Order
    {
        public int InternalId { get; set; }

        public string OrderId { get; set; }
        public int Amount { get; set; }
        public int Seller { get; set; }
        public OrderStatus Status { get; set; }
        public int CardId { get; internal set; }
    }
}
