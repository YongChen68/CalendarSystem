using System;
using System.Collections.Generic;
using Generics.Utils.Data;

namespace CalendarSystem.Utils.Data.Getters
{
    public interface ISystemParameterGetter
    {
        List<GlobalValues> GetGlobalValues(string type, DateTime start, DateTime end);
    }
}