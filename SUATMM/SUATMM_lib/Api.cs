using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using SuatmmApi.Checkups;
using SuatmmApi.Formats;
using SuatmmApi.Formats.v1;
using SuatmmApi.Formats.v1.Forms;
using SuatmmApi.Serialize;
using SuatmmApi.Transport;

namespace SuatmmApi
{
    /// <summary>
    /// API вызова сервисов процессинга банка
    /// </summary>
    public class SuatmmExchange
    {
        IPacketTransport transport;

        /// <summary>
        /// Оплата заказа
        /// </summary>
        /// <param name="order_id">идентификатор заказа на стороне продавца</param>
        /// <param name="card_number">номер карты</param>
        /// <param name="expiry_month">месяц срока действия карты</param>
        /// <param name="expiry_year">год срока действия карты</param>
        /// <param name="cvv">проверочный код</param>
        /// <param name="cardholder_name">имя держателя карты</param>
        /// <param name="amount_kop">сумма заказа в копейках</param>
        /// <returns></returns>
        public ResultCode Pay(string order_id, string card_number, int expiry_month, int expiry_year, string cvv, string cardholder_name, int amount_kop)
        {
            #region Проверка входящих параметров
            //ненулеватость
            if (String.IsNullOrWhiteSpace(order_id))
                throw new ArgumentNullException(nameof(order_id));

            if (String.IsNullOrWhiteSpace(card_number))
                throw new ArgumentNullException(nameof(card_number));

            if (String.IsNullOrWhiteSpace(cvv))
                throw new ArgumentNullException(nameof(cvv));

            if (String.IsNullOrWhiteSpace(cardholder_name))
                throw new ArgumentNullException(nameof(cardholder_name));

            //валидность
            if (!new ValidateCardNumber<object, object>(o => card_number).Invoke(null))
                throw new ArgumentException("Неверный номер карты", nameof(card_number));

            if (!new ValidateExpiryMonth<object, object>(o => expiry_month).Invoke(null))
                throw new ArgumentException("Неверный месяц срока действия", nameof(expiry_month));

            if (!new ValidateExpiryYear<object, object>(o => expiry_year).Invoke(null))
                throw new ArgumentException("Неверный год срока действия", nameof(expiry_year));

            if (!new ValidateCvv<object, object>(o => cvv).Invoke(null))
                throw new ArgumentException("Неверный код CVV", nameof(cvv));

            if (!new ValidateCardHolderName<object, object>(o => cardholder_name).Invoke(null))
                throw new ArgumentException("Неверное имя держателя карты", nameof(cardholder_name));

            if (!new ValidateAmount<object, object>(o => amount_kop).Invoke(null))
                throw new ArgumentException("Неверная сумма заказа", nameof(amount_kop));

            #endregion


            Payment payment = new Payment()
            {
                OrderId        = order_id,
                AmountKop      = amount_kop,
                CardHolderName = cardholder_name,
                CardNumber     = card_number,
                CVV            = cvv,
                ExpiryMonth    = expiry_month,
                ExpiryYear     = expiry_year
            };

            OperationStatus response = transport.SendRequest<Payment, OperationStatus>(payment);

            return response.ResultCode;
        }

        /// <summary>
        /// Проверка статуса заказа
        /// </summary>
        /// <param name="order_id">идентификатор заказа на стороне продавца</param>
        /// <returns>Код статуса заказа или код ошибки</returns>
        public ResultCode GetStatus(string order_id)
        {
            #region Проверка входящих параметров
            //ненулеватость
            if (String.IsNullOrWhiteSpace(order_id))
                throw new ArgumentNullException(nameof(order_id));
            #endregion

            RequestOrderStatus request = new RequestOrderStatus { OrderId = order_id };

            OperationStatus response = transport.SendRequest<RequestOrderStatus, OperationStatus>(request);

            return response.ResultCode;
        }

        /// <summary>
        /// Возврат заказа
        /// </summary>
        /// <param name="order_id">идентификатор заказа на стороне продавца</param>
        /// <returns>Код Ок или код ошибки</returns>
        public ResultCode Refund(string order_id)
        {
            #region Проверка входящих параметров
            //ненулеватость
            if (String.IsNullOrWhiteSpace(order_id))
                throw new ArgumentNullException(nameof(order_id));
            #endregion

            Refund request = new Refund { OrderId = order_id };

            OperationStatus response = transport.SendRequest<Refund, OperationStatus>(request);

            return response.ResultCode;
        }

        /// <summary>
        /// Создает новый объект API с указанным транспортом пакетов сообщений
        /// </summary>
        /// <param name="transport">Транспорт пакетов сообщений, например HttpPacketTransport</param>
        public SuatmmExchange(IPacketTransport transport)
        {
            if (transport == null)
                throw new ArgumentNullException(nameof(transport));

            this.transport = transport;
        }

    }
}
