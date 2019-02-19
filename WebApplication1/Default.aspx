﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CalendarSystem._Default" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <link href='js/fullcalendar.css' rel='stylesheet' />
    <link href='css/application.css' rel='stylesheet' />

    <link href='js/fullcalendar.print.css' rel='stylesheet' media='print' />
    <script src='lib/moment.min.js'></script>
    <script src='lib/jquery-2.1.4.min.js'></script>
    <script src='lib/jquery-ui.custom.min.js'></script>
    <script src='js/fullcalendar.js'></script>
    <script src="js/calendarprocessing.js"></script>
    <script>
        var readonly = "<%= ReadOnly %>";
    </script>
    <style>
        body {
            margin-top: 40px;
            text-align: center;
            font-size: 14px;
            font-family: "Lucida Grande",Helvetica,Arial,Verdana,sans-serif;
        }

        #wrap {
            width: 100%;
            margin: 0 auto;
        }

        #external-events {
            float: left;
            width: 150px;
            padding: 0 10px;
            border: 1px solid #ccc;
            background: #eee;
            text-align: left;
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
    </style>
</head>
<body>
    <form id="main" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
        </asp:ScriptManager>
        <div id="typeChange" class="modal hidden">
            <div class="modal-content">
                <span id="closeTypeChange" class="close" onclick="$('#typeChange').addClass('hidden');">×</span>
                <h2>Select Type</h2>
                <p>
                    <input id="typeWindow" class="typeButton" type="submit" name="type" value="Windows" onclick="ChangeType('Windows'); return false;"><br />
                    <input id="typeDoor" class="typeButton" type="submit" name="type" value="Doors" onclick="ChangeType('Doors'); return false;"><br />
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

                                <input type="checkbox" name="Work Scheduled" value="Work Scheduled" checked>Work Scheduled
                                

                            </td>
                            <td></td>
                        </tr>
                    </table>
                </div>
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

