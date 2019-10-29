using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Generics.Utils.Data;

namespace CalendarSystem.Utils.Data
{
    /// <summary>
    /// Summary description for IGetter
    /// </summary>
    public interface IGetter
    {
        List<Generics.Utils.CalendarEvent> GetData(Generics.Utils.ContentType type);
        List<Generics.Utils.Data.InstallationEvent> GetData();
        List<Generics.Utils.Data.InstallationEvent> GetInstallationBufferData();
        
        List<Generics.Utils.Data.RemeasureEvent> GetRemeasureData();
        List<Generics.Utils.Data.RemeasureEvent> GetRemeasureBufferData();
        List<Generics.Utils.Holiday> GetHolidayData();
        List<Generics.Utils.Product> GetProducts();

        List<Generics.Utils.UnavailableHR> GetUnavailableResources();

        List<Generics.Utils.Product> GetProductsDoors();
        List<Generics.Utils.Product> GetManufacturingWindows();
        List<Generics.Utils.Product> GetManufacturingDoors();

        List<Generics.Utils.SubTrades> GetSubTrades();

        List<Generics.Utils.Installer> GetInstallers();
       

        List<Generics.Utils.WindowsCustomer> GetWindowsCustomer();
        List<Generics.Utils.WOPicture> GetWOPicture();

        List<Generics.Utils.DocumentLibrary> GetDocumentLibrary();

        List<Generics.Utils.WOPicture> GetWOBigPicture(int recordId);

        List<Generics.Utils.DocumentFile> GetDocumentFile(int recordId);

        List<Generics.Utils.Data.InstallationEvent> GetInstallationDateByWOForReturnedJob(string wO);
        List<Generics.Utils.Data.InstallationEvent> GetInstallationDateByWOForNonReturnedJob(string wO);

        List<Generics.Utils.CalledLog> GetCalledLog();
        List<Generics.Utils.CalledLog> GetKeepedCalledLog(string recordid);
        List<Generics.Utils.CalledLog> GetCallLogByID(int recordId);


        List<Generics.Utils.Notes> GetNotes();
        List<Generics.Utils.Notes> GetKeepedNotes(string recordid);
        List<Generics.Utils.Notes> GetNotesByID(int recordId);

    }
}