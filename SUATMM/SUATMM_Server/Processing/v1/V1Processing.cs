using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuatmmApi.Checkups;
using SUATMM_Server.Processing.v1.Checkups;
using SUATMM_Server.SuatmmDatabase;
using SuatmmApi.Formats;
using SuatmmApi.Formats.v1;
using SuatmmApi.Serialize;
using SuatmmServer.Processing;
using SuatmmApi.Formats.v1.Forms;

namespace SuatmmServer.Processing.v1
{
    public class V1Processing : PacketHandler
    {
        public override Packet<IPacketContent> Handle(Packet<IPacketContent> incoming)
        {
            OperationStatus status;

            try
            {
                //Оплата
                if (incoming.ContentType == typeof(Payment))
                {
                    Out.WriteLine("Payment");

                    status = Handle(incoming.Content as Payment);

                    Out.WriteLine($"Result Code = {status.ResultCode}");

                    return new Packet<IPacketContent>(status) { Version = "v1" };
                }

                //Запрос статуса
                if (incoming.ContentType == typeof(RequestOrderStatus))
                {
                    Out.WriteLine("RequestOrderStatus");

                    status = Handle(incoming.Content as RequestOrderStatus);

                    Out.WriteLine($"Result Code = {status.ResultCode}");

                    return new Packet<IPacketContent>(status) { Version = "v1" };
                }

                //Возврат
                if (incoming.ContentType == typeof(Refund))
                {
                    Out.WriteLine("Refund");

                    status = Handle(incoming.Content as Refund);

                    Out.WriteLine($"Result Code = {status.ResultCode}");

                    return new Packet<IPacketContent>(status) { Version = "v1" };
                }
            }
            catch (Exception ex )
            {
                Out.WriteLine("General Exception");
                Out.WriteLine(ex.Message);
                Out.WriteLine(ex.StackTrace);
                return new Packet<IPacketContent>(new OperationStatus() { ResultCode = ResultCode.GeneralError });
            }
            throw new ArgumentException("Unknown packet content type", "incoming");
        }

        private OperationStatus Handle (Payment payment )
        {
            OperationStatus paymentStatus = new OperationStatus();

            //Предтранзакционные проверки
            foreach (var checkup in PaymentPreCheckups)
            {
                if (!checkup.Perform(payment, paymentStatus))
                    return paymentStatus; //Входные данные не прошли проверку
            }

            //Ищем карту
            Card card = Storage.Cards.Values.FirstOrDefault
                (c => c.Number      == payment.CardNumber
                   && c.CardHolderName  == payment.CardHolderName
                   && c.ExpiryYear  == payment.ExpiryYear
                   && c.ExpiryMonth == payment.ExpiryMonth
                   && c.Cvv         == payment.CVV);

            if (card == null)
            {
                paymentStatus.ResultCode = ResultCode.CardDoesNotExist;
                return paymentStatus;
            }

            foreach (var checkup in CardCheckups)
            {
                if (!checkup.Perform(card, paymentStatus))
                    return paymentStatus; //Карта не прошла проверку
            }

            //Поищем заказ с таким же номером
            lock (Storage.Orders)
            {
                lock (card) //А-ля начинаем транзакцию
                {
                    Order order = Storage.Orders.Values.FirstOrDefault(o => o.OrderId == payment.OrderId);

                    if (order != null)
                    {
                        paymentStatus.ResultCode = ResultCode.DuplicateOrderId;
                        return paymentStatus;
                    }

                    //остаток
                    if (!card.IsUnlimited && card.Rest < payment.AmountKop)
                    {
                        paymentStatus.ResultCode = ResultCode.InsufficientFunds;
                        return paymentStatus;
                    }

                    int Id = Storage.Orders.Count; //новый внутр id

                    Storage.Orders[Id] = new Order { InternalId = Id, Amount = payment.AmountKop, OrderId = payment.OrderId, CardId = card.CardId, Status = OrderStatus.Paid };
                    card.Rest -= (int)payment.AmountKop;

                    paymentStatus.ResultCode = ResultCode.Ok;
                    return paymentStatus;

                }
            }
        }

        /// <summary>
        /// Запрос статуса
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <returns>OperationStatus с кодом статуса или ошибкой</returns>
        private OperationStatus Handle(RequestOrderStatus request)
        {
            OperationStatus orderStatus = new OperationStatus();

            //Предтранзакционные проверки
            foreach (var checkup in RequestOrderStatusPreCheckups)
            {
                if (!checkup.Perform(request, orderStatus))
                    return orderStatus; //Входные данные не прошли проверку
            }

            Order order = Storage.Orders.Values.FirstOrDefault(o => o.OrderId == request.OrderId);

            if (order == null)
            {
                orderStatus.ResultCode = ResultCode.OrderNotFound;
                return orderStatus;
            }

            switch (order.Status)
            {
                case OrderStatus.Paid:
                    orderStatus.ResultCode = ResultCode.OrderPaid;
                    break;
                case OrderStatus.Refunded:
                    orderStatus.ResultCode = ResultCode.OrderRefunded;
                    break;
                case OrderStatus.OnHold:
                    orderStatus.ResultCode = ResultCode.OrderOnHold;
                    break;
                default:
                    orderStatus.ResultCode = ResultCode.GeneralError;
                    break;
            }

            return orderStatus;
        }

        private OperationStatus Handle (Refund request)
        {
            OperationStatus refundStatus = new OperationStatus();

            //Предтранзакционные проверки
            foreach (var checkup in RefundPreCheckups)
            {
                if (!checkup.Perform(request, refundStatus))
                    return refundStatus; //Входные данные не прошли проверку
            }

            Order order = Storage.Orders.Values.FirstOrDefault(o => o.OrderId == request.OrderId);

            if (order == null)
            {
                refundStatus.ResultCode = ResultCode.OrderNotFound;
                return refundStatus;
            }

            lock (order)
            {
                if (order.Status != OrderStatus.Paid)
                {
                    refundStatus.ResultCode = ResultCode.OrderIsNotPaid;
                    return refundStatus;
                }

                Card card = Storage.Cards[order.CardId];

                foreach (var checkup in CardCheckups)
                {
                    if (!checkup.Perform(card, refundStatus))
                        return refundStatus; //Карта не прошла проверку
                }

                lock (card)
                {
                    card.Rest += (int)order.Amount;
                    order.Status = OrderStatus.Refunded;

                    refundStatus.ResultCode = ResultCode.Ok;

                    return refundStatus;
                }
            }
        }

        //Предтранзакционные проверки оплаты
        private List<Checkup<Payment, OperationStatus>> PaymentPreCheckups = new List<Checkup<Payment, OperationStatus>>
            {
                new ValidateCardNumber<Payment, OperationStatus>
                    (accessor: p => p.CardNumber, 
                     onFail: (res) => res.ResultCode = ResultCode.WrongCardNumber ),

                new ValidateCardHolderName<Payment, OperationStatus>
                    (accessor: p => p.CardHolderName,
                    onFail: s => s.ResultCode = ResultCode.WrongCardHolderName),

                new ValidateCvv<Payment, OperationStatus>
                    (accessor: p => p.CVV,
                    onFail: s => s.ResultCode = ResultCode.WrongCvv),

                new ValidateAmount<Payment, OperationStatus>
                    (accessor: p => p.AmountKop,
                    onFail: s => s.ResultCode = ResultCode.WrongAmount),

                new ValidateExpiryYear<Payment, OperationStatus>
                    (accessor: p => p.ExpiryYear,
                    onFail: s => s.ResultCode = ResultCode.WrongYear),

                new ValidateExpiryMonth<Payment, OperationStatus>
                    (accessor: p => p.ExpiryMonth,
                    onFail: s => s.ResultCode = ResultCode.WrongMonth),

                new ValidateOrderId<Payment, OperationStatus>
                    (accessor: p => p.OrderId,
                    onFail: s => s.ResultCode = ResultCode.WrongOrderId),

            };

        private List<Checkup<RequestOrderStatus, OperationStatus>> RequestOrderStatusPreCheckups = new List<Checkup<RequestOrderStatus, OperationStatus>>
        {
            new ValidateOrderId<RequestOrderStatus, OperationStatus>
                (accessor: r => r.OrderId,
                onFail: s => s.ResultCode = ResultCode.WrongOrderId)
        };

        private List<Checkup<Refund, OperationStatus>> RefundPreCheckups = new List<Checkup<Refund, OperationStatus>>
        {
            new ValidateOrderId<Refund, OperationStatus>
                (accessor: r => r.OrderId,
                onFail: s => s.ResultCode = ResultCode.WrongOrderId)
        };

        //Проверки карты
        private List<Checkup<Card, OperationStatus>> CardCheckups = new List<Checkup<Card, OperationStatus>>()
        {
            new CardIsNotExpired<OperationStatus>(onFail: status => status.ResultCode = ResultCode.CardIsExpired),
            new CardIsNotFrozen<OperationStatus>(onFail: status => status.ResultCode = ResultCode.CardIsFrozen)
        };

    }

}
