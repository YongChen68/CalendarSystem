using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeHelper
{
    public class Runtime
    {
        public bool ProcessUpdate(Generics.Utils.ContentType type, Generics.Utils.InstallationDataEvent eventData)
        {
            Generics.RecordUpdate.IUpdateHelper _comHelper = new FlowfinityConnectionHelper.FlowfinityUpdateHelper(Lift.LiftManager.ConfigProvider.GetValue("OnBehlfOf"), 
                new FlowfinityConnectionHelper.Utils.ActionsCommHelperwithLogging());
            return _comHelper.UpdateRecord(type, eventData);
        }

        public bool ProcessUpdate(Generics.Utils.ContentType type, List<Generics.Utils.CalledLog> eventData)
        {
            Generics.RecordUpdate.IUpdateHelper _comHelper = new FlowfinityConnectionHelper.FlowfinityUpdateHelper(Lift.LiftManager.ConfigProvider.GetValue("OnBehlfOf"),
                new FlowfinityConnectionHelper.Utils.ActionsCommHelperwithLogging());
            return _comHelper.UpdateRecord(type, eventData);
        }



        public bool ProcessUpdate(Generics.Utils.ContentType type, Generics.Utils.ImproperCalendarEvent eventData)
        {
            Generics.RecordUpdate.IUpdateHelper _comHelper = new FlowfinityConnectionHelper.FlowfinityUpdateHelper(Lift.LiftManager.ConfigProvider.GetValue("OnBehlfOf"),
                new FlowfinityConnectionHelper.Utils.ActionsCommHelperwithLogging());
            return _comHelper.UpdateRecord(type, eventData);
        }

   


        public bool ProcessUpdate(Generics.Utils.ContentType type, Generics.Utils.ImproperInstallationEvent eventData)
        {
            Generics.RecordUpdate.IUpdateHelper _comHelper = new FlowfinityConnectionHelper.FlowfinityUpdateHelper(Lift.LiftManager.ConfigProvider.GetValue("OnBehlfOf"),
                new FlowfinityConnectionHelper.Utils.ActionsCommHelperwithLogging());
            return _comHelper.UpdateRecord(type, eventData);
        }

        public bool ProcessUpdate(Generics.Utils.ContentType type, Generics.Utils.ImproperRemeasureEvent eventData)
        {
            Generics.RecordUpdate.IUpdateHelper _comHelper = new FlowfinityConnectionHelper.FlowfinityUpdateHelper(Lift.LiftManager.ConfigProvider.GetValue("OnBehlfOf"),
                new FlowfinityConnectionHelper.Utils.ActionsCommHelperwithLogging());
            return _comHelper.UpdateRecord(type, eventData);
        }

        public bool ProcessUpdateReturnedJob(Generics.Utils.ContentType type, Generics.Utils.ImproperInstallationEvent eventData)
        {
            Generics.RecordUpdate.IUpdateHelper _comHelper = new FlowfinityConnectionHelper.FlowfinityUpdateHelper(Lift.LiftManager.ConfigProvider.GetValue("OnBehlfOf"),
                new FlowfinityConnectionHelper.Utils.ActionsCommHelperwithLogging());
            return _comHelper.UpdateRecordForReturnedJob(type, eventData);
        }

        public bool ProcessUpdate(Generics.Utils.ContentType type, Generics.Utils.InstallationEventWeekends eventData)
        {
            Generics.RecordUpdate.IUpdateHelper _comHelper = new FlowfinityConnectionHelper.FlowfinityUpdateHelper(Lift.LiftManager.ConfigProvider.GetValue("OnBehlfOf"),
                new FlowfinityConnectionHelper.Utils.ActionsCommHelperwithLogging());
            return _comHelper.UpdateRecord(type, eventData);
        }
    }
}
