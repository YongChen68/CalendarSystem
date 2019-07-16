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
        [WebInvoke(Method = "POST", UriTemplate = "UpdateReturnedJobSchedule?id={id}&scheduledStartDate={scheduledStartDate}&scheduledEndDate={scheduledEndDate}")]
        bool UpdateReturnedJobSchedule(string id, string scheduledStartDate, string scheduledEndDate);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateInstallationData?id={id}&scheduledStartDate={scheduledStartDate}&scheduledEndDate={scheduledEndDate}&Asbestos={Asbestos}" +
            "&WoodDropOff={WoodDropOff}&HighRisk={HighRisk}&EstInstallerCnt={EstInstallerCnt}&Saturday={Saturday}&Sunday={Sunday}&LeadPaint={LeadPaint}")]
        bool UpdateInstallationData(string id, string scheduledStartDate, string scheduledEndDate,int Asbestos, int WoodDropOff, int HighRisk, int EstInstallerCnt, string Saturday, string Sunday, string LeadPaint);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateRemeasureData?id={id}&remeasureDate={remeasureDate}&fromPopup={fromPopup}&currentState={currentState}")]
        bool UpdateRemeasureData(string id, string remeasureDate,string fromPopup, string currentState);



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
UriTemplate = "GetSubTrades?workOrderNumber={workOrderNumber}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<SubTrades> GetSubTrades(string workOrderNumber);

        [OperationContract]
        [WebInvoke(Method = "GET",
UriTemplate = "GetWOPicture?workOrderNumber={workOrderNumber}",
ResponseFormat = WebMessageFormat.Json,
BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<WOPicture> GetWOPicture(string workOrderNumber);



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
