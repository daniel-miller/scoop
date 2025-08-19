using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Scoop
{
    public class PackageReader
    {
        public CourseInfo RetrievePackage(string organization, string package)
        {
            var db = new DatabaseHelper();

            using (SqlConnection connection = new SqlConnection(db.ConnectionString))
            {
                connection.Open();

                string query = @"
SELECT p.*
FROM scoop.ScoPackage as p
WHERE p.OrganizationSlug = @OrganizationSlug AND p.PackageSlug = @PackageSlug";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@OrganizationSlug", organization);
                    cmd.Parameters.AddWithValue("@PackageSlug", package);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new CourseInfo
                            {
                                Description = reader["PackageDescription"].ToString(),
                                OrganizationSlug = reader["OrganizationSlug"].ToString(),
                                PackageNumber = Convert.ToInt32(reader["PackageNumber"].ToString()),
                                PackageSizeInKB = (int)reader["PackageSizeInKB"],
                                PackageSlug = reader["PackageSlug"].ToString(),
                                Title = reader["PackageTitle"].ToString(),
                                Version = reader["ScormVersion"].ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }

        public CourseInfo RetrievePackage(int package)
        {
            var db = new DatabaseHelper();

            using (SqlConnection conn = new SqlConnection(db.ConnectionString))
            {
                conn.Open();
                string query = @"
SELECT p.*
FROM scoop.ScoPackage as p
WHERE p.PackageNumber = @PackageNumber";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PackageNumber", package);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new CourseInfo
                            {
                                Description = reader["PackageDescription"].ToString(),
                                OrganizationSlug = reader["OrganizationSlug"].ToString(),
                                PackageNumber = Convert.ToInt32(reader["PackageNumber"].ToString()),
                                PackageSizeInKB = (int)reader["PackageSizeInKB"],
                                PackageSlug = reader["PackageSlug"].ToString(),
                                Title = reader["PackageTitle"].ToString(),
                                Version = reader["ScormVersion"].ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }

        public List<CourseInfo> CollectCourses(string organization, Guid learner)
        {
            var db = new DatabaseHelper();

            var courses = new List<CourseInfo>();

            using (SqlConnection connection = new SqlConnection(db.ConnectionString))
            {
                connection.Open();

                var sql = @"
SELECT
    p.OrganizationSlug,
    p.PackageSizeInKB,
    p.PackageSlug,
    p.PackageNumber,
    p.PackageTitle,
    p.PackageDescription,
    p.ScormVersion,
    ISNULL(d.Status, 'not attempted') as LearnerStatus,
    ISNULL(d.Score, '--') as LearnerScore,
    ISNULL(d.Duration, '00:00:00') as LearnerDuration,
    ISNULL(d.Attempts, 0) as LearnerAttempts
FROM scoop.ScoPackage as p
LEFT JOIN (
    SELECT
        PackageNumber,
        MAX(CASE WHEN ProgressKey = 'cmi.core.lesson_status' THEN ProgressValue END) as Status,
        MAX(CASE WHEN ProgressKey = 'cmi.core.score.raw' THEN ProgressValue END) as Score,
        MAX(CASE WHEN ProgressKey = 'cmi.core.total_time' THEN ProgressValue END) as Duration,
        COUNT(DISTINCT ProgressModified) as Attempts
    FROM scoop.ScoProgress
    WHERE LearnerId = @LearnerId
    GROUP BY PackageNumber
) as d ON p.PackageNumber = d.PackageNumber
WHERE p.OrganizationSlug = @OrganizationSlug AND p.PackageIsActive = 1";

                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@OrganizationSlug", organization);
                    cmd.Parameters.AddWithValue("@LearnerId", learner);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            courses.Add(new CourseInfo
                            {
                                OrganizationSlug = reader["OrganizationSlug"].ToString(),
                                PackageNumber = Convert.ToInt32(reader["PackageNumber"].ToString()),
                                PackageSizeInKB = Convert.ToInt32(reader["PackageSizeInKB"].ToString()),
                                PackageSlug = reader["PackageSlug"].ToString(),
                                Title = reader["PackageTitle"].ToString(),
                                Description = reader["PackageDescription"].ToString(),
                                Version = reader["ScormVersion"].ToString(),
                                Status = reader["LearnerStatus"].ToString(),
                                Score = reader["LearnerScore"].ToString(),
                                Duration = reader["LearnerDuration"].ToString(),
                                Attempts = Convert.ToInt32(reader["LearnerAttempts"])
                            });
                        }
                    }
                }
            }

            return courses;
        }
    }
}