using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generics.RecordUpdate
{
    public interface IUpdateHelper
    {
        bool UpdateRecord(Utils.ContentType type, Utils.InstallationDataEvent data);

        bool UpdateRecord(Utils.ContentType type, Utils.ImproperTruckInstallationEvent data);

        bool UpdateRecord(Utils.ContentType type, List<Utils.CalledLog> data, string parRecordID);

        bool UpdateRecord(Utils.ContentType type, List<Utils.DocumentFile> data);

        bool UpdateRecord(Utils.ContentType type, List<Utils.Notes> data,string parRecordID);
        bool UpdateRecord(Utils.ContentType type, Utils.ImproperRemeasureEvent data);
        bool UpdateRecord(Utils.ContentType type, Utils.ImproperCalendarEvent data);
        bool UpdateRecord(Utils.ContentType type, Utils.ImproperInstallationEvent data);
        bool UpdateRecord(Utils.ContentType type, Utils.InstallationEventWeekends data);//
        bool UpdateRecordForReturnedJob(Utils.ContentType type, Utils.ImproperInstallationEvent data);

       bool UpdateInstalltionCrew(Generics.Utils.ContentType type, List<Generics.Utils.InstallerWithLessInfo> eventData,string parRecordID);

        bool UpdateTruckInstalltionCrew(Generics.Utils.ContentType type, List<Generics.Utils.InstallerWithLessInfo> eventData, string parRecordID);

    }
}

