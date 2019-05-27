using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Generics.Utils;
using Generics.Utils.Data;
using System.Globalization;


namespace CalendarSystem.Utils.Data
{
    public class BufferedDataGetter : IGetter
    {
        private readonly List<string> branchList;
        private readonly List<string> jobTypeList;
        private readonly List<string> shippingTypeList;
     
        public BufferedDataGetter(List<string> branchList, List<string> jobType, List<string> shippingType)
        {
            this.branchList = branchList;
            this.jobTypeList = jobType;
            this.shippingTypeList = shippingType;
        }

        public BufferedDataGetter(List<string> branchList)
        {
            this.branchList = branchList;
         }

        public List<InstallationEvent> GetInstallationBufferData()
        {
            string SQL = GetInstallationSQL();
            List<InstallationEvent> returnEventList = new List<InstallationEvent>();
           
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            returnEventList = Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.Data.InstallationEvent>(SQL);

            returnEventList =  returnEventList.GroupBy(o => new { o.WorkOrderNumber })
                              .Select(o => o.FirstOrDefault()).ToList();
            return returnEventList;
            //  return returnEventList;
            // return returnEventList.Select(x => x.WorkOrderNumber).Distinct().;
            // return returnEventList.GroupBy(x => x.WorkOrderNumber).Select(y=>y.First());
        }

        public List<InstallationEvent> GetInstallationEventData(ContentType type)
        {
            throw new NotImplementedException();
        }

    
        private string GetInstallationSQL()
        {

            return String.Format(@"
          SELECT  win.Number_1 as windows	 , win.Number_1 as TotalWindows, door.Number_1 as doors	 ,  door.Number_1 as TotalDoors, others.Number_1 as others	, others.Number_1 as other, dates.ScheduledDate,ActionItemId as id,0 as SalesAmmount,
            0  as TotalSalesAmount,i.EstInstallerCnt,i.Rep_display as SalesRep,i.LeadPaint ,0 as TotalWoodDropOff, 0 as TotalAsbestos,0 as TotalHighRisk
	       ,'' as  Subtrades ,i.RecordId	 , [Branch]      ,[Branch_display]      ,[WorkOrderNumber]         ,[JobType]      ,[FirstName]      ,[LastName],0 as ReturnedJob
      ,[StreetAddress],HomePhoneNumber, CellPhone, WorkPhoneNumber, '' as CrewNames, '' as SeniorInstaller, null as hours,'' as WindowState,'' as DoorState, '' as OtherState
      , 'no' as saturday, 'no' as sunday, i.PostalCode as PostCode
      ,[City]
      ,[PostalCode]
      ,[HomePhoneNumber]
      ,[WorkPhoneNumber]
      ,[CellPhone]
      ,[Email]
         ,[Rep_display]
      ,[SalesAmmount]
         ,[Saturday]
         ,[Sunday]
         ,[WoodDropOff]        -- Wood Icon
,[EstInstallerCnt] -- Number Icon
,[HighRisk]              -- High Risk Icon
         ,[LeadPaint]                    -- Skull Icon
,[Asbestos]              -- Skull Icon
,[CurrentStateName]

  FROM[flowserv_flowfinityapps].[dbo].[HomeInstallations]
        i
inner join HomeInstallations_TypeofWork win on i.RecordId = win.ParentRecordId
inner join HomeInstallations_TypeofWork door on i.RecordId = door.ParentRecordId
inner join HomeInstallations_TypeofWork others on i.RecordId = others.ParentRecordId
left join HomeInstallations_InstallationDates dates on i.RecordId = dates.ParentRecordId
Where CurrentStateName in ('Unreviewed Work Scheduled','ReMeasure Scheduled')
  and
  win.Type_1='Windows'
  and
  door.Type_1= 'Doors'
  and
  others.Type_1= 'Other'

  and
  dates.ScheduledDate is null");
    

        }

        public List<InstallationEvent> GetInstallationEventData(ArrayList branch)
        {
            throw new NotImplementedException();
        }

        List<Generics.Utils.CalendarEvent> IGetter.GetData(Generics.Utils.ContentType type)
        {
            string SQL = GetQuery(type);

            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.CalendarEvent>(SQL);


        }

        private string GetQuery(Generics.Utils.ContentType type)
        {
            switch (type)
            {
                case Generics.Utils.ContentType.Window:
                    return String.Format(@"select p.ActionItemId as id, batchNo, p.WorkOrderNumber  +' W:' + cast(case when p.NumberofWindows is null then 0 else p.NumberofWindows end as varchar(max)) + ' D:' + 
cast(case when p.NumberofDoors is null then 0 else p.NumberofDoors end as varchar(max)) + ' PD:' + cast(case when p.NumberofDoors is null then 0 else p.NumberofDoors end as varchar(max)) + ' - ' + 
case when Branch_display is null or Len(Branch_display) = 0 then 'NULL' else  LEft(Branch_display, 3) end + ' ' + JobType 
as title, '' as [description],  null as startDateTime, null as endDateTime, 'true' as allDay, 
case when p.NumberofDoors is null then 0 else p.NumberofDoors  end as doors, case when p.NumberofWindows is null then 0 else p.NumberofWindows end as windows, '' as type,case when p.NumberofDoors is null then 0 else p.NumberofDoors end as NumberofPatioDoors, [SimpleWindows] as Simple, [ComplexWindows] as Complex, [OverSize] as Over_Size, [Arches], [Rakes], [CustomWindows] as Customs,
[PaintIcon],[WindowIcon],[DoorIcon],
case when [FlagOrder] = 'Yes' then 1 else 0 end as FlagOrder,[TotalBoxQty], [TotalGlassQty], [TotalPrice], [TotalLBRMin], [F6CA], [F27DS], [F27TS], [F27TT], [F29CA], [F29CM], [F52PD], [F68CA], [F68SL], [F68VS],[BatchNo], [Transom], [Sidelite], [SingleDoor], [DoubleDoor], [CompleteDate], [HighRiskFlag], [CustomFlag],case when [M2000Icon] = 'Yes' then 1 else 0 end as M2000Icon, Branch_display as Branch, [CardinalOrderedDate], JobType, CurrentStateName
from PlantProduction p with(nolock,noexpand) 
where p.Branch in ({0}) and p.JobType in ({1}) and p.ShippingType in ({2}) and p.RecordId NOT IN ( select pd.ParentRecordId from PlantProduction_ProductionDate pd with(nolock,noexpand)) 
and p.CurrentStateName <> 'Duplicated Work Order' and p.CurrentStateName <> 'Completed Reservations'", String.Format("'{0}'", String.Join("','", branchList)), String.Format("'{0}'", String.Join("','", jobTypeList)), String.Format("'{0}'", String.Join("','", shippingTypeList)));
                case Generics.Utils.ContentType.Customer:
                    return String.Format(@"select p.ActionItemId as id, batchNo, p.WorkOrderNumber  +' W:' + cast(case when p.NumberofWindows is null then 0 else p.NumberofWindows end as varchar(max)) + ' D:' + 
cast(case when p.NumberofDoors is null then 0 else p.NumberofDoors end as varchar(max)) + ' PD:' + cast(case when p.NumberofDoors is null then 0 else p.NumberofDoors end as varchar(max)) + ' - ' + 
case when Branch_display is null or Len(Branch_display) = 0 then 'NULL' else  LEft(Branch_display, 3) end + ' ' + JobType 
as title, '' as [description],  null as startDateTime, null as endDateTime, 'true' as allDay, 
case when p.NumberofDoors is null then 0 else p.NumberofDoors  end as doors, case when p.NumberofWindows is null then 0 else p.NumberofWindows end as windows, '' as type,
case when p.NumberofDoors is null then 0 else p.NumberofDoors end as NumberofPatioDoors, [PaintIcon],[WindowIcon],[DoorIcon],
case when [FlagOrder] = 'Yes' then 1 else 0 end as FlagOrder,[TotalBoxQty], [TotalGlassQty], [TotalPrice], [TotalLBRMin], [F6CA], [F27DS], [F27TS], [F27TT], [F29CA], [F29CM], [F52PD], [F68CA], [F68SL], [F68VS], [SimpleWindows] as Simple, [ComplexWindows] as Complex, [OverSize] as Over_Size, [Arches], [Rakes], [CustomWindows] as Customs,
[BatchNo], [Transom], [Sidelite], [SingleDoor], [DoubleDoor], [CompleteDate], [HighRiskFlag], [CustomFlag],case when [M2000Icon] = 'Yes' then 1 else 0 end as M2000Icon, Branch_display as Branch, [CardinalOrderedDate], JobType, CurrentStateName
from PlantProduction p with(nolock,noexpand) 
where p.Branch in ({0}) and p.JobType in ({1}) and p.ShippingType in ({2}) and p.DeliveryDate is null and p.CurrentStateName <> 'Duplicated Work Order' 
and p.CurrentStateName <> 'Completed Reservations'", String.Format("'{0}'", String.Join("','", branchList)), String.Format("'{0}'", String.Join("','", jobTypeList)), String.Format("'{0}'", String.Join("','", shippingTypeList)));
                case Generics.Utils.ContentType.Shipping:
                    return String.Format(@"select p.ActionItemId as id, batchNo, p.WorkOrderNumber  +' W:' + cast(case when p.NumberofWindows is null then 0 else p.NumberofWindows end as varchar(max)) + ' D:' + 
cast(case when p.NumberofDoors is null then 0 else p.NumberofDoors end as varchar(max)) + ' PD:' + cast(case when p.NumberofDoors is null then 0 else p.NumberofDoors end as varchar(max)) + ' - ' + 
case when Branch_display is null or Len(Branch_display) = 0 then 'NULL' else  LEft(Branch_display, 3) end + ' ' + JobType 
as title, '' as [description],  null as startDateTime, null as endDateTime, 'true' as allDay, 
case when p.NumberofDoors is null then 0 else p.NumberofDoors  end as doors, case when p.NumberofWindows is null then 0 else p.NumberofWindows end as windows, '' as type,
case when p.NumberofDoors is null then 0 else p.NumberofDoors end as NumberofPatioDoors,
[PaintIcon],[WindowIcon],[DoorIcon],
case when [FlagOrder] = 'Yes' then 1 else 0 end as FlagOrder,[TotalBoxQty], [TotalGlassQty], [TotalPrice], [TotalLBRMin], [F6CA], [F27DS], [F27TS], [F27TT], [F29CA], [F29CM], [F52PD], [F68CA], [F68SL], [F68VS],[BatchNo], [Transom], [Sidelite], [SingleDoor], [DoubleDoor], [CompleteDate], [HighRiskFlag], [CustomFlag],case when [M2000Icon] = 'Yes' then 1 else 0 end as M2000Icon, Branch_display as Branch, [CardinalOrderedDate], JobType, CurrentStateName, [SimpleWindows] as Simple, [ComplexWindows] as Complex, [OverSize] as Over_Size, [Arches], [Rakes], [CustomWindows] as Customs
from PlantProduction p with(nolock,noexpand) 
where p.Branch in ({0}) and p.JobType in ({1}) and p.ShippingType in ({2}) and p.RecordId NOT IN ( select pd.ParentRecordId from PlantProduction_ShippingDate pd with(nolock,noexpand)) and p.CurrentStateName <> 'Duplicated Work Order' 
and JobType <> 'RES' AND JobType <> 'PendingRES' AND p.CurrentStateName <> 'Completed Reservations' and p.CurrentStateName <> 'Shipped' and p.CurrentStateName <> 'Ready To Ship' and p.JobType <> 'RES'", String.Format("'{0}'", String.Join("','", branchList)), String.Format("'{0}'", String.Join("','", jobTypeList)), String.Format("'{0}'", String.Join("','", shippingTypeList)));
                case Generics.Utils.ContentType.Door:
                    return String.Format(@"select p.ActionItemId as id, p.WorkOrderNumber  +' W:' + cast(case when p.NumberofWindows is null then 0 else p.NumberofWindows end as varchar(max)) + ' D:' + 
cast(case when p.NumberofDoors is null then 0 else p.NumberofDoors end as varchar(max)) + ' PD:' + cast(case when p.NumberofDoors is null then 0 else p.NumberofDoors end as varchar(max)) + ' - ' + 
case when Branch_display is null or Len(Branch_display) = 0 then 'NULL' else  LEft(Branch_display, 3) end + ' ' + JobType 
as title, '' as [description],  null as startDateTime, null as endDateTime, 'true' as allDay, 
case when p.NumberofDoors is null then 0 else p.NumberofDoors  end as doors, case when p.NumberofWindows is null then 0 else p.NumberofWindows end as windows, '' as type,
case when p.NumberofDoors is null then 0 else p.NumberofDoors end as NumberofPatioDoors,
[PaintIcon],[WindowIcon],[DoorIcon],
case when [FlagOrder] = 'Yes' then 1 else 0 end as FlagOrder,[TotalBoxQty], [TotalGlassQty], [TotalPrice], [TotalLBRMin], [F6CA], [F27DS], [F27TS], [F27TT], [F29CA], [F29CM], [F52PD], [F68CA], [F68SL], [F68VS],[BatchNo], [Transom], [Sidelite], [SingleDoor], [DoubleDoor], [CompleteDate], [HighRiskFlag], [CustomFlag],case when [M2000Icon] = 'Yes' then 1 else 0 end as M2000Icon, Branch_display as Branch, [CardinalOrderedDate], JobType, CurrentStateName, [SimpleWindows] as Simple, [ComplexWindows] as Complex, [OverSize] as Over_Size, [Arches], [Rakes], [CustomWindows] as Customs
from PlantProduction p with(nolock,noexpand) 
where p.Branch in ({0}) and p.JobType in ({1}) and p.ShippingType in ({2}) and p.RecordId NOT IN ( select pd.ParentRecordId from PlantProduction_ProductionDate1 pd with(nolock,noexpand)) 
and p.NumberofDoors > 0 and p.CurrentStateName <> 'Duplicated Work Order' and p.CurrentStateName <> 'Completed Reservations'", String.Format("'{0}'", String.Join("','", branchList)), String.Format("'{0}'", String.Join("','", jobTypeList)), String.Format("'{0}'", String.Join("','", shippingTypeList)));
                case Generics.Utils.ContentType.Paint:
                    return String.Format(@"select p.ActionItemId as id, p.WorkOrderNumber  +' W:' + cast(case when p.NumberofWindows is null then 0 else p.NumberofWindows end as varchar(max)) + ' D:' + 
cast(case when p.NumberofDoors is null then 0 else p.NumberofDoors end as varchar(max)) + ' PD:' + cast(case when p.NumberofDoors is null then 0 else p.NumberofDoors end as varchar(max)) + ' - ' + 
case when Branch_display is null or Len(Branch_display) = 0 then 'NULL' else  LEft(Branch_display, 3) end + ' ' + JobType 
as title, '' as [description],  null as startDateTime, null as endDateTime, 'true' as allDay, 
case when p.NumberofDoors is null then 0 else p.NumberofDoors  end as doors, case when p.NumberofWindows is null then 0 else p.NumberofWindows end as windows, '' as type,case when p.NumberofDoors is null then 0 else p.NumberofDoors end as NumberofPatioDoors,
[PaintIcon],[WindowIcon],[DoorIcon],
case when [FlagOrder] = 'Yes' then 1 else 0 end as FlagOrder,[TotalBoxQty], [TotalGlassQty], [TotalPrice], [TotalLBRMin], [F6CA], [F27DS], [F27TS], [F27TT], [F29CA], [F29CM], [F52PD], [F68CA], [F68SL], [F68VS], [Transom], [Sidelite], [SingleDoor], [DoubleDoor],[BatchNo], [CompleteDate], [HighRiskFlag], [CustomFlag],case when [M2000Icon] = 'Yes' then 1 else 0 end as M2000Icon, Branch_display as Branch, [CardinalOrderedDate], JobType, CurrentStateName, [SimpleWindows] as Simple, [ComplexWindows] as Complex, [OverSize] as Over_Size, [Arches], [Rakes], [CustomWindows] as Customs
from PlantProduction p with(nolock,noexpand) 
where p.Branch in ({0}) and p.JobType in ({1}) and p.ShippingType in ({2}) and p.RecordId NOT IN ( select pd.ParentRecordId from PlantProduction_PaintDate pd with(nolock,noexpand))  and p.PaintIcon = 'Yes' 
and p.CurrentStateName <> 'Duplicated Work Order' and p.CurrentStateName <> 'Completed Reservations'", String.Format("'{0}'", String.Join("','", branchList)), String.Format("'{0}'", String.Join("','", jobTypeList)), String.Format("'{0}'", String.Join("','", shippingTypeList)));
                default: throw new NotSupportedException(type.ToString());
            }
        }

        public List<CalendarEvent> GetData(ContentType type)
        {
            throw new NotImplementedException();
        }

        List<InstallationEvent> IGetter.GetData()
        {
            throw new NotImplementedException();
        }

        List<Holiday> IGetter.GetHolidayData()
        {
            throw new NotImplementedException();
        }

        public List<Product> GetProducts()
        {
            throw new NotImplementedException();
        }
    }
}