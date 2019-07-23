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
    <link href='css/application.css' rel='stylesheet' />

    <link href='js/ekko-lightbox.css' rel='stylesheet' />

    
    <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">




    <link href='js/fullcalendar.print.css' rel='stylesheet' media='print' />


    <!-- Include all compiled plugins (below), or include individual files as needed -->
    <script src="js/bootstrap.min.js"></script>

    <script src='lib/moment.min.js'></script>
    <script src='lib/jquery-2.1.4.min.js'></script>
    <script src='lib/jquery-ui.custom.min.js'></script>
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
    <script src='js/fullcalendar.js'></script>
    <script src="js/calendarprocessing.js"></script>
    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAhsQnBPh07vYae9Oakwczkyv8gTDY9j-U"></script>

    <script src="js/ekko-lightbox.js"></script>
    <script src="js/ekko-lightbox.min.js"></script>

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
        var map, mapRemeasure;
        $(function () {
            // $("#from_date").datepicker();
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

            var marker = new google.maps.Marker({
                position: latlng,
                map: map,
                title: "location"
            });
        }

        function codeAddress() {
            var address = document.getElementById('Address').innerHTML;
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
        }

        #external-events, #external-events1,#external-eventsRemeasure{
            float: left;
            width: 200px;
            padding: 0 10px;
            border: 1px solid #ccc;
            background: #eee;
            text-align: left;
            overflow-y: auto;
            height: 900px;
        }

            #external-events, #external-events1,#external-eventsRemeasure h4 {
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


        .modal-header {
            background-color: #9FB6CD;
            color: white;
            font-weight: 400;
        }

        .modal-ku {
            width: 1600px;
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
                    <div class="modal-header ">
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
                                    <a href="#SubTradesTab" aria-controls="SubTradesTab" role="tab" data-toggle="tab">SUB TDADES REQUIRED</a>
                                </li>
                                <li role="presentation">
                                    <a href="#JobAnalysisTab" aria-controls="JobAnalysisTab" role="tab" data-toggle="tab">JOB ANALYSIS</a>

                                </li>
                                <li role="presentation">
                                    <a href="#CalledLogTab" aria-controls="CalledLogTab" role="tab" data-toggle="tab">Called Log</a>

                                </li>
                                 <li role="presentation">
                                    <a href="#WOPictureTab" aria-controls="WOPictureTab" role="tab" data-toggle="tab">Photo Gallery</a>
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
                                    <div id="installationContent" style="width: 600px; margin-left: 20px; padding-left: 100px;">
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
                                        <div><b>CrewNames: </b><span id="CrewNames"></span></div>
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
                                                        <input type="radio" name="Wood-DropOff-Jobs" id="Wood-DropOff-JobsYes">Yes</label>
                                                </div>
                                                <div class="radio">
                                                    <label>
                                                        <input type="radio" name="Wood-DropOff-Jobs" id="Wood-DropOff-JobsNo">No</label>
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

                                        <span><b>Weekends Setting: </b></span>
                                        <div style="width: 800px;" class="container">
                                            <div class="leftcolumn">
                                                Saturday:
                                                <input type="checkbox" name="saturday">
                                            </div>
                                            <div class="rightcolumn">
                                                Sunday: &nbsp;&nbsp;<input type="checkbox" name="sunday">
                                            </div>
                                        </div>
                                        <br>

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
                                        <input type="button" name="btnSave" id="btnSave" class="btn btn-success" value="Save" onclick="UpdateInstallationEvents()">
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

