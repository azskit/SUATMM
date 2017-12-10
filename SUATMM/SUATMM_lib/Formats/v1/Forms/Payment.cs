using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuatmmApi.Serialize;

namespace SuatmmApi.Formats.v1.Forms
{
    /// <summary>
    /// Оплата заказа
    /// </summary>
    public class Payment : IPacketContent
    {
        public string OrderId        { get; set; } //идентификатор заказа на стороне продавца
        public string CardNumber     { get; set; } //номер банковской карты
        public int    ExpiryMonth    { get; set; } //срок действия карты месяц
        public int    ExpiryYear     { get; set; } //срок действия карты год
        public string CVV            { get; set; } //верификационный код (3 цифры)
        public string CardHolderName { get; set; } //имя владельца карты 
        public int    AmountKop      { get; set; } //сумма в копейках


        #region IPacketContent implementation

        public IEnumerable<KeyValuePair<string, string>> GetProperties()
        {
            yield return new KeyValuePair<string, string>("order_id"       , OrderId);
            yield return new KeyValuePair<string, string>("card_number"    , CardNumber);
            yield return new KeyValuePair<string, string>("expiry_month"   , ExpiryMonth.ToString());
            yield return new KeyValuePair<string, string>("expiry_year"    , ExpiryYear.ToString());
            yield return new KeyValuePair<string, string>("cvv"            , CVV);
            yield return new KeyValuePair<string, string>("cardholder_name", CardHolderName);
            yield return new KeyValuePair<string, string>("amount_kop"     , AmountKop.ToString());
        }

        public void SetProperty(string property, string value)
        {
            switch (property)
            {
                case "order_id"       : OrderId        =            value; break;
                case "card_number"    : CardNumber     =            value; break;
                case "expiry_month"   : ExpiryMonth    = int.Parse( value); break; //Предполагается, что в строке действительно целое число
                case "expiry_year"    : ExpiryYear     = int.Parse( value); break;
                case "cvv"            : CVV            =            value; break;
                case "cardholder_name": CardHolderName =            value; break;
                case "amount_kop"     : AmountKop      = int.Parse( value); break;
                default               :
                    break;
            }
        }
        #endregion
    }}
