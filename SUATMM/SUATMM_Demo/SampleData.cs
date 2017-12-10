using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SUATMM_Demo
{
    public static class SampleData
    {
        public static List<Card> cards = new List<Card>()
            {
                new Card()
                {
                    Name            = "Карта с лимитом 100р",
                    Number          = "1234 5678 9012 3456",
                    ExpiryYear      = 2017,
                    ExpiryMonth     = 12,
                    Cvv             = "123",
                    CardHolderName  = "SPONGEBOB SQUAREPANTS"
                },

                new Card()
                {
                    Name           = "Карта с истекшим сроком",
                    Number         = "1234 5678 9012 3456",
                    ExpiryYear     = 2015,
                    ExpiryMonth    = 12,
                    Cvv            = "111",
                    CardHolderName = "SPONGEBOB SQUAREPANTS"
                },

                new Card()
                {
                    Name           = "Безлимитная карта",
                    Number         = "1111000011110000",
                    ExpiryYear     = 2099,
                    ExpiryMonth    = 12,
                    Cvv            = "111",
                    CardHolderName = "MR CRABS"
                },

                new Card()
                {
                    Name           = "Карта с неверным номером",
                    Number         = "1111aaaa2222bbbb",
                    ExpiryYear     = 2017,
                    ExpiryMonth    = 12,
                    Cvv            = "123",
                    CardHolderName = "STAR PATRICK"
                },

                new Card()
                {
                    Name           = "Карта с неверным Cvv",
                    Number         = "1111000011110000",
                    ExpiryYear     = 2017,
                    ExpiryMonth    = 12,
                    Cvv            = "^_^",
                    CardHolderName = "MR CRABS"
                },

                new Card()
                {
                    Name           = "Карта с неверным именем держателя",
                    Number         = "1111000011110000",
                    ExpiryYear     = 2017,
                    ExpiryMonth    = 12,
                    Cvv            = "123",
                    CardHolderName = "Мистер Крабс"
                }
            };
    }

    public class Card
    {
        public string Name, Number, CardHolderName, Cvv;
        public int ExpiryYear, ExpiryMonth;

        public override string ToString()
        {
            return string.Format($"{Name}: {Number} {CardHolderName} {ExpiryMonth}/{ExpiryYear} {Cvv}");
        }
    }
}
