using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using SuatmmApi;
using SuatmmApi.Formats.v1;
using SuatmmApi.Formats;
using SuatmmApi.Serialize;
using SuatmmApi.Transport;

namespace SUATMM_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = "localhost";
            string port = "13000";

            foreach (string arg in args)
            {
                if (arg.ToLowerInvariant().StartsWith("site="))
                    host = arg.Split('=')[1];
                else if (arg.ToLowerInvariant().StartsWith("port="))
                    port = arg.Split('=')[1];
                else
                {
                    Console.WriteLine("usage: SUATMM_Demo.exe [site=<site>] [port=<port>]");
                    Console.WriteLine("example: SUATMM_Demo.exe site=thisserver.* port=8080");
                    return;
                }
            }

            Uri serverUri = new Uri($"http://{host}:{port}");

            Console.WriteLine($"Сервер {serverUri.ToString()}");

            SuatmmExchange api = new SuatmmExchange(new HttpPacketTransport(serverUri));

            //тестовые карты
            var cards = SampleData.cards;


            string command = "0";

            while (command != "4")
            {
                switch (command)
                {
                    case "1":
                        #region Pay
                        {
                            //Выбор карты
                            int chosenCardId = int.Parse(
                                AskConsole(
                                    question: string.Format("Выбор карты:\r\n{0}", PrintList(cards)),
                                    isAnswerCorrect: answer => int.TryParse(answer, out chosenCardId) && chosenCardId < cards.Count));

                            Card card = cards[chosenCardId];

                            //Суммы
                            int amount = int.Parse(
                                AskConsole(
                                    question: "Сумма заказа (в копейках)",
                                    isAnswerCorrect: answer => int.TryParse(answer, out amount) && amount > 0));
                            //Номер заказа
                            string orderid = AskConsole(
                                    question: "Номер заказа",
                                    isAnswerCorrect: answer => !String.IsNullOrWhiteSpace(answer));

                            //Подытог
                            Console.WriteLine("");
                            Console.WriteLine("Карта оплаты: {0}", card.ToString());
                            Console.WriteLine("Сумма: {0} копеек ({1} рублей)", amount, decimal.Divide(amount, 100));
                            Console.WriteLine("Номер заказа:{0}", orderid);
                            Console.WriteLine("");

                            //Вызов Api
                            if (AskCountinue())
                            {
                                try
                                {
                                    ResultCode code = api.Pay(orderid, card.Number.Replace(" ", ""), card.ExpiryMonth, card.ExpiryYear, card.Cvv, card.CardHolderName, amount);

                                    switch (code)
                                    {
                                        //ok
                                        case ResultCode.Ok:                 Console.WriteLine("Оплата выполнена успешно");                                              break;

                                        //error 
                                        case ResultCode.CardDoesNotExist:   Console.WriteLine("Карта с такими параметрами не найдена");                                 break;
                                        case ResultCode.CardIsExpired:      Console.WriteLine("Срок действия карты закончился");                                        break;
                                        case ResultCode.CardIsFrozen:       Console.WriteLine("Карта заблокирована");                                                   break;
                                        case ResultCode.DuplicateOrderId:   Console.WriteLine("Заказ таким номером уже был обработан ранее");                           break;
                                        case ResultCode.InsufficientFunds:  Console.WriteLine("Недостаточно средств");                                                  break;

                                        //general error
                                        case ResultCode.GeneralError:       Console.WriteLine("Операция не может быть проведена банком. Попробуйте выполнить позже");   break;

                                        default:
                                            Console.WriteLine($"Результат обработки: {code}");
                                            break;
                                    }
                                }
                                catch (ArgumentException ex)
                                {
                                    Console.WriteLine("Неверный параметр");
                                    Console.WriteLine(ex.Message);
                                }

                                catch (Exception ex)
                                {
                                    Console.WriteLine("Неожидаемое исключение");
                                    Console.WriteLine(ex.Message);
                                    throw;
                                }
                            }
                        }
                        break;
                    #endregion

                    case "2":
                        #region GetStatus
                        {
                            string orderid = AskConsole("Номер заказа:", a => !String.IsNullOrWhiteSpace(a));

                            Console.WriteLine("");
                            Console.WriteLine($"Номер заказа: {orderid}");
                            Console.WriteLine("");

                            //Вызов Api
                            if (AskCountinue())
                            {
                                try
                                {
                                    ResultCode code = api.GetStatus(orderid);

                                    switch (code)
                                    {
                                        //status
                                        case ResultCode.OrderNotFound: Console.WriteLine("Заказ с таким номером не был обработан ранее");                        break;
                                        case ResultCode.OrderPaid:     Console.WriteLine("Заказ оплачен");                                                       break;
                                        case ResultCode.OrderOnHold:   Console.WriteLine("Оплата заказа была приостановлена");                                   break;
                                        case ResultCode.OrderRefunded: Console.WriteLine("Заказ аннулирован, средства возвращены плательщику");                  break;
                                        //general error
                                        case ResultCode.GeneralError:  Console.WriteLine("Операция не может быть проведена банком. Попробуйте выполнить позже"); break;

                                        default:
                                            Console.WriteLine($"Результат обработки: {code}");
                                            break;
                                    }
                                }
                                catch (ArgumentException ex)
                                {
                                    Console.WriteLine("Неверный параметр");
                                    Console.WriteLine(ex.Message);
                                }
                                catch (PacketTransportException ex)
                                {
                                    Console.WriteLine("Ошибка соединения с сервером");
                                    Console.WriteLine(ex.InnerException.Message);
                                }

                                catch (Exception ex)
                                {
                                    Console.WriteLine("Неожидаемое исключение");
                                    Console.WriteLine(ex.Message);
                                    throw;
                                }
                            }

                        }
                        break;
                    #endregion

                    case "3":
                        #region Refund
                        {

                            string orderid = AskConsole("Номер заказа для возврата:", a => !String.IsNullOrWhiteSpace(a));

                            Console.WriteLine("");
                            Console.WriteLine($"Номер заказа: {orderid}");
                            Console.WriteLine("");

                            //Вызов Api
                            if (AskCountinue())
                            {
                                try
                                {
                                    ResultCode code = api.Refund(orderid);

                                    switch (code)
                                    {
                                        //ok
                                        case ResultCode.Ok:            Console.WriteLine("Возврат выполнен успешно");                                            break;

                                        //status
                                        case ResultCode.OrderNotFound: Console.WriteLine("Заказ с таким номером не был обработан ранее");                        break;
                                        case ResultCode.OrderIsNotPaid:Console.WriteLine("Заказ был возвращен ранее или приостеновлен");                         break;
                                        case ResultCode.CardIsExpired: Console.WriteLine("Срок действия карты для возврата закончился");                         break;
                                        case ResultCode.CardIsFrozen:  Console.WriteLine("Карта для возврата заблокирована");                                    break;

                                        //general error
                                        case ResultCode.GeneralError:  Console.WriteLine("Операция не может быть проведена банком. Попробуйте выполнить позже"); break;

                                        default:
                                            Console.WriteLine($"Результат обработки: {code}");
                                            break;
                                    }
                                }
                                catch (ArgumentException ex)
                                {
                                    Console.WriteLine("Неверный параметр");
                                    Console.WriteLine(ex.Message);
                                }
                                catch (PacketTransportException ex)
                                {
                                    Console.WriteLine("Ошибка соединения с сервером");
                                    Console.WriteLine(ex.InnerException.Message);
                                }

                                catch (Exception ex)
                                {
                                    Console.WriteLine("Неожидаемое исключение");
                                    Console.WriteLine(ex.Message);
                                    throw;
                                }
                            }

                        }
                        break;
                    #endregion

                    default:
                        break;
                }

                Console.WriteLine("");
                Console.WriteLine("1. Pay (Оплата заказа)");
                Console.WriteLine("2. GetStatus (Проверка статуса заказа)");
                Console.WriteLine("3. Refund (Возврат заказа)");
                Console.WriteLine("4. Выход");
                Console.WriteLine("");

                command = Console.ReadLine();

            }
        }

        public static string AskConsole(string question, Predicate<string> isAnswerCorrect)
        {
            string answer = String.Empty;
            do
            {
                Console.WriteLine(question);
                answer = Console.ReadLine();
            }
            while (!isAnswerCorrect(answer));

            return answer;
        }

        public static bool AskCountinue()
        {
            ConsoleKey? decision = null;
            while (decision != ConsoleKey.Enter && decision != ConsoleKey.Escape)
            {
                Console.WriteLine("Enter - Продолжить, Esc - Отменить\r\n");
                decision = Console.ReadKey(false).Key;
            }
            return decision == ConsoleKey.Enter;
        }

        public static string PrintList<T>(List<T> list)
        {
            StringBuilder sb = new StringBuilder(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                sb.AppendLine($"{i}: {list[i].ToString()}");
            }
            return sb.ToString();
        }
    }
}
