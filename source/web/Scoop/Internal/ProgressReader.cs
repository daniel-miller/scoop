using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Scoop
{
    public class ProgressReader
    {
        public Dictionary<string, string> Load(int package, Guid learner)
        {
            var db = new DatabaseHelper();

            var data = new Dictionary<string, string>();

            using (SqlConnection conn = new SqlConnection(db.ConnectionString))
            {
                conn.Open();
                string query = @"SELECT ProgressKey, ProgressValue FROM scoop.ScoProgress 
                           WHERE PackageNumber = @PackageNumber AND LearnerId = @LearnerId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PackageNumber", package);
                    cmd.Parameters.AddWithValue("@LearnerId", learner);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data[reader["ProgressKey"].ToString()] = reader["ProgressValue"].ToString();
                        }
                    }
                }
            }

            return data;
        }
    }
}