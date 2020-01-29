using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
namespace Generics.Utils
{
    public class Truck
    {
        [DataMember]
        public string TruckName { get;  set; }
        [DataMember]
        public string Make { get;  set; }
        [DataMember]

        public string Model { get; set; }
        [DataMember]
        public string Plate { get; set; }
        [DataMember]
        public string VIN { get; set; }
        //[DataMember]
        //public string Notes { get; set; }
        [DataMember]
        public int Year { get; set; }
        //[DataMember]
        //public InstallerInfo Driver { get; set; }


        //public List<InstallerInfo> Crews { get; set; }

        //public string CreatedBy { get; set; }
        //public Datetime CreatedOn { get; set; }

        //[DataMember]
        //public string TruckLocation { get; set; }
        [DataMember]
        public string TruckCrewNameList { get; set; }

        [DataMember]
        public string RecordID { get; set; }


    }
}