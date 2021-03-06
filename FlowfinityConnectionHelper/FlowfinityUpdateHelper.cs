﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowfinityConnectionHelper.FASR;
using Generics.RecordUpdate;
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
            bool ret;
            bool ret1 = true;
            FASR.HomeInstallations_EditSold_Call call = new FASR.HomeInstallations_EditSold_Call()
            {
                OnBehalfOf = Owner,
                RecordID = data.id,

                Record = GetRecord(type, data)
            };
            ret = _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data)).ReturnValue;

            if (data.CurrentStateName == "Unreviewed Work Scheduled")
            {
                FASR.HomeInstallations_ScheduleNewWorkOrder_Call call1 = new FASR.HomeInstallations_ScheduleNewWorkOrder_Call()
                {
                    OnBehalfOf = Owner,
                    RecordID = data.id,

                    Record = GetRecordForStateChanges(type, data)
                };
                ret1 = _helper.Send(new FASR.OperationCall[] { call1 }, PrepareTransactionId(data)).ReturnValue;
            }

            return ret & ret1;

        }


        bool Generics.RecordUpdate.IUpdateHelper.UpdateRecord(Generics.Utils.ContentType type, Generics.Utils.ImproperTruckInstallationEvent data)
        {
            //FASR.PlantProduction_UpdateWindowMakerData_Call call = new FASR.PlantProduction_UpdateWindowMakerData_Call()
            // HomeInstallations_EditSold_Call
            bool ret;
          
            FASR.HomeInstallations_EditSold_Call call = new FASR.HomeInstallations_EditSold_Call()
            {
                OnBehalfOf = Owner,
                RecordID = data.id,

                Record = GetRecord(type, data)
            };
            ret = _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data)).ReturnValue;
            return ret ;

        }


        bool Generics.RecordUpdate.IUpdateHelper.UpdateRecord(Generics.Utils.ContentType type, Generics.Utils.ImproperRemeasureEvent data)
        {
            bool ret;
            bool ret1 = true;

            if ((data.CurrentStateName == "Ready for ReMeasure") || (data.CurrentStateName == "Rejected Remeasure"))
            {
                FASR.HomeInstallations_ScheduleRemeasure_Call call = new FASR.HomeInstallations_ScheduleRemeasure_Call()
                {
                    OnBehalfOf = Owner,
                    RecordID = data.id,

                    Record = GetRecord(type, data)
                };
                ret = _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data)).ReturnValue;
            }
            else
            {
                FASR.HomeInstallations_EditSold_Call call = new FASR.HomeInstallations_EditSold_Call()
                {
                    OnBehalfOf = Owner,
                    RecordID = data.id,

                    Record = GetRecord(type, data)
                };
                ret = _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data)).ReturnValue;

            }

            //if (data.CurrentStateName == "Unreviewed Work Scheduled")
            //{
            //    FASR.HomeInstallations_ScheduleNewWorkOrder_Call call1 = new FASR.HomeInstallations_ScheduleNewWorkOrder_Call()
            //    {
            //        OnBehalfOf = Owner,
            //        RecordID = data.id,

            //        Record = GetRecordForStateChanges(type, data)
            //    };
            //    ret1 = _helper.Send(new FASR.OperationCall[] { call1 }, PrepareTransactionId(data)).ReturnValue;
            //}

            return ret & ret1;

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

        private static FASR.HomeInstallationsRecord GetRecord(Generics.Utils.ContentType type, Generics.Utils.ImproperRemeasureEvent data)
        {

            FASR.HomeInstallationsRecord record = new FASR.HomeInstallationsRecord();
            record.RemeasureDate = new FASR.DateTimeValue() { Value = Generics.Utils.Date.DateParser.ParseTime(data.start) };
            record.RemeasureEndTime = new FASR.DateTimeValue() { Value = Generics.Utils.Date.DateParser.ParseTime(data.end) };
            return record;

        }


        private static FASR.HomeInstallationsRecord GetRecord(Generics.Utils.ContentType type, Generics.Utils.ImproperInstallationEvent data)
        {
            FASR.HomeInstallationsRecord record = new FASR.HomeInstallationsRecord();
            record.InstallationDates = PrepareInstallationDateList(data);
          
            return record;
        }

        private static FASR.HomeInstallationsRecord GetRecord(Generics.Utils.ContentType type, Generics.Utils.ImproperTruckInstallationEvent data)
        {
            FASR.HomeInstallationsRecord record = new FASR.HomeInstallationsRecord();
            record.AssignTruck = PreparTruckList(data);
            return record;
        }



        private static FASR.HomeInstallationsRecord GetCalledLogRecord(Generics.Utils.ContentType type, List<Generics.Utils.CalledLog> data)
        {
            FASR.HomeInstallationsRecord record = new FASR.HomeInstallationsRecord();
            if (data == null)
            {
                record.CallLog = new HomeInstallations_CallLogRecord[0];
            }
            else
            {
                record.CallLog = PrepareInstallationCallLogList(data);
            }
            //record.CallLog = PrepareInstallationCallLogList(data);
            //if (data.Count == 1)
            //{
            //    if (data[0].DateCalled == null)
            //    {
            //        record.CallLog = null;
            //    }
            //}
            return record;
        }


        private static FASR.HomeInstallationsRecord GetNotesRecord(Generics.Utils.ContentType type, List<Generics.Utils.Notes> data)
        {
            FASR.HomeInstallationsRecord record = new FASR.HomeInstallationsRecord();
            if (data == null)
            {
                record.GeneralNotesList = new HomeInstallations_GeneralNotesListRecord[0];
            }
            else
            {
                record.GeneralNotesList = PrepareInstallationNotesList(data);
            }


            return record;
        }

        private static FASR.HomeInstallationsRecord GetRecordForStateChanges(Generics.Utils.ContentType type, Generics.Utils.ImproperInstallationEvent data)
        {
            FASR.HomeInstallationsRecord record = new FASR.HomeInstallationsRecord();
            // record.InstallationDates = PrepareInstallationDateList(data);
            return record;
        }

        private static FASR.HomeInstallationsRecord GetRecord(Generics.Utils.ContentType type, Generics.Utils.InstallationDataEvent data)
        {
            FASR.HomeInstallationsRecord record = new FASR.HomeInstallationsRecord();
            record.InstallationDates = PrepareInstallationDateList(data);
            record.Saturday = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>(data.Saturday);
            record.Sunday = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>(data.Sunday);

            //  record.Asbestos= Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>(data.Asbestos.ToString());
            if (data.Asbestos == 1)
            {
                record.Asbestos = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>("Yes");
            }
            else
            {
                record.Asbestos = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>("No");
            }

            record.LeadPaint = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>(data.LeadPaint.ToString());

            if (data.WoodDropOff == 1)
            {
                record.WoodDropOff = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>("Yes");
                //  DateTime WoodDropOffDate = Generics.Utils.Date.DateParser.ParseTime(Convert.ToDateTime(data.WoodDropDateAndTime).ToString("yyyy-MM-ddT00:00:00.000Z"));
                DateTime WoodDropOffDate = Generics.Utils.Date.DateParser.ParseTime(Convert.ToDateTime(data.WoodDropDateAndTime).ToString("yyyy-MM-ddTHH:mm:00.000Z"));
                record.WoodDropOffDate = Lift.II.IIUtils.CreateDateTimeValue<FASR.DateTimeValue>(WoodDropOffDate);
            }
            else
            {
                record.WoodDropOff = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>("No");
            }
            //  record.WoodDropOff = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>(data.WoodDropOff.ToString());
            // record.HighRisk = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>(data.HighRisk.ToString());

            if (data.HighRisk == 1)
            {
                record.HighRisk = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>("Yes");
            }
            else
            {
                record.HighRisk = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>("No");
            }

            record.EstInstallerCnt = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>(data.EstInstallerCnt.ToString());

            return record;
        }

        //GetRecordForReturnedJob
        private static FASR.HomeInstallationsRecord GetRecordForReturnedJob(Generics.Utils.ContentType type, Generics.Utils.ImproperInstallationEvent data)
        {
            FASR.HomeInstallationsRecord record = new FASR.HomeInstallationsRecord();
            record.ReturnTrip = PrepareReturnedInstallationDateList(data);
            record.ReturnTripReason = Lift.II.IIUtils.CreateStringValue<StringValue>(data.ReturnTripReason);
            return record;
        }

        private static FASR.HomeInstallationsRecord GetCrewRecord(Generics.Utils.ContentType type, List<Generics.Utils.InstallerWithLessInfo> data)
        {
            FASR.HomeInstallationsRecord record = new FASR.HomeInstallationsRecord();
            if (data==null)
            {
                record.Crew = new UserLookupRecord[0];
            }
           else
            {
                record.Crew = PrepareInstallerCrewData(data);
            }
           
            //    record.ReturnTripReason = Lift.II.IIUtils.CreateStringValue<StringValue>(data.ReturnTripReason);
            return record;
        }

        private static FASR.TruckDictionaryRecord GetTruckCrewRecord(Generics.Utils.ContentType type, List<Generics.Utils.InstallerWithLessInfo> data)
        {
            FASR.TruckDictionaryRecord record = new FASR.TruckDictionaryRecord();
            if (data == null)
            {
                record.Crew = new UserLookupRecord[0];
            }
            else
            {
                record.Crew = PrepareTruckInstallerData(data);
            }

            //    record.ReturnTripReason = Lift.II.IIUtils.CreateStringValue<StringValue>(data.ReturnTripReason);
            return record;
        }



        private static FASR.HomeInstallationsRecord GetDocumentFileRecord(Generics.Utils.ContentType type, List<Generics.Utils.DocumentFile> data)
        {
            FASR.HomeInstallationsRecord record = new FASR.HomeInstallationsRecord();
            record.FileAttachment = PrepareInstallerFileData(data);
            //    record.ReturnTripReason = Lift.II.IIUtils.CreateStringValue<StringValue>(data.ReturnTripReason);
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

        private static string PrepareTransactionId(Generics.Utils.ImproperTruckInstallationEvent data)
        {
            return string.Format("{0} {1} {2}", "update", data.id, DateTime.Now.Ticks.ToString());
        }

        private static string PrepareTransactionId(Generics.Utils.CalledLog data)
        {
            string returnStr = string.Empty;
            if (data is null)
            {
                returnStr = string.Format("{0} {1} {2}", "update", "", DateTime.Now.Ticks.ToString());
            }
            else
            {
                returnStr = string.Format("{0} {1} {2}", "update", data.DetailRecordId, DateTime.Now.Ticks.ToString());
            }
            return returnStr;
        }

        private static string PrepareTransactionId(Generics.Utils.InstallerWithLessInfo data)
        {
            string returnStr = string.Empty;
            if (data is null)
            {
                returnStr = string.Format("{0} {1} {2}", "update", "", DateTime.Now.Ticks.ToString());
            }
            else
            {
                returnStr = string.Format("{0} {1} {2}", "update", data.DetailedRecordid, DateTime.Now.Ticks.ToString());
            }
            return returnStr;
        }


        private static string PrepareTransactionId(Generics.Utils.DocumentFile data)
        {
            string returnStr = string.Empty;
            if (data is null)
            {
                returnStr = string.Format("{0} {1} {2}", "update", "", DateTime.Now.Ticks.ToString());
            }
            else
            {
                returnStr = string.Format("{0} {1} {2}", "update", data.id, DateTime.Now.Ticks.ToString());
            }
            return returnStr;
        }


        private static string PrepareTransactionId(Generics.Utils.Notes data)
        {
            string returnStr = string.Empty;
            if (data is null)
            {
                returnStr = string.Format("{0} {1} {2}", "update", "", DateTime.Now.Ticks.ToString());
            }
            else
            {
                returnStr = string.Format("{0} {1} {2}", "update", data.DetailRecordId, DateTime.Now.Ticks.ToString());
            }
            return returnStr;
        }



        private static string PrepareTransactionId(Generics.Utils.InstallationDataEvent data)
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

        private static string PrepareTransactionId(Generics.Utils.ImproperRemeasureEvent data)
        {
            return string.Format("{0} {1} {2}", "update", data.id, DateTime.Now.Ticks.ToString());
        }
        private static FASR.HomeInstallations_InstallationDatesRecord[] PrepareInstallationDateList(Generics.Utils.ImproperInstallationEvent data)
        {
            List<FASR.HomeInstallations_InstallationDatesRecord> returnValue = new List<FASR.HomeInstallations_InstallationDatesRecord>();

            DateTime start = Generics.Utils.Date.DateParser.ParseTime(data.start);
            DateTime end = Generics.Utils.Date.DateParser.ParseTime(data.end);
            DateTime end1;
            FASR.HomeInstallations_InstallationDatesRecord newRecord ;
            if ((end - start).TotalDays >= 1)
            {
                end1 = end.AddDays(1);// -- changed on Feb  18,2020
                while ((end1 - start).TotalDays >= 1)
                {
                    //returnValue.Add(new FASR.HomeInstallations_InstallationDatesRecord()
                    //{
                    //    ScheduledDate = new FASR.DateTimeValue() { Value = start },
                    //    EndTime = new FASR.DateTimeValue() { Value = end }

                    //});
                    //  returnValue[0]
                    newRecord = new FASR.HomeInstallations_InstallationDatesRecord();
                    newRecord.AllDayEvent = new FASR.MultiSelection();
                    newRecord.AllDayEvent.Selection = new String[] { data.AllDay.ToString() };
                    newRecord.ScheduledDate = new FASR.DateTimeValue() { Value = start };
                    newRecord.EndTime = new FASR.DateTimeValue() { Value = end };
                    returnValue.Add(newRecord);
                    //returnValue.Add(new FASR.HomeInstallations_InstallationDatesRecord()
                    //{
                    //    ScheduledDate = new FASR.DateTimeValue() { Value = start },
                    // //   AllDayEvent = new FASR.MultiSelection().Selection
                    ////AllDayEvent.sele. = Lift.II.IIUtils.CreateSingleSelectValue<StringValue>(data.AllDay)

                    ////CreateSingleSelectValue<StringValue>(c.FileName),

                    //});
                    start = start.AddDays(1);
                }
            }
            else
            {
                newRecord = new FASR.HomeInstallations_InstallationDatesRecord();
                newRecord.ScheduledDate = new FASR.DateTimeValue() { Value = start };
                newRecord.EndTime = new FASR.DateTimeValue() { Value = end };
                newRecord.AllDayEvent = new FASR.MultiSelection();
                newRecord.AllDayEvent.Selection = new String[] { data.AllDay.ToString() };
                returnValue.Add(newRecord);
            }
            //returnValue.Add(new FASR.HomeInstallations_InstallationDatesRecord()
            //{
            //    //ScheduledDate = new FASR.DateTimeValue() { Value = start },
            //    //EndTime = new FASR.DateTimeValue() { Value = end }
            //    ScheduledDate = new FASR.DateTimeValue() { Value = start },
            //    EndTime = new FASR.DateTimeValue() { Value = end }

            //});
            return returnValue.ToArray();
        }

        private static FASR.HomeInstallations_AssignTruckRecord[] PreparTruckList(Generics.Utils.ImproperTruckInstallationEvent data)
        {
            //List<FASR.HomeInstallations_AssignTruckRecord> returnValue = new List<FASR.HomeInstallations_AssignTruckRecord>();
            //HomeInstallations_AssignTruckRecord newRecord = new HomeInstallations_AssignTruckRecord();
            //newRecord.TruckLookup = data.TruckLookUp;
            //returnValue.Add(newRecord)

            //return returnValue.ToArray();
            List<FASR.HomeInstallations_AssignTruckRecord> returnValue = new List<FASR.HomeInstallations_AssignTruckRecord>();

            if (data == null)
            {
                return null;
            }
            else
            {

                //  calledLogOffDate = Generics.Utils.Date.DateParser.ParseTime(Convert.ToDateTime(c.DateCalled).ToString("yyyy-MM-ddTHH:mm:00.000Z"));
                returnValue.Add(new FASR.HomeInstallations_AssignTruckRecord()
                {
                    //CalledMessage = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>(c.CalledMessage),
                    //Notes3 = Lift.II.IIUtils.CreateStringValue<StringValue>(c.Notes3),
                    //DateCalled = Lift.II.IIUtils.CreateDateTimeValue<DateTimeValue>(calledLogOffDate),
                    // TruckLookup = Lift.II.IIUtils.CreateChoiceValue<Choice>(data.id, data.id),
                    TruckLookup = Lift.II.IIUtils.CreateChoiceValue<Choice>(data.TruckName, data.TruckID),
                    //    TruckLookup = Lift.II.IIUtils.CreateChoiceValue<Choice>(data.id, data.TruckID),
                  //  TruckLookup = Lift.II.IIUtils.CreateChoiceValue<Choice>(data.TruckID, data.TruckID),

                    //FileName = Lift.II.IIUtils.CreateSingleSelectValue<BinaryValue>(c.FileSource.ToString()),
                    //Notes5 = Lift.II.IIUtils.CreateSingleSelectValue<StringValue>(c.FileName),

                });
               
                return returnValue.ToArray();
            }
        }


        private static FASR.UserLookupRecord[] PrepareInstallerCrewData(List<Generics.Utils.InstallerWithLessInfo> data)
        {
            List<FASR.UserLookupRecord> returnValue = new List<FASR.UserLookupRecord>();

            if (data == null)
            {
                return null;
            }
            else
            {
                foreach (Generics.Utils.InstallerWithLessInfo c in data)
                {
                    returnValue.Add(new FASR.UserLookupRecord()
                    {
                        Account = c.Account,
                    });
                }
                return returnValue.ToArray();
            }
        }

        private static FASR.UserLookupRecord[] PrepareTruckInstallerData(List<Generics.Utils.InstallerWithLessInfo> data)
        {
            List<FASR.UserLookupRecord> returnValue = new List<FASR.UserLookupRecord>();

            if (data == null)
            {
                return null;
            }
            else
            {
                foreach (Generics.Utils.InstallerWithLessInfo c in data)
                {
                    returnValue.Add(new FASR.UserLookupRecord()
                    {
                        Account = c.Account,
                    });
                }
                return returnValue.ToArray();
            }
        }


        private static FASR.HomeInstallations_FileAttachmentRecord[] PrepareInstallerFileData(List<Generics.Utils.DocumentFile> data)
        {
            List<FASR.HomeInstallations_FileAttachmentRecord> returnValue = new List<FASR.HomeInstallations_FileAttachmentRecord>();

            if (data == null)
            {
                return null;
            }
            else
            {
                foreach (Generics.Utils.DocumentFile c in data)
                {
                    //  calledLogOffDate = Generics.Utils.Date.DateParser.ParseTime(Convert.ToDateTime(c.DateCalled).ToString("yyyy-MM-ddTHH:mm:00.000Z"));
                    returnValue.Add(new FASR.HomeInstallations_FileAttachmentRecord()
                    {
                        //CalledMessage = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>(c.CalledMessage),
                        //Notes3 = Lift.II.IIUtils.CreateStringValue<StringValue>(c.Notes3),
                        //DateCalled = Lift.II.IIUtils.CreateDateTimeValue<DateTimeValue>(calledLogOffDate),
                        FileName = Lift.II.IIUtils.CreateSingleSelectValue<BinaryValue>(c.FileSource.ToString()),
                        Notes5 = Lift.II.IIUtils.CreateSingleSelectValue<StringValue>(c.FileName),

                    });
                }
                return returnValue.ToArray();
            }
        }

        private static FASR.HomeInstallations_CallLogRecord[] PrepareInstallationCallLogList(List<Generics.Utils.CalledLog> data)
        {
            List<FASR.HomeInstallations_CallLogRecord> returnValue = new List<FASR.HomeInstallations_CallLogRecord>();
            DateTime calledLogOffDate;
            if (data == null)
            {
                return null;
            }
            else
            {
                foreach (Generics.Utils.CalledLog c in data)
                {
                    calledLogOffDate = Generics.Utils.Date.DateParser.ParseTime(Convert.ToDateTime(c.DateCalled).ToString("yyyy-MM-ddTHH:mm:00.000Z"));
                    returnValue.Add(new FASR.HomeInstallations_CallLogRecord()
                    {
                        CalledMessage = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>(c.CalledMessage),
                        Notes3 = Lift.II.IIUtils.CreateStringValue<StringValue>(c.Notes3),
                        DateCalled = Lift.II.IIUtils.CreateDateTimeValue<DateTimeValue>(calledLogOffDate),

                    });
                }
                return returnValue.ToArray();
            }
        }

        private static FASR.HomeInstallations_GeneralNotesListRecord[] PrepareInstallationNotesList(List<Generics.Utils.Notes> data)
        {
            List<FASR.HomeInstallations_GeneralNotesListRecord> returnValue = new List<FASR.HomeInstallations_GeneralNotesListRecord>();
            DateTime notesDate;
            if (data == null)
            {
                return null;
            }
            else
            {
                foreach (Generics.Utils.Notes c in data)
                {
                    notesDate = Generics.Utils.Date.DateParser.ParseTime(Convert.ToDateTime(c.NotesDate).ToString("yyyy-MM-ddTHH:mm:00.000Z"));
                    returnValue.Add(new FASR.HomeInstallations_GeneralNotesListRecord()
                    {
                        Category_1 = Lift.II.IIUtils.CreateSingleSelectValue<SingleSelection>(c.Category),
                        GeneralNotes = Lift.II.IIUtils.CreateStringValue<StringValue>(c.GeneralNotes),
                        GerneralNotesDate = Lift.II.IIUtils.CreateDateTimeValue<DateTimeValue>(notesDate),

                    });
                }
                return returnValue.ToArray();
            }





        }


        //private static FASR.HomeInstallationsRecord PrepareRemeasureDateList(Generics.Utils.ImproperRemeasureEvent data)
        //{
        //   FASR.HomeInstallationsRecord returnValue = new FASR.HomeInstallationsRecord();
        //    returnValue.RemeasureDate = new FASR.DateTimeValue() { Value = Generics.Utils.Date.DateParser.ParseTime(data.start) };

        //    return returnValue;
        //}

        private static FASR.HomeInstallations_ReturnTripRecord[] PrepareReturnedInstallationDateList(Generics.Utils.ImproperInstallationEvent data)
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


        private static FASR.HomeInstallations_InstallationDatesRecord[] PrepareInstallationDateList(Generics.Utils.InstallationDataEvent data)
        {
            List<FASR.HomeInstallations_InstallationDatesRecord> returnValue = new List<FASR.HomeInstallations_InstallationDatesRecord>();

            DateTime start = Generics.Utils.Date.DateParser.ParseTime(data.start);
            DateTime end = Generics.Utils.Date.DateParser.ParseTime(data.end);
            DateTime end1;
            FASR.HomeInstallations_InstallationDatesRecord newRecord;
            if ((end - start).TotalDays >= 1)
            {
                end1 = end.AddDays(1);
                while ((end1 - start).TotalDays >= 1)
                {
                    //returnValue.Add(new FASR.HomeInstallations_InstallationDatesRecord()
                    //{
                    //    ScheduledDate = new FASR.DateTimeValue() { Value = start }
                    //});
                    //start = start.AddDays(1);
                    //returnValue.Add(new FASR.HomeInstallations_InstallationDatesRecord()
                    //{
                    //    ScheduledDate = new FASR.DateTimeValue() { Value = start },
                    //    EndTime = new FASR.DateTimeValue() { Value = end }

                    //});
                    //  returnValue[0]
                    newRecord = new FASR.HomeInstallations_InstallationDatesRecord();
                    newRecord.AllDayEvent = new FASR.MultiSelection();
                    newRecord.AllDayEvent.Selection = new String[] { data.AllDay.ToString() };
                    newRecord.ScheduledDate = new FASR.DateTimeValue() { Value = start };
                    newRecord.EndTime = new FASR.DateTimeValue() { Value = end };
                    returnValue.Add(newRecord);
                    //returnValue.Add(new FASR.HomeInstallations_InstallationDatesRecord()
                    //{
                    //    ScheduledDate = new FASR.DateTimeValue() { Value = start },
                    // //   AllDayEvent = new FASR.MultiSelection().Selection
                    ////AllDayEvent.sele. = Lift.II.IIUtils.CreateSingleSelectValue<StringValue>(data.AllDay)

                    ////CreateSingleSelectValue<StringValue>(c.FileName),

                    //});
                    start = start.AddDays(1);
                }
            }
            else
            {
                newRecord = new FASR.HomeInstallations_InstallationDatesRecord();
                newRecord.ScheduledDate = new FASR.DateTimeValue() { Value = start };
                newRecord.EndTime = new FASR.DateTimeValue() { Value = end };
                newRecord.AllDayEvent = new FASR.MultiSelection();
                newRecord.AllDayEvent.Selection = new String[] { data.AllDay.ToString() };
                returnValue.Add(newRecord);
            }
            //returnValue.Add(new FASR.HomeInstallations_InstallationDatesRecord()
            //{
            //    //ScheduledDate = new FASR.DateTimeValue() { Value = start },
            //    //EndTime = new FASR.DateTimeValue() { Value = end }
            //    ScheduledDate = new FASR.DateTimeValue() { Value = start },
            //    EndTime = new FASR.DateTimeValue() { Value = end }

            //});
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

        bool Generics.RecordUpdate.IUpdateHelper.UpdateInstalltionCrew(Generics.Utils.ContentType type, List<Generics.Utils.InstallerWithLessInfo> data,string parRecordID)
        {
            //FASR.PlantProduction_UpdateWindowMakerData_Call call = new FASR.PlantProduction_UpdateWindowMakerData_Call()
            // HomeInstallations_EditSold_Call
            if (data.Count > 0)
            {
                FASR.HomeInstallations_UpdateCrew_Call call = new FASR.HomeInstallations_UpdateCrew_Call()
                {
                    OnBehalfOf = Owner,
                    RecordID = data[0].id,
                    Record = GetCrewRecord(type, data)
                };
                return _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data[0])).ReturnValue;
            }
            else
            {
                //CalledLog 
                InstallerWithLessInfo data1 = null;
                FASR.HomeInstallations_UpdateCrew_Call call = new FASR.HomeInstallations_UpdateCrew_Call()
                {
                    OnBehalfOf = Owner,
                    RecordID = parRecordID,
                
                    Record = GetCrewRecord(type, null)
                };
                return _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data1)).ReturnValue;
            }
                
        }

        bool Generics.RecordUpdate.IUpdateHelper.UpdateTruckInstalltionCrew(Generics.Utils.ContentType type, List<Generics.Utils.InstallerWithLessInfo> data, string parRecordID)
        {
            //FASR.PlantProduction_UpdateWindowMakerData_Call call = new FASR.PlantProduction_UpdateWindowMakerData_Call()
            // HomeInstallations_EditSold_Call
            if (data.Count > 0)
            {
                FASR.TruckDictionary_EditRecord_Call call = new TruckDictionary_EditRecord_Call()
                {
                    OnBehalfOf = Owner,
                    RecordID = data[0].id,
                    Record = GetTruckCrewRecord(type, data)
                };
                return _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data[0])).ReturnValue;
            }
            else
            {
                //CalledLog 
                InstallerWithLessInfo data1 = null;
                FASR.TruckDictionary_EditRecord_Call call = new FASR.TruckDictionary_EditRecord_Call()
                {
                    OnBehalfOf = Owner,
                    RecordID = parRecordID,

                    Record = GetTruckCrewRecord(type, null)
                };
                return _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data1)).ReturnValue;
            }

        }

        bool IUpdateHelper.UpdateRecord(ContentType type, List<DocumentFile> data)
        {
            FASR.HomeInstallations_EditSold_Call call = new FASR.HomeInstallations_EditSold_Call()
            {
                OnBehalfOf = Owner,
                RecordID = data[0].id,
                Record = GetDocumentFileRecord(type, data)
            };
            return _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data[0])).ReturnValue;
        }



        public bool UpdateRecord(ContentType type, InstallationDataEvent data)
        {
            FASR.HomeInstallations_EditSold_Call call = new FASR.HomeInstallations_EditSold_Call()
            {
                OnBehalfOf = Owner,
                RecordID = data.id,
                Record = GetRecord(type, data)
            };
            return _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data)).ReturnValue;
        }

        //bool IUpdateHelper.UpdateRecord(ContentType type, InstallationDataEvent data)
        //{
        //    throw new NotImplementedException();
        //}

        bool IUpdateHelper.UpdateRecord(ContentType type, List<CalledLog> data, string parRecordID)
        {
            bool returnValue = true;

            if (data.Count > 0)
            {
                FASR.HomeInstallations_EditSold_Call call = new FASR.HomeInstallations_EditSold_Call()
                {
                    OnBehalfOf = Owner,
                    RecordID = data[0].id,
                    Record = GetCalledLogRecord(type, data)
                };
                returnValue = _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data[0])).ReturnValue;
            }
            else
            {
                CalledLog data1 = null;
                FASR.HomeInstallations_EditSold_Call call = new FASR.HomeInstallations_EditSold_Call()
                {
                    OnBehalfOf = Owner,
                    RecordID = parRecordID,
                    Record = GetCalledLogRecord(type, null)
                };
                returnValue = _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data1)).ReturnValue;
            }

            return returnValue;
        }




        bool IUpdateHelper.UpdateRecord(ContentType type, List<Notes> data, string parRecordID)
        {
            bool returnValue = true;

            if (data.Count > 0)
            {
                FASR.HomeInstallations_EditSold_Call call = new FASR.HomeInstallations_EditSold_Call()
                {
                    OnBehalfOf = Owner,
                    RecordID = data[0].id,
                    Record = GetNotesRecord(type, data)
                };
                returnValue = _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data[0])).ReturnValue;
            }
            else
            {
                Notes data1 = null;
                FASR.HomeInstallations_EditSold_Call call = new FASR.HomeInstallations_EditSold_Call()
                {
                    OnBehalfOf = Owner,
                    RecordID = parRecordID,
                    Record = GetNotesRecord(type, null)
                };
                returnValue = _helper.Send(new FASR.OperationCall[] { call }, PrepareTransactionId(data1)).ReturnValue;
            }

            return returnValue;
        }

    }
}
