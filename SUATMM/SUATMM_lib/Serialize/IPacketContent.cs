using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace SuatmmApi.Serialize
{
    public interface IPacketContent
    {
        IEnumerable<KeyValuePair<string, string>> GetProperties();

        void SetProperty(string property, string value);
    }





}
