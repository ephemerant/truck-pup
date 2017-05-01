using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Food_Truck_Web
{
    public static class Database
    {
        // Execute a simple non-query command with optional parameters, returing the number of rows affected
        public static int ExecuteNonQuery(string command, Dictionary<string, object> parameters = null)
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["MainServer"].ConnectionString))
            {
                cn.Open();

                var cmd = new SqlCommand(command, cn);

                if (parameters != null)
                    foreach (var pair in parameters)
                        cmd.Parameters.AddWithValue(pair.Key, pair.Value);

                return cmd.ExecuteNonQuery();
            }
        }

        // Execute and return a scalar command
        public static object ExecuteScalar(string command, Dictionary<string, object> parameters = null)
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["MainServer"].ConnectionString))
            {
                cn.Open();

                var cmd = new SqlCommand(command, cn);

                if (parameters != null)
                    foreach (var pair in parameters)
                        cmd.Parameters.AddWithValue(pair.Key, pair.Value);

                return cmd.ExecuteScalar();
            }
        }

        // Get a list of values in the first column of each row
        public static List<object> GetList(string command, Dictionary<string, object> parameters = null)
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["MainServer"].ConnectionString))
            {
                cn.Open();

                var cmd = new SqlCommand(command, cn);

                if (parameters != null)
                    foreach (var pair in parameters)
                        cmd.Parameters.AddWithValue(pair.Key, pair.Value);

                var result = new List<object>();

                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        result.Add(reader[0]);

                return result;
            }
        }

        // Get a dictionary of columns and their values for a single row
        public static Dictionary<string, object> GetDictionary(string command, Dictionary<string, object> parameters = null)
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["MainServer"].ConnectionString))
            {
                cn.Open();

                var cmd = new SqlCommand(command, cn);

                if (parameters != null)
                    foreach (var pair in parameters)
                        cmd.Parameters.AddWithValue(pair.Key, pair.Value);

                var result = new Dictionary<string, object>();

                using (var reader = cmd.ExecuteReader())
                    if (reader.Read())
                        foreach (var column in Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList())
                            result.Add(column, reader[column]);

                return result;
            }
        }

        // Get a list of dictionaries of columns and their values for all rows
        public static List<Dictionary<string, object>> GetDictionaries(string command, Dictionary<string, object> parameters = null)
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["MainServer"].ConnectionString))
            {
                cn.Open();

                var cmd = new SqlCommand(command, cn);

                if (parameters != null)
                    foreach (var pair in parameters)
                        cmd.Parameters.AddWithValue(pair.Key, pair.Value);

                var result = new List<Dictionary<string, object>>();

                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        var obj = new Dictionary<string, object>();

                        foreach (var column in Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList())
                            obj.Add(column, reader[column]);

                        result.Add(obj);
                    }

                return result;
            }
        }
    }
}