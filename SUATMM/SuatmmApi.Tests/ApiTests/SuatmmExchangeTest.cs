using System;
using System.IO;
using Moq;
using SUATMM_Demo;
using SuatmmApi;
using SuatmmApi.Formats;
using SuatmmApi.Formats.v1;
using SuatmmApi.Formats.v1.Forms;
using SuatmmApi.Serialize;
using SuatmmApi.Transport;
using Xunit;

namespace SuatmmApi.Tests
{
    public class SuatmmExchangeTest
    {
        [Fact]
        public void Pay_ValidCard_ReturnsOk()
        {
            var mock = new Mock<IPacketTransport>();

            mock.Setup(tp => tp.SendRequest<Payment, OperationStatus>(It.IsAny<Payment>()))
                .Returns(new OperationStatus { ResultCode = ResultCode.Ok });

            SuatmmExchange api = new SuatmmExchange(mock.Object);
            Card unlimitedCard = SampleData.cards.Find(card => card.Name == "Безлимитная карта");
             

            ResultCode resultCode = api.Pay("100", unlimitedCard.Number.Replace(" ", ""), unlimitedCard.ExpiryMonth, unlimitedCard.ExpiryYear, unlimitedCard.Cvv, unlimitedCard.CardHolderName, 100);


            Assert.Equal(ResultCode.Ok, resultCode);
        }

        [Theory]
        [InlineData("Карта с неверным номером", "1", 100)]
        [InlineData("Карта с неверным Cvv", "1", 100)]
        [InlineData("Карта с неверным именем держателя", "1", 100)]
        [InlineData("Безлимитная карта", "1", 0)]
        public void Pay_InvalidParameters_CauseArgumentException(string cardName, string orderId, int amount)
        {
            var mock = new Mock<IPacketTransport>();

            mock.Setup(tp => tp.SendRequest<Payment, OperationStatus>(It.IsAny<Payment>()))
                .Returns(new OperationStatus { ResultCode = ResultCode.Ok });
            Card invalidCard = SampleData.cards.Find(card => card.Name == cardName);

            SuatmmExchange api = new SuatmmExchange(mock.Object);


            Action testCode = () => api.Pay(orderId, invalidCard.Number.Replace(" ", ""), invalidCard.ExpiryMonth, invalidCard.ExpiryYear, invalidCard.Cvv, invalidCard.CardHolderName, amount);


            Assert.Throws<ArgumentException>(testCode);
        }

        [Fact]
        public void GetStatus_PaidOrder_ReturnsOrderPaid()
        {
            var mock = new Mock<IPacketTransport>();

            mock.Setup(tp => tp.SendRequest<RequestOrderStatus, OperationStatus>(It.IsAny<RequestOrderStatus>()))
                .Returns(new OperationStatus { ResultCode = ResultCode.OrderPaid });

            SuatmmExchange api = new SuatmmExchange(mock.Object);


            ResultCode resultCode = api.GetStatus("100");


            Assert.Equal(ResultCode.OrderPaid, resultCode);
        }

        [Fact]
        public void Refund_PaidOrder_ReturnsOrderPaid()
        {
            var mock = new Mock<IPacketTransport>();

            mock.Setup(tp => tp.SendRequest<Refund, OperationStatus>(It.IsAny<Refund>()))
                .Returns(new OperationStatus { ResultCode = ResultCode.Ok });

            SuatmmExchange api = new SuatmmExchange(mock.Object);


            ResultCode resultCode = api.Refund("100");


            Assert.Equal(ResultCode.Ok, resultCode);
        }

    }



}
