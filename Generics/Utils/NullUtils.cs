using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generics.Utils
{
    public enum ContentType { Window, Door, Paint, Shipping, Customer, Installation };

    public static class NullUtils
    {
        public static bool IsNull(this object obj) { return obj == null; }
        public static bool IsNullorEmpty(this string val) { return val.IsNull() || val.Trim().Length == 0; }
        public static bool IsNullorEmpty(this object[] val) { return val.IsNull() || val.Length == 0; }
        public static bool IsNullorEmpty(this List<object> val) { return val.IsNull() || val.Count == 0; }
    }
}
