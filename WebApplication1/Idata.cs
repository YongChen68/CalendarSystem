using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Generics.Utils;
using Generics.Utils.Data;

namespace CalendarSystem
{
    
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "Idata" in both code and config file together.
    [ServiceContract]
    public interface Idata
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "GetEvents?start={start}&end={end}&type={type}&states={states}&branch={branch}&jobType={jobType}&shippingType={shippingType}",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Generics.Utils.CalendarEvent> GetEvents(string start, string end, string type, string states, string branch, string jobType, string shippingType);

        [OperationContract]
        [WebInvoke(Method = "GET",
           UriTemplate = "GetInstallationEvents?start={start}&end={end}&branch={branch}&installationStates={installationStates}",
           ResponseFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Generics.Utils.Data.InstallationEvent> GetInstallationEvents(string start, string end, string branch, string installationStates);


        [OperationContract]
        [WebInvoke(Method = "GET",
           UriTemplate = "GetRemeasureEvents?start={start}&end={end}&branch={branch}&remeasureStates={remeasureStates}",
           ResponseFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Generics.Utils.Data.RemeasureEvent> GetRemeasureEvents(string start, string end, string branch, string remeasureStates);


        [OperationContract]
        [WebInvoke(Method = "POST",UriTemplate = "UpdateInstallationWeekends?id={id}&SaturdaySunday={SaturdaySunday}")]
       bool UpdateInstallationWeekends(string id, string SaturdaySunday);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateReturnedJobSchedule?id={id}&scheduledStartDate={scheduledStartDate}&scheduledEndDate={scheduledEndDate}&returnTripReason={returnTripReason}")]
        bool UpdateReturnedJobSchedule(string id, string scheduledStartDate, string scheduledEndDate, string returnTripReason);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateInstallationData?id={id}&scheduledStartDate={scheduledStartDate}&scheduledEndDate={scheduledEndDate}&Asbestos={Asbestos}" +
            "&WoodDropOff={WoodDropOff}&woodDropOffDate={woodDropOffDate}&woodDropOffTime={woodDropOffTime}&HighRisk={HighRisk}&EstInstallerCnt={EstInstallerCnt}&Saturday={Saturday}&Sunday={Sunday}&LeadPaint={LeadPaint}")]
        bool UpdateInstallationData(string id, string scheduledStartDate, string scheduledEndDate,int Asbestos, int WoodDropOff, string woodDropOffDate, string woodDropOffTime, int HighRisk, int EstInstallerCnt, string Saturday, string Sunday, string LeadPaint);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateRemeasureData?id={id}&remeasureDate={remeasureDate}&remeasureEndDate={remeasureEndDate}&fromPopup={fromPopup}&currentState={currentState}")]
        bool UpdateRemeasureData(string id, string remeasureDate, string remeasureEndDate, string fromPopup, string currentState);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateCallLogData?id={id}&WO={WO}&recordid={recordid}&callDate={callDate}&callTime={callTime}&calledMessage={calledMessage}&Notes={Notes}")]
        bool UpdateCallLogData(string id, string WO, string recordid, string callDate, string callTime, string calledMessage, string Notes);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateCrewData?recordid={recordid}&IsAdd={IsAdd}&WO={WO}")]
        bool UpdateCrewData(string recordid, int IsAdd,string WO);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateTruckInstalltionCrew?recordid={recordid}&IsAdd={IsAdd}&detailedRecordID={detailedRecordID}")]
        bool UpdateTruckInstalltionCrew(string recordid, int IsAdd, string detailedRecordID);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UploadDocument?workOrderNumber={workOrderNumber}&fileName={fileName}&fileSource={fileSource}")]
        bool UploadDocument(string workOrderNumber, string fileName, string fileSource);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateNotesData?id={id}&WO={WO}&recordid={recordid}&notesDate={notesDate}&notesTime={notesTime}&category={category}&Notes={Notes}")]
        bool UpdateNotesData(string id, string WO, string recordid, string notesDate, string notesTime, string category, string Notes);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateTruckWithWo?ActionItemId={ActionItemId}&TruckName={TruckName}&TruckID={TruckID}")]
        bool UpdateTruckWithWo(string ActionItemId,string TruckName, string TruckID);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateTruckInstallationSchedule?ActionItemId={ActionItemId}&startDate={startDate}&startTime={startTime}&endDate={endDate}&endTime={endTime}&isAllDayChecked={isAllDayChecked}")]
        bool UpdateTruckInstallationSchedule(string ActionItemId, string startDate, string startTime,string endDate, string endTime,string isAllDayChecked);



        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateInstallationSchedule?id={id}&scheduledStartDate={scheduledStartDate}&scheduledEndDate={scheduledEndDate}")]
        bool UpdateInstallationSchedule(string id, string scheduledStartDate, string scheduledEndDate);


        [OperationContract]
        [WebInvoke(Method = "GET",
       UriTemplate = "GetHolidayEvents?start={start}&end={end}",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Generics.Utils.Holiday> GetHolidayEvents(string start, string end);


        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "GetBufferJobs?type={type}&branch={branch}&jobType={jobType}&shippingType={shippingType}",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Generics.Utils.CalendarEvent> GetBufferJobs(string type, string branch, string jobType, string shippingType);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "GetInstallationBufferJobs?branch={branch}",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Generics.Utils.Data.InstallationEvent> GetInstallationBufferJobs(string branch);

        [OperationContract]
        [WebInvoke(Method = "GET",
    UriTemplate = "GetRemeasureBufferJobs?branch={branch}",
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Generics.Utils.Data.RemeasureEvent> GetRemeasureBufferJobs(string branch);


        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "GetSystemParameters?type={type}&start={start}&end={end}",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Generics.Utils.Data.GlobalValues> GetSystemParameters(string type, DateTime start, DateTime end);

        [OperationContract]
        [WebInvoke(Method = "GET",
                   UriTemplate = "GetProducts?workOrderNumber={workOrderNumber}",
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Product> GetProducts(string workOrderNumber);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "GetManufacturingWindows?workOrderNumber={workOrderNumber}",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Product> GetManufacturingWindows(string workOrderNumber);

        [OperationContract]
        [WebInvoke(Method = "GET",
    UriTemplate = "GetManufacturingDoors?workOrderNumber={workOrderNumber}",
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Product> GetManufacturingDoors(string workOrderNumber);


        [OperationContract]
        [WebInvoke(Method = "GET",
          UriTemplate = "GetProductsDoors?workOrderNumber={workOrderNumber}",
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Product> GetProductsDoors(string workOrderNumber);

        [OperationContract]
        [WebInvoke(Method = "GET",
  UriTemplate = "GetUnavailableResources?branch={branch}",
  ResponseFormat = WebMessageFormat.Json,
  BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<UnavailableHR> GetUnavailableResources(string branch);



        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "GetInstallers?workOrderNumber={workOrderNumber}",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Installer> GetInstallers(string workOrderNumber);

        [OperationContract]
        [WebInvoke(Method = "GET",
    UriTemplate = "GetCalledLog?workOrderNumber={workOrderNumber}",
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<CalledLog> GetCalledLog(string workOrderNumber);

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetInstallerInfoByWorkOrder?workOrderNumber={workOrderNumber}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<InstallerInfo> GetInstallerInfoByWorkOrder(string workOrderNumber);

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetInstallerInfoExceptWorkOrder?workOrderNumber={workOrderNumber}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<InstallerInfo> GetInstallerInfoExceptWorkOrder(string workOrderNumber);

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetResources?workOrderNumber={workOrderNumber}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<InstallerInfo> GetResources(string workOrderNumber);
        //
        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetTruckList",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Truck> GetTruckList();

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetTruckListWithWO",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<TruckWithWO> GetTruckListWithWO();


        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetInstallerInfoByNameExceptWorkOrder?workOrderNumber={workOrderNumber}&name={name}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<InstallerInfo> GetInstallerInfoByNameExceptWorkOrder(string workOrderNumber,string name);

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetTruckInstallationEventsByWO?WO={WO}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<ImproperTruckInstallationEvent> GetTruckInstallationEventsByWO(string WO);

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetInstallerInfoByRecordID?recordid={recordid}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        InstallerInfoWithImage GetInstallerInfoByRecordID(string recordid);

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetInstallerListByTruck?recordid={recordid}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<InstallerInfo> GetInstallerListByTruck(string recordid);

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetUserIdListByTruckRecordID?recordid={recordid}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<string> GetUserIdListByTruckRecordID(string recordid);

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetTruckInstallersExcludeUserIDs?userID={userID}&name={name}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<InstallerInfo> GetTruckInstallersExcludeUserIDs(string userID,string name);




        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetNotes?workOrderNumber={workOrderNumber}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Notes> GetNotes(string workOrderNumber);

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetRemeasurerName?workOrderNumber={workOrderNumber}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<string> GetRemeasurerName(string workOrderNumber);


        [OperationContract]
        [WebInvoke(Method = "GET",
    UriTemplate = "GetWindowsCustomer?workOrderNumber={workOrderNumber}",
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<WindowsCustomer> GetWindowsCustomer(string workOrderNumber);

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetSubTrades?workOrderNumber={workOrderNumber}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<SubTrades> GetSubTrades(string workOrderNumber);

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetDocumentLibrary?workOrderNumber={workOrderNumber}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<DocumentLibrary> GetDocumentLibrary(string workOrderNumber);


        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetJobReview?workOrderNumber={workOrderNumber}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<JobReview> GetJobReview(string workOrderNumber);

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetWOPicture?workOrderNumber={workOrderNumber}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<WOPicture> GetWOPicture(string workOrderNumber);



        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetWOBigPicture?recordid={recordid}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<WOPicture> GetWOBigPicture(int recordid);

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetTruckInstallationEvent",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<TruckInstallationEvent> GetTruckInstallationEvent();



        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetDocumentFile?recordid={recordid}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<DocumentFile> GetDocumentFile(int recordid);

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetCallLogByID?recordid={recordid}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<CalledLog> GetCallLogByID(int recordid);

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetNotesByID?recordid={recordid}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Notes> GetNotesByID(int recordid);


        [OperationContract]
        [WebInvoke(Method = "GET",
                  UriTemplate = "GetInstallationDateByWOForReturnedJob={workOrderNumber}",
                  ResponseFormat = WebMessageFormat.Json,
                  BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<InstallationEvent> GetInstallationDateByWOForReturnedJob(string workOrderNumber);

        [OperationContract]
        [WebInvoke(Method = "GET",
              UriTemplate = "GetInstallationDateByWOForNonReturnedJob={workOrderNumber}",
              ResponseFormat = WebMessageFormat.Json,
              BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<InstallationEvent> GetInstallationDateByWOForNonReturnedJob(string workOrderNumber);
  
    }
}
