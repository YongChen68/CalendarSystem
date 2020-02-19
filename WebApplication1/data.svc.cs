using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Generics.Utils;
using Generics.Utils.Data;

namespace CalendarSystem
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Data2" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Data2.svc or Data2.svc.cs at the Solution Explorer and start debugging.
    public class data : Idata
    {
        public List<CalendarEvent> GetEvents(string start, string end, string type, string states, string branch, string jobType, string shippingType)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Entering GetEvents({0},'{1}','{2}','{3}','{4}','{5}', '{6}' )", start, end, type, states ?? "NULL", branch ?? "NULL", jobType ?? "NULL", shippingType ?? "NULL");
            List<CalendarEvent> retValue = null;
            List<Holiday> retValue1 = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(start, end, new List<string>(states.Split(',')), new List<string>(branch.Split(',')), new List<string>(jobType.Split(',')), new List<string>(shippingType.Split(',')));
                retValue = getter.GetData(Utils.ContentTypeParser.GetType(type));

                Utils.Data.IGetter getter1 = new Utils.Data.EventDataGetter(start, end);
                retValue1 = getter1.GetHolidayData();

                CalendarEvent myEvent = null;
                foreach (Holiday holiday in retValue1)
                {
                    myEvent = new CalendarEvent();
                    myEvent.allDay = true;
                    myEvent.HolidayDate = holiday.HolidayDate;
                    myEvent.startDateTime = holiday.HolidayDate;
                    myEvent.endDateTime = holiday.HolidayDate;
                    myEvent.HolidayName = holiday.HolidayName;
                    myEvent.title = holiday.HolidayName;
                    myEvent.CurrentStateName = "Duplicated Work Order";
                    retValue.Add(myEvent);
                }

                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetEvents() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        public List<InstallationEvent> GetInstallationEvents(string start, string end, string branch, string state)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Entering InstallationEvent({0},'{1}','{2}' )", start, end, branch ?? "NULL", state ?? "NULL");
            List<InstallationEvent> retValue = null;
            List<Holiday> retValue1 = null;
            List<UnavailableHR> retValue2 = null;
            //  List<string> unavailableStaffName = new List<string>();

            Dictionary<string, string> unavailableStaffList = new Dictionary<string, string>();
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(start, end, new List<string>(branch.Split(',')), new List<string>(state.Split(',')));
                retValue = getter.GetData();

                Utils.Data.IGetter getter1 = new Utils.Data.EventDataGetter(start, end);
                retValue1 = getter1.GetHolidayData();

                //Utils.Data.IGetter getter1 = new Utils.Data.EventDataGetter(start, end);
                //retValue1 = getter1.GetHolidayData();

                Utils.Data.IGetter getter2 = new Utils.Data.EventDataGetter(new List<string>(branch.Split(',')));
                retValue2 = getter.GetUnavailableResources();

                InstallationEvent myEvent = null;
                foreach (Holiday holiday in retValue1)
                {
                    myEvent = new InstallationEvent();
                    myEvent.allDay = "Yes";
                    myEvent.HolidayDate = holiday.HolidayDate;
                    myEvent.start = holiday.HolidayDate.ToShortDateString();
                    myEvent.end = holiday.HolidayDate.ToShortDateString();
                    myEvent.HolidayName = holiday.HolidayName;
                    myEvent.LastName = "";
                    myEvent.City = "";
                    myEvent.title = holiday.HolidayName;
                    myEvent.CurrentStateName = "Rejected Scheduled Work";
                    retValue.Add(myEvent);
                }

                //var xx = retValue2.GroupBy(l => l.DateUnavailable).Select(dd => new { DateUnavailable = dd.Key, Name = string.Join(",", dd.Select(ee => ee.Name).ToArray()) });
                //foreach (KeyValuePair<string, string> un in xx.ToDictionary(x=>x.DateUnavailable,x=>x.Name))
                // {
                //     myEvent = new InstallationEvent();
                //     myEvent.allDay = true;
                //     myEvent.start = un.Key;
                //     myEvent.end = un.Key;
                //     myEvent.ScheduledDate = Convert.ToDateTime(un.Key);
                //     myEvent.WorkOrderNumber = un.Key;

                //     myEvent.LastName = "Unavailable Staff";

                //     myEvent.UnavailableStaff = un.Value;
                //     //myEvent.City = "";
                //     myEvent.title = un.Value;
                //     myEvent.CurrentStateName = "Rejected Scheduled Work";
                //     retValue.Add(myEvent);
                // }

                foreach (UnavailableHR un in retValue2)
                {
                    myEvent = new InstallationEvent();
                    myEvent.allDay = "Yes";
                    myEvent.start = un.DateUnavailable;
                    myEvent.end = un.DateUnavailable;
                    myEvent.WorkOrderNumber = un.Name + un.DateUnavailable;
                    // myEvent.ScheduledDate = Convert.ToDateTime(un.DateUnavailable);
                    // myEvent.WorkOrderNumber = un.DateUnavailable;
                    //    myEvent.WorkOrderNumber = un.Name;

                    myEvent.LastName = "";

                    myEvent.UnavailableStaff = un.Name;
                    myEvent.City = "";
                    myEvent.title = un.Name;
                    myEvent.CurrentStateName = "Rejected Scheduled Work";
                    retValue.Add(myEvent);
                }

                //   var xx = retValue2.GroupBy(l => l.DateUnavailable).Select(dd => new { DateUnavailable = dd.Key, Name = string.Join(",", dd.Select(ee => ee.Name).ToArray()) });
                //   var xx = retValue2.GroupBy(l => l.DateUnavailable).Select(dd => new { DateUnavailable = dd.Key, Name = dd.Select(ee => ee.Name)});
                //var xx = retValue2.GroupBy(l => l.DateUnavailable).Select(dd => new { DateUnavailable = dd.Key, Name = dd.Select(ee => ee.Name) });
                //foreach (KeyValuePair<string, string> un in xx.ToDictionary(x => x.DateUnavailable, x => x.Name))
                //{
                //    myEvent = new InstallationEvent();
                //    myEvent.allDay = true;
                //    myEvent.start = un.Key;
                //    myEvent.end = un.Key;
                //    myEvent.ScheduledDate = Convert.ToDateTime(un.Key);
                //    myEvent.WorkOrderNumber = un.Key;

                //    myEvent.LastName = "Unavailable Staff";

                //    myEvent.UnavailableStaff = un.Value;
                //    //myEvent.City = "";
                //    myEvent.title = un.Value;
                //    myEvent.CurrentStateName = "Rejected Scheduled Work";
                //    retValue.Add(myEvent);
                //}


                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetEvents() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        public List<RemeasureEvent> GetRemeasureEvents(string start, string end, string branch, string state)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Entering GetRemeasureEvents({0},'{1}','{2}' )", start, end, branch ?? "NULL", state ?? "NULL");
            List<RemeasureEvent> retValue = null;
            List<Holiday> retValue1 = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(start, end, new List<string>(branch.Split(',')), new List<string>(state.Split(',')));
                retValue = getter.GetRemeasureData();

                Utils.Data.IGetter getter1 = new Utils.Data.EventDataGetter(start, end);
                retValue1 = getter1.GetHolidayData();

                RemeasureEvent myEvent = null;
                foreach (Holiday holiday in retValue1)
                {
                    myEvent = new RemeasureEvent();
                    myEvent.allDay = true;
                    myEvent.HolidayDate = holiday.HolidayDate;
                    myEvent.start = holiday.HolidayDate.ToShortDateString();
                    myEvent.end = holiday.HolidayDate.ToShortDateString();
                    myEvent.HolidayName = holiday.HolidayName;
                    myEvent.LastName = "";
                    myEvent.City = "";
                    myEvent.title = holiday.HolidayName;
                    myEvent.CurrentStateName = "Rejected Scheduled Work";
                    retValue.Add(myEvent);
                }

                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetRemeasureEvents() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        public bool UpdateInstallationWeekends(string id, string SaturdaySunday)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateInstallationWeekends('{0}','{1}' )", id, SaturdaySunday);
            InstallationEventWeekends eventData = null;
            bool retValue = false;
            try
            {
                eventData = new InstallationEventWeekends();
                eventData.id = id;
                if (SaturdaySunday == "both")
                {
                    eventData.Saturday = "Yes";
                    eventData.Sunday = "Yes";
                }
                else if (SaturdaySunday == "none")
                {
                    eventData.Saturday = "No";
                    eventData.Sunday = "No";
                }
                else if (SaturdaySunday == "saturday")
                {
                    eventData.Saturday = "Yes";
                    eventData.Sunday = "No";
                }
                else if (SaturdaySunday == "sunday")
                {
                    eventData.Saturday = "No";
                    eventData.Sunday = "Yes";
                }

                RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
                retValue = runner.ProcessUpdate(Utils.ContentTypeParser.GetType("Installation"), eventData);

                Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateInstallationWeekends id= {0}", id);
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<Holiday> Idata.GetHolidayEvents(string start, string end)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting HolidayEvent({0},'{1}' )", start, end);
            List<Holiday> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(start, end);
                retValue = getter.GetHolidayData();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetEvents() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }



        List<Generics.Utils.CalendarEvent> Idata.GetBufferJobs(string type, string branch, string jobType, string shippingType)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Entering GetBufferJobs({0},'{1}','{2}','{3}')", type, branch ?? "NULL", jobType ?? "NULL", shippingType ?? "NULL");
            List<CalendarEvent> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.BufferedDataGetter(new List<string>(branch.Split(',')), new List<string>(jobType.Split(',')), new List<string>(shippingType.Split(',')));
                retValue = getter.GetData(Utils.ContentTypeParser.GetType(type));
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetBufferJobs() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<InstallationEvent> Idata.GetInstallationBufferJobs(string branch)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Entering InstallationBufferJob");
            List<InstallationEvent> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.BufferedDataGetter(new List<string>(branch.Split(',')));
                retValue = getter.GetInstallationBufferData();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetInstallationBufferJobs() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<RemeasureEvent> Idata.GetRemeasureBufferJobs(string branch)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Entering GetRemeasureBufferJobs");
            List<RemeasureEvent> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.BufferedDataGetter(new List<string>(branch.Split(',')));
                retValue = getter.GetRemeasureBufferData();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetInstallationBufferJobs() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }


        List<GlobalValues> Idata.GetSystemParameters(string type, DateTime start, DateTime end)
        {
            CalendarSystem.Utils.Data.Getters.ISystemParameterGetter getter = new CalendarSystem.Utils.Data.Getters.SystemParameterGetter();
            return getter.GetGlobalValues(type, start, end);
        }


        public List<Holiday> GetHolidays(string start, string end)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting holidays({0},'{1}' )", start, end);
            List<Holiday> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(start, end);
                retValue = getter.GetHolidayData();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetEvents() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        public bool UpdateReturnedJobSchedule(string id, string scheduledStartDate, string scheduledEndDate, string returnTripReason)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateInstallationWeekends('{0}','{1}',{2} )", id, scheduledStartDate, scheduledEndDate);
            ImproperInstallationEvent eventData = null;
            bool retValue = false;
            try
            {
                eventData = new ImproperInstallationEvent();
                eventData.id = id;

                eventData.start = scheduledStartDate;
                eventData.end = scheduledEndDate;
                eventData.ReturnTripReason = returnTripReason;

                RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
                retValue = runner.ProcessUpdateReturnedJob(Utils.ContentTypeParser.GetType("Installation"), eventData);

                Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateReturnedJobSchedule id= {0}", id);
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }


        public bool UpdateInstallationSchedule(string id, string scheduledStartDate, string scheduledEndDate)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateInstallationSchedule('{0}','{1}',{2} )", id, scheduledStartDate, scheduledEndDate);
            ImproperInstallationEvent eventData = null;
            bool retValue = false;
            try
            {
                eventData = new ImproperInstallationEvent();
                eventData.id = id;

                eventData.start = Convert.ToDateTime(scheduledStartDate).ToString("yyyy-MM-ddT00:00:00.000Z");
                eventData.end = Convert.ToDateTime(scheduledEndDate).ToString("yyyy-MM-ddT00:00:00.000Z");

                RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
                retValue = runner.ProcessUpdate(Utils.ContentTypeParser.GetType("Installation"), eventData);

                Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateReturnedJobSchedule id= {0}", id);
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }
        /// <summary>
        /// update event from pop up
        /// </summary>
        /// <param name="id"></param>
        /// <param name="scheduledStartDate"></param>
        /// <param name="scheduledEndDate"></param>
        /// <returns></returns>
        public bool UpdateInstallationData(
                string id
                , string scheduledStartDate
                , string scheduledEndDate
                 , string startTime
                , string endTime
                , int Asbestos
                , int WoodDropOff
                , string woodDropOffDate
                , string woodDropOffTime
                , int HighRisk
                 , int EstInstallerCnt
                , string Saturday
                 , string Sunday
                , string LeadPaint
                , string isAllDayChecked

            )
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateInstallationData('{0}')", id);
            InstallationDataEvent eventData = null;
            bool retValue = false;
            string timeGap = " 00:00";
            try
            {
                eventData = new InstallationDataEvent();
                eventData.id = id;

                eventData.AllDay = isAllDayChecked;
                               
                eventData.start = Convert.ToDateTime(scheduledStartDate + " " + startTime).ToString("yyyy-MM-ddTHH:mm:00.000Z");
                eventData.end = Convert.ToDateTime(scheduledEndDate + " " + endTime).ToString("yyyy-MM-ddTHH:mm:00.000Z");

                if (scheduledStartDate == scheduledEndDate)
                {
                    timeGap = " 23:59";
                }
                if (isAllDayChecked == "Yes")
                {
                    eventData.start = Convert.ToDateTime(scheduledStartDate + " 00:00").ToString("yyyy-MM-ddTHH:mm:00.000Z");
                    eventData.end = Convert.ToDateTime(scheduledEndDate + timeGap).ToString("yyyy-MM-ddTHH:mm:00.000Z");
                }


                eventData.Asbestos = Asbestos;
                eventData.WoodDropOff = WoodDropOff;
                eventData.HighRisk = HighRisk;
                eventData.LeadPaint = LeadPaint;
                eventData.EstInstallerCnt = EstInstallerCnt;
                if (woodDropOffDate.Length > 1)
                {
                    // eventData.WoodDropDateAndTime = Convert.ToDateTime(woodDropOffDate + " " + woodDropOffTime).ToString("yyyy-MM-ddTHH:mm:tt.000Z");
                    eventData.WoodDropDateAndTime = Convert.ToDateTime(woodDropOffDate + " " + woodDropOffTime).ToString();
                }

                eventData.Saturday = Saturday;
                eventData.Sunday = Sunday;



                RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
                retValue = runner.ProcessUpdate(Utils.ContentTypeParser.GetType("Installation"), eventData);

                Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateInstallationData id= {0}", id);
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        public bool UpdateRemeasureData(
               string id
               , string remeasureDate
               , string remeasureEndDate
               , string fromPopup
               , string currentState
           )
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateRemeasureData('{0}')", id);
            ImproperRemeasureEvent eventData = null;
            bool retValue = false;
            try
            {
                eventData = new ImproperRemeasureEvent();
                eventData.id = id;
                eventData.CurrentStateName = currentState;


                if (fromPopup == "yes")
                {
                    // eventData.start = Convert.ToDateTime(remeasureDate).ToString("yyyy-MM-ddT00:00:00.000Z");
                    // eventData.start = Convert.ToDateTime(remeasureDate).ToString("yyyy-MM-ddTHH:mm:ssZ");
                    eventData.start = Convert.ToDateTime(remeasureDate).ToString(@"yyyy-MM-dd\THH:mm:ss.fff\Z");
                    //YYYY-MM-DD[T]HH:mm:ss
                }
                else
                {
                    //  eventData.start = Convert.ToDateTime(remeasureDate).AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ");
                    eventData.start = Convert.ToDateTime(remeasureDate).AddDays(1).ToString(@"yyyy-MM-dd\THH:mm:ss.fff\Z");
                }
                if (remeasureEndDate == "undefined")
                {
                    eventData.end = null;
                }
                else
                {
                    DateTime dt = Convert.ToDateTime(Convert.ToDateTime(remeasureDate).Date.AddHours(Convert.ToDouble(remeasureEndDate.Split(':')[0])).AddMinutes(Convert.ToDouble(remeasureEndDate.Split(':')[1])));
                    // eventData.end = dt.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    eventData.end = dt.ToString(@"yyyy-MM-dd\THH:mm:ss.fff\Z");

                }


                RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
                retValue = runner.ProcessUpdate(Utils.ContentTypeParser.GetType("Remeasure"), eventData);

                Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateRemeasureData id= {0}", id);
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }


        public bool UploadDocument(string workOrderNumber, string fileName, string fileSource)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "UploadDocument('{0}')", workOrderNumber);
            DocumentFile eventData = null;
            bool retValue = false;
            try
            {
               
               // List<DocumentFile> keepedFiles = GetKeepedFiles(id, WO, recordID);
                List<DocumentFile> fileList = new List<DocumentFile>();
              //  fileList = keepedCalledLog.ToList();
                eventData = new DocumentFile();
                eventData.id = GetActionItemIDByWO(workOrderNumber);
                eventData.FileName = fileName;
                eventData.FileSource = System.Convert.FromBase64String(fileSource);
                fileList.Add(eventData);
                //eventData.CalledMessage = calledMessage;
                ////  eventData.DateCalled = Generics.Utils.Date.DateParser.ParseTime(Convert.ToDateTime(callDate).ToString("yyyy-MM-ddTHH:mm:00.000Z")); ;
                //if (callDate.Length != 0)
                //{
                //    eventData.DateCalled = Convert.ToDateTime(callDate + " " + callTime).ToString();
                //    calledLogList.Add(eventData);
                //}

                RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
                retValue = runner.ProcessUpdate(Utils.ContentTypeParser.GetType("Installation"), fileList);

                Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateCallLogData id= {0}", workOrderNumber);
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }


        public bool UpdateCallLogData(
               string id
             , string WO
              , string recordID
               , string callDate
               , string callTime
               , string calledMessage
               , string Notes
           )
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateCallLogData('{0}')", id);
            CalledLog eventData = null;
            bool retValue = false;
            string parRecordID = String.Empty;
            try
            {
                List<CalledLog> keepedCalledLog = GetKeepedCalledLog(id, WO, recordID);
                List<CalledLog> calledLogList = new List<CalledLog>();
                parRecordID = GetActionItemIDByWO(WO);
                calledLogList = keepedCalledLog.ToList();
                eventData = new CalledLog();
                eventData.id = id;
                eventData.Notes3 = Notes;
                eventData.CalledMessage = calledMessage;
                //  eventData.DateCalled = Generics.Utils.Date.DateParser.ParseTime(Convert.ToDateTime(callDate).ToString("yyyy-MM-ddTHH:mm:00.000Z")); ;
                if (callDate.Length != 0)
                {
                    eventData.DateCalled = Convert.ToDateTime(callDate + " " + callTime).ToString();
                    calledLogList.Add(eventData);
                }
                RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
                retValue = runner.ProcessUpdate(Utils.ContentTypeParser.GetType("Installation"), calledLogList,parRecordID);

                Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateCallLogData id= {0}", id);
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        public bool UpdateCrewData(
             string recordID,
             int IsAdd,
            string WO
           )
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateCrewData('{0}')", recordID);
          //  InstallerWithLessInfo eventData = null;
            bool retValue = false;
            List<InstallerWithLessInfo> keepedInstaller = null;
            List<InstallerWithLessInfo> addedInstaller = new List<InstallerWithLessInfo>();
            List<InstallerWithLessInfo> crewList = new List<InstallerWithLessInfo>();
            string ss;
            string parRecordID = string.Empty;
            try
            {
                keepedInstaller = GetKeepedInstaller(WO, recordID);
                parRecordID = GetActionItemIDByWO(WO);
                if (IsAdd == 1) //add
                {
                    foreach (var s in recordID.Split(','))
                    {
                        crewList.Add(GetAddedInstaller(WO, s));
                    }

                }


                foreach (var s in crewList)
                {
                    // s.id = "VVU691400";
                    s.id = parRecordID;
                }
                
                foreach(var dd in keepedInstaller)
                {
                    crewList.Add(dd);
                }
              
               
                //eventData = new InstallerWithLessInfo();
                //eventData.DetailedRecordid = recordID;
                //eventData.ParentRecordid= recordID;
                
                RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
                retValue = runner.UpdateInstalltionCrew(Utils.ContentTypeParser.GetType("Installation"), crewList, parRecordID);

                Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateCrewData id= {0}", recordID);
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        public bool UpdateTruckInstalltionCrew(
            string recordID,
            int IsAdd,
           string detailedRecordID
          )
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateTruckInstaller('{0}')", recordID);
            //  InstallerWithLessInfo eventData = null;
            bool retValue = false;
            List<InstallerWithLessInfo> keepedTruckInstaller = null;
            List<InstallerWithLessInfo> addedTruckInstaller = new List<InstallerWithLessInfo>();
            List<InstallerWithLessInfo> crewList = new List<InstallerWithLessInfo>();
            
            string parRecordID = string.Empty;
            try
            {
                keepedTruckInstaller = GetKeepedTruckInstaller(recordID,detailedRecordID);
                parRecordID = GetActionItemIDByRecordID(recordID);
                if (IsAdd == 1) //add
                {
                    foreach (var s in detailedRecordID.Split(','))
                    {
                        crewList.Add(GetTruckCrewsByUserID( s));
                    }

                }


                foreach (var s in crewList)
                {
                    s.id = parRecordID;
                }

                foreach (var dd in keepedTruckInstaller)
                {
                    crewList.Add(dd);
                }
                RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
                if (crewList.Count!=0)
                {
                    retValue = runner.UpdateTruckInstalltionCrew(Utils.ContentTypeParser.GetType("Installation"), crewList, detailedRecordID);
                }
                else
                {
                    retValue = runner.UpdateTruckInstalltionCrew(Utils.ContentTypeParser.GetType("Installation"), crewList, parRecordID);
                }
                
                

                Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateTruckInstaller id= {0}", recordID);
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        public bool UpdateNotesData(
               string id
             , string WO
              , string recordID
               , string notesDate
               , string notesTime
               , string category
               , string Notes
           )
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateNotesData('{0}')", id);
            Notes eventData = null;
            bool retValue = false;
            string parRecordID = string.Empty;
            try
            {
                List<Notes> keepedNotes = GetKeepedNotes(id, WO, recordID);
                List<Notes> notesList = new List<Notes>();
                parRecordID = GetActionItemIDByWO(WO);
                notesList = keepedNotes.ToList();
                eventData = new Notes();
                eventData.id = id;
                eventData.GeneralNotes = Notes;
                eventData.Category = category;
                //  eventData.DateCalled = Generics.Utils.Date.DateParser.ParseTime(Convert.ToDateTime(callDate).ToString("yyyy-MM-ddTHH:mm:00.000Z")); ;
                if (notesDate.Length != 0)
                {
                    eventData.NotesDate = Convert.ToDateTime(notesDate + " " + notesTime).ToString();
                    notesList.Add(eventData);
                }
                RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
                retValue = runner.ProcessUpdate(Utils.ContentTypeParser.GetType("Installation"), notesList,parRecordID);

                Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateNotesData id= {0}", id);
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }


        public bool UpdateTruckWithWo(
              string ActionItemId
             , string TruckName
            , string TruckID
          )
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateTruckWithWo('{0}')", ActionItemId);
          
            ImproperTruckInstallationEvent eventData = null;

            bool retValue = false;
            try
            {
                eventData = new ImproperTruckInstallationEvent();
                eventData.id = ActionItemId;
                eventData.TruckName = TruckName;
                eventData.TruckID = TruckID;
                

                RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
                retValue = runner.ProcessUpdate(Utils.ContentTypeParser.GetType("Installation"), eventData);

                Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateTruckWithWo id= {0}", ActionItemId);
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        public bool UpdateTruckInstallationSchedule(
           string ActionItemId
          , string startDate
         , string startTime
         , string endDate
         , string endTime
         , string isAllDayChecked
       )
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateTruckInstallationSchedule('{0}')", ActionItemId);

            ImproperInstallationEvent eventData = null;

            bool retValue = false;
            string timeGap = " 00:00";
            try
            {
                eventData = new ImproperInstallationEvent();
                eventData.id = ActionItemId;
                eventData.AllDay = isAllDayChecked;

                eventData.start = Convert.ToDateTime(startDate + " " + startTime).ToString("yyyy-MM-ddTHH:mm:00.000Z");
                eventData.end = Convert.ToDateTime(endDate + " " + endTime).ToString("yyyy-MM-ddTHH:mm:00.000Z");

                if (startDate== endDate)
                {
                    timeGap = " 23:59";
                }
                if (isAllDayChecked=="Yes")
                {
                    eventData.start = Convert.ToDateTime(startDate + " 00:00").ToString("yyyy-MM-ddTHH:mm:00.000Z");
                    eventData.end = Convert.ToDateTime(endDate + timeGap).ToString("yyyy-MM-ddTHH:mm:00.000Z");
                }
        
                RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
                retValue = runner.ProcessUpdate(Utils.ContentTypeParser.GetType("Installation"), eventData);

                Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateTruckInstallationSchedule id= {0}", ActionItemId);
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }




        List<Product> Idata.GetProducts(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetProducts({0})", workOrderNumber);
            List<Product> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetProducts();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetProducts() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }


        List<Product> Idata.GetProductsDoors(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetProductsDoors({0})", workOrderNumber);
            List<Product> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetProductsDoors();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetProductsDoors() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }


        List<UnavailableHR> Idata.GetUnavailableResources(string branch)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetUnavailableResources({0})", branch);
            List<UnavailableHR> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(new List<string>(branch.Split(',')));
                retValue = getter.GetUnavailableResources();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetUnavailableResources() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }


        List<Product> Idata.GetManufacturingWindows(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetManufacturingWindows({0})", workOrderNumber);
            List<Product> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetManufacturingWindows();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetManufacturingWindows() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<Product> Idata.GetManufacturingDoors(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetManufacturingDoors({0})", workOrderNumber);
            List<Product> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetManufacturingDoors();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetManufacturingDoors() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<Installer> Idata.GetInstallers(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetInstallers({0})", workOrderNumber);
            List<Installer> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetInstallers();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetInstallers() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<Notes> Idata.GetNotes(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetNotes({0})", workOrderNumber);
            List<Notes> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetNotes();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetNotes() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<string> Idata.GetRemeasurerName(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetRemeasurerName({0})", workOrderNumber);
            List<string> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetRemeasurerName();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetRemeasurerName() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }



        List<CalledLog> Idata.GetCalledLog(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetCalledLog({0})", workOrderNumber);
            List<CalledLog> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetCalledLog();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetCalledLog() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<CalledLog> GetKeepedCalledLog(string id, string workOrderNumber, string recordID)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetCalledLog({0})", workOrderNumber);
            List<CalledLog> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetKeepedCalledLog(recordID);
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetCalledLog() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<InstallerWithLessInfo> GetKeepedInstaller(string workOrderNumber, string recordID)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetKeepedInstaller({0})", workOrderNumber);
            List<InstallerWithLessInfo> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetKeepedInstaller(recordID);
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetCalledLog() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }


        List<InstallerWithLessInfo> GetKeepedTruckInstaller(string recordID, string detailedRecordID )
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetKeepedTruckInstaller({0})", detailedRecordID);
            List<InstallerWithLessInfo> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter();
                retValue = getter.GetKeepedTruckInstaller(recordID,detailedRecordID);
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetKeepedTruckInstaller() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        string GetActionItemIDByWO(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetActionItemIDByWO({0})", workOrderNumber);
            string retValue = string.Empty;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetActionItemIDByWO();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetCalledLog() = {0}", retValue);
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        string GetActionItemIDByRecordID(string recordID)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetActionItemIDByRecordID({0})", recordID);
            string retValue = string.Empty;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter();
                retValue = getter.GetActionItemIDByRecordID(recordID);
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetActionItemIDByRecordID() = {0}", retValue);
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        

        InstallerWithLessInfo GetAddedInstaller(string workOrderNumber, string recordID)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetAddedInstaller({0})", workOrderNumber);
            InstallerWithLessInfo retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetAddedInstaller(recordID);
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetCalledLog() = {0}", retValue.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        InstallerWithLessInfo GetAddedTruckInstaller(string detailedRecordID)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetAddedTruckInstaller({0})", detailedRecordID);
            InstallerWithLessInfo retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter();
                retValue = getter.GetTruckAddedInstaller(detailedRecordID);
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetAddedTruckInstaller() = {0}", retValue.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        InstallerWithLessInfo GetTruckCrewsByUserID(string userID)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetTruckCrewsByUserID({0})", userID);
            InstallerWithLessInfo retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter();
                retValue = getter.GetTruckCrewsByUserID(userID);
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetTruckCrewsByUserID() = {0}", retValue.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }



        List<Notes> GetKeepedNotes(string id, string workOrderNumber, string recordID)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetKeepedNotes({0})", workOrderNumber);
            List<Notes> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetKeepedNotes(recordID);
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetKeepedNotes() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }


       public  List<TruckInstallationEvent> GetTruckInstallationEvent()
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetTruckInstallationEventSQL({0})");
            List<TruckInstallationEvent> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter();
                retValue = getter.GetTruckInstallationEvent();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetTruckInstallationEvent() = {0}", retValue.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }


        List<WindowsCustomer> Idata.GetWindowsCustomer(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetWindowsCustomer({0})", workOrderNumber);
            List<WindowsCustomer> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetWindowsCustomer();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetWindowsCustomer() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<SubTrades> Idata.GetSubTrades(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetSubTrades({0})", workOrderNumber);
            List<SubTrades> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetSubTrades();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetSubTrades() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<WOPicture> Idata.GetWOPicture(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetWOPicture({0})", workOrderNumber);
            List<WOPicture> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetWOPicture();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetWOPictureGetWOPicture() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<DocumentLibrary> Idata.GetDocumentLibrary(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetDocumentLibrary({0})", workOrderNumber);
            List<DocumentLibrary> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetDocumentLibrary();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetDocumentLibrary() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }


        List<JobReview> Idata.GetJobReview(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetJobReview({0})", workOrderNumber);
            List<JobReview> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetJobReview();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetJobReview() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }



        List<WOPicture> Idata.GetWOBigPicture(int recordid)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetWOBigPicture({0})", recordid);
            List<WOPicture> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(recordid);
                retValue = getter.GetWOBigPicture(recordid);
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetWOBigPicture() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<InstallerInfo> Idata.GetInstallerInfoByWorkOrder(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetInstallerInfo({0})", workOrderNumber);
            List<InstallerInfo> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetInstallerInfoByWorkOrder();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetInstallerInfo() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

       InstallerInfoWithImage Idata.GetInstallerInfoByRecordID(string recordid)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetInstallerInfoByRecordID({0})", recordid);
            InstallerInfoWithImage retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(recordid);
                retValue = getter.GetInstallerInfoByRecordID(recordid);
              //  Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetInstallerInfoByRecordID() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<InstallerInfo> Idata.GetInstallerListByTruck(string recordid)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetInstallerListByTruck({0})", recordid);
            List<InstallerInfo> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter();
                retValue = getter.GetInstallerListByTruck(recordid);
                //  Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetInstallerInfoByRecordID() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<string> Idata.GetUserIdListByTruckRecordID(string recordid)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetUserIdListByTruckRecordID({0})", recordid);
            List<string> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter();
                retValue = getter.GetUserIdListByTruckRecordID(recordid);
                //  Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetInstallerInfoByRecordID() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<InstallerInfo> Idata.GetTruckInstallersExcludeUserIDs(string userID,string name)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetTruckInstallersExcludeUserIDs({0})", userID);
            List<InstallerInfo> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter();
                retValue = getter.GetTruckInstallersExcludeUserIDs(userID,name);
                //  Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetInstallerInfoByRecordID() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }




        List<InstallerInfo> Idata.GetInstallerInfoExceptWorkOrder(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetInstallerInfo({0})", workOrderNumber);
            List<InstallerInfo> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetInstallerInfoExceptWorkOrder();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetInstallerInfo() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<InstallerInfo> Idata.GetResources(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetResources({0})");
            List<InstallerInfo> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetResources();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetInstallerInfo() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<Truck> Idata.GetTruckList()
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetTruckList({0})");
            List<Truck> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter();
                retValue = getter.GetTruckList();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetTruckList() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<TruckWithWO> Idata.GetTruckListWithWO()
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetTruckListWithWO({0})");
            List<TruckWithWO> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter();
                retValue = getter.GetTruckListWithWO();
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetTruckListWithWO() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<ImproperTruckInstallationEvent> Idata.GetTruckInstallationEventsByWO(string WO)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetTruckInstallationEventsByWO({0})");
            List<ImproperTruckInstallationEvent> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter();
                retValue = getter.GetTruckInstallationEventsByWO(WO);
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetTruckInstallationEventsByWO() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }



        List<InstallerInfo> Idata.GetInstallerInfoByNameExceptWorkOrder(string workOrderNumber, string name)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetInstallerInfo({0})", workOrderNumber);
            List<InstallerInfo> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetInstallerInfoByNameExceptWorkOrder(name);
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetInstallerInfo() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<DocumentFile> Idata.GetDocumentFile(int recordid)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetDocumentFile({0})", recordid);
            List<DocumentFile> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(recordid);
                retValue = getter.GetDocumentFile(recordid);
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetDocumentFile() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }



        List<CalledLog> Idata.GetCallLogByID(int recordid)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetCallLogByID({0})", recordid);
            List<CalledLog> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(recordid);
                retValue = getter.GetCallLogByID(recordid);
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetCallLogByID() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<Notes> Idata.GetNotesByID(int recordid)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetNotesByID({0})", recordid);
            List<Notes> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(recordid);
                retValue = getter.GetNotesByID(recordid);
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetNotesByID() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<InstallationEvent> Idata.GetInstallationDateByWOForReturnedJob(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetInstallationDateByWOForReturnedJob({0})", workOrderNumber);
            List<InstallationEvent> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetInstallationDateByWOForReturnedJob(workOrderNumber);
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetProducts() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        List<InstallationEvent> Idata.GetInstallationDateByWOForNonReturnedJob(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Getting GetInstallationDateByWOForNonReturnedJob({0})", workOrderNumber);
            List<InstallationEvent> retValue = null;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetInstallationDateByWOForNonReturnedJob(workOrderNumber);
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Leaving GetProducts() = {0}", retValue.Count.ToString());
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

    }
    

}
