using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowfinityConnectionHelper.FASR;
using Generics.Utils;

namespace FlowfinityConnectionHelper
{
    public class FlowfinityUpdateHelper : Generics.RecordUpdate.IUpdateHelper
    {
        private readonly Utils.ActionsCommHelper _helper;
        private readonly string Owner;
        private FlowfinityUpdateHelper() { }
        public FlowfinityUpdateHelper(string owner, Utils.ActionsCommHelper _helper)
        {
            this.Owner = owner;
            this._helper = _helper;
        }
        bool Generics.RecordUpdate.IUpdateHelper.UpdateRecord(Generics.Utils.ContentType type, Generics.Utils.ImproperCalendarEvent data)
        {
            FASR.PlantProduction_UpdateWindowMakerData_Call call = new FASR.PlantProduction_UpdateWindowMakerData_Call()
            {
                OnBehalfOf = Owner,
                RecordID = data.id,
                Record = GetRecord(type, data)
            };
            return _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data)).ReturnValue;
        }

        bool Generics.RecordUpdate.IUpdateHelper.UpdateRecord(Generics.Utils.ContentType type, Generics.Utils.ImproperInstallationEvent data)
        {
            //FASR.PlantProduction_UpdateWindowMakerData_Call call = new FASR.PlantProduction_UpdateWindowMakerData_Call()
            // HomeInstallations_EditSold_Call
            FASR.HomeInstallations_EditSold_Call  call = new FASR.HomeInstallations_EditSold_Call()
            {
                OnBehalfOf = Owner,
                RecordID = data.id,
                
                Record = GetRecord(type, data)
            };
            return _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data)).ReturnValue;
        }

      

        bool Generics.RecordUpdate.IUpdateHelper.UpdateRecord(Generics.Utils.ContentType type, Generics.Utils.InstallationEventWeekends data)
        {
            FASR.HomeInstallations_EditSold_Call call = new FASR.HomeInstallations_EditSold_Call()
            {
                OnBehalfOf = Owner,
                RecordID = data.id,
                Record = GetRecord(type, data)
            };
            return _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data)).ReturnValue;
        }

        private static FASR.HomeInstallationsRecord GetRecord(Generics.Utils.ContentType type, Generics.Utils.ImproperInstallationEvent data)
        {
            FASR.HomeInstallationsRecord record = new FASR.HomeInstallationsRecord();
            record.InstallationDates = PrepareInstallationDateList(data);
            return record;
        }
        //GetRecordForReturnedJob
        private static FASR.HomeInstallationsRecord GetRecordForReturnedJob(Generics.Utils.ContentType type, Generics.Utils.ImproperInstallationEvent data)
        {
            FASR.HomeInstallationsRecord record = new FASR.HomeInstallationsRecord();
            record.ReturnTrip = PrepareReturnedInstallationDateList(data);
            return record;
        }
        private static FASR.HomeInstallationsRecord GetRecord(Generics.Utils.ContentType type, Generics.Utils.InstallationEventWeekends data)
        {
            FASR.HomeInstallationsRecord record = new FASR.HomeInstallationsRecord();
            record.Saturday = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>(data.Saturday);
            record.Sunday = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>(data.Sunday);
            return record;
        }

        private static FASR.PlantProductionRecord GetRecord(Generics.Utils.ContentType type, Generics.Utils.ImproperCalendarEvent data)
        {
            FASR.PlantProductionRecord record = new FASR.PlantProductionRecord();
            if (type == Generics.Utils.ContentType.Window)
                record.ProductionDate = PrepareWindowProductionDateList(data);
            if (type == Generics.Utils.ContentType.Customer)
                record.DeliveryDate = Lift.II.IIUtils.CreateDateTimeValue<FASR.DateTimeValue>(Generics.Utils.Date.DateParser.ParseTime(data.start));
            else if (type.Equals(Generics.Utils.ContentType.Door))
                record.ProductionDate1 = PrepareDoorProductionDateList(data);
            else if (type.Equals(Generics.Utils.ContentType.Paint))
                record.PaintDate = PreparePaintProductionDateList(data);
            else if (type.Equals(Generics.Utils.ContentType.Shipping))
                record.ShippingDate = PrepareShippingDateList(data);
            return record;
        }
        

        private static PlantProduction_ShippingDateRecord[] PrepareShippingDateList(ImproperCalendarEvent data)
        {
            List<PlantProduction_ShippingDateRecord> returnValue = new List<PlantProduction_ShippingDateRecord>();

            DateTime start = Generics.Utils.Date.DateParser.ParseTime(data.start);
            DateTime end = Generics.Utils.Date.DateParser.ParseTime(data.end);
            if ((end - start).TotalDays >= 1)
            {
                end = end.AddDays(1);
                while ((end - start).TotalDays >= 1)
                {
                    returnValue.Add(new FASR.PlantProduction_ShippingDateRecord()
                    {
                        ShippingStartDate = new FASR.DateTimeValue() { Value = start },
                        ShippingStartTime = new FASR.DateTimeValue() { Value = new DateTime() },
                        ShippingEndTime = new FASR.DateTimeValue() { Value = new DateTime() }
                    });
                    start = start.AddDays(1);
                }
            }
            else
                returnValue.Add(new FASR.PlantProduction_ShippingDateRecord()
                {
                    ShippingStartDate = new FASR.DateTimeValue() { Value = start },
                    ShippingStartTime = new FASR.DateTimeValue() { Value = start },
                    ShippingEndTime = new FASR.DateTimeValue() { Value = end }
                });
            return returnValue.ToArray();
        }

        private static string PrepareTransactionId(Generics.Utils.ImproperCalendarEvent data)
        {
            return string.Format("{0} {1} {2}", "update", data.id, DateTime.Now.Ticks.ToString());
        }

        private static string PrepareTransactionId(Generics.Utils.InstallationEventWeekends data)
        {
            return string.Format("{0} {1} {2}", "update", data.id, DateTime.Now.Ticks.ToString());
        }

        private static string PrepareTransactionId(Generics.Utils.ImproperInstallationEvent data)
        {
            return string.Format("{0} {1} {2}", "update", data.id, DateTime.Now.Ticks.ToString());
        }

        private static FASR.HomeInstallations_InstallationDatesRecord[] PrepareInstallationDateList(Generics.Utils.ImproperInstallationEvent data)
        {
            List<FASR.HomeInstallations_InstallationDatesRecord> returnValue = new List<FASR.HomeInstallations_InstallationDatesRecord>();

            DateTime start = Generics.Utils.Date.DateParser.ParseTime(data.start);
            DateTime end = Generics.Utils.Date.DateParser.ParseTime(data.end);

            if ((end - start).TotalDays >= 1)
            {
                end = end.AddDays(1);
                while ((end - start).TotalDays >= 1)
                {
                    returnValue.Add(new FASR.HomeInstallations_InstallationDatesRecord()
                    {
                        ScheduledDate = new FASR.DateTimeValue() { Value = start }
                    });
                    start = start.AddDays(1);
                }
            }
            else
                returnValue.Add(new FASR.HomeInstallations_InstallationDatesRecord()
                {
                    ScheduledDate = new FASR.DateTimeValue() { Value = start }
                });
            return returnValue.ToArray();
        }

        private static FASR.HomeInstallations_ReturnTripRecord [] PrepareReturnedInstallationDateList(Generics.Utils.ImproperInstallationEvent data)
        {
            List<FASR.HomeInstallations_ReturnTripRecord> returnValue = new List<FASR.HomeInstallations_ReturnTripRecord>();
            DateTime start = Generics.Utils.Date.DateParser.ParseTime(Convert.ToDateTime(data.start).ToString("yyyy-MM-ddT00:00:00.000Z"));
            DateTime end = Generics.Utils.Date.DateParser.ParseTime(Convert.ToDateTime(data.end).ToString("yyyy-MM-ddT00:00:00.000Z"));

            if ((end - start).TotalDays >= 1)
            {
                end = end.AddDays(1);
                while ((end - start).TotalDays >= 1)
                {
                    returnValue.Add(new FASR.HomeInstallations_ReturnTripRecord()
                    {
                        ScheduledDate1 = new FASR.DateTimeValue() { Value = start }
                    });
                    start = start.AddDays(1);
                }
            }
            else
                returnValue.Add(new FASR.HomeInstallations_ReturnTripRecord()
                {
                    ScheduledDate1 = new FASR.DateTimeValue() { Value = start }
                });
            return returnValue.ToArray();
        }



        private static FASR.PlantProduction_ProductionDateRecord[] PrepareWindowProductionDateList(Generics.Utils.ImproperCalendarEvent data)
        {
            List<FASR.PlantProduction_ProductionDateRecord> returnValue = new List<FASR.PlantProduction_ProductionDateRecord>();

            DateTime start = Generics.Utils.Date.DateParser.ParseTime(data.start);
            DateTime end = Generics.Utils.Date.DateParser.ParseTime(data.end);

            if ((end - start).TotalDays >= 1)
            {
                end = end.AddDays(1);
                while ((end - start).TotalDays >= 1)
                {
                    returnValue.Add(new FASR.PlantProduction_ProductionDateRecord()
                    {
                        StartDate = new FASR.DateTimeValue() { Value = start },
                        StartTime = new FASR.DateTimeValue() { Value = new DateTime() },
                        EndTime = new FASR.DateTimeValue() { Value = new DateTime() }
                    });
                    start = start.AddDays(1);
                }
            }
            else
                returnValue.Add(new FASR.PlantProduction_ProductionDateRecord()
                {
                    StartDate = new FASR.DateTimeValue() { Value = start },
                    StartTime = new FASR.DateTimeValue() { Value = start },
                    EndTime = new FASR.DateTimeValue() { Value = end }
                });
            return returnValue.ToArray();
        }
        private static FASR.PlantProduction_ProductionDate1Record[] PrepareDoorProductionDateList(Generics.Utils.ImproperCalendarEvent data)
        {
            List<FASR.PlantProduction_ProductionDate1Record> returnValue = new List<FASR.PlantProduction_ProductionDate1Record>();

            DateTime start = Generics.Utils.Date.DateParser.ParseTime(data.start);
            DateTime end = Generics.Utils.Date.DateParser.ParseTime(data.end);

            if ((end - start).TotalDays >= 1)
            {
                end = end.AddDays(1);
                while ((end - start).TotalDays >= 1)
                {
                    returnValue.Add(new FASR.PlantProduction_ProductionDate1Record()
                    {
                        StartDate2 = new FASR.DateTimeValue() { Value = start },
                        StartTime2 = new FASR.DateTimeValue() { Value = new DateTime() },
                        EndTime2 = new FASR.DateTimeValue() { Value = new DateTime() }
                    });
                    start = start.AddDays(1);
                }
            }
            else
                returnValue.Add(new FASR.PlantProduction_ProductionDate1Record()
                {
                    StartDate2 = new FASR.DateTimeValue() { Value = start },
                    StartTime2 = new FASR.DateTimeValue() { Value = start },
                    EndTime2 = new FASR.DateTimeValue() { Value = end }
                });
            return returnValue.ToArray();
        }
        private static FASR.PlantProduction_PaintDateRecord[] PreparePaintProductionDateList(Generics.Utils.ImproperCalendarEvent data)
        {
            List<FASR.PlantProduction_PaintDateRecord> returnValue = new List<FASR.PlantProduction_PaintDateRecord>();

            DateTime start = Generics.Utils.Date.DateParser.ParseTime(data.start);
            DateTime end = Generics.Utils.Date.DateParser.ParseTime(data.end);

            if ((end - start).TotalDays >= 1)
            {
                end = end.AddDays(1);
                while ((end - start).TotalDays >= 1)
                {
                    returnValue.Add(new FASR.PlantProduction_PaintDateRecord()
                    {
                        StartDate1 = new FASR.DateTimeValue() { Value = start },
                        StartTime1 = new FASR.DateTimeValue() { Value = new DateTime() },
                        EndTime1 = new FASR.DateTimeValue() { Value = new DateTime() }
                    });
                    start = start.AddDays(1);
                }
            }
            else
                returnValue.Add(new FASR.PlantProduction_PaintDateRecord()
                {
                    StartDate1 = new FASR.DateTimeValue() { Value = start },
                    StartTime1 = new FASR.DateTimeValue() { Value = start },
                    EndTime1 = new FASR.DateTimeValue() { Value = end }
                });
            return returnValue.ToArray();
        }

        public bool UpdateRecordForReturnedJob(ContentType type, ImproperInstallationEvent data)
        {
            throw new NotImplementedException();
        }

        bool Generics.RecordUpdate.IUpdateHelper.UpdateRecordForReturnedJob(Generics.Utils.ContentType type, Generics.Utils.ImproperInstallationEvent data)
        {
            //FASR.PlantProduction_UpdateWindowMakerData_Call call = new FASR.PlantProduction_UpdateWindowMakerData_Call()
            // HomeInstallations_EditSold_Call
            FASR.HomeInstallations_RescheduleInstallation_Call call = new FASR.HomeInstallations_RescheduleInstallation_Call()
            {
                OnBehalfOf = Owner,
                RecordID = data.id,
                Record = GetRecordForReturnedJob(type, data)
            };
            return _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data)).ReturnValue;
        }

    }
}
