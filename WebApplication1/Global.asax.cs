using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace CalendarSystem
{
    public class Global : System.Web.HttpApplication
    {

        private static int SessionCount = 0;
        protected void Application_Start(object sender, EventArgs e)
        {
            Lift.LiftManager.Initialize(new Lift.ConfigParameter[] { 
                new Lift.ConfigParameter() { Name = "OnBehlfOf", Mandatory = true, Description = "Account name for the user that will be used to update records in Flowfinity Actions" } ,
                new Lift.ConfigParameter() { Name = "ReadOnly", DefaultValue = "true" }
            },
                Lift.LiftCapability.Database, Lift.LiftCapability.IntegrationInterface);
            Lift.LiftManager.Logger.Write("Init", "Starting");  
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Lift.LiftManager.Logger.Write("Init", "Starting New Session: ", ++SessionCount); 
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Lift.LiftManager.Logger.Write("Init", "Unexpected error occured."); 
        }

        protected void Session_End(object sender, EventArgs e)
        {
            Lift.LiftManager.Logger.Write("Init", "Shuting Down Session: ", --SessionCount); 
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Lift.LiftManager.Logger.Write("Init", "Shutting Down");
            try
            {
                Lift.LiftManager.DeInitialize();
            }
            catch (Exception)
            { // ignore
            }
        }
    }
}