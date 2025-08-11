using System;
using System.Data.SqlClient;

namespace Scoop
{
    public class LearnerWriter
    {
        public void Register(Guid id, string email)
        {
            var db = new DatabaseHelper();

            using (SqlConnection connection = new SqlConnection(db.ConnectionString))
            {
                connection.Open();

                var query = @"
MERGE scoop.ScoLearner AS target
USING (SELECT @LearnerEmail AS LearnerEmail, @LearnerId as LearnerId) AS source
ON target.LearnerId = source.LearnerId
WHEN MATCHED THEN
    UPDATE SET LastAuthenticated = GETUTCDATE()
WHEN NOT MATCHED THEN
    INSERT (LearnerId, LearnerEmail, LastAuthenticated)
    VALUES (source.LearnerId, source.LearnerEmail, GETUTCDATE());
";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@LearnerEmail", email);
                    cmd.Parameters.AddWithValue("@LearnerId", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}