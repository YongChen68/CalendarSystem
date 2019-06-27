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
        List<Generics.Utils.Holiday> GetHolidayData();
        List<Generics.Utils.Product> GetProducts();
        List<Generics.Utils.Installer> GetInstallers();
        List<Generics.Utils.CalledLog> GetCalledLog();
        List<Generics.Utils.WOPicture> GetWOPicture();
        List<Generics.Utils.Data.InstallationEvent> GetInstallationDateByWOForReturnedJob(string wO);
        List<Generics.Utils.Data.InstallationEvent> GetInstallationDateByWOForNonReturnedJob(string wO);
    }
}