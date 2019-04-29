using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
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
        [WebInvoke(Method = "POST",UriTemplate = "UpdateInstallationWeekends?id={id}&SaturdaySunday={SaturdaySunday}")]
       bool UpdateInstallationWeekends(string id, string SaturdaySunday);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateReturnedJobSchedule?id={id}&scheduledStartDate={scheduledStartDate}&scheduledEndDate={scheduledEndDate}")]
        bool UpdateReturnedJobSchedule(string id, string scheduledStartDate, string scheduledEndDate);

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
            UriTemplate = "GetSystemParameters?type={type}&start={start}&end={end}",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        List<Generics.Utils.Data.GlobalValues> GetSystemParameters(string type, DateTime start, DateTime end);
        
    }
}
