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
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Entering GetEvents({0},'{1}','{2}','{3}','{4}','{5}', '{6}' )", start, end, type, states??"NULL", branch??"NULL", jobType??"NULL", shippingType??"NULL");
            List<CalendarEvent> retValue = null;
            List<Holiday> retValue1 = null;
            try {
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
            } catch(Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        public List<InstallationEvent> GetInstallationEvents(string start, string end, string branch,string state)
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "Entering InstallationEvent({0},'{1}','{2}' )", start, end, branch ?? "NULL", state ?? "NULL");
            List<InstallationEvent> retValue = null;
            List<Holiday> retValue1 = null;
            List<UnavailableHR> retValue2 = null;
          //  List<string> unavailableStaffName = new List<string>();

            Dictionary<string, string> unavailableStaffList = new Dictionary<string, string>();
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(start, end,  new List<string>(branch.Split(',')), new List<string>(state.Split(',')));
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
                    myEvent.allDay = true;
                    myEvent.HolidayDate = holiday.HolidayDate;
                    myEvent.start = holiday.HolidayDate.ToShortDateString();
                    myEvent.end = holiday.HolidayDate.ToShortDateString();
                    myEvent.HolidayName = holiday.HolidayName;
                    myEvent.LastName ="";
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
                    myEvent.allDay = true;
                    myEvent.start = un.DateUnavailable;
                    myEvent.end = un.DateUnavailable;
                    myEvent.WorkOrderNumber = un.Name + un.DateUnavailable;
                    // myEvent.ScheduledDate = Convert.ToDateTime(un.DateUnavailable);
                    // myEvent.WorkOrderNumber = un.DateUnavailable;
                    //    myEvent.WorkOrderNumber = un.Name;

                    myEvent.LastName ="";

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
                if (SaturdaySunday=="both")
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
                retValue =  runner.ProcessUpdate(Utils.ContentTypeParser.GetType("Installation"), eventData);

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

        public bool UpdateReturnedJobSchedule(string id, string scheduledStartDate, string scheduledEndDate)
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
                ,string scheduledStartDate 
                ,string scheduledEndDate
                ,int Asbestos
                , int WoodDropOff
                , string woodDropOffDate
                , string woodDropOffTime

                , int HighRisk
                 , int EstInstallerCnt
                , string Saturday
                 , string Sunday
                , string LeadPaint
            )
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateInstallationData('{0}')", id);
            InstallationDataEvent eventData = null;
            bool retValue = false;
            try
            {
                eventData = new InstallationDataEvent();
                eventData.id = id;

                eventData.start = Convert.ToDateTime(scheduledStartDate).ToString("yyyy-MM-ddT00:00:00.000Z");
                eventData.end = Convert.ToDateTime(scheduledEndDate).ToString("yyyy-MM-ddT00:00:00.000Z");

                eventData.Asbestos = Asbestos;
                eventData.WoodDropOff = WoodDropOff;
                eventData.HighRisk = HighRisk;
                eventData.LeadPaint = LeadPaint;
                eventData.EstInstallerCnt = EstInstallerCnt;
                if (woodDropOffDate.Length>1)
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


                if (fromPopup=="yes")
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



        public bool UpdateCallLogData(
               string id
             ,  string WO
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
            try
            {
               List<CalledLog> keepedCalledLog =  GetKeepedCalledLog(id, WO,recordID);
                List<CalledLog> calledLogList = new List<CalledLog>();
                calledLogList = keepedCalledLog.ToList();
                eventData = new CalledLog();
                eventData.id = id;
                eventData.Notes3 = Notes;
                eventData.CalledMessage = calledMessage;
                //  eventData.DateCalled = Generics.Utils.Date.DateParser.ParseTime(Convert.ToDateTime(callDate).ToString("yyyy-MM-ddTHH:mm:00.000Z")); ;
                if (callDate.Length!=0)
                {
                    eventData.DateCalled = Convert.ToDateTime(callDate + " " + callTime).ToString();
                    calledLogList.Add(eventData);
                }
                RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
                retValue = runner.ProcessUpdate(Utils.ContentTypeParser.GetType("Installation"), calledLogList);

                Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateCallLogData id= {0}", id);
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
            try
            {
                List<Notes> keepedNotes = GetKeepedNotes(id, WO, recordID);
                List<Notes> notesList = new List<Notes>();
                notesList = keepedNotes.ToList();
                eventData = new Notes();
                eventData.id = id;
                eventData.GeneralNotes = Notes;
                eventData.Category = category;
                //  eventData.DateCalled = Generics.Utils.Date.DateParser.ParseTime(Convert.ToDateTime(callDate).ToString("yyyy-MM-ddTHH:mm:00.000Z")); ;
                if (notesDate.Length != 0)
                {
                    eventData.NotesDate= Convert.ToDateTime(notesDate + " " + notesTime).ToString();
                    notesList.Add(eventData);
                }
                RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
                retValue = runner.ProcessUpdate(Utils.ContentTypeParser.GetType("Installation"), notesList);

                Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateNotesData id= {0}", id);
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

        List<CalledLog> GetKeepedCalledLog(string id, string workOrderNumber,string recordID)
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
