using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuatmmApi.Formats.v1;
using SuatmmApi.Formats.v1.Forms;
using SuatmmApi.Serialize;
using Xunit;

namespace SuatmmApi.Tests
{
    public class PacketSerializerTest
    {
        [Fact]
        public void Serialize_Valid_ReturnsValidXml()
        {
            PacketSerializer serializer = PacketSerializer.Create("application/xml");

            Payment payment = new Payment { OrderId = "1", AmountKop = 100, CardHolderName = "POVYSHEV NIKOLAY", CardNumber = "9999000088881111", CVV = "100", ExpiryMonth = 12, ExpiryYear = 2017 };

            const string etalon = "<?xml version=\"1.0\" encoding=\"utf-8\"?><packet version=\"v1\"><Payment order_id=\"1\" card_number=\"9999000088881111\" expiry_month=\"12\" expiry_year=\"2017\" cvv=\"100\" cardholder_name=\"POVYSHEV NIKOLAY\" amount_kop=\"100\" /></packet>";


            string xml = null;
            using (MemoryStream stream = new MemoryStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                serializer.Serialize(new Packet<Payment>(payment) { Version = "v1" }, stream);
                stream.Position = 0;
                xml = reader.ReadToEnd();
            }

            Assert.Equal(etalon, xml);
        }

        [Fact]
        public void Deserialize_Valid_ReturnsValidXml()
        {
            PacketSerializer serializer = PacketSerializer.Create("application/xml");

            Payment etalon = new Payment { OrderId = "1", AmountKop = 100, CardHolderName = "POVYSHEV NIKOLAY", CardNumber = "9999000088881111", CVV = "100", ExpiryMonth = 12, ExpiryYear = 2017 };

            const string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><packet version=\"v1\"><Payment order_id=\"1\" card_number=\"9999000088881111\" expiry_month=\"12\" expiry_year=\"2017\" cvv=\"100\" cardholder_name=\"POVYSHEV NIKOLAY\" amount_kop=\"100\" /></packet>";


            Payment payment = null;
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(Encoding.UTF8.GetBytes(xml), 0, xml.Length);
                stream.Position = 0;
                payment = serializer.Deserialize<Payment>(stream).Content;
            }

            Assert.Equal(etalon.OrderId       , payment.OrderId       );
            Assert.Equal(etalon.AmountKop     , payment.AmountKop     );
            Assert.Equal(etalon.CardHolderName, payment.CardHolderName);
            Assert.Equal(etalon.CardNumber    , payment.CardNumber    );
            Assert.Equal(etalon.CVV           , payment.CVV           );
            Assert.Equal(etalon.ExpiryMonth   , payment.ExpiryMonth   );
            Assert.Equal(etalon.ExpiryYear    , payment.ExpiryYear    );

        }

    }
}
