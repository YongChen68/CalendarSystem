using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;

namespace CalendarSystem.Utils.Data
{
    public class EventDataGetter1 : IGetter
    {
        private readonly DateTime startDate;
        private readonly DateTime endDate;
        private EventDataGetter1() { }
        public EventDataGetter1(string start, string end)
        {
            this.startDate = DateTime.ParseExact(start.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            this.endDate = DateTime.ParseExact(end.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        List<Generics.Utils.CalendarEvent> IGetter.GetData()
        {
            string SQL = @"select x.id, x.title, x.description, x.type, x.startDateTime, case when datediff(hour, x.startDateTime, x.endDateTime) = 0 then DATEADD(HOUR, 1, x.startDateTime) else x.endDateTime end as endDateTime, x.doors, 
case when datediff(day, startDateTime, x.endDateTime) > 0 then 'true' else 'false' end as allDay,[PaintIcon],[WindowIcon],[DoorIcon],[FlagOrder], CurrentStateName
from (select p.ActionItemId as id, p.WorkOrderNumber as title, '' as [description], (select min(dateadd(MINUTE, (datepart(hour, pd.StartTime)*60+DATEPART(minute, pd.starttime)), pd.StartDate)) as date from PlantProduction_ProductionDate pd with(nolock,noexpand) where p.RecordId = pd.ParentRecordId) as startDateTime, 
(select max(dateadd(MINUTE, (datepart(hour, pd.EndTime)*60+DATEPART(minute, pd.EndTime)), pd.StartDate)) as date from PlantProduction_ProductionDate pd with(nolock,noexpand) where p.RecordId = pd.ParentRecordId) as endDateTime, 1 as doors, '' as type, [PaintIcon],[WindowIcon],[DoorIcon],[FlagOrder], p.CurrentStateName
 from PlantProduction p with(nolock,noexpand) 
where p.RecordId in (select pd.ParentRecordId from PlantProduction_ProductionDate pd with(nolock,noexpand) where p.RecordId = pd.ParentRecordId and pd.StartDate >= @pStart and pd.StartDate < = @pEnd
)
) x";
            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            System.Data.SqlClient.SqlParameter startPar = new System.Data.SqlClient.SqlParameter("pStart", startDate);
            pars.Add(startPar);
            System.Data.SqlClient.SqlParameter endPar = new System.Data.SqlClient.SqlParameter("pEnd", endDate);
            pars.Add(endPar);
            return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.CalendarEvent>(SQL, pars.ToArray());
        }
    }
}