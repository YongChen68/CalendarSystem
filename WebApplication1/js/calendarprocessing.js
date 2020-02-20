var displayType = "Windows";
var GlobalParams = [];
var eventArray = [];
var debug = true;
var renderingComplete = false;
var eventid;
var eventWO = []; 
var newWOArray = []; 
var CalledLogArray = []; 
var eventReturnedJobDate1;
var eventDiff=[];
var hasSubStrade;
var JobStaffEvent;
var updateStatus = 0;
var WO;
var fileByteArray = [];
var recordIDArray = [];

var dataArray = new Array();

var truckScheduledArray = new Array();

var truckRecordID;
var AssignedTruckWithWO;
var truckName;


//window.onload = function () {
//    var fileInput = document.getElementById('fileUpload');
//  //  var fileDisplayArea = document.getElementById('fileDisplayArea');
    
//    fileInput.addEventListener('change', function (e) {
//      //  var file = fileInput.files[0];  --get(0).files
//        var file = fileInput.get(0).files[0]; 
//        var textType = /image.*/;

//        if (file.type.match(textType)) {
//            var reader = new FileReader();

//            reader.onload = function (e) {
//               // fileDisplayArea.innerText = reader.result;
//                var theBytes = e.target.result; //.split('base64,')[1]; // use with uploadFile2
//                fileByteArray.push(theBytes);
//                document.getElementById('file').innerText = '';
//                for (var i = 0; i < fileByteArray.length; i++) {
//                    document.getElementById('file').innerText += fileByteArray[i];
//                }
//            }
//            reader.readAsArrayBuffer(file);
//        } else {
//            fileDisplayArea.innerText = "File not supported!";
//        }
//    });
//$("#btnUploadDocuments").addEventListener("click", function () {
//    UploadDocuments();
//}); 



//window.onload = function () {
//    var fileInput = document.getElementById('fileUpload');
//    fileInput.addEventListener("change", function () {
//        processFile(fileInput);
//    });
//}

var is_weekend = function (date1) {
    var dt = new Date(date1);

    if (dt.getDay() == 6 || dt.getDay() == 0) {
        return "weekend";
    }
}


var is_saturday = function (date1) {
    var dt = new Date(date1);

    if (dt.getDay() == 6) {
        return "saturday";
    }
}

var is_sunday = function (date1) {
    var dt = new Date(date1);

    if (dt.getDay() == 0) {
        return "sunday";
    }
}


Date.prototype.Equals = function (pDate) {
    var retValue = (this.getUTCFullYear() === pDate.getUTCFullYear() &&
        this.getUTCMonth() === pDate.getUTCMonth() &&
        this.getUTCDate() === pDate.getUTCDate());
    return retValue;
}

function addEvent(event) {
    var id = PageMethods.GetEvent(event);

    if (id != -1)
        event.id = id;
    else
        window.alert('Failed to record event data');
}
function eventUpdate(event, dayDelta, minuteDelta, allDay, revertFunc) {
    sendUpdateToServer(event);
    $(this).remove();
}

function UpdateEventSchedule() {
    var i = eventid;
    var scheduledStartDate, scheduledEndDate;
    scheduledStartDate = $("#InstallScheduledStartDate").val();
    scheduledEndDate = $("#InstallScheduledEndDate").val();
  
    var isSaturdayChecked = document.getElementsByName('saturday')[0].checked;
    var isSundayChecked = document.getElementsByName('sunday')[0].checked;
    var xDate;
    if ((scheduledStartDate == scheduledEndDate) && (isSaturdayChecked == false) && (isSundayChecked == false)) 
    {
        xDate = is_weekend(scheduledStartDate);
        if (xDate == "weekend") {
            alert("This event has Saturday & Sunday disabled!");
            return;
        }
    }

    if ((scheduledStartDate == scheduledEndDate) && (isSaturdayChecked == false) ) {
        xDate = is_saturday(scheduledStartDate);
        if (xDate == "saturday") {
            alert("This event has Saturday disabled!");
            return;
        }
    }

    if ((scheduledStartDate == scheduledEndDate) && (isSundayChecked == false)) {
        xDate = is_sunday(scheduledStartDate);
        if (xDate == "sunday") {
            alert("This event has Sunday disabled!");
            return;
        }
    }


    $.ajax({
        url: 'data.svc/UpdateInstallationSchedule?id=' + eventid + '&ScheduledStartDate=' + scheduledStartDate + '&scheduledEndDate=' + scheduledEndDate,
        type: "POST",
        success: function (data) {
            if (debug) console.log("events.success", "data.UpdateInstallationSchedule:");
            $("#InstallScheduledStartDate").val('');
            $("#InstallScheduledEndDate").val('');

            //$("#eventContent .close").click();
            //$('#calendar').fullCalendar('refetchEvents');
            //$('#calendar').fullCalendar('rerenderEvents');

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}
function sendUpdateToServer(event) {
    var eventToUpdate = {
        id: event.id,
        start: event.start,
        end: event.end,
        title: event.title,
        description: event.description,
        doors: event.doors,
        Doors: event.Doors,
        Windows: event.Windows,
        Amount: event.TotalSalesAmount,
        totalWindows: event.TotalWindows,
        totalDoors: event.TotalDoors,
        CurrentStateName: event.CurrentStateName,
        EndTime: event.EndRemeasureDate
    };
    if (event.end === null) {
        eventToUpdate.end = eventToUpdate.start;
    }
    else {
        if (event.end._i == event.start._i) {
            eventToUpdate.end = new Date(event.end - 24 * 60 * 60000);
        } else {
            eventToUpdate.end = event.end;
        }
        
    }

    eventToUpdate.id = event.id;

    var endDate;
    if (!event.allDay) {
        endDate = new Date(eventToUpdate.end + 60 * 60000);
        endDate = endDate.toJSON();
    }
    else {
        endDate = eventToUpdate.end.toJSON();
    }

    eventToUpdate.start = eventToUpdate.start.toJSON();
 
    eventToUpdate.end = eventToUpdate.end.toJSON(); //endDate;
    eventToUpdate.allDay = event.allDay;
    eventToUpdate.eventDurationEditable = true;
   
    if ((displayType == "Installation") && (event.ReturnedJob != 1)) {
      //  eventWODict = [];

        //for (j = 0; j < eventWODict.length; j++) {
        //    if (eventWODict[j].WorkOrderNumber == event.WorkOrderNumber) {
        //        eventWODict.splice(j);
        //    }
        //}

        eventWODict = eventWODict.filter(el => el.WorkOrderNumber !== event.WorkOrderNumber);
        var date1, date2,day;
        date1 = new Date(eventToUpdate.end);
        date2 = new Date(eventToUpdate.start);
        day = date1.getDay() - date2.getDay();
        var obj;
        var k =1;
        var xDate;
       
        if (date1 == date2) {

        }
        else {
            day = day + 1;
            for (i = 0; i < day ; i++) {
                obj = new Object();
                obj.Doors = parseFloat(eventToUpdate.totalDoors / day).toFixed(2);
                obj.Windows = parseFloat(eventToUpdate.totalWindows / day).toFixed(2);
                obj.SalesAmmount = eventToUpdate.Amount / day;
                obj.ScheduledDate = new Date(event.start + k * 24 * 60 * 60000);
                k++;
                if (day == 1) {
                    xDate = is_weekend(obj.ScheduledDate);
                    if ((event.Sunday == "No") && (event.Saturday == "No") && (xDate == "weekend")) {
                         alert("This event has Saturday/Sunday disabled!");
                         revertFunc();
                    }
                    else if ((event.Saturday == "No") && (event.Sunday == "Yes")) {
                        xDate = is_saturday(obj.ScheduledDate);
                        if (xDate == "saturday") {
                            alert("This event has Saturday disabled!");
                            revertFunc();
                        }
                    }
                    else if ((event.Saturday == "Yes") && (event.Sunday == "No")) {
                        xDate = is_sunday(obj.ScheduledDate);
                        if (xDate == "sunday") {
                            alert("This event has Sunday disabled!");
                            revertFunc();
                        }
                    }
                }
                //eventWODict[j]["ScheduledDate"]
                eventWODict.push(obj);
            }
        }


        PageMethods.UpdateInstallationEventTime(displayType, eventToUpdate);
        
    }
    else if (displayType == "Remeasure") {
        UpdateRemeasureEvents(eventToUpdate);
    }
    else {
        eventDiff = [];
        updateStatus = 1;
        eventToUpdate.end = new Date(GetDatefromMoment(event.end - 24 * 60 * 60000)).toJSON();
        PageMethods.UpdateEventTime(displayType, eventToUpdate);
        $('#calendar').fullCalendar('refetchEvents');
        $('#calendar').fullCalendar('rerenderEvents');
    }

}



function GetDatefromMoment(val) {
    return moment(val).toDate();
}

function getBlankTotal() {
    var view = $('#calendar').fullCalendar('getView');
    if (view.start !== undefined) {
        var start = GetDatefromMoment(view.start);
        var firstDay = new Date(start);
        
        var end = GetDatefromMoment(view.end);
        if (debug) console.log("getBlankTotal", "view.start:", view.start.format(), "start:", start, "view.end:", view.end.format(), "end:", end);

        var retValue = [];
        if (end < start) { var tmp = start; start = end; end = tmp; }
        else if (start.Equals(end)) { start.setDate(start.getUTCDate()-1  ); }
        var numDays = (end - start) / 1000 / 60 / 60 / 24;
        for (var i = 0; i < numDays; i++) {
            if (debug) console.log("getBlankTotal", "adding date", start);
            var newDate = new Date(start);
            retValue.push(GetBlankDayData(newDate));
            start = new Date(Date.UTC(newDate.getUTCFullYear(), newDate.getUTCMonth(), newDate.getUTCDate()+1 ));
        }
        return retValue;
    } else
        return [];
}

function GetBlankDayData(day) {
   // var dayName = ["sun", "mon", "tue", "wed", "thu", "fri", "sat"][day.getUTCDay()+6];
    var dayName = ["sun", "mon", "tue", "wed", "thu", "fri", "sat"][day.getUTCDay() ];
    //var dayName = ["sun", "mon", "tue", "wed", "thu", "fri", "sat"];
  //  var dayName = ["sat", "sun", "mon", "tue", "wed", "thu", "fri"][day.getUTCDay()];
    if (displayType == "Installation") {
        //return { day: dayName, date: new Date(day.valueOf()), doors: 0, windows: 0, SalesAmmount: 0,WOCount:0 };
        var installationDay = new Date(Date.UTC(day.getUTCFullYear(), day.getUTCMonth(), day.getUTCDate()+1 ));
     //  return { day: dayName, date: new Date(day.valueOf()), doors: 0, windows: 0, SalesAmmount: 0, WOCount: 0 };
        return {
            day: dayName, date: installationDay, doors: 0, windows: 0, ExtDoors:0,
            SalesAmmount: 0, TotalAsbestos: 0, TotalWoodDropOff: 0, TotalHighRisk: 0, TotalLeadPaint: 0, WOCount: 0, installationwindowLBRMIN: 0, TotalInstallationLBRMin: 0, InstallationDoorLBRMin: 0,
            InstallationPatioDoorLBRMin: 0, subinstallationwindowLBRMIN: 0, subInstallationPatioDoorLBRMin: 0, subExtDoorLBRMIN: 0, subTotalInstallationLBRMin: 0,
            SidingLBRBudget: 0, SidingLBRMin: 0, SidingSQF: 0, MinAvailable: 0, SalesTarget: 0, HazardousBudgetedLBR:0
        };
    }
    else if (displayType == "Remeasure")
    {
        var remeasureDay = new Date(Date.UTC(day.getUTCFullYear(), day.getUTCMonth(), day.getUTCDate() + 1));
        //  return { day: dayName, date: new Date(day.valueOf()), doors: 0, windows: 0, SalesAmmount: 0, WOCount: 0 };
        return {
            day: dayName, date: remeasureDay, doors: 0, windows: 0, ExtDoors: 0,
            SalesAmmount: 0, WOCount: 0, installationwindowLBRMIN: 0, TotalInstallationLBRMin: 0, InstallationDoorLBRMin: 0,
            InstallationPatioDoorLBRMin: 0, subinstallationwindowLBRMIN: 0, subInstallationPatioDoorLBRMin: 0, subExtDoorLBRMIN: 0, subTotalInstallationLBRMin: 0,
            SidingLBRBudget: 0, SidingLBRMin: 0, SidingSQF: 0
        };
    }
    else {
      //  return { day: dayName, date: new Date(day.valueOf()), doors: 0, windows: 0, boxes: 0, glass: 0, value: 0, min: 0, max: 0, Available_Time: 0, rush: 0, float: 0, TotalBoxQty: 0, TotalGlassQty: 0, TotalPrice: 0, TotalLBRMin: 0, F6CA: 0, F27DS: 0, F27TS: 0, F27TT: 0, F29CA: 0, F29CM: 0, F52PD: 0, F68CA: 0, F68SL: 0, F68VS: 0, Transom: 0, Sidelite: 0, SingleDoor: 0, DoubleDoor: 0, Simple: 0, Complex: 0, Over_Size: 0, Arches: 0, Rakes: 0, Customs: 0, };
        return { day: dayName, date: new Date(day.valueOf()), doors: 0, windows: 0, boxes: 0, glass: 0, value: 0, min: 0, max: 0, Available_Time: 0, rush: 0, float: 0, TotalBoxQty: 0, TotalGlassQty: 0, TotalPrice: 0, TotalLBRMin: 0, F6CA: 0, F27DS: 0, F27TS: 0, F27TT: 0, F29CA: 0, F29CM: 0, F52PD: 0, F68CA: 0, F68SL: 0, F68SLMin: 0, F68VS: 0, F68VSMin: 0, F29CMMin: 0, F29CAMin: 0, F26CAMin: 0, F27DSMin: 0,Transom: 0, Sidelite: 0, SingleDoor: 0, DoubleDoor: 0 };
    }


}

function ToWDString(event) {
    return "Glass Ordered\r\n" +
        event.CardinalOrderedDate + "\r\n" +

        'Date Completed\r\n' +
        event.CompleteDate + "\r\n" +
        'Product Mix\r\n' +
        '26CA: ' + event.F6CA + '\r\n' +

        '27DS: ' + event.F27DS + '\r\n' +
     //   '27TS: ' + event.F27TS + '\r\n' +
    //    '27TT: ' + event.F27TT + '\r\n' +

        '29CA: ' + event.F29CA + '\r\n' +
        '29CM: ' + event.F29CM + '\r\n' +

        '68CA: ' + event.F68CA + '\r\n' +
        '68SL: ' + event.F68SL + '\r\n' +
        '68VS: ' + event.F68VS;
}

function ToInstallationEventString(event) {
    return "Customer:\r\n" +
        event.LastName + "\r\n" +

        'Home Phone Number:\r\n' +
        event.HomePhoneNumber + "\r\n" +
        'Work Phone Number:\r\n' +
        event.WorkPhoneNumber + '\r\n' +

        'Cell Phone Number:\r\n' +
        event.CellPhone + "\r\n\r\n" +

        'Sales Amount:\r\n $' +
      //  event.SalesAmmount + "\r\n" +
        event.TotalSalesAmount + "\r\n" +

        'Address:\r\n' +
        event.StreetAddress + "\r\n" +
        event.City + "\r\n\r\n" +


        event.CurrentStateName + "\r\n" +

        "Senior Installer: \r\n" + (event.SeniorInstaller != null && event.SeniorInstaller.trim().Length > 0 ? event.SeniorInstaller : "Unspecified") +
        "\r\n Crew: \r\n" + (event.CrewNames != null && event.CrewNames.trim().Length > 0 ? event.CrewNames : "Un assigned");

}

function ToPDString(event) {
    return '52PD: ' + event.F52PD;
}

function ToDString(event) {
    return 'Doors: ' + event.doors;
}

var totals = getBlankTotal();

function LoadBufferedJobs() {
    var branches = [];
    $.each($("input[name='branch']:checked"), function () {
        branches.push($(this).val());
    });
    console.log("checked branches: ", branches.join(","));
    var jobTypes = [];
    $.each($("input[name='jobType']:checked"), function () {
        jobTypes.push($(this).val());
    });
    console.log("checked jobTypes: ", jobTypes.join(","));
    var shippingType = [];
    $.each($("input[name='ShippingType']:checked"), function () {
        shippingType.push($(this).val());
    });
    console.log("checked shippingType: ", shippingType.join(","));
    $.getJSON("data.svc/GetBufferJobs", { type: displayType, branch: branches.join(","), jobType: jobTypes.join(","), shippingType: shippingType.join(",") }, function (data) {
        $.each(data.GetBufferJobsResult, function (key, val) {
            val.editable = (readonly == "True") ? false : true;
            AddBufferEvent(key, val);

        });
        /* initialize the external events
        -----------------------------------------------------------------*/
    });



}


function LoadInstallationBufferedJobs() {
    var branches = [];
    $.each($("input[name='branch']:checked"), function () {
        branches.push($(this).val());
    });
    console.log("checked branches: ", branches.join(","));
    var eventBufferWODict = [];
    $.ajax({
        url: 'data.svc/GetInstallationBufferJobs',
        dataType: 'json',
        data: {  branch: branches.join(",") },
        success: function (data) {
            if (debug) console.log("events.success", "data.GetInstallationBufferJobsResult:", data.GetInstallationBufferJobsResult === undefined ? "NULL" : data.GetInstallationBufferJobsResult.length);
            var events = [];
            newWOArray = [];

            var found = false;
            var con;
            $.each(data.GetInstallationBufferJobsResult, function (pos, item) {
               // item.editable = true;
                AddInstallationBufferEvent(pos, item);
                newWOArray.push(item);
            });
           
        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });


   
}

function NewWOPopup(id) {
   // alert(val);
    var index = -1;
    for (var i = 0; i < newWOArray.length; i++) {
        if (newWOArray[i].id === id) {
            index = i;
            break;
        }
    }

    //var elementary = document.getElementById(id);
  
    //$(".fc-content").attr('href', 'javascript:void(0);');
    //$(".fc-content").attr('data-toggle', "modal");
    //$(".fc-content").attr('data-target', "#eventContent");
    //$(".fc-content").attr('href', "/details");
    $("#btnSave").attr("disabled", true); 
    $("#btnSaveLog").attr("disabled", true); 
    $("#btnSaveNotes").attr("disabled", true); 
    $("#FirstName").html(newWOArray[index].FirstName);
    $("#LastName").html(newWOArray[index].LastName);
    $("#postalCode").html(newWOArray[index].PostCode);
    $("#workOrder").html(newWOArray[index].WorkOrderNumber);
    $("#WorkOrderTitle").html(newWOArray[index].WorkOrderNumber);
    $("#homePhone").html(newWOArray[index].HomePhoneNumber);


    $("#workPhone").html(newWOArray[index].WorkPhoneNumber);
    $("#cellPhone").html(newWOArray[index].CellPhone);

    $("#email").html(newWOArray[index].Email);
    $("#salesRep").html(newWOArray[index].SalesRep);

    if (newWOArray[index].HomeDepotJob == "Yes") {
        $("#WorkOrderTitleHeader").attr("style", "background-color: #fa6304");
        $("#homeDepotJob").html("Yes");
    }
    else {
        $("#homeDepotJob").html("No");
        $("#WorkOrderTitleHeader").attr("style", "background-color: #9FB6CD");
    }

    $("#ageOfHome").html(newWOArray[index].AgeOfHome);

    $("#City").html(newWOArray[index].City);

    $("#TotalWindows1").html(newWOArray[index].TotalWindows);
    $("#TotalDoors1").html(newWOArray[index].TotalDoors);
    $("#TotalDoors2").html(newWOArray[index].TotalExtDoors);

    if (newWOArray[index].TotalAsbestos == 1) {
        // 
        $("#Asbestos-JobsYes").prop("checked", true);
        $("#Asbestos-JobsNo").prop("checked", false);
    }
    else {
        $("#Asbestos-JobsNo").prop("checked", true);
        $("#Asbestos-JobsYes").prop("checked", false);
    }

    if (newWOArray[index].LeadPaint == 'Yes') {
        // 
        $("#Lead-PaintYes").prop("checked", true);
        $("#Lead-PaintNo").prop("checked", false);
    }
    else {
        $("#Lead-PaintNo").prop("checked", true);
        $("#Lead-PaintYes").prop("checked", false);
    }

    $("#wooddropTime").val('');
    $("#wooddropdate").val('');

    if (newWOArray[index].TotalWoodDropOff == 1) {
        // 
        $("#Wood-DropOff-JobsYes").prop("checked", true);
        $("#Wood-DropOff-JobsNo").prop("checked", false);
        $("#divWoodDropOffDate").show();
        $("#wooddropdate").val(newWOArray[index].StrWoodDropOffDate.slice(0, -10));
        $("#wooddropTime").val(newWOArray[index].StrWoodDropOffTime);
    }
    else {
        $("#Wood-DropOff-JobsNo").prop("checked", true);
        $("#Wood-DropOff-JobsYes").prop("checked", false);
        $("#divWoodDropOffDate").hide();

    }

    if (newWOArray[index].TotalHighRisk == 1) {
        // 
        $("#HighRisk-JobsYes").prop("checked", true);
        $("HighRisk-JobsNo").prop("checked", false);
    }
    else {
        $("#HighRisk-JobsNo").prop("checked", true);
        $("#HighRisk-JobsYes").prop("checked", false);
    }
    $("#NumOfInstallers").val(newWOArray[index].EstInstallerCnt);

    $("#Address").html(newWOArray[index].StreetAddress);
    $("#DaysOfWork").html(newWOArray[index].detailrecordCount);
    $("#SalesAmmount").html(newWOArray[index].TotalSalesAmount.formatMoney(2, "$", ",", "."));
    $("#DailySalesAmmount").html(newWOArray[index].SalesAmmount.formatMoney(2, "$", ",", "."));
    $("#SeniorInstaller").html(newWOArray[index].SeniorInstaller != null && newWOArray[index].SeniorInstaller.trim().length > 0 ? newWOArray[index].SeniorInstaller : "Unspecified");
    $("#CrewNames").html(newWOArray[index].CrewNames != null && newWOArray[index].CrewNames.trim().length > 0 ? newWOArray[index].CrewNames : "Un assigned");

    codeAddress();
    //$("#ReturnedJob").hide();
    $("#from_date").val('');
    $("#end_date").val('');
    $("#returnJobReasonID").val('');


    //if (newWOArray[index].ReturnedJob == 1) {
    //    // $("#ReturnedJob").show(); 
    //    $("#from_date").val(new Date(GetDatefromMoment(newWOArray[index].start + 24 * 60 * 60000)).toLocaleDateString('en-US'));
    //    $("#end_date").val(new Date(GetDatefromMoment(newWOArray[index].end + 24 * 60 * 60000)).toLocaleDateString('en-US'));
    //    if (newWOArray[index].end == null) {
    //        $("#end_date").val(new Date(GetDatefromMoment(newWOArray[index].start + 24 * 60 * 60000)).toLocaleDateString('en-US'));
    //    }


    //    $("#InstallScheduledStartDate").prop("disabled", true);
    //    $("#InstallScheduledEndDate").prop("disabled", true);
    //    $("#InstallScheduledStartDate").val(new Date(GetDatefromMoment(newWOArray[index].StartScheduleDate + 24 * 60 * 60000)).toLocaleDateString('en-US'));
    //    $("#InstallScheduledEndDate").val(new Date(GetDatefromMoment(newWOArray[index].EndScheduleDate + 24 * 60 * 60000)).toLocaleDateString('en-US'));


    //    $("#NumOfInstallers").prop("disabled", true);

    //    $("#Asbestos-JobsYes").prop("disabled", true);
    //    $("#Asbestos-JobsNo").prop("disabled", true);

    //    $("#Lead-PaintYes").prop("disabled", true);
    //    $("#Lead-PaintNo").prop("disabled", true);

    //    $("#Wood-DropOff-JobsYes").prop("disabled", true);
    //    $("#Wood-DropOff-JobsNo").prop("disabled", true);

    //    $("#HighRisk-JobsYes").prop("disabled", true);
    //    $("#HighRisk-JobsNo").prop("disabled", true);

    //    document.getElementsByName('saturday')[0].disabled = true;
    //    document.getElementsByName('sunday')[0].disabled = true;

    //    $("#from_date").prop("disabled", false);
    //    $("#end_date").prop("disabled", false);
    //    //etInstallationDateByWOForReturnedJob()

    //    // $("#InstallScheduledStartDate").val(new Date(GetDatefromMoment(newWOArray[index].start + 24 * 60 * 60000)).toLocaleDateString('en-US'));
    //    //  $("#InstallScheduledEndDate").val(new Date(GetDatefromMoment(newWOArray[index].end + 24 * 60 * 60000)).toLocaleDateString('en-US'));

    //}
    //else {

    //    $("#InstallScheduledStartDate").val(new Date(GetDatefromMoment(newWOArray[index].start + 24 * 60 * 60000)).toLocaleDateString('en-US'));

    //    if (newWOArray[index].end == null) {
    //        $("#InstallScheduledEndDate").val(new Date(GetDatefromMoment(newWOArray[index].start + 24 * 60 * 60000)).toLocaleDateString('en-US'));
    //    }
    //    else {
    //        $("#InstallScheduledEndDate").val(new Date(GetDatefromMoment(newWOArray[index].end + 24 * 60 * 60000)).toLocaleDateString('en-US'));
    //    }
    //    $("#InstallScheduledStartDate").prop("disabled", false);
    //    $("#InstallScheduledEndDate").prop("disabled", false);

    //    if (newWOArray[index].StartScheduleDate != null) {
    //        $("#from_date").prop("disabled", true);
    //        $("#end_date").prop("disabled", true);
    //        $("#from_date").val(new Date(GetDatefromMoment(newWOArray[index].StartScheduleDate + 24 * 60 * 60000)).toLocaleDateString('en-US'));
    //        $("#end_date").val(new Date(GetDatefromMoment(newWOArray[index].EndScheduleDate + 24 * 60 * 60000)).toLocaleDateString('en-US'));

    //    }
    //    else {
    //        $("#from_date").prop("disabled", false);
    //        $("#end_date").prop("disabled", false);
    //    }
    //    $("#NumOfInstallers").prop("disabled", false);

    //    $("#Asbestos-JobsYes").prop("disabled", false);
    //    $("#Asbestos-JobsNo").prop("disabled", false);

    //    $("#Lead-PaintYes").prop("disabled", false);
    //    $("#Lead-PaintNo").prop("disabled", false);

    //    $("#Wood-DropOff-JobsYes").prop("disabled", false);
    //    $("#Wood-DropOff-JobsNo").prop("disabled", false);

    //    $("#HighRisk-JobsYes").prop("disabled", false);
    //    $("#HighRisk-JobsNo").prop("disabled", false);

    //    document.getElementsByName('saturday')[0].disabled = false;
    //    document.getElementsByName('sunday')[0].disabled = false;



    //    //    $("#from_date").val('');
    //    //    $("#end_date").val('');

    //}

    eventid = newWOArray[index].id;

    if (newWOArray[index].Saturday == "Yes") {
        document.getElementsByName('saturday')[0].checked = true;
    }
    else {
        document.getElementsByName('saturday')[0].checked = false;
    }
    if (newWOArray[index].Sunday == "Yes") {
        document.getElementsByName('sunday')[0].checked = true;
    }
    else {
        document.getElementsByName('sunday')[0].checked = false;
    }

    //retrieve product info
    // var ss = GetNonReturnedJobDates(newWOArray[index].WorkOrderNumber);
    //GetProducts(newWOArray[index].WorkOrderNumber);
    GetInstallationProducts(newWOArray[index].WorkOrderNumber);
    GetManufacturingProducts(newWOArray[index].WorkOrderNumber);
    GetInstallers(newWOArray[index].WorkOrderNumber);
    GetCalledLog(newWOArray[index].WorkOrderNumber);
    GetNotes(newWOArray[index].WorkOrderNumber);
    GetWOPicture(newWOArray[index].WorkOrderNumber);
    GetJobReview(newWOArray[index].WorkOrderNumber);
    GetDocumentLibrary(newWOArray[index].WorkOrderNumber);
    GetSubTrades(newWOArray[index].WorkOrderNumber);
    GetRemeasurerName(newWOArray[index].WorkOrderNumber);
    // GetJobAnalysys(newWOArray[index].WorkOrderNumber);
    $("#TotalLBRMin").html(newWOArray[index].TotalInstallationLBRMin);

    $("#eventLink").attr('href', newWOArray[index].url);


     
    //var str = "ss";
    //str = "dd";
}

function AddInstallationBufferEvent(key, val) {
    var ret;
    if (val.EstInstallerCnt < 1) {
        ret =  
            (val.Windows != "0" ? "&nbsp;<img alt=\"# of Windows: " + val.Windows + "Status: " + val.WindowState + "\" src=\"images/window.PNG\" />" : "") +
            (val.Doors != "0" ? "&nbsp;<img alt=\"# of Doors: " + val.Doors + "Status: " + val.DoorState + "\" src=\"images/door.PNG\" />" : "") + "&nbsp;" +
            (" " + val.WorkOrderNumber) + "&nbsp;" +
            ("Name: " + val.LastName.trim().length > 10 ? val.LastName.trim().Substring(0, 10) : val.LastName.trim() + "&nbsp;" + val.FirstName.trim().length > 10 ? val.FirstName.trim().Substring(0, 10) : val.FirstName.trim()) +
            "&nbsp;" + (val.City.trim().Length > 5 ? val.City.trim().toLowerCase().Substring(0, 5) : val.City.trim().toLowerCase());
    }
    else {
        ret   = "<img src=\"images/installer" + val.EstInstallerCnt + ".png\" title=\"Estimated number of installers for the job: " +
            val.EstInstallerCnt + "\">" +
            (val.Windows != "0" ? "&nbsp;<img alt=\"# of Windows: " + val.Windows + "Status: " + val.WindowState + "\" src=\"images/window.PNG\" />" : "") +
            (val.Doors != "0" ? "&nbsp;<img alt=\"# of Doors: " + val.Doors + "Status: " + val.DoorState + "\" src=\"images/door.PNG\" />" : "") + "&nbsp;" +
            (" " + val.WorkOrderNumber) + "&nbsp;" +
            ("Name: " + val.LastName.trim().length > 10 ? val.LastName.trim().Substring(0, 10) : val.LastName.trim() + "&nbsp;" + val.FirstName.trim().length > 10 ? val.FirstName.trim().Substring(0, 10) : val.FirstName.trim()) +
            "&nbsp;" + (val.City.trim().Length > 5 ? val.City.trim().toLowerCase().Substring(0, 5) : val.City.trim().toLowerCase());
    }
      
   // $(element).find(dom).prepend(ret);

    var el = $("<div class='fc-event "   + (val.JobType == "RES" ? " reservation" : "") +
       "' id=\"" + val.id + "\" onclick=\"NewWOPopup('" + val.id + "');\"" + " data-toggle=\"modal\"" + " data-target=\"#eventContent\"" + " href=\"/details\"" + 
      //  "' id=\"" + val.id + "\" onclick=\"NewWOPopup('" + val.id + "');\"" + 
        " style=\"background-color:" + val.color + "\">" + ret + "</div>").appendTo('#external-events1');
    el.draggable({
        zIndex: 9999,
        revert: true,
        scroll: false,
        helper: 'clone',
        opacity: 0.70,
        revertDuration: 0  //  original position after the drag
    });

    //$("#calendar").droppable({
    //    classes: {
    //        "ui-droppable-active": "ui-state-active",
    //        "ui-droppable-hover": "ui-state-hover"
    //    },
    //    drop: function (event, ui) {
    //        $(this)
    //            .addClass("ui-state-highlight")
    //            .find("p")
    //            .html("Dropped!");
    //    }
    //});
   
   // el.editable = (readonly == "True") ? false : true;
   // el.draggable = (readonly == "True") ? false : true;
  
    $('#' + val.id).data('event', {
        // title: val.title, id: val.id, description: val.description, doors: val.doors, windows: val.windows, type: val.type, JobType: val.JobType, boxes: val.boxes, glass: val.glass, value: val.value, min: val.min, max: val.max, rush: val.rush, float: val.float, TotalBoxQty: val.TotalBoxQty, TotalGlassQty: val.TotalGlassQty, TotalPrice: val.TotalPrice, TotalLBRMin: val.TotalLBRMin, F6CA: val.F6CA, F27DS: val.F27DS, F27TS: val.F27TS, F27TT: val.F27TT, F29CA: val.F29CA, F29CM: val.F29CM, F52PD: val.F52PD, F68CA: val.F68CA, F68SL: val.F68SL, F68VS: val.F68VS, DoubleDoor: val.DoubleDoor, Transom: val.Transom, Sidelite: val.Sidelite, SingleDoor: val.SingleDoor
      //  title: val.title, id: val.id, doors: val.Doors, windows: val.Windows, WorkOrderNumber: title.WorkOrderNumber, Branch: val.Branch, City: val.City, CellPhone: val.CellPhone, CrewNames: val.CrewNames, CurrentStateName: val.CurrentStateName, LastName: val.LastName, FirstName: val.FirstName
        title: val.title, id: val.id, doors: val.Doors, City: val.City, windows: val.Windows, WorkOrderNumber: val.WorkOrderNumber, LastName: val.LastName, FirstName: val.FirstName,
        EstInstallerCnt: val.EstInstallerCnt, WindowState: val.WindowState, DoorState: val.DoorState, TotalWoodDropOff: val.TotalWoodDropOff, WoodDropOffDate: val.WoodDropOffDate,
        TotalAsbestos: val.TotalAsbestos, LeadPaint: val.LeadPaint,
        TotalHighRisk: val.TotalHighRisk, ReturnedJob: val.ReturnedJob,CurrentStateName:val.CurrentStateName

    });

    //new Draggable(val.id, {
    //    eventData: {
    //        title: 'my event',
    //        duration: '02:00'
    //    }
    //});

}

function AddRemeasureBufferEvent(key, val) {
    var ret;
 
        ret =
            (val.Windows != "0" ? "&nbsp;<img alt=\"# of Windows: " + val.Windows + "Status: " + val.WindowState + "\" src=\"images/window.PNG\" />" : "") +
            (val.Doors != "0" ? "&nbsp;<img alt=\"# of Doors: " + val.Doors + "Status: " + val.DoorState + "\" src=\"images/door.PNG\" />" : "") + "&nbsp;" +
            (" " + val.WorkOrderNumber) + "&nbsp;" +
            ("Name: " + val.LastName.trim().length > 10 ? val.LastName.trim().Substring(0, 10) : val.LastName.trim() + "&nbsp;" + val.FirstName.trim().length > 10 ? val.FirstName.trim().Substring(0, 10) : val.FirstName.trim()) +
            "&nbsp;" + (val.City.trim().Length > 5 ? val.City.trim().toLowerCase().Substring(0, 5) : val.City.trim().toLowerCase());

    // $(element).find(dom).prepend(ret);

    var el = $("<div class='fc-event " + (val.JobType == "RES" ? " reservation" : "") +
        "' id=\"" + val.id + "\" style=\"background-color:" + val.color + "\">" + ret + "</div>").appendTo('#external-eventsRemeasure');
    el.draggable({
        zIndex: 996,
        revert: true,
        scroll: false,
        helper: 'clone',
        opacity: 0.70,
        revertDuration: 0  //  original position after the drag
    });
    $('#' + val.id).data('event', {
        // title: val.title, id: val.id, description: val.description, doors: val.doors, windows: val.windows, type: val.type, JobType: val.JobType, boxes: val.boxes, glass: val.glass, value: val.value, min: val.min, max: val.max, rush: val.rush, float: val.float, TotalBoxQty: val.TotalBoxQty, TotalGlassQty: val.TotalGlassQty, TotalPrice: val.TotalPrice, TotalLBRMin: val.TotalLBRMin, F6CA: val.F6CA, F27DS: val.F27DS, F27TS: val.F27TS, F27TT: val.F27TT, F29CA: val.F29CA, F29CM: val.F29CM, F52PD: val.F52PD, F68CA: val.F68CA, F68SL: val.F68SL, F68VS: val.F68VS, DoubleDoor: val.DoubleDoor, Transom: val.Transom, Sidelite: val.Sidelite, SingleDoor: val.SingleDoor
        //  title: val.title, id: val.id, doors: val.Doors, windows: val.Windows, WorkOrderNumber: title.WorkOrderNumber, Branch: val.Branch, City: val.City, CellPhone: val.CellPhone, CrewNames: val.CrewNames, CurrentStateName: val.CurrentStateName, LastName: val.LastName, FirstName: val.FirstName
        title: val.title, id: val.id, doors: val.Doors, City: val.City, windows: val.Windows, WorkOrderNumber: val.WorkOrderNumber, LastName: val.LastName, FirstName: val.FirstName,
        EstInstallerCnt: val.EstInstallerCnt, WindowState: val.WindowState, DoorState: val.DoorState, TotalWoodDropOff: val.TotalWoodDropOff, WoodDropOffDate: val.WoodDropOffDate,
        TotalAsbestos: val.TotalAsbestos, LeadPaint: val.LeadPaint,
        TotalHighRisk: val.TotalHighRisk, ReturnedJob: val.ReturnedJob, CurrentStateName: val.CurrentStateName

    });

    //new Draggable(val.id, {
    //    eventData: {
    //        title: 'my event',
    //        duration: '02:00'
    //    }
    //});
 }



function LoadRemeasureBufferedJobs() {
    var branches = [];
    $.each($("input[name='branch']:checked"), function () {
        branches.push($(this).val());
    });
    console.log("checked branches: ", branches.join(","));
    var eventBufferWODict = [];
    $.ajax({
        url: 'data.svc/GetRemeasureBufferJobs',
        dataType: 'json',
        data: { branch: branches.join(",") },
        success: function (data) {
            if (debug) console.log("events.success", "data.GetRemeasureBufferJobsResult:", data.GetRemeasureBufferJobsResult === undefined ? "NULL" : data.GetRemeasureBufferJobsResult.length);
            var events = [];


            var found = false;
            var con;
            $.each(data.GetRemeasureBufferJobsResult, function (pos, item) {
               AddRemeasureBufferEvent(pos, item);
            });

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });



}


function AddBufferEvent(key, val) {
    var img = "";
    if (val.PaintIcon != undefined && val.PaintIcon != '' && val.PaintIcon == "Yes")
        img += "<img alt=\"#\" src=\"images/color.png\" />&nbsp;";

    if (val.windows > 0)
        img += "<img alt=\"#\" src=\"images/window.png\" title= \"" + ToWDString(val) + "\" />&nbsp;";

    if (val.F52PD > 0) // Patio Doors
        img += "<img alt=\"#\" src=\"images/patiodoor.png\" title= \"" + ToPDString(val) + "\" />&nbsp;";

    if (val.doors > 0)
        img += "<img alt=\"#\" src=\"images/door.png\" title= \"" + ToDString(val) + "\" />&nbsp;";

    if (val.FlagOrder != undefined && val.FlagOrder == 1) // Rush Order
        img += "<img alt=\"#\" src=\"images/flag.png\" />&nbsp;";

    if (val.M2000Icon != undefined && val.M2000Icon == 1) // M2000
        img += "<img alt=\"#\" src=\"images/M2000.png\" />&nbsp;";

    var el = $("<div class='fc-event" + (val.JobType == "RES" ? " reservation" : "") + "' id=\"" + val.id + "\" style=\"background-color:" + val.color + "\">" + val.title + img +   "</div>").appendTo('#external-events');
    
    el.draggable({
        zIndex: 999,
        revert: true,      // will cause the event to go back to its
        scroll: false,
             helper: 'clone',
        opacity: 0.70,
        revertDuration: 0  //  original position after the drag
    });

    el.editable = (readonly == "True") ? false : true;
    el.draggable = (readonly == "True") ? false : true;

    $('#' + val.id).data('event', {
        // title: val.title, id: val.id, description: val.description, doors: val.doors, windows: val.windows, type: val.type, JobType: val.JobType, boxes: val.boxes, glass: val.glass, value: val.value, min: val.min, max: val.max, rush: val.rush, float: val.float, TotalBoxQty: val.TotalBoxQty, TotalGlassQty: val.TotalGlassQty, TotalPrice: val.TotalPrice, TotalLBRMin: val.TotalLBRMin, F6CA: val.F6CA, F27DS: val.F27DS, F27TS: val.F27TS, F27TT: val.F27TT, F29CA: val.F29CA, F29CM: val.F29CM, F52PD: val.F52PD, F68CA: val.F68CA, F68SL: val.F68SL, F68VS: val.F68VS, DoubleDoor: val.DoubleDoor, Transom: val.Transom, Sidelite: val.Sidelite, SingleDoor: val.SingleDoor
        title: val.title, id: val.id, description: val.description, doors: val.Doors, windows: val.Windows, type: val.type, JobType: val.JobType, boxes: val.boxes, glass: val.glass, value: val.value, min: val.min, max: val.max, rush: val.rush, float: val.float, TotalBoxQty: val.TotalBoxQty, TotalGlassQty: val.TotalGlassQty, TotalPrice: val.TotalPrice, TotalLBRMin: val.TotalLBRMin, F6CA: val.F6CA, F27DS: val.F27DS, F27TS: val.F27TS, F27TT: val.F27TT, F29CA: val.F29CA, F29CM: val.F29CM, F52PD: val.F52PD, F68CA: val.F68CA, F68SL: val.F68SL, F68SLMin: val.F68SLMin, F68VS: val.F68VS, F68VSMin: val.F68VSMin, F29CMMin: val.F29CMMin, F29CAMin: val.F29CAMin, F26CAMin: val.F26CAMin, F27DSMin: val.F27DSMin,  DoubleDoor: val.DoubleDoor, Transom: val.Transom, Sidelite: val.Sidelite, SingleDoor: val.SingleDoor
    });
}
function JsonDateToDate(dateString) {
    var currentTime = new Date(parseInt(dateString.substr(6)));
    var month = currentTime.getMonth() + 1;
    var day = currentTime.getDate();
    var year = currentTime.getFullYear();
    var date = day + "/" + month + "/" + year;
    return new Date(date);
}
function LoadGlobalValues(firstDay, lastDay) {
    /*var date = nw Date();
    var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
    var lastDay = new Date((new Date(date.getFullYear(), date.getMonth() + 1, 1)) - 1);*/
    var pars = { type: displayType, start: firstDay._d.format("MM/dd/yyyy"), end: lastDay._d.format("MM/dd/yyyy") };
    $.getJSON("data.svc/GetSystemParameters", pars, function (data) {
        GlobalParams = data.GetSystemParametersResult;
    });
}
$(document).ready(function () {
    LoadBufferedJobs();
   // LoadInstallationBufferedJobs();
   //LoadRemeasureBufferedJobs();

    $('#external-events1').hide();
   $('#external-eventsRemeasure').hide();

    //$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    //    var currentTab = $(e.target).text(); // get current tab
    //    var LastTab = $(e.relatedTarget).text(); // get last tab
    // //   alert(currentTab);
    //    $(".current-tab span").html(currentTab);
    //    $(".last-tab span").html(LastTab);

    //    if (currentTab == "JOB ANALYSIS") {
    //        GetJobAnalysys("11231");
    //    }
    //});
    //$(document).on('shown.bs.tab', 'a[data-toggle="tab"]', function (e) {

    //    alert(e.target.href);
    //})

    /* initialize the calendar
    -----------------------------------------------------------------*/

    $('#calendar').fullCalendar({
        schedulerLicenseKey: 'GPL-My-Project-Is-Open-Source',   
        aspectRatio: 1.7,
       // locale: 'es',
       // Duration, default: "00:00:00"
      
        customButtons: {
            changeType: {
                text: displayType,
                click: function () {
                    $('#typeChange').removeClass('hidden');
                }
            },
            applyFilters: {
                text: "Change Record Filters",
                click: function () {
                    $('#stateFilter').removeClass('hidden');
                }
            },
            applyInstallationFilters: {
                text: "Change Installation Record Filters",
                click: function () {
                    $('#InstallationStateFilter').removeClass('hidden');
                }
            },
            applyRemeasureFilters: {
                text: "Change Remeasure Record Filters",
                click: function () {
                    $('#RemeasureStateFilter').removeClass('hidden');
                }
            },
            changeBranch: {
                text: "Change Branch",
                click: function () {
                    $('#BranchFilter').removeClass('hidden');
                }
            },
            changeJobType: {
                text: "Change Job Type",
                click: function () {
                    $('#JobTypeFilter').removeClass('hidden');
                }
            },

            changeShippingType: {
                text: "Change Shipping Type",
                click: function () {
                    $('#ShippingTypeFilter').removeClass('hidden');
                }
            },
            refresh: {
                text: "Refresh",
                click: function () {
                    for (i = 0; i < document.getElementsByName('InstallationState').length; i++) {
                        document.getElementsByName('InstallationState')[i].checked = true;
                    }
                    totals = getBlankTotal();
                    $('#calendar').fullCalendar('refetchEvents');
                }
            },
            ShowWIP: {
                text: "WIP",
                click: function () {
                  
                    var i ;
                    for (i = 0; i < document.getElementsByName('InstallationState').length; i++) {
                        document.getElementsByName('InstallationState')[i].checked = false;
                    }
                    document.getElementsByName('InstallationState')[0].checked = true;
                    document.getElementsByName('InstallationState')[3].checked = true;
                    document.getElementsByName('InstallationState')[7].checked = true;
                   // document.getElementsByName('InstallationState')[18].checked = false;
                    
                   // totals = getBlankTotal();
                    $('#calendar').fullCalendar('refetchEvents');
                }
            },
            ShowKelownaBranch: {
                text: "Kelowna",
                click: function () {

                    var i;
                    for (i = 0; i < document.getElementsByName('branch').length; i++) {
                        document.getElementsByName('branch')[i].checked = false;
                    }
                    document.getElementsByName('branch')[0].checked = true;
                    // document.getElementsByName('InstallationState')[18].checked = false;

                    // totals = getBlankTotal();
                    $('#calendar').fullCalendar('refetchEvents');
                }
            },

            ShowLowerMainlandBranch: {
                text: "LowerMainland",
                click: function () {

                    var i;
                    for (i = 0; i < document.getElementsByName('branch').length; i++) {
                        document.getElementsByName('branch')[i].checked = false;
                    }
                    document.getElementsByName('branch')[1].checked = true;
                    // document.getElementsByName('InstallationState')[18].checked = false;

                    // totals = getBlankTotal();
                    $('#calendar').fullCalendar('refetchEvents');
                }
            },
            ShowNanaimoBranch: {
                text: "Nanaimo",
                click: function () {

                    var i;
                    for (i = 0; i < document.getElementsByName('branch').length; i++) {
                        document.getElementsByName('branch')[i].checked = false;
                    }
                    document.getElementsByName('branch')[2].checked = true;
                    // document.getElementsByName('InstallationState')[18].checked = false;

                    // totals = getBlankTotal();
                    $('#calendar').fullCalendar('refetchEvents');
                }
            },
            ShowVictoriaBranch: {
                text: "Victoria",
                click: function () {

                    var i;
                    for (i = 0; i < document.getElementsByName('branch').length; i++) {
                        document.getElementsByName('branch')[i].checked = false;
                    }
                    document.getElementsByName('branch')[3].checked = true;
                    // document.getElementsByName('InstallationState')[18].checked = false;

                    // totals = getBlankTotal();
                    $('#calendar').fullCalendar('refetchEvents');
                }
            },
           
        },
        buttonText: {
            timelineDay: 'time'
           
        },

        //eventClick: function (event, jsEvent, view) {

        //    let currentClick = jsEvent.clientX;
        //    let widthEl = $(this).outerWidth();
        //    let leftPoint = $(this).offset().left;
        //    let slotWidth = view.slotWidth;
        //    let slotDuration = view.slotDuration._milliseconds / 60000;
        //    let widthEvent = currentClick - leftPoint;

        //    let stopMinute = widthEvent / slotWidth * slotDuration;

        //    console.log(stopMinute);

        //},

        dayClick: function (date, jsEvent, view, resourceObj) {

            if (view.name == "timelineDay") {
                //alert('Clicked on: ' + date.format());

                //alert('Coordinates: ' + jsEvent.pageX + ',' + jsEvent.pageY);

                //alert('Current view: ' + view.name);

                //// change the day's background color just for fun
                //$(this).css('background-color', 'red');
                //$(this).attr('data-toggle', "modal");
                //$(this).attr('data-target', "#taskModal");
                //$(this).attr('href', "/details");
                //$(this).attr('href', 'javascript:void(0);');
                //resourceObj.attr('data-toggle', "modal");
                //resourceObj.attr('data-target', "#taskModal");
                //resourceObj.attr('href', "/details");
             
                //alert('Date: ' + date.format());
                //alert('Resource id: ' + resourceObj.id);
               // $(this).attr('href', 'javascript:void(0);');
                //$(this).attr('data-toggle', "modal");
                //$(this).attr('data-target', "#taskModal");
                
                //$(this).attr('href', "/details");
                $("#TruckStartDate").val(moment().format('MM/DD/ YYYY'));
                $("#TruckEndDate").val(moment().format('MM/DD/ YYYY'));
                $("#TruckStartTime").val(moment().format('HH:mm'));

                $("#TruckEndTime").val(moment().add(2, 'hours').format('HH:mm'));
                
                $('#taskModal').modal('show');
        
            }
           

        },
        header: {
            left: 'prev,next today, changeType,applyFilters,applyInstallationFilters,applyRemeasureFilters,changeBranch,changeJobType,changeShippingType',
            center: 'title',
            right: 'ShowKelownaBranch,ShowLowerMainlandBranch,ShowNanaimoBranch,ShowVictoriaBranch,ShowWIP,month,agendaWeek,agendaOneDay,timelineDay'
        },
        views: {
            agendaOneDay: {
                type: 'agenda',
                duration: { days: 1 },
                buttonText: 'day1'
            }
        },
       // GetResources();
        resourceLabelText: 'Resources',
        resourceAreaWidth: '200px',
        //resource: [
        //    { id: '1', resourceId: '17658388', start: '2020-02-04T12:00:00', end: '2020-02-04T12:05:00', title: 'Carico su camion', id_evento_collegato: [2, 3, 4, 5, 6, 7] },
        //    { id: '2', resourceId: '17658388', start: '2020-02-04T13:00:00', end: '2020-02-04T14:00:00', title: 'Viaggio', id_evento_collegato: [1, 3, 4, 5, 6, 7], editable: false, backgroundColor: "red" }

        //],
       resourceGroupField: 'groupId',
      
        refetchResourcesOnNavigate: true,
        resources: function (callback,start, end, timezone) {
            dataArray = [];
            $.ajax({
                //type: "POST",  
                url: 'data.svc/GetTruckListWithWO',
                dataType: 'json',
                async: false,
                success: function (data) {
                    if (debug) console.log("events.success", "data.GetTruckList:");

                    if (data.GetTruckListWithWOResult.length > 0) {

                        for (var i = 0; i < data.GetTruckListWithWOResult.length; i++) {
                            var re = {};
                            if (data.GetTruckListWithWOResult[i].RecordID.length == 0) {
                                re["id"] = data.GetTruckListWithWOResult[i].ActionItemId;
                               re["title"] = data.GetTruckListWithWOResult[i].TruckName + " (Add WorkOrder)";
                              //  re["title"] = data.GetTruckListWithWOResult[i].TruckName ;
                              //  re["groupId"] = data.GetTruckListWithWOResult[i].TruckName;
                            }
                            else {
                                re["id"] = data.GetTruckListWithWOResult[i].RecordID + data.GetTruckListWithWOResult[i].TruckLookup;
                                re["title"] = data.GetTruckListWithWOResult[i].WorkOrderNumber;
                                re["parentId"] = data.GetTruckListWithWOResult[i].ActionItemId;
                              //  re["id"] = data.GetTruckListWithWOResult[i].RecordID + data.GetTruckListWithWOResult[i].TruckLookup;
                               // re["groupId"] = data.GetTruckListWithWOResult[i].TruckName;
                               //// re["parentId"] = data.GetTruckListWithWOResult[i].TruckName;
                               // re["title"] = data.GetTruckListWithWOResult[i].WorkOrderNumber;
                            }
                  
                            dataArray.push(re);
                        }
                     
                        callback(dataArray);
                    }

                }, error: function (error) {
                    console.log('Error', error);
                    $('#script-warning').show();
                }
            });
        },

        resourceRender: function (resourceObj, labelTds, bodyTds) {
            labelTds.on('click', function (resourceObj, bodyTds) {
                //alert(resourceObj.title);
             //   TruckCrewEdit(resourceObj);
               // $(this).attr('href', 'javascript:void(0);');
                if (labelTds[0].textContent.includes("Add")) {  //truck name gets clicked, populate window to assign wo
                    $(this).attr('data-toggle', "modal");
                    $(this).attr('data-target', "#TruckWorkOrderPop");
                    $(this).attr('href', "/details");
                    truckName = labelTds[0].textContent;
                  //  truckRecordID = labelTds.prevObject[0].dataset.resourceId;
                    truckRecordID = labelTds.prevObject[0].dataset.resourceId;
                    $("#txtWorkOrderSearch").val('');


                    document.getElementById('dvDateAndCrews').style.display = "none";

                    $("#TruckWorkOrderPopTitle").html(" Truck " + truckName);
                                 //  $("#txtTruckCrewName").val('');
                   // $("#dataTableTruckInstallerAdd tr").remove();
                   // LoadTruckCrews(truckRecordID, labelTds[0].textContent.replace(' ( Click to edit Crews)', ''));
                }
               
            });
        },

       //hiddenDays: [0, 6],

      //  weekends: false,

        events: function (start, end, timezone, callback) {
            if (debug) console.log("events", "start:", start.format(), "end:", end.format());
            LoadGlobalValues(start, end);
                        
            var states = [];
            $.each($("input[name='state']:checked"), function () {
                states.push($(this).val());
            });
            console.log("checked states: ", states.join(","));
                       
            var installationStates = [];
            $.each($("input[name='InstallationState']:checked"), function () {
                installationStates.push($(this).val());
            });
            console.log("checked states: ", installationStates.join(","));

            var remeasureStates = [];
            $.each($("input[name='RemeasureState']:checked"), function () {
                remeasureStates.push($(this).val());
            });
            console.log("checked remeasure states: ", remeasureStates.join(","));


            var branches = [];
            $.each($("input[name='branch']:checked"), function () {
                branches.push($(this).val());
            });
            console.log("checked branches: ", branches.join(","));
            var jobTypes = [];
            $.each($("input[name='jobType']:checked"), function () {
                jobTypes.push($(this).val());
            });
            console.log("checked jobTypes: ", jobTypes.join(","));
            var shippingType = [];
            $.each($("input[name='ShippingType']:checked"), function () {
                shippingType.push($(this).val());
            });
            console.log("checked shippingType: ", shippingType.join(","));


            var jobstaff = $('#JobStaff');
            if (jobstaff.length == 0) {
                $(".fc-left").append('<div class="fc-button-group" style="margin-top: 5px; "><select id="JobStaff"  onchange="changeJobStaff()" style="width:80px;height:20px;"><option class="fc-agendaDay-button fc-button fc-state-default fc-corner-right">Job</option><option class="fc-agendaWeek-button fc-button fc-state-default">Staff</option></select></div>');
                //   $(".fc-left").append(' <div class="dropdown" id="JobStaff"><button type="button" class="btn btn-primary dropdown-toggle" data-toggle="dropdown">Dropdown button</button><div class="dropdown-menu"><a class="dropdown-item" href="#">Link 1</a><a class="dropdown-item" href="#">Link 2</a><a class="dropdown-item" href="#">Link 3</a> </div>  </div>');

            }

            if (displayType == "Installation") {

    

                $('.fc-applyInstallationFilters-button').show();
                $('#JobStaff').show();
                $('.fc-ShowKelownaBranch-button').show();
                $('.fc-ShowLowerMainlandBranch-button').show();
                $('.fc-ShowNanaimoBranch-button').show();
                $('.fc-ShowVictoriaBranch-button').show();
               // $('.fc-changeJobStaff-button').show();

                $('.fc-ShowWIP-button').show();
                $('.fc-applyFilters-button').hide();
                $('.fc-changeJobType-button').hide();
                $('.fc-changeShippingType-button').hide();

                $('.fc-applyRemeasureFilters-button').hide();
            //    $('#calendar').fullCalendar('changeView', 'month');
              //  document.getElementById('external-InstallationEvents').style.display = "block";
              //  document.getElementById('external-events').style.display = "none";
               // LoadInstallationBufferedJobs();
             //   $('.fc-event').remove();

                
                $.ajax({
                    url: 'data.svc/GetInstallationEvents',
                    dataType: 'json',
                    async: false,
                    data: { start: start.format(), end: end.format(), branch: branches.join(","), installationStates: installationStates.join(",") },
                    success: function (data) {
                        if (debug) console.log("events.success", "data.GetInstallationEventsResult:", data.GetInstallationEventsResult === undefined ? "NULL" : data.GetInstallationEventsResult.length);
                        var events = [];
                        eventWODict = [];
                        var con;
                        $.each(data.GetInstallationEventsResult, function (pos, item) {
                        //    item.allDay = true;
                        //    if (item.WorkOrderNumber == "YongTest7") {

                            if ((item.ResourceID == undefined) || (item.ResourceID==null)) {
                            //    item.end = moment(item.EndTime).toDate().format("M/dd/yyyy HH:mm");
                                 // item.end = item.start;
                            }
                            else if (item.ResourceID.length>1)
                            {
                              // item.end = moment(item.EndTime).toDate().format("M/dd/yyyy HH:mm");
                                item.end =   moment(item.end).add(24, 'hours').format("YYYY-MM-DD HH:mm");
                               // item.allDay = false;
                            }
                            //if (item.WorkOrderNumber == "YongTest7") {

                            //   // item.end = "4:00 PM";
                            //    //       item.start = moment().add(0, 'hours').format('HH:mm');
                            //    //item.end = moment().add(4, 'hours').format('HH:mm');
                            //    //item.start = moment().add(0, 'hours').format("YYYY-MM-DD HH:mm");
                            //  //  item.start = "2020-02-14T14:30:00";
                            //    item.end = item.end = moment(item.EndTime).toDate().format("M/dd/yyyy HH:mm");
                             
                                
                            //    //item.end = moment().add(2, 'hours').format("YYYY-MM-DDTHH:mm");
                            //}
                             
                             item.resourceId = item.ParentRecordId + item.ResourceID;
                        //   }
                        
                            //if (item.WorkOrderNumber == "YongTest7") {
                            //    item.start = moment().add(0, 'hours').format('HH:mm');
                            //    item.end = moment().add(4, 'hours').format('HH:mm');
                            //    //item.start = moment().add(0, 'hours').format("YYYY-MM-DD HH:mm");
                            //    //item.end = moment().add(2, 'hours').format("YYYY-MM-DDTHH:mm");
                            //}

                     
                            
                         //   item.editable = (item.ReturnedJob == 1) ? false : true;

                           
                            item.editable = ((item.HolidayName != null || readonly == "True" || item.UnavailableStaff != null) )? false : true;
                          //  item.editable = (readonly == "True") ? false : true;
                            if ((($("#JobStaff").val() == "Job") && (item.UnavailableStaff == null)) || ((($("#JobStaff").val() == "Staff") && (item.UnavailableStaff != null)))) {
                                eventWODict.push(item);
                            }
                        });
                        //if ((item.UnavailableStaff == undefined) || (item.UnavailableStaff == null)) {
                        //    events = removeDuplicates(eventWODict, "WorkOrderNumber");
                        //}
                        events = removeDuplicates(eventWODict, "WorkOrderNumber");
                        callback(events);
                    }, error: function (error) {
                        console.log('Error', error);
                        $('#script-warning').show();
                    }
                });

            
      
            }
            else if (displayType == "Remeasure")
            {

                $('.fc-applyRemeasureFilters-button').show();
                $('#JobStaff').hide();
                $('.fc-applyInstallationFilters-button').hide();
                $('.fc-ShowWIP-button').hide();
                
                //$('.fc-changeJobStaff-button').hide();
                $('.fc-ShowKelownaBranch-button').hide();
                $('.fc-ShowLowerMainlandBranch-button').hide();
                $('.fc-ShowNanaimoBranch-button').hide();
                $('.fc-ShowVictoriaBranch-button').hide();

                $('.fc-applyFilters-button').hide();
                $('.fc-changeJobType-button').hide();
                $('.fc-changeShippingType-button').hide();
                //  $('#calendar').fullCalendar('changeView', 'month');
                //   document.getElementById('external-InstallationEvents').style.display = "none";
                //  document.getElementById('external-events').style.display = "block";
                $.ajax({
                    url: 'data.svc/GetRemeasureEvents',
                    dataType: 'json',
                    data: { start: start.format(), end: end.format(), branch: branches.join(","), remeasureStates: remeasureStates.join(",") },
                    //  data: { start: start.format(), end: end.format(), branch: branches.join(",") },
                    success: function (data) {
                        if (debug) console.log("events.success", "data.GetRemeasureEventsResult:", data.GetRemeasureEventsResult === undefined ? "NULL" : data.GetRemeasureEventsResult.length);
                        var events = [];
                        eventRemeasureWODict = [];
                        $.each(data.GetRemeasureEventsResult, function (pos, item) {
                            item.allDay = true;
                            item.editable = ((item.HolidayName != null || readonly == "True")) ? false : true;
                            //  item.editable = (readonly == "True") ? false : true;

                            eventRemeasureWODict.push(item);
                            
                        });
                        events = removeDuplicates(eventRemeasureWODict, "WorkOrderNumber");
                        callback(events);
                    }, error: function (error) {
                        console.log('Error', error);
                        $('#script-warning').show();
                    }
                });
            }
            else {
                $('.fc-applyInstallationFilters-button').hide();
                $('.fc-ShowWIP-button').hide();
                $('#JobStaff').hide();
                $('.fc-applyRemeasureFilters-button').hide();
             //   $('.fc-changeJobStaff-button').hide();
                $('.fc-ShowKelownaBranch-button').hide();
                $('.fc-ShowLowerMainlandBranch-button').hide();
                $('.fc-ShowNanaimoBranch-button').hide();
                $('.fc-ShowVictoriaBranch-button').hide();

                $('.fc-applyFilters-button').show();
                $('.fc-changeJobType-button').show();
                $('.fc-changeShippingType-button').show();
              //  $('#calendar').fullCalendar('changeView', 'month');
             //   document.getElementById('external-InstallationEvents').style.display = "none";
              //  document.getElementById('external-events').style.display = "block";
                $.ajax({
                    url: 'data.svc/GetEvents',
                    dataType: 'json',
                    data: { start: start.format(), end: end.format(), type: displayType, states: states.join(","), branch: branches.join(","), jobType: jobTypes.join(","), shippingType: shippingType.join(",") },
                    //  data: { start: start.format(), end: end.format(), branch: branches.join(",") },
                    success: function (data) {
                        if (debug) console.log("events.success", "data.GetEventsResult:", data.GetEventsResult === undefined ? "NULL" : data.GetEventsResult.length);
                        var events = [];
                        eventDiff = [];
                       // evenDict = [];
                        $.each(data.GetEventsResult, function (pos, item) {
                            item.allDay = true;
                            item.editable = (item.HolidayName != null) ? false : true;
                            item.editable = (readonly == "True") ? false : true;
                      
                          //  evenDict.push(item);
                            events.push(item);
                         
                        });
                       // events = removeDuplicates(evenDict, "title");
                        callback(events);
                    }, error: function (error) {
                        console.log('Error', error);
                        $('#script-warning').show();
                    }
                });
            }

            $("#dataTableHR tr").remove();

        },
    

        editable: readonly == "True" ? false : true,
        nowIndicator: true,
        eventDurationEditable: readonly == "True" ? false : true,
        droppable: readonly == "True" ? false : true, // this allows things to be dropped onto the calendar
        weekNumbers: true,
        businessHours: { start: '8:00', end: '17:00', dow: [1, 2, 3, 4, 5, 6] },
        eventDrop: function (event, delta, revertFunc) {

            var message = "The Work Order '" + event.title + "' was scheduled on " + event.start.toISOString() + ". Are you sure about this change ?";
            if (!confirm(message)) {
                revertFunc();
            }
            else {
                if ((displayType != "Installation") && (displayType != "Remeasure")) {
                    if (debug) console.log('eventDrop', 'event.title=' + event.title, "id: " + event.id, "labour min: " + event.TotalLBRMin, "event date:", GetDatefromMoment(event.start));
                    var eventDate = GetDatefromMoment(event.start);
                    var maxDayLabour = parseInt(FindByValue("max", eventDate).Value);
                    var allocatedDayLabour = GetDayLabourValue($('#calendar').fullCalendar('getView'), eventDate);
                    //   console.log("eventDrop", "allocated min:", allocatedDayLabour, "event min:", event.TotalLBRMin, "max min:", maxDayLabour);
                    //if (allocatedDayLabour + event.TotalLBRMin <= maxDayLabour) {
                    //    eventUpdate(event);
                    //} else {
                    //    ShowWarning(allocatedDayLabour, event.TotalLBRMin, maxDayLabour);
                    //    revertFunc();
                    //}
                    eventUpdate(event);
                }
                {
                    eventUpdate(event);
                }
                $('#calendar').fullCalendar('refetchEvents');
            }
        

        },//: eventUpdate,
        eventResize: eventUpdate,
        eventRender: function (event, element, view) {
            element.css({
                'color': 'black'
            });

            if (renderingComplete) {
                renderingComplete = false;
                totals = getBlankTotal();
                eventArray = [];
            }
            if (event.ReturnedJob == 1) {
                switch (event.CurrentStateName) {
                    case "Install in Progress":
                        element.css({
                            'background-color': '#a5d6a7'
                        });
                        break;
                    case "Installation Confirmed":
                        element.css({
                            'background-color': '#4890e2'
                        });
                        break;
                    case "Install Completed":
                        element.css({
                            'background-color': '#ffc966'
                        });
                        break;

                    case "Installation inprogress rejected":
                        element.css({
                            'background-color': '#ff6347'
                        });
                        break;
                    case "Installation Manager Review":
                        element.css({
                            'background-color': '#ffc966'
                        });
                        break;
                    case "Job Completed":
                        element.css({
                            'background-color': '#ffc966'
                        });
                        break;
                    case "Job Costing":
                        element.css({
                            'background-color': '#ffc966'
                        });
                        break;
                    case "Pending Install Completion":
                        element.css({
                            'background-color': '#ff6347'
                        });
                        break;

                    case "Ready for Invoicing":
                        element.css({
                            'background-color': '#ffc966'
                        });
                        break;
                    case "ReMeasure Scheduled":
                        element.css({
                            'background-color': '#9FB6CD'
                        });
                        break;
                    case "Rejected Installation":
                        element.css({
                            'background-color': '#ff6347'
                        });
                        break;
                    case "Rejected Job Costing":
                        element.css({
                            'background-color': '#ff6347'
                        });
                        break;

                    case "Rejected Manager Review":
                        element.css({
                            'background-color': '#ff6347'
                        });
                        break;
                    case "Rejected Remeasure":
                        element.css({
                            'background-color': '#ff6347'
                        });
                        break;
                    case "Rejected Scheduled Work":
                        element.css({
                            'background-color': '#ff6347'
                        });
                        break;
                    case "Unreviewed Job Costing":
                        element.css({
                            'background-color': '#ffc966'
                        });
                        break;

                    case "Unreviewed Work Scheduled":
                        element.css({
                            'background-color': '#9FB6CD'
                        });
                        break;
                    case "VP Installation Approval":
                        element.css({
                            'background-color': '#ffc966'
                        });
                        break;
                    case "Work Scheduled":
                        element.css({
                            'background-color': '#9FB6CD'
                        });
                        break;
                }
            
            }
            element.attr('href', 'javascript:void(0);');
            element.click(function () {
                if ((event.HolidayName != null) || (event.UnavailableStaff != null) ){
                    event.jsEvent.cancelBubble = true;
                    event.jsEvent.preventDefault();
                }

                $("#btnSave").attr("disabled", false); 
                $("#btnSaveLog").attr("disabled", false);
                $("#btnSaveNotes").attr("disabled", false); 
                if (displayType == "Installation") {
                    
                    element.attr('data-toggle', "modal");
                    element.attr('data-target', "#eventContent");
                    element.attr('href', "/details");

      
                    $("#FirstName").html(event.FirstName);
                    $("#LastName").html(event.LastName);
                    $("#postalCode").html(event.PostCode);
                    $("#workOrder").html(event.WorkOrderNumber);
                    $("#WorkOrderTitle").html(event.WorkOrderNumber);
                    if (event.HomeDepotJob == "Yes") {
                        $("#WorkOrderTitleHeader").attr("style", "background-color: #fa6304");
                        $("#homeDepotJob").html("Yes");
                    }
                    else {
                        $("#homeDepotJob").html("No");
                        $("#WorkOrderTitleHeader").attr("style", "background-color:#9FB6CD");
                    }
                    WO = event.WorkOrderNumber;

                    $("#ageOfHome").html(event.AgeOfHome);
                                  
                    $("#homePhone").html(event.HomePhoneNumber);
                    $("#workPhone").html(event.WorkPhoneNumber);
                    $("#cellPhone").html(event.CellPhone);

                    $("#email").html(event.Email);
                    $("#salesRep").html(event.SalesRep);

                    $("#City").html(event.City);

                    $("#TotalWindows1").html(event.TotalWindows);
                    $("#TotalDoors1").html(event.TotalDoors);
                    $("#TotalDoors2").html(event.TotalExtDoors);
                    
                    if (event.TotalAsbestos == 1) {
                        // 
                        $("#Asbestos-JobsYes").prop("checked", true);
                        $("#Asbestos-JobsNo").prop("checked", false);
                    }
                    else {
                        $("#Asbestos-JobsNo").prop("checked", true);
                        $("#Asbestos-JobsYes").prop("checked", false);
                    } 

                    if (event.LeadPaint == 'Yes') {
                        // 
                        $("#Lead-PaintYes").prop("checked", true);
                        $("#Lead-PaintNo").prop("checked", false);
                    }
                    else {
                        $("#Lead-PaintNo").prop("checked", true);
                        $("#Lead-PaintYes").prop("checked", false);
                    }

                    $("#wooddropTime").val('');
                    $("#wooddropdate").val('');
                    if (event.TotalWoodDropOff == 1) {
                        // 
                        $("#Wood-DropOff-JobsYes").prop("checked", true);
                        $("#Wood-DropOff-JobsNo").prop("checked", false);
                        $("#divWoodDropOffDate").show();
                      //  $("#wooddropdate").val(event.StrWoodDropOffDate.slice(0, -10));
                        $("#wooddropdate").val(event.StrWoodDropOffDate);
                        $("#wooddropTime").val(event.StrWoodDropOffTime);
                    }
                    else {
                        $("#Wood-DropOff-JobsNo").prop("checked", true);
                        $("#Wood-DropOff-JobsYes").prop("checked", false);
                        $("#divWoodDropOffDate").hide();
                    
                    }

                    if (event.TotalHighRisk == 1) {
                        // 
                        $("#HighRisk-JobsYes").prop("checked", true);
                        $("HighRisk-JobsNo").prop("checked", false); 
                    }
                    else {
                        $("#HighRisk-JobsNo").prop("checked", true);
                        $("#HighRisk-JobsYes").prop("checked", false);
                    }
                    $("#NumOfInstallers").val(event.EstInstallerCnt);

                   // 
                    //$("#Asbestos-Jobs1").html(event.TotalAsbestos == 1 ? "Yes" : "No");
                  //  $("#Lead-Paint1").html(event.LeadPaint);
                 //   $("#Wood-DropOff-Jobs1").html(event.TotalWoodDropOff == 1 ? "Yes" : "No");
                  //  $("#HighRisk-Jobs1").html(event.TotalHighRisk == 1 ? "Yes" : "No");

                    //document.getElementById("btnUpdateInstallationEventSchedule").disabled = (readonly == "True") ? true : false;
                    //document.getElementById("btnSunday").disabled = (readonly == "True") ? true : false;
                    //document.getElementById("btnReturnedJob").disabled = (readonly == "True") ? true : false;


                    $("#Address").html(event.StreetAddress);
                    $("#DaysOfWork").html(event.detailrecordCount);
                    $("#SalesAmmount").html(event.TotalSalesAmount.formatMoney(2, "$", ",", "."));
                    $("#DailySalesAmmount").html(event.SalesAmmount.formatMoney(2, "$", ",", "."));
                    $("#SeniorInstaller").html(event.SeniorInstaller != null && event.SeniorInstaller.trim().length > 0 ? event.SeniorInstaller : "Unspecified");
                    $("#CrewNames").html(event.CrewNames != null && event.CrewNames.trim().length > 0 ? event.CrewNames : "Un assigned");

                    codeAddress();
                       //$("#ReturnedJob").hide();
                    $("#from_date").val('');
                    $("#end_date").val('');
                    $("#returnJobReasonID").val('');

                    if (event.ReturnedJob == 1) {
                        // $("#ReturnedJob").show(); 
                        $("#from_date").val(new Date(GetDatefromMoment(event.start + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                        $("#end_date").val(new Date(GetDatefromMoment(event.end + 24 * 60 * 60000)).toLocaleDateString('en-US'));

                        $("#returnJobReasonID").val(event.ReturnTripReason);
                        if (event.end == null) {
                            $("#end_date").val(new Date(GetDatefromMoment(event.start + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                        }

                     
                        $("#InstallScheduledStartDate").prop("disabled", true);
                        $("#InstallScheduledStartTime").prop("disabled", true);

                        $("#InstallScheduledEndDate").prop("disabled", true);
                        $("#InstallScheduledEndTime").prop("disabled", true);

                        $("#InstallScheduledStartDate").val(new Date(GetDatefromMoment(event.StartScheduleDate + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                        $("#InstallScheduledEndDate").val(new Date(GetDatefromMoment(event.EndScheduleDate + 24 * 60 * 60000)).toLocaleDateString('en-US'));

                     
                        $("#NumOfInstallers").prop("disabled", true);

                        $("#Asbestos-JobsYes").prop("disabled", true);
                        $("#Asbestos-JobsNo").prop("disabled", true);

                        $("#Lead-PaintYes").prop("disabled", true);
                        $("#Lead-PaintNo").prop("disabled", true);

                        $("#Wood-DropOff-JobsYes").prop("disabled", true);
                        $("#Wood-DropOff-JobsNo").prop("disabled", true);

                        $("#HighRisk-JobsYes").prop("disabled", true);
                        $("#HighRisk-JobsNo").prop("disabled", true);

                        document.getElementsByName('saturday')[0].disabled = true;
                        document.getElementsByName('sunday')[0].disabled = true;

                        $("#from_date").prop("disabled", false);
                        $("#end_date").prop("disabled", false);
                        $("#returnJobReasonID").prop("disabled", false);

                       //etInstallationDateByWOForReturnedJob()

                       // $("#InstallScheduledStartDate").val(new Date(GetDatefromMoment(event.start + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                      //  $("#InstallScheduledEndDate").val(new Date(GetDatefromMoment(event.end + 24 * 60 * 60000)).toLocaleDateString('en-US'));

                    }
                    else {
               
                     //   $("#InstallScheduledStartDate").val(new Date(GetDatefromMoment(event.start + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                        //$("#InstallScheduledStartDate").val(new Date(GetDatefromMoment(event.start)).toLocaleDateString('en-US'));
                        $("#InstallScheduledStartDate").val(moment(event.start).add(0, 'hours').format('MM/DD/YYYY'));

                        $("#InstallScheduledStartTime").val(moment(event.start).add(0, 'hours').format('HH:mm'));

                    if (event.end == null) {
                       // $("#InstallScheduledEndDate").val(new Date(GetDatefromMoment(event.start + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                      //  $("#InstallScheduledEndDate").val(new Date(GetDatefromMoment(event.start )).toLocaleDateString('en-US'));
                        $("#InstallScheduledEndDate").val(moment(event.start).add(0, 'hours').format('MM/DD/YYYY'));
                    }
                    else {
                      //  $("#InstallScheduledEndDate").val(new Date(GetDatefromMoment(event.end + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                        //$("#InstallScheduledEndDate").val(new Date(GetDatefromMoment(event.end )).toLocaleDateString('en-US'));
                        $("#InstallScheduledEndDate").val(moment(event.end).add(-24, 'hours').format('MM/DD/YYYY'));
                        $("#InstallScheduledEndTime").val(moment(event.EndTime).add(0, 'hours').format('HH:mm'));
                    }
                        $("#InstallScheduledStartDate").prop("disabled", false);
                        $("#InstallScheduledEndDate").prop("disabled", false);

                        $("#InstallScheduledStartTime").prop("disabled", false);
                        $("#InstallScheduledEndTime").prop("disabled", false);

                        if (event.StartScheduleDate != null) {
                            $("#from_date").prop("disabled", true);
                            $("#end_date").prop("disabled", true);
                            $("#returnJobReasonID").prop("disabled", true);
                            $("#from_date").val(new Date(GetDatefromMoment(event.StartScheduleDate + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                            $("#end_date").val(new Date(GetDatefromMoment(event.EndScheduleDate + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                            $("#returnJobReasonID").val(event.ReturnTripReason);
                        }
                        else {
                            $("#from_date").prop("disabled", false);
                            $("#end_date").prop("disabled", false);
                            $("#returnJobReasonID").prop("disabled", false);
                        }
                        $("#NumOfInstallers").prop("disabled", false);

                        $("#Asbestos-JobsYes").prop("disabled", false);
                        $("#Asbestos-JobsNo").prop("disabled", false);

                        $("#Lead-PaintYes").prop("disabled", false);
                        $("#Lead-PaintNo").prop("disabled", false);

                        $("#Wood-DropOff-JobsYes").prop("disabled", false);
                        $("#Wood-DropOff-JobsNo").prop("disabled", false);

                        $("#HighRisk-JobsYes").prop("disabled", false);
                        $("#HighRisk-JobsNo").prop("disabled", false);

                        document.getElementsByName('saturday')[0].disabled = false;
                        document.getElementsByName('sunday')[0].disabled = false;

                      

                    //    $("#from_date").val('');
                    //    $("#end_date").val('');

                    }
                                  
                    eventid = event.id;
                  
                    if (event.Saturday == "Yes") {
                        document.getElementsByName('saturday')[0].checked = true;
                    }
                    else {
                        document.getElementsByName('saturday')[0].checked = false;
                    }
                    if (event.Sunday == "Yes") {
                        document.getElementsByName('sunday')[0].checked = true;
                    }
                    else {
                        document.getElementsByName('sunday')[0].checked = false;
                    }
                    
                    //retrieve product info
                   // var ss = GetNonReturnedJobDates(event.WorkOrderNumber);
                    //GetProducts(event.WorkOrderNumber);
                    GetInstallationProducts(event.WorkOrderNumber);
                    GetManufacturingProducts(event.WorkOrderNumber);
                    GetInstallers(event.WorkOrderNumber);
                    GetCalledLog(event.WorkOrderNumber);
                    GetNotes(event.WorkOrderNumber);
                    GetWOPicture(event.WorkOrderNumber);
                    GetJobReview(event.WorkOrderNumber);
                    GetDocumentLibrary(event.WorkOrderNumber);
                    GetSubTrades(event.WorkOrderNumber);
                    GetRemeasurerName(event.WorkOrderNumber);
                   // GetJobAnalysys(event.WorkOrderNumber);
                    $("#TotalLBRMin").html(event.TotalInstallationLBRMin);

                    $("#eventLink").attr('href', event.url);
                   // $("#eventContent").dialog({ modal: true, title: event.LastName, width: 900 });
                    //$("#eventContent").modal("show");

                }
                else if (displayType == "Remeasure") {

                    element.attr('data-toggle', "modal");
                    element.attr('data-target', "#eventRemeasureContent");
                    element.attr('href', "/details");
           
                    $("#FirstNameRemeasure").html(event.FirstName);
                    $("#LastNameRemeasure").html(event.LastName);
                    $("#postalCodeRemeasure").html(event.PostCode);
                    $("#workOrderRemeasure").html(event.WorkOrderNumber);
                    $("#WorkOrderTitleRemeasure").html(event.WorkOrderNumber);
                    
                                  
                    $("#homePhoneRemeasure").html(event.HomePhoneNumber);
                    $("#workPhoneRemeasure").html(event.WorkPhoneNumber);
                    $("#cellPhoneRemeasure").html(event.CellPhone);

                    $("#emailRemeasure").html(event.Email);
                    $("#salesRepRemeasure").html(event.SalesRep);

                    $("#CityRemeasure").html(event.City);

                    $("#TotalWindows1Remeasure").html(event.TotalWindows);
                    $("#TotalDoors1Remeasure").html(event.TotalDoors);
                    $("#TotalDoors2Remeasure").html(event.TotalExtDoors);


                    $("#AddressRemeasure").html(event.StreetAddress);
                    $("#SalesAmmountRemeasure").html(event.TotalSalesAmount.formatMoney(2, "$", ",", "."));
                    $("#RemeasureDate").val('');
                    //$("#RemeasureDate").val(new Date(GetDatefromMoment(event.RemeasureDate + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                //   $("#RemeasureDate").val( "2014-01-02T11:42");RemeasureDateFormat
                  //  $("#RemeasureDate").val(event.RemeasureDateFormat);
                    $("#RemeasureDate").val(event.RemeasureDateFormat.slice(0, -4));
                   

                    $("#EndRemeasureDate").val('');
                   // $("#EndRemeasureDate").val(event.EndRemeasureDateFormat.slice(0, -4));
                    $("#EndRemeasureDate").val(event.RemeasureEndTime);
                  
                    

                    codeAddressRemeasure();
                  //codeAddress();


                    eventid = event.id;

                  //  GetRemeasureProducts(event.WorkOrderNumber);

         

                    $("#eventLink").attr('href', event.url);
                   // $("#eventContent").dialog({ modal: true, title: event.LastName, width: 900 });
                    //$("#eventContent").modal("show");

                }
                else {
                    element.attr('data-toggle', "modal");
                    element.attr('data-target', "#eventContentWindows");
                    element.attr('href', "/details");

         
             
                    $("#WorkOrderTitleWindows").html(event.title);

                    GetWindowsCustomer(event.title.split(" ")[0]);
                    GetManufacturingWindowsProducts(event.title.split(" ")[0]);

                    eventid = event.id;

                    //  GetRemeasureProducts(event.WorkOrderNumber);



                    $("#eventLink").attr('href', event.url);
        
                }
              
                
            });

            if (event.JobType == "RES") {
                var myCss = $(element).attr('style');
                if (myCss !== undefined) {
                    myCss = myCss.replace('background-color:#999999;', '');
                    if (myCss == '') {
                        $(element).removeAttr('style');
                    } else {
                        $(element).attr('style', myCss);
                    }
                }
                element.addClass("reservation");
            }
            var dom = ""

            
            if (event.allDay) dom = 'span:first'; else {
                dom = '.fc-time';
                $(element).find(dom).empty();
            }

            if ((event.HolidayName != undefined) && (event.HolidayName != null)) {
                var ret = "<img src=\"images/holiday-icon.png\" title=\"" + "\">" +
                    "&nbsp;";
                if ((displayType == "Installation") || (displayType == "Remeasure")) {
                    ret += event.HolidayName;
                }
                element.css({
                    'background-color': '#c6abd0'
                    // 'border-color': '#333333'
                });
                $(element).find(dom).prepend(ret);
                
            }




            if (view.name == "agendaDay") {
                if ((displayType != "Installation") || (displayType != "Remeasure") ){
                    $(element).find(dom).text(element.text() + " - LBR Min: " + (event.TotalLBRMin !== undefined ? event.TotalLBRMin : 0) + ", Bundle: " + event.BatchNo);
                }
            }
            if (event.PaintIcon != undefined && event.PaintIcon != '' && event.PaintIcon == "Yes") {// Paint Icon
                $(element).find(dom).prepend("<img alt=\"#\" src=\"images/color.png\" />&nbsp;");
            }
            if (event.windows > 0) {// Windows

                $(element).find(dom).prepend("<img alt=\"#\" src=\"images/window.png\" title= \"" + ToWDString(event) + "\" />&nbsp;");
            }


            if ((event.UnavailableStaff != undefined) && (event.UnavailableStaff != null)) {
                //    (event.EstInstallerCnt != "" ? "<img src=\"images/installer" + event.EstInstallerCnt + ".png\" title=\"Estimated number of installers for the job: " + "\">": ""  ) +
                //var ret3 =
                //    //"<img src=\"images/human.png\" title=\"" + "\">" +"&nbsp;";
                //if (displayType == "Installation") {
                //    ret3 += event.UnavailableStaff;
                //}
                element.css({
                    'background-color': '#a5d6a7'
                    // 'border-color': '#333333'
                });
                //$(element).find(dom).append(ret3);
              //  (event.UnavailableStaff.split(",").length != 0? "<img src=\"images/installer" + event.UnavailableStaff.split(",").length + ".png\" title=\"Estimated number of installers for the job: " + "\">" : "") +
                dom = '.fc-title';
                $(element).find(dom).empty();

                var ret3 =
                    (event.UnavailableStaff.length != 0 ? "<img src=\"images/human.png\" title=\"Unavailable staff: " + event.UnavailableStaff + "\">" : "") +
                    //"<img src=\"images/human.png\" />" + "&nbsp;" +
                  //  (event.UnavailableStaff.split(",").length != 0 ? "<img src=\"images/human.png\" title=\"Unavailable staff: " + event.UnavailableStaff +  "\">" : "") +

                    //   "<img src=\"images/installer" + event.UnavailableStaff.split(",").length + ".png\" title=\"" +
                 //   (event.UnavailableStaff.split(",").length != 0 ? "<img src=\"images/installer" + event.UnavailableStaff.split(",").length + ".png\" title=\"" + "\">" : "") +
                    (" " + event.UnavailableStaff );
                 //   (event.UnavailableStaff.split(",").length != 0 ? " " + event.UnavailableStaff.split(",")[0] + "(" + event.UnavailableStaff.split(",").length + ")" : "")
                  //  (event.UnavailableStaff.length != 0 ? " " + event.UnavailableStaff + "(" + event.UnavailableStaff.split(",").length + ")" : "")
                $(element).find(dom).prepend(ret3);

            }

            if ((displayType == "Installation") && (event.HolidayName == null) && (event.UnavailableStaff == null) ) {
                
                //GetSubTradesCount(event.WorkOrderNumber);

                dom = '.fc-title';
                $(element).find(dom).empty();


                var ret2 =
                    (event.EstInstallerCnt != "" ? "<img src=\"images/installer" + event.EstInstallerCnt + ".png\" title=\"Estimated number of installers for the job: " + event.EstInstallerCnt + "\">" : "") +
                    //"<img src=\"images/installer" + event.EstInstallerCnt + ".png\" title=\"Estimated number of installers for the job: " +
                   // event.EstInstallerCnt + "\">" +
                    (event.TotalWindows != "0" ? "&nbsp;<img title=\"# of Windows: " + event.TotalWindows + "\" src=\"images/window.PNG\" />" : "") +
                    (event.TotalDoors != "0" ? "&nbsp;<img title=\"# of Patio Doors: " + event.TotalDoors + "\" src=\"images/window.PNG\" />" : "") + "&nbsp;" +
                    (event.TotalExtDoors != "0" ? "&nbsp;<img title=\"# of Codel Doors: " + event.TotalExtDoors + "\" src=\"images/door.PNG\" />" : "") + "&nbsp;" +
                    (event.TotalWoodDropOff == 1 ? "&nbsp;<img src=\"images/delivery.PNG\" />" : "") +
                    //(event.TotalAsbestos == 1 ? "&nbsp;<img src=\"images/asbestos.PNG\" />" : "") +
                    ((event.TotalAsbestos == 1) || (event.LeadPaint == 'Yes') ? "&nbsp;<img src=\"images/asbestos.PNG\" />" : "") +
                    (event.HomeDepotJob == "Yes" ? "&nbsp;<img src=\"images/HD.JPG\" />" : "") +
                    (event.TotalHighRisk == 1 ? "&nbsp;<img src=\"images/risk.PNG\" />" : "") +
                 //   ((event.SubTradeFlag.length > 0) && (event.SubTradeFlag>0) ? "&nbsp;<img src=\"images/subtrade.PNG\" />" : "") +

                    (event.SubTradeFlag == 1  ? "&nbsp;<img src=\"images/subtrade.PNG\" />" : "") +
                    (event.ReturnedJob == 1 ? "&nbsp;<img src=\"images/fire.PNG\" />" : "") +
                    (" " + event.WorkOrderNumber) + "&nbsp;" +
                    (",Name: " + event.LastName.trim().length > 10 ? event.LastName.trim().Substring(0, 10) : event.LastName.trim()) +
                    //   + event.FirstName.trim().length > 10 ? event.FirstName.trim().Substring(0, 10) : event.FirstName.trim()) +
                    "&nbsp;" + (event.City) + "&nbsp;";
                //  ("WO: ") + "&nbsp;" ;

                $(element).find(dom).append(ret2);

            }
            else if ((displayType == "Remeasure") && (event.HolidayName == null)) {

                dom = '.fc-title';
                $(element).find(dom).empty();

                var endTime ;
                
                if (event.RemeasureEndTime == undefined) {
                    endTime = "";
                }
                else if (event.RemeasureEndTime.length == 0) {
                    endTime = "";
                }
                else {
                    endTime = " to " + tConv24(event.RemeasureEndTime);
                }


                var ret1 =
                    //"<img src=\"images/installer" + event.EstInstallerCnt + ".png\" title=\"Estimated number of installers for the job: " +
                    //event.EstInstallerCnt + "\">" +
                    (event.EstInstallerCnt != "" ? "<img src=\"images/installer" + event.EstInstallerCnt + ".png\" title=\"Estimated number of installers for the job: " + event.EstInstallerCnt + "\">" : "") +
                    (event.RemeasureDate != "" ? "&nbsp;<img title=\"Remeasure Date & Time: " + formatDate(new Date(GetDatefromMoment(event.RemeasureDate + 24 * 60 * 60000))) + " " +
                   //     (event.RemeasureEndTime != undefined && event.RemeasureEndTime.length ? " to " + event.RemeasureEndTime:"") + "\" src=\"images/timer.PNG\" />" : "") + "&nbsp;" +
                        endTime + "\" src=\"images/timer.PNG\" />" : "") + "&nbsp;" +
                    (event.TotalWindows != "0" ? "&nbsp;<img title=\"# of Windows: " + event.TotalWindows + "\" src=\"images/window.PNG\" />" : "") +
                 
                    (event.TotalDoors != "0" ? "&nbsp;<img title=\"# of Patio Doors: " + event.TotalDoors + "\" src=\"images/window.PNG\" />" : "") + "&nbsp;" +
                  //  (formatDate(new Date(GetDatefromMoment(event.RemeasureDate + 24 * 60 * 60000)))) + "&nbsp;" +
                   // (" " + new Date(GetDatefromMoment(event.RemeasureDate + 24 * 60 * 60000)).toLocaleDateString('en-US') +  "&nbsp;" +
                    (" " + event.WorkOrderNumber) + "&nbsp;" +
                    (",Name: " + event.LastName.trim().length > 10 ? event.LastName.trim().Substring(0, 10) : event.LastName.trim()) +
                    "&nbsp;" + (event.City) + "&nbsp;";


                $(element).find(dom).append(ret1);

            }


            if (event.F52PD > 0) // Patio Doors
                $(element).find(dom).prepend("<img alt=\"#\" src=\"images/patiodoor.png\" title= \"" + ToPDString(event) + "\" />&nbsp;");

            if (event.doors > 0) // Doors
                $(element).find(dom).prepend("<img alt=\"#\" src=\"images/door.png\" title= \"" + ToDString(event) + "\" />&nbsp;");

            if (event.CustomFlag > 0) // Custom
                $(element).find(dom).prepend("<img alt=\"#\" src=\"images/custom.png\" />&nbsp;");

            if (event.HighRiskFlag > 0) // High Risk
                $(element).find(dom).prepend("<img alt=\"#\" src=\"images/risk.png\" />&nbsp;");

            if (event.FlagOrder != undefined && event.FlagOrder == 1) // Rush Order
                $(element).find(dom).prepend("<img alt=\"#\" src=\"images/flag.png\" />&nbsp;");

            if (event.M2000Icon != undefined && event.M2000Icon == 1) // M2000
                $(element).find(dom).prepend("<img alt=\"#\" src=\"images/M2000.png\" />&nbsp;");

            //var dayId = FindDayIdfromTotals(GetDatefromMoment(event.start));
          
            var dayId;
            if ((displayType == "Installation") || (displayType == "Remeasure")){
              dayId = FindDayIdfromTotals(GetDatefromMoment(event.ScheduledDate));
            }
            else
            {
              dayId = FindDayIdfromTotals(GetDatefromMoment(event.start));
              //  dayId = FindDayIdfromTotals(GetDatefromMoment(event.ScheduledProductionDate));
            }
                
            if (totals.length < dayId) console.log("eventRender", "dayId exceeds totals", dayId, totals.length);

            
            if ((displayType == "Installation") && (event.HolidayName != null)) {

            }
            else if ((displayType == "Remeasure") && (event.HolidayName != null)) {

            }
            else {
                //totals[dayId].doors += event.doors !== undefined ? event.doors : 0;
                //totals[dayId].windows += event.windows !== undefined ? event.windows : 0;
                //totals[dayId].F52PD += event.F52PD !== undefined ? event.F52PD : 0;

                var diff;
               // diff = parseInt((event.end - event.start) / (24 * 3600 * 1000));

                if (event.end != null) {
                    diff = moment(event.end._i, 'M/D/YYYY').diff(moment(event.start._i, 'M/D/YYYY'), 'days')-1;
                    if (diff > 0) {
                        //if (view.name !== "agendaMonth" && view.name !== "month") {
                        //    diff = diff + 1;
                        //}updateStatus
                        if (updateStatus==1) {
                            diff = diff + 1;
                        }

                        totals[dayId].doors += event.doors !== undefined ? event.doors / (diff+1) : 0;
                        totals[dayId].windows += event.windows !== undefined ? event.windows / (diff+1) : 0;
                        totals[dayId].F52PD += event.F52PD !== undefined ? event.F52PD / (diff + 1) : 0;

                        totals[dayId].boxes += event.TotalBoxQty !== undefined ? event.TotalBoxQty / (diff + 1) : 0;
                        totals[dayId].glass += event.TotalGlassQty !== undefined ? event.TotalGlassQty / (diff + 1) : 0;
                        totals[dayId].min += event.TotalLBRMin !== undefined ? event.TotalLBRMin / (diff + 1) : 0;
                        // Windows
                        totals[dayId].F6CA += event.F6CA !== undefined ? event.F6CA / (diff + 1) : 0;
                        totals[dayId].F68VS += event.F68VS !== undefined ? event.F68VS / (diff + 1) : 0;
                        totals[dayId].F68VSMin += event.F68VSMin !== undefined ? event.F68VSMin / (diff + 1) : 0;

                        totals[dayId].F68SL += event.F68SL !== undefined ? event.F68SL / (diff + 1) : 0;
                        totals[dayId].F68SLMin += event.F68SLMin !== undefined ? event.F68SLMin / (diff + 1) : 0;

                        totals[dayId].F29CMMin += event.F29CMMin !== undefined ? event.F29CMMin / (diff + 1) : 0;
                        totals[dayId].F29CAMin += event.F29CAMin !== undefined ? event.F29CAMin / (diff + 1) : 0;

                        totals[dayId].F26CAMin += event.F26CAMin !== undefined ? event.F26CAMin / (diff + 1) : 0;
                        totals[dayId].F27DSMin += event.F27DSMin !== undefined ? event.F27DSMin / (diff + 1) : 0;

                        totals[dayId].F68CA += event.F68CA !== undefined ? event.F68CA / (diff + 1) : 0;

                        totals[dayId].F29CM += event.F29CM !== undefined ? event.F29CM / (diff + 1): 0;
                        totals[dayId].F29CA += event.F29CA !== undefined ? event.F29CA / (diff + 1): 0;
                        totals[dayId].F27TT += event.F27TT !== undefined ? event.F27TT / (diff + 1) : 0;
                        totals[dayId].F27TS += event.F27TS !== undefined ? event.F27TS / (diff + 1) : 0;
                        totals[dayId].F27DS += event.F27DS !== undefined ? event.F27DS / (diff + 1): 0;


                        // Doors
                        totals[dayId].Transom += event.Transom !== undefined ? event.Transom / (diff + 1) : 0;
                        totals[dayId].Sidelite += event.Sidelite !== undefined ? event.Sidelite / (diff + 1) : 0;
                        totals[dayId].SingleDoor += event.SingleDoor !== undefined ? event.SingleDoor / (diff + 1) : 0;
                        totals[dayId].DoubleDoor += event.DoubleDoor !== undefined ? event.DoubleDoor / (diff + 1) : 0;

                        totals[dayId].value += event.TotalPrice !== undefined ? event.TotalPrice / (diff + 1) : 0;

                        eventDiff.push(event);
                        eventDiff = removeDuplicates(eventDiff, "title");
                    }
                    else {
                        totals[dayId].doors += event.doors !== undefined ? event.doors : 0;
                        totals[dayId].windows += event.windows !== undefined ? event.windows : 0;
                        totals[dayId].F52PD += event.F52PD !== undefined ? event.F52PD : 0;

                        totals[dayId].boxes += event.TotalBoxQty !== undefined ? event.TotalBoxQty : 0;
                        totals[dayId].glass += event.TotalGlassQty !== undefined ? event.TotalGlassQty : 0;
                        totals[dayId].min += event.TotalLBRMin !== undefined ? event.TotalLBRMin : 0;
                        // Windows
                        totals[dayId].F6CA += event.F6CA !== undefined ? event.F6CA : 0;
                        totals[dayId].F68VS += event.F68VS !== undefined ? event.F68VS : 0;
                        totals[dayId].F68VSMin += event.F68VSMin !== undefined ? event.F68VSMin : 0;

                        totals[dayId].F68SL += event.F68SL !== undefined ? event.F68SL : 0;
                        totals[dayId].F68SLMin += event.F68SLMin !== undefined ? event.F68SLMin : 0;

                        totals[dayId].F29CMMin += event.F29CMMin !== undefined ? event.F29CMMin : 0;
                        totals[dayId].F29CAMin += event.F29CAMin !== undefined ? event.F29CAMin : 0;
                        totals[dayId].F26CAMin += event.F26CAMin !== undefined ? event.F26CAMin : 0;
                        totals[dayId].F27DSMin += event.F27DSMin !== undefined ? event.F27DSMin : 0;



                        totals[dayId].F68CA += event.F68CA !== undefined ? event.F68CA : 0;

                        totals[dayId].F29CM += event.F29CM !== undefined ? event.F29CM : 0;
                        totals[dayId].F29CA += event.F29CA !== undefined ? event.F29CA : 0;
                        totals[dayId].F27TT += event.F27TT !== undefined ? event.F27TT : 0;
                        totals[dayId].F27TS += event.F27TS !== undefined ? event.F27TS : 0;
                        totals[dayId].F27DS += event.F27DS !== undefined ? event.F27DS : 0;


                        // Doors
                        totals[dayId].Transom += event.Transom !== undefined ? event.Transom : 0;
                        totals[dayId].Sidelite += event.Sidelite !== undefined ? event.Sidelite : 0;
                        totals[dayId].SingleDoor += event.SingleDoor !== undefined ? event.SingleDoor : 0;
                        totals[dayId].DoubleDoor += event.DoubleDoor !== undefined ? event.DoubleDoor : 0;

                        totals[dayId].value += event.TotalPrice !== undefined ? event.TotalPrice : 0;

                    }
                }
                else {
                    totals[dayId].doors += event.doors !== undefined ? event.doors : 0;
                    totals[dayId].windows += event.windows !== undefined ? event.windows : 0;
                    totals[dayId].F52PD += event.F52PD !== undefined ? event.F52PD : 0;

                    totals[dayId].boxes += event.TotalBoxQty !== undefined ? event.TotalBoxQty : 0;
                    totals[dayId].glass += event.TotalGlassQty !== undefined ? event.TotalGlassQty : 0;
                    totals[dayId].min += event.TotalLBRMin !== undefined ? event.TotalLBRMin : 0;
                    // Windows
                    totals[dayId].F6CA += event.F6CA !== undefined ? event.F6CA : 0;
                    totals[dayId].F68VS += event.F68VS !== undefined ? event.F68VS : 0;
                    totals[dayId].F68VSMin += event.F68VSMin !== undefined ? event.F68VSMin : 0;

                    totals[dayId].F68SL += event.F68SL !== undefined ? event.F68SL : 0;
                    totals[dayId].F68SLMin += event.F68SLMin !== undefined ? event.F68SLMin : 0;

                    totals[dayId].F68CA += event.F68CA !== undefined ? event.F68CA : 0;

                    totals[dayId].F29CM += event.F29CM !== undefined ? event.F29CM : 0;
                    totals[dayId].F29CA += event.F29CA !== undefined ? event.F29CA : 0;
                    totals[dayId].F27TT += event.F27TT !== undefined ? event.F27TT : 0;
                    totals[dayId].F27TS += event.F27TS !== undefined ? event.F27TS : 0;
                    totals[dayId].F27DS += event.F27DS !== undefined ? event.F27DS : 0;

                    totals[dayId].F29CMMin += event.F29CMMin !== undefined ? event.F29CMMin : 0;
                    totals[dayId].F29CAMin += event.F29CAMin !== undefined ? event.F29CAMin : 0;
                    totals[dayId].F26CAMin += event.F26CAMin !== undefined ? event.F26CAMin : 0;
                    totals[dayId].F27DSMin += event.F27DSMin !== undefined ? event.F27DSMin : 0;


                    // Doors
                    totals[dayId].Transom += event.Transom !== undefined ? event.Transom : 0;
                    totals[dayId].Sidelite += event.Sidelite !== undefined ? event.Sidelite : 0;
                    totals[dayId].SingleDoor += event.SingleDoor !== undefined ? event.SingleDoor : 0;
                    totals[dayId].DoubleDoor += event.DoubleDoor !== undefined ? event.DoubleDoor : 0;

                    totals[dayId].value += event.TotalPrice !== undefined ? event.TotalPrice : 0;
                }
            }

            totals[dayId].rush += event.FlagOrder !== undefined ? event.FlagOrder : 0;


        
        

            // Window Info
            //totals[dayId].Complex += event.Complex !== undefined ? event.Complex : 0;
            //totals[dayId].Simple += event.Simple !== undefined ? event.Simple : 0;
            //totals[dayId].Over_Size += event.Over_Size !== undefined ? event.Over_Size : 0;
            //totals[dayId].Arches += event.Arches !== undefined ? event.Arches : 0;
            //totals[dayId].Rakes += event.Rakes !== undefined ? event.Rakes : 0;
            //totals[dayId].Customs += event.Customs !== undefined ? event.Customs : 0;
            //console.log(dayId, "totals[dayId].Transom:", totals[dayId].Transom, "totals[dayId].Sidelite:", totals[dayId].Sidelite, "totals[dayId].SingleDoor:", totals[dayId].SingleDoor, "totals[dayId].DoubleDoor:", totals[dayId].DoubleDoor, "event.Transom:", event.Transom, "event.Sidelite:", event.Sidelite, "event.SingleDoor:", event.SingleDoor, "event.DoubleDoor:", event.DoubleDoor);
            //}
        },
        eventDragStop: function (event, jsEvent, ui, view) {
            if (debug) console.log("eventDragStop", event);
            if (event.ReturnedJob == 1) {
                alert("This is returned job!");
            }
            //if (event.)
            //totals = getBlankTotal();
        },
        eventResizeStop: function (event, jsEvent, ui, view) {
            if (debug) console.log("eventResizeStop", event);
            //totals = getBlankTotal();
        },
        eventReceive: function (event, delta, revertFunc) {
            if (debug) console.log('eventReceived', 'event.title=' + event.title, "id: " + event.id, "doors: " + event.doors, "PaintIcon: + " + event.PaintIcon);
            totals[event.start._d.getUTCDay()].doors += event.doors !== 'undefined' ? event.doors : 0;
            totals[event.start._d.getUTCDay()].windows += event.windows !== 'undefined' ? event.windows : 0;

            if ((displayType != "Installation") && (displayType != "Remeasure")) {

                var eventDate = GetDatefromMoment(event.start);
                var maxDayLabour = parseInt(FindByValue("max", eventDate).Value);
                var allocatedDayLabour = GetDayLabourValue($('#calendar').fullCalendar('getView'), eventDate) - event.TotalLBRMin;

              //  console.log("eventReceive", "allocated min:", allocatedDayLabour, "max labour:", maxDayLabour);
                //if (allocatedDayLabour + event.TotalLBRMin <= maxDayLabour) {
                //    sendUpdateToServer(event);
                //} else {
                //    ShowWarning(allocatedDayLabour, event.TotalLBRMin, maxDayLabour);
                //    $('#calendar').fullCalendar('removeEvents', event._id);
                //    AddBufferEvent(0, event);
                //}
                sendUpdateToServer(event);
            }
            else {
                sendUpdateToServer(event);
            }


        },
        drop: function (date, jsEvent, ui, resourceId) {
            $(this).remove();
        },
        eventAfterAllRender: function (view) {
            if (debug) console.log("eventAfterAllRender", "view.start:", view.start.format(), "view.intervalStart:", view.intervalStart.format(), "view.end:", view.end.format(), "view.intervalEnd:", view.intervalEnd.format());
            renderingComplete = true;
            if (view.name !== "agendaMonth" && view.name !== "month" && view.name !=="timelineDay") {
                if (debug) console.log("eventAfterAllRender", "view configuration:", view.title, view.intervalStart._d);
                var startDate = view.start._d;
                var endDate = view.end._d;
              
                var doors;
                var windows;
              
                var results;
               
                var date1, date2;
                var WOCount;
                var xDate, xDateSat, xDateSun;

                if (displayType == "Installation")  {
                    for (var i = 0; i < totals.length; i++) {
                        date1 = new Date(totals[i]["date"]).toLocaleDateString('en-US');
                        WOCount = 0; //ReturnedJob
                        for (var j = 0; j < eventWODict.length; j++) {
                            date2 = new Date(GetDatefromMoment(eventWODict[j]["ScheduledDate"])).toLocaleDateString('en-US');
                            if ((date1 == date2) && (eventWODict[j].ReturnedJob != 1)) {
                                totals[i].windows += eventWODict[j]["Windows"];
                                totals[i].doors += eventWODict[j]["Doors"];
                                totals[i].ExtDoors += eventWODict[j]["ExtDoors"];
                                totals[i].SalesAmmount += eventWODict[j]["SalesAmmount"];
                                totals[i].TotalAsbestos += eventWODict[j]["TotalAsbestos"];
                                totals[i].TotalWoodDropOff += eventWODict[j]["TotalWoodDropOff"];
                                totals[i].TotalHighRisk += eventWODict[j]["TotalHighRisk"];

                                totals[i].installationwindowLBRMIN += eventWODict[j]["installationwindowLBRMIN"];
                                totals[i].InstallationDoorLBRMin += eventWODict[j]["InstallationDoorLBRMin"];
                                totals[i].InstallationPatioDoorLBRMin += eventWODict[j]["InstallationPatioDoorLBRMin"];
                                totals[i].TotalInstallationLBRMin += eventWODict[j]["TotalInstallationLBRMin"];

                                totals[i].subinstallationwindowLBRMIN += eventWODict[j]["subinstallationwindowLBRMIN"];
                                totals[i].subInstallationPatioDoorLBRMin += eventWODict[j]["subInstallationPatioDoorLBRMin"];
                                totals[i].subExtDoorLBRMIN += eventWODict[j]["subExtDoorLBRMIN"];
                                totals[i].subTotalInstallationLBRMin += eventWODict[j]["subTotalInstallationLBRMin"];

                                totals[i].SidingLBRBudget += eventWODict[j]["SidingLBRBudget"];
                                totals[i].SidingLBRMin += eventWODict[j]["SidingLBRMin"];
                                totals[i].SidingSQF += eventWODict[j]["SidingSQF"];
                                totals[i].MinAvailable += eventWODict[j]["MinAvailable"];
                                totals[i].SalesTarget += eventWODict[j]["SalesTarget"];
                                totals[i].HazardousBudgetedLBR += eventWODict[j]["HazardousBudgetedLBR"];

                                if (eventWODict[j]["LeadPaint"] == "Yes") {
                                    totals[i].TotalLeadPaint++;
                                }

                                totals[i].WOCount = WOCount + 1;
                                WOCount++;
                                // if eventWODict[j].Saturday == "No"
                                //xDate = is_weekend(date2);
                                //xDateSat = is_saturday(date2);
                                //xDateSun = is_sunday(date2);
                                //if ((eventWODict[j].Sunday == "No" && eventWODict[j].Saturday == "No") && (xDate == "weekend")) {

                                //}
                                //else if ((eventWODict[j].Sunday == "Yes" && eventWODict[j].Saturday == "No") && (xDateSat == "saturday")) {

                                //}
                                //else if ((eventWODict[j].Sunday == "No" && eventWODict[j].Saturday == "Yes") && (xDateSun == "sunday")) {

                                //}
                                //else {
                                //    totals[i].windows += eventWODict[j]["Windows"];
                                //    totals[i].doors += eventWODict[j]["Doors"];
                                //    totals[i].ExtDoors += eventWODict[j]["ExtDoors"];
                                //    totals[i].SalesAmmount += eventWODict[j]["SalesAmmount"];
                                //    totals[i].TotalAsbestos += eventWODict[j]["TotalAsbestos"];
                                //    totals[i].TotalWoodDropOff += eventWODict[j]["TotalWoodDropOff"];
                                //    totals[i].TotalHighRisk += eventWODict[j]["TotalHighRisk"];

                                //    totals[i].installationwindowLBRMIN += eventWODict[j]["installationwindowLBRMIN"];
                                //    totals[i].InstallationDoorLBRMin += eventWODict[j]["InstallationDoorLBRMin"];
                                //    totals[i].InstallationPatioDoorLBRMin += eventWODict[j]["InstallationPatioDoorLBRMin"];
                                //    totals[i].TotalInstallationLBRMin += eventWODict[j]["TotalInstallationLBRMin"];

                                //    totals[i].subinstallationwindowLBRMIN += eventWODict[j]["subinstallationwindowLBRMIN"];
                                //    totals[i].subInstallationPatioDoorLBRMin += eventWODict[j]["subInstallationPatioDoorLBRMin"];
                                //    totals[i].subExtDoorLBRMIN += eventWODict[j]["subExtDoorLBRMIN"];
                                //    totals[i].subTotalInstallationLBRMin += eventWODict[j]["subTotalInstallationLBRMin"];

                                //    totals[i].SidingLBRBudget += eventWODict[j]["SidingLBRBudget"];
                                //    totals[i].SidingLBRMin += eventWODict[j]["SidingLBRMin"];
                                //    totals[i].SidingSQF += eventWODict[j]["SidingSQF"];

                                //    if (eventWODict[j]["LeadPaint"] == "Yes") {
                                //        totals[i].TotalLeadPaint++;
                                //    }

                                //    totals[i].WOCount = WOCount + 1;
                                //    WOCount++;
                                //}

                            }

                        }
                    }
                }
                else if (displayType == "Remeasure") {
                    for (var i = 0; i < totals.length; i++) {
                        date1 = new Date(totals[i]["date"]).toLocaleDateString('en-US');
                        WOCount = 0; //ReturnedJob
                        for (var j = 0; j < eventRemeasureWODict.length; j++) {
                            date2 = new Date(GetDatefromMoment(eventRemeasureWODict[j]["start"])).toLocaleDateString('en-US');
                            if (date1 == date2) {
                                // if eventWODict[j].Saturday == "No"
                                xDate = is_weekend(date2);
                                xDateSat = is_saturday(date2);
                                xDateSun = is_sunday(date2);
                                if ((eventRemeasureWODict[j].Sunday == "No" && eventRemeasureWODict[j].Saturday == "No") && (xDate == "weekend")) {

                                }
                                else if ((eventRemeasureWODict[j].Sunday == "Yes" && eventRemeasureWODict[j].Saturday == "No") && (xDateSat == "saturday")) {

                                }
                                else if ((eventRemeasureWODict[j].Sunday == "No" && eventRemeasureWODict[j].Saturday == "Yes") && (xDateSun == "sunday")) {

                                }
                                else {
                                    totals[i].windows += eventRemeasureWODict[j]["Windows"];
                                    totals[i].doors += eventRemeasureWODict[j]["Doors"];
                                    totals[i].ExtDoors += eventRemeasureWODict[j]["ExtDoors"];
                                    totals[i].SalesAmmount += eventRemeasureWODict[j]["SalesAmmount"];


                                    totals[i].installationwindowLBRMIN += eventRemeasureWODict[j]["installationwindowLBRMIN"];
                                    totals[i].InstallationDoorLBRMin += eventRemeasureWODict[j]["InstallationDoorLBRMin"];
                                    totals[i].InstallationPatioDoorLBRMin += eventRemeasureWODict[j]["InstallationPatioDoorLBRMin"];
                                    totals[i].TotalInstallationLBRMin += eventRemeasureWODict[j]["TotalInstallationLBRMin"];

                                    totals[i].subinstallationwindowLBRMIN += eventRemeasureWODict[j]["subinstallationwindowLBRMIN"];
                                    totals[i].subInstallationPatioDoorLBRMin += eventRemeasureWODict[j]["subInstallationPatioDoorLBRMin"];
                                    totals[i].subExtDoorLBRMIN += eventRemeasureWODict[j]["subExtDoorLBRMIN"];
                                    totals[i].subTotalInstallationLBRMin += eventRemeasureWODict[j]["subTotalInstallationLBRMin"];

                              
                                    totals[i].SidingLBRMin += eventRemeasureWODict[j]["SidingLBRMin"];
                                    totals[i].SidingSQF += eventRemeasureWODict[j]["SidingSQF"];
                        


                                    totals[i].WOCount = WOCount + 1;
                                    WOCount++;
                                }

                            }

                        }
                    }
                }
                else {
                    //for (var i = 0; i < totals.length; i++) {
                    ////}
                    //date1 = new Date(event.start).toLocaleDateString('en-US');
                    //date1 = new Date(event.end).toLocaleDateString('en-US');
                    //var dayId = FindDayIdfromTotals(GetDatefromMoment(event.start));
                    var diff,dayId;
                    var len = eventDiff.length;

                    for (var k = 0;k <eventDiff.length; k++) {
                        diff = moment(eventDiff[k].end._i, 'M/D/YYYY').diff(moment(eventDiff[k].start._i, 'M/D/YYYY'), 'days') - 1;
                        //if view is not monthview, then 
                        //if (view.name !== "agendaMonth" && view.name !== "month") {
                        //    diff = diff + 1;
                        //}
                        //if (updateStatus == 1) {
                        //    diff = diff + 1;
                        //}
                        
                        for (var i = 0; i < diff; i++) {
                            dayId = FindDayIdfromTotals(GetDatefromMoment(eventDiff[k].start));
                            totals[i + dayId+1].doors = totals[dayId].doors;
                            totals[i + dayId+1].windows = totals[dayId].windows;
                            totals[i + dayId + 1].F52PD = totals[dayId].F52PD;

                            totals[i + dayId + 1].boxes = totals[dayId].boxes;
                            totals[i + dayId + 1].glass = totals[dayId].glass;
                            totals[i + dayId + 1].value = totals[dayId].value;
                            totals[i + dayId + 1].rush = totals[dayId].rush;
                            totals[i + dayId + 1].min = totals[dayId].min;
                            // Windows
                            totals[i + dayId + 1].F6CA = totals[dayId].F6CA;
                            totals[i + dayId + 1].F68VS = totals[dayId].F68VS;
                            totals[i + dayId + 1].F68VSMin = totals[dayId].F68VSMin;

                            totals[i + dayId + 1].F68SL = totals[dayId].F68SL;
                            totals[i + dayId + 1].F68SLMin = totals[dayId].F68SLMin;

                            totals[i + dayId + 1].F29CMMin = totals[dayId].F29CMMin;
                            totals[i + dayId + 1].F29CAMin = totals[dayId].F29CAMin;
                            totals[i + dayId + 1].F26CAMin = totals[dayId].F26CAMin;
                            totals[i + dayId + 1].F27DSMin = totals[dayId].F27DSMin;

                            totals[i + dayId + 1].F68CA = totals[dayId].F68CA;

                            totals[i + dayId + 1].F29CM = totals[dayId].F29CM;
                            totals[i + dayId + 1].F29CA = totals[dayId].F29CA;
                            totals[i + dayId + 1].F27TT = totals[dayId].F27TT;
                            totals[i + dayId + 1].F27TS = totals[dayId].F27TS;
                            totals[i + dayId + 1].F27DS = totals[dayId].F27DS;
                            // Doors
                            totals[i + dayId + 1].Transom = totals[dayId].Transom;
                            totals[i + dayId + 1].Sidelite = totals[dayId].Sidelite;
                            totals[i + dayId + 1].SingleDoor = totals[dayId].SingleDoor;
                            totals[i + dayId + 1].DoubleDoor = totals[dayId].DoubleDoor;

                        }

                    }
                 
                 
                 
                }

                for (var i = 0; i < totals.length; i++) {
                    SetDayValue(i, totals[i]);
                }
                updateStatus = 0;
                eventDiff = [];
              //  eventWODict = [];
            }
        },

        //dateToCellOffset: function (date) {
        //    var offsets = this.dayToCellOffsets;
        //    var day = date.diff(this.start, 'days');

        //    if (day < 0) {
        //        return offsets[0] - 1;
        //    }
        //    else if (day >= offsets.length) {
        //        return offsets[offsets.length - 1] + 1;
        //    }
        //    else {agendaWeek

        //        return offsets[day];
        //    }
        //},
        viewRender: function (view, element) {
            if (debug) console.log("viewRender", "view configuration:", view.title, view.intervalStart._d);
            totals = getBlankTotal();
            ControlHeaderVisibility(GetDisplayItemList(displayType));
            if (view.type == 'agendaWeek') {
                $('#calendar').fullCalendar('refetchEvents');
                $('#calendar').fullCalendar('changeView', 'agendaWeek');
            }
            else if (view.type == 'timelineDay') {
                $('#calendar').fullCalendar('changeView', 'timelineDay');
            }
            else if (view.type == 'agendaDay') {
                $('#calendar').fullCalendar('changeView', 'agendaDay');
            }
            
                   
        }
    });


    //$('#calendar').addResource({
    //    id: 'e',
    //    title: 'Room E'
    //}); 
  
});

 function formatDate(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear() + "  " + strTime;
}


function formatTime(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return  strTime;
}


function changeJobStaff() {
    //if (document.getElementById("leave").value != "100") {
    //    document.getElementById("message").innerHTML = "Common message";
    //}
    //else {
    //    document.getElementById("message").innerHTML = "Having a Baby!!";
    //}
    var jobStaff = $("#JobStaff").val();
    if (jobStaff == "Job") {
        JobStaffEvent = "Job";
    }
    else {
        JobStaffEvent = "Staff";
    }
    $('#calendar').fullCalendar('refetchEvents');
}
function FindDayIdfromTotals(dayValue) {
    for (var i = 0; i < totals.length; i++) {
        //console.log("FindDayIdfromTotals", "target day:", dayValue, "current date:", totals[i].date, "equals:", dayValue.Equals(totals[i].date));
        if (dayValue.Equals(totals[i].date)) {
            //console.log("FindDayIdfromTotals", "target day:", dayValue, "current date:", totals[i].date, "equals:", dayValue.Equals(totals[i].date), "returning", i);
            return i;
        }
    }
    return 0;
}
function GetDayLabourValue(view, pDate) {
    var dayId = FindDayIdfromTotals(pDate);
    var retValue = totals[dayId].min;
    console.log("GetDayLabourValue", "date:", pDate, "dayId:", dayId, "minutes:", retValue);
    return retValue;
}
function SetDayValue(key, dayTotals) {
    if (debug) console.log("SetDayValue", key, "added", "date:", dayTotals.date, "data:", dayTotals);
    if (displayType == "Installation") {
     
        SetData('Codel-Doors', dayTotals.day, parseFloat(dayTotals.ExtDoors).toFixed(2));
       
        SetData('Patio-Doors', dayTotals.day, parseFloat(dayTotals.doors).toFixed(2));
        SetData('Installation-Min', dayTotals.day, parseFloat(0).toFixed(2));
        SetData('Wood-DropOff-Jobs', dayTotals.day, dayTotals.TotalWoodDropOff);
        SetData('HighRisk-Jobs', dayTotals.day, dayTotals.TotalHighRisk);
        SetData('Windows', dayTotals.day, parseFloat(dayTotals.windows).toFixed(2));
        SetData('Work-Orders', dayTotals.day, dayTotals.WOCount);
        SetData('Asbestos-Jobs', dayTotals.day, dayTotals.TotalAsbestos);
        SetData('Lead-Paint', dayTotals.day, dayTotals.TotalLeadPaint);
        SetData('Sales-Amount', dayTotals.day, dayTotals.SalesAmmount.formatMoney(2, "$", ",", "."));
      //  SetData('SalesTarget', dayTotals.day, parseFloat(dayTotals.SalesTarget).toFixed(2));
        SetData('SalesTarget', dayTotals.day, dayTotals.SalesTarget.formatMoney(2, "$", ",", "."));
      //  SetData('Sales+/-', dayTotals.day, parseFloat(dayTotals.SalesTarget - dayTotals.SalesAmmount).formatMoney(2, "$", ",", "."));
       // SetData('Sales+/-', dayTotals.day, (parseFloat(dayTotals.SalesTarget) - parseFloat(dayTotals.SalesAmmount)).formatMoney(2, "$", ",", "."));
        SetData('Sales-Diff', dayTotals.day, (parseFloat(dayTotals.SalesTarget) - parseFloat(dayTotals.SalesAmmount)).formatMoney(2, "$", ",", "."));

        SetData('Window-LBR', dayTotals.day, parseFloat(dayTotals.subinstallationwindowLBRMIN).toFixed(2)); 
        SetData('Patio-Door-LBR', dayTotals.day, parseFloat(dayTotals.subInstallationPatioDoorLBRMin).toFixed(2));
        SetData('Codel-Door-LBR', dayTotals.day, parseFloat(dayTotals.subExtDoorLBRMIN).toFixed(2));
        SetData('Total-LBR', dayTotals.day, (parseFloat(dayTotals.subTotalInstallationLBRMin) + parseFloat(dayTotals.subExtDoorLBRMIN)) .toFixed(2));
        SetData('Installation-Min', dayTotals.day, parseFloat(dayTotals.MinAvailable).toFixed(2));
       // SetData('LBR\+', dayTotals.day, (parseFloat(dayTotals.MinAvailable) - parseFloat(dayTotals.subTotalInstallationLBRMin)).toFixed(2));
        SetData('LBR-Diff', dayTotals.day, (parseFloat(dayTotals.MinAvailable) - parseFloat(dayTotals.subTotalInstallationLBRMin) - parseFloat(dayTotals.subExtDoorLBRMIN) ).toFixed(2));
    

      // SetData('Siding-LBRBudget', dayTotals.day, parseFloat(dayTotals.SidingLBRBudget).toFixed(2));
        SetData('Siding-LBRBudget', dayTotals.day, dayTotals.SidingLBRBudget.formatMoney(2, "$", ",", "."));
        SetData('Siding-LBRMin', dayTotals.day, parseFloat(dayTotals.SidingLBRMin).toFixed(2));
        SetData('Siding-SQF', dayTotals.day, parseFloat(dayTotals.SidingSQF).toFixed(2));
        SetData('Hazardous-LBRMin', dayTotals.day, parseFloat(dayTotals.HazardousBudgetedLBR).toFixed(2));

       
     
    }
    else if (displayType == "Remeasure") {

        SetData('Codel-Doors', dayTotals.day, parseFloat(dayTotals.ExtDoors).toFixed(2));

        SetData('Patio-Doors', dayTotals.day, parseFloat(dayTotals.doors).toFixed(2));
        SetData('Installation-Min', dayTotals.day, parseFloat(0).toFixed(2));
        //SetData('Wood-DropOff-Jobs', dayTotals.day, dayTotals.TotalWoodDropOff);
        SetData('HighRisk-Jobs', dayTotals.day, dayTotals.TotalHighRisk);
        SetData('Windows', dayTotals.day, parseFloat(dayTotals.windows).toFixed(2));
        SetData('Work-Orders', dayTotals.day, dayTotals.WOCount);
        //SetData('Asbestos-Jobs', dayTotals.day, dayTotals.TotalAsbestos);
        //SetData('Lead-Paint', dayTotals.day, dayTotals.TotalLeadPaint);
        SetData('Sales-Amount', dayTotals.day, dayTotals.SalesAmmount.formatMoney(2, "$", ",", "."));

        SetData('Window-LBR', dayTotals.day, parseFloat(dayTotals.subinstallationwindowLBRMIN).toFixed(2));
        SetData('Patio-Door-LBR', dayTotals.day, parseFloat(dayTotals.subInstallationPatioDoorLBRMin).toFixed(2));
        SetData('Codel-Door-LBR', dayTotals.day, parseFloat(dayTotals.subExtDoorLBRMIN).toFixed(2));
        SetData('Total-LBR', dayTotals.day, (parseFloat(dayTotals.subTotalInstallationLBRMin) + parseFloat(dayTotals.subExtDoorLBRMIN)).toFixed(2));

        // SetData('Siding-LBRBudget', dayTotals.day, parseFloat(dayTotals.SidingLBRBudget).toFixed(2));
        SetData('Siding-LBRBudget', dayTotals.day, dayTotals.SidingLBRBudget.formatMoney(2, "$", ",", "."));
        SetData('Siding-LBRMin', dayTotals.day, parseFloat(dayTotals.SidingLBRMin).toFixed(2));
        SetData('Siding-SQF', dayTotals.day, parseFloat(dayTotals.SidingSQF).toFixed(2));

    }
    else {
        var maxTime = parseInt(FindByValue("max", dayTotals.date).Value);
        var maxStaff = parseInt(FindByValue("manpower", dayTotals.date).Value);
        SetData('Doors', dayTotals.day, parseFloat(dayTotals.doors).toFixed(2));
     
        SetData('Rush', dayTotals.day, dayTotals.rush);
        SetData('Windows', dayTotals.day, parseFloat(dayTotals.windows).toFixed(2));
    
       // SetData('Boxes', dayTotals.day, dayTotals.boxes);
        SetData('Boxes', dayTotals.day, parseFloat(dayTotals.boxes).toFixed(2));

      //  SetData('Glass', dayTotals.day, dayTotals.glass);

        SetData('Glass', dayTotals.day, parseFloat(dayTotals.glass).toFixed(2));

        SetData('Value', dayTotals.day, dayTotals.value.formatMoney(2, "$", ",", "."));

        SetData('Min', dayTotals.day, dayTotals.min.formatMoney(0, "", ",", "."));
        SetData('Max', dayTotals.day, maxTime.formatMoney(0, "", ",", "."));
        SetData('Available_Time', dayTotals.day, (maxTime - dayTotals.min).formatMoney(0, "", ",", "."));
        SetData('Available_Staff', dayTotals.day, maxStaff);
        SetData('Float', dayTotals.day, dayTotals.float.formatMoney(0, "", ",", "."));

        //SetData('27DS', dayTotals.day, dayTotals.F27DS);


      //  SetData('27TS', dayTotals.day, dayTotals.F27TS);

     //   SetData('27TS', dayTotals.day, parseFloat(dayTotals.F27TS).toFixed(2));

       // SetData('27TT', dayTotals.day, dayTotals.F27TT);
      //  SetData('27TT', dayTotals.day, parseFloat(dayTotals.F27TT).toFixed(2));

       // SetData('29CA', dayTotals.day, dayTotals.F29CA);
        SetData('29CA', dayTotals.day, parseFloat(dayTotals.F29CA).toFixed(2));

      //  SetData('29CM', dayTotals.day, dayTotals.F29CM);
        SetData('29CM', dayTotals.day, parseFloat(dayTotals.F29CM).toFixed(2));

        SetData('52PD', dayTotals.day, parseFloat(dayTotals.F52PD).toFixed(2));

       // SetData('68CA', dayTotals.day, dayTotals.F68CA);
        SetData('68CA', dayTotals.day, parseFloat(dayTotals.F68CA).toFixed(2));

       // SetData('68VS', dayTotals.day, dayTotals.F68VS);
        SetData('68VS', dayTotals.day, parseFloat(dayTotals.F68VS).toFixed(2));

        //SetData('68SL', dayTotals.day, dayTotals.F68SL);
        SetData('68SL', dayTotals.day, parseFloat(dayTotals.F68SL).toFixed(2));

       // SetData('26CA', dayTotals.day, dayTotals.F6CA);
        SetData('26CA', dayTotals.day, parseFloat(dayTotals.F6CA).toFixed(2));

      //  SetData('Transom', dayTotals.day, dayTotals.Transom);
     //   SetData('Sidelite', dayTotals.day, dayTotals.Sidelite);
   //     SetData('SingleDoor', dayTotals.day, dayTotals.SingleDoor);
      //  SetData('SingleDoor', dayTotals.day, parseFloat(dayTotals.SingleDoor).toFixed(2));
      //  SetData('DoubleDoor', dayTotals.day, dayTotals.DoubleDoor);
       // SetData('DoubleDoor', dayTotals.day, parseFloat(dayTotals.DoubleDoor).toFixed(2));
        SetData('Patio-Doors', dayTotals.day, parseFloat(dayTotals.F52PD).toFixed(2));

        SetData('Sliders', dayTotals.day, (parseFloat(dayTotals.F68SL) + parseFloat(dayTotals.F68VS)).toFixed(2));
        SetData('Sliders-Min', dayTotals.day, (parseFloat(dayTotals.F68SLMin) + parseFloat(dayTotals.F68VSMin)).toFixed(2));

        SetData('Casements', dayTotals.day, (parseFloat(dayTotals.F29CM) + parseFloat(dayTotals.F29CA) + parseFloat(dayTotals.F6CA)).toFixed(2));
        SetData('Casements-Min', dayTotals.day, (parseFloat(dayTotals.F29CMMin) + parseFloat(dayTotals.F29CAMin) + parseFloat(dayTotals.F26CAMin)).toFixed(2));

        SetData('Vinyl-Swing', dayTotals.day, parseFloat(dayTotals.F27DS).toFixed(2));
        SetData('Vinyl-Swing-Min', dayTotals.day, parseFloat(dayTotals.F27DSMin).toFixed(2) );


        //SetData('Simple', dayTotals.day, dayTotals.Simple);
        //SetData('Complex', dayTotals.day, dayTotals.Complex);
        //SetData('Over_Size', dayTotals.day, dayTotals.Over_Size);
        //SetData('Arches', dayTotals.day, dayTotals.Arches);
        //SetData('Rakes', dayTotals.day, dayTotals.Rakes);
        //SetData('Customs', dayTotals.day, dayTotals.Customs);
    }
}
function GetOffset(day) {
    if (day == "sun") return 0;
    else if (day == "mon") return 1;
    else if (day == "tue") return 2;
    else if (day == "wed") return 3;
    else if (day == "thu") return 4;
    else if (day == "fri") return 5;
    else if (day == "sat") return 6;
}

function UpdateEventWeekends() {
    var i = eventid;
    var SaturdaySunday;
    var isSaturdayChecked = document.getElementsByName('saturday')[0].checked;
    var isSundayChecked  = document.getElementsByName('sunday')[0].checked;

    if ((isSaturdayChecked == true) && (isSundayChecked == true)) {
        SaturdaySunday = "both";
    }
    else if (isSaturdayChecked == true) {
        SaturdaySunday = "saturday";
    }
    else if (isSundayChecked == true) {
        SaturdaySunday = "sunday";
    }
    else {
        SaturdaySunday = "none";
    }

    $.ajax({
        url: 'data.svc/UpdateInstallationWeekends?id=' + eventid + '&SaturdaySunday=' + SaturdaySunday ,
        type: "POST",
        success: function (data) {
            if (debug) console.log("events.success", "data.UpdateEventWeekends:");
            //$("#eventContent .close").click();
            //$('#calendar').fullCalendar('refetchEvents');
            //$('#calendar').fullCalendar('rerenderEvents');
            var view = $('#calendar').fullCalendar('getView');
            if (view.type == 'agendaWeek') {
                $('#calendar').fullCalendar('changeView', 'agendaWeek');
            }
            }, error: function (error) {
                console.log('Error', error);
                $('#script-warning').show();
            }
    });
    
}
function FindByValue(target, val) {
    if (debug) console.log("FindByValue", val);
    for (var i = 0; i < GlobalParams.length; i++) {
        var date = new Date(parseInt(GlobalParams[i].Date.substr(6)));
        if (GlobalParams[i].Name == target && date.Equals(val)) {
            if (debug) console.log("FindByValue", val, date, GlobalParams[i].Value);
            return GlobalParams[i];
        }
    }
    if (debug) console.log("FindByValue", "not found!!");
    return { Value: "0" };
}
function SetData(target, day, value) {
    $('#' + target).find('.fc-' + day).html(value);
}
// Extend the default Number object with a formatMoney() method:
// usage: someVar.formatMoney(decimalPlaces, symbol, thousandsSeparator, decimalSeparator)
// defaults: (2, "$", ",", ".")
Number.prototype.formatMoney = function (places, symbol, thousand, decimal) {
    places = !isNaN(places = Math.abs(places)) ? places : 2;
    symbol = symbol !== undefined ? symbol : "$";
    thousand = thousand || ",";
    decimal = decimal || ".";
    var number = this,
        negative = number < 0 ? "-" : "",
        i = parseInt(number = Math.abs(+number || 0).toFixed(places), 10) + "",
        j = (j = i.length) > 3 ? j % 3 : 0;
    return symbol + negative + (j ? i.substr(0, j) + thousand : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousand) + (places ? decimal + Math.abs(number - i).toFixed(places).slice(2) : "");
};

function ApplyFilters(target) {
    console.log("ApplyFilters", "target: ", target);
    $('#' + target + 'Filter').addClass('hidden');
    $('#calendar').fullCalendar('refetchEvents');
    $('#calendar').fullCalendar('rerenderEvents');
}

function WoodDropOff(isShowing) {
    if (isShowing == 'show') {
        $('#divWoodDropOffDate').show();
    }
    else if (isShowing == 'hide') {
        $('#divWoodDropOffDate').hide();
    } 


}

function SelectAll(target) {
    console.log("SelectAll", "target: ", target);

    if (target == "InstallationState") {
        for (i = 0; i < document.getElementsByName('InstallationState').length; i++) {
            document.getElementsByName('InstallationState')[i].checked = true;
        }
        $('#' + target + 'Filter').addClass('hidden');
    }
    else if (target == "RemeasureState")
    {
        for (i = 0; i < document.getElementsByName('RemeasureState').length; i++) {
            document.getElementsByName('RemeasureState')[i].checked = true;
        }
        $('#' + target + 'Filter').addClass('hidden');
    }
    else if (target == "branch")
    {
        for (i = 0; i < document.getElementsByName('branch').length; i++)
        {
            document.getElementsByName('branch')[i].checked = true;
        }
        $('#' + 'BranchFilter').addClass('hidden');
    }
   
  
    $('#calendar').fullCalendar('refetchEvents');
    $('#calendar').fullCalendar('rerenderEvents');
}
function ChangeType(type) {
    displayType = type;
    $('.fc-changeType-button').html(displayType);
    $('#calendar').fullCalendar('refetchEvents');
    $('#external-events').find(".fc-event").remove();
    $('#external-events1').find(".fc-event").remove();
    $('#external-eventsRemeasure').find(".fc-event").remove();
    ControlHeaderVisibility(GetDisplayItemList(displayType));

    if (type == "Installation") {
        LoadInstallationBufferedJobs();
        $('#external-events1').show();
        $('#external-events').hide();
        $('#external-eventsRemeasure').hide();
    }
    else if (type == "Remeasure") {
        LoadRemeasureBufferedJobs();
        $('#external-eventsRemeasure').show();
        $('#external-events').hide();
        $('#external-events1').hide();
        
    }
    else {
        LoadBufferedJobs();
        $('#external-events').show();
        $('#external-events1').hide();
        $('#external-eventsRemeasure').hide();
    }

 
    $('#typeChange').addClass('hidden');
    $('#calendar').fullCalendar('changeView', 'month');
}




function ControlHeaderVisibility(elements) {
    for (var i = 0; i < elements.length; i++) {
        var element = $("#" + elements[i].Id);
        if (elements[i].Display && element.hasClass('hidden')) {
            element.removeClass('hidden');
        }
        else if (!elements[i].Display && !element.hasClass('hidden')) {
            element.addClass('hidden');
        }
    }
}


// Week Header for Windows Doors Paint True / False to Enable or Disable
function GetDisplayItemList(type) {
    if (type == "Windows")
        return [{ Id: 'Windows', Display: true }, { Id: 'Doors', Display: true }, { Id: 'Boxes', Display: true }, { Id: 'Glass', Display: true }, { Id: 'Rush', Display: true }, { Id: 'Min', Display: true }, { Id: 'Max', Display: true }, { Id: 'Available_Time', Display: true }, { Id: 'Available_Staff', Display: true }, { Id: '26CA', Display: true }, { Id: '27DS', Display: true }, { Id: '27TS', Display: true }, { Id: '27TT', Display: true }, { Id: '29CA', Display: true }, { Id: '29CM', Display: true }, { Id: '68CA', Display: false }, { Id: '68SL', Display: true }, { Id: '68VS', Display: true }, { Id: '52PD', Display: true }, { Id: 'Transom', Display: false }, { Id: 'Sidelite', Display: false }, { Id: 'SingleDoor', Display: false }, { Id: 'DoubleDoor', Display: false }/**/];
    else if (type == "Shipping")
        return [{ Id: 'Windows', Display: true }, { Id: 'Doors', Display: true }, { Id: 'Boxes', Display: true }, { Id: 'Glass', Display: true }, { Id: 'Rush', Display: false }, { Id: 'Min', Display: false }, { Id: 'Max', Display: false }, { Id: 'Available_Time', Display: false }, { Id: 'Available_Staff', Display: false }, { Id: '26CA', Display: true }, { Id: '27DS', Display: true }, { Id: '27TS', Display: true }, { Id: '27TT', Display: true }, { Id: '29CA', Display: true }, { Id: '29CM', Display: true }, { Id: '68CA', Display: false }, { Id: '68SL', Display: true }, { Id: '68VS', Display: true }, { Id: '52PD', Display: true }, { Id: 'Transom', Display: false }, { Id: 'Sidelite', Display: false }, { Id: 'SingleDoor', Display: false }, { Id: 'DoubleDoor', Display: false }/**/, { Id: 'Simple', Display: false }, { Id: 'Complex', Display: false }, { Id: 'Over_Size', Display: false }, { Id: 'Arches', Display: false }, { Id: 'Rakes', Display: false }, { Id: 'Customs', Display: false }];
    else if (type == "Doors")
        return [{ Id: 'Windows', Display: false }, { Id: 'Doors', Display: true }, { Id: 'Boxes', Display: false }, { Id: 'Glass', Display: false }, { Id: 'Rush', Display: true }, { Id: 'Min', Display: false }, { Id: 'Max', Display: false }, { Id: 'Available_Time', Display: false }, { Id: 'Available_Staff', Display: false }, { Id: '26CA', Display: false }, { Id: '27DS', Display: false }, { Id: '27TS', Display: false }, { Id: '27TT', Display: false }, { Id: '29CA', Display: false }, { Id: '29CM', Display: false }, { Id: '68CA', Display: false }, { Id: '68SL', Display: false }, { Id: '68VS', Display: false }, { Id: '52PD', Display: false }, { Id: 'Transom', Display: true }, { Id: 'Sidelite', Display: true }, { Id: 'SingleDoor', Display: true }, { Id: 'DoubleDoor', Display: true }/**/, { Id: 'Simple', Display: false }, { Id: 'Complex', Display: false }, { Id: 'Over_Size', Display: false }, { Id: 'Arches', Display: false }, { Id: 'Rakes', Display: false }, { Id: 'Customs', Display: false }];
    else if (type == "Paint")
        return [{ Id: 'Windows', Display: true }, { Id: 'Doors', Display: true }, { Id: 'Boxes', Display: true }, { Id: 'Glass', Display: true }, { Id: 'Rush', Display: true }, { Id: 'Min', Display: true }, { Id: 'Max', Display: true }, { Id: 'Available_Time', Display: true }, { Id: 'Available_Staff', Display: true }, { Id: '26CA', Display: false }, { Id: '27DS', Display: false }, { Id: '27TS', Display: false }, { Id: '27TT', Display: false }, { Id: '29CA', Display: false }, { Id: '29CM', Display: false }, { Id: '68CA', Display: false }, { Id: '68SL', Display: false }, { Id: '68VS', Display: false }, { Id: '52PD', Display: false }, { Id: 'Transom', Display: false }, { Id: 'Sidelite', Display: false }, { Id: 'SingleDoor', Display: false }, { Id: 'DoubleDoor', Display: false }/**/, { Id: 'Simple', Display: false }, { Id: 'Complex', Display: false }, { Id: 'Over_Size', Display: false }, { Id: 'Arches', Display: false }, { Id: 'Rakes', Display: false }, { Id: 'Customs', Display: false }];
    else
        return [{ Id: 'Windows', Display: true }, { Id: 'Doors', Display: true }, { Id: 'Boxes', Display: true }, { Id: 'Glass', Display: true }, { Id: 'Rush', Display: true }, { Id: 'Min', Display: true }, { Id: 'Max', Display: true }, { Id: 'Available_Time', Display: true }, { Id: 'Available_Staff', Display: false }, { Id: '26CA', Display: false }, { Id: '27DS', Display: false }, { Id: '27TS', Display: false }, { Id: '27TT', Display: false }, { Id: '29CA', Display: false }, { Id: '29CM', Display: false }, { Id: '68CA', Display: false }, { Id: '68SL', Display: false }, { Id: '68VS', Display: false }, { Id: '52PD', Display: false }, { Id: 'Transom', Display: false }, { Id: 'Sidelite', Display: false }, { Id: 'SingleDoor', Display: false }, { Id: 'DoubleDoor', Display: false }/**/, { Id: 'Simple', Display: false }, { Id: 'Complex', Display: false }, { Id: 'Over_Size', Display: false }, { Id: 'Arches', Display: false }, { Id: 'Rakes', Display: false }, { Id: 'Customs', Display: false }];
}

// Over Capacity Error
function ShowWarning(allocatedDayLabour, eventLabourMin, maxDayLabour) {
   // window.alert("There are already " + allocatedDayLabour + " minutes on the target day. Adding requested event with Labour minutes of " + eventLabourMin \+ " will exceed maximum available minutes (" + maxDayLabour + ") for the day.");
    window.alert("There are already " + allocatedDayLabour + " minutes on the target day. Adding requested event with Labour minutes of " + eventLabourMin + " will exceed maximum available minutes (" + maxDayLabour + ") for the day.");

}

function UpdateReturnedJobSchedule() {
    var i = eventid;
    var scheduledStartDate = $("#from_date").val();
    var scheduledEndDate = $("#end_date").val();
    var returnTripReason = $("#returnJobReasonID").val();
    $.ajax({
        url: 'data.svc/UpdateReturnedJobSchedule?id=' + eventid + '&ScheduledStartDate=' + scheduledStartDate + '&scheduledEndDate=' + scheduledEndDate + '&returnTripReason=' + returnTripReason,
        type: "POST",
        success: function (data) {
            if (debug) console.log("events.success", "data.UpdateReturnedJobSchedule:");
            //$("#from_date").val('');
            //$("#end_date").val('');
          
            //$("#eventContent .close").click();
            //$('#calendar').fullCalendar('refetchEvents');
            //$('#calendar').fullCalendar('rerenderEvents');

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}
//update events from popup
function UpdateInstallationEvents() {
   // UpdateEventSchedule();
  //  UpdateEventWeekends();

    var WoodDropOffDate, WoodDropOffTime;
   
    WoodDropOffDate = $("#wooddropdate").val();
    WoodDropOffTime = $("#wooddropTime").val();
    var scheduledReturnedJobStartDate = $("#from_date").val();
    if (scheduledReturnedJobStartDate.length != 0) {
        UpdateReturnedJobSchedule();
    }

    //if ($("#IsTruckAllDay")[0].checked) {
    //    isAllDayChecked = "Yes";
    //}
    var isAllDayChecked = "";
    if ($("#IsAllDay")[0].checked) {
        isAllDayChecked = "Yes";
    }

    

    var i = eventid;
    var scheduledStartDate, scheduledEndDate;
    scheduledStartDate = $("#InstallScheduledStartDate").val();
    scheduledEndDate = $("#InstallScheduledEndDate").val();
    var startTime = $("#InstallScheduledStartTime").val();
    var endTime = $("#InstallScheduledEndTime").val();
    


    var Asbestos = 0, WoodDropOff = 0, HighRisk = 0;
    var LeadPaint = 'No'; 
    if ($("#Asbestos-JobsYes").prop('checked')) {
        Asbestos =1;
    }

    if ($("#Lead-PaintYes").prop('checked')) {
        LeadPaint = 'Yes';
    }
    if ($("#Wood-DropOff-JobsYes").prop('checked')) {
        WoodDropOff = 1;
    }
    if ($("#HighRisk-JobsYes").prop('checked')) {
        HighRisk = 1;
    }

    NumOfInstallers = $("#NumOfInstallers").val();
    var isSaturdayChecked='No', isSundayChecked='No';
    if (document.getElementsByName('saturday')[0].checked == true) {
        isSaturdayChecked = 'Yes';
    }
    if (document.getElementsByName('saturday')[0].checked == true) {
        isSundayChecked = 'Yes';
    }

    $.ajax({
        url: 'data.svc/UpdateInstallationData?id=' + eventid
            + '&ScheduledStartDate=' + scheduledStartDate + '&scheduledEndDate=' + scheduledEndDate
            + '&startTime=' + startTime + '&endTime=' + endTime
            + '&Asbestos=' + Asbestos + '&WoodDropOff=' + WoodDropOff
            + '&woodDropOffDate=' + WoodDropOffDate
            + '&woodDropOffTime=' + WoodDropOffTime
            + '&HighRisk=' + HighRisk + '&EstInstallerCnt=' + NumOfInstallers
            + '&Saturday=' + isSaturdayChecked + '&Sunday=' + isSundayChecked + '&LeadPaint=' + LeadPaint
            + '&isAllDayChecked=' + isAllDayChecked,
        type: "POST",
        success: function (data) {
            if (debug) console.log("events.success", "data.UpdateInstallationEvents:");
           
            $("#eventContent .close").click();
            $('#calendar').fullCalendar('refetchEvents');
            //$('#calendar').fullCalendar('rerenderEvents');

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });
}

function UpdateRemeasureEvents(event) {
    var id = event.id;
    var remesureDate, currentState;
    currentState = event.CurrentStateName;
    remesureDate = event.start;
    remeasureEndDate = event.EndRemeasureDate;
    
    $.ajax({
        url: 'data.svc/UpdateRemeasureData?id=' + id
            + '&remeasureDate=' + remesureDate
            + '&remeasureEndDate=' + remeasureEndDate
            + '&currentState=' + currentState
            + '&fromPopup=no',
        type: "POST",
        success: function (data) {
            if (debug) console.log("events.success", "data.UpdateRemeasureEvents:");
            $("#RemeasureDate").val('');
            $("#EndRemeasureDate").val('');
            $('#calendar').fullCalendar('refetchEvents');
            $('#calendar').fullCalendar('rerenderEvents');
    

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });
}

function tConv24(time24) {
    var ts = time24;
    var H = +ts.substr(0, 2);
    var h = (H % 12) || 12;
   // h = (h < 10) ? ("0" + h) : h;  // leading 0 at the left for 1 digit hours
    var ampm = H < 12 ? " am" : " pm";
    ts = h + ts.substr(2, 3) + ampm;
    return ts;
}

function UpdateRemeasureEventsFromPopup() {
    var id = eventid;
    var remesureDate, endRemeasureDate; 
    remesureDate = $("#RemeasureDate").val();
    endRemeasureDate = $("#EndRemeasureDate").val();

    $("#RemeasureDate").val(new Date(GetDatefromMoment(event.start + 24 * 60 * 60000)).toLocaleDateString('en-US'));

    $.ajax({
        url: 'data.svc/UpdateRemeasureData?id=' + id
            + '&remeasureDate=' + remesureDate
            + '&remeasureEndDate=' + endRemeasureDate
            + '&fromPopup=yes',
        type: "POST",
        success: function (data) {
            if (debug) console.log("events.success", "data.UpdateRemeasureEvents:");

            //$('#calendar').fullCalendar('refetchEvents');
            //$('#calendar').fullCalendar('rerenderEvents');
      
            $("#eventRemeasureContent .close").click();
            $('#calendar').fullCalendar('refetchEvents');
            //$('#calendar').fullCalendar('rerenderEvents');

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });
}



function GetJobAnalysys(workOrder) {
   // TotalLBRMin
    //var test;
    //var tab = $("a[href='#JobAnalysisTab']");
    //if (tab.innerText == "JOB ANALYSIS") {
    //    alert("asdfasd");
    //}
    //if 
    //$("a[href='#JobAnalysisTab']").on('shown.bs.tab', function (e) {
       
    //});

    //$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    //    var target = $(e.target).attr("href") // activated tab
    //    alert(target);

    //    $.ajax({
    //        //type: "POST",  
    //        url: 'data.svc/GetJobAnalysis?workOrderNumber=' + workOrder,
    //        dataType: 'json',
    //        success: function (data) {
    //            if (debug) console.log("events.success", "data.GetProducts:");

    //            $("#dataTable tr").remove();
    //            if (data.GetProductsResult.length > 0) {
    //                $("#dataTable").append("<tr>  <th style = 'text-align:center;' > Item</th ><th style='text-align:center;'> Size</th ><th style='text-align:center;'>Quantity</th> <th style = 'text-align:center;' > SubQty</th ><th style='text-align:center;' > System</th ><th style='text-align:center;'>Description</th><th style='text-align:center;' > Status</th >  </tr > ");
    //                for (var i = 0; i < data.GetProductsResult.length; i++) {
    //                    $("#dataTable").append("<tr><td>" +
    //                        data.GetProductsResult[i].Item + "</td> <td>" +
    //                        data.GetProductsResult[i].Size + "</td> <td>" +
    //                        data.GetProductsResult[i].Quantity + "</td> <td>" +
    //                        data.GetProductsResult[i].SubQty + "</td> <td>" +
    //                        data.GetProductsResult[i].System + "</td> <td>" +
    //                        data.GetProductsResult[i].Description + "</td> <td>" +
    //                        data.GetProductsResult[i].Status + "</td></tr>");
    //                }
    //            }

    //        }, error: function (error) {
    //            console.log('Error', error);
    //            $('#script-warning').show();
    //        }
    //    });
    //});
}


function GetUnavailableResources() {
    //$("#dataTableHR tr").remove();
    
    //$.ajax({
    //    //type: "POST",  
    //    url: 'data.svc/GetUnavailableResources=' ,
    //    dataType: 'json',
    //    data: {  branch: branches.join(",") },
    //    success: function (data) {
    //        if (debug) console.log("events.success", "data.GetUnavailableResources:");
            
    //        if (data.GetUnavailableResourcesResult.length > 0) {
                
    //            $("#dataTableHR").append("<tr> < th style = 'text-align:center;' > Date Unavaiable</th ><th style='text-align:center;'> Name</th ><th style='text-align:center;'>Branch</th> <th style='text-align:center;' > Unavailable Reason</th > </tr > ");
    //            for (var i = 0; i < data.GetUnavailableResourcesResult.length; i++) {
    //                $("#dataTableHR").append("<tr><td>" +
    //                    data.GetUnavailableResourcesResult[i].DateUnavaiable + "</td> <td>" +
    //                    data.GetUnavailableResourcesResult[i].Name + "</td> <td>" +
    //                    data.GetUnavailableResourcesResult[i].Branch + "</td> <td>" +
    //                    data.GetUnavailableResourcesResult[i].Reason + "</td></tr>");
    //            }
    //        }
    //        else {
    //            noInstallationWindows.style.display = "block";
    //        }

    //    }, error: function (error) {
    //        console.log('Error', error);
    //        $('#script-warning').show();
    //    }
    //});



}
function GetInstallationProducts(workOrder) {
    $("#dataTableInstallationDoors tr").remove();
    $("#dataTableInstallationWindows tr").remove(); 
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetProducts?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetProducts:");
            var noInstallationWindows = document.getElementById('noInstallationWindows');
         
            if (data.GetProductsResult.length > 0) {
                noInstallationWindows.style.display = "none";
                $("#dataTableInstallationWindows").append("<tr>  <th style = 'text-align:center;' > Item</th ><th style='text-align:center;'> Size</th ><th style='text-align:center;'>Quantity</th> <th style = 'text-align:center;' > SubQty</th ><th style='text-align:center;' > System</th ><th style='text-align:center;'>Description</th><th style='text-align:center;' > Status</th >  </tr > ");
                for (var i = 0; i < data.GetProductsResult.length; i++) {
                    //if (data.GetProductsResult[i].Status == "On Hold") {
                    var st = "";
                    if (data.GetProductsResult[i].Status == "On Hold") {
                        st = " style='background-color:#ff6347;'";
                    }
                  
                   
                    $("#dataTableInstallationWindows").append("<tr" + st + "><td>" +
                        data.GetProductsResult[i].Item + "</td> <td>" +
                        data.GetProductsResult[i].Size + "</td> <td>" +
                        data.GetProductsResult[i].Quantity + "</td> <td>" +
                        data.GetProductsResult[i].SubQty + "</td> <td>" +
                        data.GetProductsResult[i].System + "</td> <td>" +
                        data.GetProductsResult[i].Description + "</td> <td>" + 
                        data.GetProductsResult[i].Status + "</td></tr>");
                }
            }
            else {
                noInstallationWindows.style.display = "block";
            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetProductsDoors?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.ProductsDoors:");
            var noInstallationDoors = document.getElementById('noInstallationDoors');
           
            if (data.GetProductsDoorsResult.length > 0) {
                noInstallationDoors.style.display = "none";
                $("#dataTableInstallationDoors").append("<tr>  <th style = 'text-align:center;' > Item</th ><th style='text-align:center;'> Size</th ><th style='text-align:center;'>Quantity</th> <th style = 'text-align:center;' > SubQty</th ><th style='text-align:center;' > System</th ><th style='text-align:center;'>Description</th><th style='text-align:center;' > Status</th >  </tr > ");
                for (var i = 0; i < data.GetProductsDoorsResult.length; i++) {
                    var st = "";
                    if (data.GetProductsDoorsResult[i].Status == "On Hold") {
                        st = " style='background-color:#ff6347;'";
                    }
                    $("#dataTableInstallationDoors").append("<tr" + st + "><td>" +
                        data.GetProductsDoorsResult[i].Item + "</td> <td>" +
                        data.GetProductsDoorsResult[i].Size + "</td> <td>" +
                        data.GetProductsDoorsResult[i].Quantity + "</td> <td>" +
                        data.GetProductsDoorsResult[i].SubQty + "</td> <td>" +
                        data.GetProductsDoorsResult[i].System + "</td> <td>" +
                        data.GetProductsDoorsResult[i].Description + "</td> <td>" +
                        data.GetProductsDoorsResult[i].Status + "</td></tr>");
                }
            }
            else {
                noInstallationDoors.style.display = "block";
            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}



function GetManufacturingProducts(workOrder) {
    $("#dataTableManufacturingDoors tr").remove();
    $("#dataTableManufacturingWindows tr").remove();
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetManufacturingWindows?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetManufacturingWindows:");
            var noManufacturingWindows = document.getElementById('noManufacturingWindows');
           
            if (data.GetManufacturingWindowsResult.length > 0) {
                noManufacturingWindows.style.display = "none";
                $("#dataTableManufacturingWindows").append("<tr>  <th style = 'text-align:center;' > Item</th ><th style='text-align:center;'> Size</th ><th style='text-align:center;'>Quantity</th> <th style = 'text-align:center;' > SubQty</th ><th style='text-align:center;' > System</th ><th style='text-align:center;'>Description</th><th style='text-align:center;' > Status</th >  </tr > ");
                for (var i = 0; i < data.GetManufacturingWindowsResult.length; i++) {
                    $("#dataTableManufacturingWindows").append("<tr><td>" +
                        data.GetManufacturingWindowsResult[i].Item + "</td> <td>" +
                        data.GetManufacturingWindowsResult[i].Size + "</td> <td>" +
                        data.GetManufacturingWindowsResult[i].Quantity + "</td> <td>" +
                        data.GetManufacturingWindowsResult[i].SubQty + "</td> <td>" +
                        data.GetManufacturingWindowsResult[i].System + "</td> <td>" +
                        data.GetManufacturingWindowsResult[i].Description + "</td> <td>" +
                        data.GetManufacturingWindowsResult[i].Status + "</td></tr>");
                }
            }
            else {
                noManufacturingWindows.style.display = "block";
            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetManufacturingDoors?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetManufacturingDoors:");
            var noManufacturingDoors = document.getElementById('noManufacturingDoors');
            
            if (data.GetManufacturingDoorsResult.length > 0) {
                noManufacturingDoors.style.display = "none";
                $("#dataTableManufacturingDoors").append("<tr>  <th style = 'text-align:center;' > Item</th ><th style='text-align:center;'> Size</th ><th style='text-align:center;'>Quantity</th> <th style = 'text-align:center;' > SubQty</th ><th style='text-align:center;' > System</th ><th style='text-align:center;'>Description</th><th style='text-align:center;' > Status</th >  </tr > ");
                for (var i = 0; i < data.GetManufacturingDoorsResult.length; i++) {
                    $("#dataTableManufacturingDoors").append("<tr><td>" +
                        data.GetManufacturingDoorsResult[i].Item + "</td> <td>" +
                        data.GetManufacturingDoorsResult[i].Size + "</td> <td>" +
                        data.GetManufacturingDoorsResult[i].Quantity + "</td> <td>" +
                        data.GetManufacturingDoorsResult[i].SubQty + "</td> <td>" +
                        data.GetManufacturingDoorsResult[i].System + "</td> <td>" +
                        data.GetManufacturingDoorsResult[i].Description + "</td> <td>" +
                        data.GetManufacturingDoorsResult[i].Status + "</td></tr>");
                }
            }
            else {
                noManufacturingDoors.style.display = "block";
            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}


function GetManufacturingWindowsProducts(workOrder) {
    $("#dataTableProductWindows tr").remove();
    $("#dataTableProductDoors tr").remove();
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetManufacturingWindows?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetManufacturingWindows:");
            var noProductWindows = document.getElementById('noProductWindows');

            if (data.GetManufacturingWindowsResult.length > 0) {
                noProductWindows.style.display = "none";
                $("#dataTableProductWindows").append("<tr>  <th style = 'text-align:center;' > Item</th ><th style='text-align:center;'> Size</th ><th style='text-align:center;'>Quantity</th> <th style = 'text-align:center;' > SubQty</th ><th style='text-align:center;' > System</th ><th style='text-align:center;'>Description</th><th style='text-align:center;' > Status</th >  </tr > ");
                for (var i = 0; i < data.GetManufacturingWindowsResult.length; i++) {
                    $("#dataTableProductWindows").append("<tr><td>" +
                        data.GetManufacturingWindowsResult[i].Item + "</td> <td>" +
                        data.GetManufacturingWindowsResult[i].Size + "</td> <td>" +
                        data.GetManufacturingWindowsResult[i].Quantity + "</td> <td>" +
                        data.GetManufacturingWindowsResult[i].SubQty + "</td> <td>" +
                        data.GetManufacturingWindowsResult[i].System + "</td> <td>" +
                        data.GetManufacturingWindowsResult[i].Description + "</td> <td>" +
                        data.GetManufacturingWindowsResult[i].Status + "</td></tr>");
                }
            }
            else {
                noProductWindows.style.display = "block";
            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetManufacturingDoors?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetManufacturingDoors:");
            var noProductDoors = document.getElementById('noProductDoors');

            if (data.GetManufacturingDoorsResult.length > 0) {
                noProductDoors.style.display = "none";
                $("#dataTableProductDoors").append("<tr>  <th style = 'text-align:center;' > Item</th ><th style='text-align:center;'> Size</th ><th style='text-align:center;'>Quantity</th> <th style = 'text-align:center;' > SubQty</th ><th style='text-align:center;' > System</th ><th style='text-align:center;'>Description</th><th style='text-align:center;' > Status</th >  </tr > ");
                for (var i = 0; i < data.GetManufacturingDoorsResult.length; i++) {
                    $("#dataTableProductDoors").append("<tr><td>" +
                        data.GetManufacturingDoorsResult[i].Item + "</td> <td>" +
                        data.GetManufacturingDoorsResult[i].Size + "</td> <td>" +
                        data.GetManufacturingDoorsResult[i].Quantity + "</td> <td>" +
                        data.GetManufacturingDoorsResult[i].SubQty + "</td> <td>" +
                        data.GetManufacturingDoorsResult[i].System + "</td> <td>" +
                        data.GetManufacturingDoorsResult[i].Description + "</td> <td>" +
                        data.GetManufacturingDoorsResult[i].Status + "</td></tr>");
                }
            }
            else {
                noProductDoors.style.display = "block";
            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}

//function SearchByName() {
//    var name = $("#txtName").val();
//    alert(name);
//}

function GetWindowsCustomer(workOrder) {
    //$("#dataTableManufacturingDoors tr").remove();
    //$("#dataTableManufacturingWindows tr").remove();
    $("#workOrderWindows").html("");
    $("#CustomerNameWindows").html("");
    $("#BranchWindows").html("");
    $("#ShippingTypeWindows").html("");
    $("#AddressWindows").html("");
    $("#emailWindows").html("");
    $("#PhoneWindows").html("");

    $("#TotalWindows").html("");
    $("#TotalPatioDoors").html("");
    $("#TotalDoors").html("");
    $("#TotalPrice").html("");

    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetWindowsCustomer?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetWindowsCustomer:");
            var noManufacturingWindows = document.getElementById('noProductWindows');

            if (data.GetWindowsCustomerResult.length > 0) {
                noManufacturingWindows.style.display = "none";
                //data.GetWindowsCustomerResult[0].Item = 

                $("#workOrderWindows").html(data.GetWindowsCustomerResult[0].WorkOrderNumber);
                $("#CustomerNameWindows").html(data.GetWindowsCustomerResult[0].CustomerName);
                $("#BranchWindows").html(data.GetWindowsCustomerResult[0].Branch_display);
                $("#ShippingTypeWindows").html(data.GetWindowsCustomerResult[0].ShippingType);
                $("#AddressWindows").html(data.GetWindowsCustomerResult[0].Address);
                $("#emailWindows").html(data.GetWindowsCustomerResult[0].Email);
                $("#PhoneWindows").html(data.GetWindowsCustomerResult[0].PhoneNumber);

                $("#TotalWindows").html(data.GetWindowsCustomerResult[0].TotalWindows);
                $("#TotalPatioDoors").html(data.GetWindowsCustomerResult[0].TotalPatioDoors);
                $("#TotalDoors").html(data.GetWindowsCustomerResult[0].TotalDoors);
               // $("#TotalPrice").html(data.GetWindowsCustomerResult[0].TotalPrice);
                $("#TotalPrice").html(data.GetWindowsCustomerResult[0].TotalPrice.formatMoney(2, "$", ",", "."));

                codeAddressWindows();
            }
            else {
                noManufacturingWindows.style.display = "block";
            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });
}

function GetRemeasureProducts(workOrder) {
    $("#dataTableRemeasure tr").remove();
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetProducts?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetProducts:");

           
            if (data.GetProductsResult.length > 0) {
                $("#dataTableRemeasure").append("<tr>  <th style = 'text-align:center;' > Item</th ><th style='text-align:center;'> Size</th ><th style='text-align:center;'>Quantity</th> <th style = 'text-align:center;' > SubQty</th ><th style='text-align:center;' > System</th ><th style='text-align:center;'>Description</th><th style='text-align:center;' > Status</th >  </tr > ");
                for (var i = 0; i < data.GetProductsResult.length; i++) {
                    $("#dataTableRemeasure").append("<tr><td>" +
                        data.GetProductsResult[i].Item + "</td> <td>" +
                        data.GetProductsResult[i].Size + "</td> <td>" +
                        data.GetProductsResult[i].Quantity + "</td> <td>" +
                        data.GetProductsResult[i].SubQty + "</td> <td>" +
                        data.GetProductsResult[i].System + "</td> <td>" +
                        data.GetProductsResult[i].Description + "</td> <td>" +
                        data.GetProductsResult[i].Status + "</td></tr>");
                }
            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}

function ShowCrews() {
   
    $("#dataTableInstallerEdit tr").remove();

    //$("#InstallerInfoPop").attr('href', 'javascript:void(0);');
    //$("#InstallerInfoPop").attr('data-toggle', "modal");
    //$("#InstallerInfoPop").attr('data-target', "#installerEdit");
    //$("#InstallerInfoPop").attr('href', "/details");

    $('#installerEdit').modal('show');
    $("#eventContent").css('opacity', 20);
    $("#installerDetail").hide();  
   
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetInstallerInfoByWorkOrder?workOrderNumber=' + WO,
        dataType: 'json',
        async: false,
        success: function (data) {
            if (debug) console.log("events.success", "data.GetInstallerInfoByWorkOrderResult:");
            $("#installerEditTitle").html(WO + " Installer");

            if (data.GetInstallerInfoByWorkOrderResult.length > 0) {

                $("#dataTableInstallerEdit").append("<tr>  <th style = 'text-align:center;' > Name</th ><th style='text-align:center;'> Email</th > <th style = 'text-align:center;' > Installer Level</th ><th style = 'text-align:center;' > </th >");

                for (var i = 0; i < data.GetInstallerInfoByWorkOrderResult.length; i++) {
                    $("#dataTableInstallerEdit").append("<tr><td>" +
                        data.GetInstallerInfoByWorkOrderResult[i].Name + "</td> <td>" +
                        data.GetInstallerInfoByWorkOrderResult[i].email + "</td> <td>" +
                        data.GetInstallerInfoByWorkOrderResult[i].InstallerLevel + "</td> <td>" +
                        "<a href='#'" + " id='aInstaller" + data.GetInstallerInfoByWorkOrderResult[i].recordid + "' onclick=\"InstallerInfoView(" + data.GetInstallerInfoByWorkOrderResult[i].recordid + ")\"> View  </a >" +
                        " &nbsp;&nbsp;&nbsp;&nbsp;<a href='#'" + " onclick=\"InstallerInfoDelete(" + data.GetInstallerInfoByWorkOrderResult[i].DetailRecordId + ")\"> Delete </a >" + "</td ></tr > ");
                
                    //a href='#'" + onclick > google </a > " + "</td ></tr > ");

                    //"add" + "</td></tr>");
                }
                //" <div>" + " onclick=\"ShowWOBigPicture(" + data.GetWOPictureResult[i].DetailRecordId + ")\">" +
                //  $("#SeniorInstaller").html(data.GetInstallersResult[0].SeniorInstaller != null && data.GetInstallersResult[0].SeniorInstaller.trim().length > 0 ? data.GetInstallersResult[0].SeniorInstaller : "Unspecified");
                //$("#CrewNames").html(data.GetInstallersResult[0].CrewNames != null && data.GetInstallersResult[0].CrewNames.trim().length > 0 ? data.GetInstallersResult[0].CrewNames : "Un assigned");
               




            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

  
}

function AddRecordToList(recordID) {
    if (recordIDArray.includes(recordID)) {
        recordIDArray.splice(recordIDArray.indexOf(recordID), 1);
    }
    else {
        recordIDArray.push(recordID);
    }
   
}



function AddCrewsToTruck() {
    
    var detailedRecordid = recordIDArray;

    $.ajax({
        url: 'data.svc/UpdateTruckInstalltionCrew?recordid=' + truckRecordID
            + '&IsAdd=1'
            + '&detailedRecordID=' + detailedRecordid ,
        type: "POST",
        success: function (data) {
            if (debug) console.log("events.success", "data.UpdateTruckInstalltionCrew:");
           
            LoadTruckCrews(truckRecordID, "");
            $("#txtTruckCrewName").val('');
            $("#dataTableTruckInstallerAdd tr").remove();
           
            //close the second pop up or reload the table 

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });
}

function AddInstallersToEvent() {

    var recordid = recordIDArray;
    
        $.ajax({
            url: 'data.svc/UpdateCrewData?recordid=' + recordid
                + '&IsAdd=1' 
                + '&WO=' + WO,
            type: "POST",
            success: function (data) {
                if (debug) console.log("events.success", "data.UpdateCrewData:");
                // InstallerInfoView(parentrecordID);
                $("#installerDetail").hide();

                $("#InstallerNameView").html("");
                $("#InstallerBranchView").html("");
                $("#InstallerDepartmentView").html("");
                $("#InstallerTelephoneView").html("");
                $("#InstallerWorkPhoneView").html("");
                $("#InstallerEmailView").html("");
              //  ShowCrews();
                GetInstallers(WO);
                GetTruckInstallers(WO);
              
                $("#installerAdd .close").click();
                //close the second pop up or reload the table 

            }, error: function (error) {
                console.log('Error', error);
                $('#script-warning').show();
            }
        });
}
function SearchByTruckCrewName() {
  // alert($("#txtName").val());
    var name = $("#txtTruckCrewName").val();
    var userid;
  //  var userIDArray = [];
    var UserIDs = "";
    if (name.length >= 2) {
        $("#dataTableTruckInstallerAdd tr").remove();

        $.ajax({
            //type: "POST",  
            url: 'data.svc/GetUserIdListByTruckRecordID?recordid=' + truckRecordID,
            dataType: 'json',
            async: false,
            success: function (data) {
                if (debug) console.log("events.success", "data.GetUserIdListByTruckRecordIDResult:");
                //  $("#installerAddTitle").html(WO + " Installer Add");
                $("#dataTableTruckInstallerAdd tr").remove();
                if (data.GetUserIdListByTruckRecordIDResult.length > 0) {
                    for (var i = 0; i < data.GetUserIdListByTruckRecordIDResult.length; i++) {
                      //  userIDArray.push(data.GetUserIdListByTruckRecordIDResult[i]);
                        UserIDs = UserIDs +"," + data.GetUserIdListByTruckRecordIDResult[i];
                    }
                }

            }, error: function (error) {
                console.log('Error', error);
                $('#script-warning').show();
            }
        });

        if (UserIDs.length > 0) {
            UserIDs = UserIDs.trim().substr(1); 
        }
        $.ajax({
            //type: "POST",  
            url: 'data.svc/GetTruckInstallersExcludeUserIDs?userID=' + UserIDs
                + '&name=' + name,
            dataType: 'json',
            async: false,
            success: function (data) {
                if (debug) console.log("events.success", "data.GetTruckInstallersExcludeUserIDs:");

                $("#dataTableTruckInstallerAdd tr").remove();
                if (data.GetTruckInstallersExcludeUserIDsResult.length > 0) {
                    $("#dataTableTruckInstallerAdd").append("<tr>  <th style = 'text-align:center;' > </th > <th style = 'text-align:center;' > Name</th ><th style='text-align:center;'> Email</th > <th style = 'text-align:center;' > Installer Level</th ><th style = 'text-align:center;' > </th >");
                    for (var i = 0; i < data.GetTruckInstallersExcludeUserIDsResult.length; i++) {
                        $("#dataTableTruckInstallerAdd").append("<tr><td>" +
                            //<input type="checkbox" name="vehicle1" value="Bike"> I have a bike<br>
                            "<input type='checkbox'" + " id='chkRecordID" + data.GetTruckInstallersExcludeUserIDsResult[i].recordid + "' onclick=\"AddRecordToList(" + data.GetTruckInstallersExcludeUserIDsResult[i].UserId + ")\"> " + "</td> <td>" +
                            data.GetTruckInstallersExcludeUserIDsResult[i].Name + "</td> <td>" +
                            data.GetTruckInstallersExcludeUserIDsResult[i].email + "</td> <td>" +
                            data.GetTruckInstallersExcludeUserIDsResult[i].InstallerLevel + "</td> <td>" +
                            "<a href='#'" + " id='bInstaller" + data.GetTruckInstallersExcludeUserIDsResult[i].recordid + "' onclick=\"InstallerInfoAddView(" + data.GetTruckInstallersExcludeUserIDsResult[i].recordid + ")\"> View  </a >" + "</td ></tr > ");
                        //if (count > 30) {


                    }
                }

            }, error: function (error) {
                console.log('Error', error);
                $('#script-warning').show();
            }
        });

    }
   
}

function ClearSearch() {
   // $("#txtName").val('');
    AddInstallers();

}


function SearchByWorkOrderNumber() {
    // alert($("#txtName").val());
    var searchWO = $("#txtWorkOrderSearch").val();
   
    //  var userIDArray = [];
 
    if (searchWO.length >= 2) {
        $("#dataTableTruckWO tr").remove();

        $.ajax({
            //type: "POST",  
            url: 'data.svc/GetTruckInstallationEventsByWO?WO=' + searchWO,
            dataType: 'json',
            async: false,
            success: function (data) {
                if (debug) console.log("events.success", "GetTruckInstallationEventsByWOResult:");
                //  $("#installerAddTitle").html(WO + " Installer Add");
                $("#dataTableTruckInstallerAdd tr").remove();
                if (data.GetTruckInstallationEventsByWOResult.length > 0) {
                    $("#dataTableTruckWO").append("<tr> <th style = 'text-align:center;' > Work Order </th >  <th style = 'text-align:center;' > Start </th ><th style='text-align:center;'> End</th > <th style = 'text-align:center;' > </th >");
                    for (var i = 0; i < data.GetTruckInstallationEventsByWOResult.length; i++) {
                        //  userIDArray.push(data.GetUserIdListByTruckRecordIDResult[i]);
                        $("#dataTableTruckWO").append("<tr><td>" +
                            data.GetTruckInstallationEventsByWOResult[i].WorkOrderNumber + "</td> <td>" +
                            data.GetTruckInstallationEventsByWOResult[i].start + "</td> <td>" +
                            data.GetTruckInstallationEventsByWOResult[i].end + "</td> <td>" +
                            "<a href='#'" + " id='bWOSelected" + data.GetTruckInstallationEventsByWOResult[i].id + "' onclick=\"WOSelected('" + data.GetTruckInstallationEventsByWOResult[i].ActionItemId + "','" + data.GetTruckInstallationEventsByWOResult[i].WorkOrderNumber + "','" + data.GetTruckInstallationEventsByWOResult[i].start + "','" + data.GetTruckInstallationEventsByWOResult[i].end + "')\"> Select  </a >" + "</td ></tr > ");
                    }
                }

            }, error: function (error) {
                console.log('Error', error);
                $('#script-warning').show();
            }
        });


    }

}

function WOSelected(ActionItemId,TruckWO,start, end) {
  //  var searchWO = $("#txtWorkOrderSearch").val();
    $("#dataTableTruckWO tr").remove();
    AssignedTruckWithWO = ActionItemId;
    WO = TruckWO;
    $("#TruckWorkOrder").html(TruckWO);

    //$("#TruckInstallScheduledStartDate").val(moment(start, 'MM/DD/YYYY'));

    $("#TruckInstallScheduledStartDate").val(new Date(GetDatefromMoment(start)).toLocaleDateString('en-US'));
    $("#TruckInstallScheduledStartTime").val(moment(start).add(0, 'hours').format('HH:mm'));


    
   // $("#TruckInstallScheduledEndDate").val(moment(end, 'MM/DD/YYYY'));
    $("#TruckInstallScheduledEndDate").val(new Date(GetDatefromMoment(end)).toLocaleDateString('en-US'));
    
    $("#TruckInstallScheduledEndTime").val(moment(end).add(0, 'hours').format('HH:mm'));


   // $("#TruckWorkOrderCrewNames").html("Kyle Wowk,Stephen Wilkins");
   // $("#dvDateAndCrews").style.display = "block";
    
    document.getElementById('dvDateAndCrews').style.display = "block";
    GetTruckInstallers(TruckWO);
  
}

function GetTruckInstallers(workOrder) {
    $("#AddTruckWOCrewNames").html("<a href='#'" + " id='AddInstallersPop' onclick=\"AddInstallers()\"> Add Installers");
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetInstallers?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetInstallers:");


            if (data.GetInstallersResult.length > 0) {

               // $("#SeniorInstaller").html(data.GetInstallersResult[0].SeniorInstaller != null && data.GetInstallersResult[0].SeniorInstaller.trim().length > 0 ? data.GetInstallersResult[0].SeniorInstaller : "Unspecified");
                //    $("#CrewNames").html(data.GetInstallersResult[0].CrewNames != null && data.GetInstallersResult[0].CrewNames.trim().length > 0 ? data.GetInstallersResult[0].CrewNames : "Un assigned");
                //   $("#CrewNames").html("<a href='https://www.google.com'> text </a> ");

                if (data.GetInstallersResult[0].CrewNames == null || data.GetInstallersResult[0].CrewNames.trim().length == 0) {
                    $("#TruckWorkOrderCrewNames").html("Un assigned");

                    $("#ViewDeleteTruckWOCrewNames").hide();
                }
                else {
                    $("#ViewDeleteTruckWOCrewNames").html("<a href='#'" + " id='InstallerInfoPop' onclick=\"ShowCrews()\"> View/Delete Installers");


                    $("#ViewDeleteTruckWOCrewNames").show();
                    // $("#CrewNames").html("<a href='#'" + " id='InstallerInfoPop' onclick=\"ShowCrews()\">"  + data.GetInstallersResult[0].CrewNames +  "</a >");
                    $("#TruckWorkOrderCrewNames").html(data.GetInstallersResult[0].CrewNames);

                    //  $("#CrewNames").html("<input  type='button' class='btn' data-toggle='modal' href='#stack2' value='Click me'>" );

                    //  $('#eventContent').modal('hide');

                    //   $("#CrewNames").html("<a href='#' data-toggle='modal' data-target='#stack2' >Load me" + "</a >");
                }
            } else {
                $("#ViewDeleteTruckWOCrewNames").hide();

            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}

//function AddTruckInstallers() {
//    recordIDArray = [];
//    // var name = $("#txtName").val();
//    $("#txtName").val('');

//    $("#dataTableInstallerAdd tr").remove();

//    $("#AddInstallersPop").attr('href', 'javascript:void(0);');
//    $("#AddInstallersPop").attr('data-toggle', "modal");
//    $("#AddInstallersPop").attr('data-target', "#installerAdd");
//    $("#AddInstallersPop").attr('href', "/details");

//    $("#eventContent").css('opacity', 20);
//    $("#installerAddDetail").hide();

//    $.ajax({
//        //type: "POST",  
//        url: 'data.svc/GetInstallerInfoExceptWorkOrder?workOrderNumber=' + WO,
//        dataType: 'json',
//        async: false,
//        success: function (data) {
//            if (debug) console.log("events.success", "data.GetInstallerInfoExceptWorkOrderResult:");
//            $("#installerAddTitle").html(WO + " Installer Add");

//            if (data.GetInstallerInfoExceptWorkOrderResult.length > 0) {
//                recordIDArray = [];
//                $("#dataTableInstallerAdd").append("<tr>  <th style = 'text-align:center;' > </th > <th style = 'text-align:center;' > Name</th ><th style='text-align:center;'> Email</th > <th style = 'text-align:center;' > Installer Level</th ><th style = 'text-align:center;' > </th >");
//                var count = data.GetInstallerInfoExceptWorkOrderResult.length;
//                for (var i = 0; i < data.GetInstallerInfoExceptWorkOrderResult.length; i++) {
//                    $("#dataTableInstallerAdd").append("<tr><td>" +
//                        "<input type='checkbox'" + " id='chkRecordID" + data.GetInstallerInfoExceptWorkOrderResult[i].recordid + "' onclick=\"AddRecordToList(" + data.GetInstallerInfoExceptWorkOrderResult[i].recordid + ")\"> " + "</td> <td>" +
//                        data.GetInstallerInfoExceptWorkOrderResult[i].Name + "</td> <td>" +
//                        data.GetInstallerInfoExceptWorkOrderResult[i].email + "</td> <td>" +
//                        data.GetInstallerInfoExceptWorkOrderResult[i].InstallerLevel + "</td> <td>" +
//                        "<a href='#'" + " id='bInstaller" + data.GetInstallerInfoExceptWorkOrderResult[i].recordid + "' onclick=\"InstallerInfoView(" + data.GetInstallerInfoExceptWorkOrderResult[i].recordid + ")\"> View  </a >" + "</td ></tr > ");

//                    if (count > 30) {
//                        $('#bInstaller' + data.GetInstallerInfoExceptWorkOrderResult[i].recordid).addClass('disabled');
//                    }
//                    //a href='#'" + onclick > google </a > " + "</td ></tr > ");

//                    //"add" + "</td></tr>");
//                }
//                //" <div>" + " onclick=\"ShowWOBigPicture(" + data.GetWOPictureResult[i].DetailRecordId + ")\">" +
//                //  $("#SeniorInstaller").html(data.GetInstallersResult[0].SeniorInstaller != null && data.GetInstallersResult[0].SeniorInstaller.trim().length > 0 ? data.GetInstallersResult[0].SeniorInstaller : "Unspecified");
//                //$("#CrewNames").html(data.GetInstallersResult[0].CrewNames != null && data.GetInstallersResult[0].CrewNames.trim().length > 0 ? data.GetInstallersResult[0].CrewNames : "Un assigned");





//            }

//        }, error: function (error) {
//            console.log('Error', error);
//            $('#script-warning').show();
//        }
//    });


//}
function AssignTruckToWO() {


    $.ajax({
        url: 'data.svc/UpdateTruckWithWo?ActionItemId=' + AssignedTruckWithWO
            + '&TruckName=' + truckName.replace("(Add WorkOrder)", "").trim()
            + '&TruckID=' + truckRecordID,
          
        type: "POST",
        success: function (data) {
            if (debug) console.log("events.success", "data.UpdateTruckWithWo:");

            $("#TruckWorkOrderPop .close").click();
            $('#calendar').fullCalendar('refetchResources');
            $('#calendar').fullCalendar('rerenderResources');

            $('#calendar').fullCalendar('refetchEvents');
            $('#calendar').fullCalendar('rerenderEvents');

            $('#calendar').fullCalendar('changeView', 'timelineDay');

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

    SaveTruckInstallationSchedule();
}

function SaveTruckInstallationSchedule() {
    var isAllDayChecked = "";
    if ($("#IsTruckAllDay")[0].checked) {
        isAllDayChecked = "Yes";
    }
    var startDate = $("#TruckInstallScheduledStartDate").val();
    var startTime = $("#TruckInstallScheduledStartTime").val();

    var endDate = $("#TruckInstallScheduledEndDate").val();
    var endTime = $("#TruckInstallScheduledEndTime").val();

    $.ajax({
        url: 'data.svc/UpdateTruckInstallationSchedule?ActionItemId=' + AssignedTruckWithWO
            + '&startDate=' + startDate
            + '&startTime=' + startTime
            + '&endDate=' + endDate
            + '&endTime=' + endTime
            + '&isAllDayChecked=' + isAllDayChecked,
        type: "POST",
        success: function (data) {
            if (debug) console.log("events.success", "data.UpdateTruckWithWo:");

            $("#TruckWorkOrderPop .close").click();
            $('#calendar').fullCalendar('refetchResources');
            $('#calendar').fullCalendar('rerenderResources');

            $('#calendar').fullCalendar('refetchEvents');
            $('#calendar').fullCalendar('rerenderEvents');

            $('#calendar').fullCalendar('changeView', 'timelineDay');

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });
}

function SearchByName() {
    // alert($("#txtName").val());
    var name = $("#txtName").val();
    if (name.length >= 2) {
        $("#dataTableInstallerAdd tr").remove();

        $("#AddInstallersPop").attr('href', 'javascript:void(0);');
        $("#AddInstallersPop").attr('data-toggle', "modal");
        $("#AddInstallersPop").attr('data-target', "#installerAdd");
        $("#AddInstallersPop").attr('href', "/details");

        $("#eventContent").css('opacity', 20);
        $("#installerAddDetail").hide();

        $.ajax({
            //type: "POST",  
            url: 'data.svc/GetInstallerInfoByNameExceptWorkOrder?workOrderNumber=' + WO
                + '&name=' + name,
            dataType: 'json',
            async: false,
            success: function (data) {
                if (debug) console.log("events.success", "data.GetInstallerInfoByNameExceptWorkOrderResult:");
                //  $("#installerAddTitle").html(WO + " Installer Add");

                if (data.GetInstallerInfoByNameExceptWorkOrderResult.length > 0) {
                    recordIDArray = [];
                    // $("#dataTableInstallerAdd").append("<tr>  <th style = 'text-align:center;' > </th > < th style = 'text-align:center;' > Name</th > <th style='text-align:center;'> Email</th > <th style='text-align:center;' > Installer Level</th > <th style='text-align:center;' > </th >");
                    $("#dataTableInstallerAdd").append("<tr>  <th style = 'text-align:center;' > </th > <th style = 'text-align:center;' > Name</th ><th style='text-align:center;'> Email</th > <th style = 'text-align:center;' > Installer Level</th ><th style = 'text-align:center;' > </th >");
                    var count = data.GetInstallerInfoByNameExceptWorkOrderResult.length;
                    for (var i = 0; i < data.GetInstallerInfoByNameExceptWorkOrderResult.length; i++) {
                        $("#dataTableInstallerAdd").append("<tr><td>" +
                            //<input type="checkbox" name="vehicle1" value="Bike"> I have a bike<br>
                            "<input type='checkbox'" + " id='chkRecordID" + data.GetInstallerInfoByNameExceptWorkOrderResult[i].recordid + "' onclick=\"AddRecordToList(" + data.GetInstallerInfoByNameExceptWorkOrderResult[i].recordid + ")\"> " + "</td> <td>" +
                            data.GetInstallerInfoByNameExceptWorkOrderResult[i].Name + "</td> <td>" +
                            data.GetInstallerInfoByNameExceptWorkOrderResult[i].email + "</td> <td>" +
                            data.GetInstallerInfoByNameExceptWorkOrderResult[i].InstallerLevel + "</td> <td>" +
                            "<a href='#'" + " id='bInstaller" + data.GetInstallerInfoByNameExceptWorkOrderResult[i].recordid + "' onclick=\"InstallerInfoAddView(" + data.GetInstallerInfoByNameExceptWorkOrderResult[i].recordid + ")\"> View  </a >" + "</td ></tr > ");
                        if (count > 30) {
                            $('#bInstaller' + data.GetInstallerInfoByNameExceptWorkOrderResult[i].recordid).addClass('disabled');
                        }
                    }
                }

            }, error: function (error) {
                console.log('Error', error);
                $('#script-warning').show();
            }
        });

    }

}


//function TruckCrewEdit(obj) {
    
//    // var name = $("#txtName").val();
//    $("#txtName").val('');

//    $("#dataTableInstallerAdd tr").remove();

//          //$(this).attr('data-toggle', "modal");
//                //$(this).attr('data-target', "#taskModal");
//                //$(this).attr('href', "/details");


//    $(this).attr('href', 'javascript:void(0);');
//    $(this).attr('data-toggle', "modal");
//    $(this).attr('data-target', "#TruckPop");
//    $(this).attr('href', "/details");

//    //$("#eventContent").css('opacity', 20);
//    $("#installerAddDetail").hide();
//    WO = "";
//    $.ajax({
//        //type: "POST",  
//        url: 'data.svc/GetInstallerInfoExceptWorkOrder?workOrderNumber=' + WO,
//        dataType: 'json',
//        async: false,
//        success: function (data) {
//            if (debug) console.log("events.success", "data.GetInstallerInfoExceptWorkOrderResult:");
//            $("#installerAddTitle").html(WO + " Installer Add");

//            if (data.GetInstallerInfoExceptWorkOrderResult.length > 0) {
//                recordIDArray = [];
//                $("#dataTableInstallerAdd").append("<tr>  <th style = 'text-align:center;' > </th > <th style = 'text-align:center;' > Name</th ><th style='text-align:center;'> Email</th > <th style = 'text-align:center;' > Installer Level</th ><th style = 'text-align:center;' > </th >");
//                var count = data.GetInstallerInfoExceptWorkOrderResult.length;
//                for (var i = 0; i < data.GetInstallerInfoExceptWorkOrderResult.length; i++) {
//                    $("#dataTableInstallerAdd").append("<tr><td>" +
//                        "<input type='checkbox'" + " id='chkRecordID" + data.GetInstallerInfoExceptWorkOrderResult[i].recordid + "' onclick=\"AddRecordToList(" + data.GetInstallerInfoExceptWorkOrderResult[i].recordid + ")\"> " + "</td> <td>" +
//                        data.GetInstallerInfoExceptWorkOrderResult[i].Name + "</td> <td>" +
//                        data.GetInstallerInfoExceptWorkOrderResult[i].email + "</td> <td>" +
//                        data.GetInstallerInfoExceptWorkOrderResult[i].InstallerLevel + "</td> <td>" +
//                        "<a href='#'" + " id='bInstaller" + data.GetInstallerInfoExceptWorkOrderResult[i].recordid + "' onclick=\"InstallerInfoView(" + data.GetInstallerInfoExceptWorkOrderResult[i].recordid + ")\"> View  </a >" + "</td ></tr > ");

//                    if (count > 30) {
//                        $('#bInstaller' + data.GetInstallerInfoExceptWorkOrderResult[i].recordid).addClass('disabled');
//                    }
//                    //a href='#'" + onclick > google </a > " + "</td ></tr > ");

//                    //"add" + "</td></tr>");
//                }
//                //" <div>" + " onclick=\"ShowWOBigPicture(" + data.GetWOPictureResult[i].DetailRecordId + ")\">" +
//                //  $("#SeniorInstaller").html(data.GetInstallersResult[0].SeniorInstaller != null && data.GetInstallersResult[0].SeniorInstaller.trim().length > 0 ? data.GetInstallersResult[0].SeniorInstaller : "Unspecified");
//                //$("#CrewNames").html(data.GetInstallersResult[0].CrewNames != null && data.GetInstallersResult[0].CrewNames.trim().length > 0 ? data.GetInstallersResult[0].CrewNames : "Un assigned");





//            }

//        }, error: function (error) {
//            console.log('Error', error);
//            $('#script-warning').show();
//        }
//    });


//}

function AddInstallers() {
    recordIDArray = [];
   // var name = $("#txtName").val();
    $("#txtName").val('');
    
    $("#dataTableInstallerAdd tr").remove();

    //$("#AddInstallersPop").attr('href', 'javascript:void(0);');
    //$("#AddInstallersPop").attr('data-toggle', "modal");
    //$("#AddInstallersPop").attr('data-target', "#installerAdd");
    //$("#AddInstallersPop").attr('href', "/details");

    $('#installerAdd').modal('show');

    $("#eventContent").css('opacity', 20);
    $("#installerAddDetail").hide();

    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetInstallerInfoExceptWorkOrder?workOrderNumber=' + WO,
        dataType: 'json',
        async: false,
        success: function (data) {
            if (debug) console.log("events.success", "data.GetInstallerInfoExceptWorkOrderResult:");
            $("#installerAddTitle").html(WO + " Installer Add");

            if (data.GetInstallerInfoExceptWorkOrderResult.length > 0) {
                recordIDArray = [];
                $("#dataTableInstallerAdd").append("<tr>  <th style = 'text-align:center;' > </th > <th style = 'text-align:center;' > Name</th ><th style='text-align:center;'> Email</th > <th style = 'text-align:center;' > Installer Level</th ><th style = 'text-align:center;' > </th >");
                var count = data.GetInstallerInfoExceptWorkOrderResult.length;
                for (var i = 0; i < data.GetInstallerInfoExceptWorkOrderResult.length; i++) {
                    $("#dataTableInstallerAdd").append("<tr><td>" +
                        "<input type='checkbox'" + " id='chkRecordID" + data.GetInstallerInfoExceptWorkOrderResult[i].recordid +  "' onclick=\"AddRecordToList(" + data.GetInstallerInfoExceptWorkOrderResult[i].recordid + ")\"> " + "</td> <td>" +
                        data.GetInstallerInfoExceptWorkOrderResult[i].Name + "</td> <td>" +
                        data.GetInstallerInfoExceptWorkOrderResult[i].email + "</td> <td>" +
                        data.GetInstallerInfoExceptWorkOrderResult[i].InstallerLevel + "</td> <td>" +
                        "<a href='#'" + " id='bInstaller" + data.GetInstallerInfoExceptWorkOrderResult[i].recordid + "' onclick=\"InstallerInfoView(" + data.GetInstallerInfoExceptWorkOrderResult[i].recordid + ")\"> View  </a >" + "</td ></tr > ");
                       
                    if (count > 30) {
                        $('#bInstaller' + data.GetInstallerInfoExceptWorkOrderResult[i].recordid).addClass('disabled');
                    }
                    //a href='#'" + onclick > google </a > " + "</td ></tr > ");

                    //"add" + "</td></tr>");
                }
                //" <div>" + " onclick=\"ShowWOBigPicture(" + data.GetWOPictureResult[i].DetailRecordId + ")\">" +
                //  $("#SeniorInstaller").html(data.GetInstallersResult[0].SeniorInstaller != null && data.GetInstallersResult[0].SeniorInstaller.trim().length > 0 ? data.GetInstallersResult[0].SeniorInstaller : "Unspecified");
                //$("#CrewNames").html(data.GetInstallersResult[0].CrewNames != null && data.GetInstallersResult[0].CrewNames.trim().length > 0 ? data.GetInstallersResult[0].CrewNames : "Un assigned");





            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });


}

function GetInstallers(workOrder) {
    $("#AddCrewNames").html("<a href='#'" + " id='AddInstallersPop' onclick=\"AddInstallers()\"> Add Installers");
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetInstallers?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetInstallers:");

              
            if (data.GetInstallersResult.length > 0) {

                $("#SeniorInstaller").html(data.GetInstallersResult[0].SeniorInstaller != null && data.GetInstallersResult[0].SeniorInstaller.trim().length > 0 ? data.GetInstallersResult[0].SeniorInstaller : "Unspecified");
            //    $("#CrewNames").html(data.GetInstallersResult[0].CrewNames != null && data.GetInstallersResult[0].CrewNames.trim().length > 0 ? data.GetInstallersResult[0].CrewNames : "Un assigned");
             //   $("#CrewNames").html("<a href='https://www.google.com'> text </a> ");
            
                if (data.GetInstallersResult[0].CrewNames == null || data.GetInstallersResult[0].CrewNames.trim().length == 0) {
                    $("#CrewNames").html("Un assigned");
                   
                    $("#ViewDeleteCrewNames").hide();
                }
                else {
                    $("#ViewDeleteCrewNames").html("<a href='#'" + " id='InstallerInfoPop' onclick=\"ShowCrews()\"> View/Delete Installers");
                    
                    
                  $("#ViewDeleteCrewNames").show();
                 // $("#CrewNames").html("<a href='#'" + " id='InstallerInfoPop' onclick=\"ShowCrews()\">"  + data.GetInstallersResult[0].CrewNames +  "</a >");
                  $("#CrewNames").html(data.GetInstallersResult[0].CrewNames);

                    //  $("#CrewNames").html("<input  type='button' class='btn' data-toggle='modal' href='#stack2' value='Click me'>" );
                 
                  //  $('#eventContent').modal('hide');
                 
                 //   $("#CrewNames").html("<a href='#' data-toggle='modal' data-target='#stack2' >Load me" + "</a >");
                }
            } else {
                $("#ViewDeleteCrewNames").hide();

            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}


//function UpdateCallLog() {
   
//    var id = eventid;

//    var notes;
//    var callDate,callTime;
//    var calledMessage ;

//    if (CalledLogArray.length == 0)  //new called log
//    {
//        notes = $("#comment").val();
//        callDate = $("#calledLogDate").val();
//        callTime = $("#CalledLogTime").val();
//        calledMessage = $('#MessageOption').val();
//        $.ajax({
//            url: 'data.svc/UpdateCallLogData?id=' + id
//                + '&callDate=' + callDate
//                + '&callTime=' + callTime
//                + '&calledMessage=' + calledMessage
//                + '&Notes=' + notes,
//            type: "POST",
//            success: function (data) {
//                if (debug) console.log("events.success", "data.UpdateInstallationLog:");

//                //  GetCalledLog(WO);

//            }, error: function (error) {
//                console.log('Error', error);
//                $('#script-warning').show();
//            }
//        });
//    }
//    else {
//        for (var i = 0; i < CalledLogArray.length; i++) {
//            notes = CalledLogArray[i].Notes3;
//            callDate = CalledLogArray[i].DateCalled;
//            calledMessage = CalledLogArray[i].CalledMessage;
//            $.ajax({
//                url: 'data.svc/CreateCallLogData?id=' + id
//                    + '&callDate=' + callDate
//                    + '&calledMessage=' + calledMessage
//                    + '&Notes=' + notes,
//                type: "POST",
//                success: function (data) {
//                    if (debug) console.log("events.success", "data.CreateCallLogData:");

//                    //  GetCalledLog(WO);

//                }, error: function (error) {
//                    console.log('Error', error);
//                    $('#script-warning').show();
//                }
//            });
//        }
//    }




//}

//function AddKeepedCalledLog() {
//    var id = eventid;
//    if (CalledLogArray.length == 0)  //new called log
//    {
//        UpdateCallLog();
//    }
//    else {
//        for (var i = 0; i < CalledLogArray.length; i++) {
//            var notes = $("#comment").val();
//            var callDate = $("#calledLogDate").val();
//            var calledMessage = $('#MessageOption').val();
//            $.ajax({
//                url: 'data.svc/CreateCallLogData?id=' + id
//                    + '&callDate=' + callDate
//                    + '&calledMessage=' + calledMessage
//                    + '&Notes=' + notes,
//                type: "POST",
//                success: function (data) {
//                    if (debug) console.log("events.success", "data.CreateCallLogData:");

//                    //  GetCalledLog(WO);

//                }, error: function (error) {
//                    console.log('Error', error);
//                    $('#script-warning').show();
//                }
//            });
//        }

       
//    }

//}

function UpdateInstallationCallLog() {
    var recordid = $("#callLogRecordID").val();
    var notes;
    var callDate, callTime;
    var calledMessage;

    var id = eventid;

    notes = $("#comment").val();
    callDate = $("#calledLogDate").val();
    callTime = $("#CalledLogTime").val();
    calledMessage = $('#MessageOption').val();

    $.ajax({
        url: 'data.svc/UpdateCallLogData?id=' + id
            + '&WO=' + WO 
            + '&recordid=' + recordid
            + '&callDate=' + callDate
            + '&callTime=' + callTime
            + '&calledMessage=' + calledMessage
            + '&Notes=' + notes,
        type: "POST",
        success: function (data) {
            if (debug) console.log("events.success", "data.UpdateInstallationLog:");
            $("#comment").val('');
            $("#calledLogDate").val('');
            $("#CalledLogTime").val('');
            $('#MessageOption').val('');
            GetCalledLog(WO);

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });
}

function UpdateInstallationNotes() {
    var recordid = $("#notesRecordID").val();
    var notes;
    var notesDate, notesTime;
    var category;

    var id = eventid;

    notes = $("#notes").val();
    notesDate = $("#notesDate").val();
    notesTime = $("#notesTime").val();
    category = $('#CategoryOption').val();

    $.ajax({
        url: 'data.svc/UpdateNotesData?id=' + id
            + '&WO=' + WO
            + '&recordid=' + recordid
            + '&notesDate=' + notesDate
            + '&notesTime=' + notesTime
            + '&category=' + category
            + '&Notes=' + notes,
        type: "POST",
        success: function (data) {
            if (debug) console.log("events.success", "data.UpdateNotesData:");
            $("#notes").val('');
            $("#notesDate").val('');
            $("#notesTime").val('');
            $('#CategoryOption').val('');
            GetNotes(WO);

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });
}

//function processFile(theFile) {
//    return function (e) {
//        var theBytes = e.target.result; //.split('base64,')[1]; // use with uploadFile2
//        fileByteArray.push(theBytes);
//        document.getElementById('file').innerText = '';
//        for (var i = 0; i < fileByteArray.length; i++) {
//            document.getElementById('file').innerText += fileByteArray[i];
//        }
//    }
//}


function UploadDocuments() {
   var fileName = $("#documentFile").val();
   var fileData;
    fileData = fileByteArray;
 
    PageMethods.UploadDocumentFiles( WO,fileName, fileData.toString());
   
}

function CallLogEdit(recordid) {
    $.ajax({
        url: 'data.svc/GetCallLogByID?recordId=' + recordid,
        
        success: function (data) {
            if (debug) console.log("events.success", "data.GetCallLogByID:");
            if (data.GetCallLogByIDResult.length > 0) {
                $("#comment").val(data.GetCallLogByIDResult[0].Notes3);
                $("#calledLogDate").val(data.GetCallLogByIDResult[0].CalledLogDate);
                $("#CalledLogTime").val(data.GetCallLogByIDResult[0].StrCalledLogTime);
                $("#callLogRecordID").val(recordid);
                
                //$("#CalledLogWO").val();
                
             //   $("#calledLogDate").val(new Date(GetDatefromMoment(data.GetCallLogByIDResult[0].DateCalled)).toLocaleDateString('en-US'));

         

                var linkID;
                linkID = "#aCalledLog" + recordid;

                //  if  $(linkID).text('Cancel Editing');
                if ($(linkID).text().trim() == "Edit") {
                    $(linkID).text('Cancel Editing');
                    //disable date time
                    $("#calledLogDate").prop('disabled', true);
                    $("#CalledLogTime").prop('disabled', true);
                    $('#MessageOption').val(data.GetCallLogByIDResult[0].CalledMessage).attr().add('selected');
             
                }
                else {
                    $(linkID).text('Edit');
                    //disable date time
                    $("#calledLogDate").prop('disabled', false);
                    $("#CalledLogTime").prop('disabled', false);
                    $("#comment").val('');
                    //$("#calledLogDate").val('');
                    //$("#CalledLogTime").val('');
                    var today = new Date();
                    var dd = String(today.getDate()).padStart(2, '0');
                    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
                    var yyyy = today.getFullYear();

                    today = mm + '/' + dd + '/' + yyyy;
                    $("#calledLogDate").val(today);
                    var time = new Date().toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: false });

                    $("#CalledLogTime").val(time);
                    $('#MessageOption').val('');
                }
            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    }); 
}

function ShowDocumentFile(recordid) {
    var link = document.getElementById(recordid);

    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetDocumentFile?recordId=' + recordid,
        // dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetDocumentFile:");
            if (data.GetDocumentFileResult.length > 0) {
            

                var pdfWindow = window.open(data.GetDocumentFileResult[0].DocumentFileStr, '_blank');
               
             //   pdfWindow.document.write("<iframe width='100%' height='100%' src='data:application/pdf;base64, " + data.GetDocumentFileResult[0].DocumentFileStr + "'></iframe>")
                pdfWindow.document.write("<iframe width='100%' height='100%' src='" + data.GetDocumentFileResult[0].DocumentFileStr + "'></iframe>");


            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });
}

function CallLogDelete(recordid) {
  
    var notes = "";
    var callDate = "";
    var callTime ="";
    var calledMessage = "";

    var id = eventid;

    var result = confirm("After click 'OK' button, this log info will be deleted.");
    if (result) {
     
          $.ajax({
        url: 'data.svc/UpdateCallLogData?id=' + id
            + '&WO=' + WO
            + '&recordid=' + recordid
            + '&callDate=' + callDate
            + '&callTime=' + callTime
            + '&calledMessage=' + calledMessage
            + '&Notes=' + notes,
        type: "POST",
        success: function (data) {
            if (debug) console.log("events.success", "data.UpdateInstallationLog:");
               GetCalledLog(WO);

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });
    }

    //notes = $("#comment").val();
    //callDate = $("#calledLogDate").val();
    //callTime = $("#CalledLogTime").val();
    //calledMessage = $('#MessageOption').val();


}

function ClearTruckCrewSearch() {
    $("#txtTruckCrewName").val('');

    $("#dataTableTruckInstallerAdd tr").remove();
}

function ClearWorkOrderSearch() {
    $("#txtWorkOrderSearch").val('');

    $("#dataTableTruckWO tr").remove();
}


function DeleteTruckInstallers(truck, recordid,detailedRecordID) {
    var result = confirm("After click 'OK' button, this installer will be removed from this Truck '" + truck + "' !");
    if (result) {

        $.ajax({
            url: 'data.svc/UpdateTruckInstalltionCrew?recordid=' + recordid
                + '&IsAdd=0'
                + '&detailedRecordID=' + detailedRecordID,
            type: "POST",
            success: function (data) {
                if (debug) console.log("events.success", "data.UpdateCrewData:");

                LoadTruckCrews(truckRecordID, "");

                // InstallerInfoView(parentrecordID);
                //$("#installerDetail").hide();

                //$("#InstallerNameView").html("");
                //$("#InstallerBranchView").html("");
                //$("#InstallerDepartmentView").html("");
                //$("#InstallerTelephoneView").html("");
                //$("#InstallerWorkPhoneView").html("");
                //$("#InstallerEmailView").html("");
                //ShowCrews();
                //GetInstallers(WO);

                //  $("#CrewNames").html("123213"); 
                //close the second pop up or reload the table 

            }, error: function (error) {
                console.log('Error', error);
                $('#script-warning').show();
            }
        });
    }
}

function LoadTruckCrews(truckRecordID,title) {
    $("#dataTableTruck tr").remove(); 
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetInstallerListByTruck?recordid=' + truckRecordID,
        dataType: 'json',
        async: false,
        success: function (data) {
            if (debug) console.log("events.success", "data.GetInstallerInfoExceptWorkOrderResult:");
            if (title.length != 0) {
                $("#TruckPopTitle").html(" Truck " + title);
            }
            
            $("#dataTableTruck tr").remove();

            if (data.GetInstallerListByTruckResult.length > 0) {
                recordIDArray = [];
                $("#dataTableTruck").append("<tr> <th style = 'text-align:center;' > Name</th ><th style='text-align:center;'> Email</th > <th style = 'text-align:center;' > Installer Level</th ><th style = 'text-align:center;' > </th >");
                var count = data.GetInstallerListByTruckResult.length;
                for (var i = 0; i < data.GetInstallerListByTruckResult.length; i++) {
                    $("#dataTableTruck").append("<tr><td>" +
                        //   "<input type='checkbox'" + " id='chkRecordID" + data.GetInstallerListByTruckResult[i].recordid + "' onclick=\"AddRecordToList(" + data.GetInstallerListByTruckResult[i].recordid + ")\"> " + "</td> <td>" +
                        data.GetInstallerListByTruckResult[i].Name + "</td> <td>" +
                        data.GetInstallerListByTruckResult[i].email + "</td> <td>" +
                        data.GetInstallerListByTruckResult[i].InstallerLevel + "</td> <td>" +
                        "<a href='#'" + " id='bInstaller" + data.GetInstallerListByTruckResult[i].recordid + "' onclick=\"DeleteTruckInstallers(" + title.replace(' ( Click to edit Crews)', '') + "," + data.GetInstallerListByTruckResult[i].recordid + "," + data.GetInstallerListByTruckResult[i].DetailRecordId + ")\"> Delete  </a >" + "</td ></tr > ");

                    //if (count > 30) {
                    //    $('#bInstaller' + data.GetInstallerListByTruckResult[i].recordid).addClass('disabled');
                    //}
                    //a href='#'" + onclick > google </a > " + "</td ></tr > ");

                    //"add" + "</td></tr>");
                }
                //" <div>" + " onclick=\"ShowWOBigPicture(" + data.GetWOPictureResult[i].DetailRecordId + ")\">" +
                //  $("#SeniorInstaller").html(data.GetInstallersResult[0].SeniorInstaller != null && data.GetInstallersResult[0].SeniorInstaller.trim().length > 0 ? data.GetInstallersResult[0].SeniorInstaller : "Unspecified");
                //$("#CrewNames").html(data.GetInstallersResult[0].CrewNames != null && data.GetInstallersResult[0].CrewNames.trim().length > 0 ? data.GetInstallersResult[0].CrewNames : "Un assigned");





            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });
}
function InstallerInfoDelete(recordid,parentrecordID) {
    var result = confirm("After click 'OK' button, this installer will be removed from this Work Order '" + WO + "' !");
    if (result) {

        $.ajax({
            url: 'data.svc/UpdateCrewData?recordid=' + recordid
                + '&IsAdd=0'
            + '&WO=' + WO,
            type: "POST",
            success: function (data) {
                if (debug) console.log("events.success", "data.UpdateCrewData:");
               // InstallerInfoView(parentrecordID);
                $("#installerDetail").hide();

                $("#InstallerNameView").html("");
                $("#InstallerBranchView").html("");
                $("#InstallerDepartmentView").html("");
                $("#InstallerTelephoneView").html("");
                $("#InstallerWorkPhoneView").html("");
                $("#InstallerEmailView").html("");
                ShowCrews();
                GetInstallers(WO);
                GetTruckInstallers(WO);
              
              //  $("#CrewNames").html("123213"); 
                //close the second pop up or reload the table 

            }, error: function (error) {
                console.log('Error', error);
                $('#script-warning').show();
            }
        });
    }
}


function InstallerInfoView(recordid) {

    $.ajax({
        url: 'data.svc/GetInstallerInfoByRecordID?recordid=' + recordid,

        success: function (data) {
            if (debug) console.log("events.success", "data.GetInstallerInfoByRecordID:");
            if (data.GetInstallerInfoByRecordIDResult != null) {
                $("#InstallerNameView").html(data.GetInstallerInfoByRecordIDResult.Name);
                $("#InstallerBranchView").html(data.GetInstallerInfoByRecordIDResult.Branch);
                $("#InstallerDepartmentView").html(data.GetInstallerInfoByRecordIDResult.Department);
                $("#InstallerTelephoneView").html(data.GetInstallerInfoByRecordIDResult.Telephone);
                $("#InstallerWorkPhoneView").html(data.GetInstallerInfoByRecordIDResult.WorkPhoneNumber);
                $("#InstallerEmailView").html(data.GetInstallerInfoByRecordIDResult.email);
               // $("#installerImg").src = data.GetInstallerInfoByRecordIDResult.picString;
                $("#installerImg").attr('src', 'images/installerimage.jpg');
                if (data.GetInstallerInfoByRecordIDResult.picString != null) {
                    $("#installerImg").attr('src', data.GetInstallerInfoByRecordIDResult.picString);
                }
               
                $("#installerImg").css({ 'width': '250px', 'height': '250px' });
               
            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });
    $("#installerDetail").show();

}



function InstallerInfoAddView(recordid) {
    
    $("#txtName").val('');
    $.ajax({
        url: 'data.svc/GetInstallerInfoByRecordID?recordid=' + recordid,

        success: function (data) {
            if (debug) console.log("events.success", "data.GetInstallerInfoByRecordID:");
            if (data.GetInstallerInfoByRecordIDResult != null) {
                $("#InstallerNameAdd").html(data.GetInstallerInfoByRecordIDResult.Name);
                $("#InstallerBranchAdd").html(data.GetInstallerInfoByRecordIDResult.Branch);
                $("#InstallerDepartmentAdd").html(data.GetInstallerInfoByRecordIDResult.Department);
                $("#InstallerTelephoneAdd").html(data.GetInstallerInfoByRecordIDResult.Telephone);
                $("#InstallerWorkPhoneAdd").html(data.GetInstallerInfoByRecordIDResult.WorkPhoneNumber);
                $("#InstallerEmailViewAdd").html(data.GetInstallerInfoByRecordIDResult.email);
                // $("#installerImg").src = data.GetInstallerInfoByRecordIDResult.picString;
                $("#AddinstallerImg").attr('src', 'images/installerimage.jpg');
                if (data.GetInstallerInfoByRecordIDResult.picString != null) {
                    $("#AddinstallerImg").attr('src', data.GetInstallerInfoByRecordIDResult.picString);
                }
              //  $("#AddinstallerImg").attr('src', data.GetInstallerInfoByRecordIDResult.picString);
                $("#AddinstallerImg").css({ 'width': '250px', 'height': '250px' });

            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });
    $("#installerAddDetail").show();

}

function NotesEdit(recordid) {

    $.ajax({
        url: 'data.svc/GetNotesByID?recordId=' + recordid,

        success: function (data) {
            if (debug) console.log("events.success", "data.GetNotesByID:");
            if (data.GetNotesByIDResult.length > 0) {
                $("#notes").val(data.GetNotesByIDResult[0].GeneralNotes);
                $("#notesDate").val(data.GetNotesByIDResult[0].StrNotesDate);
                $("#notesTime").val(data.GetNotesByIDResult[0].StrNotesTime);
                $("#notesRecordID").val(recordid);
                var linkID;
                linkID = "#aNotes" + recordid;

                if ($(linkID).text().trim() == "Edit") {
                    $(linkID).text('Cancel Editing');
                    //disable date time
                    $("#notesDate").prop('disabled', true);
                    $("#notesTime").prop('disabled', true);
                 
                    $('#CategoryOption').val(data.GetNotesByIDResult[0].Category).attr().add('selected');
                }
                else {
                    $(linkID).text('Edit');
                    //disable date time
                    $("#notesDate").prop('disabled', false);
                    $("#notesTime").prop('disabled', false);
                    //$("#notesDate").val('');
                    //$("#notesTime").val('');

                    var today = new Date();
                    var dd = String(today.getDate()).padStart(2, '0');
                    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
                    var yyyy = today.getFullYear();

                    today = mm + '/' + dd + '/' + yyyy;
                    $("#notesDate").val(today);
                    var time = new Date().toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: false });

                    $("#notesTime").val(time);

                    $('#CategoryOption').val('');
                    $("#notes").val('');
                }


              //  if  $(linkID).text('Cancel Editing');
                
                //$("#CalledLogWO").val();

                //   $("#calledLogDate").val(new Date(GetDatefromMoment(data.GetCallLogByIDResult[0].DateCalled)).toLocaleDateString('en-US'));

          
        

             

                //if ($(linkID).text().trim() == "Edit") {
                //    $(linkID).text('Cancel Editing');
                //    //disable date time
                //    $("#notesDate").prop('disabled', true);
                //    $("#notesTime").prop('disabled', true);
                //}
                //else {
                //    $(linkID).text('Edit');
                //    //disable date time
                //    $("#notesDate").prop('disabled', false);
                //    $("#notesTime").prop('disabled', false);
                //    //$("#notesDate").val('');
                //    //$("#notesTime").val('');
                //    //$('#CategoryOption').val('');
                //    //$("#notes").val('');
                //}

            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });
}

function IsTruckAlldayChecked() {
    var IsTruckAllDay = $("#IsTruckAllDay");
    //$("#TruckInstallScheduledStartTime").show();
    //$("#TruckInstallScheduledEndTime").show();

    document.getElementById("TruckInstallScheduledStartTime").style.visibility = "visible";
    document.getElementById("TruckInstallScheduledEndTime").style.visibility = "visible";

    if (IsTruckAllDay[0].checked) {

        //$("#TruckInstallScheduledStartTime").hide();
        //$("#TruckInstallScheduledEndTime").hide();
        document.getElementById("TruckInstallScheduledStartTime").style.visibility = "hidden";
        document.getElementById("TruckInstallScheduledEndTime").style.visibility = "hidden";
    }
   
}

function IsAllDayChecked() {
    var IsAllDay = $("#IsAllDay");
    //$("#TruckInstallScheduledStartTime").show();
    //$("#TruckInstallScheduledEndTime").show();

    document.getElementById("InstallScheduledStartTime").style.visibility = "visible";
    document.getElementById("InstallScheduledEndTime").style.visibility = "visible";

    if (IsAllDay[0].checked) {

        //$("#TruckInstallScheduledStartTime").hide();
        //$("#TruckInstallScheduledEndTime").hide();
        document.getElementById("InstallScheduledStartTime").style.visibility = "hidden";
        document.getElementById("InstallScheduledEndTime").style.visibility = "hidden";
    }

}

function SaveTruckModal() {
    var titile = $("#AddTitle").val();

}
function NotesDelete(recordid) {

    var notes = "";
    var notesDate = "";
    var notesTime = "";
    var category = "";

    var id = eventid;

    var result = confirm("After click 'OK' button, this note info will be deleted.");
    if (result) {

        $.ajax({
            url: 'data.svc/UpdateNotesData?id=' + id
                + '&WO=' + WO
                + '&recordid=' + recordid
                + '&notesDate=' + notesDate
                + '&notesTime=' + notesTime
                + '&category=' + category
                + '&Notes=' + notes,
            type: "POST",
            success: function (data) {
                if (debug) console.log("events.success", "data.UpdateNotesData:");
                GetNotes(WO);

            }, error: function (error) {
                console.log('Error', error);
                $('#script-warning').show();
            }
        });
    }


}

function GetNotes(workOrder) {
    $("#dataTableNotes tr").remove();
    //enable date time
    $("#notesDate").prop('disabled', false);
    $("#notesTime").prop('disabled', false);


    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();

    today = mm + '/' + dd + '/' + yyyy;
    $("#notesDate").val(today);
    var time = new Date().toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: false });

    $("#notesTime").val(time);
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetNotes?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetNotes:");


            if (data.GetNotesResult.length > 0) {

                //$("#DateCalled").html(new Date(GetDatefromMoment(data.GetCalledLogResult[0].DateCalled)).toLocaleDateString('en-US'));
                //$("#CalledMessage").html(data.GetCalledLogResult[0].CalledMessage);
                //$("#CalledLogNotes").html(data.GetCalledLogResult[0].Notes3);

                $("#dataTableNotes").append("<tr>  <th style = 'text-align:center;' > Date</th ><th style='text-align:center;'> Category</th > <th style = 'text-align:center;' > Notes</th ><th style = 'text-align:center;' > </th >");

                for (var i = 0; i < data.GetNotesResult.length; i++) {
                    $("#dataTableNotes").append("<tr><td>" +
                        new Date(GetDatefromMoment(data.GetNotesResult[i].NotesDate)).toLocaleString('en-US') + "</td> <td>" +
                        data.GetNotesResult[i].Category + "</td> <td>" +
                        data.GetNotesResult[i].GeneralNotes + "</td> <td>" +
                        "<a href='#'" + " id='aNotes" + data.GetNotesResult[i].DetailRecordId +  "' onclick=\"NotesEdit(" + data.GetNotesResult[i].DetailRecordId + ")\"> Edit  </a >" +
                        " &nbsp;&nbsp;&nbsp;&nbsp;<a href='#'" + " onclick=\"NotesDelete(" + data.GetNotesResult[i].DetailRecordId + ")\"> Delete </a >" + "</td ></tr > ");
                    //a href='#'" + onclick > google </a > " + "</td ></tr > ");

                    //"add" + "</td></tr>");
                }
                //" <div>" + " onclick=\"ShowWOBigPicture(" + data.GetWOPictureResult[i].DetailRecordId + ")\">" +
                //  $("#SeniorInstaller").html(data.GetInstallersResult[0].SeniorInstaller != null && data.GetInstallersResult[0].SeniorInstaller.trim().length > 0 ? data.GetInstallersResult[0].SeniorInstaller : "Unspecified");
                //$("#CrewNames").html(data.GetInstallersResult[0].CrewNames != null && data.GetInstallersResult[0].CrewNames.trim().length > 0 ? data.GetInstallersResult[0].CrewNames : "Un assigned");

            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}



function GetRemeasurerName(workOrder) {
   
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetRemeasurerName?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetNotes:");


            if (data.GetRemeasurerNameResult.length > 0) {

                $("#Remeasurer").html(data.GetRemeasurerNameResult[i]);
            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}

function GetCalledLog(workOrder) {
    $("#dataTableCalledLog tr").remove(); 
    //enable date time
    $("#calledLogDate").prop('disabled', false);
    $("#CalledLogTime").prop('disabled', false);

    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();

    today = mm + '/' + dd + '/' + yyyy;
    $("#calledLogDate").val(today);
    var time = new Date().toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: false });

    $("#CalledLogTime").val(time);
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetCalledLog?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.CalledLog:");

       
            if (data.GetCalledLogResult.length > 0) {
               
                //$("#DateCalled").html(new Date(GetDatefromMoment(data.GetCalledLogResult[0].DateCalled)).toLocaleDateString('en-US'));
                //$("#CalledMessage").html(data.GetCalledLogResult[0].CalledMessage);
                //$("#CalledLogNotes").html(data.GetCalledLogResult[0].Notes3);

                $("#dataTableCalledLog").append("<tr>  <th style = 'text-align:center;' > Date Called</th ><th style='text-align:center;'> Called Message</th > <th style = 'text-align:center;' > Notes</th ><th style = 'text-align:center;' > </th >");
              
                for (var i = 0; i < data.GetCalledLogResult.length; i++) {
                    $("#dataTableCalledLog").append("<tr><td>" +
                        new Date(GetDatefromMoment(data.GetCalledLogResult[i].DateCalled)).toLocaleString('en-US') + "</td> <td>" +
                        data.GetCalledLogResult[i].CalledMessage + "</td> <td>" +
                        data.GetCalledLogResult[i].Notes3 + "</td> <td>" +
                        "<a href='#'" + " id='aCalledLog" + data.GetCalledLogResult[i].DetailRecordId + "' onclick=\"CallLogEdit(" + data.GetCalledLogResult[i].DetailRecordId + ")\"> Edit  </a >" +
                        // "<a href='#'" + " id='a" + data.GetNotesResult[i].DetailRecordId +  "' onclick=\"NotesEdit(" + data.GetNotesResult[i].DetailRecordId + ")\"> Edit  </a >" +
                        " &nbsp;&nbsp;&nbsp;&nbsp;<a href='#'" + " onclick=\"CallLogDelete(" + data.GetCalledLogResult[i].DetailRecordId + ")\"> Delete </a >" +  "</td ></tr > ");
                        //a href='#'" + onclick > google </a > " + "</td ></tr > ");

                       //"add" + "</td></tr>");
                }
                  //" <div>" + " onclick=\"ShowWOBigPicture(" + data.GetWOPictureResult[i].DetailRecordId + ")\">" +
              //  $("#SeniorInstaller").html(data.GetInstallersResult[0].SeniorInstaller != null && data.GetInstallersResult[0].SeniorInstaller.trim().length > 0 ? data.GetInstallersResult[0].SeniorInstaller : "Unspecified");
                //$("#CrewNames").html(data.GetInstallersResult[0].CrewNames != null && data.GetInstallersResult[0].CrewNames.trim().length > 0 ? data.GetInstallersResult[0].CrewNames : "Un assigned");
   
            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}


function GetSubTrades(workOrder) {
    $("#dataTableSubTrades tr").remove();
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetSubTrades?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetSubTrades:");
            var noSubTrades = document.getElementById('noSubTrades');
           
            if (data.GetSubTradesResult.length > 0) {
                noSubTrades.style.display = "none";
                //$("#DateCalled").html(new Date(GetDatefromMoment(data.GetCalledLogResult[0].DateCalled)).toLocaleDateString('en-US'));
                //$("#CalledMessage").html(data.GetCalledLogResult[0].CalledMessage);
                //$("#CalledLogNotes").html(data.GetCalledLogResult[0].Notes3);

                $("#dataTableSubTrades").append("<tr>  <th style='text-align:center;'>SubTrade</th > <th style = 'text-align:center;' > Status</th >");

                for (var i = 0; i < data.GetSubTradesResult.length; i++) {
                    $("#dataTableSubTrades").append("<tr><td>" +

                        data.GetSubTradesResult[i].SubTrade + "</td> <td>" +

                        data.GetSubTradesResult[i].Status + "</td></tr>");
                }

            }
            else {
                noSubTrades.style.display = "block";
            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}

function GetSubTradesCount(workOrder) {
    hasSubStrade = "false";
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetSubTrades?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetSubTrades:");
            var noSubTrades = document.getElementById('noSubTrades');

            if (data.GetSubTradesResult.length > 0) {
                hasSubStrade = "true";

            }
            else {
                hasSubStrade = "false";
            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}


function ShowWOBigPicture(recordid) {
    var link = document.getElementById(recordid);
 
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetWOBigPicture?recordId=' + recordid,
       // dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetWOBigPicture:");
            if (data.GetWOBigPictureResult.length > 0) {

                var image = new Image();
                image.src = data.GetWOBigPictureResult[0].picString ;

                var w = window.open(image.src, '_blank');
                name = "Pictures";
                w.document.write("<html><head><title>" + name + "</title></head><body>"
                  
                    + '</body></html>');
               // var w = window.open("", '_blank');
                w.document.write(image.outerHTML);

            }
           
        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });
}
function GetWOPicture(workOrder) {
    $("#dataTableWOPicture tr").remove(); 
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetWOPicture?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetWOPicture:");
            var noPhoto = document.getElementById('noPhoto');
           
            if (data.GetWOPictureResult.length > 0) {
                noPhoto.style.display = "none";
                $("#dataTableWOPicture").append("<tr>  <th style = 'text-align:center;' > Picture Name</th ><th style='text-align:center;'> Picture</th > ");

                for (var i = 0; i < data.GetWOPictureResult.length; i++) {
                    $("#dataTableWOPicture").append("<tr><td>" +
                      
                        data.GetWOPictureResult[i].PictureName + "</td> <td> " +

                        data.GetWOPictureResult[i].smallpicString +

                        //" <div id='" + data.GetWOPictureResult[i].DetailRecordId + "' class='w3-modal'" + "style='display:none; position: absolute;top:0px;left: 0px;'" +  " onclick=\"this.style.display ='none'\">" +
                        //" <span class='w3-button w3-hover-red w3-xlarge w3-display-topright'></span >" +
                        //" <div class='w3-modal-content w3-animate-zoom' >" +

                        //" <div>" + " onclick=\"ShowWOBigPicture(" + data.GetWOPictureResult[i].DetailRecordId + ")\">" +
                        ////data.GetWOPictureResult[i].picString + "</div></div>" + 
                        //"</div>" + 
                        "</td></tr>");

                }


            }
            else {
                noPhoto.style.display = "block";
            }
        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}

function GetJobReview(workOrder) {
    $("#dataTableJobReview tr").remove();
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetJobReview?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetJobReview:");
            var noJobReview = document.getElementById('noJobReview');

            if (data.GetJobReviewResult.length > 0) {
                noJobReview.style.display = "none";
                $("#dataTableJobReview").append("<tr>  <th style = 'text-align:center;' >Created At</th ><th style='text-align:center;'> Created By</th>" + 
                   " <th style = 'text-align:center;' > Centra Quality</th> <th style='text-align:center;'> Codel Quality</th> <th style='text-align:center;' > Contract Quality</th >" +
                  "  <th style='text-align:center;' > Remeasure Quality</th>"+
                  "  <th style='text-align:center;'> Windows Ready</th> <th style='text-align:center;'> Codel Ready</th>"+
                   " <th style='text-align:center;'> Install Material Missing</th> <th style='text-align:center;'> Notes:</th> <th style='text-align:center;'> What Was Missing</th>");

                for (var i = 0; i < data.GetJobReviewResult.length; i++) {
                    $("#dataTableJobReview").append("<tr><td>" +

                        data.GetJobReviewResult[i].StrCreatedAt + "</td> <td> " +

                        data.GetJobReviewResult[i].CreatedBy + "</td> <td> " +
                        data.GetJobReviewResult[i].CentraQuality +
                        (data.GetJobReviewResult[i].CentraQuality > 3 ? "&nbsp;<img src=\"images/greenface.jpg\" />" : "&nbsp;<img src=\"images/yellowface.jpg\" />") + "</td> <td> " +
                        //''"<img src='images/installerimage.jpg'/></td> <td> " + 
                        //(data.GetJobReviewResult[i].CentraQuality > 3 ? "&nbsp;<img src=\"images/greenface.jpg\" />" : "&nbsp;<img src=\"images/yellowface.jpg\" />") + "</td> <td> " +
                        data.GetJobReviewResult[i].CodelQuality +
                        (data.GetJobReviewResult[i].CodelQuality > 3 ? "&nbsp;<img src=\"images/greenface.jpg\" />" : "&nbsp;<img src=\"images/yellowface.jpg\" />") + "</td> <td> " +
                       // data.GetJobReviewResult[i].CodelQuality > 3 ? "&nbsp;<img src=\"images/greenface.jpg\" />" : "") + "</td> <td> " +
                        data.GetJobReviewResult[i].ContractQuality +
                        (data.GetJobReviewResult[i].ContractQuality > 3 ? "&nbsp;<img src=\"images/greenface.jpg\" />" : "&nbsp;<img src=\"images/yellowface.jpg\" />") + "</td> <td> " +
                        data.GetJobReviewResult[i].RemeasureQuality + 
                        (data.GetJobReviewResult[i].RemeasureQuality > 3 ? "&nbsp;<img src=\"images/greenface.jpg\" />" : "&nbsp;<img src=\"images/yellowface.jpg\" />") + "</td> <td> " +
                        data.GetJobReviewResult[i].WindowProductReady + "</td> <td> " +
                        data.GetJobReviewResult[i].CodelProductReady+ "</td> <td> " +
                        data.GetJobReviewResult[i].InstallMaterialMissing + "</td> <td style='text-align:center;'> " +
                        data.GetJobReviewResult[i].Notes + "</td> <td> " +
                        data.GetJobReviewResult[i].Whatwasmissing  + 
                        "</td></tr>");

                }


            }
            else {
                noJobReview.style.display = "block";
            }
        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}


function GetDocumentLibrary(workOrder) {
    $("#dataTableDocumentLibrary tr").remove();
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetDocumentLibrary?workOrderNumber=' + workOrder,
        dataType: 'json',
        async: false,
        success: function (data) {
            if (debug) console.log("events.success", "data.GetDocumentLibrary:");
            var noDocuments = document.getElementById('noDocuments');

            if (data.GetDocumentLibraryResult.length > 0) {
                noDocuments.style.display = "none";
                $("#dataTableDocumentLibrary").append("<tr>  <th style = 'text-align:center;' >File Names</th ><th style='text-align:center;'> File</th > ");

                for (var i = 0; i < data.GetDocumentLibraryResult.length; i++) {
                    $("#dataTableDocumentLibrary").append("<tr><td>" +

                        data.GetDocumentLibraryResult[i].Notes + "</td> <td> " +
                        "<a href='#'" + " id='aDocumentFile" + data.GetDocumentLibraryResult[i].DetailRecordId + "' onclick=\"ShowDocumentFile(" + data.GetDocumentLibraryResult[i].DetailRecordId + ")\">" + data.GetDocumentLibraryResult[i].FileName + " </a >" +
                        "<a href='#'" + " id='bDocumentFile" + data.GetDocumentLibraryResult[i].DetailRecordId + "' onclick=\"DeleteDocumentFile(" + data.GetDocumentLibraryResult[i].DetailRecordId + ")\">" +" Delete" +  " </a >" +
                        "</td></tr>");

                }


            }
            else {
                noDocuments.style.display = "block";
            }
        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}


function toHexString(byteArray) {
    return Array.prototype.map.call(byteArray, function (byte) {
        return ('0' + (byte & 0xFF).toString(16)).slice(-2);
    }).join('');
}

function hexToBase64(hexstring) {
    return btoa(hexstring.match(/\w{2}/g).map(function (a) {
        return String.fromCharCode(parseInt(a, 16));
    }).join(""));
}

function GetReturnedJobDates(workOrder) {
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetInstallationDateByWOForReturnedJob?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetInstallationDateByWOForReturnedJob:");
            eventReturnedJobDate1 = data.GetInstallationDateByWOForReturnedJobResult[0];
           // eventReturnedJobDate1 = data.GetInstallationDateByWOForReturnedJobResult[0];

           

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}

function GetNonReturnedJobDates(workOrder) {
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetInstallationDateByWOForNonReturnedJob?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetInstallationDateByWOForNonReturnedJob:");
            eventReturnedJobDate1 = data.GetInstallationDateByWOForNonReturnedJobResult[0];
            // eventReturnedJobDate1 = data.GetInstallationDateByWOForReturnedJobResult[0];



        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}



function findDuplicatedObjects(originalObjectArray, newObject) {
    var isDuplicated = false;
    for (i = 0; i < originalObjectArray.length; i++) {
        if (originalObjectArray[i] == newObject) {
            isDuplicated = true;
        }
    }
    return isDuplicated;

}

function removeDuplicates(originalArray, prop) {
    var newArray = [];
    var lookupObject = {};
    var returnedObjectArray = {};
    var modifiedObjectArray = {};
    var addedObjectArray = [];

    if (displayType == "Installation") {
        modifiedObjectArray = originalArray.filter(el => el.ReturnedJob != 1);

        for (var i in modifiedObjectArray) {
            lookupObject[modifiedObjectArray[i][prop]] = modifiedObjectArray[i];
        }

        returnedObjectArray = originalArray.filter(el => el.ReturnedJob == 1);
        for (i in returnedObjectArray) {

            if (findDuplicatedObjects(addedObjectArray, returnedObjectArray[i]["WorkOrderNumber"]) == false) {
                addedObjectArray[i] = returnedObjectArray[i]["WorkOrderNumber"];
                newArray.push(returnedObjectArray[i]);
            }


        }
    }
    else {
        for (var i in originalArray) {
            lookupObject[originalArray[i][prop]] = originalArray[i];
        }

    }

    for (i in lookupObject) {
        newArray.push(lookupObject[i]);
    }
    return newArray;
}