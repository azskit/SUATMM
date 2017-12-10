using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SUATMM_Server.SuatmmDatabase
{
    class Card
    {
        public int    CardId      { get; set; }

        public string Number      { get; set; }
        public int    ExpiryYear  { get; set; }
        public int    ExpiryMonth { get; set; }
        public string CardHolderName      { get; set; }
        public string Cvv         { get; set; }
        public int    Rest        { get; set; } //остаток, в копейках
        public bool   IsUnlimited { get; set; }
    }
}
