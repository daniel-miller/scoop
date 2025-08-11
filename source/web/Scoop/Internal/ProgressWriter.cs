using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Scoop
{
    public class ProgressWriter
    {
        public bool Save(int package, Guid learner, Dictionary<string, string> data)
        {
            var db = new DatabaseHelper();

            using (SqlConnection conn = new SqlConnection(db.ConnectionString))
            {
                conn.Open();

                // Use transaction for batch insert/update
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (KeyValuePair<string, string> kvp in data)
                        {
                            // Skip empty or null keys
                            if (string.IsNullOrEmpty(kvp.Key))
                                continue;

                            string mergeQuery = @"
                            INSERT INTO scoop.ScoEvent (EventTime, PackageNumber, LearnerId, ProgressKey, ProgressValue) VALUES (getutcdate(), @PackageNumber, @LearnerId, @ProgressKey, @ProgressValue);
                            MERGE scoop.ScoProgress AS target
                            USING (SELECT @PackageNumber AS PackageNumber, @LearnerId AS LearnerId, @ProgressKey AS ProgressKey, @ProgressValue AS ProgressValue) AS source
                            ON target.PackageNumber = source.PackageNumber AND target.LearnerId = source.LearnerId AND target.ProgressKey = source.ProgressKey
                            WHEN MATCHED THEN
                                UPDATE SET ProgressValue = source.ProgressValue, ProgressModified = getutcdate()
                            WHEN NOT MATCHED THEN
                                INSERT (PackageNumber, LearnerId, ProgressKey, ProgressValue, ProgressModified)
                                VALUES (source.PackageNumber, source.LearnerId, source.ProgressKey, source.ProgressValue, getutcdate());";

                            using (SqlCommand cmd = new SqlCommand(mergeQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@PackageNumber", package);
                                cmd.Parameters.AddWithValue("@LearnerId", learner);
                                cmd.Parameters.AddWithValue("@ProgressKey", kvp.Key);
                                cmd.Parameters.AddWithValue("@ProgressValue", kvp.Value ?? string.Empty);

                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}