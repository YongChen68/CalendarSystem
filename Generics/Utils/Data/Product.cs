using System;
using System.Runtime.Serialization;

namespace Generics.Utils
{
    public class Product
    {
        [DataMember]
        public string WorkOrderNumber { get; set; }

        [DataMember]
        public string Item { get; set; }

        [DataMember]
        public string Size { get; set; }

        [DataMember]
        public string Quantity { get; set; }

        [DataMember]
        public string SubQty { get; set; }

        [DataMember]
        public string System { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Status { get; set; }


    }
}