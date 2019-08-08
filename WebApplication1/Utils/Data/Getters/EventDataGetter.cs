﻿using System;
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

        private readonly string workOrderNumber;

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

        public EventDataGetter( List<string> branchList)
        {
            this.branchList = branchList;
        }

        public EventDataGetter(string start, string end)
        {
            this.startDate = DateTime.ParseExact(start.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            this.endDate = DateTime.ParseExact(end.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public EventDataGetter(string workOrderNumber)
        {
            this.workOrderNumber = workOrderNumber;
        }


        private List<InstallationEvent> GetInstallationEventsByWO(string WO)
        {
            string SQL = GetInstallationSQLForTotal(WO);
           
            var firstDayOfMonth = new DateTime(this.startDate.Year, this.startDate.Month, 1);
            var lastDayOfMonth = new DateTime(this.endDate.Year, this.endDate.Month, 1).AddMonths(1).AddDays(-1);

            List<InstallationEvent> installationEventList = new List<InstallationEvent>();

            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            pars.Add(new System.Data.SqlClient.SqlParameter("pStart", firstDayOfMonth));
            pars.Add(new System.Data.SqlClient.SqlParameter("pEnd", lastDayOfMonth));
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            installationEventList = Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.Data.InstallationEvent>(SQL, pars.ToArray());
          //   installationEventList.Count(bt => bt.WorkOrderNumber == WO &&bt.ReturnedJob!=1);
            return installationEventList.Where(bt => bt.WorkOrderNumber == WO && bt.ReturnedJob != 1).ToList();
        }
        private string GetInstallationSQLForTotal(string WO)
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
         
         
          
            //drop table #Subtrade", this.startDate.ToString("MM/dd/yyyy"), this.endDate.ToString("MM/dd/yyyy"), branchList, sPlanedCheck);

            string SQL = string.Format(@"select d.ScheduledDate1 as ScheduledDate,d.ParentRecordId, d.detailrecordid,count(c.detailrecordid) as detailrecordCount,1 as 'ReturnedJob'
into #dates 
from [HomeInstallations_ReturnTrip] d
join [HomeInstallations_ReturnTrip] c
on d.ParentRecordId= c.ParentRecordId
where d.ScheduledDate1 >= '{0} ' and d.ScheduledDate1 <= '{1} '
group by d.ScheduledDate1,d.ParentRecordId, d.detailrecordid
union all
select d.ScheduledDate,d.ParentRecordId, d.detailrecordid,count(c.detailrecordid) as detailrecordCount,0 as 'ReturnedJob'
from HomeInstallations_InstallationDates d
join HomeInstallations_InstallationDates c
on d.ParentRecordId= c.ParentRecordId
where d.ScheduledDate >= '{0} ' and d.ScheduledDate <= '{1} '
group by d.ScheduledDate,d.ParentRecordId, d.detailrecordid


select i.* into #installs from HomeInstallations i 
where CurrentStateName in ({3}) and Branch in ({2})  and i.Recordid in (select ParentRecordId from #dates group by ParentRecordId)
insert into #installs select i.* from HomeInstallations i
where CurrentStateName not in ('Unreviewed Buffered Work', 'Buffered Work') 
and  (((PlannedInstallWeek >= 53) and PlannedInstallWeek <= 53) or 
(PlannedInstallWeek >= 1 and PlannedInstallWeek <= 7)) and RecordId not in (select ParentRecordId from #dates d group by ParentRecordId) and 
Branch in ({2})

select t.* into #Windows from HomeInstallations_TypeofWork t inner join #installs i on i.RecordId = t.ParentRecordId where t.Type_1 = 'Windows'
select t.* into #Doors from HomeInstallations_TypeofWork t inner join #installs i on i.RecordId = t.ParentRecordId where t.Type_1 = 'Doors'
select t.* into #Other from HomeInstallations_TypeofWork t inner join #installs i on i.RecordId = t.ParentRecordId where t.Type_1 = 'Other'
select s.* into #Subtrade from HomeInstallations_SubtradeReqired s inner join #installs i on i.RecordId = s.ParentRecordId 

select WorkOrderNumber, LastName,FirstName, City,PostCode, Email,SalesRep,LeadPaint,ReturnedJob,SalesAmmount,TotalSalesAmount,TotalAsbestos,TotalWoodDropOff,TotalHighRisk,TotalDoors,TotalWindows,
DetailRecordId,ParentRecordId,id,detailrecordCount,saturday, sunday, 
jobtype,CurrentStateName,case when windows > 0 then WindowState else 'notordered' end as WindowState,
                    case when doors > 0 then DoorState else 'notordered' end as DoorState, case when other > 0 then 
OtherState else 'notordered' end as OtherState,
null as Hours, case when ElectricalSubtrade is not null then ElectricalSubtrade else 'Electrical: Unspecified' end  + ',
' + 
case when SidingSubtrade is not null then SidingSubtrade else 'Siding: Unspecified' end  + ',
' + 
case when InsulationSubtrade is not null then InsulationSubtrade else 'Insulation: Unspecified' end  + ',
' + 
case when OtherSubtrade is not null then OtherSubtrade else 'Other: Unspecified' end as Subtrades, Windows, Doors, Other, null as hours, 
HomePhoneNumber, CellPhone, WorkPhoneNumber, CrewNames, SeniorInstaller, 
case when ElectricalSubtrade is null and SidingSubtrade is null and InsulationSubtrade is null and
OtherSubtrade is null then 0 else 1 end as ShowSubtrades,
EstInstallerCnt, StreetAddress, ScheduledDate, case when ScheduledDate is null
then PlannedInstallWeek else null end as PlannedInstallWeek, PaintedProduct, Branch 
from (
SELECT   i.Branch_Display as Branch, i.PaintedProduct, ReturnedJob, i.SalesAmmount/detailrecordCount as SalesAmmount,i.SalesAmmount as TotalSalesAmount,DetailRecordId ,
ParentRecordId,detailrecordCount,saturday, sunday, jobtype,ActionItemId as id,i.streetAddress, i.EstInstallerCnt,
i.WorkOrderNumber, i.LastName, i.FirstName,i.City, i.PostalCode as PostCode,i.Email,i.Rep_display as SalesRep,i.LeadPaint ,
i.CurrentStateName,PlannedInstallWeek,
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
                          (SELECT     round(SUM(Number_1) /detailrecordCount,2) AS Number
                            FROM          #Windows
                            WHERE      (ParentRecordId = i.RecordId)
                            GROUP BY Type_1) AS Windows,
                          (SELECT     round(SUM(Number_1) ,2) AS Number
                            FROM          #Windows
                            WHERE      (ParentRecordId = i.RecordId)
                            GROUP BY Type_1) AS TotalWindows,
                          (SELECT     round(SUM(Number_1)/detailrecordCount,2)  AS Number
                            FROM          #Doors AS HomeInstallations_TypeofWork_2
                            WHERE      (ParentRecordId = i.RecordId)
                            GROUP BY Type_1) AS Doors,
                         (SELECT     round(SUM(Number_1),2)  AS Number
                            FROM          #Doors AS HomeInstallations_TypeofWork_2
                            WHERE      (ParentRecordId = i.RecordId)
                            GROUP BY Type_1) AS TotalDoors,
                          (SELECT      round(SUM(Number_1)/detailrecordCount,2) AS Number
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
where jobtype<>'Multi Family'
and ScheduledDate >= '{0} ' and ScheduledDate <= '{1} ' and WorkOrderNumber='{4} ' 
) x order by ScheduledDate, Branch

drop table #dates
drop table #installs
drop table #Windows
drop table #Doors
drop table #Other
drop table #Subtrade", 
new DateTime(this.startDate.Year, this.startDate.Month, 1).ToShortDateString(), 
new DateTime(this.endDate.Year, this.endDate.Month, 1).AddMonths(1).AddDays(-1).ToShortDateString(),
"'" + String.Join("','", branchList) + "'", "'" + String.Join("','", stateList) + "'", WO);
            return SQL;

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

               
            }
           
         
            string SQL = string.Format(@"select d.ScheduledDate1 as ScheduledDate,d.ParentRecordId, d.detailrecordid,count(c.detailrecordid) as detailrecordCount,1 as 'ReturnedJob'
into #dates 
from [HomeInstallations_ReturnTrip] d
join [HomeInstallations_ReturnTrip] c
on d.ParentRecordId= c.ParentRecordId
where d.ScheduledDate1 >= '{0} ' and d.ScheduledDate1 <= '{1} '
group by d.ScheduledDate1,d.ParentRecordId, d.detailrecordid
union all
select d.ScheduledDate,d.ParentRecordId, d.detailrecordid,count(c.detailrecordid) as detailrecordCount,0 as 'ReturnedJob'
from HomeInstallations_InstallationDates d
join HomeInstallations_InstallationDates c
on d.ParentRecordId= c.ParentRecordId
where d.ScheduledDate >= '{0} ' and d.ScheduledDate <= '{1} '
group by d.ScheduledDate,d.ParentRecordId, d.detailrecordid


select i.* into #installs from HomeInstallations i 
where CurrentStateName in ({3}) and Branch in ({2})  and i.Recordid in (select ParentRecordId from #dates group by ParentRecordId)
insert into #installs select i.* from HomeInstallations i
where CurrentStateName not in ('Unreviewed Buffered Work', 'Buffered Work') 
and  (((PlannedInstallWeek >= 53) and PlannedInstallWeek <= 53) or 
(PlannedInstallWeek >= 1 and PlannedInstallWeek <= 7)) and RecordId not in (select ParentRecordId from #dates d group by ParentRecordId) and 
Branch in ({2})

select WorkOrderNumber, LastName,FirstName, City,PostCode, Email,SalesRep,LeadPaint,ReturnedJob,StartScheduleDate,EndScheduleDate,SalesAmmount,TotalSalesAmount,TotalAsbestos,TotalWoodDropOff,TotalHighRisk,
TotalDoors,TotalWindows,Windows,Doors,ExtDoors,TotalExtDoors,
DetailRecordId,ParentRecordId,id,detailrecordCount,saturday, sunday, 
installationwindowLBRMIN,InstallationPatioDoorLBRMin,InstallationDoorLBRMin,TotalInstallationLBRMin,
installationwindowLBRMIN/detailrecordCount as subinstallationwindowLBRMIN,
InstallationDoorLBRMin/detailrecordCount as subExtDoorLBRMIN,
InstallationPatioDoorLBRMin/detailrecordCount as subInstallationPatioDoorLBRMin,
TotalInstallationLBRMin/detailrecordCount as subTotalInstallationLBRMin,
SidingLBRBudget,SidingLBRMin,SidingSQF,SubTradeFlag,

jobtype,CurrentStateName,null as Hours, null as hours, HomePhoneNumber, CellPhone, WorkPhoneNumber, 

EstInstallerCnt, StreetAddress, ScheduledDate, case when ScheduledDate is null
then PlannedInstallWeek else null end as PlannedInstallWeek, PaintedProduct, Branch 
from (
SELECT   i.Branch_Display as Branch, i.PaintedProduct, ReturnedJob,
installationwindowLBRMIN,InstallationPatioDoorLBRMin,InstallationDoorLBRMin,TotalInstallationLBRMin,
i.SalesAmmount/detailrecordCount as SalesAmmount,i.SalesAmmount as TotalSalesAmount,DetailRecordId ,
ParentRecordId,detailrecordCount,saturday, sunday, jobtype,ActionItemId as id,i.streetAddress, i.EstInstallerCnt,
i.WorkOrderNumber, i.LastName, i.FirstName,i.City, i.PostalCode as PostCode,i.Email,i.Rep_display as SalesRep,i.LeadPaint ,
i.CurrentStateName,PlannedInstallWeek,
SidingLBRBudget,SidingLBRMin,SidingSQF,i.SubTradeFlag,
dbo.fGetStartScheduleDate(ReturnedJob,RecordId) as StartScheduleDate,
dbo.fGetEndScheduleDate(ReturnedJob,RecordId) as EndScheduleDate,
round(i.Windows/detailrecordCount,2) as Windows, round(i. PatioDoors/detailrecordCount,2) as Doors,round(i. ExtDoors/detailrecordCount,2) as ExtDoors,
i.Windows as TotalWindows, i.PatioDoors as TotalDoors,  i.ExtDoors as TotalExtDoors,

						   d.ScheduledDate, 
                         
(SELECT count(*) 
          FROM HomeInstallations
         WHERE WoodDropOff='Yes' and i.RecordId = RecordId) TotalWoodDropOff,
(SELECT count(*) 
          FROM HomeInstallations
         WHERE Asbestos='Yes' and i.RecordId = RecordId) TotalAsbestos,
(SELECT count(*) 
          FROM HomeInstallations
         WHERE HighRisk='Yes' and i.RecordId = RecordId) TotalHighRisk,

 HomePhoneNumber, CellPhone, WorkPhoneNumber
FROM         #installs AS i LEFT OUTER JOIN
                      #dates AS d ON i.RecordId = d.ParentRecordId
where jobtype<>'Multi Family'

and ScheduledDate >= '{0} ' and ScheduledDate <= '{1} '
) x order by ScheduledDate, Branch

drop table #dates
drop table #installs
--drop table #Windows
--drop table #Doors
--drop table #Other
--drop table #Subtrade", this.startDate.ToShortDateString(), this.endDate.ToShortDateString(), "'" + String.Join("','", branchList) + "'", "'" + String.Join("','", stateList) + "'");
            return SQL;

        }

        List<Generics.Utils.Data.InstallationEvent> IGetter.GetData()
        {
            string SQL = GetInstallationSQL();


            int count, total, diff;
            List<InstallationEvent> installationEventList = new List<InstallationEvent>();

            //  List<InstallationEvent> returnEventList = new List<InstallationEvent>();
            List<InstallationEvent> returnEventList = new List<InstallationEvent>();

            List<InstallationEvent> returnedEventList = new List<InstallationEvent>();

            InstallationEvent newEvent;
            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            pars.Add(new System.Data.SqlClient.SqlParameter("pStart", startDate));
            pars.Add(new System.Data.SqlClient.SqlParameter("pEnd", endDate));
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            installationEventList = Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.Data.InstallationEvent>(SQL, pars.ToArray());

            returnedEventList = installationEventList.Where(x => x.ReturnedJob == 1).ToList();
            installationEventList = installationEventList.Where(x => x.ReturnedJob != 1).ToList();

            List<string> woList = new List<string>();

            foreach (InstallationEvent eventx in installationEventList)
            {
                newEvent = new InstallationEvent();
                newEvent.Branch = eventx.Branch;
                newEvent.CellPhone = eventx.CellPhone;
                newEvent.City = eventx.City;
                //   newEvent.CrewNames = eventx.CrewNames;
                newEvent.CurrentStateName = eventx.CurrentStateName;
                //newEvent.DoorState = eventx.DoorState;

                newEvent.start = installationEventList.Where(a => a.WorkOrderNumber == eventx.WorkOrderNumber).Min(b => b.ScheduledDate).
                          ToString();

                newEvent.end = installationEventList.Where(a => a.WorkOrderNumber == eventx.WorkOrderNumber).Max(b => b.ScheduledDate).
                   ToString();

                //newEvent.start = GetInstallationEventsByWO(eventx.WorkOrderNumber).Min(b => b.ScheduledDate).
                //   ToString();

                //newEvent.end = GetInstallationEventsByWO(eventx.WorkOrderNumber).Max(b => b.ScheduledDate).
                //   ToString();

                newEvent.EstInstallerCnt = eventx.EstInstallerCnt;
                newEvent.HomePhoneNumber = eventx.HomePhoneNumber;
                newEvent.Hours = eventx.Hours;
                newEvent.id = eventx.id;
                newEvent.LastName = eventx.LastName;
                newEvent.FirstName = eventx.FirstName;

                newEvent.detailrecordCount = eventx.detailrecordCount;

                newEvent.SubTradeFlag = eventx.SubTradeFlag;

                //  newEvent.Other = eventx.Other;
                //      newEvent.OtherState = eventx.OtherState;


                newEvent.ScheduledDate = eventx.ScheduledDate;


                newEvent.TotalSalesAmount = eventx.TotalSalesAmount;
                newEvent.SalesAmmount = eventx.SalesAmmount;
                newEvent.TotalAsbestos = eventx.TotalAsbestos;
                newEvent.TotalWoodDropOff = eventx.TotalWoodDropOff;
                newEvent.TotalHighRisk = eventx.TotalHighRisk;

                newEvent.installationwindowLBRMIN = eventx.installationwindowLBRMIN;
                newEvent.InstallationDoorLBRMin = eventx.InstallationDoorLBRMin;
                newEvent.InstallationPatioDoorLBRMin = eventx.InstallationPatioDoorLBRMin;
                newEvent.TotalInstallationLBRMin = eventx.TotalInstallationLBRMin;

                newEvent.subInstallationPatioDoorLBRMin = eventx.subInstallationPatioDoorLBRMin;
                newEvent.subinstallationwindowLBRMIN = eventx.subinstallationwindowLBRMIN;
                newEvent.subTotalInstallationLBRMin = eventx.subTotalInstallationLBRMin;
                newEvent.subExtDoorLBRMIN = eventx.subExtDoorLBRMIN;

                newEvent.SidingLBRBudget = eventx.SidingLBRBudget;
                newEvent.SidingLBRMin = eventx.SidingLBRMin;
                newEvent.SidingSQF = eventx.SidingSQF;


                newEvent.ExtDoors = eventx.ExtDoors;
                newEvent.TotalExtDoors = eventx.TotalExtDoors;

                newEvent.StartScheduleDate = eventx.StartScheduleDate;
                newEvent.EndScheduleDate = eventx.EndScheduleDate;


                // total = GetTotalByWO(eventx.WorkOrderNumber);


                //  total = GetInstallationEventsByWO(eventx.WorkOrderNumber).Count();
            //    total = installationEventList.Count(bt => bt.WorkOrderNumber == eventx.WorkOrderNumber);

                //if ((eventx.Saturday == "Yes") && (eventx.Sunday == "Yes"))
                //{
                //    newEvent.Windows = eventx.Windows;
                //    newEvent.Doors = eventx.Doors;
                //    newEvent.SalesAmmount = eventx.SalesAmmount;
                //}
                //else if ((eventx.Saturday == "No") && (eventx.Sunday == "No"))
                //{

                //    count = installationEventList.Count(bt => bt.WorkOrderNumber == eventx.WorkOrderNumber
                //    && bt.ScheduledDate.DayOfWeek != DayOfWeek.Sunday && bt.ScheduledDate.DayOfWeek != DayOfWeek.Saturday && bt.ReturnedJob != 1);
                //    diff = total - count;
                //    if ((eventx.ScheduledDate.DayOfWeek != DayOfWeek.Sunday) &&
                //                (eventx.ScheduledDate.DayOfWeek != DayOfWeek.Saturday))
                //    {
                //        if (diff == 0)
                //        {
                //            diff = total;
                //        }
                //        newEvent.Windows = eventx.Windows * total / diff;
                //        newEvent.Doors = eventx.Doors * total / diff;
                //        newEvent.SalesAmmount = eventx.SalesAmmount * total / diff;

                //    }

                //    //if ((eventx.ScheduledDate.DayOfWeek != DayOfWeek.Sunday) && 
                //    //        (eventx.ScheduledDate.DayOfWeek != DayOfWeek.Saturday) )
                //    //{
                //    //    if (diff==0)
                //    //    {
                //    //        diff = total;
                //    //    }
                //    //    newEvent.Windows = eventx.Windows * total / diff;
                //    //    newEvent.Doors = eventx.Doors * total / diff;
                //    //    newEvent.SalesAmmount = eventx.SalesAmmount * total / diff;

                //    //}
                //}
                //else if ((eventx.Saturday == "Yes") && (eventx.Sunday == "No"))
                //{
                //    count = installationEventList.Count(bt => bt.WorkOrderNumber == eventx.WorkOrderNumber &&
                //    bt.ScheduledDate.DayOfWeek == DayOfWeek.Sunday && bt.ScheduledDate.DayOfWeek != DayOfWeek.Saturday && bt.ReturnedJob != 1);
                //    diff = total - count;
                //    if (eventx.ScheduledDate.DayOfWeek != DayOfWeek.Sunday)
                //    {
                //        if (diff == 0)
                //        {
                //            diff = total;
                //        }
                //        newEvent.Windows = eventx.Windows * total / diff;
                //        newEvent.Doors = eventx.Doors * total / diff;
                //        newEvent.SalesAmmount = eventx.SalesAmmount * total / diff;

                //    }

                //}
                //else if ((eventx.Saturday == "No") && (eventx.Sunday == "Yes"))
                //{
                //    count = installationEventList.Count(bt => bt.WorkOrderNumber == eventx.WorkOrderNumber &&
                //   bt.ScheduledDate.DayOfWeek != DayOfWeek.Sunday && bt.ScheduledDate.DayOfWeek == DayOfWeek.Saturday && bt.ReturnedJob != 1);
                //    diff = total - count;
                //    if (eventx.ScheduledDate.DayOfWeek != DayOfWeek.Saturday)
                //    {
                //        if (diff == 0)
                //        {
                //            diff = total;
                //        }
                //        newEvent.Windows = eventx.Windows * total / diff;
                //        newEvent.Doors = eventx.Doors * total / diff;
                //        newEvent.SalesAmmount = eventx.SalesAmmount * total / diff;

                //    }

                //}

                //  newEvent.SeniorInstaller = eventx.SeniorInstaller;
                newEvent.StreetAddress = eventx.StreetAddress;
                //    newEvent.Subtrades = eventx.Subtrades;

                newEvent.title = eventx.title;

                newEvent.TotalWindows = eventx.TotalWindows;
                newEvent.TotalDoors = eventx.TotalDoors;
                //     newEvent.WindowState = eventx.WindowState;
                newEvent.WorkOrderNumber = eventx.WorkOrderNumber;
                newEvent.WorkPhoneNumber = eventx.WorkPhoneNumber;

                newEvent.Saturday = eventx.Saturday;
                newEvent.Sunday = eventx.Sunday;

                newEvent.ReturnedJob = eventx.ReturnedJob;



                newEvent.PostCode = eventx.PostCode;
                newEvent.Email = eventx.Email;
                newEvent.SalesRep = eventx.SalesRep;
                newEvent.LeadPaint = eventx.LeadPaint;

                woList.Add(eventx.WorkOrderNumber);
                returnEventList.Add(newEvent);

            }

            foreach (InstallationEvent returnedEvent in returnedEventList)
            {
                newEvent = new InstallationEvent();
                newEvent.Branch = returnedEvent.Branch;
                newEvent.CellPhone = returnedEvent.CellPhone;
                newEvent.City = returnedEvent.City;
                // newEvent.CrewNames = returnedEvent.CrewNames;
                newEvent.CurrentStateName = returnedEvent.CurrentStateName;
                newEvent.Doors = returnedEvent.Doors;
                // newEvent.DoorState = returnedEvent.DoorState;
                newEvent.detailrecordCount = returnedEvent.detailrecordCount;
                newEvent.start = returnedEventList.Where(a => a.WorkOrderNumber == returnedEvent.WorkOrderNumber).Min(b => b.ScheduledDate).
                   ToString();

                newEvent.end = returnedEventList.Where(a => a.WorkOrderNumber == returnedEvent.WorkOrderNumber).Max(b => b.ScheduledDate).
                   ToString();

                newEvent.EstInstallerCnt = returnedEvent.EstInstallerCnt;
                newEvent.HomePhoneNumber = returnedEvent.HomePhoneNumber;
                newEvent.Hours = returnedEvent.Hours;
                newEvent.id = returnedEvent.id;
                newEvent.LastName = returnedEvent.LastName;
                newEvent.FirstName = returnedEvent.FirstName;

                newEvent.SubTradeFlag = returnedEvent.SubTradeFlag;
                newEvent.SalesAmmount = returnedEvent.SalesAmmount;
                // newEvent.Other = returnedEvent.Other;
                //  newEvent.OtherState = returnedEvent.OtherState;

                newEvent.SalesAmmount = returnedEvent.SalesAmmount;
                newEvent.TotalSalesAmount = returnedEvent.TotalSalesAmount;
                newEvent.TotalAsbestos = returnedEvent.TotalAsbestos;
                newEvent.TotalWoodDropOff = returnedEvent.TotalWoodDropOff;
                newEvent.TotalHighRisk = returnedEvent.TotalHighRisk;
                newEvent.ScheduledDate = returnedEvent.ScheduledDate;
                //    newEvent.SeniorInstaller = returnedEvent.SeniorInstaller;
                newEvent.StreetAddress = returnedEvent.StreetAddress;
                //   newEvent.Subtrades = returnedEvent.Subtrades;

                newEvent.installationwindowLBRMIN = returnedEvent.installationwindowLBRMIN;
                newEvent.InstallationDoorLBRMin = returnedEvent.InstallationDoorLBRMin;
                newEvent.InstallationPatioDoorLBRMin = returnedEvent.InstallationPatioDoorLBRMin;
                newEvent.TotalInstallationLBRMin = returnedEvent.TotalInstallationLBRMin;


                newEvent.subInstallationPatioDoorLBRMin = returnedEvent.subInstallationPatioDoorLBRMin;
                newEvent.subinstallationwindowLBRMIN = returnedEvent.subinstallationwindowLBRMIN;
                newEvent.subTotalInstallationLBRMin = returnedEvent.subTotalInstallationLBRMin;
                newEvent.subExtDoorLBRMIN = returnedEvent.subExtDoorLBRMIN;

                newEvent.SidingLBRBudget = returnedEvent.SidingLBRBudget;
                newEvent.SidingLBRMin = returnedEvent.SidingLBRMin;
                newEvent.SidingSQF = returnedEvent.SidingSQF;


                newEvent.ExtDoors = returnedEvent.ExtDoors;
                newEvent.TotalExtDoors = returnedEvent.TotalExtDoors;

                newEvent.StartScheduleDate = returnedEvent.StartScheduleDate;
                newEvent.EndScheduleDate = returnedEvent.EndScheduleDate;


                newEvent.title = returnedEvent.title;
                newEvent.Windows = returnedEvent.Windows;
                newEvent.TotalWindows = returnedEvent.TotalWindows;
                newEvent.TotalDoors = returnedEvent.TotalDoors;
                //   newEvent.WindowState = returnedEvent.WindowState;
                newEvent.WorkOrderNumber = returnedEvent.WorkOrderNumber;
                newEvent.WorkPhoneNumber = returnedEvent.WorkPhoneNumber;

                newEvent.Saturday = returnedEvent.Saturday;
                newEvent.Sunday = returnedEvent.Sunday;

                newEvent.ReturnedJob = returnedEvent.ReturnedJob;
                newEvent.PostCode = returnedEvent.PostCode;
                newEvent.Email = returnedEvent.Email;
                newEvent.SalesRep = returnedEvent.SalesRep;
                newEvent.LeadPaint = returnedEvent.LeadPaint;

                woList.Add(returnedEvent.WorkOrderNumber);
                returnEventList.Add(newEvent);

            }

            return returnEventList;
        }

        //        private string GetRemeasureSQL()
        //        {

        //            string sPlanedCheck = @"";
        //            // Create an instance of Norwegian culture
        //            System.Globalization.CultureInfo Culture =
        //            System.Globalization.CultureInfo.CreateSpecificCulture("ca");
        //            // Get the Norwegian calendar from the culture object
        //            System.Globalization.Calendar cal = Culture.Calendar;




        //            if (this.endDate.Year - this.startDate.Year > 0)
        //            {
        //                sPlanedCheck = string.Format(@" (((PlannedInstallWeek >= {0}) and PlannedInstallWeek <= {1}) or 
        //(PlannedInstallWeek >= {2} and PlannedInstallWeek <= {3}))",
        //                                                    cal.GetWeekOfYear(this.startDate, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday),
        //                                                    cal.GetWeekOfYear(DateTime.Parse("12/31/" + this.startDate.Year.ToString() + " 11:59:59 pm"), System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday),
        //                                                    cal.GetWeekOfYear(DateTime.Parse("1/1/" + this.endDate.Year.ToString()), System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday),
        //                                                    cal.GetWeekOfYear(this.endDate, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday)
        //                                                   );
        //            }
        //            else
        //            {
        //                // render simple Planed installation week chack
        //                sPlanedCheck = string.Format(@"PlannedInstallWeek >= {0} and PlannedInstallWeek <= {1}",
        //                    cal.GetWeekOfYear(this.startDate, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday),
        //                    cal.GetWeekOfYear(this.endDate, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday));


        //            }


        //            string SQL = string.Format(@"


        //select i.* into #installs from HomeInstallations i 
        //where CurrentStateName in ({3}) and Branch in ({2})  
        //insert into #installs select i.* from HomeInstallations i
        //where CurrentStateName in ('ReMeasure Scheduled') 
        //and  (((PlannedInstallWeek >= 53) and PlannedInstallWeek <= 53) or 
        //(PlannedInstallWeek >= 1 and PlannedInstallWeek <= 7)) and 
        //Branch in ({2})

        //select WorkOrderNumber, LastName,FirstName, City,PostCode, Email,SalesRep,LeadPaint,SalesAmmount,TotalSalesAmount,TotalAsbestos,TotalWoodDropOff,TotalHighRisk,
        //TotalDoors,TotalWindows,Windows,Doors,ExtDoors,TotalExtDoors,
        //saturday, sunday, 
        //installationwindowLBRMIN,InstallationPatioDoorLBRMin,InstallationDoorLBRMin,TotalInstallationLBRMin,
        //installationwindowLBRMIN as subinstallationwindowLBRMIN,
        //InstallationDoorLBRMin as subExtDoorLBRMIN,
        //InstallationPatioDoorLBRMin as subInstallationPatioDoorLBRMin,
        //TotalInstallationLBRMin as subTotalInstallationLBRMin,
        //SidingLBRBudget,SidingLBRMin,SidingSQF,RemeasureDate,RemeasureEndTime,id,

        //jobtype,CurrentStateName,null as Hours, null as hours, HomePhoneNumber, CellPhone, WorkPhoneNumber, 

        //EstInstallerCnt, StreetAddress, PaintedProduct, Branch 
        //from (
        //SELECT   i.Branch_Display as Branch, i.PaintedProduct,
        //installationwindowLBRMIN,InstallationPatioDoorLBRMin,InstallationDoorLBRMin,TotalInstallationLBRMin,
        //i.SalesAmmount as SalesAmmount,i.SalesAmmount as TotalSalesAmount 
        //,saturday, sunday, jobtype,ActionItemId as id,i.streetAddress, i.EstInstallerCnt,
        //i.WorkOrderNumber, i.LastName, i.FirstName,i.City, i.PostalCode as PostCode,i.Email,i.Rep_display as SalesRep,i.LeadPaint ,
        //i.CurrentStateName,PlannedInstallWeek,
        //SidingLBRBudget,SidingLBRMin,SidingSQF,i.RemeasureDate,i.RemeasureEndTime,
        //i.Windows as Windows,i. PatioDoors as Doors,i. ExtDoors as ExtDoors,
        //i.Windows as TotalWindows, i.PatioDoors as TotalDoors,  i.ExtDoors as TotalExtDoors,

        //(SELECT count(*) 
        //          FROM HomeInstallations
        //         WHERE WoodDropOff='Yes' and i.RecordId = RecordId) TotalWoodDropOff,
        //(SELECT count(*) 
        //          FROM HomeInstallations
        //         WHERE Asbestos='Yes' and i.RecordId = RecordId) TotalAsbestos,
        //(SELECT count(*) 
        //          FROM HomeInstallations
        //         WHERE HighRisk='Yes' and i.RecordId = RecordId) TotalHighRisk,

        // HomePhoneNumber, CellPhone, WorkPhoneNumber
        //FROM         #installs AS i 
        //where jobtype<>'Multi Family'

        //and RemeasureDate >= '{0} ' and RemeasureDate <= '{1} '
        //and  CurrentStateName in ('ReMeasure Scheduled') 
        //) x order by RemeasureDate, Branch

        //drop table #installs
        //--drop table #Windows
        //--drop table #Doors
        //--drop table #Other
        //--drop table #Subtrade", this.startDate.ToShortDateString(), this.endDate.ToShortDateString(), "'" + String.Join("','", branchList) + "'", "'" + String.Join("','", stateList) + "'");
        //            return SQL;

        //        }

        private string GetRemeasureSQL()
        {

            string SQL = string.Format(@"



                         
select WorkOrderNumber, LastName,FirstName, City,PostCode, Email,SalesRep,LeadPaint,SalesAmmount,TotalSalesAmount,TotalAsbestos,TotalWoodDropOff,TotalHighRisk,
TotalDoors,TotalWindows,Windows,Doors,ExtDoors,TotalExtDoors,
saturday, sunday, 
installationwindowLBRMIN,InstallationPatioDoorLBRMin,InstallationDoorLBRMin,TotalInstallationLBRMin,
installationwindowLBRMIN as subinstallationwindowLBRMIN,
InstallationDoorLBRMin as subExtDoorLBRMIN,
InstallationPatioDoorLBRMin as subInstallationPatioDoorLBRMin,
TotalInstallationLBRMin as subTotalInstallationLBRMin,
SidingLBRBudget,SidingLBRMin,SidingSQF,RemeasureDate,RemeasureEndTime,id,

jobtype,CurrentStateName,null as Hours, null as hours, HomePhoneNumber, CellPhone, WorkPhoneNumber, 

EstInstallerCnt, StreetAddress, PaintedProduct, Branch 
from (
SELECT   i.Branch_Display as Branch, i.PaintedProduct,
installationwindowLBRMIN,InstallationPatioDoorLBRMin,InstallationDoorLBRMin,TotalInstallationLBRMin,
i.SalesAmmount as SalesAmmount,i.SalesAmmount as TotalSalesAmount 
,saturday, sunday, jobtype,ActionItemId as id,i.streetAddress, i.EstInstallerCnt,
i.WorkOrderNumber, i.LastName, i.FirstName,i.City, i.PostalCode as PostCode,i.Email,i.Rep_display as SalesRep,i.LeadPaint ,
i.CurrentStateName,PlannedInstallWeek,
SidingLBRBudget,SidingLBRMin,SidingSQF,i.RemeasureDate,i.RemeasureEndTime,
i.Windows as Windows,i. PatioDoors as Doors,i. ExtDoors as ExtDoors,
i.Windows as TotalWindows, i.PatioDoors as TotalDoors,  i.ExtDoors as TotalExtDoors,
                         
(SELECT count(*) 
          FROM HomeInstallations
         WHERE WoodDropOff='Yes' and i.RecordId = RecordId) TotalWoodDropOff,
(SELECT count(*) 
          FROM HomeInstallations
         WHERE Asbestos='Yes' and i.RecordId = RecordId) TotalAsbestos,
(SELECT count(*) 
          FROM HomeInstallations
         WHERE HighRisk='Yes' and i.RecordId = RecordId) TotalHighRisk,

HomePhoneNumber, CellPhone, WorkPhoneNumber
FROM         [flowserv_flowfinityapps].[dbo].[HomeInstallations] AS i 
where jobtype<>'Multi Family' 
and RemeasureDate >= '{0} ' and RemeasureDate <= '{1} '
and i.RemeasureDate is not null
and Branch  in ({2})
and  CurrentStateName in ({3})
--and  CurrentStateName in ('Ready for ReMeasure', 'Rejected Remeasure') 

) x order by RemeasureDate,Branch", this.startDate.ToShortDateString(), this.endDate.ToShortDateString(), "'" + String.Join("','", branchList) + "'", "'" + String.Join("','", stateList) + "'");
            return SQL;

        }

        List<Generics.Utils.Data.RemeasureEvent> IGetter.GetRemeasureData()
        {
            string SQL = GetRemeasureSQL();

            List<RemeasureEvent> remeasureEventList = new List<RemeasureEvent>();

            //  List<InstallationEvent> returnEventList = new List<InstallationEvent>();
            List<RemeasureEvent> returnEventList = new List<RemeasureEvent>();


            RemeasureEvent newEvent;
            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            pars.Add(new System.Data.SqlClient.SqlParameter("pStart", startDate));
            pars.Add(new System.Data.SqlClient.SqlParameter("pEnd", endDate));
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            remeasureEventList = Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.Data.RemeasureEvent>(SQL, pars.ToArray());

            List<string> woList = new List<string>();

            foreach (RemeasureEvent eventx in remeasureEventList)
            {
                newEvent = new RemeasureEvent();
                newEvent.Branch = eventx.Branch;
                newEvent.CellPhone = eventx.CellPhone;
                newEvent.City = eventx.City;
                //   newEvent.CrewNames = eventx.CrewNames;
                newEvent.CurrentStateName = eventx.CurrentStateName;
                //newEvent.DoorState = eventx.DoorState;
                newEvent.RemeasureDate = eventx.RemeasureDate;
                newEvent.RemeasureEndTime = eventx.RemeasureEndTime;
                newEvent.title = eventx.title;
                newEvent.EstInstallerCnt = eventx.EstInstallerCnt;
                newEvent.HomePhoneNumber = eventx.HomePhoneNumber;
                newEvent.Hours = eventx.Hours;

                newEvent.start = remeasureEventList.Where(a => a.WorkOrderNumber == eventx.WorkOrderNumber).Min(b => b.RemeasureDate).
                   ToString();

                newEvent.end = newEvent.start;

                newEvent.id = eventx.id;


                newEvent.LastName = eventx.LastName;
                newEvent.FirstName = eventx.FirstName;
     
                newEvent.TotalSalesAmount = eventx.TotalSalesAmount;
                newEvent.TotalAsbestos = eventx.TotalAsbestos;
                newEvent.TotalWoodDropOff = eventx.TotalWoodDropOff;
                newEvent.TotalHighRisk = eventx.TotalHighRisk;

                newEvent.installationwindowLBRMIN = eventx.installationwindowLBRMIN;
                newEvent.InstallationDoorLBRMin = eventx.InstallationDoorLBRMin;
                newEvent.InstallationPatioDoorLBRMin = eventx.InstallationPatioDoorLBRMin;
                newEvent.TotalInstallationLBRMin = eventx.TotalInstallationLBRMin;

                newEvent.subInstallationPatioDoorLBRMin = eventx.subInstallationPatioDoorLBRMin;
                newEvent.subinstallationwindowLBRMIN = eventx.subinstallationwindowLBRMIN;
                newEvent.subTotalInstallationLBRMin = eventx.subTotalInstallationLBRMin;
                newEvent.subExtDoorLBRMIN = eventx.subExtDoorLBRMIN;

                newEvent.SidingLBRBudget = eventx.SidingLBRBudget;
                newEvent.SidingLBRMin = eventx.SidingLBRMin;
                newEvent.SidingSQF = eventx.SidingSQF;


                newEvent.ExtDoors = eventx.ExtDoors;
                newEvent.TotalExtDoors = eventx.TotalExtDoors;

                newEvent.Windows = eventx.Windows;
                newEvent.Doors = eventx.Doors;
                newEvent.SalesAmmount = eventx.SalesAmmount;

                //  newEvent.SeniorInstaller = eventx.SeniorInstaller;
                newEvent.StreetAddress = eventx.StreetAddress;
                //    newEvent.Subtrades = eventx.Subtrades;

                newEvent.TotalWindows = eventx.TotalWindows;
                newEvent.TotalDoors = eventx.TotalDoors;
                //     newEvent.WindowState = eventx.WindowState;
                newEvent.WorkOrderNumber = eventx.WorkOrderNumber;
                newEvent.WorkPhoneNumber = eventx.WorkPhoneNumber;

                newEvent.Saturday = eventx.Saturday;
                newEvent.Sunday = eventx.Sunday;

                newEvent.PostCode = eventx.PostCode;
                newEvent.Email = eventx.Email;
                newEvent.SalesRep = eventx.SalesRep;
                newEvent.LeadPaint = eventx.LeadPaint;

                woList.Add(eventx.WorkOrderNumber);
                returnEventList.Add(newEvent);

            }

            return returnEventList;
        }

        //List<InstallationEvent> IGetter.GetReturnEventData()
        //{
        //    string SQL = GetInstallationReturnEventSQL();
        //    List<InstallationEvent> installationEventList = new List<InstallationEvent>();
        //    //  List<InstallationEvent> returnEventList = new List<InstallationEvent>();
        //    List<InstallationEvent> returnEventList = new List<InstallationEvent>();
        //    InstallationEvent newEvent;
        //    List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
        //    pars.Add(new System.Data.SqlClient.SqlParameter("pStart", startDate));
        //    pars.Add(new System.Data.SqlClient.SqlParameter("pEnd", endDate));
        //    Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
        //    installationEventList = Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.Data.InstallationEvent>(SQL, pars.ToArray());
        //    List<string> woList = new List<string>();
        //    foreach (InstallationEvent eventx in installationEventList)
        //    {
        //        //if (woList.Contains(eventx.WorkOrderNumber))
        //        //{
        //        //    continue;
        //        //}
        //        newEvent = new InstallationEvent();
        //        newEvent.Branch = eventx.Branch;
        //        newEvent.CellPhone = eventx.CellPhone;
        //        newEvent.City = eventx.City;
        //        newEvent.CrewNames = eventx.CrewNames;
        //        newEvent.CurrentStateName = eventx.CurrentStateName;
        //        newEvent.Doors = eventx.Doors;
        //        newEvent.DoorState = eventx.DoorState;


        //        newEvent.start = installationEventList.Where(a => a.WorkOrderNumber == eventx.WorkOrderNumber).Min(b => b.ScheduledDate).
        //           ToString();

        //        newEvent.end = installationEventList.Where(a => a.WorkOrderNumber == eventx.WorkOrderNumber).Max(b => b.ScheduledDate).
        //           ToString();

        //        newEvent.EstInstallerCnt = eventx.EstInstallerCnt;
        //        newEvent.HomePhoneNumber = eventx.HomePhoneNumber;
        //        newEvent.Hours = eventx.Hours;
        //        newEvent.id = eventx.id;
        //        newEvent.LastName = eventx.LastName;
        //        newEvent.Other = eventx.Other;
        //        newEvent.OtherState = eventx.OtherState;

        //        newEvent.SalesAmmount = eventx.SalesAmmount;
        //        newEvent.TotalSalesAmount = eventx.TotalSalesAmount;
        //        newEvent.TotalAsbestos = eventx.TotalAsbestos;
        //        newEvent.TotalWoodDropOff = eventx.TotalWoodDropOff;
        //        newEvent.TotalHighRisk = eventx.TotalHighRisk;
        //        newEvent.ScheduledDate = eventx.ScheduledDate;
        //        newEvent.ReturnEventDate= eventx.ReturnEventDate;
        //        newEvent.SeniorInstaller = eventx.SeniorInstaller;
        //        newEvent.StreetAddress = eventx.StreetAddress;
        //        newEvent.Subtrades = eventx.Subtrades;

        //        newEvent.title = eventx.title;
        //        newEvent.Windows = eventx.Windows;
        //        newEvent.TotalWindows = eventx.TotalWindows;
        //        newEvent.TotalDoors = eventx.TotalDoors;
        //        newEvent.WindowState = eventx.WindowState;
        //        newEvent.WorkOrderNumber = eventx.WorkOrderNumber;
        //        newEvent.WorkPhoneNumber = eventx.WorkPhoneNumber;

        //        newEvent.Saturday = eventx.Saturday;
        //        newEvent.Sunday = eventx.Sunday;

        //        woList.Add(eventx.WorkOrderNumber);
        //        returnEventList.Add(newEvent);

        //    }
        //    return returnEventList;
        //}


        private List<InstallationEvent> GetInstallationDateByWOForReturnedJob(string WO)
        {
            string SQL = GetInstallationDateByWOSQL(WO);
            List<InstallationEvent> returnList = new List<InstallationEvent>();

            returnList = Lift.LiftManager.DbHelper.ReadObjects<InstallationEvent>(SQL).Where(b=>b.ReturnedJob==1).ToList();
         
            return returnList;
        }

        private List<InstallationEvent> GetInstallationDateByWOForNonReturnedJob(string WO)
        {
            string SQL = GetInstallationDateByWOSQL(WO);
            List<InstallationEvent> returnList = new List<InstallationEvent>();

            returnList = Lift.LiftManager.DbHelper.ReadObjects<InstallationEvent>(SQL).Where(b => b.ReturnedJob != 1).ToList();

            return returnList;
        }



        private string GetInstallationDateByWOSQL(string WO)
        {
            string SQL=string.Empty ;
            SQL =  String.Format(@"
            select d.ScheduledDate1 as ScheduledDate,d.ParentRecordId, d.detailrecordid,1 as 'ReturnedJob'
            from [HomeInstallations_ReturnTrip] d
            join [HomeInstallations_ReturnTrip] c
            on d.ParentRecordId= c.ParentRecordId
            where  d.ParentRecordId=(select recordid from HomeInstallations where workordernumber in '{0} '
            union all
            select d.ScheduledDate,d.ParentRecordId, d.detailrecordid,0 as 'ReturnedJob'
            from HomeInstallations_InstallationDates d
            join HomeInstallations_InstallationDates c
            on d.ParentRecordId= c.ParentRecordId)
            where  d.ParentRecordId =(select recordid from HomeInstallations where workordernumber in '{0}')",WO);
           

            return SQL;
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
        //List<Generics.Utils.CalendarEvent> IGetter.GetData(Generics.Utils.ContentType type)
        //{
        //    //string SQL = GetSQL(type);

        //    //List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
        //    //pars.Add(new System.Data.SqlClient.SqlParameter("pStart", startDate));
        //    //pars.Add(new System.Data.SqlClient.SqlParameter("pEnd", endDate));
        //    //Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
        //    //return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.CalendarEvent>(SQL, pars.ToArray());
        //    string SQL = GetSQL(type);
        //    List<CalendarEvent> eventList = new List<CalendarEvent>();
        //    List<CalendarEvent> returnList = new List<CalendarEvent>();
        //    CalendarEvent newEvent = new CalendarEvent();
        //    List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
        //    pars.Add(new System.Data.SqlClient.SqlParameter("pStart", startDate));
        //    pars.Add(new System.Data.SqlClient.SqlParameter("pEnd", endDate));
        //    Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
        //    int dayDiff = 0;
        //    List<CalendarEvent> re = new List<CalendarEvent>();
        //    eventList = Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.CalendarEvent>(SQL, pars.ToArray());

        //    foreach (CalendarEvent e in eventList)
        //    {
        //        dayDiff = Convert.ToInt32((e.endDateTime - e.startDateTime).TotalDays);

        //        if (dayDiff == 0)
        //        {
        //            e.ScheduledProductionDate = e.startDateTime;
        //            returnList.Add(e);
        //        }
        //        else
        //        {
        //            for (int i = 0; i <= dayDiff; i++)
        //            {
        //                newEvent = new CalendarEvent();
        //                newEvent.allDay = e.allDay;
        //                newEvent.Arches = e.Arches;
        //                newEvent.BatchNo = e.BatchNo;
        //                newEvent.Branch = e.Branch;
        //                newEvent.CardinalOrderedDate = e.CardinalOrderedDate;
        //                newEvent.CurrentStateName = e.CurrentStateName;
        //                newEvent.color = e.color;

        //                newEvent.CompleteDate = e.CompleteDate;
        //                newEvent.Complex = e.Complex;
        //                newEvent.CustomFlag = e.CustomFlag;
        //                newEvent.Customs = e.Customs;

        //                newEvent.description = e.description;
        //                newEvent.DoorIcon = e.DoorIcon;

        //                newEvent.end = e.end;

        //                newEvent.F27DS = e.F27DS;
        //                newEvent.F27TS = e.F27TS;
        //                newEvent.F27TT = e.F27TT;
        //                newEvent.F29CA = e.F29CA;
        //                newEvent.F29CM = e.F29CM;

        //                newEvent.F68CA = e.F68CA;
        //                newEvent.F68SL = e.F68SL;
        //                newEvent.F68VS = e.F68VS;
        //                newEvent.F6CA = e.F6CA;
        //                newEvent.FlagOrder = e.FlagOrder;

        //                newEvent.HighRiskFlag = e.HighRiskFlag;
        //                newEvent.HolidayDate = e.HolidayDate;
        //                newEvent.HolidayName = e.HolidayName;

        //                newEvent.id = e.id;
        //                newEvent.isHoliday = e.isHoliday;
        //                newEvent.JobType = e.JobType;
        //                newEvent.M2000Icon = e.M2000Icon;
        //                newEvent.NumberOfPatioDoors = e.NumberOfPatioDoors;
        //                newEvent.Over_Size = e.Over_Size;
        //                newEvent.PaintIcon = e.PaintIcon;
        //                newEvent.Rakes = e.Rakes;
        //                newEvent.Sidelite = e.Sidelite;
        //                newEvent.Simple = e.Simple;

        //                newEvent.start = e.start;
        //                newEvent.end = e.end;
        //                newEvent.startDateTime = e.startDateTime;
        //                newEvent.endDateTime = e.endDateTime;
        //                newEvent.ScheduledProductionDate = e.startDateTime.AddDays(i);

        //                newEvent.title = e.title;

        //                newEvent.TotalBoxQty = e.TotalBoxQty;
        //                newEvent.TotalGlassQty = e.TotalGlassQty;
        //                newEvent.TotalLBRMin = e.TotalLBRMin;
        //                newEvent.TotalPrice = e.TotalPrice;
        //                newEvent.Transom = e.Transom;
        //                newEvent.WindowIcon = e.WindowIcon;

        //                newEvent.SingleDoor = e.SingleDoor;

        //                newEvent.F52PD = e.F52PD / (dayDiff + 1);
        //                newEvent.doors = e.doors / (dayDiff + 1);
        //                newEvent.DoubleDoor = e.DoubleDoor / (dayDiff + 1);
        //                newEvent.windows = e.windows / (dayDiff + 1);

        //                returnList.Add(newEvent);
        //            }

        //        }
        //    }



        //    return returnList;

        //}
        //List<Generics.Utils.CalendarEvent> IGetter.GetData(Generics.Utils.ContentType type)
        //{
        //    string SQL = GetSQL(type);
        //    List<CalendarEvent> eventList = new List<CalendarEvent>();
        //    List<CalendarEvent> returnList = new List<CalendarEvent>();
        //    CalendarEvent newEvent = new CalendarEvent();
        //    List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
        //    pars.Add(new System.Data.SqlClient.SqlParameter("pStart", startDate));
        //    pars.Add(new System.Data.SqlClient.SqlParameter("pEnd", endDate));
        //    Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
        //    eventList = Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.CalendarEvent>(SQL, pars.ToArray());

        //    int dayDiff = 0;
        //    int k = 0;
        //    List<CalendarEvent> re = new List<CalendarEvent>();

        //    foreach (CalendarEvent e in eventList)
        //    {
        //        dayDiff = Convert.ToInt32((e.endDateTime - e.startDateTime).TotalDays);
        //        if (dayDiff == 0)
        //        {
        //            e.ScheduledProductionDate = e.startDateTime;
        //            returnList.Add(e);
        //        }
        //        else
        //        {
        //            for (int i = 0; i <= dayDiff; i++)
        //            {
        //                newEvent = new CalendarEvent();
        //                newEvent.allDay = e.allDay;
        //                newEvent.Arches = e.Arches;
        //                newEvent.BatchNo = e.BatchNo;
        //                newEvent.Branch = e.Branch;
        //                newEvent.CardinalOrderedDate = e.CardinalOrderedDate;
        //                newEvent.CurrentStateName = e.CurrentStateName;
        //                newEvent.color = e.color;

        //                newEvent.CompleteDate = e.CompleteDate;
        //                newEvent.Complex = e.Complex;
        //                newEvent.CustomFlag = e.CustomFlag;
        //                newEvent.Customs = e.Customs;

        //                newEvent.description = e.description;
        //                newEvent.DoorIcon = e.DoorIcon;
        //                newEvent.doors = e.doors;
        //                newEvent.DoubleDoor = e.DoubleDoor;

        //                newEvent.end = e.end;

        //                newEvent.F27DS = e.F27DS;
        //                newEvent.F27TS = e.F27TS;
        //                newEvent.F27TT = e.F27TT;
        //                newEvent.F29CA = e.F29CA;
        //                newEvent.F29CM = e.F29CM;
        //                newEvent.F52PD = e.F52PD;
        //                newEvent.F68CA = e.F68CA;
        //                newEvent.F68SL = e.F68SL;
        //                newEvent.F68VS = e.F68VS;
        //                newEvent.F6CA = e.F6CA;
        //                newEvent.FlagOrder = e.FlagOrder;

        //                newEvent.HighRiskFlag = e.HighRiskFlag;
        //                newEvent.HolidayDate = e.HolidayDate;
        //                newEvent.HolidayName = e.HolidayName;

        //                newEvent.id = e.id;
        //                newEvent.isHoliday = e.isHoliday;
        //                newEvent.JobType = e.JobType;
        //                newEvent.M2000Icon = e.M2000Icon;
        //                newEvent.NumberOfPatioDoors = e.NumberOfPatioDoors;
        //                newEvent.Over_Size = e.Over_Size;
        //                newEvent.PaintIcon = e.PaintIcon;
        //                newEvent.Rakes = e.Rakes;
        //                newEvent.ScheduledProductionDate = e.startDateTime.AddDays(i);
        //                newEvent.Sidelite = e.Sidelite;
        //                newEvent.Simple = e.Simple;


        //               newEvent.start = e.start;
        //               newEvent.end= e.end;
        //               newEvent.startDateTime = e.startDateTime;
        //               newEvent.endDateTime = e.endDateTime;
        //               newEvent.ScheduledProductionDate = e.startDateTime.AddDays(i);

        //                newEvent.title = e.title;

        //                newEvent.TotalBoxQty = e.TotalBoxQty;
        //                newEvent.TotalGlassQty = e.TotalGlassQty;
        //                newEvent.TotalLBRMin = e.TotalLBRMin;
        //                newEvent.TotalPrice = e.TotalPrice;
        //                newEvent.Transom = e.Transom;
        //                newEvent.WindowIcon = e.WindowIcon;
        //                newEvent.windows = e.windows;

        //                newEvent.SingleDoor = e.SingleDoor;
        //                re.Add(newEvent);
        //            }
        //            returnList.AddRange(re);
        //        }


        //    }

        //    return returnList.OrderBy(x => x.startDateTime).ToList(); ;
        //}

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
where HolidayDate >= '{0}' and HolidayDate <= '{1}'", this.startDate, this.endDate);
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
x.startDateTime,x.ScheduledProductionDate, 
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

        List<InstallationEvent> IGetter.GetInstallationBufferData()
        {
            throw new NotImplementedException();
        }

        public List<Product> GetProducts()
        {
            string SQL = GetProductSQL();

            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            pars.Add(new System.Data.SqlClient.SqlParameter("pWorkOrderNumber", this.workOrderNumber));
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.Product>(SQL, pars.ToArray());
        }

        private string GetProductSQL()
        {
            string SQL = string.Format(@"  SELECT i.WorkOrderNumber,
				    [Item]
      ,[System_1] as System
      ,[Description1] as Description
      ,[Size_1] as Size
      ,[Quantity1] as Quantity
      ,[SubQty]
      ,[Status1] as Status
   
  FROM [flowserv_flowfinityapps].[dbo].[HomeInstallations_WindowItems] INNER JOIN
       [flowserv_flowfinityapps].[dbo].[HomeInstallations] AS i ON i.RecordId = ParentRecordId
where i.WorkOrderNumber = '{0}' ", this.workOrderNumber);
            return SQL;
        }


        public List<UnavailableHR> GetUnavailableResources()
        {
            string SQL = GetGetUnavailableResourcesSQL();

            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.UnavailableHR>(SQL);
        }


        private string GetGetUnavailableResourcesSQL()
        {
            string SQL = string.Format(@"SELECT
                   DateUnavailable
                                 ,u.[Name]
                                ,[Branch_display] AS Branch
                                ,[UnavailableReason_display] AS Reason
                     ,[AdminNote]

  FROM [flowserv_flowfinityapps].[dbo].[UnavailableStaff] as r INNER JOIN
       [flowserv_flowfinityapps].[dbo].[UnavailableStaff_UnavailableStaff] as us ON recordid = us.[ParentRecordId] INNER JOIN
                   [flowserv_flowfinityapps].[dbo].[Users] as u ON us.[UserId] = u.[userid]
where branch in ('{0}') ", String.Join("','", branchList));
            return SQL;
        }


        public List<Product> GetProductsDoors()
        {
            string SQL = GetProductDoorSQL();

            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            pars.Add(new System.Data.SqlClient.SqlParameter("pWorkOrderNumber", this.workOrderNumber));
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.Product>(SQL, pars.ToArray());
        }

        private string GetProductDoorSQL()
        {
            string SQL = string.Format(@"select  i.WorkOrderNumber
         ,[Item1]  as Item
      ,[Size1] as Size
         ,[Quantity11] as Quantity
      ,[SubQty1] as SubQty
      ,[System1] as System
      ,[Description11] as Description
      ,[Status11] as Status
FROM [flowserv_flowfinityapps].[dbo].[HomeInstallations_DoorItems] INNER JOIN
       [flowserv_flowfinityapps].[dbo].[HomeInstallations] AS i ON i.RecordId = ParentRecordId

where i.WorkOrderNumber = '{0}' ", this.workOrderNumber);
            return SQL;
        }

        public List<Product> GetManufacturingWindows()
        {
            string SQL = GetManufacturingWindowsSQL();

            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            pars.Add(new System.Data.SqlClient.SqlParameter("pWorkOrderNumber", this.workOrderNumber));
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.Product>(SQL, pars.ToArray());
        }

        private string GetManufacturingWindowsSQL()
        {
            string SQL = string.Format(@"select  p.WorkOrderNumber
         ,i.[Item] 
      ,i.[Size_1] as Size
         ,i.[Quantity]
      ,i.[SubQty]
      ,i.[System_1] as System
      ,i.[Description]
      ,i.[Status]
  FROM [flowserv_flowfinityapps].[dbo].[PlantProduction_items] as i INNER JOIN
       [flowserv_flowfinityapps].[dbo].[PlantProduction] as p ON i.ParentRecordId = p.RecordId
where p.WorkOrderNumber = '{0}' ", this.workOrderNumber);
            return SQL;
        }

        public List<Product> GetManufacturingDoors()
        {
            string SQL = GetManufacturingDoorsSQL();

            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            pars.Add(new System.Data.SqlClient.SqlParameter("pWorkOrderNumber", this.workOrderNumber));
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.Product>(SQL, pars.ToArray());
        }

        private string GetManufacturingDoorsSQL()
        {
            string SQL = string.Format(@"				   	select  p.WorkOrderNumber
         ,[Item1]  as Item
      ,[Size1] as Size
         ,[Quantity1]as Quantity
      ,[SubQty1] as SubQty
      ,[System1] as System
      ,[Description1] as Description
      ,[Status1] as Status
  FROM [flowserv_flowfinityapps].[dbo].[PlantProduction_DoorItems] as i INNER JOIN
       [flowserv_flowfinityapps].[dbo].[PlantProduction] as p ON i.ParentRecordId = p.RecordId
where p.WorkOrderNumber = '{0}' ", this.workOrderNumber);
            return SQL;
        }

        public List<Installer> GetInstallers()
        {
            string SQL = GeInstallerSQL();

            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            pars.Add(new System.Data.SqlClient.SqlParameter("WorkOrderNumber", this.workOrderNumber));
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.Installer>(SQL, pars.ToArray());
        }

        private string GeInstallerSQL()
        {
            string SQL = string.Format(@"SELECT     e.InstallerName   as SeniorInstaller,
dbo.fGetCrewNames(i.RecordId) as CrewNames
  from   Employees AS e INNER JOIN
                      Users AS u ON e.Account_1 = u.Account INNER JOIN
                      HomeInstallations_SeniorInstaller AS si ON u.UserId = si.userId
					  inner join HomeInstallations i on  i.RecordId=si.ParentRecordId
where WorkOrderNumber = '{0}' ", this.workOrderNumber);
            return SQL;
        }


        private string GetWindowsCustomerInfo()
        {
            string SQL = string.Format(@"select distinct p.WorkOrderNumber,CustomerName, City, Address, PhoneNUmber, ShippingType, Email, Branch_display,
NumberOfWindows as TotalWindows,NumberOfDoors as TotalDoors,NumberOfPatioDoors as TotalPatioDoors,TotalPrice
       
  FROM [flowserv_flowfinityapps].[dbo].[PlantProduction_items] as i INNER JOIN
       [flowserv_flowfinityapps].[dbo].[PlantProduction] as p ON i.ParentRecordId = p.RecordId
where p.WorkOrderNumber = '{0}' ", this.workOrderNumber);
            return SQL;
        }

        public List<WindowsCustomer> GetWindowsCustomer()
        {
            string SQL = GetWindowsCustomerInfo();

            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            pars.Add(new System.Data.SqlClient.SqlParameter("WorkOrderNumber", this.workOrderNumber));
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.WindowsCustomer>(SQL, pars.ToArray());
        }

        private string GetCalledLogSQL()
        {
            string SQL = string.Format(@"SELECT  [DateCalled]
      ,[CalledMessage]
      ,[Notes3]
      ,[ParentRecordId]
      ,[DetailRecordId]
  FROM [flowserv_flowfinityapps].[dbo].[HomeInstallations_CallLog] cl

    inner join HomeInstallations i on  i.RecordId=cl.ParentRecordId
where WorkOrderNumber = '{0}' ", this.workOrderNumber);
            return SQL;
        }

        public List<CalledLog> GetCalledLog()
        {
            string SQL = GetCalledLogSQL();

            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            pars.Add(new System.Data.SqlClient.SqlParameter("WorkOrderNumber", this.workOrderNumber));
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.CalledLog>(SQL, pars.ToArray());
        }


        private string GetSubTradesSQL()
        {
            string SQL = string.Format(@"SELECT [SubTrade]
      ,[Status]
  FROM [flowserv_flowfinityapps].[dbo].[HomeInstallations_SubtradeReqired] INNER JOIN
       [flowserv_flowfinityapps].[dbo].[HomeInstallations] AS i ON i.RecordId = ParentRecordId

where i.WorkOrderNumber = '{0}' ", this.workOrderNumber);
            return SQL;
        }

        public List<SubTrades> GetSubTrades()
        {
            string SQL = GetSubTradesSQL();

            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            pars.Add(new System.Data.SqlClient.SqlParameter("WorkOrderNumber", this.workOrderNumber));
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.SubTrades>(SQL, pars.ToArray());
        }
        public List<WOPicture> GetWOPicture()
        {
            string SQL = GetWOPictureSQL();

            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            pars.Add(new System.Data.SqlClient.SqlParameter("WorkOrderNumber", this.workOrderNumber));
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.WOPicture>(SQL, pars.ToArray());
        }

        private string GetWOPictureSQL()
        {
            string SQL = string.Format(@"SELECT [input125] as PictureName
      ,[Picture]
      ,[ParentRecordId]
      ,[DetailRecordId],WorkOrderNumber,tb.[Picture_thumbnail2] as pic
  FROM [flowserv_flowfinityapps].[dbo].[HomeInstallations_TakePicture] tp
  inner join  HomeInstallations i on  i.RecordId=tp.ParentRecordId
  inner join [HomeInstallations_TakePicture__binaries] tb on tb.recordid = tp.DetailRecordId
where WorkOrderNumber = '{0}' ", this.workOrderNumber);
            return SQL;
        }

        List<InstallationEvent> IGetter.GetInstallationDateByWOForReturnedJob(string wO)
        {
            throw new NotImplementedException();
        }

        List<InstallationEvent> IGetter.GetInstallationDateByWOForNonReturnedJob(string wO)
        {
            throw new NotImplementedException();
        }

        public List<RemeasureEvent> GetRemeasureBufferData()
        {
            throw new NotImplementedException();
        }
    }



}