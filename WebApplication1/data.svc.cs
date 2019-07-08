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
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(start, end,  new List<string>(branch.Split(',')), new List<string>(state.Split(',')));
                retValue = getter.GetData();

                Utils.Data.IGetter getter1 = new Utils.Data.EventDataGetter(start, end);
                retValue1 = getter1.GetHolidayData();

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
               , string fromPopup
           )
        {
            Lift.LiftManager.Logger.Write(this.GetType().Name, "UpdateRemeasureData('{0}')", id);
            ImproperRemeasureEvent eventData = null;
            bool retValue = false;
            try
            {
                eventData = new ImproperRemeasureEvent();
                eventData.id = id;
                
                if (fromPopup=="yes")
                {
                    eventData.start = Convert.ToDateTime(remeasureDate).ToString("yyyy-MM-ddT00:00:00.000Z");
                }
                else
                {
                    eventData.start = Convert.ToDateTime(remeasureDate).AddDays(1).ToString("yyyy-MM-ddT00:00:00.000Z");
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
