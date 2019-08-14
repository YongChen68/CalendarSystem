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


    public class WindowsCustomer
    {
        [DataMember]
        public string WorkOrderNumber { get; set; }

        [DataMember]
        public string CustomerName { get; set; }


        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string ShippingType { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Branch_display { get; set; }

        [DataMember]
        public int TotalWindows { get; set; }

        [DataMember]
        public int TotalDoors { get; set; }

        [DataMember]
        public int TotalPatioDoors { get; set; }

        [DataMember]
        public decimal TotalPrice { get; set; }


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

    public class Projections
    {
        [DataMember]
        public int NumberOfInstallers { get; set; }

        [DataMember]
        public string NumberOfMinAvailable { get; set; }

        [DataMember]
        public string Branch { get; set; }

        [DataMember]
        public DateTime ProjectionsDate { get; set; }

    }

}