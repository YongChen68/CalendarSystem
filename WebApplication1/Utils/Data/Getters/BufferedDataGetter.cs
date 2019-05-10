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

            string sPlanedCheck = @"";
            // Create an instance of Norwegian culture
            System.Globalization.CultureInfo Culture =
            System.Globalization.CultureInfo.CreateSpecificCulture("ca");
            // Get the Norwegian calendar from the culture object
            System.Globalization.Calendar cal = Culture.Calendar;

            //string SQL = string.Format(@"select * into #dates from HomeInstallations_InstallationDates d
            //where d.ScheduledDate >= '{0} 12:00' and d.ScheduledDate <= '{1} 11:59'

            //select i.* into #installs from HomeInstallations i 
            //where CurrentStateName in ('Pending Install Completion', 'VP Installation Approval', 'Installation Manager Review', 'ReMeasure Scheduled', 'Work Scheduled', 'Unreviewed Work Scheduled', 'Install in Progress', 'Install Completed', 'Ready for Invoicing', 'Job Completed', 
            //         'Installation Confirmed', 'Installation inprogress rejected', 'Rejected Remeasure', 'Rejected Scheduled Work', 
            //        'Rejected Installation', 'Job Costing', 'Unreviewed Job Costing', 'Rejected Job Costing', 'VP Installation Approval', 'Rejected Manager Review', 
            //'Pending Install Completion') and Branch in ('{2}') and i.Recordid in (select ParentRecordId from #dates group by ParentRecordId)

            //insert into #installs select i.* from HomeInstallations i
            //where CurrentStateName in ('Unreviewed Buffered Work', 'Buffered Work') and PlannedInstallWeek = {3} and RecordId not in (select ParentRecordId from #dates d group by ParentRecordId) and Branch in ('{2')

            //select t.* into #Windows from HomeInstallations_TypeofWork t inner join #installs i on i.RecordId = t.ParentRecordId where t.Type_1 = 'Windows'
            //select t.* into #Doors from HomeInstallations_TypeofWork t inner join #installs i on i.RecordId = t.ParentRecordId where t.Type_1 = 'Doors'
            //select t.* into #Other from HomeInstallations_TypeofWork t inner join #installs i on i.RecordId = t.ParentRecordId where t.Type_1 = 'Other'
            //select s.* into #Subtrade from HomeInstallations_SubtradeReqired s inner join #installs i on i.RecordId = s.ParentRecordId 

            //select WorkOrderNumber, LastName, City, CurrentStateName, case when windows > 0 then WindowState else 'notordered' end as WindowState,
            //                    case when doors > 0 then DoorState else 'notordered' end as DoorState, case when other > 0 then OtherState else 'notordered' end as OtherState,
            //null as Hours, case when ElectricalSubtrade is not null then ElectricalSubtrade else 'Electrical: Unspecified' end  + ',
            //' + 
            //case when SidingSubtrade is not null then SidingSubtrade else 'Siding: Unspecified' end  + ',
            //' + 
            //case when InsulationSubtrade is not null then InsulationSubtrade else 'Insulation: Unspecified' end  + ',
            //' + 
            //case when OtherSubtrade is not null then OtherSubtrade else 'Other: Unspecified' end as Subtrades, Windows, Doors, Other, null as hours, 
            //HomePhoneNumber, CellPhone, WorkPhoneNumber, CrewNames, SeniorInstaller, 
            //case when ElectricalSubtrade is null and SidingSubtrade is null and InsulationSubtrade is null and OtherSubtrade is null then 0 else 1 end as ShowSubtrades,
            //EstInstallerCnt, StreetAddress, ScheduledDate, case when ScheduledDate is null then PlannedInstallWeek else null end as PlannedInstallWeek, PaintedProduct, Branch 
            //from (
            //SELECT  i.Branch_Display as Branch, i.PaintedProduct, i.streetAddress, i.EstInstallerCnt, i.WorkOrderNumber, i.LastName, i.City, i.CurrentStateName,PlannedInstallWeek,
            //                          case when (SELECT     count(ManufacturingStatus)
            //                            FROM          #Windows AS ms
            //                            WHERE      (ParentRecordId = i.RecordId)) > 1 then 'Undetermined' else (SELECT     ManufacturingStatus
            //                            FROM          #Windows AS ms
            //                            WHERE      (ParentRecordId = i.RecordId)) end AS WindowState,
            //                          case when (SELECT    count(ManufacturingStatus)
            //                            FROM          #Doors AS ms
            //                            WHERE      (ParentRecordId = i.RecordId)) > 1 then 'Undetermined' else (SELECT    ManufacturingStatus
            //                            FROM          #Doors AS ms
            //                            WHERE      (ParentRecordId = i.RecordId)) end  AS DoorState,
            //                          case when (SELECT     count(ManufacturingStatus)
            //                            FROM          #Other AS ms
            //                            WHERE      (ParentRecordId = i.RecordId)) > 1 then 'Undetermined' else (SELECT     ManufacturingStatus
            //                            FROM          #Other AS ms
            //                            WHERE      (ParentRecordId = i.RecordId)) end AS OtherState, d.ScheduledDate, 
            //                          (SELECT     SUM(Number_1) AS Number
            //                            FROM          #Windows
            //                            WHERE      (ParentRecordId = i.RecordId)
            //                            GROUP BY Type_1) AS Windows,
            //                          (SELECT     SUM(Number_1) AS Number
            //                            FROM          #Doors AS HomeInstallations_TypeofWork_2
            //                            WHERE      (ParentRecordId = i.RecordId)
            //                            GROUP BY Type_1) AS Doors,
            //                          (SELECT     SUM(Number_1) AS Number
            //                            FROM          #Other AS HomeInstallations_TypeofWork_1
            //                            WHERE      (ParentRecordId = i.RecordId)
            //                            GROUP BY Type_1) AS Other, 
            //case when (select count(SubTrade)from #Subtrade sr
            //where SubTrade = 'Electrical' and sr.ParentRecordId = i.RecordId) > 1 then 'Undetermined' else (select top 1 SubTrade + ': ' + Status as SubTrade from #Subtrade sr
            //where SubTrade = 'Electrical' and sr.ParentRecordId = i.RecordId) end  as ElectricalSubtrade, 
            //case when (select count(SubTrade)from #Subtrade sr
            //where SubTrade = 'Siding' and sr.ParentRecordId = i.RecordId) > 1 then 'Undetermined' else (select top 1 SubTrade + ': ' + Status as SubTrade from #Subtrade sr
            //where SubTrade = 'Siding' and sr.ParentRecordId = i.RecordId) end as SidingSubtrade, 
            //case when (select count(SubTrade)from #Subtrade sr
            //where SubTrade = 'Insulation' and sr.ParentRecordId = i.RecordId) > 1 then 'Undetermined' else (select top 1 SubTrade + ': ' + Status as SubTrade from #Subtrade sr
            //where SubTrade = 'Insulation' and sr.ParentRecordId = i.RecordId) end as InsulationSubtrade, 
            //case when (select count(SubTrade) from #Subtrade sr
            //where SubTrade = 'Other' and sr.ParentRecordId = i.RecordId) > 1 then 'Undetermined' else (select SubTrade + ': ' + Status as SubTrade from #Subtrade sr
            //where SubTrade = 'Other' and sr.ParentRecordId = i.RecordId) end as OtherSubtrade, HomePhoneNumber, CellPhone, WorkPhoneNumber, 
            //dbo.fGetCrewNames(i.RecordId) as CrewNames, (SELECT     e.InstallerName
            //FROM         Employees AS e INNER JOIN
            //                      Users AS u ON e.Account_1 = u.Account INNER JOIN
            //                      HomeInstallations_SeniorInstaller AS si ON u.UserId = si.userId
            //where ParentRecordId = i.RecordId) as SeniorInstaller, i.HVAC
            //FROM         #installs AS i LEFT OUTER JOIN
            //                      #dates AS d ON i.RecordId = d.ParentRecordId
            //) x order by ScheduledDate, Branch

            //drop table #dates
            //drop table #installs
            //drop table #Windows
            //drop table #Doors
            //drop table #Other

            //drop table #Subtrade", this.startDate.ToString("MM/dd/yyyy"), this.endDate.ToString("MM/dd/yyyy"), branchList, sPlanedCheck);

            string SQL = string.Format(@"select d.ScheduledDate,d.ParentRecordId, d.detailrecordid,count(c.detailrecordid) as detailrecordCount
into #dates from HomeInstallations_InstallationDates d
join HomeInstallations_InstallationDates c
on d.ParentRecordId= c.ParentRecordId
group by d.ScheduledDate,d.ParentRecordId, d.detailrecordid

select i.* into #installs from HomeInstallations i 
where CurrentStateName in ('Unreviewed Work Scheduled','ReMeasure Scheduled') and Branch in ({0})  and i.Recordid in (select ParentRecordId from #dates group by ParentRecordId)
insert into #installs select i.* from HomeInstallations i
where CurrentStateName in ('Unreviewed Work Scheduled','ReMeasure Scheduled') and  (((PlannedInstallWeek >= 53) and PlannedInstallWeek <= 53) or 
(PlannedInstallWeek >= 1 and PlannedInstallWeek <= 7)) and RecordId not in (select ParentRecordId from #dates d group by ParentRecordId) and 
Branch in ({0})

select t.* into #Windows from HomeInstallations_TypeofWork t inner join #installs i on i.RecordId = t.ParentRecordId where t.Type_1 = 'Windows'
select t.* into #Doors from HomeInstallations_TypeofWork t inner join #installs i on i.RecordId = t.ParentRecordId where t.Type_1 = 'Doors'
select t.* into #Other from HomeInstallations_TypeofWork t inner join #installs i on i.RecordId = t.ParentRecordId where t.Type_1 = 'Other'
select s.* into #Subtrade from HomeInstallations_SubtradeReqired s inner join #installs i on i.RecordId = s.ParentRecordId 

select WorkOrderNumber, LastName,FirstName,City,PostCode, Email,SalesRep,LeadPaint, ReturnedJob,SalesAmmount,TotalSalesAmount,TotalAsbestos,TotalWoodDropOff,TotalHighRisk,TotalDoors,TotalWindows,DetailRecordId,ParentRecordId,id,detailrecordCount,saturday, sunday, 
jobtype,CurrentStateName,case when windows > 0 then WindowState else 'notordered' end as WindowState,
                    case when doors > 0 then DoorState else 'notordered' end as DoorState, case when other > 0 
then OtherState else 'notordered' end as OtherState,
null as Hours, case when ElectricalSubtrade is not null then ElectricalSubtrade else 'Electrical: Unspecified' end  + ',
' + 
case when SidingSubtrade is not null then SidingSubtrade else 'Siding: Unspecified' end  + ',
' + 
case when InsulationSubtrade is not null then InsulationSubtrade else 'Insulation: Unspecified' end  + ',
' + 
case when OtherSubtrade is not null then OtherSubtrade else 'Other: Unspecified' end as Subtrades, Windows, Doors, Other, null as hours, 
HomePhoneNumber, CellPhone, WorkPhoneNumber, CrewNames, SeniorInstaller, 
case when ElectricalSubtrade is null and SidingSubtrade is null and InsulationSubtrade is null and OtherSubtrade is null 
then 0 else 1 end as ShowSubtrades,
EstInstallerCnt, StreetAddress, ScheduledDate, case when ScheduledDate is null then PlannedInstallWeek else null end as PlannedInstallWeek, PaintedProduct, Branch 
from (
SELECT   i.Branch_Display as Branch, i.PaintedProduct, 0 as ReturnedJob, i.SalesAmmount/detailrecordCount as SalesAmmount,i.SalesAmmount as TotalSalesAmount,DetailRecordId ,ParentRecordId,detailrecordCount,saturday, sunday, jobtype,ActionItemId as id,i.streetAddress, i.EstInstallerCnt, i.WorkOrderNumber, i.LastName, i.FirstName,i.City, 
i.PostalCode as PostCode,i.Email,i.Rep_display as SalesRep,i.LeadPaint ,i.CurrentStateName,PlannedInstallWeek,
                          case when (SELECT     count(ManufacturingStatus)
                            FROM          #Windows AS ms
                            WHERE      (ParentRecordId = i.RecordId)) > 1 then 'Undetermined' else (SELECT     ManufacturingStatus
                            FROM          #Windows AS ms
                            WHERE      (ParentRecordId = i.RecordId)) end AS WindowState,
                          case when (SELECT    count(ManufacturingStatus)
                            FROM          #Doors AS ms
                            WHERE      (ParentRecordId = i.RecordId)) > 1 then 'Undetermined' else (SELECT    ManufacturingStatus
                            FROM          #Doors AS ms
                            WHERE      (ParentRecordId = i.RecordId)) end  AS DoorState,
                          case when (SELECT     count(ManufacturingStatus)
                            FROM          #Other AS ms
                            WHERE      (ParentRecordId = i.RecordId)) > 1 then 'Undetermined' else (SELECT     ManufacturingStatus
                            FROM          #Other AS ms
                            WHERE      (ParentRecordId = i.RecordId)) end AS OtherState, d.ScheduledDate, 
                          (SELECT     round(SUM(Number_1) /detailrecordCount,0) AS Number
                            FROM          #Windows
                            WHERE      (ParentRecordId = i.RecordId)
                            GROUP BY Type_1) AS Windows,
                         (SELECT     round(SUM(Number_1) ,2) AS Number
                            FROM          #Windows
                            WHERE      (ParentRecordId = i.RecordId)
                            GROUP BY Type_1) AS TotalWindows,
                          (SELECT     round(SUM(Number_1)/detailrecordCount,0)  AS Number
                            FROM          #Doors AS HomeInstallations_TypeofWork_2
                            WHERE      (ParentRecordId = i.RecordId)
                            GROUP BY Type_1) AS Doors,
                         (SELECT     round(SUM(Number_1),2)  AS Number
                            FROM          #Doors AS HomeInstallations_TypeofWork_2
                            WHERE      (ParentRecordId = i.RecordId)
                            GROUP BY Type_1) AS TotalDoors,
                          (SELECT      round(SUM(Number_1)/detailrecordCount,0) AS Number
                            FROM          #Other AS HomeInstallations_TypeofWork_1
                            WHERE      (ParentRecordId = i.RecordId)
                            GROUP BY Type_1) AS Other, 
(SELECT count(*) 
          FROM HomeInstallations
         WHERE WoodDropOff='Yes' and i.RecordId = RecordId) TotalWoodDropOff,
(SELECT count(*) 
          FROM HomeInstallations
         WHERE Asbestos='Yes' and i.RecordId = RecordId) TotalAsbestos,
(SELECT count(*) 
          FROM HomeInstallations
         WHERE HighRisk='Yes' and i.RecordId = RecordId) TotalHighRisk,
case when (select count(SubTrade)from #Subtrade sr
where SubTrade = 'Electrical' and sr.ParentRecordId = i.RecordId) > 1 then 'Undetermined' else (select top 1 SubTrade + ': ' + Status as SubTrade from #Subtrade sr
where SubTrade = 'Electrical' and sr.ParentRecordId = i.RecordId) end  as ElectricalSubtrade, 
case when (select count(SubTrade)from #Subtrade sr
where SubTrade = 'Siding' and sr.ParentRecordId = i.RecordId) > 1 then 'Undetermined' else (select top 1 SubTrade + ': ' + Status as SubTrade from #Subtrade sr
where SubTrade = 'Siding' and sr.ParentRecordId = i.RecordId) end as SidingSubtrade, 
case when (select count(SubTrade)from #Subtrade sr
where SubTrade = 'Insulation' and sr.ParentRecordId = i.RecordId) > 1 then 'Undetermined' else (select top 1 SubTrade + ': ' + Status as SubTrade from #Subtrade sr
where SubTrade = 'Insulation' and sr.ParentRecordId = i.RecordId) end as InsulationSubtrade, 
case when (select count(SubTrade) from #Subtrade sr
where SubTrade = 'Other' and sr.ParentRecordId = i.RecordId) > 1 then 'Undetermined' else (select SubTrade + ': ' + Status as SubTrade from #Subtrade sr
where SubTrade = 'Other' and sr.ParentRecordId = i.RecordId) end as OtherSubtrade, HomePhoneNumber, CellPhone, WorkPhoneNumber, 
dbo.fGetCrewNames(i.RecordId) as CrewNames, (SELECT     e.InstallerName
FROM         Employees AS e INNER JOIN
                      Users AS u ON e.Account_1 = u.Account INNER JOIN
                      HomeInstallations_SeniorInstaller AS si ON u.UserId = si.userId
where ParentRecordId = i.RecordId) as SeniorInstaller, i.HVAC
FROM         #installs AS i LEFT OUTER JOIN
                      #dates AS d ON i.RecordId = d.ParentRecordId
where jobtype<>'Multi Family' and ScheduledDate is null
) x order by ScheduledDate, Branch

drop table #dates
drop table #installs
drop table #Windows
drop table #Doors
drop table #Other
drop table #Subtrade", "'" + String.Join("','", branchList) + "'", "'");
            return SQL;

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