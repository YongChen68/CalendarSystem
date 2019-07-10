using System;
using System.Runtime.Serialization;

using static System.Net.Mime.MediaTypeNames;

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
    public class Installer
    {
        [DataMember]
        public string SeniorInstaller { get; set; }

        [DataMember]
        public string CrewNames { get; set; }
      
    }

    public class CalledLog
    {
        [DataMember]
        public DateTime DateCalled { get; set; }

        [DataMember]
        public string CalledMessage { get; set; }


        [DataMember]
        public string Notes3  { get; set; }


    }

    public class SubTrades
    {
     

        [DataMember]
        public string SubTrade { get; set; }


        [DataMember]
        public string Status { get; set; }


    }

    public class WOPicture
    {
        [DataMember]
        public string PictureName { get; set; }

        [DataMember]
        public byte[] pic {
            get; set;
        }

        [DataMember]
        [Lift.Database.DbIgnore]
        public string picString
        {
            get
            {
                if (pic != null)
                {
                     return "<img src=\"data:image/jpeg;base64," + Convert.ToBase64String(pic) + "\">";
                
                }
                else
                {
                    return null;
                }
            }
            set {  }
        }

    }
    public class UnavailableHR
    {
        [DataMember]
        public string DateUnavailable { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Branch { get; set; }


        [DataMember]
        public string Reason { get; set; }

    }

    public class Datetime
    {
    }
}