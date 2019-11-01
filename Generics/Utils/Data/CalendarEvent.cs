using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Generics.Utils;

namespace Generics.Utils
{
    /// <summary>
    /// Summary description for CalendarEvent
    /// </summary>
    [DataContract]
    public class CalendarEvent
    {

        public string CurrentStateName { get; set; }

        public bool isHoliday = false;

        [Lift.Database.DbIgnore]
        [DataMember]
        public string color
        {
            get
            {
                string color = "";
                switch (CurrentStateName.Trim())
                {
                    case "Draft Work Order": if (JobType == "RES") color = "#DDA0DD"; else color = "#DDA0DD"; break;          // Pink Res + Draft
                    case "Scheduled Work Order": if (CardinalOrderedDate != null && CardinalOrderedDate.Trim().Length > 0) color = "#7AA9DD"; else color = "#9FB6CD"; break;      // Blue - Grey Blue
                    case "In-Progress": color = " #a5d6a7"; break;                   // Green
                    case "Ready To Ship": color = "LemonChiffon"; break;             // Yellow LemonChiffon
                    case "Shipped": color = "#ffc966"; break;                        // Orange
                    case "On Hold": color = "#ff6347"; break;                        // Tomato
                    case "Duplicated Work Order": color = "#ff6347"; break;          // Tomato <> sql filtered
                    case "Completed Reservations": color = "#ff6347"; break;         // Tomato <> sql filtered
                    default: color = ""; break;
                }
                return color;
            }
            set { }
        }

        [DataMember]
        public string Branch { get; set; }
        [DataMember]
        public string JobType { get; set; }

        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string description { get; set; }
        public DateTime startDateTime { get; set; }
        [DataMember]
        [Lift.Database.DbIgnore]
        public string start {
            get
            {
                return startDateTime.ToString();
            }
            set { } }
        public DateTime endDateTime { get; set; }
        [DataMember]
        [Lift.Database.DbIgnore]
        //  public string end { get { return FormatDateTime(endDateTime.AddDays(1)); } set { } }

        public string end
        {
            get
            {
                int dayDiff = Convert.ToInt32((endDateTime - startDateTime).TotalDays);
                // return FormatDateTime(endDateTime.AddDays(dayDiff));
                return startDateTime.AddDays(dayDiff+1).ToString();
            }
            set { }
        }
        [DataMember]
        public bool allDay { get; set; }
        [DataMember]
        public decimal doors { get; set; }
        [DataMember]
        public int NumberOfPatioDoors { get; set; }
        [DataMember]
        public string type { get; set; }

        [DataMember]
        public string PaintIcon { get; set; }

        [DataMember]
        public string WindowIcon { get; set; }
        [DataMember]
        public string DoorIcon { get; set; }
        [DataMember]
        public string BatchNo { get; set; }
        [DataMember]
        public int FlagOrder { get; set; }
        [DataMember]
        public decimal windows { get; set; }
        [DataMember]
        public int TotalBoxQty { get; set; }
        [DataMember]
        public int TotalGlassQty { get; set; }
        [DataMember]
        public int TotalPrice { get; set; }
        [DataMember]
        public int TotalLBRMin { get; set; }
        [DataMember]
        public int F6CA { get; set; }
        [DataMember]
        public int F27DS { get; set; }
        [DataMember]
        public int F27TS { get; set; }
        [DataMember]
        public int F27TT { get; set; }
        [DataMember]
        public int F29CA { get; set; }
        [DataMember]
        public int F29CM { get; set; }
        [DataMember]
        public decimal F52PD { get; set; }
        [DataMember]
        public int F68CA { get; set; }
        [DataMember]
        public int F68SL { get; set; }
        [DataMember]
        public int F68SLMin { get; set; }
        [DataMember]
        public int F68VS { get; set; }
        [DataMember]
        public int F68VSMin { get; set; }

        [DataMember]
        public int M2000Icon { get; set; }
        [DataMember]
        public int CustomFlag { get; set; }
        [DataMember]
        public int HighRiskFlag { get; set; }
        [DataMember]
        public string CardinalOrderedDate { get; set; }
        [DataMember]
        public string CompleteDate { get; set; }
        [DataMember]
        public int Transom { get; set; }
        [DataMember]
        public int Sidelite { get; set; }
        [DataMember]
        public int SingleDoor { get; set; }
        [DataMember]
        public int DoubleDoor { get; set; }


        [DataMember]
        public int Simple { get; set; }
        [DataMember]
        public int Complex { get; set; }
        [DataMember]
        public int Over_Size { get; set; }
        [DataMember]
        public int Arches { get; set; }
        [DataMember]
        public int Rakes { get; set; }
        [DataMember]
        public int Customs { get; set; }


        private string FormatDateTime(DateTime val)
        {
            return val.ToString("yyyy-MM-ddTHH:mm:00Z");
        }

        [DataMember]
        [Lift.Database.DbIgnore]
        public DateTime HolidayDate { get; set; }
        [Lift.Database.DbIgnore]
        [DataMember]
        public string HolidayName { get; set; }
        [DataMember]
        [Lift.Database.DbIgnore]
        public DateTime ScheduledProductionDate { get; set; }
        

    }


}