using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Generics.Utils.Data
{
    [DataContract]
    public class GlobalValues
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}
