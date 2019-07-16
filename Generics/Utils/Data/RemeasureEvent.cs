using System;
using System.Runtime.Serialization;

using static System.Net.Mime.MediaTypeNames;

namespace Generics.Utils.Data
{
    public class RemeasureEvent
    {
        public RemeasureEvent()
        {

        }
        [DataMember]
        public string WorkOrderNumber { get; set; }

        [DataMember]
        public DateTime RemeasureDate { get; set; }

        [DataMember]
        public string RemeasureEndTime { get; set; }
        
        [DataMember]
        [Lift.Database.DbIgnore]
        public string color
        {
            get
            {
                string color = "";
                switch (CurrentStateName.Trim())
                {
                    case "Install in Progress": color = " #a5d6a7"; break;                            // Green
                    case "Installation Confirmed": color = "#4890e2"; break;                          // Blue - Grey Blue
                    case "Install Completed": color = "#ffc966"; break;                               // Orange
                    case "Installation inprogress rejected": color = "#ff6347"; break;                // Tomato
                    case "Installation Manager Review": color = "#ffc966"; break;                     // Orange
                    case "Job Completed": color = "#ffc966"; break;                                   // Orange
                    case "Job Costing": color = "#ffc966"; break;                                     // Orange
                    case "Pending Install Completion": color = "#ff6347"; break;                      // Tomato
                    case "Ready for Invoicing": color = "#ffc966"; break;                             // Orange
                    case "ReMeasure Scheduled": color = "#9FB6CD"; break;                             // Blue - Grey Blue
                    case "Rejected Installation": color = "#ff6347"; break;                           // Tomato
                    case "Rejected Job Costing": color = "#ff6347"; break;                            // Tomato
                    case "Rejected Manager Review": color = "#ff6347"; break;                         // Tomato
                    case "Rejected Remeasure": color = "#ff6347"; break;                              // Tomato
                    case "Rejected Scheduled Work": color = "#ff6347"; break;                         // Tomato
                    case "Unreviewed Job Costing": color = "#ffc966"; break;                          // Orange
                    case "Unreviewed Work Scheduled": color = "#9FB6CD"; break;                       // Blue - Grey Blue
                    case "VP Installation Approval": color = "#ffc966"; break;                        // Orange
                    case "Work Scheduled": color = "#9FB6CD"; break;                                  // Blue - Grey Blue

                    default: color = ""; break;
                }
                return color;
            }
            set { }
        }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string StreetAddress { get; set; }

        [DataMember]
        [Lift.Database.DbIgnore]
        public string title { get { return WorkOrderNumber; } set { } }
        [DataMember]
        [Lift.Database.DbIgnore]
        //public string end { get { return FormatDateTime(ScheduledDate); } set { } }
        public string end { get; set; }

        [DataMember]
        [Lift.Database.DbIgnore]
        //   public string start { get { return FormatDateTime(ScheduledDate); } set { } }
        public string start { get; set; }

        [DataMember]
        public decimal SalesAmmount { get; set; }

        [DataMember]
        public decimal TotalSalesAmount { get; set; }

        [DataMember]
        public int TotalAsbestos { get; set; }

        [DataMember]
        public int TotalWoodDropOff { get; set; }

        [DataMember]
        public int TotalHighRisk { get; set; }


        [DataMember]
        public string EstInstallerCnt { get; set; }


        [DataMember]
        [Lift.Database.DbIgnore]
        public bool allDay { get; set; }

        [DataMember]
        public decimal Windows
        {
            get; set;
        }
        [DataMember]
        public decimal TotalWindows
        {
            get; set;
        }
        [DataMember]
        public decimal Doors
        {
            get; set;
        }
        [DataMember]
        public decimal TotalDoors
        {
            get; set;
        }

        [DataMember]
        public decimal ExtDoors
        {
            get; set;
        }
        [DataMember]
        public decimal TotalExtDoors
        {
            get; set;
        }

        [DataMember]
        public string Hours
        {
            get; set;
        }
 


        [DataMember]
        public string PostCode
        {
            get; set;
        }


        [DataMember]
        public string Email
        {
            get; set;
        }

        [DataMember]
        public string SalesRep
        {
            get; set;
        }


        [DataMember]
        public decimal installationwindowLBRMIN
        {
            get; set;
        }

        [DataMember]
        public decimal InstallationDoorLBRMin
        {
            get; set;
        }


        [DataMember]
        public decimal InstallationPatioDoorLBRMin
        {
            get; set;
        }

        [DataMember]
        public decimal TotalInstallationLBRMin
        {
            get; set;
        }

        [DataMember]
        public string LeadPaint
        {
            get; set;
        }
        [DataMember]
        public decimal subinstallationwindowLBRMIN
        {
            get; set;
        }
        [DataMember]
        public decimal subExtDoorLBRMIN
        {
            get; set;
        }

        [DataMember]
        public decimal subInstallationPatioDoorLBRMin
        {
            get; set;
        }

        [DataMember]
        public decimal subTotalInstallationLBRMin
        {
            get; set;
        }
        [DataMember]
        public decimal SidingLBRBudget
        {
            get; set;
        }
        [DataMember]
        public decimal SidingLBRMin
        {
            get; set;
        }
        [DataMember]
        public decimal SidingSQF
        {
            get; set;
        }

        [DataMember]
        public string LastName
        {
            get; set;
        }

        [DataMember]
        public string FirstName
        {
            get; set;
        }

        [DataMember]
        public string City
        {
            get; set;
        }
        //[DataMember]
        //public string State
        //{
        //    get; set;
        //}
        [DataMember]
        public string HomePhoneNumber
        {
            get; set;
        }
        [DataMember]
        public string CellPhone
        {
            get; set;
        }
        [DataMember]
        public string WorkPhoneNumber
        {
            get; set;
        }
        //[DataMember]
        //public string CrewNames
        //{
        //    get; set;
        //}
        [DataMember]
        public string CurrentStateName
        {
            get; set;
        }
        // [DataMember]
        //public string SeniorInstaller
        //{
        //    get; set;
        //}
        [DataMember]
        public string Saturday
        {
            get; set;
        }
        [DataMember]
        public string Sunday
        {
            get; set;
        }
        //[DataMember]
        //public bool ShowServices
        //{
        //    get; set;
        //}
        [DataMember]
        public string Branch { get; set; }
        //[DataMember]
        //public string HVAC { get; set; }
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


    }


}