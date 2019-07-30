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
        List<Generics.Utils.CalledLog> GetCalledLog();

        List<Generics.Utils.WindowsCustomer> GetWindowsCustomer();
        List<Generics.Utils.WOPicture> GetWOPicture();
        List<Generics.Utils.Data.InstallationEvent> GetInstallationDateByWOForReturnedJob(string wO);
        List<Generics.Utils.Data.InstallationEvent> GetInstallationDateByWOForNonReturnedJob(string wO);
    }
}