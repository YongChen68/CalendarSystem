<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CalendarSystem._Default" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <!-- Latest compiled and minified CSS -->

    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">

    <!-- Optional theme -->
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap-theme.min.css" integrity="sha384-rHyoN1iRsVXV4nD0JutlnGaslCJuC7uwjduW9SVrLvRYooPp2bWYgmgJQIXwl/Sp" crossorigin="anonymous">
 <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <!-- Latest compiled and minified JavaScript -->
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js" integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa" crossorigin="anonymous"></script>
    <link href='js/fullcalendar.css' rel='stylesheet' />
    <link href='js/scheduler.css' rel='stylesheet' />
    <link href='css/application.css' rel='stylesheet' />

 
<link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">



    <link href='js/fullcalendar.print.css' rel='stylesheet' media='print' />


    <!-- Include all compiled plugins (below), or include individual files as needed -->
    <script src='lib/jquery-ui.custom.min.js'></script>
       <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>

    <script src='lib/moment.min.js'></script>
    <script src='js/fullcalendar.js'></script>
    <script src='js/scheduler.js'></script>
    <script src="js/calendarprocessing.js"></script>
    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAhsQnBPh07vYae9Oakwczkyv8gTDY9j-U"></script>

    <script>
        var readonly = "<%= ReadOnly %>";
    </script>
    <%--<script type="text/javascript">
        $(function () 
        {
            $("#<%=this.txtStart.ClientID %>").datepicker({
                maxDate: new Date(),
                dateFormat: "dd-M-yy",
                orientation: "top right",
                numberOfMonths: 1,
                onSelect: function (selected) {
                    alert("test");
                    var sd = new Date(selected);
                    sd.setDate(sd.getDate() + 1);
                    $("#<%=this.txtEnd.ClientID %>").datepicker("option", "minDate", sd);
                }
            });

              $("#<%=this.txtEnd.ClientID %>").datepicker({
                maxDate: new Date(),
                  dateFormat: "dd-M-yy",
                orientation: "top right",
                numberOfMonths: 1,
                onSelect: function (selected) {
                    var ed = new Date(selected);
                    sd.setDate(sd.getDate() - 1);
                    $("#<%=this.txtStart.ClientID %>").datepicker("option", "maxDate", ed);
                }
            });

        });
    </script>--%>

    <script>
        var geocoder;
        var map, mapRemeasure, mapWindows;



        $(function () {
            // $("#from_date").datepicker();
            $('#wooddropdate').datepicker({
                dateFormat: "m/d/yy"

            });
            $('#calledLogDate').datepicker({
                dateFormat: "m/d/yy"

            });

            $('#notesDate').datepicker({
                dateFormat: "m/d/yy"

            });
            $('#from_date').datepicker({
                dateFormat: "m/d/yy"

            });
            //   $("#end_date").datepicker();
            $('#end_date').datepicker({
                dateFormat: "m/d/yy"

            });
            // $("#InstallScheduledStartDate").datepicker();
            $('#InstallScheduledStartDate').datepicker({
                dateFormat: "m/d/yy"

            });
            // $("#InstallScheduledEndDate").datepicker();
            $('#InstallScheduledEndDate').datepicker({
                dateFormat: "m/d/yy"

            });

            $('#RemeasureDate').datepicker({
                dateFormat: "m/d/yy"

            });

            $('.btn-minuse').on('click', function () {
                $(this).parent().siblings('input').val(parseInt($(this).parent().siblings('input').val()) - 1)
            })

            $('.btn-pluss').on('click', function () {
                $(this).parent().siblings('input').val(parseInt($(this).parent().siblings('input').val()) + 1)
            })
        });

        function initialize() {
            geocoder = new google.maps.Geocoder();
            var latlng = new google.maps.LatLng(-34.397, 150.644);
            var mapOptions = {
                zoom: 16,
                center: latlng,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                mapTypeControl: false
            }
            map = new google.maps.Map(document.getElementById('map'), mapOptions);
            mapRemeasure = new google.maps.Map(document.getElementById('mapRemeasure'), mapOptions);
            mapWindows = new google.maps.Map(document.getElementById('mapWindows'), mapOptions);

            var marker = new google.maps.Marker({
                position: latlng,
                map: map,
                title: "location"
            });

            var fileUpload = $("#fileUpload").get(0);
            var fileDisplayArea = document.getElementById('file');


            fileUpload.addEventListener('change', function (e) {
                var file = fileUpload.files[0];
                var reader = new FileReader();
                reader.onload = function (e) {
                    fileDisplayArea.innerHTML = "";
                    var theBytes = e.target.result; //.split('base64,')[1]; // use with uploadFile2
                    fileByteArray.push(theBytes);
                    for (var i = 0; i < fileByteArray.length; i++) {
                        fileDisplayArea.innerText += fileByteArray[i];
                    }
                    //fileDisplayArea.innerText = reader.result;
                }

               // reader.readAsArrayBuffer(file);
                reader.readAsDataURL(file);

            });
        }



        function codeAddress() {
            var address = document.getElementById('Address').innerHTML + "," + document.getElementById('City').innerHTML;
            var request = {
                address: address,
                componentRestrictions: {
                    country: 'CA'
                }
            }

            geocoder.geocode(request, function (results, status) {
                if (status == 'OK') {
                    map.setCenter(results[0].geometry.location);
                    var marker = new google.maps.Marker({
                        map: map,
                        position: results[0].geometry.location
                    });
                } else {
                    alert('Geocode was not successful for the following reason: ' + status);
                }
            });


        }

        function codeAddressRemeasure() {
            var address = document.getElementById('AddressRemeasure').innerHTML;
            var request = {
                address: address,
                componentRestrictions: {
                    country: 'CA'
                }
            }

            geocoder.geocode(request, function (results, status) {
                if (status == 'OK') {
                    mapRemeasure.setCenter(results[0].geometry.location);
                    var marker = new google.maps.Marker({
                        map: mapRemeasure,
                        position: results[0].geometry.location
                    });
                } else {
                    alert('Geocode was not successful for the following reason: ' + status);
                    mapRemeasure = null;
                }
            });


        }

        function codeAddressWindows() {
            var address = document.getElementById('AddressWindows').innerHTML;

            var request = {
                address: address,
                componentRestrictions: {
                    country: 'CA'
                }
            }

            geocoder.geocode(request, function (results, status) {
                if (status == 'OK') {
                    mapWindows.setCenter(results[0].geometry.location);
                    var marker = new google.maps.Marker({
                        map: mapWindows,
                        position: results[0].geometry.location
                    });
                } else {
                    alert('Geocode was not successful for the following reason: ' + status);
                    mapWindows = null;
                }
            });


        }
    </script>


    <style>
        body {
            margin-top: 40px;
            text-align: center;
            font-size: 14px;
            font-family: "Lucida Grande",Helvetica,Arial,Verdana,sans-serif;
        }

        .dot {
            height: 12px;
            width: 12px;
            border-radius: 50%;
            display: inline-block;
            margin-right: 10px;
        }

        #map {
            float: left;
            width: 45%;
        }

        #mapRemeasure {
            float: left;
            width: 45%;
        }

        #mapWindows {
            float: left;
            width: 45%;
        }


        #content {
            float: left;
            width: 55%;
            vertical-align: top;
            text-align: left;
            padding-left: 20px;
        }

        #contentRemeasure {
            float: left;
            width: 55%;
            vertical-align: top;
            text-align: left;
            padding-left: 20px;
        }

        #contentWindows {
            float: left;
            width: 55%;
            vertical-align: top;
            text-align: left;
            padding-left: 20px;
        }


        #installationContent {
            float: left;
            width: 55%;
            vertical-align: top;
            text-align: left;
            padding-left: 20px;
        }

        #wrap {
            width: 100%;
            margin: 0 auto;
            overflow: hidden;
        }

        #external-events, #external-events1, #external-eventsRemeasure {
            float: left;
            overflow-x: hidden;
            width: 200px;
            padding: 0 10px;
            border: 1px solid #ccc;
            background: #eee;
            text-align: left;
            overflow-y: auto;
            height: 1000px;
        }


            #external-events, #external-events1, #external-eventsRemeasure h4 {
                font-size: 16px;
                margin-top: 0;
                padding-top: 1em;
            }

                #external-events .fc-event {
                    margin: 10px 0;
                    cursor: pointer;
                }

                #external-events1 .fc-event {
                    margin: 10px 0;
                    cursor: pointer;
                }

            #external-eventsRemeasure .fc-event {
                margin: 10px 0;
                cursor: pointer;
            }

            #external-events p {
                margin: 1.5em 0;
                font-size: 11px;
                color: #666;
            }

            #external-events1 p {
                margin: 1.5em 0;
                font-size: 11px;
                color: #666;
            }

            #external-eventsRemeasure p {
                margin: 1.5em 0;
                font-size: 11px;
                color: #666;
            }

            #external-events p input {
                margin: 0;
                vertical-align: middle;
            }

            #external-events1 p input {
                margin: 0;
                vertical-align: middle;
            }

            #external-eventsRemeasure p input {
                margin: 0;
                vertical-align: middle;
            }

        #calendar {
            float: right;
            width: 85%;
        }

        #eventContent {
            z-index: 214748367;
        }

        #eventRemeasureContent {
            z-index: 214748367;
        }

        #taskModal {
            z-index: 214748367;
        }


        #installerEdit {
            z-index: 214748367;
        }

        #installerAdd {
            z-index: 214748367;
        }

        
        #TruckPop {
            z-index: 214748366;
        }


        #eventContentWindows {
            z-index: 214748367;
        }

        .modal-header {
            background-color: #9FB6CD;
            color: white;
            font-weight: 400;
        }

        .task-modal-header {
            background-color: white;
            color:black;
            font-weight: 400;
        }

        .modal-ku {
            width: 1800px;
            margin: auto;
        }

        .modal-sm {
            width: 1200px;
            padding-top: 100px;
            margin: auto;
        }



        
        .modal-task {
            width: 800px;
            height:500px;
            padding-top: 100px;
            margin: auto;
        }






        .close {
            color: #fff;
            opacity: 1;
        }

        .table-condensed {
        }

        #FromDate, #ToDate {
            width: 50%;
            float: left;
        }

        #wrapper1, #wrapper2, #wrapper3 {
            display: flex;
        }

        #left1, #left2, #left3, {
            float: left;
            width: 55%;
            overflow: hidden;
        }

        #right1, #right2, #right3 {
            float: right;
            width: 45%;
            overflow: hidden;
        }

        .leftcolumn {
            float: left;
            width: 20%;
        }

        .rightcolumn {
            float: left;
            width: 80%;
        }

        /* Clear floats after the columns */
        .container:after {
            content: "";
            display: table;
            clear: both;
        }
    </style>

</head>
<body onload="initialize()">
    <form id="main" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
        </asp:ScriptManager>

        <div id="eventContent" style="display: none;" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel"
            aria-hidden="true" class="modal fade">
            <div class="modal-dialog modal-ku" role="document">
                <div class="modal-content">
                    <div class="modal-header " id="WorkOrderTitleHeader">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>

                        </button>
                        <h4 class="modal-title" id="WorkOrderTitle"></h4>

                    </div>

                    <div class="modal-body">
                        <div role="tabpanel">
                            <!-- Nav tabs -->
                            <ul class="nav nav-tabs" role="tablist">
                                <li role="presentation" class="active">
                                    <a href="#CustomerTab" aria-controls="customerTab" role="tab" data-toggle="tab">CUSTOMER</a>

                                </li>
                                <li role="presentation">
                                    <a href="#InstallTab" aria-controls="uploadTab" role="tab" data-toggle="tab">INSTALL</a>

                                </li>
                                <li role="presentation">
                                    <a href="#InstallationTab" aria-controls="installationTab" role="tab" data-toggle="tab">INSTALLATION STATUS</a>

                                </li>
                                <li role="presentation">
                                    <a href="#ManufacturingTab" aria-controls="manufacturingTab" role="tab" data-toggle="tab">MANUFACTURING STATUS</a>

                                </li>
                                <li role="presentation">
                                    <a href="#SubTradesTab" aria-controls="SubTradesTab" role="tab" data-toggle="tab">SUB TRADES</a>
                                </li>
                                <li role="presentation">
                                    <a href="#JobAnalysisTab" aria-controls="JobAnalysisTab" role="tab" data-toggle="tab">JOB ANALYSIS</a>

                                </li>
                                <li role="presentation">
                                    <a href="#CalledLogTab" aria-controls="CalledLogTab" role="tab" data-toggle="tab">Call Log</a>

                                </li>

                                <li role="presentation">
                                    <a href="#NotesTab" aria-controls="NotesTab" role="tab" data-toggle="tab">Notes</a>

                                </li>
                                 <li role="presentation">
                                    <a href="#WOPictureTab" aria-controls="WOPictureTab" role="tab" data-toggle="tab">Photo Gallery</a>
                                </li>
                                 <li role="presentation">
                                    <a href="#DocumentLibraryTab" aria-controls="DocumentLibraryTab" role="tab" data-toggle="tab">Document</a>
                                </li>
                                <li role="presentation">
                                    <a href="#JobReviewTab" aria-controls="JobReviewTab" role="tab" data-toggle="tab">Job Review</a>
                                </li>
                            </ul>
                            <!-- Tab panes -->
                            <div class="tab-content">
                                <div role="tabpanel" class="tab-pane active" id="CustomerTab">
                                    <div id="map"></div>

                                    <div id="content">
                                        <br>
                                        <br>
                                        <div><b>First Name: </b><span id="FirstName"></span></div>
                                        <br>
                                        <div><b>Last Name: </b><span id="LastName"></span></div>
                                        <br>
                                        <div><b>Work Order: </b><span id="workOrder"></span></div>
                                        <br>
                                        <div><b>Street Address: </b><span id="Address"></span></div>
                                        <br>
                                        <div><b>City: </b><span id="City"></span></div>
                                        <br>
                                        <div><b>Postal Code: </b><span id="postalCode"></span></div>
                                        <br>
                                        <div><b>Home Phone: </b><span id="homePhone"></span></div>
                                        <br>
                                        <div><b>Work Phone: </b><span id="workPhone"></span></div>
                                        <br>
                                        <div><b>Cell Phone: </b><span id="cellPhone"></span></div>
                                        <br>
                                        <div><b>Email: </b><span id="email"></span></div>
                                        <br>
                                        <div><b>Sales Rep: </b><span id="salesRep"></span></div>
                                        <br>

                                        <div><b>Age of Home: </b><span id="ageOfHome"></span></div>
                                        <br>

                                        <div><b>HomeDepot Job: </b><span id="homeDepotJob"></span></div>
                                        <br>

                                        <div><b>Total Windows: </b><span id="TotalWindows1"></span></div>

                                        <br>
                                        <div><b>Total Patio Doors: </b><span id="TotalDoors1"></span></div>
                                        <br>

                                        <div><b>Total Codel Doors: </b><span id="TotalDoors2"></span></div>
                                        <br>
                                        <div><b>Total Sales Amount: </b><span id="SalesAmmount"></span></div>

                                        <br>
                                         <div><b>Daily Sales Amount: </b><span id="DailySalesAmmount"></span></div>

                                        <br>
                                    </div>
                                </div>
                                <div role="tabpanel" class="tab-pane" id="InstallTab">
                                    <div id="installationContent" style="width: 600px; margin-left: 20px; padding-left: 100px; float: left;">
                                        <br>
                                        <br>
                                        <span><b>Installation Scheduled Date:  &nbsp;</b>
                                            <span  id="DaysOfWork"></span>  &nbsp; days
                                         
                                        </span>
                                        <div class="container">

                                            <div class="leftcolumn">
                                                <div class="container">
                                                    <div class="leftcolumn" style="width: 5%; align-items: center;">
                                                        Start Date:
                                                    </div>
                                                    <div class="rightcolumn" style="width: 20%;">
                                                        <input id="InstallScheduledStartDate" style="width: 160px; text-align: center;" class="form-control" data-toggle="tooltip" title="Start Date">
                                                    </div>

                                                </div>

                                            </div>
                                            <div class="rightcolumn">
                                                <div class="container">
                                                    <div class="leftcolumn" style="width: 5%; padding-left: 50px;">
                                                        End Date:
                                                    </div>

                                                    <div class="rightcolumn" style="padding-left: 50px; width: 5%;">
                                                        <input id="InstallScheduledEndDate" style="width: 160px; text-align: center;" class="form-control" data-toggle="tooltip" title="End Date">
                                                    </div>
                                                </div>
                                            </div>

                                        </div>

                                        <br>

                                        <div><b>Senior Installer: </b><span id="SeniorInstaller"></span></div>
                                        <br>
                                        <div><b>CrewNames: </b><span id="CrewNames"></span>
                                      
                                       </div>
                                        <div>
                                             <span id="ViewDeleteCrewNames" style="display:none;padding-left:20px;"> </span>
                                             <span id="AddCrewNames" style="padding-left:40px;"></span> 
                                        </div>
                                        <br>

                                        <div><b>Remeasurer: </b><span id="Remeasurer"></span></div>
                                        <br>
                                        
                                        <div class="container">
                                            <div class="leftcolumn">
                                                <b>Number of Installers: </b>
                                            </div>


                                            <div class="input-group" style="width: 20%">
                                                <span class="input-group-btn">
                                                    <button class="btn btn-white btn-minuse" type="button">-</button>
                                                </span>
                                                <input type="text" class="form-control no-padding add-color text-center height-25" maxlength="3" id="NumOfInstallers" style="padding-left: -30px;">
                                                <span class="input-group-btn">
                                                    <button class="btn btn-red btn-pluss" type="button">+</button>
                                                </span>
                                            </div>
                                        </div>
                                        <br>

                                        <div class="container">
                                            <div class="leftcolumn">
                                                    <image src="images/asbestos.PNG"></image>  <b>Asbestos-Jobs: </b>

                                                <div class="radio">
                                                    <label>
                                                        <input type="radio" name="Asbestos-Jobs" id="Asbestos-JobsYes">Yes</label>
                                                </div>
                                                <div class="radio">
                                                    <label>
                                                        <input type="radio" name="Asbestos-Jobs" id="Asbestos-JobsNo">No</label>
                                                </div>
                                            </div>
                                            <div class="rightcolumn">
                                        <image src="images/asbestos.PNG"></image>   <b>Lead-Paint : </b>

                                                <div class="radio">
                                                    <label>
                                                        <input type="radio" name="Lead-Paint" id="Lead-PaintYes">Yes</label>
                                                </div>
                                                <div class="radio">
                                                    <label>
                                                        <input type="radio" name="Lead-Paint" id="Lead-PaintNo">No</label>
                                                </div>
                                            </div>

                                        </div>


                                        <div class="container">
 

                                            <div class="leftcolumn">
                                             <image src="images/delivery.PNG"></image>     <b>Wood-DropOff-Jobs : </b>

                                                <div class="radio">
                                                    <label>
                                                        <input type="radio" name="Wood-DropOff-Jobs" id="Wood-DropOff-JobsYes" onclick="WoodDropOff('show');">Yes</label>
                                                </div>
                                                <div class="radio">
                                                    <label>
                                                        <input type="radio" name="Wood-DropOff-Jobs" id="Wood-DropOff-JobsNo" onclick="WoodDropOff('hide');">No</label>
                                                </div>
                                            </div>
                                            <div class="rightcolumn">
                                            <image src="images/risk.PNG"></image>     <b>HighRisk-Jobs : </b>

                                                <div class="radio">
                                                    <label>
                                                        <input type="radio" name="HighRisk-Jobs" id="HighRisk-JobsYes">Yes</label>
                                                </div>
                                                <div class="radio">
                                                    <label>
                                                        <input type="radio" name="HighRisk-Jobs" id="HighRisk-JobsNo">No</label>
                                                </div>

                                            </div>
                                        </div>
                                        <br>

                                        <span style="display:none;"><b>Weekends Setting: </b></span>
                                        <div style="width: 800px;display:none;" class="container" >
                                            <div class="leftcolumn">
                                                Saturday:
                                                <input type="checkbox" name="saturday" disabled="disabled">
                                            </div>
                                            <div class="rightcolumn">
                                                Sunday: &nbsp;&nbsp;<input type="checkbox" name="sunday" disabled="disabled">
                                            </div>
                                        </div>
                                     
                                        <span><b>Return Scheduled Date:</b></span>
                                        <div class="form-group" style="width: 800px;" id="ReturnedJob">
                                            <div class="container">

                                                <div class="leftcolumn">
                                                    <div class="container">
                                                        <div class="leftcolumn" style="width: 5%; align-items: center;">
                                                            Start Date:
                                                        </div>
                                                        <div class="rightcolumn" style="width: 20%;">
                                                            <input id="from_date" style="width: 160px; text-align: center;" class="form-control" data-toggle="tooltip" title="Start Date">
                                                        </div>

                                                    </div>

                                                </div>

                                                <div class="rightcolumn">
                                                    <div class="container">
                                                        <div class="leftcolumn" style="width: 5%; padding-left: 50px;">
                                                            End Date:
                                                        </div>

                                                        <div class="rightcolumn" style="padding-left: 50px; width: 5%;">
                                                            <input id="end_date" style="width: 160px; text-align: center;" class="form-control" data-toggle="tooltip" title="End Date">
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                         <div class="form-group" style="width: 800px;" id="ReturnedJobReason">
                                            <div class="container">

                                                <div class="leftcolumn">
                                                    <div class="container">
                                                        <div class="leftcolumn" style="width: 5%; align-items: center;">
                                                           Reason:
                                                        </div>
                                                        <div class="rightcolumn" style="width: 20%;">
                                                              <textarea class="form-control" rows="5" id="returnJobReasonID" style="width:400px;"></textarea>
                                                        </div>

                                                    </div>

                                                </div>

                                                <div class="rightcolumn">
                                                    <div class="container">
                                                       
                                                    </div>
                                                </div>

                                            </div>
                                        </div>

                                        <div  id="divWoodDropOffDate" style="display:none;"> 
                                            <span><b>Wood Drop Off Date:</b></span>
                                            <div class="form-group" style="width: 800px;" id="WoodDropOffDate">
                                            <div class="container">

                                                <div class="leftcolumn">
                                                    <div class="container">
                                                        <div class="leftcolumn" style="width: 5%; align-items: center;">
                                                            Start Date:
                                                        </div>
                                                        <div class="rightcolumn" style="width: 20%;">
                                                            <input id="wooddropdate" style="width: 160px; text-align: center;" class="form-control" data-toggle="tooltip" title="Date">
                                                        </div>

                                                    </div>

                                                </div>

                                                <div class="rightcolumn">
                                                    <div class="container">
                                                        <div class="leftcolumn" style="width: 5%; padding-left: 50px;">
                                                            Time:
                                                        </div>

                                                        <div class="rightcolumn" style="padding-left: 50px; width: 5%;">
                                                            <input type="time" id="wooddropTime" name="appt">
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                        </div>
                                        <input type="button" name="btnSave" id="btnSave" class="btn btn-success" value="Save" onclick="UpdateInstallationEvents()">
                                    </div>
                                    <div >
                                         <div  id="installerDiv" style="margin-left: 620px; display:none;">  <img src="images/installerimage.jpg" /></div>
                                    </div>
                                </div>

                                <div role="tabpanel" class="tab-pane " id="InstallationTab">
                                    <br />
                                    <div style=" text-align:left;"><b> <span id="installationWindows" >WINDOWS</span></b>
                                    </div>
                                    <div > <span id="noInstallationWindows" style=" text-align:left; display:none;">This order does not contain any windows.</span>
                                    </div>
                                    <div style="overflow: auto; ">
                                        <table id="dataTableInstallationWindows" class="table table-striped table-bordered table-hover table-condensed"></table>
                                    </div>
                                    <br />
                                    <div style=" text-align:left;"> <b> <span id="installationDoors">DOORS</span></b>  </div>
                                    <div > <span id="noInstallationDoors" style=" text-align:left; display:none;">This order does not contain any doors.</span>
                                    </div>
                                    <div style="overflow: auto; ">
                                        <table id="dataTableInstallationDoors" class="table table-striped table-bordered table-hover table-condensed"></table>
                                    </div>

                                </div>

                                 <div role="tabpanel" class="tab-pane " id="ManufacturingTab">
                                    <br />
                                    <div style=" text-align:left;"><b> <span id="manufacturingWindows" >WINDOWS</span></b>
                                    </div>
                                    <div > <span id="noManufacturingWindows" style=" text-align:left; display:none;">This order does not contain any windows.</span>
                                    </div>
                                    <div style="overflow: auto; ">
                                        <table id="dataTableManufacturingWindows" class="table table-striped table-bordered table-hover table-condensed"></table>
                                    </div>
                                    <br />
                                    <div style=" text-align:left;"> <b> <span id="manufacturinDoors">DOORS</span></b>  </div>
                                    <div > <span id="noManufacturingDoors" style=" text-align:left; display:none;">This order does not contain any doors.</span>
                                    </div>
                                    <div style="overflow: auto; ">
                                        <table id="dataTableManufacturingDoors" class="table table-striped table-bordered table-hover table-condensed"></table>
                                    </div>

                                </div>
                                <div role="tabpanel" class="tab-pane " id="SubTradesTab">
                                     <br/>
                                    <div > <span id="noSubTrades" style=" text-align:left; display:none;">No Sub Trades required for this job.</span>
                                    </div>
                                    <div style="overflow: auto; ">
                                        <table id="dataTableSubTrades" class="table table-striped table-bordered table-hover table-condensed"></table>
                                    </div>
                                </div>
                                <div role="tabpanel" class="tab-pane " id="JobAnalysisTab"> 
                                     <div id="LBRContent" style="width: 600px; margin-left: 20px; padding-left: 100px;  text-align:left;"">
                                        <br/>
                                        <br/>
                                         <b>Total LBR Mins: </b><span id="TotalLBRMin"></span>
                                    </div>

                                </div>
                                <div role="tabpanel" class="tab-pane " id="CalledLogTab">
                                     <br/>
                                    
                                    <div style="overflow: auto;">
                                        <table id="dataTableCalledLog" class="table table-striped table-bordered table-hover table-condensed"></table>
                                    </div>
                                    <div id="CalledLogWin"  style="width: 800px; text-align:left;">
                                        <div class="form-group">
                                            <label for="calledDate">Called Date :</label>
                                        <input id="calledLogDate" style="text-align: center;" data-toggle="tooltip" title="Date">

                                         <label for="Time">Time :</label>
                                         <input type="time" id="CalledLogTime" name="appt"  >

                                             
                                        </div>
                              
                                        <div class="form-group">
                                            <label for="comment">Called Message :</label>
                                             <select id="MessageOption" name="MessageOption">
                                              <option value="Left Message">Left Message</option>
                                              <option value="No Answering Machine">No Answering Machine</option>
                                              <option value="Spoke With Customer">Spoke With Customer</option>
                                            </select>
                                             
                                        </div>
                                        <div class="form-group">
                                              <label for="comment">Notes:</label>
                                            <textarea class="form-control" rows="5" id="comment"></textarea>
  
                                        </div>
                                        
                                          <div class="form-group" style="display:none;">
                                           
                                            <textarea class="form-control" rows="5" id="callLogRecordID"></textarea>
  
                                        </div>
                                        
                                         <input type="button" name="btnSaveLog" id="btnSaveLog" class="btn btn-success" value="Save" onclick="UpdateInstallationCallLog()">
                                    </div>

                                </div>

                                     <div role="tabpanel" class="tab-pane " id="NotesTab">
                                     <br/>
                                    
                                    <div style="overflow: auto;">
                                        <table id="dataTableNotes" class="table table-striped table-bordered table-hover table-condensed"></table>
                                    </div>
                                    <div id="NotesWin" style="text-align:left;" >
                                        <div class="form-group">
                                          
                                           <label for="comment">General Notes Date :</label>
                                             <input id="notesDate" style="width: 160px; text-align: center;"  data-toggle="tooltip" title="Date">

                                            <label for="Time">Time :</label>
                                             <input type="time" id="notesTime" name="appt">
                                            
                                        </div>
                                        <div class="form-group">
                                            <label for="comment">Category :</label>
                                             <select id="CategoryOption" name="CategoryOption">
                                              <option value="General">General</option>
                                              <option value="Installation">Installation</option>
                                              <option value="High Risk">High Risk</option>
                                              <option value="Re-Measure">Re-Measure</option>
                                              <option value="Admin">Admin</option>
                                              <option value="Customer">Customer</option>
                                            </select>
                                        </div>
                                        <div class="form-group">
                                              <label for="comment">Notes:</label>
                                            <textarea class="form-control" rows="5" id="notes"></textarea>
  
                                        </div>
                                        
                                          <div class="form-group" style="display:none;">
                                           
                                            <textarea class="form-control" rows="5" id="notesRecordID"></textarea>
  
                                        </div>
                                        
                                         <input type="button" name="btnSaveNotes" id="btnSaveNotes" class="btn btn-success" value="Save" onclick="UpdateInstallationNotes()">
                                    </div>

                                </div>
                                <div role="tabpanel" class="tab-pane " id="WOPictureTab">
                                     <br/>
                                     <div > <span id="noPhoto" style=" text-align:left; display:none;">No photo attached to this job.</span>
                                    </div>
                                    <div style="overflow: auto; ">
                                        <table id="dataTableWOPicture" class="table table-striped table-bordered table-hover table-condensed"></table>
                                        <img id="ItemPreview" src="" />
                                    </div>

                                </div>
                                <div role="tabpanel" class="tab-pane " id="DocumentLibraryTab">
                                     <br/>
                                     <div > <span id="noDocuments" style=" text-align:left; display:none;"></span>
                                    </div>
                                    <div style="overflow: auto; ">
                                        <table id="dataTableDocumentLibrary" class="table table-striped table-bordered table-hover table-condensed"></table>
                                     </div>
                                    <div style="margin-left:-1100px;">
                                        <label for="documentNotes">File Name:</label>

                                        <input id="documentFile" style="text-align: center;" >

                                     </div>
                                  <br />
                                    <div>
                                         <input id="fileUpload" type="file"  accept=".jpg, .jpeg, .png,.doc,.docx,.xml,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document"/>
                                        <span id="file"></span>
                                     </div>
                                     <div style="margin-left:-1100px;">   
                                        <input type="button" name="btnUploadDocuments" id="btnUploadDocuments" class="btn btn-success" value="Save" onclick="UploadDocuments()">
                                     </div>
                                      
                                 

                                </div>
                                <div role="tabpanel" class="tab-pane " id="JobReviewTab">
                                     <br/>
                                     <div > <span id="noJobReview" style=" text-align:left; display:none;">No job-review record to this job.</span>
                                    </div>
                                    <div style="overflow-y:auto;height:400px;">
                                        <table id="dataTableJobReview" class="table table-striped table-bordered table-hover table-condensed"></table>
                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>


                    <div class="modal-footer">
                    </div>
                </div>
            </div>

            <br>
        </div>
        <div id="eventRemeasureContent" style="display: none;" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel"
         aria-hidden="true" class="modal fade">
        <div class="modal-dialog modal-ku" role="document">
            <div class="modal-content">
                <div class="modal-header ">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>

                    </button>
                    <h4 class="modal-title" id="WorkOrderTitleRemeasure"></h4>

                </div>

                <div class="modal-body">

                    <div role="tabpanel">
                        <!-- Nav tabs -->
                        <ul class="nav nav-tabs" role="tablist">
                            <li role="presentation" class="active">
                                <a href="#CustomerTabRemeasure" aria-controls="customerTab" role="tab" data-toggle="tab">CUSTOMER</a>

                            </li>
                            <li role="presentation">
                                <a href="#RemeasureTab" aria-controls="remeasureTab" role="tab" data-toggle="tab">REMEASURE</a>

                            </li>
                <%--            <li role="presentation">
                                <a href="#ProductTabRemeasure" aria-controls="productTab" role="tab" data-toggle="tab">PRODUCT</a>

                            </li>--%>


                        </ul>
                        <!-- Tab panes -->
                        <div class="tab-content">
                            <div role="tabpanel" class="tab-pane active" id="CustomerTabRemeasure">
                                <div id="mapRemeasure" style="height:500px;margin-top:20px;">

                                </div>

                                <div id="contentRemeasure">
                                    <br>
                                    <br>
                                    <div><b>First Name: </b><span id="FirstNameRemeasure"></span></div>
                                    <br>
                                    <div><b>Last Name: </b><span id="LastNameRemeasure"></span></div>
                                    <br>
                                    <div><b>Work Order: </b><span id="workOrderRemeasure"></span></div>
                                    <br>
                                    <div><b>Street Address: </b><span id="AddressRemeasure"></span></div>
                                    <br>
                                    <div><b>City: </b><span id="CityRemeasure"></span></div>
                                    <br>
                                    <div><b>Postal Code: </b><span id="postalCodeRemeasure"></span></div>
                                    <br>
                                    <div><b>Home Phone: </b><span id="homePhoneRemeasure"></span></div>
                                    <br>
                                    <div><b>Work Phone: </b><span id="workPhoneRemeasure"></span></div>
                                    <br>
                                    <div><b>Cell Phone: </b><span id="cellPhoneRemeasure"></span></div>
                                    <br>
                                    <div><b>Email: </b><span id="emailRemeasure"></span></div>
                                    <br>
                                    <div><b>Sales Rep: </b><span id="salesRepRemeasure"></span></div>
                                    <br>

                                    <div><b>Total Windows: </b><span id="TotalWindows1Remeasure"></span></div>

                                    <br>
                                    <div><b>Total Patio Doors: </b><span id="TotalDoors1Remeasure"></span></div>
                                    <br>

                                    <div><b>Total Codel Doors: </b><span id="TotalDoors2Remeasure"></span></div>
                                    <br>
                                    <div><b>Sales Amount: </b><span id="SalesAmmountRemeasure"></span></div>

                                    <br>
                                </div>
                            </div>

                            <div role="tabpanel" class="tab-pane" id="RemeasureTab">
                                <div id="RemeasureContent" style="width: 600px; margin-left: 0px; padding-left: 0px;">
                                    <br>
                                    <br>
                                    <div style="margin-left:-160px;"><b>Remeasure Date & Time: </b>
                                         <input type="datetime-local"  id="RemeasureDate" style="width: 250px; text-align: center;">  
                                    </div>
                                    <br>
                                    <div style="margin-left:-210px;"><b>End Time: </b>
       <%--                                  <input type="datetime-local"  id="EndRemeasureDate" style="width: 250px; text-align: center;">--%>
                                        <input type="time" id="EndRemeasureDate" name="appt">
                                    </div>
                                  
                                    <br>
                                 

                                <input type="button" name="btnSave" id="btnRemeasureSave" class="btn btn-success" value="Save" style="margin-left:-450px; " onclick="UpdateRemeasureEventsFromPopup()">
                                </div>
                            </div>
     <%--                       <div role="tabpanel" class="tab-pane " id="ProductTabRemeasure">
                                <div style="overflow: auto; height: 600px;">
                                    <table id="dataTableRemeasure" class="table table-striped table-bordered table-hover table-condensed"></table>
                                </div>

                            </div>--%>


                        </div>
                    </div>
                </div>


                <div class="modal-footer">
                </div>
            </div>
        </div>

        <br>
    </div>


        <div id="eventContentWindows" style="display: none;" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel"
            aria-hidden="true" class="modal fade">
            <div class="modal-dialog modal-ku" role="document">
                <div class="modal-content">
                    <div class="modal-header ">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>

                        </button>
                        <h4 class="modal-title" id="WorkOrderTitleWindows"></h4>

                    </div>

                    <div class="modal-body">
                        <div role="tabpanel">
                            <!-- Nav tabs -->
                            <ul class="nav nav-tabs" role="tablist">
                                <li role="presentation" class="active">
                                    <a href="#CustomerTabWindows" aria-controls="customerTab" role="tab" data-toggle="tab">CUSTOMER</a>

                                </li>
                                <li role="presentation">
                                    <a href="#WindowItemsTab" aria-controls="uploadTab" role="tab" data-toggle="tab">Windows Items</a>

                                </li>
              

                            </ul>
                            <!-- Tab panes -->
                            <div class="tab-content">
                                <div role="tabpanel" class="tab-pane active" id="CustomerTabWindows">
                                    <div id="mapWindows" style="height:500px;margin-top:20px;"></div>

                                    <div id="contentWindows">
                                        <br>
                                        <br>
                                    
                                        <div><b>Work Order: </b><span id="workOrderWindows"></span></div>
                                         <br>
                                        <div><b>Customer Name: </b><span id="CustomerNameWindows"></span></div>
                                        <br>
                                        <div><b>Branch: </b><span id="BranchWindows"></span></div>
                                        <br>
                                        <div><b>Shipping Type: </b><span id="ShippingTypeWindows"></span></div>
                                        <br>
                                        <div><b>Street Address: </b><span id="AddressWindows"></span></div>
                                         <br>
                                        <div><b>Email: </b><span id="emailWindows"></span></div>
                                        <br>
                                        <div><b>Total Windows: </b><span id="TotalWindows"></span></div>
                                       
                                        <br>
                                        <div><b>Total Patio Doors: </b><span id="TotalPatioDoors"></span></div>
                                         <br>
                              
                                        <div><b>Total Doors: </b><span id="TotalDoors"></span></div>
                                        <br>
                                        <div><b>Total Price: </b><span id="TotalPrice"></span></div>
                                     
                                      
                                       <br>
                                    </div>
                                </div>
                     

                                <div role="tabpanel" class="tab-pane " id="WindowItemsTab">
                                    <br />
                                    <div style=" text-align:left;"><b> <span id="ProductnWindows" >WINDOWS</span></b>
                                    </div>
                                    <div > <span id="noProductWindows" style=" text-align:left; display:none;">This order does not contain any windows.</span>
                                    </div>
                                    <div style="overflow: auto; ">
                                        <table id="dataTableProductWindows" class="table table-striped table-bordered table-hover table-condensed"></table>
                                    </div>
                                    <br />
                                    <div style=" text-align:left;"> <b> <span id="ProductDoors">DOORS</span></b>  </div>
                                    <div > <span id="noProductDoors" style=" text-align:left; display:none;">This order does not contain any doors.</span>
                                    </div>
                                    <div style="overflow: auto; ">
                                        <table id="dataTableProductDoors" class="table table-striped table-bordered table-hover table-condensed"></table>
                                    </div>

                                </div>

                            </div>
                        </div>
                    </div>


                    <div class="modal-footer">
                    </div>
                </div>
            </div>

            <br>
        </div>


        <div id="installerEdit" style="display: none;" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel"
         aria-hidden="true" class="modal fade">
            <div class="modal-dialog modal-sm" role="document">
            <div class="modal-content">
            <div class="modal-header" style="height:80px;">
                <h4 class="modal-title" id="installerEditTitle"></h4>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <div>
                    <table id="dataTableInstallerEdit" class="table table-striped table-bordered table-hover table-condensed"></table>
                </div>
                <div id="installerDetail" style="display:none;">
                    <div style="float: left; padding-left:100px;">
                       <img id="installerImg" src="" />
                    </div>

                     <div  style="text-align:left; padding-left:400px;">
                        <div><b>Name: </b><span id="InstallerNameView"></span></div>
                        <br>
                        <div><b>Branch: </b><span id="InstallerBranchView"></span></div>
                        <br>
                        <div><b>Department: </b><span id="InstallerDepartmentView"></span></div>
                        <br>
                        <div><b>Home Telephone: </b><span id="InstallerTelephoneView"></span></div>
                        <br>
                        <div><b>Work Phone Number: </b><span id="InstallerWorkPhoneView"></span></div>
                        <br>
                        <div><b>Email: </b><span id="InstallerEmailView"></span></div>
                        <br>
                    </div>
                    
                 
                  
      
                </div>
                
            </div>
            <div class="modal-footer">

                <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>
            </div>
            </div>
              </div>
        </div>

        <div id="installerAdd" style="display: none;" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel"
         aria-hidden="true" class="modal fade">
            <div class="modal-dialog modal-sm" role="document">
            <div class="modal-content">
            <div class="modal-header" style="height:80px;">
                <h4 class="modal-title" id="installerAddTitle"></h4>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                  <div>
                   <!-- Search form -->
                        <input class="form-control" type="text" placeholder="Search" aria-label="Search" id="txtName"  style="width:300px;" onkeyup="SearchByName()" />
                     
                      
                </div>
                <div  style="overflow-y:auto;margin-left:-150px;margin-top:-20px;">
                      <a  href="#" onclick="ClearSearch()"  >Clear Search</a>
                </div>
                <br />
                <div style="overflow-y:auto;height:200px;">
                    <table id="dataTableInstallerAdd" class="table table-striped table-bordered table-hover table-condensed" ></table>
                </div>
                <div id="installerAddDetail" style="display:none; ">
                    <div style="float: left; padding-left:100px;">
                       <img id="AddinstallerImg" src="" />
                    </div>

                     <div  style="text-align:left; padding-left:400px;">
                        <div><b>Name: </b><span id="InstallerNameAdd"></span></div>
                        <br>
                        <div><b>Branch: </b><span id="InstallerBranchAdd"></span></div>
                        <br>
                        <div><b>Department: </b><span id="InstallerDepartmentAdd"></span></div>
                        <br>
                        <div><b>Home Telephone: </b><span id="InstallerTelephoneAdd"></span></div>
                        <br>
                        <div><b>Work Phone Number: </b><span id="InstallerWorkPhoneAdd"></span></div>
                        <br>
                        <div><b>Email: </b><span id="InstallerEmailViewAdd"></span></div>
                        <br>
                    </div>
                    
                 
                  
      
                </div>
                
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-primary" onclick="AddInstallersToEvent();">Add Installers</button>
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
            </div>
            </div>
              </div>
        </div>

       <div id="TruckPop" style="display: none;" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel"
         aria-hidden="true" class="modal fade">
            <div class="modal-dialog modal-sm" role="document">
            <div class="modal-content">
            <div class="modal-header" style="height:80px;">
                <h4 class="modal-title" id="TruckPopTitle"></h4>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <div>
                    <table id="dataTableTruck" class="table table-striped table-bordered table-hover table-condensed"></table>
                </div>  
                <div>
                   <!-- Search form -->
                        <input class="form-control" type="text" placeholder="Search" aria-label="Search" id="txtTruckCrewName"  style="width:300px;" onkeyup="SearchByTruckCrewName()" />
                     
                      
                </div>
                <div  style="overflow-y:auto;margin-left:-150px;margin-top:-20px;">
                      <a  href="#" onclick="ClearTruckCrewSearch()" >Clear Search</a>
                </div>
                <br />
                <div style="overflow-y:auto;height:200px;">
                    <table id="dataTableTruckInstallerAdd" class="table table-striped table-bordered table-hover table-condensed" ></table>
                </div>

                
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-primary" onclick="AddCrewsToTruck();">Add Truck Crews</button>
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
            </div>
            </div>
              </div>
        </div>
        <div id="taskModal" style="display: none;" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel"
            aria-hidden="true" class="modal fade">
            <div class="modal-dialog modal-task" role="document">
                <div class="modal-content">
                    <div class="task-modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="false">&times;</span>

                        </button>
                     

                    </div>

                    <div class="modal-body">
                            <div>
                                <div><input id="AddTitle" style="text-align: left;  border-top:none;border-left:none; border-right:none;width:300px; margin-left:-200px; height:60px; font-size:larger;"  placeholder="Add Title" ></div>
                                <br />
                                <div style="margin-left:-350px;"><img src="images/timer2.png"  style="width:2%;height:2%;"/>  &nbsp;  &nbsp;  <input id="TaskTime" placeholder="Jan 15, 2020 1:30 pm"  style="border:hidden;" ></div>
                                 <br />
                                <div style="margin-left:-350px;"><img src="images/wo.png" style="width:2%;height:2%;" />  &nbsp;  &nbsp;  <input id="AddWO" placeholder="Assign to Work Order"  style="border:hidden;"  ></div>
                                 <br />
                                <div style="margin-left:-350px;"><img src="images/client.png"  style="width:2%;height:2%;"/>  &nbsp;  &nbsp;  <input id="AddClient" placeholder="Assign to Client"  style="border:hidden;" ></div>
                                 <br />
                                <div style="margin-left:-350px;"><img src="images/desc1.png" style="width:2%;height:2%;" />  &nbsp;  &nbsp;   <input id="AddDesc" placeholder="Assign to Client"  style="border:hidden;"></div>
                  
                            </div>
                            
                    </div>


                    <div class="modal-footer">
						 <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>

            <br>
        </div>
      


        <div id="openviewWeather">
            <a class="weatherwidget-io" href="https://forecast7.com/en/49d28n123d12/vancouver/" data-label_1="Vancouver" data-label_2="Weather" data-font="Roboto" data-icons="Climacons Animated" data-theme="original" data-accent="rgba(1, 1, 1, 0.0)"></a>
        </div>
        <script>
            !function (d, s, id) { var js, fjs = d.getElementsByTagName(s)[0]; if (!d.getElementById(id)) { js = d.createElement(s); js.id = id; js.src = 'https://weatherwidget.io/js/widget.min.js'; fjs.parentNode.insertBefore(js, fjs); } }(document, 'script', 'weatherwidget-io-js');
        </script>
        <br />

        <div id="typeChange" class="modal hidden">
            <div class="modal-content">
                <span id="closeTypeChange" class="close" onclick="$('#typeChange').addClass('hidden');">×</span>
                <h2>Select Type</h2>
                <p>
                    <input id="typeWindow" class="typeButton" type="submit" name="type" value="Windows" onclick="ChangeType('Windows'); return false;"><br />

                    <input id="typePaint" class="typeButton" type="submit" name="type" value="Paint" onclick="ChangeType('Paint'); return false;"><br />
                    <input id="typeShipping" class="typeButton" type="submit" name="type" value="Shipping" onclick="ChangeType('Shipping'); return false;"><br />
                    <input id="typeCustomer" class="typeButton" type="submit" name="type" value="Customer" onclick="ChangeType('Customer'); return false;"><br />
                    <input id="typeInstallation" class="typeButton" type="submit" name="type" value="Installation" onclick="ChangeType('Installation'); return false;"><br />
                    <input id="typeRemeasure" class="typeButton" type="submit" name="type" value="Remeasure" onclick="ChangeType('Remeasure'); return false;">
                </p>
            </div>
        </div>
        <div id="stateFilter" class="modal hidden">
            <div class="modal-content">
                <span id="closeStateFilter" class="close" onclick="$('#stateFilter').addClass('hidden');">×</span>
                <h2>Applied Filters:</h2>
                <div>
                    <table width="100%">
                        <tr width="100%">
                            <td width="45%"></td>
                            <td width="25%" style="text-align: left;">
                                <input type="checkbox" name="state" value="Scheduled Work Order" checked>Scheduled Work Order<br />
                                <input type="checkbox" name="state" value="Draft Work Order" checked>Draft Work Order<br />
                                <input type="checkbox" name="state" value="In-Progress" checked>In-Progress<br />
                                <input type="checkbox" name="state" value="Ready To Ship" checked>Ready To Ship<br />
                                <input type="checkbox" name="state" value="Shipped" checked>Shipped<br />
                                <input type="checkbox" name="state" value="On Hold" checked>On Hold

                            </td>
                            <td></td>
                        </tr>
                    </table>
                </div>
                <p>
                    <input id="ApplyStateChange" class="typeButton" type="submit" value="Apply" onclick="ApplyFilters('state'); return false;" />
                </p>
            </div>
        </div>

        <div id="InstallationStateFilter" class="modal hidden">
            <div class="modal-content">
                <span id="closeInstallationStateFilter" class="close" onclick="$('#InstallationStateFilter').addClass('hidden');">×</span>
                <h2>Installation Applied Filters:</h2>
                <div>
                    <table width="100%">
                        <tr width="100%">
                            <td width="45%"></td>
                            <td width="25%" style="text-align: left;">



                                   <span class="dot" style="background-color: #a5d6a7; "></span><input type="checkbox" name="InstallationState" value="Install in Progress" checked>Install in Progress
                             
                                <br />
                                <span class="dot" style="background-color: #4890e2; "> </span><input type="checkbox" name="InstallationState" value="Installation Confirmed" checked>Installation Confirmed<br />
                                <span class="dot" style="background-color: #ffc966; "> </span><input type="checkbox" name="InstallationState" value="Install Completed" checked>Install Completed<br />
                                  <span class="dot" style="background-color: #ff6347; "> </span><input type="checkbox" name="InstallationState" value="Installation inprogress rejected" checked>Installation inprogress rejected<br />
                                  <span class="dot" style="background-color: #ffc966; "> </span> <input type="checkbox" name="InstallationState" value="Installation Manager Review" checked>Installation Manager Review<br />

                                  <span class="dot" style="background-color: #ffc966; "> </span> <input type="checkbox" name="InstallationState" value="Job Completed" checked>Job Completed<br />
                                <span class="dot" style="background-color: #ffc966; "> </span>  <input type="checkbox" name="InstallationState" value="Job Costing" checked>Job Costing<br />
                                <span class="dot" style="background-color: #ff6347; "> </span>  <input type="checkbox" name="InstallationState" value="Pending Install Completion" checked>Pending Install Completion<br />

                                <span class="dot" style="background-color: #ffc966; "> </span>   <input type="checkbox" name="InstallationState" value="Ready for Invoicing" checked>Ready for Invoicing<br />


                                <span class="dot" style="background-color: #9FB6CD; "> </span> <input type="checkbox" name="InstallationState" value="ReMeasure Scheduled" checked>ReMeasure Scheduled<br />




                               <span class="dot" style="background-color: #ff6347; "> </span>   <input type="checkbox" name="InstallationState" value="Rejected Installation" checked>Rejected Installation<br />
                                 <span class="dot" style="background-color: #ff6347; "> </span>  <input type="checkbox" name="InstallationState" value="Rejected Job Costing" checked>Rejected Job Costing<br />
                                 <span class="dot" style="background-color: #ff6347; "> </span>  <input type="checkbox" name="InstallationState" value="Rejected Manager Review" checked>Rejected Manager Review<br />
                                 <span class="dot" style="background-color: #ff6347; "> </span>    <input type="checkbox" name="InstallationState" value="Rejected Remeasure" checked>Rejected Remeasure<br />
                               <span class="dot" style="background-color: #ff6347; "> </span>   <input type="checkbox" name="InstallationState" value="Rejected Scheduled Work" checked>Rejected Scheduled Work<br />



                                <span class="dot" style="background-color: #ffc966; "> </span>  <input type="checkbox" name="InstallationState" value="Unreviewed Job Costing" checked>Unreviewed Job Costing<br />

                                 <span class="dot" style="background-color: #9FB6CD; "> </span>  <input type="checkbox" name="InstallationState" value="Unreviewed Work Scheduled" checked>Unreviewed Work Scheduled<br />
                                <span class="dot" style="background-color: #ffc966; "> </span>  <input type="checkbox" name="InstallationState" value="VP Installation Approval" checked>VP Installation Approval<br />

                               <span class="dot" style="background-color: #9FB6CD; "> </span>  <input type="checkbox" name="InstallationState" value="Work Scheduled" checked>Work Scheduled
                                

                            </td>
                            <td></td>
                        </tr>
                    </table>
                </div>
                <p>
                    <input id="SelectAllState" class="typeButton" type="submit" value="SelectAll" onclick="SelectAll('InstallationState'); return false;" />

                </p>
                <p>
                    <input id="ApplyInstallationStateChange" class="typeButton" type="submit" value="Apply" onclick="ApplyFilters('InstallationState'); return false;" />
                </p>
            </div>
        </div>

        <div id="RemeasureStateFilter" class="modal hidden">
            <div class="modal-content">
                <span id="closeRemeasureStateFilter" class="close" onclick="$('#RemeasureStateFilter').addClass('hidden');">×</span>
                <h2>Remeasure Applied Filters:</h2>
                <div>
                    <table width="100%">
                        <tr width="100%">
                            <td width="45%"></td>
                            <td width="25%" style="text-align: left;">


                          <%--      <input type="checkbox" name="RemeasureState" value="Allocated Work Order" checked>Allocated Work Order<br />
                                <input type="checkbox" name="RemeasureState" value="Cancelled" checked>Cancelled<br />--%>

                              <span class="dot" style="background-color: #4890e2; "> </span>  <input type="checkbox" name="RemeasureState" value="Installation Confirmed" checked>Installation Confirmed<br />
                                  <span class="dot" style="background-color: #a5d6a7; "></span><input type="checkbox" name="RemeasureState" value="Install in Progress" checked>Install in Progress<br />
                             <span class="dot" style="background-color: #ffc966; "> </span>   <input type="checkbox" name="RemeasureState" value="Install Completed" checked>Install Completed<br />

                              <span class="dot" style="background-color: #ff6347; "> </span>    <input type="checkbox" name="RemeasureState" value="Installation inprogress rejected" checked>Installation inprogress rejected<br />
                               <span class="dot" style="background-color: #ffc966; "> </span>      <input type="checkbox" name="RemeasureState" value="Installation Manager Review" checked>Installation Manager Review<br />

                                <span class="dot" style="background-color: #ffc966; "> </span>   <input type="checkbox" name="RemeasureState" value="Job Completed" checked>Job Completed<br />
                             <span class="dot" style="background-color: #ffc966; "> </span>    <input type="checkbox" name="RemeasureState" value="Job Costing" checked>Job Costing<br />
                            <span class="dot" style="background-color: #ff6347; "> </span>     <input type="checkbox" name="RemeasureState" value="Pending Install Completion" checked>Pending Install Completion<br />

                             <span class="dot" style="background-color: #ffc966; "> </span>      <input type="checkbox" name="RemeasureState" value="Ready for Invoicing" checked>Ready for Invoicing<br />
                            <span class="dot" style="background-color: #9FB6CD; "> </span>    <input type="checkbox" name="RemeasureState" value="Ready for ReMeasure" checked>Ready for ReMeasure<br />

                          <span class="dot" style="background-color: #9FB6CD; "> </span>             <input type="checkbox" name="RemeasureState" value="ReMeasure Scheduled" checked>ReMeasure Scheduled<br />
                      <span class="dot" style="background-color: #ff6347; "> </span>  <input type="checkbox" name="RemeasureState" value="Rejected Installation" checked>Rejected Installation<br />
                           <span class="dot" style="background-color: #ff6347; "> </span>       <input type="checkbox" name="RemeasureState" value="Rejected Job Costing" checked>Rejected Job Costing<br />
                            <span class="dot" style="background-color: #ff6347; "> </span>         <input type="checkbox" name="RemeasureState" value="Rejected Manager Review" checked>Rejected Manager Review<br />
                            <span class="dot" style="background-color: #ff6347; "> </span>      <input type="checkbox" name="RemeasureState" value="Rejected New Work Order" checked>Rejected New Work Order<br />

                             <span class="dot" style="background-color: #ff6347; "> </span>     <input type="checkbox" name="RemeasureState" value="Rejected Remeasure" checked>Rejected Remeasure<br />
                          

                                <span class="dot" style="background-color: #ff6347; "> </span>                <input type="checkbox" name="RemeasureState" value="Rejected Scheduled Work" checked>Rejected Scheduled Work<br />
                                

<%--                                <input type="checkbox" name="RemeasureState" value="Sold" checked>Sold<br />

                                 <input type="checkbox" name="RemeasureState" value="System Duplicated" checked>System Duplicated<br />
                              
                                     <input type="checkbox" name="RemeasureState" value="Unreviewed Job Costing" checked>Unreviewed Job Costing<br />--%>
                 
          <%--                      <input type="checkbox" name="RemeasureState" value="Unreviewed Buffered Work" checked>Unreviewed Buffered Work<br />--%>
                                <span class="dot" style="background-color: #ffc966; "> </span>  <input type="checkbox" name="RemeasureState" value="Unreviewed Job Costing" checked>Unreviewed Job Costing<br />
                                <span class="dot" style="background-color: #ffc966; "> </span>            <input type="checkbox" name="RemeasureState" value="VP Installation Approval" checked>VP Installation Approval<br />
                             <span class="dot" style="background-color: #9FB6CD; "> </span>     <input type="checkbox" name="RemeasureState" value="Unreviewed Work Scheduled" checked>Unreviewed Work Scheduled<br />
                     

                            <span class="dot" style="background-color: #9FB6CD; "> </span>     <input type="checkbox" name="RemeasureState" value="Work Scheduled" checked>Work Scheduled
                                

                            </td>
                            <td></td>
                        </tr>
                    </table>
                </div>
                <p>
                    <input id="SelectAllRemeasureState" class="typeButton" type="submit" value="SelectAll" onclick="SelectAll('RemeasureState'); return false;" />

                </p>
                <p>
                    <input id="ApplyRemeasureStateChange" class="typeButton" type="submit" value="Apply" onclick="ApplyFilters('RemeasureState'); return false;" />
                </p>
            </div>
        </div>
        <div id="JobTypeFilter" class="modal hidden">
            <div class="modal-content">
                <span id="closeJobTypeFilter" class="close" onclick="$('#JobTypeFilter').addClass('hidden');">×</span>
                <h2>Job Types:</h2>
                <div>
                    <table width="100%">
                        <tr width="100%">
                            <td width="45%"></td>
                            <td width="25%" style="text-align: left;">
                                <input type="checkbox" name="jobType" value="" checked>Missing Data<br />
                                <input type="checkbox" name="jobType" value="SO" checked>Supply Only<br />
                                <input type="checkbox" name="jobType" value="SI" checked>Supply & Install<br />
                                <input type="checkbox" name="jobType" value="RES" checked>Reservation<br />
                                <input type="checkbox" name="jobType" value="PendingRES" checked>Pending Reservation
                            </td>
                            <td></td>
                        </tr>
                    </table>
                </div>
                <p>
                    <input id="ApplyJobTypeChange" class="typeButton" type="submit" value="Apply" onclick="ApplyFilters('JobType'); return false;" />
                </p>
            </div>
        </div>
        <div id="ShippingTypeFilter" class="modal hidden">
            <div class="modal-content">
                <span id="closeShippingTypeFilter" class="close" onclick="$('#ShippingTypeFilter').addClass('hidden');">×</span>
                <h2>Shipping Types:</h2>
                <div>
                    <table width="100%">
                        <tr width="100%">
                            <td width="45%"></td>
                            <td width="25%" style="text-align: left;">
                                <input type="checkbox" name="ShippingType" value="Select One" checked>Missing Data<br />
                                <input type="checkbox" name="ShippingType" value="PickUp" checked>Pick Up<br />
                                <input type="checkbox" name="ShippingType" value="Delivery" checked>Delivery
                            </td>
                            <td></td>
                        </tr>
                    </table>
                </div>
                <p>
                    <input id="ApplyShippingTypeChange" class="typeButton" type="submit" value="Apply" onclick="ApplyFilters('ShippingType'); return false;" />
                </p>
            </div>
        </div>
        <div id="BranchFilter" class="modal hidden">
            <div class="modal-content">
                <span id="closeBranchFilter" class="close" onclick="$('#BranchFilter').addClass('hidden');">×</span>
                <h2>Branch:</h2>
                <div>
                    <table width="100%">
                        <tr width="100%">
                            <td width="45%"></td>
                            <td width="25%" style="text-align: left;">
                                <%=branchHtml %>
                                
                            </td>
                            <td></td>
                        </tr>
                    </table>
                </div>
                <p>
                    <input id="SelectAllBranch" class="typeButton" type="submit" value="SelectAll" onclick="SelectAll('branch'); return false;" />

                </p>
                <p>
                    <input id="ApplyBranchChange" class="typeButton" type="submit" value="Apply" onclick="ApplyFilters('Branch'); return false;" />
                </p>
            </div>

        </div>
        <div id="JobStaffFilter" class="modal hidden">
                      <div class="modal-content">'
                            <div class="dropdown">
                                  <button type="button" class="btn btn-primary dropdown-toggle" data-toggle="dropdown">
                                    Dropdown button
                                  </button>
                                      <div class="dropdown-menu">
                                        <a class="dropdown-item" href="#">Link 1</a>
                                        <a class="dropdown-item" href="#">Link 2</a>
                                        <a class="dropdown-item" href="#">Link 3</a>
                                      </div>
                            </div>
                    </div>
        </div>
        <div id='wrap'>

            <div id='external-events'>
                <h4>New Work Orders</h4>
            </div>
            <div id='external-events1'>
                <h4>New Work Orders</h4>
            </div>
             <div id='external-eventsRemeasure'>
                <h4>New Work Orders</h4>
            </div>

            <div id='calendar'></div>

            <div style='clear: both'></div>

        </div>


        <div id="updatedialog" style="font: 70% 'Trebuchet MS', sans-serif; margin: 50px; display: none;" title="Update Event">
            <table cellpadding="0" class="style1">
                <tr>
                    <td class="alignRight">Name:</td>
                    <td class="alignLeft">
                        <input id="eventName" type="text" /><br />
                    </td>
                </tr>
                <tr>
                    <td class="alignRight">Description:</td>
                    <td class="alignLeft">
                        <textarea id="eventDesc" cols="30" rows="3"></textarea></td>
                </tr>
                <tr>
                    <td class="alignRight">Start:</td>
                    <td class="alignLeft">
                        <span id="eventStart"></span></td>
                </tr>
                <tr>
                    <td class="alignRight">End: </td>
                    <td class="alignLeft">
                        <span id="eventEnd"></span>
                        <input type="hidden" id="eventId" /></td>
                </tr>
            </table>
        </div>
        <div id="addDialog" style="font: 70% 'Trebuchet MS', sans-serif; margin: 50px; display: none;" title="Add Event">
            <table cellpadding="0" class="style1">
                <tr>
                    <td class="alignRight">Name:</td>
                    <td class="alignLeft">
                        <input id="addEventName" type="text" size="50" /><br />
                    </td>
                </tr>
                <tr>
                    <td class="alignRight">Description:</td>
                    <td class="alignLeft">
                        <textarea id="addEventDesc" cols="30" rows="3"></textarea></td>
                </tr>
                <tr>
                    <td class="alignRight">Start:</td>
                    <td class="alignLeft">
                        <span id="addEventStartDate"></span></td>
                </tr>
                <tr>
                    <td class="alignRight">End:</td>
                    <td class="alignLeft">
                        <span id="addEventEndDate"></span></td>
                </tr>
            </table>

        </div>
        <div>
        
        </div>
    </form>

</body>
</html>

