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
        $(function () {
            $("#from_date").datepicker();
            $("#end_date").datepicker();
        });
    </script>
    <style>
        body {
            margin-top: 40px;
            text-align: center;
            font-size: 14px;
            font-family: "Lucida Grande",Helvetica,Arial,Verdana,sans-serif;
        }


        #map {
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

        #external-events {
            float: left;
            width: 200px;
            padding: 0 10px;
            border: 1px solid #ccc;
            background: #eee;
            text-align: left;
            overflow-y: auto;
            height: 900px;
        }

            #external-events h4 {
                font-size: 16px;
                margin-top: 0;
                padding-top: 1em;
            }

            #external-events .fc-event {
                margin: 10px 0;
                cursor: pointer;
            }

            #external-events p {
                margin: 1.5em 0;
                font-size: 11px;
                color: #666;
            }

                #external-events p input {
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

        .modal-header {
            background-color: #9FB6CD;
            color: white;
            font-weight: 400;
        }

        .modal-ku {
            width: 1400px;
            margin: auto;
        }

        .close {
            color: #fff;
            opacity: 1;
        }

        .table-condensed {
        }
    </style>
</head>
<body>
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
                                    <a href="#ProductTab" aria-controls="productTab" role="tab" data-toggle="tab">PRODUCT</a>

                                </li>
                                <li role="presentation">
                                    <a href="#JobAnalysisTab" aria-controls="JobAnalysisTab" role="tab" data-toggle="tab">JOB ANALYSIS</a>

                                </li>

                            </ul>
                            <!-- Tab panes -->
                            <div class="tab-content">
                                <div role="tabpanel" class="tab-pane active" id="CustomerTab">
                                    <div id="map"></div>
                                    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyBspoEtc4kjoRkvnuVk0bnV9G3NUb1N8Kk"></script>
                                    <script>
                                        var locationRio = { lat: 49.181520, lng: -122.664260 };
                                        var map = new google.maps.Map(document.getElementById('map'), {
                                            zoom: 16,
                                            center: locationRio,
                                            gestureHandling: 'cooperative'
                                        });
                                        var marker = new google.maps.Marker({
                                            position: locationRio,
                                            map: map

                                        });
                                    </script>
                                    <div id="content">
                                        <br>
                                        <br>
                                        <div><b>Customer: </b><span id="Customer"></span></div>
                                        <br>
                                        <div><b>Work Order: </b><span id="workOrder"></span></div>
                                        <br>
                                        <div><b>Home Phone: </b><span id="homePhone"></span></div>
                                        <br>
                                        <div><b>Cell Phone: </b><span id="cellPhone"></span></div>
                                        <br>
                                        <div><b>Branch: </b><span id="branch"></span></div>
                                        <br>
                                        <div><b>Address: </b><span id="Address"></span></div>
                                                      <br>
                                        <div><b>Total Windows: </b><span id="TotalWindows1"></span></div>

                                         <br>
                                        <div><b>Total Doors: </b><span id="TotalDoors1"></span></div>
                                        <br>
                                        <div><b>SalesAmount: </b><span id="SalesAmmount"></span></div>
                          
                                        <br>
                                    </div>
                                </div>
                                <div role="tabpanel" class="tab-pane" id="InstallTab">
                                    <div id="installationContent" style="width:600px; margin-left:40px;">
                                        <br>
                                        <br>

                                        <div>
                                            <b>Installation Scheduled Date </b>
                                            <div>
                                           &nbsp;&nbsp; &nbsp;&nbsp;   &nbsp;&nbsp; &nbsp;&nbsp;      &nbsp;&nbsp; &nbsp;&nbsp;   &nbsp;&nbsp; &nbsp;&nbsp;  From:  <span id="InstallScheduledStartDate"></span>
                                            </div>
                                            <div>
                                                &nbsp;&nbsp; &nbsp;&nbsp;    &nbsp;&nbsp; &nbsp;&nbsp;  To:  <span id="InstallScheduledEndDate"></span>
                                            </div>


                                        </div>

                                        <br>
                                        <div><b>Senior Installer: </b><span id="SeniorInstaller"></span></div>
                                        <br>
                                        <div><b>CrewNames: </b><span id="CrewNames"></span></div>
                                        <br>

                                        <div><b>Asbestos-Jobs: </b><span id="Asbestos-Jobs1"></span></div>

                                         <br>
                                        <div><b>Wood-DropOff-Jobs: </b><span id="Wood-DropOff-Jobs1"></span></div>

                                          <br>
                                        <div><b>HighRisk-Jobs: </b><span id="HighRisk-Jobs1"></span></div>


                                        <div style="background-color: #D3D3D3">
                                            <div>
                                                <b>Saturday: </b>
                                                <input type="checkbox" name="saturday">
                                                &nbsp;&nbsp;
                                            </div>
                                            <br>
                                            <div>
                                                <b>Sunday: </b>&nbsp;&nbsp;<input type="checkbox" name="sunday">
                                                &nbsp;&nbsp;&nbsp;&nbsp;
                                                <input type="button" name="btnSunday" id="btnSunday" style="text-decoration-line: underline; border-style: none;" value="Update" onclick="UpdateEventWeekends()">
                                            </div>
                                        </div>
                                        <br>

                                        <div class="form-group" style="background-color: #D3D3D3" id="ReturnedJob">
                                            <b>Return Scheduled Date:</b>
                                            <div>
                                                From: 
                                                <input id="from_date" style="width: 160px; text-align: center;" class="form-control" data-toggle="tooltip" title="Start Date">
                                                To:   &nbsp;&nbsp;<input id="end_date" style="width: 160px; text-align: center;" class="form-control" data-toggle="tooltip" title="End Date">
                                                &nbsp;&nbsp;<input type="button" name="btnReturnedJob" id="btnReturnedJob" style="text-decoration-line: underline; border-style: none;" value="Save" onclick="UpdateReturnedJobSchedule()">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div role="tabpanel" class="tab-pane " id="ProductTab">
                                    <div style="overflow: auto; height: 600px;">
                                        <table id="dataTable" class="table table-striped table-bordered table-hover table-condensed"></table>
                                    </div>

                                </div>

                                <div role="tabpanel" class="tab-pane " id="JobAnalysisTab">
                                    <div >
                                         
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
                    <input id="typeInstallation" class="typeButton" type="submit" name="type" value="Installation" onclick="ChangeType('Installation'); return false;">
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



                                <input type="checkbox" name="InstallationState" value="Install in Progress" checked>Install in Progress<br />
                                <input type="checkbox" name="InstallationState" value="Installation Confirmed" checked>Installation Confirmed<br />
                                <input type="checkbox" name="InstallationState" value="Install Completed" checked>Install Completed<br />
                                <input type="checkbox" name="InstallationState" value="Installation inprogress rejected" checked>Installation inprogress rejected<br />
                                <input type="checkbox" name="InstallationState" value="Installation Manager Review" checked>Installation Manager Review<br />

                                <input type="checkbox" name="InstallationState" value="Job Completed" checked>Job Completed<br />
                                <input type="checkbox" name="InstallationState" value="Job Costing" checked>Job Costing<br />
                                <input type="checkbox" name="InstallationState" value="Pending Install Completion" checked>Pending Install Completion<br />

                                <input type="checkbox" name="InstallationState" value="Ready for Invoicing" checked>Ready for Invoicing<br />


                                <input type="checkbox" name="InstallationState" value="ReMeasure Scheduled" checked>ReMeasure Scheduled<br />




                                <input type="checkbox" name="InstallationState" value="Rejected Installation" checked>Rejected Installation<br />
                                <input type="checkbox" name="InstallationState" value="Rejected Job Costing" checked>Rejected Job Costing<br />
                                <input type="checkbox" name="InstallationState" value="Rejected Manager Review" checked>Rejected Manager Review<br />
                                <input type="checkbox" name="InstallationState" value="Rejected Remeasure" checked>Rejected Remeasure<br />
                                <input type="checkbox" name="InstallationState" value="Rejected Scheduled Work" checked>Rejected Scheduled Work<br />



                                <input type="checkbox" name="InstallationState" value="Unreviewed Job Costing" checked>Unreviewed Job Costing<br />

                                <input type="checkbox" name="InstallationState" value="Unreviewed Work Scheduled" checked>Unreviewed Work Scheduled<br />
                                <input type="checkbox" name="InstallationState" value="VP Installation Approval" checked>VP Installation Approval<br />

                                <input type="checkbox" name="InstallationState" value="Work Scheduled" checked>Work Scheduled
                                

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

        <div id='wrap'>

            <div id='external-events'>
                <h4>New Work Orders</h4>
            </div>

            <div id='calendar'></div>

            <div style='clear: both'></div>

        </div>

        <div id="updatedialog" style="font: 70% 'Trebuchet MS', sans-serif; margin: 50px; display: none;"
            title="Update Event">
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
    </form>
</body>
</html>

