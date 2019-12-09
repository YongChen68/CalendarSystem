using Generics.Utils;
using System;
using System.Collections.Generic;

namespace CalendarSystem
{
    public partial class _Default : System.Web.UI.Page
    {
        public string branchHtml = "";
        protected bool ReadOnly { get; private set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            ReadOnly = Lift.LiftManager.ConfigProvider.GetValue("ReadOnly").Equals("true", StringComparison.InvariantCultureIgnoreCase);
            string s = Request.Params[0];
            try
            {
                branchHtml = PrepareBranchHTMLfromList(GetBranchList());
            }catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write(this.GetType().Name, "Error occured while retrieving Branch information. Error message:\r\n{0}", ex.ToString());
            }
        }

        private static List<Branch> GetBranchList()
        {
            return Lift.LiftManager.DbHelper.ReadObjects<Branch>
            //    (@"select BranchName as Name, ActionItemId as Id from Branch b with(nolock,noexpand) Where BranchName NOT IN('CEL Branch', 'CRL Branch', 'Unknown', 'Langley HVAC') order by b.BranchName");
            (@"select BranchName as Name, ActionItemId as Id from Branch b with(nolock,noexpand) Where Showincalendar = 'Yes' order by b.BranchName");
        }

        private string PrepareBranchHTMLfromList(List<Branch> branchList)
        {
            string retValue = "";
            if (branchList != null && branchList.Count > 0)
                foreach (var branch in branchList)
                    retValue += (retValue.Length > 0 ? "<br/>" : "") + String.Format("<input type=\"checkbox\" name=\"branch\" value=\"{0}\" checked>{1}", branch.Id, branch.Name??"NOT SPECIFIED");
            return retValue;
        }

        public static string GetActionItemIDByWO(string workOrderNumber)
        {
            Lift.LiftManager.Logger.Write("Getting GetActionItemIDByWO({0})", workOrderNumber);
            string retValue = string.Empty;
            try
            {
                Utils.Data.IGetter getter = new Utils.Data.EventDataGetter(workOrderNumber);
                retValue = getter.GetActionItemIDByWO();
                Lift.LiftManager.Logger.Write("Leaving GetCalledLog() = {0}", retValue);
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write("Error occured: {0}", ex.ToString());
            }
            return retValue;
        }

        //this method only updates start and end time
        //this is called when a event is dragged or resized in the calendar
        [System.Web.Services.WebMethod(true)]
        public static bool UploadDocumentFiles(string WO, string fileName, string data)
        {
            string aa = fileName;
            string bb = data;
            Lift.LiftManager.Logger.Write( "UploadDocument('{0}')", WO);
            DocumentFile eventData = null;
            bool retValue = false;
            try
            {

                // List<DocumentFile> keepedFiles = GetKeepedFiles(id, WO, recordID);
                List<DocumentFile> fileList = new List<DocumentFile>();
                //  fileList = keepedCalledLog.ToList();
                eventData = new DocumentFile();
                eventData.id = GetActionItemIDByWO(WO);
                eventData.FileName = fileName;
                eventData.FileSource = System.Convert.FromBase64String(data);
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

                Lift.LiftManager.Logger.Write("UpdateCallLogData id= {0}", WO);
            }
            catch (Exception ex)
            {
                Lift.LiftManager.Logger.Write("Error occured: {0}", ex.ToString());
            }
            return retValue;
            return true;
        }

        [System.Web.Services.WebMethod(true)]
        public static bool UpdateEventTime(string type, Generics.Utils.ImproperCalendarEvent eventData)
        {
            if (eventData.end == eventData.start)
            {
                eventData.end = Convert.ToDateTime(eventData.end).AddDays(1).ToString("yyyy-MM-ddT00:00:00.000Z");
            }
            RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
            return runner.ProcessUpdate(Utils.ContentTypeParser.GetType(type), eventData);
        }

        [System.Web.Services.WebMethod(true)]
        public static bool UpdateInstallationEventTime(string type, Generics.Utils.ImproperInstallationEvent eventData)
        {
            // eventData.end =Convert.ToDateTime(eventData.end).AddDays(-1).ToShortDateString();
            //eventData.end = Convert.ToDateTime(eventData.end).AddDays(0).ToString("yyyy-MM-ddT00:00:00.000Z");
            if (eventData.end== eventData.start)
            {
                eventData.end = Convert.ToDateTime(eventData.end).AddDays(1).ToString("yyyy-MM-ddT00:00:00.000Z");
            }
            else
            {
                // eventData.end = Convert.ToDateTime(eventData.end).AddDays(1).ToString("yyyy-MM-ddT00:00:00.000Z");
               // eventData.end = Convert.ToDateTime(eventData.end).AddDays(0).ToString("yyyy-MM-ddT00:00:00.000Z");
            }
         

            RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
            return runner.ProcessUpdate(Utils.ContentTypeParser.GetType(type), eventData);
        }

        [System.Web.Services.WebMethod(true)]
        public static bool UpdateRemeasureEventTime(string type, Generics.Utils.ImproperRemeasureEvent eventData)
        {
            RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
            return runner.ProcessUpdate(Utils.ContentTypeParser.GetType(type), eventData);
        }

        [System.Web.Services.WebMethod(true)]
        public static bool UpdateInstallationEventForSaturday(string type, Generics.Utils.ImproperInstallationEvent eventData)
        {
            RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
            return runner.ProcessUpdate(Utils.ContentTypeParser.GetType(type), eventData);
        }

        [System.Web.Services.WebMethod(true)]
        public static bool UpdateInstallationEventForSunday(string type, Generics.Utils.ImproperInstallationEvent eventData)
        {
            RuntimeHelper.Runtime runner = new RuntimeHelper.Runtime();
            return runner.ProcessUpdate(Utils.ContentTypeParser.GetType(type), eventData);
        }



        [System.Web.Services.WebMethod(true)]
        public static int AddEvent(Generics.Utils.ImproperCalendarEvent eventData)
        {
             return -1;
        }
    }
}