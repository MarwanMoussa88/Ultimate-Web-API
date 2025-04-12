using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class Extensions
    {
        public static List<T> MapToList<T>(this DbDataReader dr)
        {
            var objList = new List<T>();
            var props = typeof(T).GetRuntimeProperties();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    T obj = Activator.CreateInstance<T>();
                    foreach (var prop in props)
                    {
                        var ordinal = dr.GetOrdinal(prop.Name);
                        var val = dr.GetValue(ordinal);
                        prop.SetValue(obj, val == DBNull.Value ? null : val);
                    }
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static DataTable MapToDataTable<T>(this IEnumerable<T> list)
        {
            DataTable dt = new DataTable();
            IEnumerable<PropertyInfo> props = typeof(T).GetProperties();


            foreach (PropertyInfo property in props)
            {
                dt.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            foreach (T item in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo property in props)
                {
                    row[property.Name] = property.GetValue(item) ?? DBNull.Value;
                }
                dt.Rows.Add(row);
            }

            return dt;
        }

        public static async Task<string> GetBody(this Stream body)
        {
            body.Position = 0;
            using StreamReader streamReader = new StreamReader(body, Encoding.UTF8, leaveOpen: true, bufferSize: 8192);

            return await streamReader.ReadToEndAsync();
        }

    }
}
