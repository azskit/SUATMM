using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuatmmApi.Formats
{
    public enum ResultCode
    {
        //Общие статусы
        Ok                  = 0,
        WrongCardNumber     = 1000,
        WrongCardHolderName = 1001,
        WrongCvv            = 1002,
        WrongYear           = 1003,
        WrongMonth          = 1004,
        WrongAmount         = 1005,
        WrongOrderId        = 1006,

        //Статусы карты
        CardIsExpired       = 1050,
        CardIsFrozen        = 1051,
        CardDoesNotExist    = 1052,

        //Статусы Payment
        InsufficientFunds   = 1100,
        DuplicateOrderId    = 1101,

        //Статусы GetStatus
        OrderNotFound       = 2000,
        OrderPaid           = 2001,
        OrderRefunded       = 2002,
        OrderOnHold         = 2003,

        //Статусы Refund
        OrderIsNotPaid      = 3000,

        //Ошибка при обработке
        GeneralError = 9999
    }
}
