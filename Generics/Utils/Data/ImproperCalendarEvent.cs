using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Generics.Utils
{
    //Do not use this object, it is used just as a go between between javascript and asp.net
    [DataContract]
    public class ImproperCalendarEvent
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string start { get; set; }
        [DataMember]
        public string end { get; set; }
        [DataMember]
        public bool allDay { get; set; }
        [DataMember]
        public int doors { get; set; }
        [DataMember]
        public int windows { get; set; }
        [DataMember]
        public string type { get; set; }
    }

    public class ImproperInstallationEvent
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string start { get; set; }
        [DataMember]
        public string end { get; set; }

        [DataMember]
        public string CurrentStateName { get; set; }

        [DataMember]
        public string ReturnTripReason { get; set; }

    }

    public class ImproperRemeasureEvent
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string start { get; set; }

        [DataMember]
        public string end { get; set; }

        [DataMember]
        public string CurrentStateName { get; set; }

    }



public class InstallationDataEvent
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string start { get; set; }
        [DataMember]
        public string end { get; set; }

        [DataMember]
        public int Asbestos { get; set; }

        [DataMember]
        public int WoodDropOff { get; set; }

        [DataMember]
        public int HighRisk { get; set; }


        [DataMember]
        public string Saturday { get; set; }

        [DataMember]
        public string Sunday
        {
            get; set;
        }

        [DataMember]
        public string WoodDropDateAndTime { get; set; }

        [DataMember]
        public int EstInstallerCnt { get; set; }



        [DataMember]
        public string LeadPaint
        {
            get; set;
        }


    }


    public class InstallationEventWeekends
    {
        [DataMember]
        public string id { get; set; }


        [DataMember]
        public string Saturday { get; set; }
        [DataMember]
        public string Sunday { get; set; }


    }
}