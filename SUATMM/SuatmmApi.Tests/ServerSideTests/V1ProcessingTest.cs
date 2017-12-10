using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SUATMM_Demo;
using SuatmmApi.Formats;
using SuatmmApi.Formats.v1;
using SuatmmApi.Formats.v1.Forms;
using SuatmmApi.Serialize;
using SuatmmServer.Processing.v1;
using Xunit;

namespace SuatmmApi.Tests
{
    public class V1ProcessingTest
    {
        [Fact]
        public void Handle_Payment_ReturnsOk()
        {
            V1Processing processing = new V1Processing();
            Card card = SampleData.cards.Find(c => c.Name == "Безлимитная карта");

            Payment payment = new Payment { OrderId = "1", AmountKop = 100, CardHolderName = card.CardHolderName, CardNumber = card.Number, CVV = card.Cvv, ExpiryMonth = card.ExpiryMonth, ExpiryYear = card.ExpiryYear };

            OperationStatus answer = processing.Handle(new Packet<IPacketContent>(payment) { Version = "v1" }).Content as OperationStatus;


            Assert.Equal(ResultCode.Ok, answer.ResultCode);
        }

        [Fact]
        public void Handle_RequestonOrderStatus_ReturnsOrderPaid()
        {
            V1Processing processing = new V1Processing();

            RequestOrderStatus request = new RequestOrderStatus { OrderId = "1" };

            OperationStatus answer = processing.Handle(new Packet<IPacketContent>(request) { Version = "v1" }).Content as OperationStatus;


            Assert.Equal(ResultCode.OrderPaid, answer.ResultCode);
        }

        [Fact]
        public void Handle_Refund_ReturnsOrderPaid()
        {
            V1Processing processing = new V1Processing();

            Refund request = new Refund { OrderId = "1" };

            OperationStatus answer = processing.Handle(new Packet<IPacketContent>(request) { Version = "v1" }).Content as OperationStatus;


            Assert.Equal(ResultCode.Ok, answer.ResultCode);
        }


        [Fact]
        public void Handle_PaymentIsTooExpensive_ReturnsInsifficientFunds()
        {
            V1Processing processing = new V1Processing();
            Card card = SampleData.cards.Find(c => c.Name == "Карта с лимитом 100р");

            Payment payment = new Payment { OrderId = "2", AmountKop = 10001, CardHolderName = card.CardHolderName, CardNumber = card.Number.Replace(" ", ""), CVV = card.Cvv, ExpiryMonth = card.ExpiryMonth, ExpiryYear = card.ExpiryYear };

            OperationStatus answer = processing.Handle(new Packet<IPacketContent>(payment) { Version = "v1" }).Content as OperationStatus;


            Assert.Equal(ResultCode.InsufficientFunds, answer.ResultCode);
        }

        [Fact]
        public void Handle_RequestonOrderStatus_ReturnsOrderNotFound()
        {
            V1Processing processing = new V1Processing();

            RequestOrderStatus request = new RequestOrderStatus { OrderId = "2" };

            OperationStatus answer = processing.Handle(new Packet<IPacketContent>(request) { Version = "v1" }).Content as OperationStatus;


            Assert.Equal(ResultCode.OrderNotFound, answer.ResultCode);
        }

        [Fact]
        public void Handle_Refund_ReturnsOrderNotFound()
        {
            V1Processing processing = new V1Processing();

            Refund request = new Refund { OrderId = "2" };

            OperationStatus answer = processing.Handle(new Packet<IPacketContent>(request) { Version = "v1" }).Content as OperationStatus;


            Assert.Equal(ResultCode.OrderNotFound, answer.ResultCode);
        }

        [Theory]
        [InlineData("Карта с неверным номером"          , "1", 100, ResultCode.WrongCardNumber    )]
        [InlineData("Карта с неверным Cvv"              , "1", 100, ResultCode.WrongCvv           )]
        [InlineData("Карта с неверным именем держателя" , "1", 100, ResultCode.WrongCardHolderName)]
        [InlineData("Безлимитная карта"                 , "1", 0  , ResultCode.WrongAmount        )]
        public void Pay_InvalidParameters_CauseArgumentException(string cardName, string orderId, int amount, ResultCode expectedCode)
        {
            V1Processing processing = new V1Processing();
            Card card = SampleData.cards.Find(c => c.Name == cardName);

            Payment payment = new Payment { OrderId = orderId, AmountKop = amount, CardHolderName = card.CardHolderName, CardNumber = card.Number.Replace(" ", ""), CVV = card.Cvv, ExpiryMonth = card.ExpiryMonth, ExpiryYear = card.ExpiryYear };

            OperationStatus answer = processing.Handle(new Packet<IPacketContent>(payment) { Version = "v1" }).Content as OperationStatus;


            Assert.Equal(expectedCode, answer.ResultCode);
        }

    }
}
