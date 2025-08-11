using System.Data.SqlClient;

namespace Scoop
{

    public class ReportReader
    {
        public PlatformStatistics RetrievePlatformStatistics()
        {
            var statistics = new PlatformStatistics();

            var db = new DatabaseHelper();

            var sql = @"
SELECT (SELECT COUNT(*) FROM scoop.ScoEvent AS E)            AS EventCount,
       (SELECT COUNT(*) FROM scoop.ScoLearner AS L)          AS LearnerCount,
       (SELECT COUNT(*) FROM scoop.ScoPackage AS P)          AS PackageCount,
       (SELECT COUNT(*) AS UniqueCount
        FROM (SELECT DISTINCT R.PackageNumber, R.LearnerId
              FROM scoop.ScoProgress AS R) AS DistinctPairs) AS RegistrationCount
";
            using (SqlConnection connection = new SqlConnection(db.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            statistics.EventCount = (int)reader["EventCount"];
                            statistics.LearnerCount = (int)reader["LearnerCount"];
                            statistics.PackageCount = (int)reader["PackageCount"];
                            statistics.RegistrationCount = (int)reader["RegistrationCount"];
                        }
                    }
                }
            }

            return statistics;
        }

        public PlatformStatistics RetrieveOrganizationStatistics(string organization)
        {
            var statistics = new PlatformStatistics();

            var db = new DatabaseHelper();

            var sql = @"
SELECT (SELECT COUNT(*)
        FROM scoop.ScoEvent AS E
                 INNER JOIN scoop.ScoPackage AS P ON E.PackageNumber = P.PackageNumber AND P.PackageIsActive = 1 AND
                                                     P.OrganizationSlug = @OrganizationSlug)                               AS EventCount,
       (SELECT COUNT(*) AS UniqueCount
        FROM (SELECT DISTINCT R.LearnerId
              FROM scoop.ScoProgress AS R
                       INNER JOIN scoop.ScoPackage AS P
                                  ON R.PackageNumber = P.PackageNumber AND P.PackageIsActive = 1 AND
                                     P.OrganizationSlug =
                                     @OrganizationSlug) AS DistinctPairs)                                                  AS LearnerCount,
       (SELECT COUNT(*)
        FROM scoop.ScoPackage AS P
        WHERE P.PackageIsActive = 1
          AND P.OrganizationSlug = @OrganizationSlug)                                                                      AS PackageCount,
       (SELECT COUNT(*) AS UniqueCount
        FROM (SELECT DISTINCT R.PackageNumber, R.LearnerId
              FROM scoop.ScoProgress AS R
                       INNER JOIN scoop.ScoPackage AS P
                                  ON R.PackageNumber = P.PackageNumber AND P.PackageIsActive = 1 AND
                                     P.OrganizationSlug =
                                     @OrganizationSlug) AS DistinctPairs)                                                  AS RegistrationCount
";
            using (SqlConnection connection = new SqlConnection(db.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@OrganizationSlug", organization);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            statistics.EventCount = (int)reader["EventCount"];
                            statistics.LearnerCount = (int)reader["LearnerCount"];
                            statistics.PackageCount = (int)reader["PackageCount"];
                            statistics.RegistrationCount = (int)reader["RegistrationCount"];
                        }
                    }
                }
            }

            return statistics;
        }
    }
}