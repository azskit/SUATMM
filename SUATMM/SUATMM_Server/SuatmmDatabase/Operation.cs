using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SUATMM_Server.SuatmmDatabase
{
    enum OperationKind
    {
        Payment,
        Refund
    }

    class Operation
    {
        public int OrderId { get; set; }
        public int CardId { get; set; }
        public uint Amount { get; set; }

        public OperationKind Kind { get; set; }

    }
}
