using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuatmmApi.Serialize;

namespace SuatmmApi.Formats.v1.Forms
{
    /// <summary>
    /// Результат оплаты заказа
    /// </summary>
    public class OperationStatus : IPacketContent
    {
        public ResultCode ResultCode { get; set; }



        //public static Dictionary<ResultCodeKind, string> ResultDescription { get; set; } =
        //    new Dictionary<ResultCodeKind, string>
        //    {
        //        [ResultCodeKind.Ok] = "Оплата выполнена успешно",
        //        [ResultCodeKind.WrongCardNumber] = "Неверный номер карты"
        //    };

        #region ISerializing implementation

        public IEnumerable<KeyValuePair<string, string>> GetProperties()
        {
            yield return new KeyValuePair<string, string>("ResultCode", ((int)ResultCode).ToString());
        }

        public void SetProperty(string property, string value)
        {
            switch (property)
            {
                case "ResultCode": ResultCode = (ResultCode)int.Parse(value); break;
                default:
                    break;
            }
        }
        #endregion

    }
}
