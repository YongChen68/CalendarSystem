var displayType = "Windows";
var GlobalParams = [];
var eventArray = [];
var debug = true;
var renderingComplete = false;
var eventid;
var eventWO = [];
var eventWODict = [];


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
function sendUpdateToServer(event) {
    var eventToUpdate = {
        id: event.id,
        start: event.start,
        title: event.title,
        description: event.description,
        doors: event.doors
    };
    if (event.end === null) {
        eventToUpdate.end = eventToUpdate.start;
    }
    else {
        eventToUpdate.end = event.end;
    }
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

    if (displayType == "Installation") {
        PageMethods.UpdateInstallationEventTime(displayType, eventToUpdate);
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
    var dayName = ["sun", "mon", "tue", "wed", "thu", "fri", "sat"][day.getUTCDay()];
    if (displayType == "Installation") {
        return { day: dayName, date: new Date(day.valueOf()), doors: 0, windows: 0, SalesAmmount: 0,WOCount:0 };
    }
    else {
        return { day: dayName, date: new Date(day.valueOf()), doors: 0, windows: 0, boxes: 0, glass: 0, value: 0, min: 0, max: 0, Available_Time: 0, rush: 0, float: 0, TotalBoxQty: 0, TotalGlassQty: 0, TotalPrice: 0, TotalLBRMin: 0, F6CA: 0, F27DS: 0, F27TS: 0, F27TT: 0, F29CA: 0, F29CM: 0, F52PD: 0, F68CA: 0, F68SL: 0, F68VS: 0, Transom: 0, Sidelite: 0, SingleDoor: 0, DoubleDoor: 0, Simple: 0, Complex: 0, Over_Size: 0, Arches: 0, Rakes: 0, Customs: 0, };
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

        'Sales Amount:\r\n' +
        event.SalesAmmount + "\r\n" +

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
    var ret = "<img src=\"images/home.png\" title=\"" + ToInstallationEventString(val) + "\">" +
        "<img src=\"images/installer" + val.EstInstallerCnt + ".png\" title=\"Estimated number of installers for the job: " +
        val.EstInstallerCnt + "\">" +
        (val.Windows != "0" ? "&nbsp;<img alt=\"# of Windows: " + val.Windows + "Status: " + val.WindowState + "\" src=\"images/window.PNG\" />" : "") +
        (val.Doors != "0" ? "&nbsp;<img alt=\"# of Doors: " + val.Doors + "Status: " + val.DoorState + "\" src=\"images/door.PNG\" />" : "") + "&nbsp;" +
        ("WO: " + val.WorkOrderNumber) + "&nbsp;" +
        ("Name: " + val.LastName.trim().Length > 10 ? val.LastName.trim().Substring(0, 10) : val.LastName.trim()) +
        "&nbsp;" + (val.City.trim().Length > 5 ? val.City.trim().toLowerCase().Substring(0, 5) : val.City.trim().toLowerCase());
   // $(element).find(dom).prepend(ret);

    var el = $("<div class='fc-event" + (val.JobType == "RES" ? " reservation" : "") +
        "' id=\"" + val.id + "\" style=\"background-color:" + val.color + "\">" + ret + "</div>").appendTo('#external-events');
    el.draggable({
        zIndex: 999,
        revert: true,      // will cause the event to go back to its
        revertDuration: 0  //  original position after the drag
    });
    $('#' + val.id).data('event', {
        // title: val.title, id: val.id, description: val.description, doors: val.doors, windows: val.windows, type: val.type, JobType: val.JobType, boxes: val.boxes, glass: val.glass, value: val.value, min: val.min, max: val.max, rush: val.rush, float: val.float, TotalBoxQty: val.TotalBoxQty, TotalGlassQty: val.TotalGlassQty, TotalPrice: val.TotalPrice, TotalLBRMin: val.TotalLBRMin, F6CA: val.F6CA, F27DS: val.F27DS, F27TS: val.F27TS, F27TT: val.F27TT, F29CA: val.F29CA, F29CM: val.F29CM, F52PD: val.F52PD, F68CA: val.F68CA, F68SL: val.F68SL, F68VS: val.F68VS, DoubleDoor: val.DoubleDoor, Transom: val.Transom, Sidelite: val.Sidelite, SingleDoor: val.SingleDoor
        title: val.title, id: val.id, doors: val.Doors, windows: val.Windows
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

    var el = $("<div class='fc-event" + (val.JobType == "RES" ? " reservation" : "") + "' id=\"" + val.id + "\" style=\"background-color:" + val.color + "\">" + img + val.title + "</div>").appendTo('#external-events');
    el.draggable({
        zIndex: 999,
        revert: true,      // will cause the event to go back to its
        revertDuration: 0  //  original position after the drag
    });
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
   // LoadInstallationBufferedJobs();

    /* initialize the calendar
    -----------------------------------------------------------------*/

    $('#calendar').fullCalendar({
        aspectRatio: 1.7,

      

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
           
        },
        header: {
            left: 'prev,next today, changeType,applyFilters,applyInstallationFilters,changeBranch,changeJobType,changeShippingType',
            center: 'title',
            right: 'ShowWIP,month,agendaWeek,agendaDay',
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
                $('.fc-ShowWIP-button').show();
                $('.fc-applyFilters-button').hide();
                $('.fc-changeJobType-button').hide();
                $('.fc-changeShippingType-button').hide();
            //    $('#calendar').fullCalendar('changeView', 'month');
              //  document.getElementById('external-InstallationEvents').style.display = "block";
              //  document.getElementById('external-events').style.display = "none";
                LoadInstallationBufferedJobs();
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
                            item.editable = (item.HolidayName != null) ? false : true;
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
            else {
                $('.fc-applyInstallationFilters-button').hide();
                $('.fc-ShowWIP-button').hide();
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
            if (displayType != "Installation") {
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
         
            element.attr('href', 'javascript:void(0);');
            element.click(function () {
                if (displayType == "Installation") {
                    $("#workOrder").html(event.WorkOrderNumber);
                    $("#homePhone").html(event.HomePhoneNumber);
                    $("#cellPhone").html(event.CellPhone);
                    $("#branch").html(event.Branch);
                    $("#Address").html(event.StreetAddress + "\r\n" +"," +
                        event.City + "\r\n\r\n" );
                    $("#SalesAmmount").html(event.SalesAmmount.formatMoney(2, "$", ",", "."));
                    $("#SeniorInstaller").html(event.SeniorInstaller != null && event.SeniorInstaller.trim().length > 0 ? event.SeniorInstaller : "Unspecified");
                    $("#CrewNames").html(event.CrewNames != null && event.CrewNames.trim().length > 0 ? event.CrewNames : "Un assigned");

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
                  
                    $("#eventLink").attr('href', event.url);
                    $("#eventContent").dialog({ modal: true, title: event.LastName, width: 800 });

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
               
                var ret = "<img src=\"images/home.png\" title=\"" + ToInstallationEventString(event) + "\">" +
                    "<img src=\"images/installer" + event.EstInstallerCnt + ".png\" title=\"Estimated number of installers for the job: " +
                    event.EstInstallerCnt+ "\">" +
                    (event.Windows != "0" ? "&nbsp;<img alt=\"# of Windows: " + event.Windows + "Status: " + event.WindowState + "\" src=\"images/window.PNG\" />" : "") +
                    (event.Doors != "0" ? "&nbsp;<img alt=\"# of Doors: " + event.Doors + "Status: " + event.DoorState + "\" src=\"images/door.PNG\" />" : "") + "&nbsp;" +
                    ("WO: " + event.WorkOrderNumber) + "&nbsp;" +
                    ("Name: " + event.LastName.trim().Length > 10 ? event.LastName.trim().Substring(0, 10) : event.LastName.trim()) +
                    "&nbsp;" + (event.City.trim().Length > 5 ? event.City.trim().toLowerCase().Substring(0, 5) : event.City.trim().toLowerCase());

                $(element).find(dom).prepend(ret);
              
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
          
            var dayId = FindDayIdfromTotals(GetDatefromMoment(event.ScheduledDate));
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
            totals[dayId].Complex += event.Complex !== undefined ? event.Complex : 0;
            totals[dayId].Simple += event.Simple !== undefined ? event.Simple : 0;
            totals[dayId].Over_Size += event.Over_Size !== undefined ? event.Over_Size : 0;
            totals[dayId].Arches += event.Arches !== undefined ? event.Arches : 0;
            totals[dayId].Rakes += event.Rakes !== undefined ? event.Rakes : 0;
            totals[dayId].Customs += event.Customs !== undefined ? event.Customs : 0;
            //console.log(dayId, "totals[dayId].Transom:", totals[dayId].Transom, "totals[dayId].Sidelite:", totals[dayId].Sidelite, "totals[dayId].SingleDoor:", totals[dayId].SingleDoor, "totals[dayId].DoubleDoor:", totals[dayId].DoubleDoor, "event.Transom:", event.Transom, "event.Sidelite:", event.Sidelite, "event.SingleDoor:", event.SingleDoor, "event.DoubleDoor:", event.DoubleDoor);
            //}
        },
        eventDragStop: function (event, jsEvent, ui, view) {
            if (debug) console.log("eventDragStop", event);
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

            if (displayType != "Installation") {

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
                
                for (var i = 0; i < totals.length; i++) {
                    date1 = new Date(totals[i]["date"]).toLocaleDateString('en-US');
                    WOCount = 0;
                    for (var j = 0; j < eventWODict.length; j++) {
                        date2 = new Date(GetDatefromMoment(eventWODict[j]["ScheduledDate"])).toLocaleDateString('en-US');
                        if  (date1 == date2){

                            totals[i].windows += eventWODict[j]["Windows"];
                            totals[i].doors += eventWODict[j]["Doors"];
                            totals[i].SalesAmmount += eventWODict[j]["SalesAmmount"];
                            totals[i].WOCount = WOCount + 1;
                            WOCount++;
                        }
                       
                    }
                }

                for (var i = 0; i < totals.length; i++) {
                    SetDayValue(i, totals[i]);
                }
            }
        },
        viewRender: function (view, element) {
            if (debug) console.log("viewRender", "view configuration:", view.title, view.intervalStart._d);
            totals = getBlankTotal();
            ControlHeaderVisibility(GetDisplayItemList(displayType));
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
     
        SetData('Codel-Doors', dayTotals.day, dayTotals.doors);
        SetData('Windows', dayTotals.day, dayTotals.windows);
        SetData('Work-Orders', dayTotals.day, dayTotals.WOCount);
       // SetData('Windows', dayTotals.day, 3);
        //  SetData('Work Orders', dayTotals.day, 0);
        //  SetData('Installation Min', dayTotals.day, 0);
        //  SetData('Asbestos Jobs', dayTotals.day, 0);
        //  SetData('High Risk Jobss', dayTotals.day, 0);
        SetData('Sales-Amount', dayTotals.day, dayTotals.SalesAmmount.formatMoney(2, "$", ",", "."));
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
        SetData('Simple', dayTotals.day, dayTotals.Simple);
        SetData('Complex', dayTotals.day, dayTotals.Complex);
        SetData('Over_Size', dayTotals.day, dayTotals.Over_Size);
        SetData('Arches', dayTotals.day, dayTotals.Arches);
        SetData('Rakes', dayTotals.day, dayTotals.Rakes);
        SetData('Customs', dayTotals.day, dayTotals.Customs);
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

    if ((isSaturdayChecked == true) && (isSaturdayChecked == true)) {
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
    for (i = 0; i < document.getElementsByName('InstallationState').length; i++) {
            document.getElementsByName('InstallationState')[i].checked = true;
        }
    $('#' + target + 'Filter').addClass('hidden');
    $('#calendar').fullCalendar('refetchEvents');
    $('#calendar').fullCalendar('rerenderEvents');
}
function ChangeType(type) {
    displayType = type;
    $('.fc-changeType-button').html(displayType);
    $('#calendar').fullCalendar('refetchEvents');
    $('#external-events').find(".fc-event").remove();
    ControlHeaderVisibility(GetDisplayItemList(displayType));

    LoadBufferedJobs();
    $('#typeChange').addClass('hidden');
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
        return [{ Id: 'Windows', Display: true }, { Id: 'Doors', Display: true }, { Id: 'Boxes', Display: true }, { Id: 'Glass', Display: true }, { Id: 'Rush', Display: true }, { Id: 'Min', Display: true }, { Id: 'Max', Display: true }, { Id: 'Available_Time', Display: true }, { Id: 'Available_Staff', Display: true }, { Id: '26CA', Display: true }, { Id: '27DS', Display: true }, { Id: '27TS', Display: true }, { Id: '27TT', Display: true }, { Id: '29CA', Display: true }, { Id: '29CM', Display: true }, { Id: '68CA', Display: false }, { Id: '68SL', Display: true }, { Id: '68VS', Display: true }, { Id: '52PD', Display: true }, { Id: 'Transom', Display: false }, { Id: 'Sidelite', Display: false }, { Id: 'SingleDoor', Display: false }, { Id: 'DoubleDoor', Display: false }/**/, { Id: 'Simple', Display: true }, { Id: 'Complex', Display: true }, { Id: 'Over_Size', Display: true }, { Id: 'Arches', Display: true }, { Id: 'Rakes', Display: true }, { Id: 'Customs', Display: true }];
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
function removeDuplicates(originalArray, prop) {
    var newArray = [];
    var lookupObject = {};

    for (var i in originalArray) {
        lookupObject[originalArray[i][prop]] = originalArray[i];
    }

    for (i in lookupObject) {
        newArray.push(lookupObject[i]);
    }
    return newArray;
}
