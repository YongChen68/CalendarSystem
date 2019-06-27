var displayType = "Windows";
var GlobalParams = [];
var eventArray = [];
var debug = true;
var renderingComplete = false;
var eventid;
var eventWO = []; 
var eventReturnedJobDate1;

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
        CurrentStateName: event.CurrentStateName

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

        PageMethods.UpdateEventTime(displayType, eventToUpdate);
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
            SidingLBRBudget: 0, SidingLBRMin: 0, SidingSQF: 0
        };
    }
    else if (displayType == "Remeasure")
    {
        var remeasureDay = new Date(Date.UTC(day.getUTCFullYear(), day.getUTCMonth(), day.getUTCDate() + 1));
        //  return { day: dayName, date: new Date(day.valueOf()), doors: 0, windows: 0, SalesAmmount: 0, WOCount: 0 };
        return {
            day: dayName, date: remeasureDay, doors: 0, windows: 0, ExtDoors: 0,
            SalesAmmount: 0, TotalAsbestos: 0, TotalWoodDropOff: 0, TotalHighRisk: 0, TotalLeadPaint: 0, WOCount: 0, installationwindowLBRMIN: 0, TotalInstallationLBRMin: 0, InstallationDoorLBRMin: 0,
            InstallationPatioDoorLBRMin: 0, subinstallationwindowLBRMIN: 0, subInstallationPatioDoorLBRMin: 0, subExtDoorLBRMIN: 0, subTotalInstallationLBRMin: 0,
            SidingLBRBudget: 0, SidingLBRMin: 0, SidingSQF: 0
        };
    }
    else {
      //  return { day: dayName, date: new Date(day.valueOf()), doors: 0, windows: 0, boxes: 0, glass: 0, value: 0, min: 0, max: 0, Available_Time: 0, rush: 0, float: 0, TotalBoxQty: 0, TotalGlassQty: 0, TotalPrice: 0, TotalLBRMin: 0, F6CA: 0, F27DS: 0, F27TS: 0, F27TT: 0, F29CA: 0, F29CM: 0, F52PD: 0, F68CA: 0, F68SL: 0, F68VS: 0, Transom: 0, Sidelite: 0, SingleDoor: 0, DoubleDoor: 0, Simple: 0, Complex: 0, Over_Size: 0, Arches: 0, Rakes: 0, Customs: 0, };
        return { day: dayName, date: new Date(day.valueOf()), doors: 0, windows: 0, boxes: 0, glass: 0, value: 0, min: 0, max: 0, Available_Time: 0, rush: 0, float: 0, TotalBoxQty: 0, TotalGlassQty: 0, TotalPrice: 0, TotalLBRMin: 0, F6CA: 0, F27DS: 0, F27TS: 0, F27TT: 0, F29CA: 0, F29CM: 0, F52PD: 0, F68CA: 0, F68SL: 0, F68VS: 0, Transom: 0, Sidelite: 0, SingleDoor: 0, DoubleDoor: 0 };
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
        '27TS: ' + event.F27TS + '\r\n' +
        '27TT: ' + event.F27TT + '\r\n' +

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
    //$.getJSON("data.svc/GetInstallationBufferJobs", { branch: branches.join(",")}, function (data) {
    //    //$.each(data, function (key, val) {
    //    //    AddInstallationBufferEvent(key, val);
    //    //});
    //    var events = [];
    //    $.each(data, function (pos, item) {
    //        item.editable = (item.HolidayName != null) ? false : true;
    //        events.push(item);
    //    });
    //    /* initialize the external events
    //    -----------------------------------------------------------------*/
    //});
    $.ajax({
        url: 'data.svc/GetInstallationBufferJobs',
        dataType: 'json',
        data: {  branch: branches.join(",") },
        success: function (data) {
            if (debug) console.log("events.success", "data.GetInstallationBufferJobsResult:", data.GetInstallationBufferJobsResult === undefined ? "NULL" : data.GetInstallationBufferJobsResult.length);
            var events = [];
         

            var found = false;
            var con;
            $.each(data.GetInstallationBufferJobsResult, function (pos, item) {
                AddInstallationBufferEvent(pos, item);
            });
           
        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });


   
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

    var el = $("<div class='fc-event " + (val.JobType == "RES" ? " reservation" : "") +
        "' id=\"" + val.id + "\" style=\"background-color:" + val.color + "\">" + ret + "</div>").appendTo('#external-events1');
    el.draggable({
        zIndex: 996,
        revert: true,
        revertDuration: 0  //  original position after the drag
    });
    $('#' + val.id).data('event', {
        // title: val.title, id: val.id, description: val.description, doors: val.doors, windows: val.windows, type: val.type, JobType: val.JobType, boxes: val.boxes, glass: val.glass, value: val.value, min: val.min, max: val.max, rush: val.rush, float: val.float, TotalBoxQty: val.TotalBoxQty, TotalGlassQty: val.TotalGlassQty, TotalPrice: val.TotalPrice, TotalLBRMin: val.TotalLBRMin, F6CA: val.F6CA, F27DS: val.F27DS, F27TS: val.F27TS, F27TT: val.F27TT, F29CA: val.F29CA, F29CM: val.F29CM, F52PD: val.F52PD, F68CA: val.F68CA, F68SL: val.F68SL, F68VS: val.F68VS, DoubleDoor: val.DoubleDoor, Transom: val.Transom, Sidelite: val.Sidelite, SingleDoor: val.SingleDoor
      //  title: val.title, id: val.id, doors: val.Doors, windows: val.Windows, WorkOrderNumber: title.WorkOrderNumber, Branch: val.Branch, City: val.City, CellPhone: val.CellPhone, CrewNames: val.CrewNames, CurrentStateName: val.CurrentStateName, LastName: val.LastName, FirstName: val.FirstName
        title: val.title, id: val.id, doors: val.Doors, City: val.City, windows: val.Windows, WorkOrderNumber: val.WorkOrderNumber, LastName: val.LastName, FirstName: val.FirstName,
        EstInstallerCnt: val.EstInstallerCnt, WindowState: val.WindowState, DoorState: val.DoorState, TotalWoodDropOff: val.TotalWoodDropOff,
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
        revertDuration: 0  //  original position after the drag
    });

    el.editable = (readonly == "True") ? false : true;
    el.draggable = (readonly == "True") ? false : true;

    $('#' + val.id).data('event', {
        // title: val.title, id: val.id, description: val.description, doors: val.doors, windows: val.windows, type: val.type, JobType: val.JobType, boxes: val.boxes, glass: val.glass, value: val.value, min: val.min, max: val.max, rush: val.rush, float: val.float, TotalBoxQty: val.TotalBoxQty, TotalGlassQty: val.TotalGlassQty, TotalPrice: val.TotalPrice, TotalLBRMin: val.TotalLBRMin, F6CA: val.F6CA, F27DS: val.F27DS, F27TS: val.F27TS, F27TT: val.F27TT, F29CA: val.F29CA, F29CM: val.F29CM, F52PD: val.F52PD, F68CA: val.F68CA, F68SL: val.F68SL, F68VS: val.F68VS, DoubleDoor: val.DoubleDoor, Transom: val.Transom, Sidelite: val.Sidelite, SingleDoor: val.SingleDoor
        title: val.title, id: val.id, description: val.description, doors: val.Doors, windows: val.Windows, type: val.type, JobType: val.JobType, boxes: val.boxes, glass: val.glass, value: val.value, min: val.min, max: val.max, rush: val.rush, float: val.float, TotalBoxQty: val.TotalBoxQty, TotalGlassQty: val.TotalGlassQty, TotalPrice: val.TotalPrice, TotalLBRMin: val.TotalLBRMin, F6CA: val.F6CA, F27DS: val.F27DS, F27TS: val.F27TS, F27TT: val.F27TT, F29CA: val.F29CA, F29CM: val.F29CM, F52PD: val.F52PD, F68CA: val.F68CA, F68SL: val.F68SL, F68VS: val.F68VS, DoubleDoor: val.DoubleDoor, Transom: val.Transom, Sidelite: val.Sidelite, SingleDoor: val.SingleDoor
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
    LoadInstallationBufferedJobs();
    $('#external-events1').hide();

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
        header: {
            left: 'prev,next today, changeType,applyFilters,applyInstallationFilters,applyRemeasureFilters,changeBranch,changeJobType,changeShippingType',
            center: 'title',
            right: 'ShowKelownaBranch,ShowLowerMainlandBranch,ShowNanaimoBranch,ShowVictoriaBranch,ShowWIP,month,agendaWeek,agendaDay'
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

            if (displayType == "Installation") {
                $('.fc-applyInstallationFilters-button').show();
                
                $('.fc-ShowKelownaBranch-button').show();
                $('.fc-ShowLowerMainlandBranch-button').show();
                $('.fc-ShowNanaimoBranch-button').show();
                $('.fc-ShowVictoriaBranch-button').show();

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
                    data: { start: start.format(), end: end.format(), branch: branches.join(","), installationStates: installationStates.join(",") },
                    success: function (data) {
                        if (debug) console.log("events.success", "data.GetInstallationEventsResult:", data.GetInstallationEventsResult === undefined ? "NULL" : data.GetInstallationEventsResult.length);
                        var events = [];
                        eventWODict = [];
                        var con;
                        $.each(data.GetInstallationEventsResult, function (pos, item) {
                            item.allDay = true;
                         //   item.editable = (item.ReturnedJob == 1) ? false : true;

                           
                            item.editable =( (item.HolidayName != null || readonly == "True") )? false : true;
                          //  item.editable = (readonly == "True") ? false : true;

                            eventWODict.push(item);

                        });
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

                $('.fc-applyInstallationFilters-button').hide();
                $('.fc-ShowWIP-button').hide();

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
                $('.fc-applyRemeasureFilters-button').hide();

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
                        $.each(data.GetEventsResult, function (pos, item) {
                            item.allDay = true;
                            item.editable = (item.HolidayName != null) ? false : true;
                            item.editable = (readonly == "True") ? false : true;
                            events.push(item);
                        });
                        callback(events);
                    }, error: function (error) {
                        console.log('Error', error);
                        $('#script-warning').show();
                    }
                });
            }


        },
        editable: readonly == "True" ? false : true,
        nowIndicator: true,
        eventDurationEditable: readonly == "True" ? false : true,
        droppable: readonly == "True" ? false : true, // this allows things to be dropped onto the calendar
        weekNumbers: true,
        businessHours: { start: '8:00', end: '17:00', dow: [1, 2, 3, 4, 5, 6] },
        eventDrop: function (event, delta, revertFunc) {
            if ((displayType != "Installation") && (displayType != "Remeasure")){
                if (debug) console.log('eventDrop', 'event.title=' + event.title, "id: " + event.id, "labour min: " + event.TotalLBRMin, "event date:", GetDatefromMoment(event.start));
                var eventDate = GetDatefromMoment(event.start);
                var maxDayLabour = parseInt(FindByValue("max", eventDate).Value);
                var allocatedDayLabour = GetDayLabourValue($('#calendar').fullCalendar('getView'), eventDate);
                console.log("eventDrop", "allocated min:", allocatedDayLabour, "event min:", event.TotalLBRMin, "max min:", maxDayLabour);
                if (allocatedDayLabour + event.TotalLBRMin <= maxDayLabour) {
                    eventUpdate(event);
                } else {
                    ShowWarning(allocatedDayLabour, event.TotalLBRMin, maxDayLabour);
                    revertFunc();
                }
            }
            {
                eventUpdate(event);
            }
            $('#calendar').fullCalendar('refetchEvents');

        },//: eventUpdate,
        eventResize: eventUpdate,
        eventRender: function (event, element, view) {
          

            if (renderingComplete) {
                renderingComplete = false;
                totals = getBlankTotal();
                eventArray = [];
            }
            if (event.ReturnedJob == 1) {
                element.css({
                    'background-color': '#ff6347'
                    // 'border-color': '#333333'
                });
            }
            element.attr('href', 'javascript:void(0);');
            element.click(function () {
                if (displayType == "Installation") {
                    element.attr('data-toggle', "modal");
                    element.attr('data-target', "#eventContent");
                    element.attr('href', "/details");
                    $("#FirstName").html(event.FirstName);
                    $("#LastName").html(event.LastName);
                    $("#postalCode").html(event.PostCode);
                    $("#workOrder").html(event.WorkOrderNumber);
                    $("#WorkOrderTitle").html(event.WorkOrderNumber);
                    
                                  
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

                    if (event.TotalWoodDropOff == 1) {
                        // 
                        $("#Wood-DropOff-JobsYes").prop("checked", true);
                        $("#Wood-DropOff-JobsNo").prop("checked", false);
                    }
                    else {
                        $("#Wood-DropOff-JobsNo").prop("checked", true);
                        $("#Wood-DropOff-JobsYes").prop("checked", false);
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
                    $("#SalesAmmount").html(event.TotalSalesAmount.formatMoney(2, "$", ",", "."));
                    $("#SeniorInstaller").html(event.SeniorInstaller != null && event.SeniorInstaller.trim().length > 0 ? event.SeniorInstaller : "Unspecified");
                    $("#CrewNames").html(event.CrewNames != null && event.CrewNames.trim().length > 0 ? event.CrewNames : "Un assigned");

                    codeAddress();
                       //$("#ReturnedJob").hide();
                    $("#from_date").val('');
                    $("#end_date").val('');
              

                    if (event.ReturnedJob == 1) {
                        // $("#ReturnedJob").show(); 
                        $("#from_date").val(new Date(GetDatefromMoment(event.start + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                        $("#end_date").val(new Date(GetDatefromMoment(event.end + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                        if (event.end == null) {
                            $("#end_date").val(new Date(GetDatefromMoment(event.start + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                        }

                     
                            $("#InstallScheduledStartDate").prop("disabled", true);
                            $("#InstallScheduledEndDate").prop("disabled", true);
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
                       //etInstallationDateByWOForReturnedJob()

                       // $("#InstallScheduledStartDate").val(new Date(GetDatefromMoment(event.start + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                      //  $("#InstallScheduledEndDate").val(new Date(GetDatefromMoment(event.end + 24 * 60 * 60000)).toLocaleDateString('en-US'));

                    }
                    else {
               
                        $("#InstallScheduledStartDate").val(new Date(GetDatefromMoment(event.start + 24 * 60 * 60000)).toLocaleDateString('en-US'));

                    if (event.end == null) {
                        $("#InstallScheduledEndDate").val(new Date(GetDatefromMoment(event.start + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                    }
                    else {
                        $("#InstallScheduledEndDate").val(new Date(GetDatefromMoment(event.end + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                    }
                        $("#InstallScheduledStartDate").prop("disabled", false);
                        $("#InstallScheduledEndDate").prop("disabled", false);

                        if (event.StartScheduleDate != null) {
                            $("#from_date").prop("disabled", true);
                            $("#end_date").prop("disabled", true);
                            $("#from_date").val(new Date(GetDatefromMoment(event.StartScheduleDate + 24 * 60 * 60000)).toLocaleDateString('en-US'));
                            $("#end_date").val(new Date(GetDatefromMoment(event.EndScheduleDate + 24 * 60 * 60000)).toLocaleDateString('en-US'));

                        }
                        else {
                            $("#from_date").prop("disabled", false);
                            $("#end_date").prop("disabled", false);
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
                    GetProducts(event.WorkOrderNumber);
                    GetInstallers(event.WorkOrderNumber);
                    GetCalledLog(event.WorkOrderNumber);
                    GetWOPicture(event.WorkOrderNumber);
                   // GetJobAnalysys(event.WorkOrderNumber);
                    $("#TotalLBRMin").html(event.TotalInstallationLBRMin);

                    $("#eventLink").attr('href', event.url);
                   // $("#eventContent").dialog({ modal: true, title: event.LastName, width: 900 });
                    //$("#eventContent").modal("show");

                }
                else {
                  //  $("#eventContent").dialog({ modal: true, title: event.title, width: 800 });
                  
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
                if (displayType == "Installation") {
                    ret += event.HolidayName;
                }
                element.css({
                    'background-color': '#c6abd0'
                    // 'border-color': '#333333'
                });
                $(element).find(dom).prepend(ret);
                
            }



            if (view.name == "agendaDay") {
                if (displayType != "Installation") {
                    $(element).find(dom).text(element.text() + " - LBR Min: " + (event.TotalLBRMin !== undefined ? event.TotalLBRMin : 0) + ", Bundle: " + event.BatchNo);
                }
            }
            if (event.PaintIcon != undefined && event.PaintIcon != '' && event.PaintIcon == "Yes") {// Paint Icon
                $(element).find(dom).prepend("<img alt=\"#\" src=\"images/color.png\" />&nbsp;");
            }
            if (event.windows > 0) {// Windows

                $(element).find(dom).prepend("<img alt=\"#\" src=\"images/window.png\" title= \"" + ToWDString(event) + "\" />&nbsp;");
            }

            if ((displayType == "Installation") && (event.HolidayName == null)){

                dom = '.fc-title';
                $(element).find(dom).empty();
                

                var ret ="<img src=\"images/installer" + event.EstInstallerCnt + ".png\" title=\"Estimated number of installers for the job: " +
                    event.EstInstallerCnt + "\">" +
                    (event.TotalWindows != "0" ? "&nbsp;<img title=\"# of Windows: " + event.TotalWindows  + "\" src=\"images/window.PNG\" />" : "") +
                    (event.TotalDoors != "0" ? "&nbsp;<img title=\"# of Patio Doors: " + event.TotalDoors + "\" src=\"images/window.PNG\" />" : "") + "&nbsp;" +
                    (event.TotalExtDoors != "0" ? "&nbsp;<img title=\"# of Codel Doors: " + event.TotalExtDoors + "\" src=\"images/door.PNG\" />" : "") + "&nbsp;" +
                    (event.TotalWoodDropOff == 1 ? "&nbsp;<img src=\"images/delivery.PNG\" />" : "") +
                    //(event.TotalAsbestos == 1 ? "&nbsp;<img src=\"images/asbestos.PNG\" />" : "") +
                    ((event.TotalAsbestos == 1) || (event.LeadPaint=='Yes') ? "&nbsp;<img src=\"images/asbestos.PNG\" />" : "") +
                    (event.TotalHighRisk == 1 ? "&nbsp;<img src=\"images/risk.PNG\" />" : "") +
                    (event.ReturnedJob == 1 ? "&nbsp;<img src=\"images/fire.PNG\" />" : "") +
                    (" " + event.WorkOrderNumber) + "&nbsp;" +
                    (",Name: " + event.LastName.trim().length > 10 ? event.LastName.trim().Substring(0, 10) : event.LastName.trim())  +
                     //   + event.FirstName.trim().length > 10 ? event.FirstName.trim().Substring(0, 10) : event.FirstName.trim()) +
                "&nbsp;" + (event.City) + "&nbsp;";
                  //  ("WO: ") + "&nbsp;" ;

                $(element).find(dom).append(ret);
              
            }


            if ((displayType == "Remeasure") && (event.HolidayName == null)) {

                dom = '.fc-title';
                $(element).find(dom).empty();


                var ret = "<img src=\"images/installer" + event.EstInstallerCnt + ".png\" title=\"Estimated number of installers for the job: " +
                    event.EstInstallerCnt + "\">" +
                    (event.TotalWindows != "0" ? "&nbsp;<img title=\"# of Windows: " + event.TotalWindows + "\" src=\"images/window.PNG\" />" : "") +
                    (event.TotalDoors != "0" ? "&nbsp;<img title=\"# of Patio Doors: " + event.TotalDoors + "\" src=\"images/window.PNG\" />" : "") + "&nbsp;" +
                    (event.TotalExtDoors != "0" ? "&nbsp;<img title=\"# of Codel Doors: " + event.TotalExtDoors + "\" src=\"images/door.PNG\" />" : "") + "&nbsp;" +
                    (event.TotalWoodDropOff == 1 ? "&nbsp;<img src=\"images/delivery.PNG\" />" : "") +
                    //(event.TotalAsbestos == 1 ? "&nbsp;<img src=\"images/asbestos.PNG\" />" : "") +
                    ((event.TotalAsbestos == 1) || (event.LeadPaint == 'Yes') ? "&nbsp;<img src=\"images/asbestos.PNG\" />" : "") +
                    (event.TotalHighRisk == 1 ? "&nbsp;<img src=\"images/risk.PNG\" />" : "") +

                    (" " + event.WorkOrderNumber) + "&nbsp;" +
                    (",Name: " + event.LastName.trim().length > 10 ? event.LastName.trim().Substring(0, 10) : event.LastName.trim()) +
                    //   + event.FirstName.trim().length > 10 ? event.FirstName.trim().Substring(0, 10) : event.FirstName.trim()) +
                    "&nbsp;" + (event.City) + "&nbsp;";
                //  ("WO: ") + "&nbsp;" ;

                $(element).find(dom).append(ret);

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
            if (displayType == "Installation") {
              dayId = FindDayIdfromTotals(GetDatefromMoment(event.ScheduledDate));
            }
            else
            {
                dayId = FindDayIdfromTotals(GetDatefromMoment(event.start));
            }
                
            if (totals.length < dayId) console.log("eventRender", "dayId exceeds totals", dayId, totals.length);

            
            if ((displayType == "Installation") && (event.HolidayName != null )){
               
            }
            else {
                totals[dayId].doors += event.doors !== undefined ? event.doors : 0;
                totals[dayId].windows += event.windows !== undefined ? event.windows : 0;
            }

            totals[dayId].boxes += event.TotalBoxQty !== undefined ? event.TotalBoxQty : 0;
            totals[dayId].glass += event.TotalGlassQty !== undefined ? event.TotalGlassQty : 0;
            totals[dayId].value += event.TotalPrice !== undefined ? event.TotalPrice : 0;
            totals[dayId].rush += event.FlagOrder !== undefined ? event.FlagOrder : 0;
            totals[dayId].min += event.TotalLBRMin !== undefined ? event.TotalLBRMin : 0;
            // Windows
            totals[dayId].F6CA += event.F6CA !== undefined ? event.F6CA : 0;
            totals[dayId].F68VS += event.F68VS !== undefined ? event.F68VS : 0;
            totals[dayId].F68SL += event.F68SL !== undefined ? event.F68SL : 0;
            totals[dayId].F68CA += event.F68CA !== undefined ? event.F68CA : 0;
            totals[dayId].F52PD += event.F52PD !== undefined ? event.F52PD : 0;
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

                console.log("eventReceive", "allocated min:", allocatedDayLabour, "max labour:", maxDayLabour);
                if (allocatedDayLabour + event.TotalLBRMin <= maxDayLabour) {
                    sendUpdateToServer(event);
                } else {
                    ShowWarning(allocatedDayLabour, event.TotalLBRMin, maxDayLabour);
                    $('#calendar').fullCalendar('removeEvents', event._id);
                    AddBufferEvent(0, event);
                }
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
            if (view.name !== "agendaMonth" && view.name !== "month") {
                if (debug) console.log("eventAfterAllRender", "view configuration:", view.title, view.intervalStart._d);
                var startDate = view.start._d;
                var endDate = view.end._d;
              
                var doors;
                var windows;
              
                var results;
               
                var date1, date2;
                var WOCount;
                var xDate, xDateSat, xDateSun;

                if (displayType == "Installation" ) {
                    for (var i = 0; i < totals.length; i++) {
                        date1 = new Date(totals[i]["date"]).toLocaleDateString('en-US');
                        WOCount = 0; //ReturnedJob
                        for (var j = 0; j < eventWODict.length; j++) {
                            date2 = new Date(GetDatefromMoment(eventWODict[j]["ScheduledDate"])).toLocaleDateString('en-US');
                            if ((date1 == date2) && (eventWODict[j].ReturnedJob != 1)) {
                               // if eventWODict[j].Saturday == "No"
                                xDate = is_weekend(date2);
                                xDateSat = is_saturday(date2);
                                xDateSun = is_sunday(date2);
                                if ((eventWODict[j].Sunday == "No" && eventWODict[j].Saturday == "No") && (xDate == "weekend")) {

                                }
                                else if ((eventWODict[j].Sunday == "Yes" && eventWODict[j].Saturday == "No") && (xDateSat == "saturday")) {
                                   
                                }
                                else if ((eventWODict[j].Sunday == "No" && eventWODict[j].Saturday == "Yes") && (xDateSun == "sunday")) {
                                  
                                }
                                else {
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

                                    if (eventWODict[j]["LeadPaint"] == "Yes") {
                                        totals[i].TotalLeadPaint++;
                                    }

                                    totals[i].WOCount = WOCount + 1;
                                    WOCount++;
                                }

                            }

                        }
                    }
                }
               

                for (var i = 0; i < totals.length; i++) {
                    SetDayValue(i, totals[i]);
                }

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
        //    else {
        //        return offsets[day];
        //    }
        //},
        viewRender: function (view, element) {
            if (debug) console.log("viewRender", "view configuration:", view.title, view.intervalStart._d);
            totals = getBlankTotal();
            ControlHeaderVisibility(GetDisplayItemList(displayType));
            if (view.type == 'agendaWeek') {
                $('#calendar').fullCalendar('refetchEvents');
            }
            
        }
    });

  
});
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

        SetData('Window-LBR', dayTotals.day, parseFloat(dayTotals.subinstallationwindowLBRMIN).toFixed(2)); 
        SetData('Patio-Door-LBR', dayTotals.day, parseFloat(dayTotals.subInstallationPatioDoorLBRMin).toFixed(2));
        SetData('Codel-Door-LBR', dayTotals.day, parseFloat(dayTotals.subExtDoorLBRMIN).toFixed(2));
        SetData('Total-LBR', dayTotals.day, parseFloat(dayTotals.subTotalInstallationLBRMin).toFixed(2));

      // SetData('Siding-LBRBudget', dayTotals.day, parseFloat(dayTotals.SidingLBRBudget).toFixed(2));
        SetData('Siding-LBRBudget', dayTotals.day, dayTotals.SidingLBRBudget.formatMoney(2, "$", ",", "."));
        SetData('Siding-LBRMin', dayTotals.day, parseFloat(dayTotals.SidingLBRMin).toFixed(2));
        SetData('Siding-SQF', dayTotals.day, parseFloat(dayTotals.SidingSQF).toFixed(2));
    }
    else {
        var maxTime = parseInt(FindByValue("max", dayTotals.date).Value);
        var maxStaff = parseInt(FindByValue("manpower", dayTotals.date).Value);
        SetData('Doors', dayTotals.day, dayTotals.doors);
        SetData('Rush', dayTotals.day, dayTotals.rush);
        SetData('Windows', dayTotals.day, dayTotals.windows);
        SetData('Boxes', dayTotals.day, dayTotals.boxes);
        SetData('Glass', dayTotals.day, dayTotals.glass);
        SetData('Value', dayTotals.day, dayTotals.value.formatMoney(2, "$", ",", "."));
        SetData('Min', dayTotals.day, dayTotals.min.formatMoney(0, "", ",", "."));
        SetData('Max', dayTotals.day, maxTime.formatMoney(0, "", ",", "."));
        SetData('Available_Time', dayTotals.day, (maxTime - dayTotals.min).formatMoney(0, "", ",", "."));
        SetData('Available_Staff', dayTotals.day, maxStaff);
        SetData('Float', dayTotals.day, dayTotals.float.formatMoney(0, "", ",", "."));
        SetData('27DS', dayTotals.day, dayTotals.F27DS);
        SetData('27TS', dayTotals.day, dayTotals.F27TS);
        SetData('27TT', dayTotals.day, dayTotals.F27TT);
        SetData('29CA', dayTotals.day, dayTotals.F29CA);
        SetData('29CM', dayTotals.day, dayTotals.F29CM);
        SetData('52PD', dayTotals.day, dayTotals.F52PD);
        SetData('68CA', dayTotals.day, dayTotals.F68CA);
        SetData('68VS', dayTotals.day, dayTotals.F68VS);
        SetData('68SL', dayTotals.day, dayTotals.F68SL);
        SetData('26CA', dayTotals.day, dayTotals.F6CA);
        SetData('Transom', dayTotals.day, dayTotals.Transom);
        SetData('Sidelite', dayTotals.day, dayTotals.Sidelite);
        SetData('SingleDoor', dayTotals.day, dayTotals.SingleDoor);
        SetData('DoubleDoor', dayTotals.day, dayTotals.DoubleDoor);
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
    ControlHeaderVisibility(GetDisplayItemList(displayType));

    if (type == "Installation") {
        LoadInstallationBufferedJobs();
        $('#external-events1').show();
        $('#external-events').hide();
    }
    else {
        LoadBufferedJobs();
        $('#external-events').show();
        $('#external-events1').hide();
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
    window.alert("There are already " + allocatedDayLabour + " minutes on the target day. Adding requested event with Labour minutes of " + eventLabourMin + " will exceed maximum available minutes (" + maxDayLabour + ") for the day.");
}

function UpdateReturnedJobSchedule() {
    var i = eventid;
    var scheduledStartDate = $("#from_date").val();
    var scheduledEndDate = $("#end_date").val();
    $.ajax({
        url: 'data.svc/UpdateReturnedJobSchedule?id=' + eventid + '&ScheduledStartDate=' + scheduledStartDate + '&scheduledEndDate=' + scheduledEndDate,
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


    var scheduledReturnedJobStartDate = $("#from_date").val();
    if (scheduledReturnedJobStartDate.length != 0) {
        UpdateReturnedJobSchedule();
    }


    var i = eventid;
    var scheduledStartDate, scheduledEndDate;
    scheduledStartDate = $("#InstallScheduledStartDate").val();
    scheduledEndDate = $("#InstallScheduledEndDate").val();

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
            + '&Asbestos=' + Asbestos + '&WoodDropOff=' + WoodDropOff
            + '&HighRisk=' + HighRisk + '&EstInstallerCnt=' + NumOfInstallers
            + '&Saturday=' + isSaturdayChecked + '&Sunday=' + isSundayChecked + '&LeadPaint=' + LeadPaint,
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
    var remesureDate;
    remesureDate = event.start;
    
    $.ajax({
        url: 'data.svc/UpdateRemeasureData?id=' + id
            + '&remeasureDate=' + remesureDate,
        type: "POST",
        success: function (data) {
            if (debug) console.log("events.success", "data.UpdateRemeasureEvents:");

            $('#calendar').fullCalendar('refetchEvents');
            $('#calendar').fullCalendar('rerenderEvents');

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });
}



function GetJobAnalysys(workOrder) {
    TotalLBRMin
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


function GetProducts(workOrder) {
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetProducts?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetProducts:");

            $("#dataTable tr").remove(); 
            if (data.GetProductsResult.length > 0) {
                $("#dataTable").append("<tr>  <th style = 'text-align:center;' > Item</th ><th style='text-align:center;'> Size</th ><th style='text-align:center;'>Quantity</th> <th style = 'text-align:center;' > SubQty</th ><th style='text-align:center;' > System</th ><th style='text-align:center;'>Description</th><th style='text-align:center;' > Status</th >  </tr > ");
                for (var i = 0; i < data.GetProductsResult.length; i++) {
                    $("#dataTable").append("<tr><td>" +
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


function GetInstallers(workOrder) {
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetInstallers?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.GetProducts:");

      
            if (data.GetInstallersResult.length > 0) {

                $("#SeniorInstaller").html(data.GetInstallersResult[0].SeniorInstaller != null && data.GetInstallersResult[0].SeniorInstaller.trim().length > 0 ? data.GetInstallersResult[0].SeniorInstaller : "Unspecified");
                $("#CrewNames").html(data.GetInstallersResult[0].CrewNames != null && data.GetInstallersResult[0].CrewNames.trim().length > 0 ? data.GetInstallersResult[0].CrewNames : "Un assigned");
               
            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}

function GetCalledLog(workOrder) {
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetCalledLog?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.CalledLog:");

            $("#dataTableCalledLog tr").remove(); 
            if (data.GetCalledLogResult.length > 0) {
               
                //$("#DateCalled").html(new Date(GetDatefromMoment(data.GetCalledLogResult[0].DateCalled)).toLocaleDateString('en-US'));
                //$("#CalledMessage").html(data.GetCalledLogResult[0].CalledMessage);
                //$("#CalledLogNotes").html(data.GetCalledLogResult[0].Notes3);

                $("#dataTableCalledLog").append("<tr>  <th style = 'text-align:center;' > Date Called</th ><th style='text-align:center;'> Called Message</th > <th style = 'text-align:center;' > Notes</th >");
              
                for (var i = 0; i < data.GetCalledLogResult.length; i++) {
                    $("#dataTableCalledLog").append("<tr><td>" +
                        new Date(GetDatefromMoment(data.GetCalledLogResult[i].DateCalled)).toLocaleDateString('en-US') + "</td> <td>" +
                        data.GetCalledLogResult[i].CalledMessage + "</td> <td>" +
                     
                        data.GetCalledLogResult[i].Notes3 + "</td></tr>");
                }

              //  $("#SeniorInstaller").html(data.GetInstallersResult[0].SeniorInstaller != null && data.GetInstallersResult[0].SeniorInstaller.trim().length > 0 ? data.GetInstallersResult[0].SeniorInstaller : "Unspecified");
                //$("#CrewNames").html(data.GetInstallersResult[0].CrewNames != null && data.GetInstallersResult[0].CrewNames.trim().length > 0 ? data.GetInstallersResult[0].CrewNames : "Un assigned");

            }

        }, error: function (error) {
            console.log('Error', error);
            $('#script-warning').show();
        }
    });

}

function GetWOPicture(workOrder) {
    $.ajax({
        //type: "POST",  
        url: 'data.svc/GetWOPicture?workOrderNumber=' + workOrder,
        dataType: 'json',
        success: function (data) {
            if (debug) console.log("events.success", "data.CalledLog:");

            $("#dataTableWOPicture tr").remove(); 
            if (data.GetWOPictureResult.length > 0) {
               $("#dataTableWOPicture").append("<tr>  <th style = 'text-align:center;' > Picture Name</th ><th style='text-align:center;'> Picture</th > ");

                for (var i = 0; i < data.GetWOPictureResult.length; i++) {
                    $("#dataTableWOPicture").append("<tr><td>" +
                        data.GetWOPictureResult[i].PictureName + "</td> <td> <image " +

                     //   $('#item').attr('src', `data:image/jpg;base64,' + hexToBase64{data.GetWOPictureResult[0].pic)  + "</image></td></tr>");
                    //  " document.getElementById('item').src = '" + data.GetWOPictureResult[0].picString + "'</image></td></tr>");
                       data.GetWOPictureResult[i].picString + "'</image></td></tr>");

                 //   $("#dataTableWOPicture").append("<tr><td><image id='item'" +
                //       $('#item').attr('src', `data:image/jpg;base64,' + hexToBase64{data.GetWOPictureResult[0].pic)  + "</image></td></tr>");
                       //(data.GetCalledLogResult[i].DateCalled)).toLocaleDateString('en-US') + "</td> <td>" +
                       // data.GetCalledLogResult[i].CalledMessage + "</td> <td>" +

                       // data.GetCalledLogResult[i].Notes3 + "</td></tr>");
                }

                //  $("#SeniorInstaller").html(data.GetInstallersResult[0].SeniorInstaller != null && data.GetInstallersResult[0].SeniorInstaller.trim().length > 0 ? data.GetInstallersResult[0].SeniorInstaller : "Unspecified");
                //$("#CrewNames").html(data.GetInstallersResult[0].CrewNames != null && data.GetInstallersResult[0].CrewNames.trim().length > 0 ? data.GetInstallersResult[0].CrewNames : "Un assigned");

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