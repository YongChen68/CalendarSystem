﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Generics.Utils.Data
{
    //class InstallationEvent
    //{
    //}
    [DataContract]
    public class InstallationEvent
    {
        public enum OrderState { NotRequired = 0, Unknown, Ordered, Done };
        private string wonum;
        private string lastname;
        private string city;
        private string state;
        private string hours;
        private string services;
        private string sWindows;
        private string sDoors;
        private string sOther;
        private string sHomePhone;
        private string sCellPhone;
        private string sWorkPhone;
        private string sCrew;
        private string sSeniorInstaller;
        private string sEstInstallerCnt;
        private string sAddress;
        private string hvac;

        private bool bShowServices = false;

        private OrderState OrderStateWindows;
        private OrderState OrderStateDoor;
        private OrderState OrderStateOther;
        private OrderState OrderStatePainted;

        private string sBranch;

        // order from sql
        public InstallationEvent()
        {

        }

        [DataMember]
        public int ReturnedJob { get; set; }
        [DataMember]
        public string ReturnTripReason { get; set; }

        [DataMember]
        public string ResourceID { get; set; }

        [DataMember]
        public string ParentRecordId { get; set; }

        

        [DataMember]
        public DateTime ScheduledDate { get; set; }

        [DataMember]
        public DateTime EndTime { get; set; }

        [DataMember]
        public String WoodDropOffDate {
            get; set;
        }

   

        [DataMember]
        [Lift.Database.DbIgnore]
        public string StrWoodDropOffTime {
            get
            {
                string str = string.Empty;
                if ((WoodDropOffDate != null) && (WoodDropOffDate.Length > 0))
                {
                    str = WoodDropOffDate.Split(' ')[1];
                }
                return str;
            }
            set { }
        }
        [DataMember]
        [Lift.Database.DbIgnore]
        //   public string start { get { return FormatDateTime(ScheduledDate); } set { } }
        public string StrWoodDropOffDate {
            get {
                string str = string.Empty;
                if ((WoodDropOffDate!=null) && (WoodDropOffDate.Length>0))
                {
                 //   str = FormatDateTime(Convert.ToDateTime(WoodDropOffDate.Split(' ')[0]));
                    str =Convert.ToDateTime(WoodDropOffDate.Split(' ')[0]).ToString("MM/d/yyyy");
                }
                return str; }
            set { }
        }
        //[DataMember]
        //public DateTime ReturnEventDate { get; set; }
        [DataMember]
        [Lift.Database.DbIgnore]
        //   public string start { get { return FormatDateTime(ScheduledDate); } set { } }
        public string start { get; set; }

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
        [Lift.Database.DbIgnore]
        //public string end { get { return FormatDateTime(ScheduledDate); } set { } }
        public string end { get; set; }

        //[DataMember]
        //public DateTime ScheduledDate { get; set; }

        [DataMember]
        public string StreetAddress { get; set; }

        [DataMember]
        public int detailrecordCount { get; set; }


        [DataMember]
        public string id { get; set; }


        [DataMember]
        public string HomeDepotJob { get; set; }

        [DataMember]
        public string AgeOfHome { get; set; }


        [DataMember]
        public decimal SalesAmmount { get; set; }

        [DataMember]
        public decimal HazardousBudgetedLBR { get; set; }

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


        //[DataMember]
        //[Lift.Database.DbIgnore]
        //public bool allDay { get; set; }



        [DataMember]
        public string allDay { get; set; }

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
        public DateTime? StartScheduleDate
        {
            get; set;
        }

        [DataMember]
        public DateTime? EndScheduleDate
        {
            get; set;
        }

        //[DataMember]
        //public bool HasReturnedJob
        //{
        //    get; set;
        //}

        //[DataMember]
        //public string Other
        //{
        //    get; set;
        //}
        //[DataMember]
        //public string Subtrades
        //{
        //    get; set;
        //}
        [DataMember]
        public string Hours
        {
            get; set;
        }
        //[DataMember]
        //public OrderState WindowState
        //{
        //    get; set;
        //}
        //[DataMember]
        //public string WindowState
        //{
        //    get; set;
        //}
        //[DataMember]
        //public OrderState DoorState
        //{
        //    get; set;
        //}

        //[DataMember]
        //public string DoorState
        //{
        //    get; set;
        //}

      
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
        public  string LeadPaint
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
        public string SubTradeFlag
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

        //[DataMember]
        //public OrderState OtherState
        //{
        //    get; set;
        //}
        //[DataMember]
        //public string OtherState
        //{
        //    get; set;
        //}

        //[DataMember]
        //public OrderState PaintedState
        //{
        //    get; set;
        //}
        [DataMember]
        public string WorkOrderNumber
        {
            get; set;
        }

      
        [DataMember]
        [Lift.Database.DbIgnore]
        public string title { get { return WorkOrderNumber; } set { } }
       
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

        [Lift.Database.DbIgnore]
        [DataMember]
        public string UnavailableStaff { get; set; }

        [DataMember]
        public int MinAvailable
        {
            get; set;
        }

        [DataMember]
        public Decimal SalesTarget
        {
            get; set;
        }
    }
}
