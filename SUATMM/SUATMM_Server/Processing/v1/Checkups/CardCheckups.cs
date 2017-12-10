using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuatmmApi.Checkups;
using SUATMM_Server.SuatmmDatabase;
using SuatmmApi.Formats.v1;

namespace SUATMM_Server.Processing.v1.Checkups
{
    /// <summary>
    /// Карта действительна
    /// </summary>
    class CardIsNotExpired<TOut> : Checkup<Card, TOut>
    {
        public override bool Invoke(Card card)
        {
            return (card.ExpiryYear > DateTime.Now.Year || (card.ExpiryYear == DateTime.Now.Year && card.ExpiryMonth >= DateTime.Now.Month));
        }

        public CardIsNotExpired(Action<TOut> onSuccess = null, Action<TOut> onFail = null) : base (onSuccess, onFail) {}
    }

    /// <summary>
    /// Карта не заблокирована
    /// </summary>
    class CardIsNotFrozen<TOut> : Checkup<Card, TOut>
    {
        public override bool Invoke(Card card)
        {
            return true; //Пока что нет такого поля
        }
        public CardIsNotFrozen(Action<TOut> onSuccess = null, Action<TOut> onFail = null) : base (onSuccess, onFail) { }
    }
}
