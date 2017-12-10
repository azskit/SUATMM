using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuatmmApi.Formats
{
    public partial class Format
    {
        public string Version { get; set; }

        public Dictionary<string, Type> SupportedForms { get; private set; }

        public static Dictionary<string, Format> SupportedFormats { get; private set; }

        static Format()
        {
            SupportedFormats = new Dictionary<string, Format>();

            SupportedFormats["v1"] = new Format()
            {
                Version = "v1",
                SupportedForms = new Dictionary<string, Type>
                {
                    {typeof(v1.Forms.Payment).Name,             typeof(v1.Forms.Payment)            },
                    {typeof(v1.Forms.OperationStatus).Name,     typeof(v1.Forms.OperationStatus)    },
                    {typeof(v1.Forms.RequestOrderStatus).Name,  typeof(v1.Forms.RequestOrderStatus) },
                    {typeof(v1.Forms.Refund).Name,              typeof(v1.Forms.Refund)             }
                }
            };
        }
    }
}
