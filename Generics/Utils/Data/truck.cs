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
        //[DataMember]
        //public string TruckCrewNameList { get; set; }

        [DataMember]
        public string RecordID { get; set; }


        //[DataMember]
        //public string WorkOrderNumber { get; set; }

        //[DataMember]
        //public string ParentRecordId { get; set; }



    }

    public class TruckWithWO
    {

        [DataMember]
        public string TruckID { get; set; }

        [DataMember]
        public string TruckLookup { get; set; }


        //[DataMember]
        //public List<InstallerWithLessInfo>  TruckCrews { get; set; }

        [DataMember]
        public string RecordID { get; set; }


        [DataMember]
        public string WorkOrderNumber { get; set; }

        //[DataMember]
        //public string ParentRecordId { get; set; }

        //[DataMember]
        //public Datetime ScheduledDate { get; set; }

        //[DataMember]
        //public Datetime EndTime { get; set; }
        [DataMember]
        public string TruckName { get; set; }

    }

    public class TruckInstallationEvent
    {
        [DataMember]
        public string DetailRecordId { get; set; }


        [DataMember]
        public string ActionItemId { get; set; }


        [DataMember]
        public string ScheduledDate { get; set; }
        [DataMember]
        public string endtime { get; set; }

        //[DataMember]
        //public Datetime title { get; set; }
        [DataMember]
        public string RecordID { get; set; }

    }

}