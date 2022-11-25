using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace WebService
{
    public static class ConvertHelper<T> where T : new()
    {
        public static IList<T> dataTableToList(DataTable dt)
        {
            IList<T> ls = new List<T>();

            foreach (DataRow dr in dt.Rows)
            {
                var t = new T();

                var properties = t.GetType().GetProperties();
                foreach (PropertyInfo pi in properties)
                {
                    var tempName = pi.Name;

                    if (dt.Columns.Contains(tempName))
                    {
                        if (!pi.CanWrite) continue;

                        var value = dr[tempName];
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                ls.Add(t);
            }

            return ls;
        }
    }
}