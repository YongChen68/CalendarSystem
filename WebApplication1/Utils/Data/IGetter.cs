using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Generics.Utils;
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
        List<Generics.Utils.JobReview> GetJobReview();
        
        List<Generics.Utils.WOPicture> GetWOBigPicture(int recordId);

        List<Generics.Utils.DocumentFile> GetDocumentFile(int recordId);

        List<Generics.Utils.Data.InstallationEvent> GetInstallationDateByWOForReturnedJob(string wO);
        List<Generics.Utils.Data.InstallationEvent> GetInstallationDateByWOForNonReturnedJob(string wO);

        List<Generics.Utils.CalledLog> GetCalledLog();

        List<Generics.Utils.InstallerInfo> GetInstallerInfoByWorkOrder();

        Generics.Utils.InstallerInfoWithImage GetInstallerInfoByRecordID(string recordid);

        List<Generics.Utils.InstallerInfo> GetInstallerInfoExceptWorkOrder();

        List<Generics.Utils.CalledLog> GetKeepedCalledLog(string recordid);

        List<Generics.Utils.InstallerWithLessInfo> GetKeepedInstaller(string recordid);

        List<Generics.Utils.InstallerWithLessInfo> GetKeepedTruckInstaller(string recordid,string detailedRecordID);

        string GetActionItemIDByWO();

        string GetActionItemIDByRecordID(string recordid);

        Generics.Utils.InstallerWithLessInfo GetAddedInstaller(string recordid);

       Generics.Utils.InstallerWithLessInfo GetTruckAddedInstaller(string recordid);

        Generics.Utils.InstallerWithLessInfo GetTruckCrewsByUserID(string userID);

        List<Generics.Utils.CalledLog> GetCallLogByID(int recordId);


        List<Generics.Utils.Notes> GetNotes();

        List<string> GetRemeasurerName();
        List<Generics.Utils.Notes> GetKeepedNotes(string recordid);
        List<Generics.Utils.Notes> GetNotesByID(int recordId);
        List<InstallerInfo> GetInstallerInfoByNameExceptWorkOrder(string name);
        List<InstallerInfo> GetResources();

        List<Truck> GetTruckList();

        List<InstallerInfo> GetInstallerListByTruck(string recordid);

        List<string> GetUserIdListByTruckRecordID(string recordid);

        List<InstallerInfo> GetTruckInstallersExcludeUserIDs(string userID,string name);
    }
}