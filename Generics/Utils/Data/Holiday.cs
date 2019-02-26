using System;
using System.Runtime.Serialization;

namespace Generics.Utils
{
    public class Holiday
    {

        private string FormatDateTime(DateTime val)
        {
            return val.ToString("yyyy-MM-ddTHH:mm:00Z");
        }

        [DataMember]
        public DateTime HolidayDate { get; set; }

        [DataMember]
        public string HolidayName { get; set; }

        [DataMember]
        [Lift.Database.DbIgnore]
        public bool allDay { get; set; }


        [DataMember]
        [Lift.Database.DbIgnore]
        public string start { get { return FormatDateTime(HolidayDate); } set { } }
      
        [DataMember]
        [Lift.Database.DbIgnore]
        public string end { get { return FormatDateTime(HolidayDate); } set { } }

    }
}