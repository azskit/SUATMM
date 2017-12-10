using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SUATMM_Server.SuatmmDatabase
{
    internal static class Storage
    {
        internal static Dictionary<int, Card> Cards { get; private set; }            = new Dictionary<int, Card>(); 
        internal static Dictionary<int, Seller> Sellers { get; private set; }        = new Dictionary<int, Seller>();
        internal static Dictionary<int, Order> Orders { get; private set; }          = new Dictionary<int, Order>();
        internal static Dictionary<int, Operation> Operations { get; private set; }  = new Dictionary<int, Operation>();

        static Storage()
        {
            #region Cards

            Cards[0] = new Card
            {
                CardId      = 0,
                Number      = String.Concat("1234", "5678", "9012", "3456"),
                ExpiryYear  = 2017,
                ExpiryMonth = 12,
                Cvv         = "123",
                CardHolderName  = "SPONGEBOB SQUAREPANTS",
                IsUnlimited = false,
                Rest        = 10000 //100 рублей
            };

            Cards[1] = new Card
            {
                CardId         = 1,
                Number         = "1111000011110000",
                ExpiryYear     = 2099,
                ExpiryMonth    = 12,
                Cvv            = "111",
                CardHolderName = "MR CRABS",
                IsUnlimited    = true, //безлимит
                Rest           = 39287639 // 392 876р 39 коп
            };

            Cards[2] = new Card //старая карта спанчбоба
            {
                CardId         = 2,
                Number         = "1234567890123456",
                ExpiryYear     = 2015,
                ExpiryMonth    = 12,
                Cvv            = "111",
                CardHolderName = "SPONGEBOB SQUAREPANTS",
                IsUnlimited    = false,
                Rest           = 10000 //100 рублей
            };

            #endregion

            #region Sellers
            Sellers[0] = new Seller() { Id = 0, Name = "KRUSTY CRABS" };

            #endregion
        }
    }
}
