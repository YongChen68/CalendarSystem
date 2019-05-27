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

        //[DataMember]
        //public string CurrentState { get; set; }

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