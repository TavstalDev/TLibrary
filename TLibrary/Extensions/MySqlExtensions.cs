using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Extensions
{
    public static class MySqlExtensions
    {
        public static T GetJsonObject<T>(this MySqlDataReader reader, string column)
        {
            return JsonConvert.DeserializeObject<T>(reader.GetString(column));
        }

        public static List<T> GetJsonArray<T>(this MySqlDataReader reader, string column)
        {
            return JsonConvert.DeserializeObject<List<T>>(reader.GetString(column));
        }

        public static float GetFloatSafe(this MySqlDataReader reader, string column)
        {
            return Convert.ToSingle(reader.GetString(column).Replace(".", ","));
        }

        public static decimal GetDecimalSafe(this MySqlDataReader reader, string column)
        {
            return Convert.ToDecimal(reader.GetString(column).Replace(".", ","));
        }

        public static double GetDoubleSafe(this MySqlDataReader reader, string column)
        {
            return Convert.ToDouble(reader.GetString(column).Replace(".", ","));
        }
    }
}
