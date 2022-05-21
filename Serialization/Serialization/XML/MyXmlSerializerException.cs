using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization.XML
{
    internal class MyXmlSerializerException: Exception
    {
        public MyXmlSerializerException(string message): base(message)
        { }
    }
}
