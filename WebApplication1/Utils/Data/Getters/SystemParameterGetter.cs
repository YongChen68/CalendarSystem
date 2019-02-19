using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CalendarSystem.Utils.Data.Getters
{
    public class TestSystemParameterGetter : ISystemParameterGetter
    {
        public List<Generics.Utils.Data.GlobalValues> GetGlobalValues(string type, DateTime startDate, DateTime endDate)
        {
            var retValue = new List<Generics.Utils.Data.GlobalValues>();
            for (int i = 0; i < (endDate - startDate).Days; i++)
                retValue.Add(new Generics.Utils.Data.GlobalValues() { Date = DateTime.Now.AddDays(i), Name = "max", Value = "10000" });
            return retValue;
        }
    }
    public class SystemParameterGetter : ISystemParameterGetter
    {
        public List<Generics.Utils.Data.GlobalValues> GetGlobalValues(string type, DateTime startDate, DateTime endDate)
        {
            string SQL = @"select 'max' as Name, ProductionDate as Date, AvailableManPower as Value from ManPower m with(nolock,noexpand) where m.productionDate >= @pStart and m.ProductionDate <= @pEnd
union all
select 'manpower' as Name, ProductionDate as Date, AvailableStaff as Value from ManPower m with(nolock,noexpand) where m.productionDate >= @pStart and m.ProductionDate <= @pEnd";
            List<System.Data.SqlClient.SqlParameter> pars = new List<System.Data.SqlClient.SqlParameter>();
            pars.Add(new System.Data.SqlClient.SqlParameter("pStart", startDate));
            pars.Add(new System.Data.SqlClient.SqlParameter("pEnd", endDate));
            Lift.LiftManager.Logger.Write(this.GetType().Name, "About to execute: {0}", SQL);
            return Lift.LiftManager.DbHelper.ReadObjects<Generics.Utils.Data.GlobalValues>(SQL, pars.ToArray());
        }
    }
}