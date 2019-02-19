using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CalendarSystem.Utils.Data
{
    public class BufferedDataGetter1 : IGetter
    {
        public BufferedDataGetter1()
        {
        }

        List<Generics.Utils.CalendarEvent> IGetter.GetData()
        {
            string SQL = @"select p.ActionItemId as id, p.WorkOrderNumber as title, '' as [description], null as startDateTime, null as endDateTime, 'false' as allDay, 
1 as doors, '' as type,[PaintIcon],[WindowIcon],[DoorIcon],[FlagOrder], CurrentStateName
from PlantProduction p with(nolock,noexpand) 
where p.RecordId not in ( select pd.ParentRecordId from PlantProduction_ProductionDate pd with(nolock,noexpand))";
            return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.CalendarEvent>(SQL);
        }
    }
}