using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SuatmmApi.Checkups
{
    /// <summary>
    /// Абстрактный класс проверок
    /// </summary>
    /// <typeparam name="TIn">Класс проверяемых объектов</typeparam>
    /// <typeparam name="TOut">Класс объектов с результатами проверки</typeparam>
    public abstract class Checkup<TIn, TOut>
    {
        protected Action<TOut> OnSuccess;
        protected Action<TOut> OnFail;

        /// <summary>
        /// Вызов проверки
        /// </summary>
        /// <param name="inParam">Проверяемый объект</param>
        /// 
        /// <returns></returns>
        public abstract bool Invoke(TIn inParam);

        /// <summary>
        /// Абстрактный класс проверок
        /// </summary>
        /// <param name="onSuccess">Вызывается если проверка выполнена успешно, здесь может быть заполнен объект класса TOut с результатами проверки</param>
        /// <param name="onFail">Вызывается если проверка не выполнена, здесь может быть заполнен объект класса TOut с результатами проверки</param>
        public Checkup(Action<TOut> onSuccess = null, Action<TOut> onFail = null)
        {
            OnSuccess = onSuccess;
            OnFail = onFail;
        }

        public bool Perform(TIn inParam, TOut outParam)
        {
            if (Invoke(inParam))
            {
                OnSuccess?.Invoke(outParam);
                return true;
            }
            else
            {
                OnFail?.Invoke(outParam);
                return false;
            }

        }
    }

    /// <summary>
    /// Проверить номер карты
    /// </summary>
    public class ValidateCardNumber<TIn, TOut> : Checkup<TIn, TOut>
    {
        private Func<TIn, string> accessor;

        /// <summary>
        /// Шаблон номера карты, для простоты допустим, что нам достаточно чтобы это были 15 или 16 цифр
        /// </summary>
        private static Regex validationExpression = new Regex("^\\d{15,16}$");

        /// <summary>
        /// Проверить номер карты
        /// </summary>
        /// <param name="accessor">Получить значение проверяемого номера карты из объекта класса TIn</param>
        public ValidateCardNumber(Func<TIn, string> accessor, Action<TOut> onSuccess = null, Action<TOut> onFail = null) : base(onSuccess, onFail)
        {
            this.accessor = accessor;
        }

        public override bool Invoke(TIn inParam)
        {
            string cardNumber = accessor(inParam);
            return (!String.IsNullOrEmpty(cardNumber) && validationExpression.IsMatch(cardNumber));
        }
    }

    /// <summary>
    /// Проверить CVV
    /// </summary>
    public class ValidateCvv<TIn, TOut> : Checkup<TIn, TOut>
    {
        private Func<TIn, string> accessor;

        private static Regex validationExpression = new Regex("^\\d{3}$");


        public ValidateCvv(Func<TIn, string> accessor, Action<TOut> onSuccess = null, Action<TOut> onFail = null) : base(onSuccess, onFail)
        {
            this.accessor = accessor;
        }

        public override bool Invoke(TIn inParam)
        {
            string cvv = accessor(inParam);
            return (!String.IsNullOrEmpty(cvv) && validationExpression.IsMatch(cvv));
        }
    }


    /// <summary>
    /// Проверить имя держателя карты
    /// </summary>
    public class ValidateCardHolderName<TIn, TOut> : Checkup<TIn, TOut>
    {
        private Func<TIn, string> accessor;

        //Начинается и заканчивается с заглавной латинской буквы, в середине строки допустимы пробелы. 
        //Не сработает если имя состоит из одной буквы, тогда можно добавить альтернативную группу именно для этого случая ^([A-Z][A-Z ]*[A-Z]|[A-Z])$
        private static Regex validationExpression = new Regex("^[A-Z][A-Z ]*[A-Z]$");


        public ValidateCardHolderName(Func<TIn, string> accessor, Action<TOut> onSuccess = null, Action<TOut> onFail = null) : base(onSuccess, onFail)
        {
            this.accessor = accessor;
        }

        public override bool Invoke(TIn inParam)
        {
            string name = accessor(inParam);
            return (!String.IsNullOrEmpty(name) && validationExpression.IsMatch(name));
        }
    }

    /// <summary>
    /// Проверить год срока действия карты
    /// </summary>
    public class ValidateExpiryYear<TIn, TOut> : Checkup<TIn, TOut>
    {
        private Func<TIn, int> accessor;

        public ValidateExpiryYear(Func<TIn, int> accessor, Action<TOut> onSuccess = null, Action<TOut> onFail = null) : base(onSuccess, onFail)
        {
            this.accessor = accessor;
        }

        public override bool Invoke(TIn inParam)
        {
            return accessor(inParam) > 0; //выглядит абсурдно, но какой год можно считать недопустимым?
        }
    }

    /// <summary>
    /// Проверить месяц срока действия карты
    /// </summary>
    public class ValidateExpiryMonth<TIn, TOut> : Checkup<TIn, TOut>
    {
        private Func<TIn, int> accessor;

        public ValidateExpiryMonth(Func<TIn, int> accessor, Action<TOut> onSuccess = null, Action<TOut> onFail = null) : base(onSuccess, onFail)
        {
            this.accessor = accessor;
        }

        public override bool Invoke(TIn inParam)
        {
            int month = accessor(inParam);
            return (month >= 1 && month <= 12); 
        }
    }

    public class ValidateOrderId<TIn, TOut> : Checkup<TIn, TOut>
    {
        private Func<TIn, string> accessor;

        public ValidateOrderId(Func<TIn, string> accessor, Action<TOut> onSuccess = null, Action<TOut> onFail = null) : base(onSuccess, onFail)
        {
            this.accessor = accessor;
        }

        public override bool Invoke(TIn inParam)
        {
            return (!String.IsNullOrWhiteSpace(accessor(inParam)));
        }
    }

    public class ValidateAmount<TIn, TOut> : Checkup<TIn, TOut>
    {
        private Func<TIn, int> accessor;

        public ValidateAmount(Func<TIn, int> accessor, Action<TOut> onSuccess = null, Action<TOut> onFail = null) : base(onSuccess, onFail)
        {
            this.accessor = accessor;
        }

        public override bool Invoke(TIn inParam)
        {
            return (accessor(inParam) > 0);
        }
    }

}
