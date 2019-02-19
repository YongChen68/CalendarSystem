using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Generics.Utils;

namespace CalendarSystem.Utils
{
    public static class ContentTypeParser
    {

        public static ContentType GetType(string type)
        {
            switch (type)
            {
                case "Windows": return ContentType.Window;
                case "Customer": return ContentType.Customer;
                case "Doors": return ContentType.Door;
                case "Paint": return ContentType.Paint;
                case "Shipping": return ContentType.Shipping;
                case "Installation": return ContentType.Installation;
                default: throw new NotSupportedException(type);
            }
        }
    }
}