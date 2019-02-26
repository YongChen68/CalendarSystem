using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Generics.Utils;
using Generics.Utils.Data;
using System.Linq;

namespace CalendarSystem.Utils.Data
{
    public class EventDataGetter : IGetter
    {
        private readonly DateTime startDate;
        private readonly DateTime endDate;
        private readonly List<string> stateList;
        private readonly List<string> branchList;
        private readonly List<string> jobTypeList;
        private readonly List<string> shippingTypeList;

        private EventDataGetter() { }

        public EventDataGetter(string start, string end, List<string> stateList, List<string> branchList, List<string> jobType, List<string> shippingType)
        {
            this.startDate = DateTime.ParseExact(start.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            this.endDate = DateTime.ParseExact(end.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            this.stateList = stateList;
            this.branchList = branchList;
            this.jobTypeList = jobType;
            this.shippingTypeList = shippingType;
        }

        public EventDataGetter(string start, string end, List<string> branchList, List<string> stateList)
        {
            this.startDate = DateTime.ParseExact(start.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            this.endDate = DateTime.ParseExact(end.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            this.branchList = branchList;
            this.stateList = stateList;
        }

        public EventDataGetter(string start, string end)
        {
            this.startDate = DateTime.ParseExact(start.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            this.endDate = DateTime.ParseExact(end.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }


        private string GetInstallationSQL()
        {

            string sPlanedCheck = @"";
            // Create an instance of Norwegian culture
            System.Globalization.CultureInfo Culture =
            System.Globalization.CultureInfo.CreateSpecificCulture("ca");
            // Get the Norwegian calendar from the culture object
            System.Globalization.Calendar cal = Culture.Calendar;

          

         
            if (this.endDate.Year - this.startDate.Year > 0)
            {
                sPlanedCheck = string.Format(@" (((PlannedInstallWeek >= {0}) and PlannedInstallWeek <= {1}) or 
(PlannedInstallWeek >= {2} and PlannedInstallWeek <= {3}))",
                                                    cal.GetWeekOfYear(this.startDate, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday),
                                                    cal.GetWeekOfYear(DateTime.Parse("12/31/" + this.startDate.Year.ToString() + " 11:59:59 pm"), System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday),
                                                    cal.GetWeekOfYear(DateTime.Parse("1/1/" + this.endDate.Year.ToString()), System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday),
                                                    cal.GetWeekOfYear(this.endDate, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday)
                                                   );
            }
            else
            {
                // render simple Planed installation week chack
                sPlanedCheck = string.Format(@"PlannedInstallWeek >= {0} and PlannedInstallWeek <= {1}",
                    cal.GetWeekOfYear(this.startDate, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday),
                    cal.GetWeekOfYear(this.endDate, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday));

                //int week = ;
            }
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

           string SQL = string.Format(@"select * into #dates from HomeInstallations_InstallationDates d
where d.ScheduledDate >= '{0} 12:00' and d.ScheduledDate <= '{1} 11:59'

select i.* into #installs from HomeInstallations i 
where CurrentStateName in ({3}) and Branch in ({2})  and i.Recordid in (select ParentRecordId from #dates group by ParentRecordId)

insert into #installs select i.* from HomeInstallations i
where CurrentStateName in ('Unreviewed Buffered Work', 'Buffered Work') and  (((PlannedInstallWeek >= 53) and PlannedInstallWeek <= 53) or 
(PlannedInstallWeek >= 1 and PlannedInstallWeek <= 7)) and RecordId not in (select ParentRecordId from #dates d group by ParentRecordId) and 
Branch in ({2})

select t.* into #Windows from HomeInstallations_TypeofWork t inner join #installs i on i.RecordId = t.ParentRecordId where t.Type_1 = 'Windows'
select t.* into #Doors from HomeInstallations_TypeofWork t inner join #installs i on i.RecordId = t.ParentRecordId where t.Type_1 = 'Doors'
select t.* into #Other from HomeInstallations_TypeofWork t inner join #installs i on i.RecordId = t.ParentRecordId where t.Type_1 = 'Other'
select s.* into #Subtrade from HomeInstallations_SubtradeReqired s inner join #installs i on i.RecordId = s.ParentRecordId 

select WorkOrderNumber, LastName, City, SalesAmmount,DetailRecordId,ParentRecordId,id, CurrentStateName,case when windows > 0 then WindowState else 'notordered' end as WindowState,
                    case when doors > 0 then DoorState else 'notordered' end as DoorState, case when other > 0 then OtherState else 'notordered' end as OtherState,
null as Hours, case when ElectricalSubtrade is not null then ElectricalSubtrade else 'Electrical: Unspecified' end  + ',
' + 
case when SidingSubtrade is not null then SidingSubtrade else 'Siding: Unspecified' end  + ',
' + 
case when InsulationSubtrade is not null then InsulationSubtrade else 'Insulation: Unspecified' end  + ',
' + 
case when OtherSubtrade is not null then OtherSubtrade else 'Other: Unspecified' end as Subtrades, Windows, Doors, Other, null as hours, 
HomePhoneNumber, CellPhone, WorkPhoneNumber, CrewNames, SeniorInstaller, 
case when ElectricalSubtrade is null and SidingSubtrade is null and InsulationSubtrade is null and OtherSubtrade is null then 0 else 1 end as ShowSubtrades,
EstInstallerCnt, StreetAddress, ScheduledDate, case when ScheduledDate is null then PlannedInstallWeek else null end as PlannedInstallWeek, PaintedProduct, Branch 
from (
SELECT   i.Branch_Display as Branch, i.PaintedProduct, i.SalesAmmount,DetailRecordId ,ParentRecordId, ActionItemId as id,i.streetAddress, i.EstInstallerCnt, i.WorkOrderNumber, i.LastName, i.City, i.CurrentStateName,PlannedInstallWeek,
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
                          (SELECT     SUM(Number_1) AS Number
                            FROM          #Windows
                            WHERE      (ParentRecordId = i.RecordId)
                            GROUP BY Type_1) AS Windows,
                          (SELECT     SUM(Number_1) AS Number
                            FROM          #Doors AS HomeInstallations_TypeofWork_2
                            WHERE      (ParentRecordId = i.RecordId)
                            GROUP BY Type_1) AS Doors,
                          (SELECT     SUM(Number_1) AS Number
                            FROM          #Other AS HomeInstallations_TypeofWork_1
                            WHERE      (ParentRecordId = i.RecordId)
                            GROUP BY Type_1) AS Other, 
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
) x order by ScheduledDate, Branch

drop table #dates
drop table #installs
drop table #Windows
drop table #Doors
drop table #Other
drop table #Subtrade", this.startDate.ToShortDateString(),this.endDate.ToShortDateString(), "'" + String.Join("','", branchList) + "'", "'" + String.Join("','", stateList) + "'");
            return SQL;
          
        }

        List<Generics.Utils.Data.InstallationEvent> IGetter.GetData()
        {
            string SQL = GetInstallationSQL();
            List<InstallationEvent> installationEventList = new List<InstallationEvent>();
            //  List<InstallationEvent> returnEventList = new List<InstallationEvent>();
            List<InstallationEvent> returnEventList = new List<InstallationEvent>();
            InstallationEvent newEvent ;
            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            pars.Add(new System.Data.SqlClient.SqlParameter("pStart", startDate));
            pars.Add(new System.Data.SqlClient.SqlParameter("pEnd", endDate));
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            installationEventList = Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.Data.InstallationEvent>(SQL, pars.ToArray());
            List<string> woList = new List<string>();
            foreach (InstallationEvent eventx in installationEventList)
            {
                if (woList.Contains(eventx.WorkOrderNumber))
                 {
                    continue;
                }
                newEvent = new InstallationEvent();
                newEvent.Branch = eventx.Branch;
                newEvent.CellPhone= eventx.CellPhone;
                newEvent.City = eventx.City;
                newEvent.CrewNames = eventx.CrewNames;
                newEvent.CurrentStateName = eventx.CurrentStateName;
                newEvent.Doors = eventx.Doors;
                newEvent.DoorState = eventx.DoorState;

               
                newEvent.start = installationEventList.Where(a => a.WorkOrderNumber == eventx.WorkOrderNumber).Min(b => b.ScheduledDate).
                   ToString();

                newEvent.end = installationEventList.Where(a => a.WorkOrderNumber == eventx.WorkOrderNumber).Max(b => b.ScheduledDate).
                   ToString();

                newEvent.EstInstallerCnt = eventx.EstInstallerCnt;
                newEvent.HomePhoneNumber = eventx.HomePhoneNumber;
                newEvent.Hours= eventx.Hours;
                newEvent.id = eventx.id;
                newEvent.LastName = eventx.LastName;
                newEvent.Other = eventx.Other;
                newEvent.OtherState = eventx.OtherState;

                newEvent.SalesAmmount= eventx.SalesAmmount;
                newEvent.ScheduledDate = eventx.ScheduledDate;
                newEvent.SeniorInstaller= eventx.SeniorInstaller;
                newEvent.StreetAddress= eventx.StreetAddress;
                newEvent.Subtrades = eventx.Subtrades;

                newEvent.title1 = eventx.title1;
                newEvent.Windows = eventx.Windows;
                newEvent.WindowState = eventx.WindowState;
                newEvent.WorkOrderNumber = eventx.WorkOrderNumber;
                newEvent.WorkPhoneNumber= eventx.WorkPhoneNumber;

                woList.Add(eventx.WorkOrderNumber);
                returnEventList.Add(newEvent);
                
            }
            return returnEventList;
        }

        List<Generics.Utils.CalendarEvent> IGetter.GetData(Generics.Utils.ContentType type)
        {
            string SQL = GetSQL(type);

            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            pars.Add(new System.Data.SqlClient.SqlParameter("pStart", startDate));
            pars.Add(new System.Data.SqlClient.SqlParameter("pEnd", endDate));
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.CalendarEvent>(SQL, pars.ToArray());
        }

        List<Generics.Utils.Holiday> IGetter.GetHolidayData()
        {
            string SQL = GetHolidaySQL();

            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            pars.Add(new System.Data.SqlClient.SqlParameter("pStart", startDate));
            pars.Add(new System.Data.SqlClient.SqlParameter("pEnd", endDate));
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.Holiday>(SQL, pars.ToArray());
        }


        private string GetHolidaySQL()
        {
            string SQL = string.Format(@"select HolidayName, HolidayDate from Holidays
where HolidayDate >= '{0}' and HolidayDate <= '{1}'", this.startDate,this.endDate);
            return SQL;
        }


            private string GetSQL(Generics.Utils.ContentType type)
        {
            switch (type)
            {
                case Generics.Utils.ContentType.Shipping:
                    return String.Format(@"select x.id, x.title +' W:' + cast(x.windows as varchar(max)) + ' D:' + cast(x.doors as varchar(max)) + ' PD:' + 
cast(x.NumberOfPatioDoors as varchar(max)) + ' - ' + case when x.Branch is null or Len(x.Branch) = 0 then 'NULL' else  LEft(x.Branch, 3) end + ' ' + x.JobType + case when @pStart = @pEnd then 'Day View stuff' else '' end as title, 
x.description, x.type, x.startDateTime, case when datediff(hour, x.startDateTime, x.endDateTime) = 0 then DATEADD(HOUR, 1, x.startDateTime) else x.endDateTime end as endDateTime, 
x.doors, x.windows, 'true' as allDay,[PaintIcon],[WindowIcon],[DoorIcon], [M2000Icon], [FlagOrder],[TotalBoxQty], [TotalGlassQty], [TotalPrice], [TotalLBRMin], [F6CA], [F27DS], [F27TS], [F27TT], [F29CA], [F29CM], [F52PD], [F68CA], [F68SL], 
[F68VS], [Transom], [Sidelite], [SingleDoor], [DoubleDoor],  [BatchNo], Branch, JobType, [CardinalOrderedDate], [CompleteDate], [HighRiskFlag], [CustomFlag], CurrentStateName,NumberOfPatioDoors, Simple, Complex, Over_Size, [Arches], [Rakes], Customs
from (select p.ActionItemId as id, p.WorkOrderNumber as title, '' as [description], 
(select min(dateadd(MINUTE, (datepart(hour, pd.ShippingStartTime)*60+DATEPART(minute, pd.ShippingStartTime)), pd.ShippingStartDate)) as date from PlantProduction_ShippingDate pd with(nolock,noexpand) 
where p.RecordId = pd.ParentRecordId) as startDateTime, 
(select max(dateadd(MINUTE, (datepart(hour, pd.ShippingEndTime)*60+DATEPART(minute, pd.ShippingEndTime)), pd.ShippingStartDate)) as date
from PlantProduction_ShippingDate pd with(nolock,noexpand) where p.RecordId = pd.ParentRecordId) as endDateTime, case when p.NumberofDoors is null then 0 else p.NumberofDoors end as doors, case when p.NumberOfPatioDoors is null then 0 else p.NumberOfPatioDoors end as NumberOfPatioDoors,
case when p.NumberofWindows is null then 0 else p.NumberofWindows end as windows, '' as type, [PaintIcon],[WindowIcon],[DoorIcon],case when [M2000Icon] = 'Yes' then 1 else 0 end as M2000Icon,case when [FlagOrder] = 'Yes' then 1 else 0 end as FlagOrder, 
[TotalBoxQty], [TotalGlassQty], [TotalPrice], [TotalLBRMin], [F6CA], [F27DS], [F27TS], [F27TT], [F29CA], [F29CM], [F52PD], [F68CA], [F68SL], [F68VS], [Transom], [Sidelite], [SingleDoor], [DoubleDoor], [BatchNo], Branch_display as Branch, JobType, 
[CardinalOrderedDate], [CompleteDate], [HighRiskFlag], [CustomFlag], CurrentStateName, [SimpleWindows] as Simple, [ComplexWindows] as Complex, [OverSize] as Over_Size, [Arches], [Rakes], [CustomWindows] as Customs
 from PlantProduction p with(nolock,noexpand) 
where p.currentstatename in ({0}) and p.Branch in ({1}) and p.JobType in ({2}) and p.ShippingType in ({3}) and p.RecordId in (select pd.ParentRecordId from PlantProduction_ShippingDate pd with(nolock,noexpand) 
where p.RecordId = pd.ParentRecordId and pd.ShippingStartDate >= @pStart and pd.ShippingStartDate <= @pEnd and p.CurrentStateName <> 'Duplicated Work Order' and p.CurrentStateName <> 'Completed Reservations'
)) x", String.Format("'{0}'", String.Join("','", stateList)), String.Format("'{0}'", String.Join("','", branchList)), String.Format("'{0}'", String.Join("','", jobTypeList)), String.Format("'{0}'", String.Join("','", shippingTypeList)));
                case Generics.Utils.ContentType.Customer:
                    return String.Format(@"select x.id, x.title +' W:' + cast(x.windows as varchar(max)) + ' D:' + cast(x.doors as varchar(max)) + ' PD:' + cast(x.NumberOfPatioDoors as varchar(max)) + ' - ' + case when x.Branch is null or Len(x.Branch) = 0 then 'NULL' else Left(x.Branch, 3) end + ' ' + x.JobType + case when @pStart = @pEnd then 'Day View stuff' else '' end as title, x.description, x.type, 
x.startDateTime, 
case when datediff(hour, x.startDateTime, x.endDateTime) = 0 then DATEADD(HOUR, 1, x.startDateTime) else x.endDateTime end as endDateTime, 
x.doors, x.windows, 'true' as allDay,[PaintIcon],[WindowIcon],[DoorIcon], [M2000Icon], [FlagOrder],[TotalBoxQty], [TotalGlassQty], [TotalPrice], [TotalLBRMin], [F6CA], [F27DS], [F27TS], [F27TT], [F29CA], [F29CM], [F52PD], [F68CA], [F68SL], [F68VS], [Transom], [Sidelite], [SingleDoor], [DoubleDoor],  [BatchNo], Branch, JobType, [CardinalOrderedDate], [CompleteDate], [HighRiskFlag], [CustomFlag], CurrentStateName,NumberOfPatioDoors, Simple, Complex, Over_Size, [Arches], [Rakes], Customs
from (select p.ActionItemId as id, p.WorkOrderNumber as title, '' as [description], 
dateadd(MINUTE, (datepart(hour, p.DeliveryDate)*60+DATEPART(minute, p.DeliveryDate)), p.DeliveryDate) as startDateTime, 
dateadd(MINUTE, (datepart(hour, p.DeliveryDate)*60+DATEPART(minute, p.DeliveryDate)), p.DeliveryDate) as endDateTime, 
case when p.NumberofDoors is null then 0 else p.NumberofDoors end as doors, case when p.NumberOfPatioDoors is null then 0 else p.NumberOfPatioDoors end as NumberOfPatioDoors,
case when p.NumberofWindows is null then 0 else p.NumberofWindows end as windows, '' as type, [PaintIcon],[WindowIcon],[DoorIcon],case when [M2000Icon] = 'Yes' then 1 else 0 end as M2000Icon,case when [FlagOrder] = 'Yes' then 1 else 0 end as FlagOrder, 
[TotalBoxQty], [TotalGlassQty], [TotalPrice], [TotalLBRMin], [F6CA], [F27DS], [F27TS], [F27TT], [F29CA], [F29CM], [F52PD], [F68CA], [F68SL], [F68VS], [Transom], [Sidelite], [SingleDoor], [DoubleDoor], [BatchNo], 
Branch_display as Branch, JobType, [CardinalOrderedDate], [CompleteDate], [HighRiskFlag], [CustomFlag], CurrentStateName, [SimpleWindows] as Simple, [ComplexWindows] as Complex, [OverSize] as Over_Size, [Arches], [Rakes], [CustomWindows] as Customs
 from PlantProduction p with(nolock,noexpand) 
where p.currentstatename in ({0}) and p.Branch in ({1}) and p.JobType in ({2}) and p.ShippingType in ({3}) and p.DeliveryDate >= @pStart and p.DeliveryDate <= @pEnd and p.CurrentStateName <> 'Duplicated Work Order' and p.CurrentStateName <> 'Completed Reservations') x",
String.Format("'{0}'", String.Join("','", stateList)), String.Format("'{0}'", String.Join("','", branchList)), String.Format("'{0}'", String.Join("','", jobTypeList)), String.Format("'{0}'", String.Join("','", shippingTypeList)));
                case Generics.Utils.ContentType.Window:
                    return String.Format(@"select x.id, x.title +' W:' + cast(x.windows as varchar(max)) + ' D:' + cast(x.doors as varchar(max)) + ' PD:' + cast(x.NumberOfPatioDoors as varchar(max)) + ' - ' + case when x.Branch is null or Len(x.Branch) = 0 then 'NULL' else  LEft(x.Branch, 3) end + ' ' + x.JobType + case when @pStart = @pEnd then 'Day View stuff' else '' end as title, x.description, x.type, x.startDateTime, case when datediff(hour, x.startDateTime, x.endDateTime) = 0 then DATEADD(HOUR, 1, x.startDateTime) else x.endDateTime end as endDateTime, 
x.doors, x.windows, /*case when datediff(day, startDateTime, x.endDateTime) > 0 or datepart(hour, x.startDateTime) = 0 then 'true' else 'false' end*/ 'true' as allDay,[PaintIcon],[WindowIcon],[DoorIcon], [M2000Icon], [FlagOrder],[TotalBoxQty], [TotalGlassQty], [TotalPrice], [TotalLBRMin], [F6CA], [F27DS], [F27TS], [F27TT], [F29CA], [F29CM], [F52PD], [F68CA], [F68SL], [F68VS], [Transom], [Sidelite], [SingleDoor], [DoubleDoor],  [BatchNo], Branch, JobType, [CardinalOrderedDate], [CompleteDate], [HighRiskFlag], [CustomFlag], CurrentStateName,NumberOfPatioDoors, Simple, Complex, Over_Size, [Arches], [Rakes], Customs
from (select p.ActionItemId as id, p.WorkOrderNumber as title, '' as [description], 
(select min(dateadd(MINUTE, (datepart(hour, pd.StartTime)*60+DATEPART(minute, pd.starttime)), pd.StartDate)) as date from PlantProduction_ProductionDate pd with(nolock,noexpand) where p.RecordId = pd.ParentRecordId) as startDateTime, 
(select max(dateadd(MINUTE, (datepart(hour, pd.EndTime)*60+DATEPART(minute, pd.EndTime)), pd.StartDate)) as date
from PlantProduction_ProductionDate pd with(nolock,noexpand) where p.RecordId = pd.ParentRecordId) as endDateTime, case when p.NumberofDoors is null then 0 else p.NumberofDoors end as doors, case when p.NumberOfPatioDoors is null then 0 else p.NumberOfPatioDoors end as NumberOfPatioDoors,
case when p.NumberofWindows is null then 0 else p.NumberofWindows end as windows, '' as type, [PaintIcon],[WindowIcon],[DoorIcon],case when [M2000Icon] = 'Yes' then 1 else 0 end as M2000Icon,case when [FlagOrder] = 'Yes' then 1 else 0 end as FlagOrder, [TotalBoxQty], [TotalGlassQty], [TotalPrice], [TotalLBRMin], [F6CA], [F27DS], [F27TS], [F27TT], [F29CA], [F29CM], [F52PD], [F68CA], [F68SL], [F68VS], [Transom], [Sidelite], [SingleDoor], [DoubleDoor], [BatchNo], Branch_display as Branch, JobType, [CardinalOrderedDate], [CompleteDate], [HighRiskFlag], [CustomFlag], CurrentStateName, [SimpleWindows] as Simple, [ComplexWindows] as Complex, [OverSize] as Over_Size, [Arches], [Rakes], [CustomWindows] as Customs 
 from PlantProduction p with(nolock,noexpand) 
where p.currentstatename in ({0}) and p.Branch in ({1}) and p.JobType in ({2}) and p.ShippingType in ({3}) and p.RecordId in (select pd.ParentRecordId from PlantProduction_ProductionDate pd with(nolock,noexpand) where p.RecordId = pd.ParentRecordId and pd.StartDate >= @pStart and pd.StartDate <= @pEnd and p.CurrentStateName <> 'Duplicated Work Order' and p.CurrentStateName <> 'Completed Reservations'
)) x", String.Format("'{0}'", String.Join("','", stateList)), String.Format("'{0}'", String.Join("','", branchList)), String.Format("'{0}'", String.Join("','", jobTypeList)), String.Format("'{0}'", String.Join("','", shippingTypeList)));
                case Generics.Utils.ContentType.Door:
                    return String.Format(@"select x.id, x.title +' W:' + cast(x.windows as varchar(max)) + ' D:' + cast(x.doors as varchar(max)) + ' PD:' + cast(x.NumberOfPatioDoors as varchar(max)) + ' - ' + case when x.Branch is null or Len(x.Branch) = 0 then 'NULL' else  LEft(x.Branch, 3) end + ' ' + x.JobType + case when @pStart = @pEnd then 'Day View stuff' else '' end as title, x.description, x.type, x.startDateTime, case when datediff(hour, x.startDateTime, x.endDateTime) = 0 then DATEADD(HOUR, 1, x.startDateTime) else x.endDateTime end as endDateTime, 
x.doors, x.windows, /*case when datediff(day, startDateTime, x.endDateTime) > 0 or datepart(hour, x.startDateTime) = 0 then 'true' else 'false' end*/ 'true' as allDay,[PaintIcon],[WindowIcon],[DoorIcon], [M2000Icon], [FlagOrder],[TotalBoxQty], [TotalGlassQty], [TotalPrice], [TotalLBRMin], [F6CA], [F27DS], [F27TS], [F27TT], [F29CA], [F29CM], [F52PD], [F68CA], [F68SL], [F68VS], [Transom], [Sidelite], [SingleDoor], [DoubleDoor],  [BatchNo], Branch, JobType, [CardinalOrderedDate], [CompleteDate], [HighRiskFlag], [CustomFlag],  CurrentStateName,NumberOfPatioDoors, Simple, Complex, Over_Size, [Arches], [Rakes], Customs
from (select p.ActionItemId as id, p.WorkOrderNumber as title, '' as [description], 
(select min(dateadd(MINUTE, (datepart(hour, pd.StartTime2)*60+DATEPART(minute, pd.StartTime2)), pd.StartDate2)) as date from PlantProduction_ProductionDate1 pd with(nolock,noexpand) where p.RecordId = pd.ParentRecordId) as startDateTime, 
(select max(dateadd(MINUTE, (datepart(hour, pd.EndTime)*60+DATEPART(minute, pd.EndTime)), pd.StartDate)) as date
from PlantProduction_ProductionDate pd with(nolock,noexpand) where p.RecordId = pd.ParentRecordId) as endDateTime, case when p.NumberofDoors is null then 0 else p.NumberofDoors end as doors, case when p.NumberOfPatioDoors is null then 0 else p.NumberOfPatioDoors end as NumberOfPatioDoors,
case when p.NumberofWindows is null then 0 else p.NumberofWindows end as windows, '' as type, [PaintIcon],[WindowIcon],[DoorIcon],case when [M2000Icon] = 'Yes' then 1 else 0 end as M2000Icon,case when [FlagOrder] = 'Yes' then 1 else 0 end as FlagOrder, [TotalBoxQty], [TotalGlassQty], [TotalPrice], [TotalLBRMin], [F6CA], [F27DS], [F27TS], [F27TT], [F29CA], [F29CM], [F52PD], [F68CA], [F68SL], [F68VS], [Transom], [Sidelite], [SingleDoor], [DoubleDoor],  [BatchNo], Branch_display as Branch, JobType, [CardinalOrderedDate], [CompleteDate], [HighRiskFlag], [CustomFlag], CurrentStateName, [SimpleWindows] as Simple, [ComplexWindows] as Complex, [OverSize] as Over_Size, [Arches], [Rakes], [CustomWindows] as Customs
 from PlantProduction p with(nolock,noexpand) 
where p.currentstatename in ({0}) and p.Branch in ({1}) and p.JobType in ({2}) and p.ShippingType in ({3}) and p.RecordId in (select pd.ParentRecordId from PlantProduction_ProductionDate1 pd with(nolock,noexpand) where p.RecordId = pd.ParentRecordId and pd.StartDate2 >= @pStart and pd.StartDate2 <= @pEnd and p.NumberOfDoors > 0 and p.CurrentStateName <> 'Duplicated Work Order' and p.CurrentStateName <> 'Completed Reservations'
)) x", String.Format("'{0}'", String.Join("','", stateList)), String.Format("'{0}'", String.Join("','", branchList)), String.Format("'{0}'", String.Join("','", jobTypeList)), String.Format("'{0}'", String.Join("','", shippingTypeList)));
                case Generics.Utils.ContentType.Paint:
                    return String.Format(@"select x.id, x.title +' W:' + cast(x.windows as varchar(max)) + ' D:' + cast(x.doors as varchar(max)) + ' PD:' + cast(x.NumberOfPatioDoors as varchar(max)) + ' - ' + case when x.Branch is null or Len(x.Branch) = 0 then 'NULL' else  LEft(x.Branch, 3) end + ' ' + x.JobType + case when @pStart = @pEnd then 'Day View stuff' else '' end as title, x.description, x.type, x.startDateTime, case when datediff(hour, x.startDateTime, x.endDateTime) = 0 then DATEADD(HOUR, 1, x.startDateTime) else x.endDateTime end as endDateTime, 
x.doors, x.windows, /*case when datediff(day, startDateTime, x.endDateTime) > 0 or datepart(hour, x.startDateTime) = 0 then 'true' else 'false' end*/ 'true' as allDay,[PaintIcon],[WindowIcon],[DoorIcon], [M2000Icon], [FlagOrder],[TotalBoxQty], [TotalGlassQty], [TotalPrice], [TotalLBRMin], [F6CA], [F27DS], [F27TS], [F27TT], [F29CA], [F29CM], [F52PD], [F68CA], [F68SL], [F68VS], [Transom], [Sidelite], [SingleDoor], [DoubleDoor],  [BatchNo], Branch, JobType, [CardinalOrderedDate], [CompleteDate], [HighRiskFlag], [CustomFlag],  CurrentStateName, NumberOfPatioDoors, Simple, Complex, Over_Size, [Arches], [Rakes], Customs
from (select p.ActionItemId as id, p.WorkOrderNumber as title, '' as [description], 
(select min(dateadd(MINUTE, (datepart(hour, pd.StartTime1)*60+DATEPART(minute, pd.StartTime1)), pd.StartDate1)) as date from PlantProduction_PaintDate pd with(nolock,noexpand) where p.RecordId = pd.ParentRecordId) as startDateTime, 
(select max(dateadd(MINUTE, (datepart(hour, pd.EndTime)*60+DATEPART(minute, pd.EndTime)), pd.StartDate)) as date
from PlantProduction_ProductionDate pd with(nolock,noexpand) where p.RecordId = pd.ParentRecordId) as endDateTime, case when p.NumberofDoors is null then 0 else p.NumberofDoors end as doors, case when p.NumberOfPatioDoors is null then 0 else p.NumberOfPatioDoors end as NumberOfPatioDoors,
case when p.NumberofWindows is null then 0 else p.NumberofWindows end as windows, '' as type, [PaintIcon],[WindowIcon],[DoorIcon],case when [M2000Icon] = 'Yes' then 1 else 0 end as M2000Icon,case when [FlagOrder] = 'Yes' then 1 else 0 end as FlagOrder, [TotalBoxQty], [TotalGlassQty], [TotalPrice], [TotalLBRMin], [F6CA], [F27DS], [F27TS], [F27TT], [F29CA], [F29CM], [F52PD], [F68CA], [F68SL], [F68VS], [Transom], [Sidelite], [SingleDoor], [DoubleDoor],  [BatchNo], Branch_display as Branch, JobType, [CardinalOrderedDate], [CompleteDate], [HighRiskFlag], [CustomFlag], CurrentStateName, [SimpleWindows] as Simple, [ComplexWindows] as Complex, [OverSize] as Over_Size, [Arches], [Rakes], [CustomWindows] as Customs
 from PlantProduction p with(nolock,noexpand) 
where p.currentstatename in ({0}) and p.Branch in ({1}) and p.JobType in ({2}) and p.ShippingType in ({3}) and p.RecordId in (select pd.ParentRecordId from PlantProduction_PaintDate pd with(nolock,noexpand) where p.RecordId = pd.ParentRecordId and pd.StartDate1 >= @pStart and pd.StartDate1 <= @pEnd and p.PaintIcon = 'Yes' and p.CurrentStateName <> 'Duplicated Work Order' and p.CurrentStateName <> 'Completed Reservations'
)) x", String.Format("'{0}'", String.Join("','", stateList)), String.Format("'{0}'", String.Join("','", branchList)), String.Format("'{0}'", String.Join("','", jobTypeList)), String.Format("'{0}'", String.Join("','", shippingTypeList)));
//                case Generics.Utils.ContentType.Installation:
//                    return string.Format(@"select * into #dates from HomeInstallations_InstallationDates d
//where d.ScheduledDate >= '12/30/2018 12:00' and d.ScheduledDate <= '02/10/2019 11:59'

//select i.* into #installs from HomeInstallations i 
//where CurrentStateName in ('Pending Install Completion', 'VP Installation Approval', 'Installation Manager Review', 'ReMeasure Scheduled', 'Work Scheduled', 'Unreviewed Work Scheduled', 'Install in Progress', 'Install Completed', 'Ready for Invoicing', 'Job Completed', 
//         'Installation Confirmed', 'Installation inprogress rejected', 'Rejected Remeasure', 'Rejected Scheduled Work', 
//        'Rejected Installation', 'Job Costing', 'Unreviewed Job Costing', 'Rejected Job Costing', 'VP Installation Approval', 'Rejected Manager Review', 
//'Pending Install Completion') and Branch in ('000000034','000000033','000000032','000000035','000075296','000023276') and i.Recordid in (select ParentRecordId from #dates group by ParentRecordId)

//insert into #installs select i.* from HomeInstallations i
//where CurrentStateName in ('Unreviewed Buffered Work', 'Buffered Work') and  (((PlannedInstallWeek >= 53) and PlannedInstallWeek <= 53) or 
//(PlannedInstallWeek >= 1 and PlannedInstallWeek <= 7)) and RecordId not in (select ParentRecordId from #dates d group by ParentRecordId) and  Branch in ('000000034','000000033','000000032','000000035','000075296','000023276')

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
//SELECT   i.Branch_Display as Branch, i.PaintedProduct, i.streetAddress, i.EstInstallerCnt, i.WorkOrderNumber, i.LastName, i.City, i.CurrentStateName,PlannedInstallWeek,
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
//drop table #Subtrade");
                default: throw new NotSupportedException(type.ToString());
            }
        }

        public List<InstallationEvent> GetInstallationEventData(ContentType type)
        {
            throw new NotImplementedException();
        }

        public List<InstallationEvent> GetData()
        {
            throw new NotImplementedException();
        }
    }



}